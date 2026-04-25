module Fable.Beam.Tests.UriString

open Fable.Beam.Testing
open Fable.Beam.UriString

// ============================================================================
// parse + accessors
// ============================================================================

[<Fact>]
let ``test parse full uri`` () =
    let m = parse "https://user:pass@example.com:8080/path?q=hello#frag"

    match m with
    | Error _ -> () |> equal ()
    | Ok uri ->
        scheme uri |> equal (Some "https")
        userinfo uri |> equal (Some "user:pass")
        host uri |> equal (Some "example.com")
        port uri |> equal (Some 8080)
        path uri |> equal (Some "/path")
        query uri |> equal (Some "q=hello")
        fragment uri |> equal (Some "frag")

[<Fact>]
let ``test parse minimal uri`` () =
    let m = parse "https://example.com"

    match m with
    | Error _ -> () |> equal ()
    | Ok uri ->
        scheme uri |> equal (Some "https")
        host uri |> equal (Some "example.com")
        port uri |> equal None
        query uri |> equal None
        fragment uri |> equal None

[<Fact>]
let ``test parse relative uri has no scheme or host`` () =
    let m = parse "/relative/path"

    match m with
    | Error _ -> () |> equal ()
    | Ok uri ->
        scheme uri |> equal None
        host uri |> equal None
        path uri |> equal (Some "/relative/path")

[<Fact>]
let ``test parse path only uri`` () =
    let m = parse "just/a/path"

    match m with
    | Error _ -> () |> equal ()
    | Ok uri ->
        scheme uri |> equal None
        host uri |> equal None
        path uri |> equal (Some "just/a/path")

// ============================================================================
// normalize
// ============================================================================

[<Fact>]
let ``test normalize lowercases scheme and host`` () =
    let result = normalize "HTTP://EXAMPLE.COM/path"
    result |> equal (Ok "http://example.com/path")

[<Fact>]
let ``test normalize removes default http port`` () =
    let result = normalize "http://example.com:80/path"
    result |> equal (Ok "http://example.com/path")

[<Fact>]
let ``test normalize removes default https port`` () =
    let result = normalize "https://example.com:443/path"
    result |> equal (Ok "https://example.com/path")

[<Fact>]
let ``test normalize resolves dot segments`` () =
    let result = normalize "http://example.com/a/b/../c"
    result |> equal (Ok "http://example.com/a/c")

// ============================================================================
// resolve
// ============================================================================

[<Fact>]
let ``test resolve absolute path reference`` () =
    let result = resolve "/new" "https://example.com/old/page"
    result |> equal (Ok "https://example.com/new")

[<Fact>]
let ``test resolve relative path reference`` () =
    let result = resolve "new" "https://example.com/old/page"
    result |> equal (Ok "https://example.com/old/new")

[<Fact>]
let ``test resolve full uri preserves reference`` () =
    let result = resolve "https://other.com/path" "https://example.com/base"
    result |> equal (Ok "https://other.com/path")

// ============================================================================
// dissectQuery / composeQuery
// ============================================================================

[<Fact>]
let ``test dissect query parses key value pairs`` () =
    let pairs = dissectQuery "q=hello&lang=en"
    pairs |> equal [ ("q", "hello"); ("lang", "en") ]

[<Fact>]
let ``test dissect query empty string`` () =
    let pairs = dissectQuery ""
    pairs |> equal []

[<Fact>]
let ``test compose query builds query string`` () =
    let qs = composeQuery [ ("q", "search"); ("page", "1") ]
    qs |> equal "q=search&page=1"

[<Fact>]
let ``test compose query empty list`` () =
    let qs = composeQuery []
    qs |> equal ""

[<Fact>]
let ``test dissect and compose query roundtrip`` () =
    let original = "name=Alice&role=admin"
    let roundtrip = original |> dissectQuery |> composeQuery
    roundtrip |> equal original

// ============================================================================
// percentDecode
// ============================================================================

[<Fact>]
let ``test percent decode decodes encoded chars`` () =
    let result = percentDecode "hello%20world"
    result |> equal (Ok "hello world")

[<Fact>]
let ``test percent decode passthrough for plain string`` () =
    let result = percentDecode "hello"
    result |> equal (Ok "hello")

[<Fact>]
let ``test percent decode returns error for malformed encoding`` () =
    let result = percentDecode "invalid%GG"

    match result with
    | Error _ -> true |> equal true
    | Ok _ -> false |> equal true

// ============================================================================
// quote / quoteWith / unquote
// ============================================================================

[<Fact>]
let ``test quote encodes spaces and slashes`` () =
    let result = quote "hello world"
    result |> equal "hello%20world"

[<Fact>]
let ``test quote with safe chars preserves slash`` () =
    let result = quoteWith "hello/world" "/"
    result |> equal "hello/world"

[<Fact>]
let ``test unquote decodes percent encoded string`` () =
    let result = unquote "hello%20world"
    result |> equal "hello world"

[<Fact>]
let ``test unquote passthrough for plain string`` () =
    let result = unquote "hello"
    result |> equal "hello"
