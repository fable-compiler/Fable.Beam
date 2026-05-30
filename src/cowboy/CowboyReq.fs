/// Fable bindings for Cowboy's cowboy_req module.
/// See: https://ninenines.eu/docs/en/cowboy/2.12/manual/cowboy_req/
module Fable.Beam.Cowboy.CowboyReq

open Fable.Core
open Fable.Beam
open Fable.Beam.Maps

/// Opaque Cowboy request object. Erased at runtime — prevents accidental
/// mixing with other untyped Erlang terms.
[<Erase>]
type Req = Req of obj

/// Get the HTTP method (e.g. <<"GET">>, <<"POST">>).
[<Emit("cowboy_req:method($0)")>]
let method' (req: Req) : string = nativeOnly

/// Get the request path (e.g. <<"/api/users">>).
[<Emit("cowboy_req:path($0)")>]
let path (req: Req) : string = nativeOnly

/// Get a specific header value.
[<Emit("cowboy_req:header($0, $1)")>]
let header (name: string) (req: Req) : string = nativeOnly

/// Get a specific header value with a default.
[<Emit("cowboy_req:header($0, $1, $2)")>]
let headerDefault (name: string) (req: Req) (defaultValue: string) : string = nativeOnly

/// Get all request headers as a map of binary-keyed strings.
[<Emit("cowboy_req:headers($0)")>]
let headers (req: Req) : BeamMap<string, string> = nativeOnly

/// Completeness status of a `readBody` call. `Ok` means the whole body was
/// read; `More` means it exceeded the read length and `readBody` must be called
/// again with the returned `Req` to read the next chunk.
[<RequireQualifiedAccess>]
type BodyStatus =
    | Ok
    | More

/// Read a chunk of the request body. Returns the completeness status, the chunk
/// data (a binary), and the updated request. For bodies within the default read
/// length this is a single `Ok` read; larger bodies yield `More` until the final
/// `Ok`. Maps `cowboy_req:read_body/1`'s `{ok | more, Data, Req}`.
[<Emit("cowboy_req:read_body($0)")>]
let readBody (req: Req) : BodyStatus * string * Req = nativeOnly

/// Get the query string.
[<Emit("cowboy_req:qs($0)")>]
let queryString (req: Req) : string = nativeOnly

/// Send a full response: status, headers map, body, req.
[<Emit("cowboy_req:reply($0, $1, $2, $3)")>]
let reply (status: int) (headers: BeamMap<string, string>) (body: string) (req: Req) : Req = nativeOnly

/// Send a response with status and headers only (no body).
[<Emit("cowboy_req:reply($0, $1, $2)")>]
let replyNoBody (status: int) (headers: BeamMap<string, string>) (req: Req) : Req = nativeOnly

/// Remote peer address (as a string, e.g. "127.0.0.1") and port.
type Peer = { Ip: string; Port: int }

/// Get the remote peer address and port. The raw `inet:ip_address()` tuple is
/// rendered to a string via `inet:ntoa/1`.
[<Emit("(fun() -> {PeerIp__, PeerPort__} = cowboy_req:peer($0), #{ip => erlang:list_to_binary(inet:ntoa(PeerIp__)), port => PeerPort__} end)()")>]
let peer (req: Req) : Peer = nativeOnly

/// Get the scheme (<<"http">> or <<"https">>).
[<Emit("cowboy_req:scheme($0)")>]
let scheme (req: Req) : string = nativeOnly

/// Get the host.
[<Emit("cowboy_req:host($0)")>]
let host (req: Req) : string = nativeOnly

/// Get the port number.
[<Emit("cowboy_req:port($0)")>]
let port (req: Req) : int = nativeOnly

/// Parse the query string into key-value pairs. Valueless keys (e.g. `?flag`)
/// yield an empty-string value.
[<Emit("(fun() -> [{ParseQsK__, case ParseQsV__ of true -> <<>>; _ -> ParseQsV__ end} || {ParseQsK__, ParseQsV__} <- cowboy_req:parse_qs($0)] end)()")>]
let parseQs (req: Req) : (string * string) list = nativeOnly

/// Get a binding value from the route.
[<Emit("cowboy_req:binding($0, $1)")>]
let binding (name: Atom) (req: Req) : string = nativeOnly

/// Get a binding value with a default.
[<Emit("cowboy_req:binding($0, $1, $2)")>]
let bindingDefault (name: Atom) (req: Req) (defaultValue: string) : string = nativeOnly
