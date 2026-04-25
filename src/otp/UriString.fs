/// Type bindings for Erlang uri_string module (URI parsing and manipulation)
/// See https://www.erlang.org/doc/apps/stdlib/uri_string
///
/// Available since OTP 21. All functions accept and return binary strings, which
/// is what F# strings compile to on BEAM — no charlist conversion is needed.
/// `quote/1`, `quote/2`, and `unquote/1` require OTP 24+.
module Fable.Beam.UriString

open Fable.Core
open Fable.Beam

// ============================================================================
// Types
// ============================================================================

/// Opaque URI component map returned by UriString.parse.
/// Use the accessor functions (scheme, host, path, etc.) to extract fields.
[<Erase>]
type UriMap = UriMap of obj

// ============================================================================
// Parsing
// ============================================================================

/// Parses a URI string into its components.
/// Returns Ok UriMap on success; Error "invalid_uri" if the URI is malformed.
[<Emit("(fun() -> UriParseR__ = uri_string:parse($0), case UriParseR__ of {error, _, _} -> {error, <<\"invalid_uri\">>}; _ -> {ok, UriParseR__} end end)()")>]
let parse (uri: string) : Result<UriMap, string> = nativeOnly

/// Returns the scheme component of a parsed URI (e.g. "https", "ftp").
[<Emit("maps:get(scheme, $0, undefined)")>]
let scheme (m: UriMap) : string option = nativeOnly

/// Returns the userinfo component of a parsed URI (e.g. "user:password").
[<Emit("maps:get(userinfo, $0, undefined)")>]
let userinfo (m: UriMap) : string option = nativeOnly

/// Returns the host component of a parsed URI (e.g. "example.com").
[<Emit("maps:get(host, $0, undefined)")>]
let host (m: UriMap) : string option = nativeOnly

/// Returns the port component of a parsed URI as an integer.
[<Emit("maps:get(port, $0, undefined)")>]
let port (m: UriMap) : int option = nativeOnly

/// Returns the path component of a parsed URI (e.g. "/api/v1").
[<Emit("maps:get(path, $0, undefined)")>]
let path (m: UriMap) : string option = nativeOnly

/// Returns the query component of a parsed URI without the leading '?' (e.g. "q=hello&lang=en").
[<Emit("maps:get(query, $0, undefined)")>]
let query (m: UriMap) : string option = nativeOnly

/// Returns the fragment component of a parsed URI without the leading '#' (e.g. "section-1").
[<Emit("maps:get(fragment, $0, undefined)")>]
let fragment (m: UriMap) : string option = nativeOnly

// ============================================================================
// Normalization and resolution
// ============================================================================

/// Normalizes a URI per RFC 3986: lowercases scheme and host, removes default ports,
/// normalizes path segments (removes . and ..), normalizes percent-encoding.
/// Returns Ok normalized URI string; Error "invalid_uri" on malformed input.
[<Emit("(fun() -> UriNormR__ = uri_string:normalize($0), case UriNormR__ of {error, _, _} -> {error, <<\"invalid_uri\">>}; _ -> {ok, UriNormR__} end end)()")>]
let normalize (uri: string) : Result<string, string> = nativeOnly

/// Resolves a URI reference against a base URI per RFC 3986.
/// E.g. resolve "/new" "https://example.com/old" → Ok "https://example.com/new"
/// Returns Ok resolved URI string; Error "invalid_uri" on invalid input.
[<Emit("(fun() -> UriResolveR__ = uri_string:resolve($0, $1), case UriResolveR__ of {error, _, _} -> {error, <<\"invalid_uri\">>}; _ -> {ok, UriResolveR__} end end)()")>]
let resolve (reference: string) (base': string) : Result<string, string> = nativeOnly

// ============================================================================
// Query string handling
// ============================================================================

/// Parses a URI query string into a list of (key, value) pairs.
/// Percent-encoded characters (and '+' for spaces) in keys and values are decoded.
/// Note: query keys without values (e.g. "flag") are returned with the atom `true`
/// as the value, which cannot be represented as a string — avoid such query strings
/// when using this binding.
///
/// E.g. dissectQuery "q=hello&lang=en" → [("q", "hello"); ("lang", "en")]
[<Emit("uri_string:dissect_query($0)")>]
let dissectQuery (queryStr: string) : (string * string) list = nativeOnly

/// Composes a URI query string from a list of (key, value) pairs.
/// Special characters in keys and values are percent-encoded; spaces are encoded as '+'.
///
/// E.g. composeQuery [("q", "hello world"); ("lang", "en")] → "q=hello+world&lang=en"
[<Emit("uri_string:compose_query($0)")>]
let composeQuery (pairs: (string * string) list) : string = nativeOnly

// ============================================================================
// Percent encoding / decoding
// ============================================================================

/// Percent-decodes a URI string: converts every %XX sequence to its byte value.
/// Returns Ok decoded string; Error "invalid_encoding" if any %XX sequence is malformed.
[<Emit("(fun() -> case uri_string:percent_decode($0) of {error, {invalid, _}} -> {error, <<\"invalid_encoding\">>}; UriPctDecR__ -> {ok, UriPctDecR__} end end)()")>]
let percentDecode (uri: string) : Result<string, string> = nativeOnly

/// Percent-encodes Data, preserving only the unreserved characters (A–Z, a–z, 0–9, -, _, ., ~).
/// All other bytes are encoded as %XX.
/// E.g. quote "hello world/path" → "hello%20world%2Fpath"
///
/// Requires OTP 24+.
[<Emit("uri_string:quote($0)")>]
let quote (data: string) : string = nativeOnly

/// Percent-encodes Data like quote/1 but also preserves any characters in SafeChars.
/// E.g. quoteWith "hello/world" "/" → "hello/world"   (/ is not encoded)
///
/// Requires OTP 24+.
[<Emit("uri_string:quote($0, $1)")>]
let quoteWith (data: string) (safeChars: string) : string = nativeOnly

/// Percent-decodes a string like percentDecode but never fails — malformed
/// sequences are passed through as-is.
///
/// Requires OTP 24+.
[<Emit("uri_string:unquote($0)")>]
let unquote (data: string) : string = nativeOnly
