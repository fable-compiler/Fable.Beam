module Fable.Beam.Tests.String

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.String
open Fable.Beam.Lists

#if FABLE_COMPILER
// The raw variants return unflattened chardata: an iolist/charlist, i.e. a *list*, never a binary.
[<Emit("is_list($0)")>]
let private isList (x: BeamChardata) : bool = nativeOnly
#endif

let tests =
    testList (
        "String",
        [ test ("is_empty returns true for empty", fun _ ->
                  assertThat (str.is_empty "") (isTrue))

          test ("is_empty returns false for non-empty", fun _ ->
                  assertThat (str.is_empty "hello") (isFalse))

          test ("length returns grapheme count", fun _ ->
                  assertThat (str.length "hello") (isEqualTo 5))

          test ("lowercase converts to lowercase", fun _ ->
                  assertThat (str.lowercase "HELLO") (isEqualTo "hello"))

          test ("uppercase converts to uppercase", fun _ ->
                  assertThat (str.uppercase "hello") (isEqualTo "HELLO"))

          test ("titlecase capitalises first grapheme", fun _ ->
                  assertThat (str.titlecase "hello world") (isEqualTo "Hello world"))

          test ("casefold lowercases for comparison", fun _ ->
                  assertThat (str.casefold "HELLO") (isEqualTo "hello"))

          test ("reverse reverses string", fun _ ->
                  assertThat (reverse "hello") (isEqualTo "olleh"))

          test ("trim strips whitespace", fun _ ->
                  assertThat (str.trim "  hello  ") (isEqualTo "hello"))

          test ("trim with leading direction", fun _ ->
                  let leading = Erlang.binaryToAtom "leading"
                  assertThat (str.trim ("  hello  ", leading)) (isEqualTo "hello  ")
                  )

          test ("trim with trailing direction", fun _ ->
                  let trailing = Erlang.binaryToAtom "trailing"
                  assertThat (str.trim ("  hello  ", trailing)) (isEqualTo "  hello")
                  )

          test ("pad trailing to length", fun _ ->
                  assertThat (pad "hi" 5) (isEqualTo "hi   "))

          test ("pad leading with direction", fun _ ->
                  let leading = Erlang.binaryToAtom "leading"
                  assertThat (padDir "hi" 5 leading) (isEqualTo "   hi"))

          test ("pad with custom character", fun _ ->
                  let leading = Erlang.binaryToAtom "leading"
                  assertThat (padWith "7" 3 leading "0") (isEqualTo "007"))

          test ("padRaw returns unflattened chardata that flattens to pad", fun _ ->
                  let raw = padRaw "hi" 5
                  // proves it is genuinely raw: string:pad yields an iolist ([<<"hi">>,32,32,32]), not a binary
                  assertThat (isList raw) (isTrue)
                  assertThat (BeamChardata.toString raw) (isEqualTo "hi   ")
                  assertThat (BeamChardata.toString raw) (isEqualTo (pad "hi" 5))
                  )

          test ("reverseRaw flattens back to reverse", fun _ ->
                  let raw = reverseRaw "hello"
                  assertThat (isList raw) (isTrue)
                  assertThat (BeamChardata.toString raw) (isEqualTo "olleh")
                  )

          test ("replaceAllRaw flattens back to replaceAll", fun _ ->
                  let raw = replaceAllRaw "aXbXa" "X" "Y"
                  assertThat (BeamChardata.toString raw) (isEqualTo "aYbYa")
                  assertThat (BeamChardata.toString raw) (isEqualTo (replaceAll "aXbXa" "X" "Y"))
                  )

          test ("BeamChardata ofString roundtrips through toString", fun _ ->
                  let result = "hi" |> BeamChardata.ofString |> BeamChardata.toString
                  assertThat result (isEqualTo "hi"))

          test ("slice from position", fun _ ->
                  assertThat (str.slice ("hello world", 6)) (isEqualTo "world"))

          test ("slice with length", fun _ ->
                  assertThat (str.slice ("hello world", 0, 5)) (isEqualTo "hello"))

          test ("equal compares strings", fun _ ->
                  assertThat (str.equal ("hello", "hello")) (isTrue)
                  assertThat (str.equal ("hello", "world")) (isFalse)
                  )

          test ("equal case-insensitive", fun _ ->
                  assertThat (str.equal ("Hello", "hello", true)) (isTrue)
                  assertThat (str.equal ("Hello", "world", true)) (isFalse)
                  )

          test ("find returns Some on match", fun _ ->
                  assertThat (find "hello world" "world") (isEqualTo (Some "world")))

          test ("find returns None when not found", fun _ ->
                  assertThat (find "hello world" "xyz") (isEqualTo None))

          test ("findFrom trailing finds last occurrence", fun _ ->
                  let trailing = Erlang.binaryToAtom "trailing"
                  assertThat (findFrom "a-b-c" "-" trailing) (isEqualTo (Some "-c")))

          test ("prefix returns Some rest when prefix matches", fun _ ->
                  assertThat (prefix "hello world" "hello ") (isEqualTo (Some "world")))

          test ("prefix returns None when no match", fun _ ->
                  assertThat (prefix "hello world" "xyz") (isEqualTo None))

          test ("splitFirst splits at first occurrence", fun _ ->
                  let parts = splitFirst "hello world" " "
                  assertThat (Array.length parts) (isEqualTo 2)
                  assertThat (parts.[0]) (isEqualTo "hello")
                  assertThat (parts.[1]) (isEqualTo "world")
                  )

          test ("splitAll splits at all occurrences", fun _ ->
                  let parts = splitAll "a,b,c" ","
                  assertThat (Array.length parts) (isEqualTo 3)
                  assertThat (parts.[0]) (isEqualTo "a")
                  assertThat (parts.[1]) (isEqualTo "b")
                  assertThat (parts.[2]) (isEqualTo "c")
                  )

          test ("replaceFirst replaces first occurrence", fun _ ->
                  assertThat (replaceFirst "aabbaa" "aa" "XX") (isEqualTo "XXbbaa"))

          test ("replaceAll replaces all occurrences", fun _ ->
                  assertThat (replaceAll "aabbaa" "aa" "XX") (isEqualTo "XXbbXX"))

          test ("toInteger parses valid integer", fun _ ->
                  match toInteger "42abc" with
                  | Ok (n, rest) ->
                      assertThat n (isEqualTo 42)
                      assertThat rest (isEqualTo "abc")
                  | Error _ -> assertThat false (isTrue))

          test ("toInteger returns error for non-integer", fun _ ->
                  match toInteger "abc" with
                  | Error _ -> assertThat true (isTrue)
                  | Ok _ -> assertThat false (isTrue))

          test ("toFloat parses valid float", fun _ ->
                  match toFloat "3.14rest" with
                  | Ok (f, _) -> assertThat ((f > 3.13 && f < 3.15)) (isTrue)
                  | Error _ -> assertThat false (isTrue))

          test ("toGraphemes splits into grapheme clusters", fun _ ->
                  let graphemes = toGraphemes "abc"
                  assertThat (Array.length graphemes) (isEqualTo 3)
                  assertThat (graphemes.[0]) (isEqualTo "a")
                  assertThat (graphemes.[1]) (isEqualTo "b")
                  assertThat (graphemes.[2]) (isEqualTo "c")
                  )

          test ("splitAllRaw returns the native list form of splitAll", fun _ ->
                  let parts: BeamList<string> = splitAllRaw "a,b,c" ","
                  let expected: BeamList<string> = emitErlExpr () "[<<\"a\">>, <<\"b\">>, <<\"c\">>]"
                  assertThat parts (isEqualTo expected))

          test ("splitFirstRaw returns the native list form of splitFirst", fun _ ->
                  let parts: BeamList<string> = splitFirstRaw "hello world" " "
                  let expected: BeamList<string> = emitErlExpr () "[<<\"hello\">>, <<\"world\">>]"
                  assertThat parts (isEqualTo expected)) ]
    )
