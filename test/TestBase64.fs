module Fable.Beam.Tests.Base64

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop

module BBase64 = Fable.Beam.Base64

let tests =
    testList (
        "Base64",
        [ test (
              "encode produces non-empty string",
              fun _ ->
                  let encoded = BBase64.encode "hello"
                  assertThat (encoded.Length > 0) (isTrue)
          )

          test ("encode of hello", fun _ -> assertThat (BBase64.encode "hello") (isEqualTo "aGVsbG8="))

          test ("encode of empty string", fun _ -> assertThat (BBase64.encode "") (isEqualTo ""))

          test (
              "decode reverses encode",
              fun _ ->
                  let original = "hello world"
                  let encoded = BBase64.encode original
                  let decoded = BBase64.decode encoded
                  assertThat decoded (isEqualTo original)
          )

          test ("decode of known value", fun _ -> assertThat (BBase64.decode "aGVsbG8=") (isEqualTo "hello"))

          test (
              "encode decode roundtrip with binary data",
              fun _ ->
                  let data = "Fable.Beam rocks!"
                  let encoded = BBase64.encode data
                  let decoded = BBase64.decode encoded
                  assertThat decoded (isEqualTo data)
          )

          test (
              "mime_decode handles whitespace",
              fun _ ->
                  let encoded = BBase64.encode "hello"
                  let decoded = BBase64.mimeDecode encoded
                  assertThat decoded (isEqualTo "hello")
          )

          test (
              "tryDecode returns Some for valid base64",
              fun _ ->
                  let result = BBase64.tryDecode "aGVsbG8="
                  assertThat result (isEqualTo (Some "hello"))
          )

          test (
              "tryDecode returns None for invalid base64",
              fun _ ->
                  let result = BBase64.tryDecode "not!valid@base64#"
                  assertThat result (isEqualTo None)
          )

          test (
              "tryMimeDecode returns Some for valid input",
              fun _ ->
                  let result = BBase64.tryMimeDecode "aGVsbG8="
                  assertThat result (isEqualTo (Some "hello"))
          ) ]
    )
