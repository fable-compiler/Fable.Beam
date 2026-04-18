module Fable.Beam.Tests.String

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.String
#endif

[<Fact>]
let ``test string.is_empty returns true for empty`` () =
#if FABLE_COMPILER
    string.is_empty "" |> equal true
#else
    ()
#endif

[<Fact>]
let ``test string.is_empty returns false for non-empty`` () =
#if FABLE_COMPILER
    string.is_empty "hello" |> equal false
#else
    ()
#endif

[<Fact>]
let ``test string.length returns grapheme count`` () =
#if FABLE_COMPILER
    string.length "hello" |> equal 5
#else
    ()
#endif

[<Fact>]
let ``test string.lowercase converts to lowercase`` () =
#if FABLE_COMPILER
    string.lowercase "HELLO" |> equal "hello"
#else
    ()
#endif

[<Fact>]
let ``test string.uppercase converts to uppercase`` () =
#if FABLE_COMPILER
    string.uppercase "hello" |> equal "HELLO"
#else
    ()
#endif

[<Fact>]
let ``test string.reverse reverses string`` () =
#if FABLE_COMPILER
    string.reverse "hello" |> equal "olleh"
#else
    ()
#endif

[<Fact>]
let ``test string.trim strips whitespace`` () =
#if FABLE_COMPILER
    string.trim "  hello  " |> equal "hello"
#else
    ()
#endif

[<Fact>]
let ``test string.slice from position`` () =
#if FABLE_COMPILER
    string.slice ("hello world", 6) |> equal "world"
#else
    ()
#endif

[<Fact>]
let ``test string.slice with length`` () =
#if FABLE_COMPILER
    string.slice ("hello world", 0, 5) |> equal "hello"
#else
    ()
#endif

[<Fact>]
let ``test string.equal compares strings`` () =
#if FABLE_COMPILER
    string.equal ("hello", "hello") |> equal true
    string.equal ("hello", "world") |> equal false
#else
    ()
#endif

[<Fact>]
let ``test string.equal case-insensitive`` () =
#if FABLE_COMPILER
    string.equal ("Hello", "hello", true) |> equal true
    string.equal ("Hello", "world", true) |> equal false
#else
    ()
#endif

[<Fact>]
let ``test find returns Some on match`` () =
#if FABLE_COMPILER
    find "hello world" "world" |> equal (Some "world")
#else
    ()
#endif

[<Fact>]
let ``test find returns None when not found`` () =
#if FABLE_COMPILER
    find "hello world" "xyz" |> equal None
#else
    ()
#endif

[<Fact>]
let ``test prefix returns Some rest when prefix matches`` () =
#if FABLE_COMPILER
    prefix "hello world" "hello " |> equal (Some "world")
#else
    ()
#endif

[<Fact>]
let ``test prefix returns None when no match`` () =
#if FABLE_COMPILER
    prefix "hello world" "xyz" |> equal None
#else
    ()
#endif

[<Fact>]
let ``test splitFirst splits at first occurrence`` () =
#if FABLE_COMPILER
    let parts = splitFirst "hello world" " "
    Array.length parts |> equal 2
    parts.[0] |> equal "hello"
    parts.[1] |> equal "world"
#else
    ()
#endif

[<Fact>]
let ``test splitAll splits at all occurrences`` () =
#if FABLE_COMPILER
    let parts = splitAll "a,b,c" ","
    Array.length parts |> equal 3
    parts.[0] |> equal "a"
    parts.[1] |> equal "b"
    parts.[2] |> equal "c"
#else
    ()
#endif

[<Fact>]
let ``test replaceFirst replaces first occurrence`` () =
#if FABLE_COMPILER
    replaceFirst "aabbaa" "aa" "XX" |> equal "XXbbaa"
#else
    ()
#endif

[<Fact>]
let ``test replaceAll replaces all occurrences`` () =
#if FABLE_COMPILER
    replaceAll "aabbaa" "aa" "XX" |> equal "XXbbXX"
#else
    ()
#endif

[<Fact>]
let ``test toInteger parses valid integer`` () =
#if FABLE_COMPILER
    match toInteger "42abc" with
    | Ok (n, rest) ->
        n |> equal 42
        rest |> equal "abc"
    | Error _ -> equal true false
#else
    ()
#endif

[<Fact>]
let ``test toInteger returns error for non-integer`` () =
#if FABLE_COMPILER
    match toInteger "abc" with
    | Error _ -> equal true true
    | Ok _ -> equal true false
#else
    ()
#endif

[<Fact>]
let ``test toFloat parses valid float`` () =
#if FABLE_COMPILER
    match toFloat "3.14rest" with
    | Ok (f, _) ->
        (f > 3.13 && f < 3.15) |> equal true
    | Error _ -> equal true false
#else
    ()
#endif
