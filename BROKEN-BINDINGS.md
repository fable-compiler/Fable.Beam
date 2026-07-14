# 14 broken bindings, previously hidden by a no-op test suite

## How these surfaced

Until the Fable 5.8.1 upgrade, **`equal` never raised**, so no assertion in this repo could fail.
`Fable.Beam.Testing.equal` delegates to `Fable.Core.Testing.Assert.AreEqual`, and Fable's BEAM
backend lowered that to a bare *equality expression* whose boolean result was discarded:

```erlang
equal(Expected, Actual) ->
    fable_comparison:equals(Actual, Expected).   %% returns false; never raises
```

```
1> testing:equal(1, 2).
false
```

The suite reported **367/367 passing** while only ever catching outright crashes — never a wrong
value. Fixed upstream in `../Fable` (see `BEAM-ASSERT-AND-ERASED-REFLECTION-PROMPT.md` there);
with real assertions the same suite reports **353 passed, 14 failed**.

None of them are Fable bugs.

**Status: all 14 are fixed — the suite is 367 passed, 0 failed, and this time the green is real.**

The split turned out to be instructive: **8 were broken bindings, 6 were broken tests.** Once
assertions could actually fail, the tests got checked for the first time too — and a third of them
were asserting the wrong thing. A test that can't fail is not a weaker test; it's not a test.

### How could these ever have "worked"?

They didn't — nothing verified them. But they *looked* fine for a second reason worth remembering:
an iolist is still valid Erlang chardata. Hand `string:pad`'s result straight to `io:format`,
`file:write` or a Cowboy body and OTP flattens it for you, producing exactly the bytes you expected.
It only bites when the value is **compared, pattern-matched, or stored as a binary** — which is
exactly what a test does, and exactly what the F# type claims you're holding.

## The bugs

Most are the same root cause, and it's the one `CLAUDE.md` already warns about: **F# strings are
Erlang binaries**, but several OTP functions return an *iolist* or a *charlist*. The bindings type
them as `string` and hand the raw result straight back, so the value is structurally wrong even
though it prints plausibly.

| # | Test | Expected | Actual | Cause |
|---|---|---|---|---|
| 1 | `string.replace_all` | `<<"XXbbXX">>` | `[<<>>,<<"XX">>,<<"bb">>,<<"XX">>,<<>>]` | `string:replace/4` returns an iolist |
| 2 | `string.replace_first` | `<<"XXbbaa">>` | `[<<>>,<<"XX">>,<<"bbaa">>]` | same |
| 3 | `str.pad_leading_with_direction` | `<<"   hi">>` | `["   ",<<"hi">>]` | `string:pad/4` returns an iolist |
| 4 | `str.pad_trailing_to_length` | `<<"hi   ">>` | `[<<"hi">>,32,32,32]` | same |
| 5 | `str.pad_with_custom_character` | `<<"007">>` | `[[<<"0">>,<<"0">>],<<"7">>]` | same |
| 6 | `str.reverse` | `<<"olleh">>` | `"olleh"` (charlist) | `string:reverse/1` returns a charlist |
| 7 | `to_graphemes` | `<<"a">>` | `97` | `string:to_graphemes/1` returns codepoints, not binaries |
| 8 | `uri_string.compose_query` empty list | `<<>>` | `[]` | empty iolist not normalised |
| 9 | `binary.longest_common_prefix` | `4` | `3` | **bad test** — 3 is correct |
| 10 | `erlang.send_and_receive` | `0` | `1` | **bad test** — sent the wrong message shape |
| 11–14 | `jsx.labels_*` (atom, attempt_atom, binary, existing_atom) | `true` | `false` | **bad tests** — `is_json` can't observe `labels` |

### Fix for 1–8 (done)

Every result is flattened at the binding boundary with `unicode:characters_to_binary/1` — chosen over
`iolist_to_binary/1` because it also handles charlists and codepoints above 255, which
`string:reverse` and a non-ASCII `pad` character produce:

```fsharp
[<Emit("unicode:characters_to_binary(string:replace($0, $1, $2, all))")>]
let replaceAll (s: string) (pattern: string) (replacement: string) : string = nativeOnly
```

`toGraphemes` (#7) needed each *cluster* converted, not just the outer list, since
`string:to_graphemes` yields codepoints (`97`) or codepoint lists:

```fsharp
[<Emit("fable_utils:new_ref([unicode:characters_to_binary([StringGrapheme__]) || StringGrapheme__ <- string:to_graphemes($0)])")>]
```

**API change:** `reverse` and the three `pad` arities could not be fixed in place. They were
`abstract` members on the `[<ImportAll("string")>]` interface, which emits `string:pad(S, N)`
directly — there is nowhere to wrap the result. They are now module-level `Emit` bindings, so the
call sites move and the overloads become distinct names (module-level F# functions can't overload):

| Was | Now |
|---|---|
| `str.reverse s` | `reverse s` |
| `str.pad (s, n)` | `pad s n` |
| `str.pad (s, n, dir)` | `padDir s n dir` |
| `str.pad (s, n, dir, ch)` | `padWith s n dir ch` |

The old interface members are left in `String.fs` as commented-out stubs explaining why.

### Fix for 9–14 (done) — every one of these was a bad *test*, not a bad binding

Reproducing each by hand in `erl` first was the right call: all six bindings were correct.

- **#9 `longest_common_prefix`.** `binary:longest_common_prefix([<<"foobar">>,<<"foobaz">>,<<"fooqux">>])`
  really is `3` — `"foo"` is the prefix common to *all three*; only the first two share `"fooba"`.
  The test asserted 4. Fixed the expectation.

- **#10 `send_and_receive`.** A nullary DU case compiles to a **bare atom** (`ping`); only a case
  *with* fields becomes a tagged tuple (`Data 42` → `{data, 42}`). The test sent the 1-tuple
  `{ping}`, which never matched the generated receive clause — so it silently sat out the full
  1000 ms timeout and took the `None` branch. Now sends `ping`. (This one was also costing a
  wall-clock second per run.)

- **#11–14 `jsx labels`.** `labels` is a **decoder** option. `jsx:is_json/2` does not accept it and
  simply answers `false`:
  ```erlang
  1> jsx:is_json(<<"{\"key\":\"value\"}">>, [{labels, atom}]).
  false
  2> jsx:is_json(<<"{\"key\":\"value\"}">>, []).
  true
  ```
  So `is_json` could never demonstrate that the option took effect — the tests' own premise
  ("is_json is enough to prove the option was accepted") was false. Tellingly, the one test in the
  group that used `decode` and inspected the key type *passed all along*. The four now decode and
  assert on the resulting keys, and `ExistingAtom` asserts the badarg rejection that gives it its
  name.

The `labels` binding itself was fine throughout: Fable emits the raw atom `{labels, atom}`, not
`{labels, <<"atom">>}` — which is exactly what the surviving probe test was written to catch.

## Why this happened, and what stops it recurring

Two independent failure modes conspired, and *both* reported success:

1. Assertions could not fail (fixed upstream in Fable).
2. Fable 5.8.1 renamed generated modules (`test_maps` → `fable_beam_test_test_maps`), which broke
   `test_runner.erl`'s `lists:prefix("test_", ...)` discovery. The runner then found **zero** tests
   and printed *"All tests passed!"*.

`test/test_runner.erl` now discovers tests by **exports** (any module exporting `test_*/0`) rather
than by module name, and **refuses to report success on an empty suite**. That empty-suite guard is
the cheap insurance: a discovery bug can now never again look like a green build.

The longer-term answer is the Scriptorium spike (`spike/scriptorium`, `just spike`): Nib's assertions
raise from F# itself and don't depend on the backend lowering `Assert.AreEqual`, and Quill registers
tests explicitly instead of rediscovering them by naming convention — so neither failure mode is
expressible.
