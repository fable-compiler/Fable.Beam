module Fable.Beam.Tests.Jsx

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Beam.Jsx.Jsx
#endif

[<Fact>]
let ``test jsx encode integer`` () =
#if FABLE_COMPILER
    let json = jsx.encode 42
    json |> equal "42"
#else
    ()
#endif

[<Fact>]
let ``test jsx encode string`` () =
#if FABLE_COMPILER
    let json = jsx.encode "hello"
    json |> equal "\"hello\""
#else
    ()
#endif

[<Fact>]
let ``test jsx decode string`` () =
#if FABLE_COMPILER
    let result: string = jsx.decode "\"hello\""
    result |> equal "hello"
#else
    ()
#endif

[<Fact>]
let ``test jsx is_json valid`` () =
#if FABLE_COMPILER
    jsx.is_json """{"key": "value"}""" |> equal true
#else
    ()
#endif

[<Fact>]
let ``test jsx is_json invalid`` () =
#if FABLE_COMPILER
    jsx.is_json "not json" |> equal false
#else
    ()
#endif

[<Fact>]
let ``test jsx minify`` () =
#if FABLE_COMPILER
    let result = jsx.minify """{ "key" : "value" }"""
    result |> equal """{"key":"value"}"""
#else
    ()
#endif

[<Fact>]
let ``test jsx prettify and minify roundtrip`` () =
#if FABLE_COMPILER
    let json = """{"key":"value"}"""
    let pretty = jsx.prettify json
    let mini = jsx.minify pretty
    mini |> equal json
#else
    ()
#endif

[<Fact>]
let ``test jsx is_json with strict rejects trailing comma`` () =
#if FABLE_COMPILER
    jsx.is_json ("""{"key": "value",}""", [ strict ]) |> equal false
#else
    ()
#endif

[<Fact>]
let ``test jsx format with indent`` () =
#if FABLE_COMPILER
    let result = jsx.format ("""{"key":"value"}""", [ indent 2 ])
    // Formatted output should be longer than minified
    (String.length result > String.length """{"key":"value"}""") |> equal true
#else
    ()
#endif
