# Fable.Beam Bindings Guide

How to write F# bindings for Erlang/OTP modules using Fable's BEAM backend.

## Cross-Reference with Erlang Docs

Always verify your bindings against the official Erlang documentation at
<https://www.erlang.org/doc/readme.html>. Each binding module should link to
its corresponding Erlang doc page in a top-level `///` comment (e.g.,
`/// See https://www.erlang.org/doc/apps/erts/erlang`).

When writing or reviewing a binding, check:

- **Parameter types** — match the Erlang typespec (e.g., if the doc says `atom()`, use `Atom` not `obj`)
- **Return types** — match the Erlang typespec (e.g., `{ok, pid()}` → `Result<Pid, string>`)
- **Arity** — ensure all arities of a function are covered or the most common ones are bound
- **Charlist vs binary** — the docs say `string()` for charlists; F# strings are binaries, so convert

## Quick Reference

|           Pattern           |                        When to use                         |          Example           |
| --------------------------- | ---------------------------------------------------------- | -------------------------- |
| `[<Emit>]`                  | BIFs, operators, inline Erlang code                        | `erlang:self()`, `$0 ! $1` |
| `[<Erase>] + [<ImportAll>]` | Binding an Erlang module with multiple functions           | `timer`, `gen_server`      |
| `[<Erase>]` on DU           | Opaque Erlang types (compile-time safety, no runtime cost) | `Pid`, `Ref`, `TableId`    |

## Type Mappings: F# to Erlang

|        F#        |           Erlang            |                    Notes                   |
| ---------------- | --------------------------- | ------------------------------------------ |
| `int`, `float`   | `integer()`, `float()`      | Direct                                     |
| `string`         | `binary()`                  | `<<"hello">>` — **not** charlists          |
| `bool`           | `true` \| `false`           | Atoms                                      |
| `unit`           | `ok`                        | Atom                                       |
| `tuple`          | `tuple`                     | `{A, B, C}` — direct mapping              |
| `list<T>`        | `list()`                    | Both are linked lists                      |
| `option<T>`      | value or `undefined`        | Erased wrapper                             |
| `Result<T,E>`    | `{ok, V}` \| `{error, E}`  | Matches Erlang idiom                       |
| `record`         | `map`                       | `#{field_name => Value}`                   |
| DU (with fields) | tagged tuple                | `{tag, Field1, Field2}`                    |
| DU (no fields)   | atom                        | `tag`                                      |
| `bigint`         | `integer()`                 | Erlang has native arbitrary-precision ints |

## Using Specific Types Instead of `obj`

Erlang is dynamically typed, so it's tempting to use `obj` everywhere. But F# bindings
should be as precise as possible — `obj` defeats the purpose of having typed bindings.
Here's how to choose the right F# type for each Erlang value.

### Primitives: always use the concrete type

These map directly and should never be `obj`:

```fsharp
// GOOD — concrete types
[<Emit("erlang:byte_size($0)")>]
let byteSize (bin: string) : int = nativeOnly

[<Emit("erlang:is_process_alive($0)")>]
let isProcessAlive (pid: Pid) : bool = nativeOnly

[<Emit("timer:sleep($0)")>]
let sleep (ms: int) : unit = nativeOnly
```

```fsharp
// BAD — obj hides the actual types
[<Emit("erlang:byte_size($0)")>]
let byteSize (bin: obj) : obj = nativeOnly
```

### Tuples: use F# tuples

Erlang tuples map directly to F# tuples. Use them for fixed-size structured returns:

```fsharp
/// Returns the current date as {Year, Month, Day}.
[<Emit("erlang:date()")>]
let date () : int * int * int = nativeOnly

/// Returns the current local date and time as {{Year,Month,Day},{Hour,Minute,Second}}.
[<Emit("erlang:localtime()")>]
let localtime () : (int * int * int) * (int * int * int) = nativeOnly

/// Get the peer IP address and port.
[<Emit("cowboy_req:peer($0)")>]
let peer (req: Req) : obj * int = nativeOnly
```

### Lists: use `T list` when the element type is known

When the Erlang function returns a homogeneous list with a known element type, use a
typed list:

```fsharp
// GOOD — we know the element type
abstract supports: ``type``: Atom -> Atom list

// OK — element type is genuinely heterogeneous or unknown
abstract tab2list: table: TableId -> obj array
```

For `[<ImportAll>]` bindings that return Erlang lists, use `obj array` (since Fable
treats raw Erlang lists as arrays in the interface). Use `T list` in `[<Emit>]`
bindings where you control the return type:

```fsharp
/// Lists files in a directory. Converts charlist filenames to binaries.
[<Emit("...")>]
let listDir (path: string) : Result<string list, string> = nativeOnly
```

### Option: use for "value or not found"

Map Erlang's various "not found" sentinels (`undefined`, `false`, `eof`, `error`)
to `option<T>` by returning the atom `undefined` for `None`:

```fsharp
/// Reads a line from standard input. Returns None on EOF.
[<Emit("(fun() -> case io:get_line($0) of eof -> undefined; V__ -> erlang:list_to_binary(V__) end end)()")>]
let getLine (prompt: string) : string option = nativeOnly

/// Gets an environment variable. Returns None if not set.
[<Emit("(fun() -> case os:getenv(binary_to_list($0)) of false -> undefined; V__ -> ... end end)()")>]
let getenv (name: string) : string option = nativeOnly
```

### Result: use for `{ok, V} | {error, Reason}`

This is Erlang's standard error convention. Map it directly to `Result<'T, 'E>`:

```fsharp
/// Reads a file. Returns Ok with contents or Error with reason.
[<Emit("(fun() -> case file:read_file(binary_to_list($0)) of ... end end)()")>]
let readFile (path: string) : Result<string, string> = nativeOnly

/// Writes data to a file.
[<Emit("(fun() -> case file:write_file(binary_to_list($0), $1) of ok -> {ok, ok}; ... end end)()")>]
let writeFile (path: string) (data: string) : Result<unit, string> = nativeOnly
```

Note: for functions that return bare `ok` (not `{ok, Value}`), wrap it as
`{ok, ok}` in the Emit so it maps to `Result<unit, string>`.

### Records: use for structured Erlang maps

When an Erlang function returns a map with known fields, define an F# record:

```fsharp
type HttpResponse =
    { StatusCode: int
      Body: string }

// In Emit, construct the map with snake_case field names:
// #{status_code => StatusCode__0, body => erlang:list_to_binary(Body__0)}
```

F# records compile to Erlang maps with snake_case field names, so this roundtrips
cleanly.

### Opaque types: use `[<Erase>]` DUs to distinguish Erlang references

When Erlang has multiple kinds of opaque handles (pids, refs, table IDs, timer refs),
wrap each in its own erased type so they can't be mixed up:

```fsharp
[<Erase>] type Pid = Pid of obj
[<Erase>] type Ref = Ref of obj
[<Erase>] type TableId = TableId of obj
[<Erase>] type TimerRef = TimerRef of obj
[<Erase>] type SslOptions = SslOptions of obj
```

Then use them in signatures:

```fsharp
// GOOD — can't accidentally pass a Ref where a Pid is expected
[<Emit("erlang:monitor(process, $0)")>]
let monitor (pid: Pid) : Ref = nativeOnly

// BAD — no compile-time safety
[<Emit("erlang:monitor(process, $0)")>]
let monitor (pid: obj) : obj = nativeOnly
```

### Functions: use F# function types for callbacks

When an Erlang function takes a fun/callback, use the appropriate F# function type:

```fsharp
/// Spawn a new process that executes the given function.
[<Emit("erlang:spawn(fun() -> $0(ok) end)")>]
let spawn (f: unit -> unit) : Pid = nativeOnly
```

Note: `unit` compiles to the atom `ok`, so `$0(ok)` calls the F# function with unit.

### When `obj` is acceptable

Use `obj` only when the type is genuinely dynamic or polymorphic in Erlang:

- **Process dictionary** — keys and values can be anything: `put (key: obj) (value: obj)`
- **Message passing** — messages are untyped: `send (pid: Pid) (msg: obj)`
- **Generic containers** — when the Erlang API is polymorphic and you can't express
  the type constraint in F#
- **Raw `ImportAll` interfaces** — where the typed API is provided separately via
  `[<Emit>]` helpers (the raw binding acts as an escape hatch)

### Type choice decision tree

```text
Is the Erlang type...
├── a number?              → int, float, or int64
├── a string/binary?       → string
├── a boolean atom?        → bool
├── the atom 'ok'?         → unit
├── a fixed-size tuple?    → T1 * T2 * ...
├── a list of known type?  → T list (Emit) or T array (ImportAll)
├── {ok, V} | {error, R}? → Result<T, E>
├── value | sentinel?      → T option (return undefined for None)
├── a map with known keys? → F# record
├── an opaque handle?      → [<Erase>] type Foo = Foo of obj
├── a callback fun?        → F# function type
└── genuinely dynamic?     → obj (last resort)
```

## Pattern 1: `[<Emit>]` — Inline Erlang Code

Use `[<Emit>]` for Erlang BIFs, operators, and any expression that should
generate inline Erlang code. Parameters are referenced with `$0`, `$1`, etc.

### Simple BIF binding

```fsharp
open Fable.Core

/// Get the current process's pid.
[<Emit("erlang:self()")>]
let self () : Pid = nativeOnly

/// Create a unique reference.
[<Emit("erlang:make_ref()")>]
let makeRef () : Ref = nativeOnly
```

### BIF with parameters

```fsharp
/// Send a message to a process (Pid ! Msg).
[<Emit("$0 ! $1")>]
let send (pid: Pid) (msg: obj) : unit = nativeOnly

/// Monitor a process. Returns a monitor reference.
[<Emit("erlang:monitor(process, $0)")>]
let monitor (pid: Pid) : Ref = nativeOnly
```

### Returning tuples

```fsharp
/// Returns the current date as {Year, Month, Day}.
[<Emit("erlang:date()")>]
let date () : int * int * int = nativeOnly

/// Returns element at position N (1-based) in a tuple.
[<Emit("erlang:element($0, $1)")>]
let element (n: int) (tuple: obj) : obj = nativeOnly
```

### Returning `Result` from Erlang `{ok, V} | {error, Reason}`

Erlang functions commonly return `{ok, Value}` or `{error, Reason}`. Map
these to F#'s `Result<'T, string>` by pattern matching in the Emit expression:

```fsharp
[<Emit("""
(fun() ->
    case file:read_file(binary_to_list($0)) of
        {ok, FileReadData__} ->
            {ok, FileReadData__};
        {error, FileReadReason__} ->
            {error, erlang:atom_to_binary(FileReadReason__)}
    end
end)()
""")>]
let readFile (path: string) : Result<string, string> = nativeOnly
```

### Returning `Option` from Erlang

When Erlang returns a sentinel value for "not found", convert it to
`option<T>` by returning `undefined` for `None`:

```fsharp
[<Emit("""
(fun() ->
    case os:getenv(binary_to_list($0)) of
        false -> undefined;
        OsGetEnv__ -> erlang:list_to_binary(OsGetEnv__)
    end
end)()
""")>]
let getenv (name: string) : string option = nativeOnly
```

### Emit values (not functions)

`[<Emit>]` also works for constant values:

```fsharp
/// Disable certificate verification (for development only).
[<Emit("[{ssl, [{verify, verify_none}]}]")>]
let verifyNone: SslOptions = nativeOnly
```

## Pattern 2: `[<Erase>] + [<ImportAll>]` — Module Bindings

Use this pattern to bind an entire Erlang module. Define an interface with
`[<Erase>]` describing the module's functions, then bind it with
`[<ImportAll("module_name")>]`.

### Basic module binding

```fsharp
open Fable.Core

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Suspends the process for Time milliseconds.
    abstract sleep: time: int -> unit
    /// Converts hours to milliseconds.
    abstract hours: hours: int -> int
    /// Converts minutes to milliseconds.
    abstract minutes: minutes: int -> int
    /// Converts seconds to milliseconds.
    abstract seconds: seconds: int -> int

/// timer module
[<ImportAll("timer")>]
let timer: IExports = nativeOnly
```

Usage:

```fsharp
timer.sleep 1000
let ms = timer.seconds 30  // 30000
```

### Module with overloaded functions

Erlang functions with different arities map to overloaded abstract members:

```fsharp
[<Erase>]
type IExports =
    /// Makes a synchronous call to a gen_server.
    abstract call: serverRef: obj * request: obj -> obj
    /// Makes a synchronous call with timeout.
    abstract call: serverRef: obj * request: obj * timeout: int -> obj
    /// Log an error message.
    abstract error: msg: string -> unit
    /// Log an error message with metadata.
    abstract error: msg: string * metadata: obj -> unit
```

### Escaping F# keywords

Use double-backtick notation for Erlang function names that are F# keywords:

```fsharp
[<Erase>]
type IExports =
    /// Matches objects in the table against a pattern.
    abstract ``match``: table: TableId * pattern: obj -> obj array
    /// Creates a new ETS table. (Erlang: ets:new/2)
    abstract new_: name: Atom * options: obj list -> TableId
    /// Check list membership. (Erlang: lists:member/2)
    abstract ``member``: elem: obj * list: obj -> bool
```

### Combining both patterns

A module binding can be paired with typed `[<Emit>]` helpers for a
better F# API:

```fsharp
/// Raw module binding (returns obj, caller must handle Erlang tuples)
[<Erase>]
type IExports =
    abstract read_file: filename: string -> obj
    abstract write_file: filename: string * data: obj -> obj

[<ImportAll("file")>]
let file: IExports = nativeOnly

/// Typed helper with charlist conversion and Result return
[<Emit("""
(fun() ->
    case file:read_file(binary_to_list($0)) of
        {ok, FileReadData__} -> {ok, FileReadData__};
        {error, FileReadReason__} -> {error, erlang:atom_to_binary(FileReadReason__)}
    end
end)()
""")>]
let readFile (path: string) : Result<string, string> = nativeOnly
```

## Defining Opaque Types

Wrap Erlang opaque types in `[<Erase>]` single-case discriminated unions.
This gives you compile-time type safety with zero runtime overhead:

```fsharp
/// Erlang process identifier.
[<Erase>]
type Pid = Pid of obj

/// Erlang reference (from make_ref, monitor, etc.).
[<Erase>]
type Ref = Ref of obj

/// Erlang atom.
[<Erase>]
type Atom = Atom of obj

/// ETS table identifier.
[<Erase>]
type TableId = TableId of obj
```

For truly opaque types where you don't need to wrap/unwrap, use a
simple type alias:

```fsharp
/// Cowboy request object (opaque).
type Req = obj
```

## String and Atom Conversions

F# strings compile to Erlang binaries (`<<"hello">>`), **not** charlists.
Many OTP functions expect charlists, so you need to convert:

|       Direction       |                      Emit code                      |
| --------------------- | ---------------------------------------------------- |
| F# string → charlist  | `binary_to_list($0)`                                 |
| charlist → F# string  | `erlang:list_to_binary(...)`                         |
| F# string → atom      | `binary_to_atom($0)` or `erlang:binary_to_atom($0)` |
| atom → F# string      | `erlang:atom_to_binary($0)`                          |

Example — an Erlang function that takes a charlist path and returns a
charlist result:

```fsharp
[<Emit("""
(fun() ->
    case file:get_cwd() of
        {ok, FileGetCwdDir__} ->
            {ok, erlang:list_to_binary(FileGetCwdDir__)};
        {error, FileGetCwdReason__} ->
            {error, erlang:atom_to_binary(FileGetCwdReason__)}
    end
end)()
""")>]
let getCwd () : Result<string, string> = nativeOnly
```

## IIFE Wrapping for Variable Scoping

Wrap complex Emit expressions in `(fun() -> ... end)()` (an Immediately
Invoked Function Expression). This prevents Erlang "unsafe variable"
errors when multiple Emit calls appear in the same generated function:

```fsharp
// BAD — may cause "unsafe variable" errors in generated Erlang
[<Emit("case file:read_file($0) of {ok, Data} -> {ok, Data}; {error, R} -> {error, R} end")>]

// GOOD — scoped in IIFE
[<Emit("(fun() -> case file:read_file($0) of {ok, Data} -> {ok, Data}; {error, R} -> {error, R} end end)()")>]
```

## Variable Naming in Emit

Use descriptive, suffixed variable names in Emit expressions to avoid
collisions with other generated Erlang variables. The convention is
`ModuleDescription__` or `Description__N`:

```fsharp
// BAD — generic names like Data, Reason risk collisions
[<Emit("case file:read_file($0) of {ok, Data} -> ... end")>]

// GOOD — descriptive suffixed names
[<Emit("case file:read_file($0) of {ok, FileReadData__} -> ... end")>]

// GOOD — numbered suffix for multiple bindings in one module
[<Emit("""
(fun() ->
    Url__0 = binary_to_list($0),
    Headers__0 = [{binary_to_list(K__0), binary_to_list(V__0)} || {K__0, V__0} <- $1],
    case httpc:request(get, {Url__0, Headers__0}, $2, []) of
        {ok, {{_, StatusCode__0, _}, _RespHeaders__0, Body__0}} ->
            {ok, #{status_code => StatusCode__0, body => erlang:list_to_binary(Body__0)}};
        {error, Reason__0} ->
            {error, erlang:list_to_binary(io_lib:format(<<"~p">>, [Reason__0]))}
    end
end)()
""")>]
let get (url: string) (headers: (string * string) list) (ssl: SslOptions) : Result<HttpResponse, string> = nativeOnly
```

## Defining Record Types for Structured Returns

Use F# records to provide typed access to Erlang map results:

```fsharp
type HttpResponse =
    { StatusCode: int
      Body: string }
```

Then construct them in Emit using Erlang map syntax (`#{field => value}`),
since F# records compile to Erlang maps:

```fsharp
#{status_code => StatusCode__0, body => erlang:list_to_binary(Body__0)}
```

Note: record field names are converted to snake_case in the generated
Erlang.

## Interop Gotchas

### Erlang lists vs F# arrays

F# arrays on BEAM are ref-wrapped values in the process dictionary. Raw
Erlang lists returned from OTP calls (e.g., `ets:tab2list/1`,
`maps:keys/1`) are **not** ref-wrapped. This means F# `.Length` will
not work on them. Use `Erlang.length` instead:

```fsharp
let all = ets.tab2list table
// Don't: all.Length (fails at runtime)
// Do:
let count = Erlang.length (box all)
```

### ImportAll functions use tupled arguments

`[<ImportAll>]` interface members use **tupled** arguments (comma-separated),
not curried:

```fsharp
// Interface definition — tupled
abstract put: key: obj * value: obj * map: obj -> obj

// Usage — tupled call
let m = maps.put (box "key", box "value", m)
```

`[<Emit>]` bindings use **curried** arguments:

```fsharp
// Emit definition — curried
[<Emit("erlang:monitor(process, $0)")>]
let monitor (pid: Pid) : Ref = nativeOnly

// Usage — curried call
let ref = monitor child
```

### Module-level Emit functions and cross-module calls

All public API functions that use Emit should be direct Emit bindings, not
wrappers around private Emit functions. Fable compiles cross-module
non-Emit calls as Erlang module calls (e.g., `httpc:get/3`), which won't
exist in the target Erlang module.

## Module File Template

```fsharp
/// Type bindings for Erlang <module_name> module
/// See https://www.erlang.org/doc/apps/<app>/<module_name>
module Fable.Beam.ModuleName

open Fable.Core
open Fable.Beam.Erlang  // if you need Pid, Ref, Atom, etc.

// fsharplint:disable MemberNames

// ============================================================================
// Opaque types (if any)
// ============================================================================

[<Erase>]
type MyHandle = MyHandle of obj

// ============================================================================
// Raw module binding
// ============================================================================

[<Erase>]
type IExports =
    /// Does something.
    abstract do_something: arg: obj -> obj

[<ImportAll("module_name")>]
let moduleName: IExports = nativeOnly

// ============================================================================
// Typed API (if raw binding needs conversion)
// ============================================================================

[<Emit("""
(fun() ->
    case module_name:do_something($0) of
        {ok, DoSomethingResult__} -> {ok, DoSomethingResult__};
        {error, DoSomethingReason__} -> {error, erlang:atom_to_binary(DoSomethingReason__)}
    end
end)()
""")>]
let doSomething (arg: string) : Result<string, string> = nativeOnly
```

## Complete Example: Binding a New OTP Module

Here's how you'd bind `crypto` (a module not yet in Fable.Beam):

```fsharp
/// Type bindings for Erlang crypto module
/// See https://www.erlang.org/doc/apps/crypto/crypto
module Fable.Beam.Crypto

open Fable.Core
open Fable.Beam.Erlang

// fsharplint:disable MemberNames

// ============================================================================
// Raw module binding
// ============================================================================

[<Erase>]
type IExports =
    /// Computes a message digest (hash).
    abstract hash: ``type``: Atom * data: obj -> obj
    /// Generates N random bytes.
    abstract strong_rand_bytes: n: int -> obj
    /// Lists supported hash algorithms.
    abstract supports: ``type``: Atom -> obj list

[<ImportAll("crypto")>]
let crypto: IExports = nativeOnly

// ============================================================================
// Typed helpers
// ============================================================================

/// Compute a SHA-256 hash of a binary string.
[<Emit("crypto:hash(sha256, $0)")>]
let sha256 (data: string) : obj = nativeOnly

/// Generate N cryptographically strong random bytes.
[<Emit("crypto:strong_rand_bytes($0)")>]
let randomBytes (n: int) : obj = nativeOnly
```

Usage:

```fsharp
open Fable.Beam.Crypto

let hash = sha256 "hello world"
let bytes = randomBytes 16
```
