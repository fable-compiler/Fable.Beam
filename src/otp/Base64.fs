/// Type bindings for Erlang base64 module
/// See https://www.erlang.org/doc/apps/stdlib/base64
module Fable.Beam.Base64

open Fable.Core

/// Encodes Data as a base64 binary. Data can be a binary or a charlist.
[<Emit("base64:encode($0)")>]
let encode (data: string) : string = nativeOnly

/// Decodes a base64-encoded binary. Raises badarg if the input is not valid base64.
[<Emit("base64:decode($0)")>]
let decode (base64: string) : string = nativeOnly

/// MIME-compatible decode: decodes a base64 binary, silently ignoring illegal characters
/// (whitespace, line breaks, etc.) that are valid in MIME base64.
[<Emit("base64:mime_decode($0)")>]
let mimeDecode (base64: string) : string = nativeOnly

// ============================================================================
// Typed helpers — wrapping results for safe use from F#
// ============================================================================

/// Decodes a base64-encoded binary. Returns Some decoded binary, or None if
/// the input contains invalid base64 characters.
[<Emit("(fun() -> try base64:decode($0) of Base64DecodeVal__ -> Base64DecodeVal__ catch error:_ -> undefined end end)()")>]
let tryDecode (base64: string) : string option = nativeOnly

/// MIME-compatible decode. Returns Some decoded binary, or None on unexpected error.
[<Emit("(fun() -> try base64:mime_decode($0) of Base64MimeDecodeVal__ -> Base64MimeDecodeVal__ catch error:_ -> undefined end end)()")>]
let tryMimeDecode (base64: string) : string option = nativeOnly
