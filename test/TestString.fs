module Fable.Beam.Tests.String

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.String
open Fable.Beam.Lists
#endif

// Keep test assertions unambiguous now that String exposes its own `equal` binding.
let private equal = Fable.Beam.Testing.equal

[<Fact>]
let ``test isEmpty returns true for empty`` () =
#if FABLE_COMPILER
    isEmpty "" |> equal true
#else
    ()
#endif

[<Fact>]
let ``test isEmpty returns false for non-empty`` () =
#if FABLE_COMPILER
    isEmpty "hello" |> equal false
#else
    ()
#endif

[<Fact>]
let ``test length returns grapheme count`` () =
#if FABLE_COMPILER
    length "hello" |> equal 5
#else
    ()
#endif

[<Fact>]
let ``test lowercase converts to lowercase`` () =
#if FABLE_COMPILER
    lowercase "HELLO" |> equal "hello"
#else
    ()
#endif

[<Fact>]
let ``test uppercase converts to uppercase`` () =
#if FABLE_COMPILER
    uppercase "hello" |> equal "HELLO"
#else
    ()
#endif

[<Fact>]
let ``test titlecase capitalises first grapheme`` () =
#if FABLE_COMPILER
    titlecase "hello world" |> equal "Hello world"
#else
    ()
#endif

[<Fact>]
let ``test casefold lowercases for comparison`` () =
#if FABLE_COMPILER
    casefold "HELLO" |> equal "hello"
#else
    ()
#endif

[<Fact>]
let ``test reverse reverses string`` () =
#if FABLE_COMPILER
    reverse "hello" |> equal "olleh"
#else
    ()
#endif

[<Fact>]
let ``test trim strips whitespace`` () =
#if FABLE_COMPILER
    trim "  hello  " |> equal "hello"
#else
    ()
#endif

[<Fact>]
let ``test trimStart strips leading whitespace`` () =
#if FABLE_COMPILER
    trimStart "  hello  " |> equal "hello  "
#else
    ()
#endif

[<Fact>]
let ``test trimEnd strips trailing whitespace`` () =
#if FABLE_COMPILER
    trimEnd "  hello  " |> equal "  hello"
#else
    ()
#endif

[<Fact>]
let ``test padEnd pads trailing to length`` () =
#if FABLE_COMPILER
    padEnd "hi" 5 |> equal "hi   "
#else
    ()
#endif

[<Fact>]
let ``test padStart pads leading to length`` () =
#if FABLE_COMPILER
    padStart "hi" 5 |> equal "   hi"
#else
    ()
#endif

[<Fact>]
let ``test padStartWith pads with custom character`` () =
#if FABLE_COMPILER
    padStartWith "7" 3 "0" |> equal "007"
#else
    ()
#endif

[<Fact>]
let ``test padEndWith pads with custom character`` () =
#if FABLE_COMPILER
    padEndWith "7" 3 "0" |> equal "700"
#else
    ()
#endif

[<Fact>]
let ``test padBoth pads both sides`` () =
#if FABLE_COMPILER
    padBoth "hi" 6 |> equal "  hi  "
#else
    ()
#endif

[<Fact>]
let ``test padBothWith pads both sides with custom character`` () =
#if FABLE_COMPILER
    padBothWith "7" 5 "0" |> equal "00700"
#else
    ()
#endif

// ----------------------------------------------------------------------------
// Raw chardata variants (BeamChardata)
// ----------------------------------------------------------------------------

#if FABLE_COMPILER
// The raw variants return unflattened chardata: an iolist/charlist, i.e. a *list*, never a binary.
[<Emit("is_list($0)")>]
let private isList (x: BeamChardata) : bool = nativeOnly
#endif

[<Fact>]
let ``test padEndRaw returns unflattened chardata that flattens to padEnd`` () =
#if FABLE_COMPILER
    let raw = padEndRaw "hi" 5
    // proves it is genuinely raw: string:pad yields an iolist ([<<"hi">>,32,32,32]), not a binary
    isList raw |> equal true
    BeamChardata.toString raw |> equal "hi   "
    BeamChardata.toString raw |> equal (padEnd "hi" 5)
#else
    ()
#endif

[<Fact>]
let ``test padStartRaw returns unflattened chardata that flattens to padStart`` () =
#if FABLE_COMPILER
    let raw = padStartRaw "hi" 5
    isList raw |> equal true
    BeamChardata.toString raw |> equal (padStart "hi" 5)
#else
    ()
#endif

[<Fact>]
let ``test padBothRaw returns unflattened chardata that flattens to padBoth`` () =
#if FABLE_COMPILER
    let raw = padBothRaw "hi" 6
    isList raw |> equal true
    BeamChardata.toString raw |> equal (padBoth "hi" 6)
#else
    ()
#endif

[<Fact>]
let ``test reverseRaw flattens back to reverse`` () =
#if FABLE_COMPILER
    let raw = reverseRaw "hello"
    isList raw |> equal true
    BeamChardata.toString raw |> equal "olleh"
#else
    ()
#endif

[<Fact>]
let ``test replaceAllRaw flattens back to replaceAll`` () =
#if FABLE_COMPILER
    let raw = replaceAllRaw "aXbXa" "X" "Y"
    BeamChardata.toString raw |> equal "aYbYa"
    BeamChardata.toString raw |> equal (replaceAll "aXbXa" "X" "Y")
#else
    ()
#endif

[<Fact>]
let ``test BeamChardata ofString roundtrips through toString`` () =
#if FABLE_COMPILER
    "hi" |> BeamChardata.ofString |> BeamChardata.toString |> equal "hi"
#else
    ()
#endif

[<Fact>]
let ``test slice from position`` () =
#if FABLE_COMPILER
    slice "hello world" 6 |> equal "world"
#else
    ()
#endif

[<Fact>]
let ``test sliceLen with length`` () =
#if FABLE_COMPILER
    sliceLen "hello world" 0 5 |> equal "hello"
#else
    ()
#endif

[<Fact>]
let ``test equal compares strings`` () =
#if FABLE_COMPILER
    Fable.Beam.String.equal "hello" "hello" |> equal true
    Fable.Beam.String.equal "hello" "world" |> equal false
#else
    ()
#endif

[<Fact>]
let ``test equalCaseInsensitive compares strings`` () =
#if FABLE_COMPILER
    equalCaseInsensitive "Hello" "hello" |> equal true
    equalCaseInsensitive "Hello" "world" |> equal false
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
let ``test findLast finds last occurrence`` () =
#if FABLE_COMPILER
    findLast "a-b-c" "-" |> equal (Some "-c")
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

[<Fact>]
let ``test splitAllRaw returns the native list form of splitAll`` () =
#if FABLE_COMPILER
    let parts: BeamList<string> = splitAllRaw "a,b,c" ","
    let expected: BeamList<string> = emitErlExpr () "[<<\"a\">>, <<\"b\">>, <<\"c\">>]"
    parts |> equal expected
#else
    ()
#endif

[<Fact>]
let ``test splitFirstRaw returns the native list form of splitFirst`` () =
#if FABLE_COMPILER
    let parts: BeamList<string> = splitFirstRaw "hello world" " "
    let expected: BeamList<string> = emitErlExpr () "[<<\"hello\">>, <<\"world\">>]"
    parts |> equal expected
#else
    ()
#endif
