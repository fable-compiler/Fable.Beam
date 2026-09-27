/// Typed passive-client bindings for Erlang's `gen_tcp` module.
/// See https://www.erlang.org/doc/apps/kernel/gen_tcp
module Fable.Beam.GenTcp

open Fable.Core

/// An opaque connected TCP socket owned by the calling BEAM process.
[<Erase>]
type Socket = private Socket of obj

/// Packet framing applied by OTP before `recv` returns binary data.
[<RequireQualifiedAccess>]
type PacketMode =
    | Raw
    | Line

/// Options for a passive binary TCP connection.
/// `None` uses OTP's infinite connect timeout.
type ConnectOptions =
    {
        packetMode: PacketMode
        connectTimeoutMs: int option
    }

/// Common passive binary connection configurations.
[<RequireQualifiedAccess>]
module ConnectOptions =
    /// Passive binary stream with no packet framing and OTP's default timeout.
    let defaultOptions =
        {
            packetMode = PacketMode.Raw
            connectTimeoutMs = None
        }

    /// Passive binary, line-delimited stream with OTP's default timeout.
    let lineClient =
        {
            packetMode = PacketMode.Line
            connectTimeoutMs = None
        }

[<Emit("gen_tcp:connect(binary_to_list($0), $1, [binary, {active, false}, {packet, $2}])")>]
let private connectDefaultRaw (host: string) (port: int) (packetMode: PacketMode) : Result<Socket, Dynamic> =
    nativeOnly

[<Emit("gen_tcp:connect(binary_to_list($0), $1, [binary, {active, false}, {packet, $2}], $3)")>]
let private connectTimeoutRaw
    (host: string)
    (port: int)
    (packetMode: PacketMode)
    (timeoutMs: int)
    : Result<Socket, Dynamic> =
    nativeOnly

[<Emit("erlang:iolist_to_binary(io_lib:format(\"~p\", [$0]))")>]
let private formatError (reason: Dynamic) : string = nativeOnly

/// Connects a passive binary TCP client to a hostname or textual IP address.
let connect (host: string) (port: int) (options: ConnectOptions) : Result<Socket, string> =
    match options.connectTimeoutMs with
    | Some timeoutMs when timeoutMs < 0 -> Error "connect timeout must be non-negative"
    | Some timeoutMs -> connectTimeoutRaw host port options.packetMode timeoutMs |> Result.mapError formatError
    | None -> connectDefaultRaw host port options.packetMode |> Result.mapError formatError

/// Connects a passive binary, line-delimited client using OTP's default timeout.
let connectLineClient (host: string) (port: int) : Result<Socket, string> =
    connect host port ConnectOptions.lineClient

/// Receives binary data from a passive socket.
/// `length = 0` returns all currently available data in raw mode; packet modes
/// such as `Line` determine their own returned packet boundary.
[<Emit("gen_tcp:recv($0, $1, $2)")>]
let private recvRaw (socket: Socket) (length: int) (timeoutMs: int) : Result<string, Dynamic> = nativeOnly

/// Receives binary data from a passive socket.
let recv (socket: Socket) (length: int) (timeoutMs: int) : Result<string, string> =
    if length < 0 then
        Error "receive length must be non-negative"
    elif timeoutMs < 0 then
        Error "receive timeout must be non-negative"
    else
        recvRaw socket length timeoutMs |> Result.mapError formatError

/// Sends binary data on a connected socket.
[<Emit("case gen_tcp:send($0, $1) of ok -> {ok, ok}; {error, GenTcpSendReason__} -> {error, GenTcpSendReason__} end")>]
let private sendRaw (socket: Socket) (data: string) : Result<unit, Dynamic> = nativeOnly

/// Sends binary data on a connected socket.
let send (socket: Socket) (data: string) : Result<unit, string> =
    sendRaw socket data |> Result.mapError formatError

/// Closes a TCP socket.
[<Emit("gen_tcp:close($0)")>]
let close (socket: Socket) : unit = nativeOnly
