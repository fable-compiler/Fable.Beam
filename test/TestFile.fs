module Fable.Beam.Tests.File

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core

module BFile = Fable.Beam.File

[<Emit("(fun() -> case file:make_symlink(binary_to_list($0), binary_to_list($1)) of ok -> {ok, ok}; {error, MakeSymlinkReason__} -> {error, erlang:atom_to_binary(MakeSymlinkReason__)} end end)()")>]
let private makeSymlink (target: string) (link: string) : Result<unit, string> = nativeOnly

let tests =
    testList (
        "File",
        [ test (
              "readFile and writeFile roundtrip",
              fun _ ->
                  let path = "/tmp/fable_beam_typed_test.txt"
                  let writeResult = BFile.writeFile path "typed hello"
                  assertThat writeResult (isEqualTo (Ok()))
                  let readResult = BFile.readFile path
                  assertThat readResult (isEqualTo (Ok "typed hello"))
                  BFile.delete path |> ignore
          )

          test (
              "readFile returns Error for missing file",
              fun _ ->
                  let result = BFile.readFile "/tmp/fable_beam_nonexistent_file.txt"
                  assertThat result (isEqualTo (Error "enoent"))
          )

          test (
              "writeFile and delete roundtrip",
              fun _ ->
                  let path = "/tmp/fable_beam_delete_test.txt"
                  BFile.writeFile path "to delete" |> ignore
                  let delResult = BFile.delete path
                  assertThat delResult (isEqualTo (Ok()))
                  let readResult = BFile.readFile path
                  assertThat readResult (isEqualTo (Error "enoent"))
          )

          test (
              "delete returns Error for missing file",
              fun _ ->
                  let result = BFile.delete "/tmp/fable_beam_nonexistent_delete.txt"
                  assertThat result (isEqualTo (Error "enoent"))
          )

          test (
              "makeDir and delDir",
              fun _ ->
                  let path = "/tmp/fable_beam_test_dir"
                  let mkResult = BFile.makeDir path
                  assertThat mkResult (isEqualTo (Ok()))
                  let delResult = BFile.delDir path
                  assertThat delResult (isEqualTo (Ok()))
          )

          test (
              "listDir returns files",
              fun _ ->
                  let dir = "/tmp/fable_beam_listdir_test"
                  BFile.makeDir dir |> ignore
                  BFile.writeFile (dir + "/a.txt") "a" |> ignore
                  BFile.writeFile (dir + "/b.txt") "b" |> ignore
                  let result = BFile.listDir dir

                  match result with
                  | Ok files -> assertThat ((List.length files >= 2)) (isTrue)
                  | Error e -> failwith "ok"
                  // cleanup
                  BFile.delete (dir + "/a.txt") |> ignore
                  BFile.delete (dir + "/b.txt") |> ignore
                  BFile.delDir dir |> ignore
          )

          test (
              "listDir returns Error for missing dir",
              fun _ ->
                  let result = BFile.listDir "/tmp/fable_beam_no_such_dir"
                  assertThat result (isEqualTo (Error "enoent"))
          )

          test (
              "rename moves a file",
              fun _ ->
                  let src = "/tmp/fable_beam_rename_src.txt"
                  let dst = "/tmp/fable_beam_rename_dst.txt"
                  BFile.writeFile src "rename me" |> ignore
                  let result = BFile.rename src dst
                  assertThat result (isEqualTo (Ok()))
                  assertThat (BFile.readFile dst) (isEqualTo (Ok "rename me"))
                  assertThat (BFile.readFile src) (isEqualTo (Error "enoent"))
                  BFile.delete dst |> ignore
          )

          test (
              "getCwd returns a path",
              fun _ ->
                  match BFile.getCwd () with
                  | Ok dir -> assertThat ((String.length dir > 0)) (isTrue)
                  | Error e -> failwith "ok"
          )

          test (
              "exists returns true for existing file",
              fun _ ->
                  let path = "/tmp/fable_beam_exists_test.txt"
                  BFile.writeFile path "exists" |> ignore
                  assertThat (BFile.exists path) (isTrue)
                  BFile.delete path |> ignore
          )

          test (
              "exists returns false for missing file",
              fun _ -> assertThat (BFile.exists "/tmp/fable_beam_no_such_file.txt") (isFalse)
          )

          test (
              "readFileInfo exposes kind, mode, and stable identity fields",
              fun _ ->
                  let path = "/tmp/fable_beam_file_info_test.txt"
                  BFile.writeFile path "metadata" |> ignore

                  match BFile.readFileInfo path with
                  | Ok info ->
                      assertThat info.kind (isEqualTo BFile.FileKind.Regular)
                      assertThat info.mode.IsSome (isTrue)
                      assertThat info.majorDevice.IsSome (isTrue)
                      assertThat info.minorDevice.IsSome (isTrue)
                      assertThat info.inode.IsSome (isTrue)
                  | Error reason -> failwithf "readFileInfo failed: %s" reason

                  BFile.delete path |> ignore
          )

          test (
              "readLinkInfo distinguishes symbolic links without following them",
              fun _ ->
                  let target = "/tmp/fable_beam_link_target.txt"
                  let link = "/tmp/fable_beam_link_info_test"
                  BFile.delete link |> ignore
                  BFile.writeFile target "target" |> ignore

                  match makeSymlink target link with
                  | Error reason -> failwithf "could not create symlink fixture: %s" reason
                  | Ok() ->
                      match BFile.readLinkInfo link with
                      | Ok info -> assertThat info.kind (isEqualTo BFile.FileKind.Symlink)
                      | Error reason -> failwithf "readLinkInfo failed: %s" reason

                  BFile.delete link |> ignore
                  BFile.delete target |> ignore
          )

          test (
              "readLinkInfo preserves missing-path errors",
              fun _ ->
                  let result =
                      BFile.readLinkInfo "/tmp/fable_beam_no_such_link_info_entry"

                  assertThat result (isEqualTo (Error "enoent"))
          ) ]
    )
