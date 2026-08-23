# Fable.Beam — Working Notes

Live working document for the **v5.0.0 API freeze**. Update it as things land; it is meant to be
edited, not archived.

Consolidated 2026-08-01 from `OBJ-AUDIT.md`, `TYPING-FIXES.md` and `FABLE-IMPROVEMENTS.md`, which
were deleted in favour of this file. `API-SURFACES.md` remains separate — it is a standing RFC with
its own open decision, referenced below.

## Where things stand

- **Versions:** the three packages version *independently*, from their own changelogs — `Fable.Beam`
  **5.0.0-rc.33**, `Fable.Beam.Cowboy` **5.0.0-rc.24**, `Fable.Beam.Jsx` **5.0.0-rc.8**. Freezing at
  v5.0.0 locks every public signature under semver, and the three can freeze on different dates.
  Read the version from the `## <version>` heading at the top of `src/otp/CHANGELOG.md`,
  `src/cowboy/CHANGELOG.md` and `src/jsx/CHANGELOG.md` — that is what `just pack` uses.
  **Do not read it from `git tag`:** the newest repo tag is `v5.0.0-rc.15` from 2026-03-26, left
  behind when ShipIt moved to per-project versioning. No tag corresponds to a current package version.
- **Toolchain:** Fable 5.13.0 (`.config/dotnet-tools.json`). Plain `just test` is green — the old
  `dev=true` workaround is obsolete, the fixes it waited on shipped.
- **Suite:** 380 BEAM tests green on `main`; 400 on the `fix/atom-construction-and-callback-types`
  branch (PR #132). Note `just test` does not clean `build/tests/` — a renamed or deleted test module
  leaves a stale `.erl` behind and the runner still counts it. `rm -rf build/tests` when a count
  looks wrong.
- **`obj` in `src/`:** 107 lines. That number is *not* the goal — see "Settled decisions".

## Outstanding before v5.0.0

Ordered by what would hurt most to get wrong, since semver locks these.

### 1. Implement the API-surface decision — blocking

`API-SURFACES.md` adopts one public curried surface per module, with intentional `*Raw` escape hatches
only. Collapsing the arbitrary interface/module split is breaking, so it happens before the freeze or
not at all. Implement it as one atomic PR with reviewable per-module commits, and migrate Synapse in
the same release window. Everything else on this list is small by comparison.

### 2. `obj` → `Dynamic` renames

Pure renames of genuinely-dynamic terms. No semantic change; makes the signature honest.

- [ ] `src/otp/Erlang.fs:39` — `processFlag (flag: Atom) (value: obj) : obj`
- [ ] `src/otp/Erlang.fs:113` — `phash2 (term: obj)`
- [ ] `src/otp/Erlang.fs:190` — `tupleSize (tuple: obj)`
- [ ] `src/otp/Erlang.fs:198` — `element (n: int) (tuple: obj) : obj`
- [ ] `src/otp/Erlang.fs:257` — `formatTerm (term: obj)`
- [ ] `src/otp/Queue.fs:22` — `is_queue (term: obj)`
- [ ] `src/otp/Ets.fs:58` — `select (matchSpec: obj) : obj array` (match specs are genuinely dynamic)
- [ ] `src/otp/Maps.fs:58` — `find : … -> obj`; mark `[<Obsolete>]`, `tryFind` already exists

### 3. Category-5 stragglers — decide, then either type or document

Raw-only bindings with no typed alternative. Each is a conscious call: build the typed layer, or
declare it an honest escape hatch and say so in the doc comment.

- [ ] `src/cowboy/CowboyWebsocket.fs:46` — `upgradeWithOpts (opts: obj)`. Deliberately deferred:
      wants a *combinable* WS-opts builder (marker-interface pattern) rather than a too-narrow
      record frozen too early.
- [ ] `src/otp/Io.fs:49` — `setopts (opts: obj)` → marker-interface option list
- [ ] `src/otp/Rand.fs:22` — `seed : RandAlg -> obj` → opaque `RandState`
- [ ] `src/otp/Logger.fs:57,63,85` — `update_handler_config` / `add_primary_filter` /
      `set_handler_config` value params

### 4. Typed layers designed but never built

Carried over from `TYPING-FIXES.md`; these exist only as guidance in `BINDINGS-GUIDE.md`.

- [ ] **Marker-interface option lists.** The `IEtsOption` pattern is documented but implemented
      nowhere in `src/`. It is the intended answer for `ets:new/2` options, `Io.setopts` and the WS
      opts above — so building it once unblocks three items.
- [ ] **Records for structured maps.** `TableInfo` (`ets:info/1`), `LogMetadata` (`logger`) and
      `AppInfo` (`application:which_applications/0`) do not exist. `Peer`, `TransportOpts`,
      `ProtocolOpts` and `HttpResponse` do.

### 5. Category-6 heterogeneous lists — ship as-is, write down why

`Proplists.fs` (×10), `Logger.fs` metadata/args (×8), `Io.fs`/`Timer.fs` format/apply args (×5).
These mirror BEAM's own untyped semantics, so freezing them is low-risk. The work is a paragraph in
`BINDINGS-GUIDE.md` explaining the rationale, not a retype. Optionally rename the element type to
`Dynamic` for signalling.

### 6. `test/TestTypeSafety.fs`

Never created. Intent: cases that *would* have compiled against the old `obj` API and must not
compile against the tightened one (e.g. a `string` where `Pid<int>` is expected), kept as
`// Uncomment to confirm it fails to compile` markers — F# has no `[<CompileError>]`.

## Settled decisions — do not re-litigate

- **Counting `obj` is the wrong metric.** The original plan targeted "132 → under 20". A raw grep
  conflates opaque-type internals (`type Pid<'Msg> = Pid of obj` — the intended end state),
  deliberate escape hatches, and genuine leaks. Classify, don't count. The number moves *up* when a
  leaked param becomes a properly opaque type.
- **Plain nullary DUs, not `[<StringEnum>]`, for discrete atom sets.** DU cases already compile to
  atom literals with `[<CompiledName>]` honoured (`Rand.fs`, `Ets.fs`). StringEnum emitted binaries
  on BEAM; fixed upstream (below), but plain DUs remain the recommendation and cost nothing at runtime.
- **`System.Func` is never required for callbacks.** An F# function value compiles to an Erlang fun
  of its *remaining* arity, at any arity, including through `ImportAll` members and partial
  application. Pinned by `test/TestCallbacks.fs`. (PR #132)
- **`Atom`'s constructor is private.** The type is erased, so `Atom "name"` produced the binary
  `<<"name"/utf8>>` and silently failed to match atom-keyed terms. Use `Atom.ofString`. (PR #132)
- **Manual IIFE wrapping of `case` Emits is unnecessary.** Fable auto-wraps; existing wrappers are
  harmless.
- **Raw `IExports` under a typed API is fine** — `File` and `Application` mark theirs `internal`,
  which removes them from the public surface without retyping anything.

## Upstream Fable dependencies

Re-verified 2026-08-01 against Fable 5.13.0 by transpiling probe modules and reading the generated
`.erl`. Most of the original list was already fixed.

**Live:**

- [ ] **`[<StringEnum>]` → atoms.** Merged upstream as #4867; ships in **5.14.0**, pending release
      PR fable-compiler/Fable#4853. Write-up: `../Fable/BEAM-STRINGENUM-ATOMS-PROMPT.md`.
      *Action:* bump `.config/dotnet-tools.json` when it publishes and confirm. Not a freeze blocker —
      the bindings use plain DUs.
- [ ] **Arrays returned from BIFs still round-trip through a process-dict ref.** `maps:keys`,
      `values`, `to_list` wrap results in `fable_utils:new_ref`. Never filed upstream.
      (The *input* half — array literals passed to an FFI BIF — is fixed.)
- [ ] **Diagnostics for a missing `op_ErasedCast`.** F# reports the type error with no hint to use
      `!^`. Tooling polish; never filed.

**Closed:** `!^` added to `BeamInterop` (#4659, in 5.10.0+) · curried Emit `$N` misplacement with
function-valued params · "unsafe variable" in `case`-containing Emits (auto-wrapped) ·
ignored/let-bound result shadowing Emit vars (never reproduced) · array *literals* lowering to a
process-dict ref.

## Reference: how the `obj` surface classifies

Kept because it is the reasoning behind "counting is the wrong metric".

| Category | What it is | Action |
| --- | --- | --- |
| 1. Opaque type definitions | `[<Erase>] type X = X of obj` — callers never see `obj` | none; the intended end state |
| 2. Comments | not code | ignore |
| 3. Raw hatch under a typed API | `File`/`Application` `IExports` | mark `internal` — **done** |
| 4. Genuinely dynamic terms | any BEAM term in/out | rename to `Dynamic` — **outstanding (§2)** |
| 5. Untyped public surface, no alternative | the real pre-freeze work | **mostly done**; stragglers in §3 |
| 6. Heterogeneous arg/option lists | proplists, format args | ship + document — **§5** |

**Done in Category 5:** Cowboy (`readBody` — which was *wrong*, not merely untyped — `peer`,
`parseQs`, router, listener config), GenServer (`From`, typed start errors), Supervisor (full
`ChildSpec` rewrite, `SupRef`, Result-returning children ops).

## Field evidence: synapse

`../synapse` is a real downstream consumer and independently corroborated the Cowboy findings — it
carried inline `[<Emit>]` workarounds for exactly the signatures flagged as Category 5, including a
reimplementation of `CowboyReq.readBody` whose comment noted the library version returned a
`byte array` that "may not map correctly on BEAM". Those are fixed; re-grepping synapse's inline
Emits is a cheap way to find the next round of gaps.

## History

- **2026-08-01** — Consolidated three planning docs into this file. `Atom` constructor privatised,
  `System.Func` dropped from `Lists`/`Maps`/`Queue`/`Decode`, `test/TestCallbacks.fs` added (PR #132).
  StringEnum→atoms filed and merged upstream. Fable improvement list re-verified: 4 of 7 already fixed.
- Earlier — Tier-A typing work landed (#103, #108): Cowboy, GenServer and Supervisor typed;
  `Dynamic` + `Decode` introduced; phantom parameters on opaque handles; `BINDINGS-GUIDE.md` rewritten.
