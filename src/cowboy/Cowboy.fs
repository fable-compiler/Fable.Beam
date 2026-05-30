/// Fable bindings for the Cowboy HTTP server.
/// See: https://ninenines.eu/docs/en/cowboy/2.14/manual/cowboy/
module Fable.Beam.Cowboy.Cowboy

open Fable.Core
open Fable.Beam
open Fable.Beam.Cowboy.CowboyRouter

// ============================================================================
// Transport options (ranch socket options)
// ============================================================================

/// Cowboy/ranch transport options. Erased at runtime. Construct with the
/// helpers below (more constructors can be added without breaking callers).
[<Erase>]
type TransportOpts = TransportOpts of obj

/// Transport options listening on the given TCP port: `[{port, Port}]`.
[<Emit("[{port, $0}]")>]
let tcpPort (port: int) : TransportOpts = nativeOnly

// ============================================================================
// Protocol options
// ============================================================================

/// Cowboy protocol options. Erased at runtime.
[<Erase>]
type ProtocolOpts = ProtocolOpts of obj

/// Protocol options carrying the router dispatch table:
/// `#{env => #{dispatch => Dispatch}}`.
[<Emit("#{env => #{dispatch => $0}}")>]
let protocolOpts (dispatch: Dispatch) : ProtocolOpts = nativeOnly

// ============================================================================
// Listeners
// ============================================================================

/// Start a clear (HTTP) listener. Returns the listener pid, or an error string.
[<Emit("(fun() -> case cowboy:start_clear($0, $1, $2) of {ok, StartClearPid__} -> {ok, StartClearPid__}; {error, StartClearReason__} -> {error, erlang:list_to_binary(io_lib:format(<<\"~p\">>, [StartClearReason__]))} end end)()")>]
let startClear (name: Atom) (transportOpts: TransportOpts) (protoOpts: ProtocolOpts) : Result<Pid<obj>, string> =
    nativeOnly

/// Start a TLS (HTTPS) listener. Returns the listener pid, or an error string.
[<Emit("(fun() -> case cowboy:start_tls($0, $1, $2) of {ok, StartTlsPid__} -> {ok, StartTlsPid__}; {error, StartTlsReason__} -> {error, erlang:list_to_binary(io_lib:format(<<\"~p\">>, [StartTlsReason__]))} end end)()")>]
let startTls (name: Atom) (transportOpts: TransportOpts) (protoOpts: ProtocolOpts) : Result<Pid<obj>, string> =
    nativeOnly

/// Stop a running listener. Returns Ok, or an error string if not found.
[<Emit("(fun() -> case cowboy:stop_listener($0) of ok -> {ok, ok}; {error, StopListenerReason__} -> {error, erlang:atom_to_binary(StopListenerReason__)} end end)()")>]
let stopListener (name: Atom) : Result<unit, string> = nativeOnly

/// Retrieve a listener's environment value (dynamic — decode as needed).
[<Emit("cowboy:get_env($0, $1)")>]
let getEnv (name: Atom) (key: Atom) : Dynamic = nativeOnly

/// Update a listener's environment value.
[<Emit("cowboy:set_env($0, $1, $2)")>]
let setEnv (name: Atom) (key: Atom) (value: 'V) : unit = nativeOnly
