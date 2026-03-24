/// Type bindings for OTP gen_server behaviour
/// See https://www.erlang.org/doc/apps/stdlib/gen_server
module Fable.Beam.GenServer

open Fable.Core
open Fable.Beam.Erlang

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Starts a gen_server process. Returns {ok, Pid} | ignore | {error, Reason}.
    abstract start_link: ``module``: Atom * args: obj * options: obj list -> obj
    /// Starts a named gen_server process. Returns {ok, Pid} | ignore | {error, Reason}.
    abstract start_link: name: obj * ``module``: Atom * args: obj * options: obj list -> obj
    /// Starts a gen_server without linking. Returns {ok, Pid} | ignore | {error, Reason}.
    abstract start: ``module``: Atom * args: obj * options: obj list -> obj
    /// Starts a named gen_server without linking. Returns {ok, Pid} | ignore | {error, Reason}.
    abstract start: name: obj * ``module``: Atom * args: obj * options: obj list -> obj
    /// Makes a synchronous call to a gen_server.
    abstract call: serverRef: obj * request: obj -> obj
    /// Makes a synchronous call with timeout. Use Erlang.binaryToAtom "infinity" for no timeout.
    abstract call: serverRef: obj * request: obj * timeout: obj -> obj
    /// Sends an asynchronous request to a gen_server.
    abstract cast: serverRef: obj * request: obj -> unit
    /// Sends a reply to a client that called call/2,3.
    abstract reply: from: obj * reply: obj -> unit
    /// Stops a gen_server.
    abstract stop: serverRef: obj -> unit
    /// Stops a gen_server with reason and timeout. Use Erlang.binaryToAtom "infinity" for no timeout.
    abstract stop: serverRef: obj * reason: obj * timeout: obj -> unit

/// gen_server module
[<ImportAll("gen_server")>]
let gen_server: IExports = nativeOnly
