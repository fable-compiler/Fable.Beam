module Fable.Beam.Tests.Re

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Re
#endif

[<Fact>]
let ``test re.isMatch returns true for matching pattern`` () =
#if FABLE_COMPILER
    isMatch "hello world" "hello" |> equal true
#else
    ()
#endif

[<Fact>]
let ``test re.isMatch returns false for non-matching pattern`` () =
#if FABLE_COMPILER
    isMatch "hello world" "xyz" |> equal false
#else
    ()
#endif

[<Fact>]
let ``test re.isMatch with digit pattern`` () =
#if FABLE_COMPILER
    isMatch "abc123" "\\d+" |> equal true
#else
    ()
#endif

[<Fact>]
let ``test re.isMatch anchored no match`` () =
#if FABLE_COMPILER
    isMatch "hello" "^world" |> equal false
#else
    ()
#endif

[<Fact>]
let ``test re.compile returns Ok for valid pattern`` () =
#if FABLE_COMPILER
    match compile "hello" with
    | Ok _ -> true |> equal true
    | Error _ -> false |> equal true
#else
    ()
#endif

[<Fact>]
let ``test re.compile returns Error for invalid pattern`` () =
#if FABLE_COMPILER
    match compile "[invalid" with
    | Ok _ -> false |> equal true
    | Error msg -> (msg.Length > 0) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test re.isMatchMP with compiled pattern`` () =
#if FABLE_COMPILER
    match compile "\\d+" with
    | Ok mp -> isMatchMP "abc123" mp |> equal true
    | Error _ -> false |> equal true
#else
    ()
#endif

[<Fact>]
let ``test re.isMatchMP with compiled pattern no match`` () =
#if FABLE_COMPILER
    match compile "\\d+" with
    | Ok mp -> isMatchMP "abcdef" mp |> equal false
    | Error _ -> false |> equal true
#else
    ()
#endif

[<Fact>]
let ``test re.run returns Some with whole match at index 0`` () =
#if FABLE_COMPILER
    match run "hello world" "hello" with
    | Some captures -> captures.[0] |> equal "hello"
    | None -> false |> equal true
#else
    ()
#endif

[<Fact>]
let ``test re.run returns Some with capture groups`` () =
#if FABLE_COMPILER
    match run "hello world" "h(e)(l+)o" with
    | Some captures ->
        captures.[0] |> equal "hello"
        captures.[1] |> equal "e"
        captures.[2] |> equal "ll"
    | None -> false |> equal true
#else
    ()
#endif

[<Fact>]
let ``test re.run returns None for no match`` () =
#if FABLE_COMPILER
    run "hello world" "xyz" |> equal None
#else
    ()
#endif

[<Fact>]
let ``test re.runMP returns captures for compiled pattern`` () =
#if FABLE_COMPILER
    match compile "(\\d+)" with
    | Ok mp ->
        match runMP "abc123def" mp with
        | Some captures ->
            captures.[0] |> equal "123"
            captures.[1] |> equal "123"
        | None -> false |> equal true
    | Error _ -> false |> equal true
#else
    ()
#endif

[<Fact>]
let ``test re.replaceFirst replaces only first occurrence`` () =
#if FABLE_COMPILER
    replaceFirst "aabbaa" "a+" "X" |> equal "Xbbaa"
#else
    ()
#endif

[<Fact>]
let ``test re.replaceAll replaces all occurrences`` () =
#if FABLE_COMPILER
    replaceAll "aabbaa" "a+" "X" |> equal "XbbX"
#else
    ()
#endif

[<Fact>]
let ``test re.replaceAll with digit pattern`` () =
#if FABLE_COMPILER
    replaceAll "abc123def456" "\\d+" "N" |> equal "abcNdefN"
#else
    ()
#endif

[<Fact>]
let ``test re.split on comma`` () =
#if FABLE_COMPILER
    let parts = split "one,two,three" ","
    parts.[0] |> equal "one"
    parts.[1] |> equal "two"
    parts.[2] |> equal "three"
#else
    ()
#endif

[<Fact>]
let ``test re.split on whitespace pattern`` () =
#if FABLE_COMPILER
    let parts = split "a b  c" "\\s+"
    parts.[0] |> equal "a"
    parts.[1] |> equal "b"
    parts.[2] |> equal "c"
#else
    ()
#endif

[<Fact>]
let ``test re.splitParts limits result count`` () =
#if FABLE_COMPILER
    let parts = splitParts "one,two,three,four" "," 2
    parts.[0] |> equal "one"
    parts.[1] |> equal "two,three,four"
#else
    ()
#endif
