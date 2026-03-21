/// Fable bindings for Cowboy's cowboy_router module.
/// See: https://ninenines.eu/docs/en/cowboy/2.14/manual/cowboy_router/
module Fable.Beam.Cowboy.CowboyRouter

open Fable.Core

/// Compile routing rules into a dispatch list.
[<Emit("cowboy_router:compile($0)")>]
let compile (routes: obj) : obj = nativeOnly

/// Route: {Path, Handler, InitialState}.
[<Emit("{$0, $1, $2}")>]
let route (path: string) (handler: obj) (state: obj) : obj = nativeOnly

/// Host rule: {HostMatch, Routes}.
[<Emit("{$0, $1}")>]
let hostRule (host: obj) (routes: obj list) : obj = nativeOnly
