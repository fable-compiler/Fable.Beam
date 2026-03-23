/// Type bindings for Erlang init module
/// See https://www.erlang.org/doc/apps/erts/init
module Fable.Beam.Init

open Fable.Core

/// Gracefully shut down the Erlang runtime system.
[<Emit("init:stop()")>]
let stop () : unit = nativeOnly
