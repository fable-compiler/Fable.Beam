# Fable.Beam

[![Build and Test](https://github.com/fable-compiler/Fable.Beam/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/fable-compiler/Fable.Beam/actions/workflows/build-and-test.yml)
[![Nuget](https://img.shields.io/nuget/vpre/Fable.Beam)](https://www.nuget.org/packages/Fable.Beam/)

F# bindings for Erlang/OTP on the BEAM virtual machine,
powered by [Fable](https://fable.io/).

Write idiomatic F# and compile to Erlang using Fable's
BEAM backend. This package provides typed bindings for
Erlang/OTP standard modules so you can call them directly
from F#.

## Ecosystem

Libraries built on top of Fable.Beam:

| Library | Description |
| --- | --- |
| [Fable.Actor](https://github.com/fable-hub/Fable.Actor) | F# actor model for Fable and the BEAM |
| [Fable.Logging](https://github.com/fable-hub/Fable.Logging) | Logging framework for Fable |
| [Fable.TypedJson](https://github.com/dbrattli/Fable.TypedJson) | Pydantic-flavored JSON validation and serialization for F# records |

## Packages

| Package | Description |
| --- | --- |
| `Fable.Beam` | Core Erlang/OTP bindings |
| `Fable.Beam.Cowboy` | Cowboy HTTP server bindings |
| `Fable.Beam.Jsx` | jsx JSON library bindings |

`Fable.Beam.Jsx` depends on `Fable.Beam`. During the 5.0 release-candidate
series, install the paired versions: `Fable.Beam 5.0.0-rc.37` and
`Fable.Beam.Jsx 5.0.0-rc.9`. The JSX package requires `Fable.Beam >=
5.0.0-rc.37` and `< 6.0.0`, so NuGet reports an incompatible selection at
restore time.

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
| `Fable.Beam.Binary` | `binary` | Binary data operations |
| `Fable.Beam.Math` | `math` | Mathematical functions |
| `Fable.Beam.Proplists` | `proplists` | Property list operations |
| `Fable.Beam.Queue` | `queue` | Functional FIFO queue |
| `Fable.Beam.Base64` | `base64` | Base64 encoding and decoding |
| `Fable.Beam.Rand` | `rand` | Pseudo-random number generation |
| `Fable.Beam.Re` | `re` | Regular expressions (PCRE-compatible) |
| `Fable.Beam.Calendar` | `calendar` | Date, time, and Gregorian conversions |
| `Fable.Beam.UriString` | `uri_string` | URI parsing, normalization, and encoding |
| `Fable.Beam.String` | `string` | Unicode string operations |
| `Fable.Beam.Io` | `io` | I/O functions |
| `Fable.Beam.Logger` | `logger` | OTP logger |
| `Fable.Beam.File` | `file` | File system operations |
| `Fable.Beam.Os` | `os` | OS interaction, env vars, system time |
| `Fable.Beam.Port` | `erlang:open_port` | Configurable external-process ports, lifecycle, and monitoring |
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
open Fable.Core.BeamInterop

open Fable.Beam.Erlang

module BeamMaps = Fable.Beam.Maps
module BeamTimer = Fable.Beam.Timer

// Process management
let pid = self ()
let ref = makeRef ()
let child = spawn (fun () ->
    BeamTimer.sleep 1000
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
let map: BeamMap<string, int> = BeamMaps.empty ()
let map = BeamMaps.put "key" 42 map
let value = BeamMaps.get "key" map  // returns int

// Timers
BeamTimer.sleep 100
let ms = BeamTimer.seconds 30  // 30000

// Process monitoring
let monRef = monitor child
demonitorFlush monRef

// Process dictionary
put (box "my_key") (box 42) |> ignore
let value = get (box "my_key")
```

For modules that collide with FSharp.Core names, use a short explicit alias in
examples and application code: `BeamMaps`, `BeamLists`, `BeamMath`, and
`BString` for `Fable.Beam.String`. All public binding functions are curried;
the evolving collection or value is the final argument, so they compose with
pipelines. See [the curried API migration guide](MIGRATING-TO-CURRIED-API.md)
for renamed and raw-value APIs.

### JSON with jsx

```fsharp
open Fable.Beam.Jsx.Jsx

let json = encode {| name = "world" |}
let valid = isJsonWith json [ strict ]
let mini = minify """{ "key" : "value" }"""
```

### Ports (external processes)

Open an external OS process as a typed port. Output is read from the
process's standard output as newline-delimited lines, and its exit status is
reported by default. `PortOptions` keeps the launch configuration explicit;
arguments are passed as an argument vector — never interpolated into a shell
command.

```fsharp
open Fable.Beam.Port

let options =
    { PortOptions.defaultOptions with
        arguments = []
        maxLineLength = 1024 }

// Resolve the executable on PATH and start it with explicit options.
match startOnPath "cat" options with
| Ok port ->
    match trySend port "hello\n" with
    | Ok () ->
        receiveUntil port 1000
        |> List.iter (function
            | Line line -> printfn "%s" line
            | IncompleteLine fragment -> printfn "partial: %s" fragment
            | ExitStatus code -> printfn "exited %d" code)
    | Error reason -> printfn "send failed: %s" reason

    match tryClose port with
    | Ok () -> ()
    | Error reason -> printfn "close failed: %s" reason
| Error reason -> printfn "failed to start: %s" reason
```

`receive` is a *selective* receive: it only consumes messages from this port
and leaves unrelated mailbox messages in place, so it composes with
`Fable.Actor` and other process protocols. Call it from the process that
opened the port, since ERTS delivers port messages to the opening process.
For line-mode consumption, prefer `receiveUntil` or `foldMessages`: they
preserve a trailing `IncompleteLine` when ERTS delivers `ExitStatus` first.
Options also support a working directory, environment overrides, stderr
redirection, owner linking, and independent `monitor`/`receiveDown` exit
notifications.

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
    File.fs, Os.fs, Port.fs, Httpc.fs, Application.fs, Init.fs,
    Binary.fs, Math.fs, Proplists.fs, String.fs, Queue.fs,
    Base64.fs, Rand.fs, Testing.fs
  cowboy/          # Fable.Beam.Cowboy — HTTP server bindings
    Cowboy.fs, CowboyReq.fs, CowboyRouter.fs,
    CowboyHandler.fs, CowboyWebsocket.fs
  jsx/             # Fable.Beam.Jsx — JSON library bindings
    Jsx.fs
test/
  Test*.fs           # Test files
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
