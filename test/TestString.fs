module Fable.Beam.Tests.String

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.String
#endif

[<Fact>]
let ``test str.is_empty returns true for empty`` () =
#if FABLE_COMPILER
    str.is_empty "" |> equal true
#else
    ()
#endif

[<Fact>]
let ``test str.is_empty returns false for non-empty`` () =
#if FABLE_COMPILER
    str.is_empty "hello" |> equal false
#else
    ()
#endif

[<Fact>]
let ``test str.length returns grapheme count`` () =
#if FABLE_COMPILER
    str.length "hello" |> equal 5
#else
    ()
#endif

[<Fact>]
let ``test str.lowercase converts to lowercase`` () =
#if FABLE_COMPILER
    str.lowercase "HELLO" |> equal "hello"
#else
    ()
#endif

[<Fact>]
let ``test str.uppercase converts to uppercase`` () =
#if FABLE_COMPILER
    str.uppercase "hello" |> equal "HELLO"
#else
    ()
#endif

[<Fact>]
let ``test str.titlecase capitalises first grapheme`` () =
#if FABLE_COMPILER
    str.titlecase "hello world" |> equal "Hello world"
#else
    ()
#endif

[<Fact>]
let ``test str.casefold lowercases for comparison`` () =
#if FABLE_COMPILER
    str.casefold "HELLO" |> equal "hello"
#else
    ()
#endif

[<Fact>]
let ``test str.reverse reverses string`` () =
#if FABLE_COMPILER
    str.reverse "hello" |> equal "olleh"
#else
    ()
#endif

[<Fact>]
let ``test str.trim strips whitespace`` () =
#if FABLE_COMPILER
    str.trim "  hello  " |> equal "hello"
#else
    ()
#endif

[<Fact>]
let ``test str.trim with leading direction`` () =
#if FABLE_COMPILER
    let leading = Erlang.binaryToAtom "leading"
    str.trim ("  hello  ", leading) |> equal "hello  "
#else
    ()
#endif

[<Fact>]
let ``test str.trim with trailing direction`` () =
#if FABLE_COMPILER
    let trailing = Erlang.binaryToAtom "trailing"
    str.trim ("  hello  ", trailing) |> equal "  hello"
#else
    ()
#endif

[<Fact>]
let ``test str.pad trailing to length`` () =
#if FABLE_COMPILER
    str.pad ("hi", 5) |> equal "hi   "
#else
    ()
#endif

[<Fact>]
let ``test str.pad leading with direction`` () =
#if FABLE_COMPILER
    let leading = Erlang.binaryToAtom "leading"
    str.pad ("hi", 5, leading) |> equal "   hi"
#else
    ()
#endif

[<Fact>]
let ``test str.pad with custom character`` () =
#if FABLE_COMPILER
    let leading = Erlang.binaryToAtom "leading"
    str.pad ("7", 3, leading, "0") |> equal "007"
#else
    ()
#endif

[<Fact>]
let ``test str.slice from position`` () =
#if FABLE_COMPILER
    str.slice ("hello world", 6) |> equal "world"
#else
    ()
#endif

[<Fact>]
let ``test str.slice with length`` () =
#if FABLE_COMPILER
    str.slice ("hello world", 0, 5) |> equal "hello"
#else
    ()
#endif

[<Fact>]
let ``test str.equal compares strings`` () =
#if FABLE_COMPILER
    str.equal ("hello", "hello") |> equal true
    str.equal ("hello", "world") |> equal false
#else
    ()
#endif

[<Fact>]
let ``test str.equal case-insensitive`` () =
#if FABLE_COMPILER
    str.equal ("Hello", "hello", true) |> equal true
    str.equal ("Hello", "world", true) |> equal false
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
let ``test findFrom trailing finds last occurrence`` () =
#if FABLE_COMPILER
    let trailing = Erlang.binaryToAtom "trailing"
    findFrom "a-b-c" "-" trailing |> equal (Some "-c")
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
    | Ok(n, rest) ->
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
    | Ok(f, _) -> (f > 3.13 && f < 3.15) |> equal true
    | Error _ -> equal true false
#else
    ()
#endif

[<Fact>]
let ``test toGraphemes splits into grapheme clusters`` () =
#if FABLE_COMPILER
    let graphemes = toGraphemes "abc"
    Array.length graphemes |> equal 3
    graphemes.[0] |> equal "a"
    graphemes.[1] |> equal "b"
    graphemes.[2] |> equal "c"
#else
    ()
#endif
