# Fable.Beam Bindings Guide

How to write F# bindings for Erlang/OTP modules using Fable's BEAM backend.

## Cross-Reference with Erlang Docs

Always verify your bindings against the official Erlang documentation at
<https://www.erlang.org/doc/readme.html>. Each binding module should link to
its corresponding Erlang doc page in a top-level `///` comment (e.g.,
`/// See https://www.erlang.org/doc/apps/erts/erlang`).

When writing or reviewing a binding, check:

- **No bare `obj`** — see "Core Rule: Avoid `obj`" below. Use generics, phantom types, DUs, records, or `Dynamic` instead.
- **Phantom type parameters** on opaque handles (`Pid<'Msg>`, `Ref<'Tag>`, `TableId<'K, 'R>`)
- **Parameter types** — match the Erlang typespec (e.g., if the doc says `atom()`, use `Atom` not `obj`)
- **Return types** — match the Erlang typespec (e.g., `{ok, pid()}` → `Result<Pid<'Msg>, string>`)
- **Arity** — ensure all arities of a function are covered or the most common ones are bound
- **Charlist vs binary** — the docs say `string()` for charlists; F# strings are binaries, so convert
- **Discrete atom sets** — model as plain DUs with no fields (add `[<CompiledName>]` for snake_case atoms)
- **Callbacks** — use `System.Func<>` / `System.Action<>` (not raw F# function types) to avoid Emit positional bugs

## Quick Reference

| Pattern | When to use | Example |
| --- | --- | --- |
| `[<Emit>]` | BIFs, operators, inline Erlang code | `erlang:self()`, `$0 ! $1` |
| `[<Erase>] + [<ImportAll>]` | Binding an Erlang module with multiple functions | `timer`, `gen_server` |
| `[<Erase>]` on DU | Opaque Erlang types (compile-time safety, no runtime cost) | `Pid`, `Ref`, `TableId` |
| `[<Erase>]` on generic DU | Typed Erlang containers (maps, lists) | `BeamMap<'K,'V>`, `BeamList<'T>` |
| `[<Emit>]` on abstract member | Override `ImportAll` codegen for specific methods | `fable_utils:new_ref(...)` wrapping |
| `System.Func<>` / `System.Action` | Typed callbacks in `ImportAll` interfaces | `fold`, `filter`, `foreach` |
| `U2<>` / `U3<>` / erased union | Parameters or returns that accept multiple types | timeout: `int` or `infinity` |
| Regular DU (no fields) | Discrete atom sets (table types, log levels) | `type EtsTableType = Set \| Bag \| ...` |
| `Dynamic` + `Decode` combinators | Genuinely unknown Erlang terms | `binaryToTerm`, `application:get_env` |

## Core Rule: Avoid `obj`

`obj` is the maximum-entropy type — it admits any value, so using it in a binding signature
defeats the purpose of typed bindings. When adding new bindings, treat `obj` as a last resort.
Reach for the most specific type that fits, in this order:

1. **A concrete F# type** — `int`, `string`, `bool`, `Atom`, `unit`
2. **A generic type parameter** — `'Args`, `'Msg`, `'Key`, `'Value` when callers pick the type
3. **A phantom-typed opaque handle** — `Pid<'Msg>`, `Ref<'Tag>`, `TableId<'K, 'R>`, `ServerRef<'Call, 'Cast>`
4. **A plain F# DU** for discrete atom sets — compiles directly to atoms (see "DU cases compile to atoms")
5. **A record** for structured Erlang maps — compiles to `#{field => value}` with atom keys
6. **An erased DU `[<Erase>] type X = X of obj`** for opaque tagged wrappers (e.g., `WsFrame`, `ServerName`)
7. **`U2<A, B>` / `U3<A, B, C>`** for parameters or returns that accept multiple known types
8. **`Dynamic` + `Decode` combinators** for values whose shape is not known until runtime

**Never** write `type X = obj` — a bare alias silently admits anything. Use `[<Erase>] type X = X of obj`
instead; the erased DU has zero runtime cost but prevents accidental mixing with other untyped terms.

The only narrow places `obj` is acceptable are listed in the "When `obj` is acceptable" section below.

## Type Mappings: F# to Erlang

|        F#        |           Erlang            |                    Notes                   |
| ---------------- | --------------------------- | ------------------------------------------ |
| `int`, `float`   | `integer()`, `float()`      | Direct                                     |
| `string`         | `binary()`                  | `<<"hello">>` — **not** charlists          |
| `bool`           | `true` \| `false`           | Atoms                                      |
| `unit`           | `ok`                        | Atom                                       |
| `tuple`          | `tuple`                     | `{A, B, C}` — direct mapping               |
| `list<T>`        | `list()`                    | Both are linked lists                      |
| `option<T>`      | value or `undefined`        | Erased wrapper                             |
| `Result<T,E>`    | `{ok, V}` \| `{error, E}`   | Matches Erlang idiom                       |
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

### Opaque types: use `[<Erase>]` DUs with phantom type parameters

When Erlang has multiple kinds of opaque handles (pids, refs, table IDs, timer refs),
wrap each in its own erased type so they can't be mixed up. **Always add a phantom
type parameter** to capture additional information the F# type system can reason about:

```fsharp
/// Phantom 'Msg captures the message type this process accepts.
[<Erase>] type Pid<'Msg> = Pid of obj

/// Phantom 'Tag captures what the reference refers to (e.g., Ref<Pid<'Msg>> for a monitor).
[<Erase>] type Ref<'Tag> = Ref of obj

/// Phantom 'Msg captures the message delivered when the timer fires.
[<Erase>] type TimerRef<'Msg> = TimerRef of obj

/// Phantom 'Key and 'Row capture the ETS table's key and stored-row types.
[<Erase>] type TableId<'Key, 'Row> = TableId of obj

/// Phantom 'Call and 'Cast capture the gen_server's call/cast message types.
[<Erase>] type ServerRef<'Call, 'Cast> = ServerRef of obj
```

Then propagate the phantom through consuming functions:

```fsharp
// GOOD — phantom enforces that the sent message matches the process's mailbox type
[<Emit("$0 ! $1")>]
let send (pid: Pid<'Msg>) (msg: 'Msg) : unit = nativeOnly

// GOOD — monitor returns a Ref tagged with the monitored Pid
[<Emit("erlang:monitor(process, $0)")>]
let monitor (pid: Pid<'Msg>) : Ref<Pid<'Msg>> = nativeOnly

// BAD — no compile-time safety, any term can be sent
let send (pid: obj) (msg: obj) : unit = nativeOnly
```

For **constructor functions** that return a phantom-typed value, declare the generic
explicitly (`<'Msg>`) so F# generalizes properly across bindings. Without it, F# value
restriction may reject call sites:

```fsharp
// GOOD — explicit generic lets F# generalize across call sites
[<Emit("erlang:self()")>]
let self<'Msg> () : Pid<'Msg> = nativeOnly

// Subtly wrong — value restriction can make this non-generalizable
// let self () : Pid<'Msg> = nativeOnly
```

**Key caveat**: phantom typing guarantees *local F# consistency*, not cross-module
correctness. A `Pid<int>` that was constructed for a process actually expecting strings
is a lie the compiler cannot detect. Gate phantom construction through smart constructors
where possible (`spawn`, `whereis`, `makeRef<'Tag>`).

### DU cases compile to atoms (no `[<StringEnum>]` needed)

Plain F# discriminated union cases **without fields** compile directly to Erlang atoms on
Fable BEAM. The default rule is: **lowercase the first letter** of the case name.

```fsharp
type RandAlg =
    | Exsss       // → atom exsss
    | Exro928ss   // → atom exro928ss
    | Exs1024s    // → atom exs1024s
```

Use this for discrete atom sets: table types, log levels, protocol atoms, option flags.
Callers get compile-time checking that they're passing a valid atom — typos surface as
F# errors instead of runtime `function_clause`.

For **multi-word atom names** where the default lowercased first letter isn't right,
use `[<CompiledName>]`:

```fsharp
type EtsTableType =
    | Set                                             // → atom set
    | [<CompiledName("ordered_set")>] OrderedSet      // → atom ordered_set
    | Bag                                             // → atom bag
    | [<CompiledName("duplicate_bag")>] DuplicateBag  // → atom duplicate_bag
```

Verified: `[<CompiledName>]` on DU cases works on Fable BEAM (see `TestEts.fs` and
`TestRand.fs` for round-trip confirmation via `term_to_binary`/`binary_to_term`).

Keep all cases field-less when the DU represents an atom set. DUs *with* fields compile
to tagged tuples (`Local of Atom` → `{local, Atom}`), which is useful for tagged unions
but mixes two behaviours if the same type has both forms.

### `Dynamic` + `Decode` combinators for unknown shapes

When an Erlang function returns something whose shape isn't known statically, return
`Dynamic` instead of `obj`. Callers are forced through the `Decode` combinators to extract
typed values — validation localised at the boundary, not sprinkled through the codebase.

```fsharp
// Binding
[<Emit("erlang:binary_to_term($0)")>]
let binaryToTerm (bin: string) : Dynamic = nativeOnly

// Caller narrows with decoders
let d = Erlang.binaryToTerm payload

match Decode.int d with
| Ok n -> printfn "got int %d" n
| Error msg -> eprintfn "decode failed: %s" msg
```

Available combinators (see `src/otp/Dynamic.fs`):

| Combinator | Signature |
| --- | --- |
| `Decode.int` / `float` / `bool` / `atom` / `string` | `Dynamic -> Result<T, string>` |
| `Decode.dynamic` | `Dynamic -> Result<Dynamic, string>` (identity) |
| `Decode.field` | `Atom -> Func<Dynamic, Result<V, string>> -> Dynamic -> Result<V, string>` |
| `Decode.list` | `Func<Dynamic, Result<V, string>> -> Dynamic -> Result<V array, string>` |
| `Decode.optional` | `Func<Dynamic, Result<V, string>> -> Dynamic -> Result<V option, string>` |
| `Decode.tuple2` | `Func<..A..> -> Func<..B..> -> Dynamic -> Result<A * B, string>` |
| `Decode.succeed` / `map` / `andThen` | Result combinators |

Composing a record decoder:

```fsharp
let userDecoder (d: Dynamic) : Result<User, string> =
    Decode.field (Atom "name") (System.Func<_, _> Decode.string) d
    |> Result.bind (fun name ->
        Decode.field (Atom "age") (System.Func<_, _> Decode.int) d
        |> Result.map (fun age -> { Name = name; Age = age }))
```

`System.Func` is used for decoder callbacks — see "Curried Emit gotcha" below.

### Functions: use F# function types for callbacks

When an Erlang function takes a fun/callback, use the appropriate F# function type.

For `[<Emit>]` bindings, use curried F# function types:

```fsharp
/// Spawn a new process that executes the given function.
[<Emit("erlang:spawn(fun() -> $0(ok) end)")>]
let spawn (f: unit -> unit) : Pid = nativeOnly
```

Note: `unit` compiles to the atom `ok`, so `$0(ok)` calls the F# function with unit.

For `[<ImportAll>]` interfaces, use `System.Func<>` and `System.Action<>`
to type callback parameters. Fable compiles these to Erlang funs:

```fsharp
[<Erase>]
type IExports =
    /// Filters elements by a predicate.
    abstract filter: pred: System.Func<'T, bool> * list: BeamList<'T> -> BeamList<'T>
    /// Left fold over a list.
    abstract foldl: f: System.Func<'T, 'Acc, 'Acc> * acc: 'Acc * list: BeamList<'T> -> 'Acc
    /// Applies a function to each element for side effects.
    abstract foreach: f: System.Action<'T> * list: BeamList<'T> -> unit
    /// Applies a function to each key-value pair in a map.
    abstract fold: f: System.Func<'K, 'V, 'Acc, 'Acc> * init: 'Acc * map: BeamMap<'K, 'V> -> 'Acc
```

Usage — pass an F# lambda wrapped in `System.Func`:

```fsharp
lists.filter (System.Func<_, _>(fun x -> x > 3), xs)
lists.foldl (System.Func<_, _, _>(fun x acc -> acc + x), 0, xs)
```

### Erased unions: use for parameters or results with multiple types

Erlang APIs sometimes accept or return values of different types. For
example, a timeout might be an integer or the atom `infinity`, or a
server ref might be a `Pid`, an `Atom`, or a `{global, Name}` tuple.

Use `Fable.Core`'s erased union types (`U2`, `U3`, etc.) to express
this without losing type safety:

```fsharp
open Fable.Core

/// A gen_server timeout: either milliseconds or the atom 'infinity'.
[<Erase>]
type Timeout =
    | Milliseconds of int
    | Infinity of Atom

    static member op_ErasedCast(x: int) = Milliseconds x
    static member op_ErasedCast(x: Atom) = Infinity x
```

Or use the built-in `U2<'A, 'B>` / `U3<'A, 'B, 'C>` types directly:

```fsharp
[<Erase>]
type IExports =
    /// Call with a timeout that is either an int or the atom 'infinity'.
    abstract call: serverRef: obj * request: obj * timeout: U2<int, Atom> -> obj
```

The `op_ErasedCast` static members enable implicit conversion, so
callers can pass either type directly:

```fsharp
// Both work — the erased union accepts either type
gen_server.call (ref, msg, !^ 5000)
gen_server.call (ref, msg, !^ (Erlang.binaryToAtom "infinity"))
```

The `!^` operator triggers the erased cast. At the Erlang level,
the value is passed through unchanged — no wrapping or tagging.

### When `obj` is acceptable

**Very narrow** use cases only. The default for anything "polymorphic" should be a
generic parameter or `Dynamic`, not `obj`. Acceptable uses:

- **Genuinely type-irrelevant built-ins** — `phash2(Term, Range) -> int`, `exactEquals`
  already fall back to `obj` or a free generic because any term is valid; wrapping every
  call site in `Dynamic` would yield zero benefit.
- **Backing field of an erased DU** — `[<Erase>] type Pid<'Msg> = Pid of obj`. The user
  never sees this; the phantom is what matters.
- **Raw `IExports` escape hatches** — when the main API is a typed helper and the raw
  `IExports` is reserved as a fallback (e.g., `File.fs`).

For everything else that feels "polymorphic":

- **Caller decides the type?** → generic parameter (`'T`, `'Args`, `'Msg`).
- **Process dictionary / message passing?** → generic (`get<'Key, 'Value>`, `send (pid: Pid<'Msg>) (msg: 'Msg)`).
- **Unknown Erlang term?** → `Dynamic` and let the caller decode.
- **Multiple known types?** → `U2<A, B>` or a custom erased union.
- **Opaque reference?** → `[<Erase>] type X<'phantom> = X of obj`.

**Never** write `type X = obj` (a bare alias). Use `[<Erase>] type X = X of obj` instead.

### Type choice decision tree

```text
Is the Erlang type...
├── a number?                   → int, float, int64
├── a binary/string?            → string
├── a boolean atom?             → bool
├── the atom 'ok'?              → unit
├── a fixed-size tuple?         → T1 * T2 * ... (or Decode.tuple2/3 from Dynamic)
├── a known atom set?           → plain DU (no fields), [<CompiledName>] for snake_case
├── an open atom?               → Atom
├── {ok, V} | {error, R}?       → Result<T, string> (Emit wraps the tuple shape)
├── value | undefined?          → T option (IIFE converts undefined → None)
├── a map with known keys?      → F# record (compiles to #{field => value})
├── a map with arbitrary keys?  → BeamMap<K, V>
├── a list of known type?       → BeamList<T>  (or T array when you need F# array ops)
├── a list of tuples?           → BeamList<A * B>
├── an opaque handle?           → [<Erase>] type Foo<'phantom> = Foo of obj
├── one of N known types?       → U2<A,B> / U3<A,B,C> or custom erased union + op_ErasedCast
├── a callback fun?             → System.Func<...> or System.Action<...>
├── a heterogeneous tagged tuple? → [<Erase>] DU + [<Emit>] constructors (see WsFrame, ServerName)
├── a discrete atom enumeration? → plain DU (no fields); see RandAlg, EtsTableType
├── genuinely unknown at runtime? → Dynamic + Decode combinators (never bare obj)
└── truly caller-polymorphic?   → 'T (never bare obj)
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

Wrap Erlang opaque types in `[<Erase>]` single-case discriminated unions **with a phantom
type parameter**. This gives compile-time type safety with zero runtime overhead, and the
phantom lets the F# type system carry additional information across call sites (see the
earlier "Opaque types" section for full guidance on phantom parameters).

```fsharp
/// Erlang process identifier. 'Msg captures the mailbox message type.
[<Erase>]
type Pid<'Msg> = Pid of obj

/// Erlang reference. 'Tag captures what the reference refers to.
[<Erase>]
type Ref<'Tag> = Ref of obj

/// Erlang atom. (Atom does not need a phantom — all atoms are the same kind.)
[<Erase>]
type Atom = Atom of obj

/// ETS table identifier. 'Key and 'Row capture the stored tuple shape.
[<Erase>]
type TableId<'Key, 'Row> = TableId of obj
```

For bindings that don't benefit from a phantom (the handle is truly opaque and not
parameterised over anything), still wrap in a single-case `[<Erase>]` DU — **never use a
bare `type X = obj` alias** (see the anti-patterns section):

```fsharp
// GOOD
[<Erase>] type Req = Req of obj

// BAD — silently admits any obj
type Req = obj
```

### Generic erased types

For Erlang containers like maps and lists, use **generic** erased types
to get compile-time type safety on keys, values, and elements:

```fsharp
/// Erlang map with typed keys and values.
[<Erase>]
type BeamMap<'K, 'V> = BeamMap of obj

/// Erlang list with typed elements.
[<Erase>]
type BeamList<'T> = BeamList of obj
```

Use a `Beam` prefix to avoid confusion with F#'s built-in `Map` and
`list` types. The generics exist only at compile time — at the Erlang
level these are plain maps and lists with zero overhead.

Then use the generic types throughout the interface:

```fsharp
[<Erase>]
type IExports =
    abstract new_: unit -> BeamMap<'K, 'V>
    abstract get: key: 'K * map: BeamMap<'K, 'V> -> 'V
    abstract put: key: 'K * value: 'V * map: BeamMap<'K, 'V> -> BeamMap<'K, 'V>
    abstract is_key: key: 'K * map: BeamMap<'K, 'V> -> bool
    abstract size: map: BeamMap<'K, 'V> -> int
```

Usage becomes fully typed — no `box` needed:

```fsharp
let m: BeamMap<string, int> = maps.new_ ()
let m = maps.put ("key", 42, m)
maps.get ("key", m) |> equal 42  // returns int, not obj
```

## String and Atom Conversions

F# strings compile to Erlang binaries (`<<"hello">>`), **not** charlists.
Many OTP functions expect charlists, so you need to convert:

|       Direction       |                      Emit code                       |
| --------------------- | ---------------------------------------------------- |
| F# string → charlist  | `binary_to_list($0)`                                 |
| charlist → F# string  | `erlang:list_to_binary(...)`                         |
| F# string → atom      | `binary_to_atom($0)` or `erlang:binary_to_atom($0)`  |
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

F# arrays on BEAM are ref-wrapped values in the process dictionary
(via `fable_utils:new_ref/1`). Raw Erlang lists returned from OTP calls
(e.g., `ets:tab2list/1`, `maps:keys/1`) are **not** ref-wrapped. This
means `Array.length` and other F# array operations will fail on raw
Erlang lists.

**Solution:** Use `[<Emit>]` on abstract members to wrap the return
value with `fable_utils:new_ref()`, converting the Erlang list into a
proper F# array. For the reverse direction, unwrap with `erlang:get()`:

```fsharp
[<Erase>]
type IExports =
    /// Returns keys as an F# array (wraps Erlang list with new_ref).
    [<Emit("fable_utils:new_ref(maps:keys($0))")>]
    abstract keys: map: BeamMap<'K, 'V> -> 'K array

    /// Converts a map to key-value pairs as an F# array.
    [<Emit("fable_utils:new_ref(maps:to_list($0))")>]
    abstract to_list: map: BeamMap<'K, 'V> -> ('K * 'V) array

    /// Converts an F# array of key-value pairs to a map (unwraps ref).
    [<Emit("maps:from_list(erlang:get($0))")>]
    abstract from_list: list: ('K * 'V) array -> BeamMap<'K, 'V>
```

This allows standard F# array operations to work naturally:

```fsharp
maps.keys m |> Array.length |> equal 2       // works
maps.to_list m |> Array.map fst              // works
```

Note: `[<Emit>]` on an abstract member overrides the `[<ImportAll>]`
code generation for that specific method — other members still get the
automatic `module:function(...)` calls.

If you don't need F# array interop (e.g., the result stays in Erlang
list land), use `BeamList<'T>` as the return type instead and skip the
wrapping.

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

### Curried Emit + function-valued param: use `System.Func`

When an Emit-bound function is **curried** AND takes a **function-valued argument**,
Fable-BEAM can misplace positional `$N` substitutions in the generated Erlang. The symptom
is the decoder fn appearing where another argument should be — typically manifesting as a
runtime `{badmap, #Fun<...>}` or `badarity` error.

**Fix**: wrap callback parameters as `System.Func<>` (matches the existing `Lists.fs`
convention for `map`, `filter`, `foldl`):

```fsharp
// BAD — curried signature with raw F# function type. Positional $N may be swapped.
[<Emit("(fun() -> case maps:find($0, $2) of {ok, V__} -> $1(V__); ... end end)()")>]
let field (key: Atom) (decoder: Dynamic -> Result<'V, string>) (d: Dynamic) : Result<'V, string>
// Generated (wrong): maps:find(Key, <decoder-fun>)   ← $1 and $2 swapped

// GOOD — System.Func makes the decoder a single .NET value; substitutions are correct.
[<Emit("(fun() -> case maps:find($0, $2) of {ok, V__} -> $1(V__); ... end end)()")>]
let field (key: Atom) (decoder: System.Func<Dynamic, Result<'V, string>>) (d: Dynamic)
    : Result<'V, string>
// Generated (correct): maps:find(Key, D)
```

Call site: `Decode.field key (System.Func<_, _> Decode.int) d`.

The alternative is to switch the whole function to **tupled args**:

```fsharp
let field (key: Atom, decoder: Dynamic -> Result<'V, string>, d: Dynamic) : Result<'V, string>
// Call site: Decode.field (key, Decode.int, d)
```

`System.Func` is preferred when the function is a natural fit for curried partial
application and there are multiple callbacks in the same module's API (consistency with
`Lists.fs`).

## Anti-patterns

Things to avoid when writing new bindings:

### Anti-pattern: `type X = obj`

A bare alias makes `X` freely interchangeable with any `obj`, offering no type safety at
all. Always wrap in an erased DU instead:

```fsharp
// BAD
type Req = obj

// GOOD
[<Erase>] type Req = Req of obj
```

The `[<Erase>]` DU compiles away at runtime (zero cost) but prevents `obj` values from
silently flowing into a `Req`-typed slot.

### Anti-pattern: `obj list` as an options bag

```fsharp
// BAD — no type safety on what goes in the list
abstract new_: name: Atom * options: obj list -> TableId<'K, 'R>
```

Options lists in Erlang are usually heterogeneous but drawn from a closed set of shapes.
Model the shapes explicitly. Either use a plain DU (when all options are atoms):

```fsharp
type EtsTableType = Set | OrderedSet | Bag | DuplicateBag
abstract new_: name: Atom * options: EtsTableType list -> TableId<'K, 'R>
```

Or define a marker interface plus `[<Erase>]` static constructors (for heterogeneous
options including tuples like `{keypos, N}`):

```fsharp
type IEtsOption = interface end

[<Erase>]
type EtsOption =
    static member inline named_table: IEtsOption = unbox "named_table"
    static member inline tableType (t: EtsTableType): IEtsOption = unbox t
    static member inline keypos (n: int): IEtsOption = unbox (box (Atom "keypos", n))
```

### Anti-pattern: `obj` as handler state

```fsharp
// BAD — state is untyped; callers lose type info across handler boundaries
let ok (req: Req) (state: obj) : obj = nativeOnly
```

Handler callbacks thread a user-controlled state across invocations. Make it generic:

```fsharp
// GOOD — 'State is inferred from caller; the handler result carries it
[<Erase>] type HandlerResult<'State> = HandlerResult of obj

[<Emit("{ok, $0, $1}")>]
let ok (req: Req) (state: 'State) : HandlerResult<'State> = nativeOnly
```

### Anti-pattern: returning `obj` from an OTP call

```fsharp
// BAD — caller has no guidance on what the return actually is
abstract get_env: app: Atom * key: Atom -> obj
```

Prefer one of:

- **`Dynamic`** if the value is genuinely unknown — caller decodes.
- **Typed `Result<T, string>`** if the Erlang returns `{ok, V} | {error, R}` with an IIFE
  Emit wrapping.
- **Record** if the return is a map with known keys.
- **`T option`** if the return is `V | undefined` (IIFE converts the sentinel).

### Anti-pattern: boxing at call sites just to satisfy `obj`

If you find yourself writing `Erlang.foo (box value)` routinely, the binding is under-typed.
Add a generic parameter so `value` flows through without the `box`:

```fsharp
// BAD — forces box at every call site
let send (pid: Pid) (msg: obj) : unit = nativeOnly

// GOOD — types flow naturally
let send (pid: Pid<'Msg>) (msg: 'Msg) : unit = nativeOnly
```

## Module File Template

```fsharp
/// Type bindings for Erlang <module_name> module
/// See https://www.erlang.org/doc/apps/<app>/<module_name>
module Fable.Beam.ModuleName

open Fable.Core
open Fable.Beam  // for Atom, Pid, Ref, TimerRef, Dynamic, ...

// fsharplint:disable MemberNames

// ============================================================================
// Opaque types (add phantom type parameters!)
// ============================================================================

/// Erased handle — phantom 'Tag carries additional compile-time info.
[<Erase>]
type MyHandle<'Tag> = MyHandle of obj

// ============================================================================
// Discrete atom sets (plain DUs compile to atoms)
// ============================================================================

/// Options for MyOp. Each case compiles to an Erlang atom.
type MyOpFlag =
    | Simple                                        // → atom simple
    | [<CompiledName("with_log")>] WithLog          // → atom with_log

// ============================================================================
// Raw module binding — prefer typed helpers below for new code
// ============================================================================

[<Erase>]
type IExports =
    /// Does something. Use the typed `doSomething` below in new code.
    abstract do_something: arg: 'Arg -> obj

[<ImportAll("module_name")>]
let moduleName: IExports = nativeOnly

// ============================================================================
// Typed API (Result return for {ok, V} | {error, R} shapes)
// ============================================================================

/// WORKAROUND: IIFE wrapping required for case expressions until Fable fixes
/// the "unsafe variable" bug. Keep (fun() -> ... end)() around Emit cases.
[<Emit("(fun() -> case module_name:do_something($0) of {ok, Result__} -> {ok, Result__}; {error, Reason__} -> {error, erlang:atom_to_binary(Reason__)} end end)()")>]
let doSomething (arg: string) : Result<string, string> = nativeOnly

// ============================================================================
// For unknown-shape returns — return Dynamic, not obj
// ============================================================================

[<Emit("module_name:get_info($0)")>]
let getInfo (arg: string) : Dynamic = nativeOnly
```

## Complete Example: Binding a New OTP Module

Here's how you'd bind `crypto` (a module not yet in Fable.Beam), following every typing
convention in this guide:

```fsharp
/// Type bindings for Erlang crypto module
/// See https://www.erlang.org/doc/apps/crypto/crypto
module Fable.Beam.Crypto

open Fable.Core
open Fable.Beam

// fsharplint:disable MemberNames

// ============================================================================
// Discrete atom sets — plain DUs compile to atoms
// ============================================================================

/// Hash algorithms. Each case is a plain atom; multi-word names use [<CompiledName>].
type HashAlg =
    | Sha
    | Sha224
    | Sha256
    | Sha384
    | Sha512
    | Sha3_256
    | Md5
    | [<CompiledName("blake2b")>] Blake2b
    | [<CompiledName("blake2s")>] Blake2s

// ============================================================================
// Raw module binding — typed helpers below are the preferred API
// ============================================================================

[<Erase>]
type IExports =
    /// Lists supported hash algorithms.
    abstract supports: ``type``: Atom -> Atom list

[<ImportAll("crypto")>]
let crypto: IExports = nativeOnly

// ============================================================================
// Typed helpers — prefer these over the raw IExports
// ============================================================================

/// Compute a message digest using the given algorithm.
[<Emit("crypto:hash($0, $1)")>]
let hash (alg: HashAlg) (data: string) : string = nativeOnly

/// Compute a SHA-256 hash of a binary string.
[<Emit("crypto:hash(sha256, $0)")>]
let sha256 (data: string) : string = nativeOnly

/// Generate N cryptographically strong random bytes as a binary.
[<Emit("crypto:strong_rand_bytes($0)")>]
let randomBytes (n: int) : string = nativeOnly
```

Call sites are now type-safe — a typo in the algorithm name is a compile error, and the
`data` / result types are `string` (binaries) instead of `obj`:

```fsharp
let digest = Crypto.hash HashAlg.Sha256 "hello world"  // ok
let digest = Crypto.hash HashAlg.Shah256 "hello world" // COMPILE ERROR — no such case
```

Usage:

```fsharp
open Fable.Beam.Crypto

let hash = sha256 "hello world"
let bytes = randomBytes 16
```
