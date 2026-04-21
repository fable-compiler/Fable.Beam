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
    | Some home -> (String.length home > 0) |> equal true
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

[<Fact>]
let ``test systemTimeMs returns int64 value above 32-bit range`` () =
#if FABLE_COMPILER
    let t = systemTimeMs ()
    // Millisecond timestamps are around 1.7 * 10^12, well above int32 max (~2.1 * 10^9)
    (t > 1_000_000_000_000L) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test systemTimeSeconds returns int64`` () =
#if FABLE_COMPILER
    let t = systemTimeSeconds ()
    // Unix epoch seconds are around 1.7 * 10^9
    (t > 1_000_000_000L) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test osType returns a string tuple`` () =
#if FABLE_COMPILER
    let (family, _name) = osType ()
    // Should be "unix" on Linux/macOS or "win32" on Windows
    (family = "unix" || family = "win32") |> equal true
#else
    ()
#endif

[<Fact>]
let ``test version returns an int tuple`` () =
#if FABLE_COMPILER
    let (major, minor, release) = version ()
    (major >= 0) |> equal true
    (minor >= 0) |> equal true
    (release >= 0) |> equal true
#else
    ()
#endif
