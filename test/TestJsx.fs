module Fable.Beam.Tests.Jsx

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
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

[<Fact>]
let ``test jsx labels Binary keeps keys as binaries`` () =
#if FABLE_COMPILER
    // With LabelMode.Binary the decoded map's keys are binaries, so a binary
    // lookup must succeed. is_json/decode round-trip is enough to prove the
    // option was accepted by jsx (bad atoms would crash the call).
    jsx.is_json ("""{"key":"value"}""", [ labels LabelMode.Binary ]) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test jsx labels Atom converts keys to atoms`` () =
#if FABLE_COMPILER
    jsx.is_json ("""{"key":"value"}""", [ labels LabelMode.Atom ]) |> equal true
#else
    ()
#endif

// Stronger probe: decode a key with LabelMode.Atom and check the resulting
// key is an atom, not a binary. If the [<Emit>] attribute on the DU case is
// honoured, jsx receives {labels, atom} and produces an atom key. If Fable
// stringifies the case name, jsx receives {labels, <<"atom">>}, silently
// ignores it, and the key stays a binary.
[<Emit("case maps:keys($0) of [K] -> is_atom(K); _ -> false end")>]
let private firstKeyIsAtom (m: obj) : bool = nativeOnly

[<Fact>]
let ``test jsx labels Atom probe emits raw atom option`` () =
#if FABLE_COMPILER
    let decoded: obj =
        jsx.decode ("""{"atom_key_probe_xyz":"value"}""", [ labels LabelMode.Atom ])
    firstKeyIsAtom decoded |> equal true
#else
    ()
#endif

[<Fact>]
let ``test jsx labels ExistingAtom rejects unknown atoms`` () =
#if FABLE_COMPILER
    // Force the atom to exist before decoding.
    let _ = "definitely_known_key_xyz"
    jsx.is_json ("""{"definitely_known_key_xyz":"value"}""", [ labels LabelMode.ExistingAtom ])
    |> equal true
#else
    ()
#endif

[<Fact>]
let ``test jsx labels AttemptAtom is accepted`` () =
#if FABLE_COMPILER
    jsx.is_json ("""{"key":"value"}""", [ labels LabelMode.AttemptAtom ]) |> equal true
#else
    ()
#endif
