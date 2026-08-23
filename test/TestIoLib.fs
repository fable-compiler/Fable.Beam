module Fable.Beam.Tests.IoLib

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Beam
open Fable.Beam.IoLib

let tests =
    testList (
        "IoLib",
        [ test ("format renders a string", fun _ -> assertThat (format "~s-~p" [ box "x"; box 42 ]) (isEqualTo "x-42"))

          test (
              "formatRaw returns unflattened chardata that flattens to format",
              fun _ ->
                  let raw = formatRaw "~s-~p" [ box "x"; box 42 ]
                  assertThat (Erlang.isList raw) (isTrue)
                  assertThat (BeamChardata.toString raw) (isEqualTo "x-42")
                  assertThat (BeamChardata.toString raw) (isEqualTo (format "~s-~p" [ box "x"; box 42 ]))
          ) ]
    )
