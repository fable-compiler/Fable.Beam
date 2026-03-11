/// Fable bindings for the Cowboy HTTP server.
/// See: https://ninenines.eu/docs/en/cowboy/2.12/manual/cowboy/
module Fable.Beam.Cowboy.Cowboy

open Fable.Core

/// Start a clear (HTTP) listener.
[<Emit("cowboy:start_clear($0, $1, $2)")>]
let startClear (name: obj) (transportOpts: obj) (protoOpts: obj) : obj = nativeOnly

/// Start a TLS (HTTPS) listener.
[<Emit("cowboy:start_tls($0, $1, $2)")>]
let startTls (name: obj) (transportOpts: obj) (protoOpts: obj) : obj = nativeOnly

/// Stop a running listener.
[<Emit("cowboy:stop_listener($0)")>]
let stopListener (name: obj) : obj = nativeOnly
