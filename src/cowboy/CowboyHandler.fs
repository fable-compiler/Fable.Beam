/// Fable helpers for Cowboy HTTP handler return types.
/// See: https://ninenines.eu/docs/en/cowboy/2.12/manual/cowboy_handler/
module Fable.Beam.Cowboy.CowboyHandler

open Fable.Core

/// Return {ok, Req, State} from init/2.
[<Emit("{ok, $0, $1}")>]
let ok (req: obj) (state: obj) : obj = nativeOnly
