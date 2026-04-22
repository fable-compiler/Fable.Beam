/// Fable helpers for Cowboy HTTP handler return types.
/// See: https://ninenines.eu/docs/en/cowboy/2.12/manual/cowboy_handler/
module Fable.Beam.Cowboy.CowboyHandler

open Fable.Core

/// Erased wrapper for a handler callback's return value.
/// The phantom 'State captures the handler state threaded through callbacks.
[<Erase>]
type HandlerResult<'State> = HandlerResult of obj

/// Return {ok, Req, State} from init/2.
[<Emit("{ok, $0, $1}")>]
let ok (req: CowboyReq.Req) (state: 'State) : HandlerResult<'State> = nativeOnly
