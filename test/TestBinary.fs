module Fable.Beam.Tests.Binary

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.Binary
open Fable.Beam.Lists

let tests =
    testList (
        "Binary",
        [ test (
              "copy makes a copy",
              fun _ ->
                  let b = "hello"
                  assertThat (copy b) (isEqualTo "hello")
          )

          test ("copy N times concatenates", fun _ -> assertThat (copyN "ab" 3) (isEqualTo "ababab"))

          test (
              "at returns byte at position",
              fun _ ->
                  // 'A' = 65, 'B' = 66
                  assertThat (at "AB" 0) (isEqualTo 65)
                  assertThat (at "AB" 1) (isEqualTo 66)
          )

          test (
              "first returns first byte",
              fun _ ->
                  // 'h' = 104
                  assertThat (first "hello") (isEqualTo 104)
          )

          test (
              "last returns last byte",
              fun _ ->
                  // 'o' = 111
                  assertThat (last "hello") (isEqualTo 111)
          )

          test ("part extracts subbinary", fun _ -> assertThat (part "hello world" 6 5) (isEqualTo "world"))

          test (
              "matchFirst returns Some on match",
              fun _ -> assertThat (matchFirst "hello" "ll") (isEqualTo (Some(2, 2)))
          )

          test (
              "matchFirst returns None when not found",
              fun _ -> assertThat (matchFirst "hello" "xyz") (isEqualTo None)
          )

          test (
              "matchAll returns all occurrences",
              fun _ ->
                  let results = matchAll "abcabc" "b"
                  assertThat (Array.length results) (isEqualTo 2)
          )

          test (
              "splitFirst splits on first occurrence",
              fun _ ->
                  let parts = splitFirst "hello world" " "
                  assertThat (Array.length parts) (isEqualTo 2)
                  assertThat (parts.[0]) (isEqualTo "hello")
                  assertThat (parts.[1]) (isEqualTo "world")
          )

          test (
              "splitAll splits on all occurrences",
              fun _ ->
                  let parts = splitAll "a,b,c" ","
                  assertThat (Array.length parts) (isEqualTo 3)
                  assertThat (parts.[0]) (isEqualTo "a")
                  assertThat (parts.[1]) (isEqualTo "b")
                  assertThat (parts.[2]) (isEqualTo "c")
          )

          test (
              "replaceFirst replaces first occurrence",
              fun _ -> assertThat (replaceFirst "aabbaa" "aa" "XX") (isEqualTo "XXbbaa")
          )

          test (
              "replaceAll replaces all occurrences",
              fun _ -> assertThat (replaceAll "aabbaa" "aa" "XX") (isEqualTo "XXbbXX")
          )

          test (
              "longest_common_prefix",
              fun _ ->
                  // "foo" is the longest prefix common to *all three* ("foobar"/"foobaz" share "fooba",
                  // but "fooqux" diverges at the 4th byte).
                  assertThat (longestCommonPrefix [ "foobar"; "foobaz"; "fooqux" ]) (isEqualTo 3)
          )

          test (
              "longest_common_suffix",
              fun _ -> assertThat (longestCommonSuffix [ "foobar"; "bazbar"; "quuxbar" ]) (isEqualTo 3)
          )

          test (
              "bin_to_list returns list of bytes",
              fun _ ->
                  // "ABC" = [65, 66, 67]
                  let bytes = toByteList "ABC"
                  assertThat (lists.nth (1, bytes)) (isEqualTo 65)
                  assertThat (lists.nth (2, bytes)) (isEqualTo 66)
                  assertThat (lists.nth (3, bytes)) (isEqualTo 67)
          )

          test (
              "list_to_bin converts bytes to binary",
              fun _ ->
                  // [104, 105] = "hi"
                  let bytes: BeamList<int> = emitErlExpr () "[104, 105]"
                  assertThat (ofByteList bytes) (isEqualTo "hi")
          )

          test (
              "bin_to_list and list_to_bin roundtrip",
              fun _ ->
                  let original = "hello"
                  let bytes = toByteList original
                  assertThat (ofByteList bytes) (isEqualTo original)
          )

          test (
              "encode_unsigned and decode_unsigned roundtrip",
              fun _ ->
                  let n = 12345
                  let encoded = encodeUnsigned n
                  assertThat (decodeUnsigned encoded) (isEqualTo n)
          )

          test (
              "encode_unsigned of zero roundtrips",
              fun _ ->
                  let encoded = encodeUnsigned 0
                  assertThat (decodeUnsigned encoded) (isEqualTo 0)
          )

          test (
              "decode_unsigned with little endian",
              fun _ ->
                  let little = Erlang.binaryToAtom "little"
                  let big = Erlang.binaryToAtom "big"
                  // Big-endian encoding of 256 is <<1, 0>>.
                  // Decoded as little-endian, those bytes read as 1.
                  let encoded_big = encodeUnsignedWithEndianness 256 big
                  assertThat (decodeUnsignedWithEndianness encoded_big little) (isEqualTo 1)
                  // Roundtrip via little endian preserves the value.
                  let encoded_little = encodeUnsignedWithEndianness 256 little
                  assertThat (decodeUnsignedWithEndianness encoded_little little) (isEqualTo 256)
          )

          test (
              "referenced_byte_size is at least the logical size",
              fun _ ->
                  // referenced_byte_size reports the size of the *underlying* memory a (sub-)binary points into,
                  // which OTP's own docs call "a hint for optimization, not exact": for a plain binary it varies
                  // with how the binary was constructed and the OTP release (5 on OTP 25, 40 on OTP 27, 256 for a
                  // shell literal). The only portable guarantee is that it references at least what it contains.
                  let s = "hello"
                  assertThat (referencedByteSize s >= Erlang.byteSize s) (isTrue)
                  assertThat (referencedByteSize "" >= 0) (isTrue)
          )

          test (
              "splitAllRaw returns the native list form of splitAll",
              fun _ ->
                  let parts: BeamList<string> = splitAllRaw "a-b-c" "-"
                  let expected: BeamList<string> = emitErlExpr () "[<<\"a\">>, <<\"b\">>, <<\"c\">>]"
                  assertThat parts (isEqualTo expected)
          )

          test (
              "splitFirstRaw returns the native list form of splitFirst",
              fun _ ->
                  let parts: BeamList<string> = splitFirstRaw "a-b-c" "-"
                  let expected: BeamList<string> = emitErlExpr () "[<<\"a\">>, <<\"b-c\">>]"
                  assertThat parts (isEqualTo expected)
          ) ]
    )
