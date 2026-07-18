# RFC: Reduce the per-module API surface count

Status: **proposed** — not started. Written as a hand-off for a follow-up session.
Prompted by review of #128 (the dual-API / `BeamChardata` work).

## The concern

A consumer of a single binding module today meets up to **three** distinct API surfaces. For
`Fable.Beam.String`:

| Surface | Style | Examples |
| --- | --- | --- |
| `str.*` (ImportAll interface) | tupled, member access | `str.lowercase "x"`, `str.trim ("  x  ", dir)`, `str.slice ("x", 0, 3)` |
| module helpers | curried, module function | `reverse "x"`, `pad "x" 5`, `find "x" "y"`, `splitAll "a,b" ","` |
| `*Raw` variants | curried, module function | `padRaw "x" 5`, `splitAllRaw "a,b" ","` |

Every stdlib module has this shape: `maps.get`/`maps.put` (interface) alongside `tryFind`/`ofList`/
`keysRaw` (module); likewise `lists`, `binary`, `re`, `proplists`.

## What is and isn't actually wrong

- **The `*Raw` layer is fine.** It is opt-in, self-documenting, and clearly derived from its friendly
  sibling — a deliberate BEAM-native escape hatch (see the "Dual API" section of
  `BINDINGS-GUIDE.md`). Nobody reaches for `padRaw` by accident. This RFC does **not** propose
  changing it.
- **The wart is the `xxx.*`-interface vs. module-helper split**, and it predates the dual-API work.
  The boundary is drawn by an *implementation detail*, not by anything a caller can see: if the raw
  Erlang return maps straight to an F# type it becomes an ImportAll member (`str.lowercase` — binary
  → string, no wrapper); if it needs an Emit wrapper — a `Result`, a `characters_to_binary` flatten,
  a `new_ref` array wrap — it becomes a module `[<Emit>]` function (`reverse`, `find`, `splitAll`).
  So `str.lowercase` and `reverse` are the same *kind* of thing to a caller, yet live on different
  surfaces with different call conventions. That arbitrariness is the problem.

So this is really a **two-surface** problem (arbitrary interface/module split) plus one justified
escape hatch — not three equally-arbitrary surfaces.

## Why it is this way (history)

`[<ImportAll>]` is a cheap way to bind many functions of a module at once, but ImportAll codegen
emits a bare `module:function(args)` call — it cannot insert a wrapper. So any binding needing a
non-trivial return (Result, option, flatten, ref-wrap) *had* to be a module-level `[<Emit>]`
function. The interface caught the rest. Nothing chose the split deliberately; it fell out of the two
mechanisms.

Note: `[<Emit>]` **can** decorate an interface member (it overrides ImportAll codegen for that one
method — see `maps.keys`, `BINDINGS-GUIDE.md` "Erlang lists vs F# arrays"). So the split is not
forced by the tooling — either surface *can* host any binding. Which means we can collapse it.

## Options

**A. Unify onto curried module functions (recommended).** Drop the `xxx` ImportAll interfaces (or
demote them to an internal/raw escape hatch), and expose every friendly operation as a curried
module-level `[<Emit>]` function. Result: one friendly surface + `*Raw` where a BEAM-native form
exists.
- Pro: curried is F#-idiomatic and pipe-friendly (`s |> trim |> reverse`); matches where the typed
  helpers already live; one mental model.
- Con: overloaded arities lose overloading and become distinct names (`slice`/`sliceLen`,
  `trim`/`trimDir`, `equal`/`equalCaseInsensitive`) — the same trade `pad`/`padDir`/`padWith`
  already made. More `[<Emit>]` lines than interface members.

**B. Unify onto the ImportAll interface (tupled).** Put every binding on `str.*` etc., using
`[<Emit>]` on members for the wrapped ones.
- Pro: keeps arity overloading; one binding block per module.
- Con: tupled args don't pipe; less idiomatic F#; a `BeamChardata`/`Result`-returning member sits
  visually next to `string`-returning ones with no cue about the different return — the exact
  "looks like it returns a string" trap that motivated the chardata fixes.

**C. Do nothing; document the split.** Add a short "why two surfaces" note to `BINDINGS-GUIDE.md` so
the pattern is at least predictable. Cheapest; leaves the arbitrariness in place.

## Recommendation

**Option A**, done deliberately and repo-wide, but **phased and prototyped first**. It gives the
cleanest end state — "friendly curried functions + `*Raw` escape hatch" — and aligns the whole
library on one call convention.

Suggested sequence:
1. Prototype on `String.fs` only. Produce a before/after and measure the call-site churn (tests,
   `spike/`, Cowboy, synapse). Decide go/no-go from real numbers, not this doc.
2. If go: convert module by module, each its own commit. Keep `*Raw` variants unchanged.
3. Update `BINDINGS-GUIDE.md` (the "Quick Reference", the `[<ImportAll>]`-vs-`[<Emit>]` guidance, and
   the module-file template) to make curried module functions the house style, and mark ImportAll as
   the raw/escape-hatch mechanism only.

## Impact / cautions

- **Breaking, repo-wide.** Every `str.pad`/`maps.get` call site flips tupled → curried. Pre-1.0
  (`-rc.x`), so breaking is acceptable, but it touches this repo, the `Fable.Beam.Cowboy` package, and
  the downstream `synapse` app. Grep synapse's call sites before committing to it.
- **Naming churn** from lost overloads (see Option A cons). Agree the naming scheme up front
  (`sliceLen`, `trimDir`, `equalCaseInsensitive`, …) so it's consistent across modules.
- **Not a rider on any feature PR.** This is its own initiative with its own review.

## Concrete sketch — `String.fs`, before → after (Option A)

```fsharp
// before
str.lowercase "HELLO"                 // interface, tupled
str.slice ("hello world", 0, 5)       // interface, overloaded arity
str.trim ("  x  ", leading)           // interface, overloaded arity
reverse "hello"                       // module, curried
pad "hi" 5                            // module, curried

// after — one curried surface
lowercase "HELLO"
slice "hello world" 0                 // slice/2
sliceLen "hello world" 0 5            // slice/3 -> distinct name
trim "  x  "                          // trim/1
trimDir "  x  " leading               // trim/2 -> distinct name
reverse "hello"                       // unchanged
pad "hi" 5                            // unchanged
// raw escape hatch unchanged: reverseRaw, padRaw, splitAllRaw, ...
```

## Open questions for the follow-up session

1. Do we keep a raw ImportAll interface per module (e.g. `str`) as a documented escape hatch for
   arbitrary OTP calls, or drop it entirely?
2. Naming scheme for de-overloaded arities — settle it once, apply everywhere.
3. Phasing: all modules in one PR, or a stack of per-module PRs (easier review, longer breakage
   window on `main`)?
4. Is the churn to `synapse` acceptable, and should we land a synapse-side migration in the same
   change or right after?

## Starting point

Begin at `src/otp/String.fs` (smallest self-contained case with all three surfaces) and its test
`test/TestString.fs`. The `pad`/`padDir`/`padWith` split already models the de-overloading approach.
