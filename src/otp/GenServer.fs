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

[<Erase>]
type IExports =
    /// Starts a gen_server process.
    abstract start_link: ``module``: Atom * args: 'Args * options: obj list -> Result<Pid<'Msg>, obj>
    /// Starts a named gen_server process.
    abstract start_link: name: ServerName * ``module``: Atom * args: 'Args * options: obj list -> Result<Pid<'Msg>, obj>
    /// Starts a gen_server without linking.
    abstract start: ``module``: Atom * args: 'Args * options: obj list -> Result<Pid<'Msg>, obj>
    /// Starts a named gen_server without linking.
    abstract start: name: ServerName * ``module``: Atom * args: 'Args * options: obj list -> Result<Pid<'Msg>, obj>
    /// Makes a synchronous call to a gen_server.
    abstract call: serverRef: ServerRef<'Call, 'Cast> * request: 'Call -> 'Reply
    /// Makes a synchronous call with timeout (int ms or atom 'infinity').
    abstract call: serverRef: ServerRef<'Call, 'Cast> * request: 'Call * timeout: U2<int, Atom> -> 'Reply
    /// Sends an asynchronous request to a gen_server.
    abstract cast: serverRef: ServerRef<'Call, 'Cast> * request: 'Cast -> unit
    /// Sends a reply to a client that called call/2,3.
    abstract reply: from: obj * reply: 'Reply -> unit
    /// Stops a gen_server.
    abstract stop: serverRef: ServerRef<'Call, 'Cast> -> unit
    /// Stops a gen_server with reason and timeout (int ms or atom 'infinity').
    abstract stop: serverRef: ServerRef<'Call, 'Cast> * reason: Atom * timeout: U2<int, Atom> -> unit

/// gen_server module
[<ImportAll("gen_server")>]
let gen_server: IExports = nativeOnly
