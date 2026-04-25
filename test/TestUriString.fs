module Fable.Beam.Tests.UriString

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.UriString
#endif

// ============================================================================
// parse + accessors
// ============================================================================

[<Fact>]
let ``test parse full uri`` () =
#if FABLE_COMPILER
    match parse "https://user:pass@example.com:8080/path?q=hello#frag" with
    | Ok uri ->
        scheme uri |> equal (Some "https")
        userinfo uri |> equal (Some "user:pass")
        host uri |> equal (Some "example.com")
        port uri |> equal (Some 8080)
        path uri |> equal (Some "/path")
        query uri |> equal (Some "q=hello")
        fragment uri |> equal (Some "frag")
    | Error _ -> false |> equal true
#else
    ()
#endif

[<Fact>]
let ``test parse minimal uri`` () =
#if FABLE_COMPILER
    match parse "https://example.com" with
    | Ok uri ->
        scheme uri |> equal (Some "https")
        host uri |> equal (Some "example.com")
        port uri |> equal None
        query uri |> equal None
        fragment uri |> equal None
    | Error _ -> false |> equal true
#else
    ()
#endif

[<Fact>]
let ``test parse relative uri has no scheme or host`` () =
#if FABLE_COMPILER
    match parse "/relative/path" with
    | Ok uri ->
        scheme uri |> equal None
        host uri |> equal None
        path uri |> equal (Some "/relative/path")
    | Error _ -> false |> equal true
#else
    ()
#endif

[<Fact>]
let ``test parse path only uri`` () =
#if FABLE_COMPILER
    match parse "just/a/path" with
    | Ok uri ->
        scheme uri |> equal None
        host uri |> equal None
        path uri |> equal (Some "just/a/path")
    | Error _ -> false |> equal true
#else
    ()
#endif

// ============================================================================
// normalize
// ============================================================================

[<Fact>]
let ``test normalize lowercases scheme and host`` () =
#if FABLE_COMPILER
    normalize "HTTP://EXAMPLE.COM/path" |> equal (Ok "http://example.com/path")
#else
    ()
#endif

[<Fact>]
let ``test normalize removes default http port`` () =
#if FABLE_COMPILER
    normalize "http://example.com:80/path" |> equal (Ok "http://example.com/path")
#else
    ()
#endif

[<Fact>]
let ``test normalize removes default https port`` () =
#if FABLE_COMPILER
    normalize "https://example.com:443/path" |> equal (Ok "https://example.com/path")
#else
    ()
#endif

[<Fact>]
let ``test normalize resolves dot segments`` () =
#if FABLE_COMPILER
    normalize "http://example.com/a/b/../c" |> equal (Ok "http://example.com/a/c")
#else
    ()
#endif

// ============================================================================
// resolve
// ============================================================================

[<Fact>]
let ``test resolve absolute path reference`` () =
#if FABLE_COMPILER
    resolve "/new" "https://example.com/old/page" |> equal (Ok "https://example.com/new")
#else
    ()
#endif

[<Fact>]
let ``test resolve relative path reference`` () =
#if FABLE_COMPILER
    resolve "new" "https://example.com/old/page" |> equal (Ok "https://example.com/old/new")
#else
    ()
#endif

[<Fact>]
let ``test resolve full uri preserves reference`` () =
#if FABLE_COMPILER
    resolve "https://other.com/path" "https://example.com/base"
    |> equal (Ok "https://other.com/path")
#else
    ()
#endif

// ============================================================================
// dissectQuery / composeQuery
// ============================================================================

[<Fact>]
let ``test dissect query parses key value pairs`` () =
#if FABLE_COMPILER
    dissectQuery "q=hello&lang=en" |> equal [ ("q", "hello"); ("lang", "en") ]
#else
    ()
#endif

[<Fact>]
let ``test dissect query empty string`` () =
#if FABLE_COMPILER
    dissectQuery "" |> equal []
#else
    ()
#endif

[<Fact>]
let ``test compose query builds query string`` () =
#if FABLE_COMPILER
    composeQuery [ ("q", "search"); ("page", "1") ] |> equal "q=search&page=1"
#else
    ()
#endif

[<Fact>]
let ``test compose query empty list`` () =
#if FABLE_COMPILER
    composeQuery [] |> equal ""
#else
    ()
#endif

[<Fact>]
let ``test dissect and compose query roundtrip`` () =
#if FABLE_COMPILER
    let original = "name=Alice&role=admin"
    original |> dissectQuery |> composeQuery |> equal original
#else
    ()
#endif

// ============================================================================
// percentDecode
// ============================================================================

[<Fact>]
let ``test percent decode decodes encoded chars`` () =
#if FABLE_COMPILER
    percentDecode "hello%20world" |> equal (Ok "hello world")
#else
    ()
#endif

[<Fact>]
let ``test percent decode passthrough for plain string`` () =
#if FABLE_COMPILER
    percentDecode "hello" |> equal (Ok "hello")
#else
    ()
#endif

[<Fact>]
let ``test percent decode returns error for malformed encoding`` () =
#if FABLE_COMPILER
    match percentDecode "invalid%GG" with
    | Error _ -> true |> equal true
    | Ok _ -> false |> equal true
#else
    ()
#endif

// ============================================================================
// quote / quoteWith / unquote
// ============================================================================

[<Fact>]
let ``test quote encodes spaces and slashes`` () =
#if FABLE_COMPILER
    quote "hello world" |> equal "hello%20world"
#else
    ()
#endif

[<Fact>]
let ``test quote with safe chars preserves slash`` () =
#if FABLE_COMPILER
    quoteWith "hello/world" "/" |> equal "hello/world"
#else
    ()
#endif

[<Fact>]
let ``test unquote decodes percent encoded string`` () =
#if FABLE_COMPILER
    unquote "hello%20world" |> equal "hello world"
#else
    ()
#endif

[<Fact>]
let ``test unquote passthrough for plain string`` () =
#if FABLE_COMPILER
    unquote "hello" |> equal "hello"
#else
    ()
#endif
