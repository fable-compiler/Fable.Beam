module Fable.Beam.Tests.Timer

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Beam.Timer

let tests =
    testList (
        "Timer",
        [ test ("hours converts correctly", fun _ -> assertThat (timer.hours 1) (isEqualTo 3600000))

          test ("minutes converts correctly", fun _ -> assertThat (timer.minutes 1) (isEqualTo 60000))

          test ("seconds converts correctly", fun _ -> assertThat (timer.seconds 1) (isEqualTo 1000))

          test ("sleep does not crash", fun _ -> timer.sleep 10) ]
    )
