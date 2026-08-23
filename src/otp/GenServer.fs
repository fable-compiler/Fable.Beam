/// Type bindings for OTP gen_server behaviour
/// See https://www.erlang.org/doc/apps/stdlib/gen_server
module Fable.Beam.GenServer

open Fable.Core
open Fable.Beam

// fsharplint:disable MemberNames

/// A gen_server reference: Pid, registered name, or {global, Name}.
/// The phantom parameters capture the types of call/cast messages this server
/// accepts — analogous to Gleam's Subject pattern.
[<Erase>]
type ServerRef<'Call, 'Cast> = ServerRef of obj

/// How a gen_server is identified when starting/registering. Erased at runtime.
[<Erase>]
type ServerName = ServerName of obj

/// Register the gen_server locally as Name ({local, Name} tuple).
[<Emit("{local, $0}")>]
let localName (name: Atom) : ServerName = nativeOnly

/// Register the gen_server globally as Name ({global, Name} tuple).
[<Emit("{global, $0}")>]
let globalName (name: Atom) : ServerName = nativeOnly

/// Register the gen_server via an alternative registry ({via, Module, Name} tuple).
[<Emit("{via, $0, $1}")>]
let viaName (``module``: Atom) (name: Atom) : ServerName = nativeOnly

/// Opaque client tag passed to a `handle_call` callback and forwarded to
/// `reply` for a deferred response. Erased at runtime.
[<Erase>]
type From = From of obj

/// Starts a gen_server process and links it to the caller.
[<Emit("gen_server:start_link($0, $1, $2)")>]
let startLink (``module``: Atom) (args: 'Args) (options: obj list) : Result<Pid<'Msg>, Dynamic> = nativeOnly

/// Starts a named gen_server process and links it to the caller.
[<Emit("gen_server:start_link($0, $1, $2, $3)")>]
let startLinkNamed
    (name: ServerName)
    (``module``: Atom)
    (args: 'Args)
    (options: obj list)
    : Result<Pid<'Msg>, Dynamic> =
    nativeOnly

/// Starts a gen_server process without linking it to the caller.
[<Emit("gen_server:start($0, $1, $2)")>]
let start (``module``: Atom) (args: 'Args) (options: obj list) : Result<Pid<'Msg>, Dynamic> = nativeOnly

/// Starts a named gen_server process without linking it to the caller.
[<Emit("gen_server:start($0, $1, $2, $3)")>]
let startNamed (name: ServerName) (``module``: Atom) (args: 'Args) (options: obj list) : Result<Pid<'Msg>, Dynamic> =
    nativeOnly

/// Makes a synchronous call to a gen_server.
[<Emit("gen_server:call($0, $1)")>]
let call (serverRef: ServerRef<'Call, 'Cast>) (request: 'Call) : 'Reply = nativeOnly

/// Makes a synchronous call with a timeout in milliseconds or the `infinity` atom.
[<Emit("gen_server:call($0, $1, $2)")>]
let callWithTimeout (serverRef: ServerRef<'Call, 'Cast>) (request: 'Call) (timeout: U2<int, Atom>) : 'Reply = nativeOnly

/// Sends an asynchronous request to a gen_server.
[<Emit("gen_server:cast($0, $1)")>]
let cast (serverRef: ServerRef<'Call, 'Cast>) (request: 'Cast) : unit = nativeOnly

/// Sends a reply to a client that called `call`.
[<Emit("gen_server:reply($0, $1)")>]
let reply (from: From) (value: 'Reply) : unit = nativeOnly

/// Stops a gen_server.
[<Emit("gen_server:stop($0)")>]
let stop (serverRef: ServerRef<'Call, 'Cast>) : unit = nativeOnly

/// Stops a gen_server with a reason and timeout.
[<Emit("gen_server:stop($0, $1, $2)")>]
let stopWith (serverRef: ServerRef<'Call, 'Cast>) (reason: Atom) (timeout: U2<int, Atom>) : unit = nativeOnly
