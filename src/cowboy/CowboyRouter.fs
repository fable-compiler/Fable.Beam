/// Fable bindings for Cowboy's cowboy_router module.
/// See: https://ninenines.eu/docs/en/cowboy/2.12/manual/cowboy_router/
module Fable.Beam.Cowboy.CowboyRouter

open Fable.Core

/// Compile routing rules into a dispatch list.
[<Emit("cowboy_router:compile($0)")>]
let compile (routes: obj) : obj = nativeOnly
