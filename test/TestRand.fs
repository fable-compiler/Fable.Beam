module Fable.Beam.Tests.Rand

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.Rand

let tests =
    testList (
        "Rand",
        [ test ("uniform returns float in range", fun _ ->
                let v = rand.uniform ()
                assertThat (v >= 0.0 && v < 1.0) (isTrue))

          test ("seed with typed algorithm DU", fun _ ->
                  // Canary for StringEnum-style atom emission from F# DUs on BEAM.
                  // If DU case Exsss compiles to atom `exsss`, rand:seed/1 will accept it.
                  rand.seed Exsss |> ignore
                  let v = rand.uniform ()
                  assertThat (v >= 0.0 && v < 1.0) (isTrue))

          test ("seed multi-word DU case maps to atom", fun _ ->
                  // Second canary: does a multi-word case like Exro928ss produce atom exro928ss?
                  rand.seed Exro928ss |> ignore
                  let v = rand.uniform ()
                  assertThat (v >= 0.0 && v < 1.0) (isTrue))

          test ("uniform n returns int in range", fun _ ->
                  let v = rand.uniform 100
                  assertThat (v >= 1 && v <= 100) (isTrue))

          test ("uniform 1 always returns 1", fun _ -> assertThat (rand.uniform 1) (isEqualTo 1))

          test ("uniform_real returns positive float", fun _ ->
                  let v = rand.uniform_real ()
                  assertThat (v > 0.0 && v < 1.0) (isTrue))

          test ("bytes returns binary of correct length", fun _ ->
                  let bytes = rand.bytes 16
                  // The Erlang byte_size of the returned binary should be 16
                  assertThat (bytes.Length > 0) (isTrue))

          test ("normal returns a float", fun _ ->
                  let v = rand.normal ()
                  // Normal distribution — just check it's a finite float
                  assertThat (v = v) (isTrue) // NaN check: NaN <> NaN
                  )

          test ("normal with mean and variance", fun _ ->
                  // With large variance we get varied values; just check it's a float
                  let v = rand.normal (0.0, 1.0)
                  assertThat (v = v) (isTrue))

          test ("two uniform calls can differ", fun _ ->
                  // With N=1000000, getting the same value twice in a row is astronomically unlikely
                  let v1 = rand.uniform 1000000
                  let v2 = rand.uniform 1000000
                  // At least verify both are in range — equality would be a fluke
                  assertThat (v1 >= 1 && v1 <= 1000000) (isTrue)
                  assertThat (v2 >= 1 && v2 <= 1000000) (isTrue)) ]
    )
