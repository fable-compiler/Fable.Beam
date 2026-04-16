module Fable.Beam.Tests.Binary

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Binary
#endif

[<Fact>]
let ``test binary.copy makes a copy`` () =
#if FABLE_COMPILER
    let b = "hello"
    binary.copy b |> equal "hello"
#else
    ()
#endif

[<Fact>]
let ``test binary.copy N times concatenates`` () =
#if FABLE_COMPILER
    binary.copy ("ab", 3) |> equal "ababab"
#else
    ()
#endif

[<Fact>]
let ``test binary.at returns byte at position`` () =
#if FABLE_COMPILER
    // 'A' = 65, 'B' = 66
    binary.at ("AB", 0) |> equal 65
    binary.at ("AB", 1) |> equal 66
#else
    ()
#endif

[<Fact>]
let ``test binary.first returns first byte`` () =
#if FABLE_COMPILER
    // 'h' = 104
    binary.first "hello" |> equal 104
#else
    ()
#endif

[<Fact>]
let ``test binary.last returns last byte`` () =
#if FABLE_COMPILER
    // 'o' = 111
    binary.last "hello" |> equal 111
#else
    ()
#endif

[<Fact>]
let ``test binary.part extracts subbinary`` () =
#if FABLE_COMPILER
    binary.part ("hello world", 6, 5) |> equal "world"
#else
    ()
#endif

[<Fact>]
let ``test matchFirst returns Some on match`` () =
#if FABLE_COMPILER
    matchFirst "hello" "ll" |> equal (Some (2, 2))
#else
    ()
#endif

[<Fact>]
let ``test matchFirst returns None when not found`` () =
#if FABLE_COMPILER
    matchFirst "hello" "xyz" |> equal None
#else
    ()
#endif

[<Fact>]
let ``test matchAll returns all occurrences`` () =
#if FABLE_COMPILER
    let results = matchAll "abcabc" "b"
    Array.length results |> equal 2
#else
    ()
#endif

[<Fact>]
let ``test splitFirst splits on first occurrence`` () =
#if FABLE_COMPILER
    let parts = splitFirst "hello world" " "
    Array.length parts |> equal 2
    parts.[0] |> equal "hello"
    parts.[1] |> equal "world"
#else
    ()
#endif

[<Fact>]
let ``test splitAll splits on all occurrences`` () =
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
let ``test binary.longest_common_prefix`` () =
#if FABLE_COMPILER
    binary.longest_common_prefix (["foobar"; "foobaz"; "fooqux"]) |> equal 4
#else
    ()
#endif
