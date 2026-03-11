/// Fable helpers for Cowboy WebSocket handlers.
/// See: https://ninenines.eu/docs/en/cowboy/2.12/manual/cowboy_websocket/
module Fable.Beam.Cowboy.CowboyWebsocket

open Fable.Core

// -- Handler return types --

/// Return {cowboy_websocket, Req, State} from init/2 to upgrade to WebSocket.
[<Emit("{cowboy_websocket, $0, $1}")>]
let upgrade (req: obj) (state: obj) : obj = nativeOnly

/// Return {cowboy_websocket, Req, State, Opts} from init/2 with options.
[<Emit("{cowboy_websocket, $0, $1, $2}")>]
let upgradeWithOpts (req: obj) (state: obj) (opts: obj) : obj = nativeOnly

/// Return {ok, State} from websocket_init/1 or websocket_handle/2.
[<Emit("{ok, $0}")>]
let ok (state: obj) : obj = nativeOnly

/// Return {reply, Frame, State} to send a single frame.
[<Emit("{reply, $0, $1}")>]
let reply (frame: obj) (state: obj) : obj = nativeOnly

/// Return {reply, Frames, State} to send multiple frames.
[<Emit("{reply, $0, $1}")>]
let replyMany (frames: obj list) (state: obj) : obj = nativeOnly

/// Return {stop, State} to close the WebSocket connection.
[<Emit("{stop, $0}")>]
let stop (state: obj) : obj = nativeOnly

// -- Frame constructors --

/// Create a text frame: {text, Data}.
[<Emit("{text, $0}")>]
let textFrame (data: string) : obj = nativeOnly

/// Create a binary frame: {binary, Data}.
[<Emit("{binary, $0}")>]
let binaryFrame (data: obj) : obj = nativeOnly

/// Create a ping frame.
[<Emit("ping")>]
let pingFrame: obj = nativeOnly

/// Create a pong frame.
[<Emit("pong")>]
let pongFrame: obj = nativeOnly

/// Create a close frame: {close, Code, Reason}.
[<Emit("{close, $0, $1}")>]
let closeFrame (code: int) (reason: string) : obj = nativeOnly
