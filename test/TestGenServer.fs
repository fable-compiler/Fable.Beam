module Fable.Beam.Tests.GenServer

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam

module BGenServer = Fable.Beam.GenServer

let tests =
    testList (
        "GenServer",
        [ test (
              "stop on non-existent catches error",
              fun _ ->
                  try
                      BGenServer.stop (BGenServer.ServerRef "nonexistent_process_xyz")
                  with _ ->
                      ()
          )

          test (
              "start_link returns ok with pid",
              fun _ ->
                  let result =
                      BGenServer.startLink (Erlang.binaryToAtom "test_counter_server") (box 0) []

                  match result with
                  | Ok pid -> assertThat (Erlang.isProcessAlive pid) (isEqualTo true)
                  | Error _ -> failwith "start_link should succeed"
          )

          test (
              "start returns ok with pid",
              fun _ ->
                  let result = BGenServer.start (Erlang.binaryToAtom "test_counter_server") (box 0) []

                  match result with
                  | Ok pid ->
                      assertThat (Erlang.isProcessAlive pid) (isEqualTo true)
                      BGenServer.stop (BGenServer.ServerRef pid)
                  | Error _ -> failwith "start should succeed"
          )

          test (
              "call gets state",
              fun _ ->
                  let result =
                      BGenServer.start (Erlang.binaryToAtom "test_counter_server") (box 42) []

                  match result with
                  | Ok pid ->
                      let value =
                          BGenServer.call (BGenServer.ServerRef pid) (box (Erlang.binaryToAtom "get"))

                      assertThat (Erlang.exactEquals value (box 42)) (isEqualTo true)
                      BGenServer.stop (BGenServer.ServerRef pid)
                  | Error _ -> failwith "start should succeed"
          )

          test (
              "call increment",
              fun _ ->
                  let result = BGenServer.start (Erlang.binaryToAtom "test_counter_server") (box 0) []

                  match result with
                  | Ok pid ->
                      let ref = BGenServer.ServerRef pid
                      let v1 = BGenServer.call ref (box (Erlang.binaryToAtom "increment"))
                      assertThat (Erlang.exactEquals v1 (box 1)) (isEqualTo true)
                      let v2 = BGenServer.call ref (box (Erlang.binaryToAtom "increment"))
                      assertThat (Erlang.exactEquals v2 (box 2)) (isEqualTo true)
                      BGenServer.stop ref
                  | Error _ -> failwith "start should succeed"
          )

          test (
              "call with timeout",
              fun _ ->
                  let result =
                      BGenServer.start (Erlang.binaryToAtom "test_counter_server") (box 10) []

                  match result with
                  | Ok pid ->
                      let ref = BGenServer.ServerRef pid

                      let value =
                          BGenServer.callWithTimeout ref (box (Erlang.binaryToAtom "get")) (U2.Case1 5000)

                      assertThat (Erlang.exactEquals value (box 10)) (isEqualTo true)
                      BGenServer.stop ref
                  | Error _ -> failwith "start should succeed"
          )

          test (
              "cast updates state",
              fun _ ->
                  let result = BGenServer.start (Erlang.binaryToAtom "test_counter_server") (box 0) []

                  match result with
                  | Ok pid ->
                      let ref = BGenServer.ServerRef pid
                      let setMsg: obj = emitErlExpr () "{set, 99}"
                      BGenServer.cast ref setMsg
                      // Small delay to let cast process
                      Fable.Beam.Timer.sleep 10
                      let value = BGenServer.call ref (box (Erlang.binaryToAtom "get"))
                      assertThat (Erlang.exactEquals value (box 99)) (isEqualTo true)
                      BGenServer.stop ref
                  | Error _ -> failwith "start should succeed"
          )

          test (
              "stop with reason and timeout",
              fun _ ->
                  let result = BGenServer.start (Erlang.binaryToAtom "test_counter_server") (box 0) []

                  match result with
                  | Ok pid ->
                      let ref = BGenServer.ServerRef pid
                      assertThat (Erlang.isProcessAlive pid) (isEqualTo true)
                      BGenServer.stopWith ref (Erlang.binaryToAtom "normal") (U2.Case1 5000)
                      // Process should be dead after stop
                      Fable.Beam.Timer.sleep 10
                      assertThat (Erlang.isProcessAlive pid) (isEqualTo false)
                  | Error _ -> failwith "start should succeed"
          ) ]
    )
