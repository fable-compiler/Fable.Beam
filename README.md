# Fable.Beam

F# bindings for Erlang/OTP on the BEAM virtual machine,
powered by [Fable](https://fable.io/).

Write idiomatic F# and compile to Erlang using Fable's
BEAM backend. This package provides typed bindings for
Erlang/OTP standard modules so you can call them directly
from F#.

## Available Modules

| Module | Binding | Description |
| --- | --- | --- |
| `Fable.Beam.Erlang` | `erlang` | BIFs: processes, send/receive, monitors |
| `Fable.Beam.GenServer` | `gen_server` | Generic server behaviour |
| `Fable.Beam.Supervisor` | `supervisor` | Supervisor behaviour |
| `Fable.Beam.Timer` | `timer` | Timer functions, sleep, conversions |
| `Fable.Beam.Ets` | `ets` | Erlang Term Storage |
| `Fable.Beam.Maps` | `maps` | Erlang map operations |
| `Fable.Beam.Lists` | `lists` | Erlang list operations |
| `Fable.Beam.Io` | `io` | I/O functions |
| `Fable.Beam.Logger` | `logger` | OTP logger |
| `Fable.Beam.File` | `file` | File system operations |
| `Fable.Beam.Testing` | - | Test helpers (Fact, assertions) |

## Usage

Add the NuGet package to your project:

```text
paket add Fable.Beam
```

Then use the bindings in your F# code:

```fsharp
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Erlang
open Fable.Beam.Timer
open Fable.Beam.Maps

// Process management
let pid = self ()
let ref = makeRef ()
let child = spawn (fun () ->
    timer.sleep 1000
)

// Send and receive messages
// Erlang.receive is from Fable.Core.BeamInterop
type Msg =
    | [<CompiledName("hello")>] Hello of name: string
    | [<CompiledName("stop")>] Stop

send pid (box "a message")

match Erlang.receive<Msg> 5000 with
| Some (Hello name) -> printfn "Hello %s" name
| Some Stop -> exit (box "normal")
| None -> printfn "Timeout"

// Erlang maps
let m = maps.new_ ()
let m = maps.put (box "key", box "value", m)
let v = maps.get (box "key", m)

// Timers
timer.sleep 100
let ms = timer.seconds 30  // 30000

// Process monitoring
let monRef = monitor child
demonitorFlush monRef

// Process dictionary
put (box "my_key") (box 42) |> ignore
let value = get (box "my_key")
```

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/) 10+
- [Erlang/OTP](https://www.erlang.org/)
- [rebar3](https://rebar3.org/)
- [just](https://github.com/casey/just) (command runner)

## Getting Started

```bash
# Install .NET tools (Fable, Paket, Fantomas)
just setup

# Install dependencies
just restore

# Build
just build

# Run tests on BEAM
just test
```

## Development

```bash
# Show all available commands
just

# Build and run tests on BEAM
just test

# Verify F# compiles (without BEAM)
just test-dotnet

# Format code
just format

# Use local Fable repo for development
just dev=true test

# Create NuGet package
just pack
```

## Project Structure

```text
src/
  otp/
    Erlang.fs        # Erlang BIFs (Emit-based bindings)
    GenServer.fs     # gen_server module (ImportAll binding)
    Supervisor.fs    # supervisor module
    Timer.fs         # timer module
    Ets.fs           # ets module
    Maps.fs          # maps module
    Lists.fs         # lists module
    Io.fs            # io module
    Logger.fs        # logger module
    File.fs          # file module
    Testing.fs       # Test utilities
test/
  TestErlang.fs      # Erlang BIF tests
  TestEts.fs         # ETS tests
  TestMaps.fs        # Maps tests
  ...
  test_runner.erl    # BEAM test runner
```

## Binding Patterns

The bindings use two Fable interop patterns:

**`[<Emit>]`** for Erlang BIFs and operators
(direct Erlang code generation):

```fsharp
[<Emit("erlang:self()")>]
let self () : obj = nativeOnly

[<Emit("$0 ! $1")>]
let send (pid: obj) (msg: obj) : unit = nativeOnly
```

**`[<Erase>]` + `[<ImportAll>]`** for Erlang module
bindings:

```fsharp
[<Erase>]
type IExports =
    abstract sleep: time: int -> unit
    abstract hours: hours: int -> int

[<ImportAll("timer")>]
let timer: IExports = nativeOnly
```

## Interop Notes

**Erlang lists vs F# arrays:** Fable on BEAM represents
F# arrays as ref-wrapped values in the process dictionary.
Raw Erlang lists returned from OTP calls (e.g.,
`ets:tab2list/1`, `maps:keys/1`) are *not* ref-wrapped,
so F# `.Length` will not work on them. Use
`Erlang.length` instead:

```fsharp
open Fable.Beam.Erlang
open Fable.Beam.Ets

let table =
    ets.new_ (
        binaryToAtom "my_table",
        [ box (binaryToAtom "set") ]
    )
let all = ets.tab2list table

// Don't use: all.Length (will fail at runtime)
// Use instead:
let count = Erlang.length (box all)
```

Similarly, use `Erlang.element` and `Erlang.tupleSize`
for raw Erlang tuples, and `Erlang.byteSize` for binaries.

**Atoms from strings:** Fable compiles F# strings to
Erlang binaries (`<<"hello">>`), not charlists. Use
`binaryToAtom`/`atomToBinary` rather than
`listToAtom`/`atomToList` when converting between F#
strings and atoms.

## License

MIT
