module Fable.Beam.Tests.String

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.Lists

module BString = Fable.Beam.String

let tests =
    testList (
        "String",
        [ test ("isEmpty returns true for empty", fun _ -> assertThat (BString.isEmpty "") (isTrue))

          test ("isEmpty returns false for non-empty", fun _ -> assertThat (BString.isEmpty "hello") (isFalse))

          test ("length returns grapheme count", fun _ -> assertThat (BString.length "hello") (isEqualTo 5))

          test ("lowercase converts to lowercase", fun _ -> assertThat (BString.lowercase "HELLO") (isEqualTo "hello"))

          test ("uppercase converts to uppercase", fun _ -> assertThat (BString.uppercase "hello") (isEqualTo "HELLO"))

          test (
              "titlecase capitalises first grapheme",
              fun _ -> assertThat (BString.titlecase "hello world") (isEqualTo "Hello world")
          )

          test (
              "casefold lowercases for comparison",
              fun _ -> assertThat (BString.casefold "HELLO") (isEqualTo "hello")
          )

          test ("reverse reverses string", fun _ -> assertThat (BString.reverse "hello") (isEqualTo "olleh"))

          test ("trim strips whitespace", fun _ -> assertThat (BString.trim "  hello  ") (isEqualTo "hello"))

          test (
              "trimStart strips leading whitespace",
              fun _ -> assertThat (BString.trimStart "  hello  ") (isEqualTo "hello  ")
          )

          test (
              "trimEnd strips trailing whitespace",
              fun _ -> assertThat (BString.trimEnd "  hello  ") (isEqualTo "  hello")
          )

          test ("pad trailing to length", fun _ -> assertThat (BString.padEnd "hi" 5) (isEqualTo "hi   "))

          test ("padStart pads leading to length", fun _ -> assertThat (BString.padStart "hi" 5) (isEqualTo "   hi"))

          test (
              "padStartWith pads with custom character",
              fun _ -> assertThat (BString.padStartWith "7" 3 "0") (isEqualTo "007")
          )

          test (
              "padEndRaw returns unflattened chardata that flattens to padEnd",
              fun _ ->
                  let raw = BString.padEndRaw "hi" 5
                  // proves it is genuinely raw: string:pad yields an iolist ([<<"hi">>,32,32,32]), not a binary
                  assertThat (Erlang.isList raw) (isTrue)
                  assertThat (BeamChardata.toString raw) (isEqualTo "hi   ")
                  assertThat (BeamChardata.toString raw) (isEqualTo (BString.padEnd "hi" 5))
          )

          test (
              "padStartRaw returns unflattened chardata that flattens to padStart",
              fun _ ->
                  let raw = BString.padStartRaw "hi" 5
                  assertThat (Erlang.isList raw) (isTrue)
                  assertThat (BeamChardata.toString raw) (isEqualTo (BString.padStart "hi" 5))
          )

          test (
              "padBoth and padBothRaw pad both sides",
              fun _ ->
                  let raw = BString.padBothRaw "7" 5
                  assertThat (BString.padBoth "7" 5) (isEqualTo "  7  ")
                  assertThat (Erlang.isList raw) (isTrue)
                  assertThat (BeamChardata.toString raw) (isEqualTo (BString.padBoth "7" 5))
                  assertThat (BString.padBothWith "7" 5 "0") (isEqualTo "00700")
          )

          test (
              "reverseRaw flattens back to reverse",
              fun _ ->
                  let raw = BString.reverseRaw "hello"
                  assertThat (Erlang.isList raw) (isTrue)
                  assertThat (BeamChardata.toString raw) (isEqualTo "olleh")
          )

          test (
              "replaceAllRaw flattens back to replaceAll",
              fun _ ->
                  let raw = BString.replaceAllRaw "aXbXa" "X" "Y"
                  assertThat (BeamChardata.toString raw) (isEqualTo "aYbYa")
                  assertThat (BeamChardata.toString raw) (isEqualTo (BString.replaceAll "aXbXa" "X" "Y"))
          )

          test (
              "BeamChardata ofString roundtrips through toString",
              fun _ ->
                  let result = "hi" |> BeamChardata.ofString |> BeamChardata.toString
                  assertThat result (isEqualTo "hi")
          )

          test ("slice from position", fun _ -> assertThat (BString.slice "hello world" 6) (isEqualTo "world"))

          test ("sliceLen with length", fun _ -> assertThat (BString.sliceLen "hello world" 0 5) (isEqualTo "hello"))

          test (
              "equal compares strings",
              fun _ ->
                  assertThat (BString.equal "hello" "hello") (isTrue)
                  assertThat (BString.equal "hello" "world") (isFalse)
          )

          test (
              "equal case-insensitive",
              fun _ ->
                  assertThat (BString.equalCaseInsensitive "Hello" "hello") (isTrue)
                  assertThat (BString.equalCaseInsensitive "Hello" "world") (isFalse)
          )

          test (
              "find returns Some on match",
              fun _ -> assertThat (BString.find "hello world" "world") (isEqualTo (Some "world"))
          )

          test (
              "find returns None when not found",
              fun _ -> assertThat (BString.find "hello world" "xyz") (isEqualTo None)
          )

          test (
              "findLast finds last occurrence",
              fun _ -> assertThat (BString.findLast "a-b-c" "-") (isEqualTo (Some "-c"))
          )

          test (
              "prefix returns Some rest when prefix matches",
              fun _ -> assertThat (BString.prefix "hello world" "hello ") (isEqualTo (Some "world"))
          )

          test (
              "prefix returns None when no match",
              fun _ -> assertThat (BString.prefix "hello world" "xyz") (isEqualTo None)
          )

          test (
              "splitFirst splits at first occurrence",
              fun _ ->
                  let parts = BString.splitFirst "hello world" " "
                  assertThat (Array.length parts) (isEqualTo 2)
                  assertThat (parts.[0]) (isEqualTo "hello")
                  assertThat (parts.[1]) (isEqualTo "world")
          )

          test (
              "splitAll splits at all occurrences",
              fun _ ->
                  let parts = BString.splitAll "a,b,c" ","
                  assertThat (Array.length parts) (isEqualTo 3)
                  assertThat (parts.[0]) (isEqualTo "a")
                  assertThat (parts.[1]) (isEqualTo "b")
                  assertThat (parts.[2]) (isEqualTo "c")
          )

          test (
              "replaceFirst replaces first occurrence",
              fun _ -> assertThat (BString.replaceFirst "aabbaa" "aa" "XX") (isEqualTo "XXbbaa")
          )

          test (
              "replaceAll replaces all occurrences",
              fun _ -> assertThat (BString.replaceAll "aabbaa" "aa" "XX") (isEqualTo "XXbbXX")
          )

          test (
              "toInteger parses valid integer",
              fun _ ->
                  match BString.toInteger "42abc" with
                  | Ok(n, rest) ->
                      assertThat n (isEqualTo 42)
                      assertThat rest (isEqualTo "abc")
                  | Error _ -> assertThat false (isTrue)
          )

          test (
              "toInteger returns error for non-integer",
              fun _ ->
                  match BString.toInteger "abc" with
                  | Error _ -> assertThat true (isTrue)
                  | Ok _ -> assertThat false (isTrue)
          )

          test (
              "toFloat parses valid float",
              fun _ ->
                  match BString.toFloat "3.14rest" with
                  | Ok(f, _) -> assertThat ((f > 3.13 && f < 3.15)) (isTrue)
                  | Error _ -> assertThat false (isTrue)
          )

          test (
              "toGraphemes splits into grapheme clusters",
              fun _ ->
                  let graphemes = BString.toGraphemes "abc"
                  assertThat (Array.length graphemes) (isEqualTo 3)
                  assertThat (graphemes.[0]) (isEqualTo "a")
                  assertThat (graphemes.[1]) (isEqualTo "b")
                  assertThat (graphemes.[2]) (isEqualTo "c")
          )

          test (
              "splitAllRaw returns the native list form of splitAll",
              fun _ ->
                  let parts: BeamList<string> = BString.splitAllRaw "a,b,c" ","
                  let expected: BeamList<string> = emitErlExpr () "[<<\"a\">>, <<\"b\">>, <<\"c\">>]"
                  assertThat parts (isEqualTo expected)
          )

          test (
              "splitFirstRaw returns the native list form of splitFirst",
              fun _ ->
                  let parts: BeamList<string> = BString.splitFirstRaw "hello world" " "
                  let expected: BeamList<string> = emitErlExpr () "[<<\"hello\">>, <<\"world\">>]"
                  assertThat parts (isEqualTo expected)
          ) ]
    )
