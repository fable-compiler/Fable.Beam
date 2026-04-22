/// Fable helpers for Cowboy WebSocket handlers.
/// See: https://ninenines.eu/docs/en/cowboy/2.12/manual/cowboy_websocket/
module Fable.Beam.Cowboy.CowboyWebsocket

open Fable.Core

/// Erased wrapper for a WebSocket frame. Use the helpers below to construct.
[<Erase>]
type WsFrame = WsFrame of obj

/// Erased wrapper for a WebSocket callback's return value.
/// The phantom 'State captures the handler state.
[<Erase>]
type WsResult<'State> = WsResult of obj

// -- Frame constructors --

/// Create a text frame: {text, Data}.
[<Emit("{text, $0}")>]
let textFrame (data: string) : WsFrame = nativeOnly

/// Create a binary frame: {binary, Data}.
[<Emit("{binary, $0}")>]
let binaryFrame (data: byte array) : WsFrame = nativeOnly

/// Create a ping frame.
[<Emit("ping")>]
let pingFrame: WsFrame = nativeOnly

/// Create a pong frame.
[<Emit("pong")>]
let pongFrame: WsFrame = nativeOnly

/// Create a close frame: {close, Code, Reason}.
[<Emit("{close, $0, $1}")>]
let closeFrame (code: int) (reason: string) : WsFrame = nativeOnly

// -- Handler return types --

/// Return {cowboy_websocket, Req, State} from init/2 to upgrade to WebSocket.
[<Emit("{cowboy_websocket, $0, $1}")>]
let upgrade (req: CowboyReq.Req) (state: 'State) : WsResult<'State> = nativeOnly

/// Return {cowboy_websocket, Req, State, Opts} from init/2 with options.
[<Emit("{cowboy_websocket, $0, $1, $2}")>]
let upgradeWithOpts (req: CowboyReq.Req) (state: 'State) (opts: obj) : WsResult<'State> = nativeOnly

/// Return {ok, State} from websocket_init/1 or websocket_handle/2.
[<Emit("{ok, $0}")>]
let ok (state: 'State) : WsResult<'State> = nativeOnly

/// Return {reply, Frame, State} to send a single frame.
[<Emit("{reply, $0, $1}")>]
let reply (frame: WsFrame) (state: 'State) : WsResult<'State> = nativeOnly

/// Return {reply, Frames, State} to send multiple frames.
[<Emit("{reply, $0, $1}")>]
let replyMany (frames: WsFrame list) (state: 'State) : WsResult<'State> = nativeOnly

/// Return {stop, State} to close the WebSocket connection.
[<Emit("{stop, $0}")>]
let stop (state: 'State) : WsResult<'State> = nativeOnly
