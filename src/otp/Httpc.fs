/// Type bindings for Erlang httpc module (inets application)
/// See https://www.erlang.org/doc/apps/inets/httpc
///
/// Note: You must start the inets application before using httpc:
///   application.ensure_all_started (Erlang.binaryToAtom "inets")
///   application.ensure_all_started (Erlang.binaryToAtom "ssl")
module Fable.Beam.Httpc

open Fable.Core

// fsharplint:disable MemberNames

/// HTTP response with typed fields.
type HttpResponse =
    { StatusCode: int
      Body: string }

// ============================================================================
// SSL configuration
// ============================================================================

/// Opaque SSL options passed to httpc requests.
[<Erase>]
type SslOptions = SslOptions of obj

/// Disable certificate verification (for development only).
[<Emit("[{ssl, [{verify, verify_none}]}]")>]
let verifyNone: SslOptions = nativeOnly

/// Verify peer certificate using system CA certificates (OTP 25+).
[<Emit("[{ssl, [{verify, verify_peer}, {cacerts, public_key:cacerts_get()}]}]")>]
let verifyPeer: SslOptions = nativeOnly

/// Verify peer certificate using a specific CA certificate file.
[<Emit("[{ssl, [{verify, verify_peer}, {cacertfile, binary_to_list($0)}]}]")>]
let verifyPeerCaFile (caFile: string) : SslOptions = nativeOnly

// ============================================================================
// Internal helpers
// ============================================================================
// WORKAROUND: Emit expressions wrapped in (fun() -> ... end)() to prevent
// Erlang "unsafe variable" errors. Remove IIFEs once fixed in Fable.

[<Emit("""
(fun() ->
    Url = binary_to_list($0),
    Headers = [{binary_to_list(K), binary_to_list(V)} || {K, V} <- $1],
    case httpc:request(get, {Url, Headers}, $2, []) of
        {ok, {{_, StatusCode, _}, _RespHeaders, Body}} ->
            {ok, {StatusCode, erlang:list_to_binary(Body)}};
        {error, Reason} ->
            {error, erlang:list_to_binary(io_lib:format(<<"~p">>, [Reason]))}
    end
end)()
""")>]
let private getRaw (url: string) (headers: (string * string) list) (ssl: SslOptions) : Result<int * string, string> = nativeOnly

[<Emit("""
(fun() ->
    Url = binary_to_list($0),
    Headers = [{binary_to_list(K), binary_to_list(V)} || {K, V} <- $1],
    ContentType = binary_to_list($2),
    ReqBody = $3,
    case httpc:request(post, {Url, Headers, ContentType, ReqBody}, $4, []) of
        {ok, {{_, StatusCode, _}, _RespHeaders, Body}} ->
            {ok, {StatusCode, erlang:list_to_binary(Body)}};
        {error, Reason} ->
            {error, erlang:list_to_binary(io_lib:format(<<"~p">>, [Reason]))}
    end
end)()
""")>]
let private postRaw (url: string) (headers: (string * string) list) (contentType: string) (body: string) (ssl: SslOptions) : Result<int * string, string> = nativeOnly

[<Emit("""
(fun() ->
    Url = binary_to_list($0),
    Headers = [{binary_to_list(K), binary_to_list(V)} || {K, V} <- $1],
    ContentType = binary_to_list($2),
    ReqBody = $3,
    case httpc:request(put, {Url, Headers, ContentType, ReqBody}, $4, []) of
        {ok, {{_, StatusCode, _}, _RespHeaders, Body}} ->
            {ok, {StatusCode, erlang:list_to_binary(Body)}};
        {error, Reason} ->
            {error, erlang:list_to_binary(io_lib:format(<<"~p">>, [Reason]))}
    end
end)()
""")>]
let private putRaw (url: string) (headers: (string * string) list) (contentType: string) (body: string) (ssl: SslOptions) : Result<int * string, string> = nativeOnly

[<Emit("""
(fun() ->
    Url = binary_to_list($0),
    Headers = [{binary_to_list(K), binary_to_list(V)} || {K, V} <- $1],
    case httpc:request(delete, {Url, Headers}, $2, []) of
        {ok, {{_, StatusCode, _}, _RespHeaders, Body}} ->
            {ok, {StatusCode, erlang:list_to_binary(Body)}};
        {error, Reason} ->
            {error, erlang:list_to_binary(io_lib:format(<<"~p">>, [Reason]))}
    end
end)()
""")>]
let private deleteRaw (url: string) (headers: (string * string) list) (ssl: SslOptions) : Result<int * string, string> = nativeOnly

let private toResponse (result: Result<int * string, string>) : Result<HttpResponse, string> =
    result |> Result.map (fun (code, body) -> { StatusCode = code; Body = body })

// ============================================================================
// Public API
// ============================================================================

/// Performs an HTTP GET request.
let get (url: string) (headers: (string * string) list) (ssl: SslOptions) : Result<HttpResponse, string> =
    getRaw url headers ssl |> toResponse

/// Performs an HTTP POST request with a content type and body.
let post (url: string) (headers: (string * string) list) (contentType: string) (body: string) (ssl: SslOptions) : Result<HttpResponse, string> =
    postRaw url headers contentType body ssl |> toResponse

/// Performs an HTTP PUT request with a content type and body.
let put (url: string) (headers: (string * string) list) (contentType: string) (body: string) (ssl: SslOptions) : Result<HttpResponse, string> =
    putRaw url headers contentType body ssl |> toResponse

/// Performs an HTTP DELETE request.
let delete (url: string) (headers: (string * string) list) (ssl: SslOptions) : Result<HttpResponse, string> =
    deleteRaw url headers ssl |> toResponse
