module Fable.Beam.Tests.Os

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Beam.Os
#endif

[<Fact>]
let ``test getenv returns None for unset var`` () =
#if FABLE_COMPILER
    getenv "FABLE_BEAM_TEST_UNSET_12345" |> equal None
#else
    ()
#endif

[<Fact>]
let ``test putenv and getenv roundtrip`` () =
#if FABLE_COMPILER
    putenv "FABLE_BEAM_TEST_VAR" "hello_beam"
    getenv "FABLE_BEAM_TEST_VAR" |> equal (Some "hello_beam")
    unsetenv "FABLE_BEAM_TEST_VAR"
#else
    ()
#endif

[<Fact>]
let ``test unsetenv removes a variable`` () =
#if FABLE_COMPILER
    putenv "FABLE_BEAM_TEST_UNSET" "temp"
    unsetenv "FABLE_BEAM_TEST_UNSET"
    getenv "FABLE_BEAM_TEST_UNSET" |> equal None
#else
    ()
#endif

[<Fact>]
let ``test getenv returns Some for HOME`` () =
#if FABLE_COMPILER
    match getenv "HOME" with
    | Some home ->
        (String.length home > 0) |> equal true
    | None ->
        // HOME should be set on any unix system
        equal "Some" "None"
#else
    ()
#endif

[<Fact>]
let ``test cmd runs a command`` () =
#if FABLE_COMPILER
    let result = cmd "echo hello"
    result |> equal "hello\n"
#else
    ()
#endif

[<Fact>]
let ``test systemTimeSeconds returns positive`` () =
#if FABLE_COMPILER
    let t = systemTimeSeconds ()
    (t > 0) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test systemTimeMs is monotonically increasing`` () =
#if FABLE_COMPILER
    let t1 = systemTimeMs ()
    let t2 = systemTimeMs ()
    (t2 >= t1) |> equal true
#else
    ()
#endif
