module Fable.Beam.Tests.Math

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Math

let tests =
    testList (
        "Math",
        [ test (
              "pi returns pi",
              fun _ ->
                  let piValue = pi ()
                  // pi is approximately 3.14159
                  assertThat (piValue > 3.14 && piValue < 3.15) (isTrue)
          )

          test ("sin of zero is zero", fun _ -> assertThat (sin 0.0) (isEqualTo 0.0))

          test ("cos of zero is one", fun _ -> assertThat (cos 0.0) (isEqualTo 1.0))

          test ("sqrt of four is two", fun _ -> assertThat (sqrt 4.0) (isEqualTo 2.0))

          test ("pow computes power", fun _ -> assertThat (pow 2.0 10.0) (isEqualTo 1024.0))

          test ("exp of zero is one", fun _ -> assertThat (exp 0.0) (isEqualTo 1.0))

          test (
              "log of e is one",
              fun _ ->
                  let e = exp 1.0
                  let result = log e
                  // result should be approximately 1.0
                  assertThat (result > 0.9999 && result < 1.0001) (isTrue)
          )

          test (
              "log2 of eight is three",
              fun _ ->
                  let result = log2 8.0
                  assertThat (result > 2.9999 && result < 3.0001) (isTrue)
          )

          test (
              "log10 of one hundred is two",
              fun _ ->
                  let result = log10 100.0
                  assertThat (result > 1.9999 && result < 2.0001) (isTrue)
          )

          test ("floor rounds down", fun _ -> assertThat (floor 3.9) (isEqualTo 3.0))

          test ("ceil rounds up", fun _ -> assertThat (ceil 3.1) (isEqualTo 4.0))

          test (
              "atan2 quadrant",
              fun _ ->
                  // atan2(1, 1) = pi/4 approximately 0.785
                  let result = atan2 1.0 1.0
                  assertThat (result > 0.78 && result < 0.79) (isTrue)
          ) ]
    )
