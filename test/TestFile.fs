module Fable.Beam.Tests.File

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Beam.File

let tests =
    testList (
        "File",
        [ test ("readFile and writeFile roundtrip", fun _ ->
                  let path = "/tmp/fable_beam_typed_test.txt"
                  let writeResult = writeFile path "typed hello"
                  assertThat writeResult (isEqualTo (Ok ()))
                  let readResult = readFile path
                  assertThat readResult (isEqualTo (Ok "typed hello"))
                  delete path |> ignore
                  )

          test ("readFile returns Error for missing file", fun _ ->
                  let result = readFile "/tmp/fable_beam_nonexistent_file.txt"
                  assertThat result (isEqualTo (Error "enoent"))
                  )

          test ("writeFile and delete roundtrip", fun _ ->
                  let path = "/tmp/fable_beam_delete_test.txt"
                  writeFile path "to delete" |> ignore
                  let delResult = delete path
                  assertThat delResult (isEqualTo (Ok ()))
                  let readResult = readFile path
                  assertThat readResult (isEqualTo (Error "enoent"))
                  )

          test ("delete returns Error for missing file", fun _ ->
                  let result = delete "/tmp/fable_beam_nonexistent_delete.txt"
                  assertThat result (isEqualTo (Error "enoent"))
                  )

          test ("makeDir and delDir", fun _ ->
                  let path = "/tmp/fable_beam_test_dir"
                  let mkResult = makeDir path
                  assertThat mkResult (isEqualTo (Ok ()))
                  let delResult = delDir path
                  assertThat delResult (isEqualTo (Ok ()))
                  )

          test ("listDir returns files", fun _ ->
                  let dir = "/tmp/fable_beam_listdir_test"
                  makeDir dir |> ignore
                  writeFile (dir + "/a.txt") "a" |> ignore
                  writeFile (dir + "/b.txt") "b" |> ignore
                  let result = listDir dir

                  match result with
                  | Ok files -> assertThat ((List.length files >= 2)) (isTrue)
                  | Error e -> failwith "ok"
                  // cleanup
                  delete (dir + "/a.txt") |> ignore
                  delete (dir + "/b.txt") |> ignore
                  delDir dir |> ignore
                  )

          test ("listDir returns Error for missing dir", fun _ ->
                  let result = listDir "/tmp/fable_beam_no_such_dir"
                  assertThat result (isEqualTo (Error "enoent"))
                  )

          test ("rename moves a file", fun _ ->
                  let src = "/tmp/fable_beam_rename_src.txt"
                  let dst = "/tmp/fable_beam_rename_dst.txt"
                  writeFile src "rename me" |> ignore
                  let result = rename src dst
                  assertThat result (isEqualTo (Ok ()))
                  assertThat (readFile dst) (isEqualTo (Ok "rename me"))
                  assertThat (readFile src) (isEqualTo (Error "enoent"))
                  delete dst |> ignore
                  )

          test ("getCwd returns a path", fun _ ->
                  match getCwd () with
                  | Ok dir -> assertThat ((String.length dir > 0)) (isTrue)
                  | Error e -> failwith "ok"
                  )

          test ("exists returns true for existing file", fun _ ->
                  let path = "/tmp/fable_beam_exists_test.txt"
                  writeFile path "exists" |> ignore
                  assertThat (exists path) (isTrue)
                  delete path |> ignore
                  )

          test ("exists returns false for missing file", fun _ ->
                  assertThat (exists "/tmp/fable_beam_no_such_file.txt") (isFalse)) ]
    )
