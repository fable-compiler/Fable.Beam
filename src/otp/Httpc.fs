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
// Public API
// ============================================================================
// Emit expressions are wrapped in (fun() -> ... end)() to scope internal
// variables. Internal variables use __0 suffix to avoid collisions with
// caller-scope variables (Erlang's = is pattern matching, not rebinding).
//
// All public functions are direct Emit bindings (not wrappers around private
// functions) because Fable compiles cross-module non-Emit calls as Erlang
// module calls (e.g. httpc:get/3), which don't exist in Erlang's httpc module.

/// Performs an HTTP GET request.
[<Emit("""
(fun() ->
    Url__0 = binary_to_list($0),
    Headers__0 = [{binary_to_list(K__0), binary_to_list(V__0)} || {K__0, V__0} <- $1],
    case httpc:request(get, {Url__0, Headers__0}, $2, []) of
        {ok, {{_, StatusCode__0, _}, _RespHeaders__0, Body__0}} ->
            {ok, #{status_code => StatusCode__0, body => erlang:list_to_binary(Body__0)}};
        {error, Reason__0} ->
            {error, erlang:list_to_binary(io_lib:format(<<"~p">>, [Reason__0]))}
    end
end)()
""")>]
let get (url: string) (headers: (string * string) list) (ssl: SslOptions) : Result<HttpResponse, string> = nativeOnly

/// Performs an HTTP POST request with a content type and body.
[<Emit("""
(fun() ->
    Url__0 = binary_to_list($0),
    Headers__0 = [{binary_to_list(K__0), binary_to_list(V__0)} || {K__0, V__0} <- $1],
    ContentType__0 = binary_to_list($2),
    ReqBody__0 = $3,
    case httpc:request(post, {Url__0, Headers__0, ContentType__0, ReqBody__0}, $4, []) of
        {ok, {{_, StatusCode__0, _}, _RespHeaders__0, Body__0}} ->
            {ok, #{status_code => StatusCode__0, body => erlang:list_to_binary(Body__0)}};
        {error, Reason__0} ->
            {error, erlang:list_to_binary(io_lib:format(<<"~p">>, [Reason__0]))}
    end
end)()
""")>]
let post (url: string) (headers: (string * string) list) (contentType: string) (body: string) (ssl: SslOptions) : Result<HttpResponse, string> = nativeOnly

/// Performs an HTTP PUT request with a content type and body.
[<Emit("""
(fun() ->
    Url__0 = binary_to_list($0),
    Headers__0 = [{binary_to_list(K__0), binary_to_list(V__0)} || {K__0, V__0} <- $1],
    ContentType__0 = binary_to_list($2),
    ReqBody__0 = $3,
    case httpc:request(put, {Url__0, Headers__0, ContentType__0, ReqBody__0}, $4, []) of
        {ok, {{_, StatusCode__0, _}, _RespHeaders__0, Body__0}} ->
            {ok, #{status_code => StatusCode__0, body => erlang:list_to_binary(Body__0)}};
        {error, Reason__0} ->
            {error, erlang:list_to_binary(io_lib:format(<<"~p">>, [Reason__0]))}
    end
end)()
""")>]
let put (url: string) (headers: (string * string) list) (contentType: string) (body: string) (ssl: SslOptions) : Result<HttpResponse, string> = nativeOnly

/// Performs an HTTP DELETE request.
[<Emit("""
(fun() ->
    Url__0 = binary_to_list($0),
    Headers__0 = [{binary_to_list(K__0), binary_to_list(V__0)} || {K__0, V__0} <- $1],
    case httpc:request(delete, {Url__0, Headers__0}, $2, []) of
        {ok, {{_, StatusCode__0, _}, _RespHeaders__0, Body__0}} ->
            {ok, #{status_code => StatusCode__0, body => erlang:list_to_binary(Body__0)}};
        {error, Reason__0} ->
            {error, erlang:list_to_binary(io_lib:format(<<"~p">>, [Reason__0]))}
    end
end)()
""")>]
let delete (url: string) (headers: (string * string) list) (ssl: SslOptions) : Result<HttpResponse, string> = nativeOnly

// ============================================================================
// Timeout-aware API
// ============================================================================
// Same as above but with an explicit timeout in milliseconds.
// Maps to httpc:request HttpOptions [{timeout, Ms} | SslOpts].
// On timeout, httpc returns {error, timeout} which we format as an error string.

/// Performs an HTTP GET request with a timeout in milliseconds.
[<Emit("""
(fun() ->
    Url__0 = binary_to_list($0),
    Headers__0 = [{binary_to_list(K__0), binary_to_list(V__0)} || {K__0, V__0} <- $1],
    HttpOpts__0 = [{timeout, $2} | $3],
    case httpc:request(get, {Url__0, Headers__0}, HttpOpts__0, []) of
        {ok, {{_, StatusCode__0, _}, _RespHeaders__0, Body__0}} ->
            {ok, #{status_code => StatusCode__0, body => erlang:list_to_binary(Body__0)}};
        {error, Reason__0} ->
            {error, erlang:list_to_binary(io_lib:format(<<"~p">>, [Reason__0]))}
    end
end)()
""")>]
let getWithTimeout (url: string) (headers: (string * string) list) (timeoutMs: int) (ssl: SslOptions) : Result<HttpResponse, string> = nativeOnly

/// Performs an HTTP POST request with a timeout in milliseconds.
[<Emit("""
(fun() ->
    Url__0 = binary_to_list($0),
    Headers__0 = [{binary_to_list(K__0), binary_to_list(V__0)} || {K__0, V__0} <- $1],
    ContentType__0 = binary_to_list($2),
    ReqBody__0 = $3,
    HttpOpts__0 = [{timeout, $4} | $5],
    case httpc:request(post, {Url__0, Headers__0, ContentType__0, ReqBody__0}, HttpOpts__0, []) of
        {ok, {{_, StatusCode__0, _}, _RespHeaders__0, Body__0}} ->
            {ok, #{status_code => StatusCode__0, body => erlang:list_to_binary(Body__0)}};
        {error, Reason__0} ->
            {error, erlang:list_to_binary(io_lib:format(<<"~p">>, [Reason__0]))}
    end
end)()
""")>]
let postWithTimeout (url: string) (headers: (string * string) list) (contentType: string) (body: string) (timeoutMs: int) (ssl: SslOptions) : Result<HttpResponse, string> = nativeOnly

/// Performs an HTTP PUT request with a timeout in milliseconds.
[<Emit("""
(fun() ->
    Url__0 = binary_to_list($0),
    Headers__0 = [{binary_to_list(K__0), binary_to_list(V__0)} || {K__0, V__0} <- $1],
    ContentType__0 = binary_to_list($2),
    ReqBody__0 = $3,
    HttpOpts__0 = [{timeout, $4} | $5],
    case httpc:request(put, {Url__0, Headers__0, ContentType__0, ReqBody__0}, HttpOpts__0, []) of
        {ok, {{_, StatusCode__0, _}, _RespHeaders__0, Body__0}} ->
            {ok, #{status_code => StatusCode__0, body => erlang:list_to_binary(Body__0)}};
        {error, Reason__0} ->
            {error, erlang:list_to_binary(io_lib:format(<<"~p">>, [Reason__0]))}
    end
end)()
""")>]
let putWithTimeout (url: string) (headers: (string * string) list) (contentType: string) (body: string) (timeoutMs: int) (ssl: SslOptions) : Result<HttpResponse, string> = nativeOnly

/// Performs an HTTP DELETE request with a timeout in milliseconds.
[<Emit("""
(fun() ->
    Url__0 = binary_to_list($0),
    Headers__0 = [{binary_to_list(K__0), binary_to_list(V__0)} || {K__0, V__0} <- $1],
    HttpOpts__0 = [{timeout, $2} | $3],
    case httpc:request(delete, {Url__0, Headers__0}, HttpOpts__0, []) of
        {ok, {{_, StatusCode__0, _}, _RespHeaders__0, Body__0}} ->
            {ok, #{status_code => StatusCode__0, body => erlang:list_to_binary(Body__0)}};
        {error, Reason__0} ->
            {error, erlang:list_to_binary(io_lib:format(<<"~p">>, [Reason__0]))}
    end
end)()
""")>]
let deleteWithTimeout (url: string) (headers: (string * string) list) (timeoutMs: int) (ssl: SslOptions) : Result<HttpResponse, string> = nativeOnly
