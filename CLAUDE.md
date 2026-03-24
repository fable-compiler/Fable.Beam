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
just test-dotnet    # Verify F# compiles (no BEAM needed)
just format         # Format with Fantomas
just format-check   # Check formatting
just dev=true test  # Test against local ../fable repo instead of dotnet tool
```

### Test pipeline detail (`just test`)

1. `dotnet build test/` — compile F# to IL
2. `dotnet fable test/ --lang Erlang --outDir build/tests` — transpile to `.erl`
3. Copy `test/test_runner.erl` into `build/tests/src/`
4. `cd build/tests && rebar3 compile` — compile Erlang to BEAM bytecode
5. `erl -noshell ...` — run test_runner which discovers and executes all `test_*` functions

## Writing Tests

Tests live in `test/Test*.fs`. Each test function is marked `[<Fact>]` and uses `equal` for assertions:

```fsharp
open Fable.Beam.Testing

[<Fact>]
let ``test something works`` () =
    let result = 2 + 2
    result |> equal 4
```

The test runner discovers functions prefixed with `test_` in modules prefixed with `test_`.
F# test names like `` ``test something works`` `` compile to `test_something_works` in Erlang.

To add a new test file: create `test/TestFoo.fs` and add `<Compile Include="TestFoo.fs" />`
to `test/Fable.Beam.Test.fsproj` (order matters — put before `Main.fs`).

## Writing Bindings

See `BINDINGS-GUIDE.md` for the full guide. Two core patterns:

- **`[<Emit("erlang:foo($0)")>]`** — inline Erlang code generation (for BIFs, operators)
- **`[<Erase>] + [<ImportAll("module")>]`** — bind an entire Erlang module via interface

Key rules:
- Use concrete F# types (`int`, `string`, `Pid`, `Result<T,E>`) instead of `obj` wherever possible
- F# strings compile to Erlang binaries (`<<"hello">>`), not charlists — use `binary_to_list($0)` when OTP expects charlists
- Wrap complex Emit expressions in `(fun() -> ... end)()` to prevent Erlang "unsafe variable" errors
- Use suffixed variable names in Emit (`FileReadData__`, not `Data`) to avoid Erlang variable collisions
- `[<ImportAll>]` members use tupled args; `[<Emit>]` bindings use curried args

## Architecture

```
src/
  otp/         — Bindings for OTP stdlib modules (Erlang.fs, GenServer.fs, Ets.fs, ...)
  cowboy/      — Bindings for Cowboy HTTP framework (separate NuGet package)
test/          — Test files (Test*.fs) + test_runner.erl
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
