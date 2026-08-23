module Fable.Beam.Tests.Io

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Beam.Io

let tests =
    testList (
        "Io",
        [ test ("put_chars works", fun _ ->
                assertThat (fun () -> io.put_chars "test output\n") doesNotThrow)

          test ("putChars does not crash", fun _ ->
                  assertThat (fun () -> putChars "typed putChars test\n") doesNotThrow)

          test ("format does not crash", fun _ ->
                  assertThat (fun () -> format "hello ~s~n" [ box "beam" ]) doesNotThrow) ]
    )
