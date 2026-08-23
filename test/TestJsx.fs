module Fable.Beam.Tests.Jsx

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop

module BJsx = Fable.Beam.Jsx.Jsx

#if FABLE_COMPILER
// `labels` is a *decoder* option: jsx:is_json/2 does not accept it and simply answers `false`,
// so is_json can never show that the option took effect. These tests decode instead and inspect
// the key type of the resulting map -- if Fable stringified the DU case, jsx would receive
// {labels, <<"atom">>}, silently ignore it, and the keys would stay binaries.
[<Emit("case maps:keys($0) of [K] -> is_atom(K); _ -> false end")>]
let private firstKeyIsAtom (m: obj) : bool = nativeOnly

[<Emit("case maps:keys($0) of [K] -> is_binary(K); _ -> false end")>]
let private firstKeyIsBinary (m: obj) : bool = nativeOnly
#endif

let tests =
    testList (
        "Jsx",
        [ test (
              "jsx encode integer",
              fun _ ->
                  let json = BJsx.encode 42
                  assertThat json (isEqualTo "42")
          )

          test (
              "jsx encode string",
              fun _ ->
                  let json = BJsx.encode "hello"
                  assertThat json (isEqualTo "\"hello\"")
          )

          test (
              "jsx decode string",
              fun _ ->
                  let result: string = BJsx.decode "\"hello\""
                  assertThat result (isEqualTo "hello")
          )

          test ("jsx is_json valid", fun _ -> assertThat (BJsx.isJson """{"key": "value"}""") (isTrue))

          test ("jsx is_json invalid", fun _ -> assertThat (BJsx.isJson "not json") (isFalse))

          test (
              "jsx minify",
              fun _ ->
                  let result = BJsx.minify """{ "key" : "value" }"""
                  assertThat result (isEqualTo """{"key":"value"}""")
          )

          test (
              "jsx prettify and minify roundtrip",
              fun _ ->
                  let json = """{"key":"value"}"""
                  let pretty = BJsx.prettify json
                  let mini = BJsx.minify pretty
                  assertThat mini (isEqualTo json)
          )

          test (
              "jsx is_json with strict rejects trailing comma",
              fun _ -> assertThat (BJsx.isJsonWith """{"key": "value",}""" [ BJsx.strict ]) (isFalse)
          )

          test (
              "jsx format with indent",
              fun _ ->
                  let result = BJsx.formatWith """{"key":"value"}""" [ BJsx.indent 2 ]
                  // Formatted output should be longer than minified
                  assertThat ((String.length result > String.length """{"key":"value"}""")) (isTrue)
          )

          test (
              "jsx labels Binary keeps keys as binaries",
              fun _ ->
                  let decoded: obj =
                      BJsx.decodeWith """{"key":"value"}""" [ BJsx.labels BJsx.LabelMode.Binary ]

                  assertThat (firstKeyIsBinary decoded) (isTrue)
          )

          test (
              "jsx labels Atom converts keys to atoms",
              fun _ ->
                  let decoded: obj =
                      BJsx.decodeWith """{"key":"value"}""" [ BJsx.labels BJsx.LabelMode.Atom ]

                  assertThat (firstKeyIsAtom decoded) (isTrue)
          )

          test (
              "jsx labels Atom probe emits raw atom option",
              fun _ ->
                  let decoded: obj =
                      BJsx.decodeWith """{"atom_key_probe_xyz":"value"}""" [ BJsx.labels BJsx.LabelMode.Atom ]

                  assertThat (firstKeyIsAtom decoded) (isTrue)
          )

          test (
              "jsx labels ExistingAtom rejects unknown atoms",
              fun _ ->
                  // existing_atom uses binary_to_existing_atom, so a key whose atom was never created
                  // raises badarg rather than silently interning it. That rejection *is* the behaviour.
                  assertThat
                      (fun () ->
                          BJsx.decodeWith
                              """{"never_interned_key_qqq":"value"}"""
                              [ BJsx.labels BJsx.LabelMode.ExistingAtom ]
                          |> ignore)
                      throws
          )

          test (
              "jsx labels AttemptAtom is accepted",
              fun _ ->
                  // attempt_atom converts the key when the atom already exists and leaves it a binary
                  // otherwise. "key" is interned by the decode above, so it comes back as an atom.
                  let decoded: obj =
                      BJsx.decodeWith """{"key":"value"}""" [ BJsx.labels BJsx.LabelMode.AttemptAtom ]

                  assertThat (firstKeyIsAtom decoded) (isTrue)
          ) ]
    )
