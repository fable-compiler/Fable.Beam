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
let ``test re.replaceFirstWith caseless replaces first case-insensitively`` () =
#if FABLE_COMPILER
    replaceFirstWith "Aabbaa" "a+" "X" [ caseless ] |> equal "Xbbaa"
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

// ============================================================================
// Options
// ============================================================================

[<Fact>]
let ``test re.isMatchWith caseless option matches different case`` () =
#if FABLE_COMPILER
    isMatchWith "HELLO" "hello" [ caseless ] |> equal true
    // Sanity check: default is case-sensitive
    isMatch "HELLO" "hello" |> equal false
#else
    ()
#endif

[<Fact>]
let ``test re.isMatchWith multiline option matches after newline`` () =
#if FABLE_COMPILER
    // ^world only matches at line starts in multiline mode
    isMatchWith "hello\nworld" "^world" [ multiline ] |> equal true
    isMatch "hello\nworld" "^world" |> equal false
#else
    ()
#endif

[<Fact>]
let ``test re.isMatchWith unicode option handles multi-byte characters`` () =
#if FABLE_COMPILER
    // "é" is 2 bytes in UTF-8. Without unicode, ^.$ expects exactly 1 byte — no match.
    // With unicode, ^.$ expects exactly 1 codepoint — matches.
    isMatch "é" "^.$" |> equal false
    isMatchWith "é" "^.$" [ unicode ] |> equal true
#else
    ()
#endif

[<Fact>]
let ``test re.runWith caseless returns original-case captures`` () =
#if FABLE_COMPILER
    match runWith "HELLO world" "hello" [ caseless ] with
    | Some captures -> captures.[0] |> equal "HELLO"
    | None -> false |> equal true
#else
    ()
#endif

[<Fact>]
let ``test re.compileWith caseless produces case-insensitive MP`` () =
#if FABLE_COMPILER
    match compileWith "hello" [ caseless ] with
    | Ok mp ->
        isMatchMP "HELLO" mp |> equal true
        isMatchMP "hello" mp |> equal true
    | Error _ -> false |> equal true
#else
    ()
#endif

[<Fact>]
let ``test re.replaceAllWith caseless replaces all cases`` () =
#if FABLE_COMPILER
    replaceAllWith "Hello HELLO hello" "hello" "X" [ caseless ] |> equal "X X X"
#else
    ()
#endif

[<Fact>]
let ``test re.splitWith caseless splits on either case`` () =
#if FABLE_COMPILER
    let parts = splitWith "aXbxc" "x" [ caseless ]
    parts.[0] |> equal "a"
    parts.[1] |> equal "b"
    parts.[2] |> equal "c"
#else
    ()
#endif

// ============================================================================
// Compiled-pattern reuse (replace / split)
// ============================================================================

[<Fact>]
let ``test re.replaceFirstMP with compiled pattern`` () =
#if FABLE_COMPILER
    match compile "a+" with
    | Ok mp -> replaceFirstMP "aabbaa" mp "X" |> equal "Xbbaa"
    | Error _ -> false |> equal true
#else
    ()
#endif

[<Fact>]
let ``test re.replaceAllMP with compiled pattern`` () =
#if FABLE_COMPILER
    match compile "\\d+" with
    | Ok mp -> replaceAllMP "abc123def456" mp "N" |> equal "abcNdefN"
    | Error _ -> false |> equal true
#else
    ()
#endif

[<Fact>]
let ``test re.splitMP with compiled pattern`` () =
#if FABLE_COMPILER
    match compile "," with
    | Ok mp ->
        let parts = splitMP "one,two,three" mp
        parts.[0] |> equal "one"
        parts.[1] |> equal "two"
        parts.[2] |> equal "three"
    | Error _ -> false |> equal true
#else
    ()
#endif

// ============================================================================
// Edge cases
// ============================================================================

[<Fact>]
let ``test re.isMatch with empty subject`` () =
#if FABLE_COMPILER
    // Empty pattern matches empty subject (zero-width match at position 0)
    isMatch "" "" |> equal true
    isMatch "" "a" |> equal false
#else
    ()
#endif

[<Fact>]
let ``test re.run with empty subject and optional group returns Some empty`` () =
#if FABLE_COMPILER
    match run "" "a*" with
    | Some captures -> captures.[0] |> equal ""
    | None -> false |> equal true
#else
    ()
#endif
