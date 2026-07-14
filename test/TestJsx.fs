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

// `labels` is a *decoder* option: jsx:is_json/2 does not accept it and simply answers `false`,
// so is_json can never show that the option took effect. These tests decode instead and inspect
// the key type of the resulting map -- if Fable stringified the DU case, jsx would receive
// {labels, <<"atom">>}, silently ignore it, and the keys would stay binaries.
[<Emit("case maps:keys($0) of [K] -> is_atom(K); _ -> false end")>]
let private firstKeyIsAtom (m: obj) : bool = nativeOnly

[<Emit("case maps:keys($0) of [K] -> is_binary(K); _ -> false end")>]
let private firstKeyIsBinary (m: obj) : bool = nativeOnly

[<Fact>]
let ``test jsx labels Binary keeps keys as binaries`` () =
#if FABLE_COMPILER
    let decoded: obj = jsx.decode ("""{"key":"value"}""", [ labels LabelMode.Binary ])

    firstKeyIsBinary decoded |> equal true
#else
    ()
#endif

[<Fact>]
let ``test jsx labels Atom converts keys to atoms`` () =
#if FABLE_COMPILER
    let decoded: obj = jsx.decode ("""{"key":"value"}""", [ labels LabelMode.Atom ])

    firstKeyIsAtom decoded |> equal true
#else
    ()
#endif

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
    // existing_atom uses binary_to_existing_atom, so a key whose atom was never created
    // raises badarg rather than silently interning it. That rejection *is* the behaviour.
    (fun () -> jsx.decode ("""{"never_interned_key_qqq":"value"}""", [ labels LabelMode.ExistingAtom ]))
    |> throwsAnyError
#else
    ()
#endif

[<Fact>]
let ``test jsx labels AttemptAtom is accepted`` () =
#if FABLE_COMPILER
    // attempt_atom converts the key when the atom already exists and leaves it a binary
    // otherwise. "key" is interned by the decode above, so it comes back as an atom.
    let decoded: obj =
        jsx.decode ("""{"key":"value"}""", [ labels LabelMode.AttemptAtom ])

    firstKeyIsAtom decoded |> equal true
#else
    ()
#endif
