/// Type bindings for OTP gen_server behaviour
/// See https://www.erlang.org/doc/apps/stdlib/gen_server
module Fable.Beam.GenServer

open Fable.Core
open Fable.Beam.Erlang

// fsharplint:disable MemberNames

/// A gen_server reference: Pid, registered name, or {global, Name}.
[<Erase>]
type ServerRef = ServerRef of obj

[<Erase>]
type IExports =
    /// Starts a gen_server process.
    abstract start_link: ``module``: Atom * args: obj * options: obj list -> Result<Pid, obj>
    /// Starts a named gen_server process.
    abstract start_link: name: obj * ``module``: Atom * args: obj * options: obj list -> Result<Pid, obj>
    /// Starts a gen_server without linking.
    abstract start: ``module``: Atom * args: obj * options: obj list -> Result<Pid, obj>
    /// Starts a named gen_server without linking.
    abstract start: name: obj * ``module``: Atom * args: obj * options: obj list -> Result<Pid, obj>
    /// Makes a synchronous call to a gen_server.
    abstract call: serverRef: ServerRef * request: obj -> obj
    /// Makes a synchronous call with timeout (int ms or atom 'infinity').
    abstract call: serverRef: ServerRef * request: obj * timeout: U2<int, Atom> -> obj
    /// Sends an asynchronous request to a gen_server.
    abstract cast: serverRef: ServerRef * request: obj -> unit
    /// Sends a reply to a client that called call/2,3.
    abstract reply: from: obj * reply: obj -> unit
    /// Stops a gen_server.
    abstract stop: serverRef: ServerRef -> unit
    /// Stops a gen_server with reason and timeout (int ms or atom 'infinity').
    abstract stop: serverRef: ServerRef * reason: Atom * timeout: U2<int, Atom> -> unit

/// gen_server module
[<ImportAll("gen_server")>]
let gen_server: IExports = nativeOnly
