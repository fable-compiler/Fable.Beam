module Fable.Beam.Tests.Timer

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

module BTimer = Fable.Beam.Timer

let tests =
    testList (
        "Timer",
        [ test ("hours converts correctly", fun _ -> assertThat (BTimer.hours 1) (isEqualTo 3600000))

          test ("minutes converts correctly", fun _ -> assertThat (BTimer.minutes 1) (isEqualTo 60000))

          test ("seconds converts correctly", fun _ -> assertThat (BTimer.seconds 1) (isEqualTo 1000))

          test ("sleep does not crash", fun _ -> BTimer.sleep 10) ]
    )
