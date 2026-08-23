# RFC: Reduce the per-module API surface count

Status: **in progress** — Option A is the v5.0 public-API direction. `String.fs` is the completed
prototype; `Base64.fs`, `Binary.fs`, `Maps.fs`, and `Proplists.fs` have been migrated.
Prompted by review of #128 (the dual-API / `BeamChardata` work).

## The concern

A consumer of a single binding module today meets up to **three** distinct API surfaces. For
`Fable.Beam.String`:

| Surface | Style | Examples |
| --- | --- | --- |
| `str.*` (ImportAll interface) | tupled, member access | `str.lowercase "x"`, `str.trim ("  x  ", dir)`, `str.slice ("x", 0, 3)` |
| module helpers | curried, module function | `reverse "x"`, `pad "x" 5`, `find "x" "y"`, `splitAll "a,b" ","` |
| `*Raw` variants | curried, module function | `padRaw "x" 5`, `splitAllRaw "a,b" ","` |

Many stdlib modules have this shape: `maps.get`/`maps.put` (interface) alongside `tryFind`/`ofList`/
`keysRaw` (module); likewise `lists`, `binary`, and `proplists`. Some newer modules, notably `re`,
already use the intended curried shape.

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

## Prior art — Fable.Python

The sibling project `../Fable.Python` (F# → Python via Fable) faced the same choice and **already lives
in Option B's end state**, uniformly across all 18 stdlib modules. It is a working existence proof that a
single-surface collapse is stable in production.

- **One surface per module.** Every module is exactly one `[<Erase>] type IExports` + one
  `[<ImportAll("mod")>] let mod: IExports = nativeOnly`. A consumer meets one object (`math.sqrt(x)`,
  `sys.exit(1)`). See `Fable.Python/src/stdlib/Math.fs`, `Sys.fs`, `String.fs`.
- **The split never happens because the wrapper stays on the member.** When a binding needs custom
  emit (the exact thing that forks Fable.Beam), Fable.Python decorates the *abstract member* rather
  than promoting a module function — e.g. `Sys.fs:66` `[<Emit("$0.exit(int($1))")>] abstract exit: status: int -> 'a`
  sits right next to the plain `abstract exit: unit -> 'a`.
- **Convention is tupled member access**, arity handled by *overloads* (`Math.fs` `log/1` vs `log/2`),
  never currying.
- **No `*Raw` pattern anywhere** — Python return types map cleanly to F#, so there's no BEAM-native
  escape hatch to mirror. The only curried module-level `[<Emit>]` `let`s in the whole tree are in the
  third-party Flask binding, not in stdlib.

Caveat for us: Fable.Python can afford Option B's "wrapped member sits next to a plain one with no cue"
because Python strings map cleanly to F# strings — it never had the chardata/`Result`-looks-like-a-string
trap that motivated #128. On BEAM that con is real, which is why the recommendation below is still Option A
(curried) rather than simply copying Fable.Python.

## Options

**A. Unify onto curried module functions (recommended).** Drop the `xxx` ImportAll interfaces (or
demote them to an internal/raw escape hatch), and expose every friendly operation as a curried
module-level `[<Emit>]` function. Result: one friendly surface + `*Raw` where a BEAM-native form
exists.
- Pro: curried is F#-idiomatic and pipe-friendly (`s |> trim |> reverse`); matches where the typed
  helpers already live; one mental model.
- Con: overloaded arities lose overloading and become distinct names (`slice`/`sliceLen`,
  `trim`/`trimStart`/`trimEnd`, `equal`/`equalCaseInsensitive`, `padStart`/`padEnd`) rather than
  preserving the Erlang arities. More `[<Emit>]` lines than interface members.

**B. Unify onto the ImportAll interface (tupled).** Put every binding on `str.*` etc., using
`[<Emit>]` on members for the wrapped ones.
- Pro: keeps arity overloading; one binding block per module.
- Con: tupled args don't pipe; less idiomatic F#; a `BeamChardata`/`Result`-returning member sits
  visually next to `string`-returning ones with no cue about the different return — the exact
  "looks like it returns a string" trap that motivated the chardata fixes.

**C. Do nothing; document the split.** Add a short "why two surfaces" note to `BINDINGS-GUIDE.md` so
the pattern is at least predictable. Cheapest; leaves the arbitrariness in place.

## Decision

Adopt **Option A** for the v5.0 release: every public, F#-friendly binding is a curried module
function. Keep a `*Raw` variant only where the Erlang return has a distinct, useful native shape.
Do not retain a public per-module `ImportAll` object as a general escape hatch: it recreates the
second surface, has weak typing, and makes the public API harder to learn. An `ImportAll` binding may
remain `internal` where it is useful to implement a typed wrapper.

This prioritises one predictable, pipe-friendly F# convention over preserving OTP's overload-like
arities. Public names use the base name for the lowest arity and add the semantic extra argument for
additional arities: `slice`/`sliceLen`; finite atom modes become semantic operations such as
`trim`/`trimStart`/`trimEnd` and `padStart`/`padEnd`, rather than public direction parameters.
Avoid generic numeric suffixes and boolean mode parameters: expose a named operation such as
`equalCaseInsensitive` rather than `equal ... true`.

The decision is library-wide, not String-specific. `String.fs` remains the implementation prototype
because it exercises direct calls, return conversion, overload removal, and `*Raw` pairs in one small
module. Its purpose is to validate code generation and the naming rules, not to reopen the direction.

Suggested sequence:
1. Convert and test `String.fs`; use it to verify the emitted Erlang and update the naming examples.
2. Convert the remaining public modules module by module, as separate commits in **one atomic PR**.
   Keep `*Raw` variants unchanged.
3. Migrate this repository's tests, examples, documentation, and Synapse in the same release window;
   do not ship a stable Fable.Beam whose documented downstream consumer no longer compiles.
4. Update `BINDINGS-GUIDE.md` (the "Quick Reference", the `[<ImportAll>]`-vs-`[<Emit>]` guidance, and
   the module-file template) to make curried module functions the house style. Treat `ImportAll` as an
   implementation technique, not a public API convention.

## Impact / cautions

- **Breaking, repo-wide.** Every `str.pad`/`maps.get` call site flips tupled → curried. Pre-1.0
  (`-rc.x`), so breaking is acceptable, but it touches this repo, the `Fable.Beam.Cowboy` package, and
  the downstream `synapse` app. Grep synapse's call sites before committing to it.
- **Naming churn** from lost overloads (see Option A cons). Agree the naming scheme up front
  (`sliceLen`, `trimStart`, `trimEnd`, `equalCaseInsensitive`, …) so it's consistent across modules.
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
trimStart "  x  "                     // trim/2 with leading -> distinct name
reverse "hello"                       // unchanged
padEnd "hi" 5                         // string:pad/2's trailing default
// raw escape hatch follows the friendly name: reverseRaw, padEndRaw, splitAllRaw, ...
```

## A vs B — side by side (real `String.fs` bindings)

### Today (the wart): the split is visible at the call site

Same *kind* of operation, two surfaces, two conventions — decided only by whether the Erlang return
needs a wrapper. A caller can't predict which side `lowercase` vs `reverse` lands on.

```fsharp
str.lowercase "HELLO"              // interface, tupled  — no wrapper needed
str.slice ("hello world", 0, 5)    // interface, tupled  — overloaded arity
str.trim ("  x  ", leading)        // interface, tupled  — overloaded arity
str.equal ("a", "A", true)         // interface, tupled  — overloaded arity
reverse "hello"                    // module,    curried — needs flatten wrapper
pad "hi" 5                         // module,    curried — needs flatten wrapper
find "lo" "hello"                  // module,    curried — returns option
splitAll "a,b,c" ","               // module,    curried — returns array
toInteger "42rest"                 // module,    curried — returns Result
```

### Option A — collapse to curried module functions (recommended)

Overloaded arities lose overloading and get distinct names, including named directions for finite atom modes.

```fsharp
// binding — no public `str` interface
[<Emit("string:lowercase($0)")>]        let lowercase (s: string) : string = nativeOnly
[<Emit("string:slice($0, $1)")>]        let slice (s: string) (start: int) : string = nativeOnly            // slice/2
[<Emit("string:slice($0, $1, $2)")>]    let sliceLen (s: string) (start: int) (length: int) : string = nativeOnly  // slice/3 → distinct name
[<Emit("string:trim($0)")>]             let trim (s: string) : string = nativeOnly                          // trim/1
[<Emit("string:trim($0, leading)")>]     let trimStart (s: string) : string = nativeOnly                      // trim/2, named mode
[<Emit("string:trim($0, trailing)")>]    let trimEnd (s: string) : string = nativeOnly                        // trim/2, named mode
[<Emit("string:equal($0, $1)")>]        let equal (s1: string) (s2: string) : bool = nativeOnly
[<Emit("string:equal($0, $1, true)")>]  let equalCaseInsensitive (s1: string) (s2: string) : bool = nativeOnly
// reverse, find, splitAll, toInteger — already have this shape

// call sites — one convention, pipe-friendly
lowercase "HELLO"
sliceLen "hello world" 0 5
trimStart "  x  "
equalCaseInsensitive "a" "A"
"  Hello World  " |> trim |> lowercase |> reverse
```

Trade: more names to learn (`sliceLen`, `trimStart`, `trimEnd`, `equalCaseInsensitive`) and more `[<Emit>]` lines;
gains one pipeable mental model and kills the arbitrary split.

### Option B — collapse to the tupled interface (Fable.Python's choice)

Wrapped bindings stay on the interface via `[<Emit>]`-on-member (the `Sys.fs:66` technique). Overloads
are kept.

```fsharp
[<Erase>]
type IExports =
    abstract lowercase: s: string -> string
    abstract slice: s: string * start: int -> string
    abstract slice: s: string * start: int * length: int -> string   // overload kept
    abstract trim: s: string -> string
    abstract trim: s: string * dir: Atom -> string                   // overload kept
    abstract equal: s1: string * s2: string -> bool
    abstract equal: s1: string * s2: string * ignoreCase: bool -> bool
    // wrapped ones stay ON the interface, so they don't fork off:
    [<Emit("unicode:characters_to_binary(string:reverse($1))")>]
    abstract reverse: s: string -> string
    [<Emit("unicode:characters_to_binary(string:pad($1, $2))")>]
    abstract pad: s: string * length: int -> string
    [<Emit("(fun() -> case string:to_integer($1) of {error, E__} -> {error, atom_to_binary(E__)}; {V__, R__} -> {ok, {V__, R__}} end end)()")>]
    abstract toInteger: s: string -> Result<int * string, string>

[<ImportAll("string")>]
let str: IExports = nativeOnly

// call sites — one convention, tupled member access (no piping)
str.lowercase "HELLO"
str.slice ("hello world", 0, 5)    // overload survives
str.reverse "hello"                // now on str.* too
str.toInteger "42rest"             // returns Result — but reads like it returns a string
```

Trade: keeps overloads and one binding block; loses piping, and a `Result`/chardata-returning member
sits visually next to `string`-returning ones with no cue — the exact trap #128 fought. Fable.Python
tolerates this because Python strings map cleanly; BEAM does not.

### The one fact that makes either collapse possible

`[<Emit>]` can decorate an interface member *or* be a module `let` (this doc, "Why it is this way").
So the wrapper can live on either surface — the current split is arbitrary, not forced. A puts every
wrapper on a curried `let`; B puts every wrapper on an interface member.

## Settled rollout choices

1. No public raw `ImportAll` hatch; expose only named, typed bindings and intentional `*Raw` pairs.
2. Use semantic suffixes for de-overloaded arities; expose finite atom and boolean modes as named
   operations rather than public mode parameters (`padStart`/`padEnd`, with matching `*Raw` names).
3. Use one atomic PR, structured as reviewable per-module commits.
4. Migrate Synapse in the same release window, before the v5.0 stable release.

## Rollout checklist

- [x] Decide on the curried public API and semantic naming rules.
- [x] Convert `src/otp/String.fs` and `test/TestString.fs` as the prototype.
- [x] Verify the prototype's generated Erlang and BEAM behavior.
- [x] Inventory the remaining public `ImportAll` surfaces and their downstream call sites.
- [ ] Convert the remaining modules in reviewable commits while preserving intentional `*Raw` pairs.
  - [x] `Base64.fs`, `Binary.fs`, `Maps.fs`, and `Proplists.fs`.
  - [ ] `Lists.fs` and the remaining public `ImportAll` modules.
  - [x] `Math.fs`, `Queue.fs`, and `Rand.fs`.
- [ ] Update the bindings guide's general conventions, template, examples, and README call sites.
- [ ] Migrate Synapse during the same release window and verify its build.
- [ ] Publish the v5.0 release only after the repository and downstream migration are complete.
