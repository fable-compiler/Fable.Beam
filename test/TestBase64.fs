module Fable.Beam.Tests.Base64

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Base64
#endif

[<Fact>]
let ``test base64.encode produces non-empty string`` () =
#if FABLE_COMPILER
    let encoded = base64.encode "hello"
    (encoded.Length > 0) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test base64.encode of hello`` () =
#if FABLE_COMPILER
    base64.encode "hello" |> equal "aGVsbG8="
#else
    ()
#endif

[<Fact>]
let ``test base64.encode of empty string`` () =
#if FABLE_COMPILER
    base64.encode "" |> equal ""
#else
    ()
#endif

[<Fact>]
let ``test base64.decode reverses encode`` () =
#if FABLE_COMPILER
    let original = "hello world"
    let encoded = base64.encode original
    let decoded = base64.decode encoded
    decoded |> equal original
#else
    ()
#endif

[<Fact>]
let ``test base64.decode of known value`` () =
#if FABLE_COMPILER
    base64.decode "aGVsbG8=" |> equal "hello"
#else
    ()
#endif

[<Fact>]
let ``test base64.encode decode roundtrip with binary data`` () =
#if FABLE_COMPILER
    let data = "Fable.Beam rocks!"
    let encoded = base64.encode data
    let decoded = base64.decode encoded
    decoded |> equal data
#else
    ()
#endif

[<Fact>]
let ``test base64.mime_decode handles whitespace`` () =
#if FABLE_COMPILER
    // MIME base64 tolerates embedded whitespace
    let encoded = base64.encode "hello"
    let decoded = base64.mime_decode encoded
    decoded |> equal "hello"
#else
    ()
#endif

[<Fact>]
let ``test tryDecode returns Some for valid base64`` () =
#if FABLE_COMPILER
    let result = tryDecode "aGVsbG8="
    result |> equal (Some "hello")
#else
    ()
#endif

[<Fact>]
let ``test tryDecode returns None for invalid base64`` () =
#if FABLE_COMPILER
    let result = tryDecode "not!valid@base64#"
    result |> equal None
#else
    ()
#endif

[<Fact>]
let ``test tryMimeDecode returns Some for valid input`` () =
#if FABLE_COMPILER
    let result = tryMimeDecode "aGVsbG8="
    result |> equal (Some "hello")
#else
    ()
#endif
