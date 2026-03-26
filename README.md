# Fable.Beam

F# bindings for Erlang/OTP on the BEAM virtual machine,
powered by [Fable](https://fable.io/).

Write idiomatic F# and compile to Erlang using Fable's
BEAM backend. This package provides typed bindings for
Erlang/OTP standard modules so you can call them directly
from F#.

## Packages

| Package | Description |
| --- | --- |
| `Fable.Beam` | Core Erlang/OTP bindings |
| `Fable.Beam.Cowboy` | Cowboy HTTP server bindings |
| `Fable.Beam.Jsx` | jsx JSON library bindings |

### Fable.Beam — OTP Modules

| Module | Binding | Description |
| --- | --- | --- |
| `Fable.Beam.Erlang` | `erlang` | BIFs: processes, send/receive, monitors |
| `Fable.Beam.GenServer` | `gen_server` | Generic server behaviour |
| `Fable.Beam.Supervisor` | `supervisor` | Supervisor behaviour |
| `Fable.Beam.Application` | `application` | OTP application management |
| `Fable.Beam.Timer` | `timer` | Timer functions, sleep, conversions |
| `Fable.Beam.Ets` | `ets` | Erlang Term Storage |
| `Fable.Beam.Maps` | `maps` | Erlang map operations |
| `Fable.Beam.Lists` | `lists` | Erlang list operations |
| `Fable.Beam.Io` | `io` | I/O functions |
| `Fable.Beam.Logger` | `logger` | OTP logger |
| `Fable.Beam.File` | `file` | File system operations |
| `Fable.Beam.Os` | `os` | OS interaction, env vars, system time |
| `Fable.Beam.Httpc` | `httpc` | HTTP client (inets) |
| `Fable.Beam.Init` | `init` | Runtime system control |
| `Fable.Beam.Testing` | - | Test helpers (Fact, assertions) |

### Fable.Beam.Cowboy

| Module | Binding | Description |
| --- | --- | --- |
| `Fable.Beam.Cowboy.Cowboy` | `cowboy` | Listener start/stop |
| `Fable.Beam.Cowboy.CowboyReq` | `cowboy_req` | Request/response handling |
| `Fable.Beam.Cowboy.CowboyRouter` | `cowboy_router` | Route compilation |
| `Fable.Beam.Cowboy.CowboyHandler` | `cowboy_handler` | Handler callbacks |
| `Fable.Beam.Cowboy.CowboyWebsocket` | `cowboy_websocket` | WebSocket support |

### Fable.Beam.Jsx

| Module | Binding | Description |
| --- | --- | --- |
| `Fable.Beam.Jsx.Jsx` | `jsx` | JSON encode, decode, format, validate |

## Usage

Add the NuGet packages to your project:

```text
paket add Fable.Beam
paket add Fable.Beam.Cowboy   # optional: HTTP server
paket add Fable.Beam.Jsx      # optional: JSON
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

// Typed Erlang maps (generic — no box needed)
let m: BeamMap<string, int> = maps.new_ ()
let m = maps.put ("key", 42, m)
let v = maps.get ("key", m)  // returns int

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

### JSON with jsx

```fsharp
open Fable.Beam.Jsx.Jsx

let json = jsx.encode {| name = "world" |}
let valid = jsx.is_json (json, [strict])
let mini = jsx.minify """{ "key" : "value" }"""
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
  otp/             # Fable.Beam — OTP stdlib bindings
    Erlang.fs, GenServer.fs, Supervisor.fs, Timer.fs,
    Ets.fs, Maps.fs, Lists.fs, Io.fs, Logger.fs,
    File.fs, Os.fs, Httpc.fs, Application.fs, Init.fs,
    Testing.fs
  cowboy/          # Fable.Beam.Cowboy — HTTP server bindings
    Cowboy.fs, CowboyReq.fs, CowboyRouter.fs,
    CowboyHandler.fs, CowboyWebsocket.fs
  jsx/             # Fable.Beam.Jsx — JSON library bindings
    Jsx.fs
test/
  Test*.fs           # Test files
  test_runner.erl    # BEAM test runner
  rebar.config       # Erlang test dependencies
```

## Binding Patterns

The bindings use two Fable interop patterns:

**`[<Emit>]`** for Erlang BIFs and operators
(direct Erlang code generation):

```fsharp
[<Emit("erlang:self()")>]
let self () : Pid = nativeOnly

[<Emit("$0 ! $1")>]
let send (pid: Pid) (msg: obj) : unit = nativeOnly
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
F# arrays as ref-wrapped values (via `fable_utils:new_ref`).
Raw Erlang lists returned from some OTP calls (e.g.,
`ets:tab2list/1`) are *not* ref-wrapped, so F#
`Array.length` will not work on them directly. Bindings
that return F# arrays (e.g., `maps.keys`, `maps.to_list`)
wrap the result automatically so standard array operations
work.

**Atoms from strings:** Fable compiles F# strings to
Erlang binaries (`<<"hello">>`), not charlists. Use
`binaryToAtom`/`atomToBinary` rather than
`listToAtom`/`atomToList` when converting between F#
strings and atoms.

## License

MIT
