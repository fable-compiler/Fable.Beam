# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Is

Fable.Beam provides F# bindings for Erlang/OTP on the BEAM virtual machine. F# code is transpiled
to Erlang source via Fable's BEAM backend. This is a **bindings library** — it doesn't contain a
compiler; it provides typed F# interfaces to Erlang modules (BIFs, gen_server, ETS, etc.).

## Build Commands

All commands use `just` (command runner). Run `just` to see all available commands.

```bash
just setup          # Install .NET tools (Fable, Paket, Fantomas, ShipIt)
just restore        # Restore NuGet dependencies
just build          # Build F# source
just test           # Full pipeline: F# → Erlang → compile → run on BEAM
just format         # Format with Fantomas
just format-check   # Check formatting
just dev=true test  # Test against local ../fable repo instead of dotnet tool
```

### Test pipeline detail (`just test`)

1. `dotnet build test/` — compile F# to IL
2. `dotnet fable test/ --lang Erlang --outDir build/tests` — transpile to `.erl` (Quill's
   `[<EntryPoint>]` in `Main.fs` becomes `main:main/1`)
3. Copy the helper servers (`test_counter_server.erl`, `test_basic_sup.erl`) + `rebar.config`
   into `build/tests/src/`
4. `cd build/tests && rebar3 compile` — compile Erlang to BEAM bytecode
5. `erl -noshell ...` — run `main:main([])`, the Scriptorium (Quill) runner, which executes the
   registered suites and halts the VM with its exit code (non-zero on failure)

The test project consumes Scriptorium from NuGet (`Scriptorium.Quill` + `Scriptorium.Nib`) via
explicit `PackageReference`s, pinned to the same versions as `../Fable.Actor`. Fable.Core is also
pinned explicitly — an unpinned, paket-injected Fable.Core left `Compiler.isDotnet` undefined when
Fable transpiled Scriptorium's shipped source and failed the BEAM build.

## Writing Tests

Tests live in `test/Test*.fs` and use the Scriptorium test framework: **Nib** for assertions and
the **Quill** runner to execute them. Each file exposes a `tests` value and registers itself in
`Main.fs`:

```fsharp
module Fable.Beam.Tests.Foo

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

let tests =
    testList (
        "Foo",
        [ test ("something works", fun _ ->
                let result = 2 + 2
                assertThat result (isEqualTo 4) ) ]
    )
```

- `test ("desc", fun _ -> ...)` registers one test; `testList` groups them.
- `assertThat actual (expected)` is the Nib assertion; chain with `>>` (e.g. `isGreaterThan 0 >> isEven`).
- No `#if FABLE_COMPILER` needed — Scriptorium runs directly on the BEAM (Fable.Beam's target platform), so write each test body once.
- Quill halts the VM with a non-zero exit code on failure, so a failing test fails `just test`.

To add a new test file: create `test/TestFoo.fs`, expose `let tests = ...`, then add
`<Compile Include="TestFoo.fs" />` to `test/Fable.Beam.Test.fsproj` (order matters — put before
`Main.fs`) and register `Foo.tests` in `Main.fs`.

> **Migration status:** the suite is migrating from the old `[<Fact>]` + Erlang `test_runner.erl`
> discovery to Scriptorium. Only the files that expose a `tests` value are compiled (see the fsproj);
> re-add each remaining file as it migrates.

## Writing Bindings

See `BINDINGS-GUIDE.md` for the full guide. Two core patterns:

- **`[<Emit("erlang:foo($0)")>]`** — inline Erlang code generation (for BIFs, operators)
- **`[<Erase>] + [<ImportAll("module")>]`** — bind an entire Erlang module via interface

Key rules:
- Use concrete F# types (`int`, `string`, `Pid`, `Result<T,E>`) instead of `obj` wherever possible
- F# strings compile to Erlang binaries (`<<"hello">>`), not charlists — use `binary_to_list($0)` when OTP expects charlists
- `case`-containing Emit expressions are auto-wrapped in `(fun() -> ... end)()` by Fable (>= 5.0.0), so manual IIFE wrapping is no longer required (existing wrappers are harmless)
- Suffixed Emit variable names (`FileReadData__`, not `Data`) are still good practice for non-`case` bindings, though the auto-wrap isolates `case`-clause variables
- `[<ImportAll>]` members use tupled args; `[<Emit>]` bindings use curried args

## Architecture

```
src/
  otp/         — Bindings for OTP stdlib modules (Erlang.fs, GenServer.fs, Ets.fs, ...)
  cowboy/      — Bindings for Cowboy HTTP framework (separate NuGet package)
test/          — Test files (Test*.fs) using Scriptorium; helper .erl servers for gen_server/supervisor tests
build/tests/   — Generated: transpiled .erl files, rebar3 project, compiled BEAM
```

Two NuGet packages: `Fable.Beam` (main) and `Fable.Beam.Cowboy` (HTTP server).
Both target `netstandard2.0`. The packages ship `.fsproj` + `.fs` source files so Fable
can transpile them at the consumer's build time.

## Compilation Model

F# source → (Fable transpiler) → Erlang `.erl` files → (erlc/rebar3) → BEAM bytecode.

The `FABLE_COMPILER` define is set in the test project. Use `#if FABLE_COMPILER` for
BEAM-only code paths.

## Conventions

- Binding modules: one file per Erlang module, in `src/otp/` or `src/cowboy/`
- Add `// fsharplint:disable MemberNames` when interface members use snake_case
- Opaque Erlang types: `[<Erase>] type Pid = Pid of obj` (compile-time safety, zero runtime cost)
- Erlang keyword escaping: use double backticks (`` abstract ``match``: ... ``)
- Commits follow Conventional Commits (`feat:`, `fix:`, `chore:`, etc.)
