module Fable.Beam.Tests.File

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core.BeamInterop
open Fable.Beam.File
#endif

// ============================================================================
// Raw bindings
// ============================================================================

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

// ============================================================================
// Typed API
// ============================================================================

[<Fact>]
let ``test readFile and writeFile roundtrip`` () =
#if FABLE_COMPILER
    let path = "/tmp/fable_beam_typed_test.txt"
    let writeResult = writeFile path "typed hello"
    writeResult |> equal (Ok ())
    let readResult = readFile path
    readResult |> equal (Ok "typed hello")
    delete path |> ignore
#else
    ()
#endif

[<Fact>]
let ``test readFile returns Error for missing file`` () =
#if FABLE_COMPILER
    let result = readFile "/tmp/fable_beam_nonexistent_file.txt"
    result |> equal (Error "enoent")
#else
    ()
#endif

[<Fact>]
let ``test writeFile and delete roundtrip`` () =
#if FABLE_COMPILER
    let path = "/tmp/fable_beam_delete_test.txt"
    writeFile path "to delete" |> ignore
    let delResult = delete path
    delResult |> equal (Ok ())
    let readResult = readFile path
    readResult |> equal (Error "enoent")
#else
    ()
#endif

[<Fact>]
let ``test delete returns Error for missing file`` () =
#if FABLE_COMPILER
    let result = delete "/tmp/fable_beam_nonexistent_delete.txt"
    result |> equal (Error "enoent")
#else
    ()
#endif

[<Fact>]
let ``test makeDir and delDir`` () =
#if FABLE_COMPILER
    let path = "/tmp/fable_beam_test_dir"
    let mkResult = makeDir path
    mkResult |> equal (Ok ())
    let delResult = delDir path
    delResult |> equal (Ok ())
#else
    ()
#endif

[<Fact>]
let ``test listDir returns files`` () =
#if FABLE_COMPILER
    let dir = "/tmp/fable_beam_listdir_test"
    makeDir dir |> ignore
    writeFile (dir + "/a.txt") "a" |> ignore
    writeFile (dir + "/b.txt") "b" |> ignore
    let result = listDir dir
    match result with
    | Ok files ->
        (List.length files >= 2) |> equal true
    | Error e ->
        equal "ok" e
    // cleanup
    delete (dir + "/a.txt") |> ignore
    delete (dir + "/b.txt") |> ignore
    delDir dir |> ignore
#else
    ()
#endif

[<Fact>]
let ``test listDir returns Error for missing dir`` () =
#if FABLE_COMPILER
    let result = listDir "/tmp/fable_beam_no_such_dir"
    result |> equal (Error "enoent")
#else
    ()
#endif

[<Fact>]
let ``test rename moves a file`` () =
#if FABLE_COMPILER
    let src = "/tmp/fable_beam_rename_src.txt"
    let dst = "/tmp/fable_beam_rename_dst.txt"
    writeFile src "rename me" |> ignore
    let result = rename src dst
    result |> equal (Ok ())
    readFile dst |> equal (Ok "rename me")
    readFile src |> equal (Error "enoent")
    delete dst |> ignore
#else
    ()
#endif

[<Fact>]
let ``test getCwd returns a path`` () =
#if FABLE_COMPILER
    match getCwd () with
    | Ok dir ->
        (String.length dir > 0) |> equal true
    | Error e ->
        equal "ok" e
#else
    ()
#endif

[<Fact>]
let ``test exists returns true for existing file`` () =
#if FABLE_COMPILER
    let path = "/tmp/fable_beam_exists_test.txt"
    writeFile path "exists" |> ignore
    exists path |> equal true
    delete path |> ignore
#else
    ()
#endif

[<Fact>]
let ``test exists returns false for missing file`` () =
#if FABLE_COMPILER
    exists "/tmp/fable_beam_no_such_file.txt" |> equal false
#else
    ()
#endif
