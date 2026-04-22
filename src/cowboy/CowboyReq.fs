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

/// Read the full request body. Returns {ok, Body, Req}.
[<Emit("cowboy_req:read_body($0)")>]
let readBody (req: Req) : obj * byte array * Req = nativeOnly

/// Get the query string.
[<Emit("cowboy_req:qs($0)")>]
let queryString (req: Req) : string = nativeOnly

/// Send a full response: status, headers map, body, req.
[<Emit("cowboy_req:reply($0, $1, $2, $3)")>]
let reply (status: int) (headers: BeamMap<string, string>) (body: string) (req: Req) : Req = nativeOnly

/// Send a response with status and headers only (no body).
[<Emit("cowboy_req:reply($0, $1, $2)")>]
let replyNoBody (status: int) (headers: BeamMap<string, string>) (req: Req) : Req = nativeOnly

/// Get the peer IP address and port.
[<Emit("cowboy_req:peer($0)")>]
let peer (req: Req) : obj * int = nativeOnly

/// Get the scheme (<<"http">> or <<"https">>).
[<Emit("cowboy_req:scheme($0)")>]
let scheme (req: Req) : string = nativeOnly

/// Get the host.
[<Emit("cowboy_req:host($0)")>]
let host (req: Req) : string = nativeOnly

/// Get the port number.
[<Emit("cowboy_req:port($0)")>]
let port (req: Req) : int = nativeOnly

/// Parse query string into a list of key-value pairs.
[<Emit("cowboy_req:parse_qs($0)")>]
let parseQs (req: Req) : obj = nativeOnly

/// Get a binding value from the route.
[<Emit("cowboy_req:binding($0, $1)")>]
let binding (name: Atom) (req: Req) : string = nativeOnly

/// Get a binding value with a default.
[<Emit("cowboy_req:binding($0, $1, $2)")>]
let bindingDefault (name: Atom) (req: Req) (defaultValue: string) : string = nativeOnly
