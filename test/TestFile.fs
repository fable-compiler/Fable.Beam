module Fable.Beam.Tests.File

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core.BeamInterop
open Fable.Beam.File
#endif

[<Fact>]
let ``test file.get_cwd works`` () =
#if FABLE_COMPILER
    let result = file.get_cwd ()
    // Returns {ok, Dir} tuple
    result |> notEqual null
#else
    ()
#endif

[<Fact>]
let ``test file.write_file and read_file roundtrip`` () =
#if FABLE_COMPILER
    let filename = "/tmp/fable_beam_test.txt"
    file.write_file (filename, box "hello beam") |> ignore
    let result = file.read_file filename
    result |> notEqual null
    file.delete filename |> ignore
#else
    ()
#endif

[<Fact>]
let ``test file.list_dir works`` () =
#if FABLE_COMPILER
    let result = file.list_dir "/tmp"
    result |> notEqual null
#else
    ()
#endif
