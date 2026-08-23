module Fable.Beam.Tests.Io

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

module BIo = Fable.Beam.Io

let tests =
    testList (
        "Io",
        [ test ("putChars does not crash", fun _ -> assertThat (fun () -> BIo.putChars "test output\n") doesNotThrow)

          test (
              "format does not crash",
              fun _ -> assertThat (fun () -> BIo.format "hello ~s~n" [ box "beam" ]) doesNotThrow
          ) ]
    )
