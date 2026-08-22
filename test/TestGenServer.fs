module Fable.Beam.Tests.GenServer

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.GenServer

let tests =
    testList (
        "GenServer",
        [ test (
              "stop on non-existent catches error",
              fun _ ->
                  try
                      gen_server.stop (ServerRef "nonexistent_process_xyz")
                  with _ ->
                      ()
          )

          test (
              "start_link returns ok with pid",
              fun _ ->
                  let result =
                      gen_server.start_link (Erlang.binaryToAtom "test_counter_server", box 0, [])

                  match result with
                  | Ok pid -> assertThat (Erlang.isProcessAlive pid) (isEqualTo true)
                  | Error _ -> failwith "start_link should succeed"
          )

          test (
              "start returns ok with pid",
              fun _ ->
                  let result = gen_server.start (Erlang.binaryToAtom "test_counter_server", box 0, [])

                  match result with
                  | Ok pid ->
                      assertThat (Erlang.isProcessAlive pid) (isEqualTo true)
                      gen_server.stop (ServerRef pid)
                  | Error _ -> failwith "start should succeed"
          )

          test (
              "call gets state",
              fun _ ->
                  let result =
                      gen_server.start (Erlang.binaryToAtom "test_counter_server", box 42, [])

                  match result with
                  | Ok pid ->
                      let value = gen_server.call (ServerRef pid, box (Erlang.binaryToAtom "get"))
                      assertThat (Erlang.exactEquals value (box 42)) (isEqualTo true)
                      gen_server.stop (ServerRef pid)
                  | Error _ -> failwith "start should succeed"
          )

          test (
              "call increment",
              fun _ ->
                  let result = gen_server.start (Erlang.binaryToAtom "test_counter_server", box 0, [])

                  match result with
                  | Ok pid ->
                      let ref = ServerRef pid
                      let v1 = gen_server.call (ref, box (Erlang.binaryToAtom "increment"))
                      assertThat (Erlang.exactEquals v1 (box 1)) (isEqualTo true)
                      let v2 = gen_server.call (ref, box (Erlang.binaryToAtom "increment"))
                      assertThat (Erlang.exactEquals v2 (box 2)) (isEqualTo true)
                      gen_server.stop ref
                  | Error _ -> failwith "start should succeed"
          )

          test (
              "call with timeout",
              fun _ ->
                  let result =
                      gen_server.start (Erlang.binaryToAtom "test_counter_server", box 10, [])

                  match result with
                  | Ok pid ->
                      let ref = ServerRef pid
                      let value = gen_server.call (ref, box (Erlang.binaryToAtom "get"), U2.Case1 5000)
                      assertThat (Erlang.exactEquals value (box 10)) (isEqualTo true)
                      gen_server.stop ref
                  | Error _ -> failwith "start should succeed"
          )

          test (
              "cast updates state",
              fun _ ->
                  let result = gen_server.start (Erlang.binaryToAtom "test_counter_server", box 0, [])

                  match result with
                  | Ok pid ->
                      let ref = ServerRef pid
                      let setMsg: obj = emitErlExpr () "{set, 99}"
                      gen_server.cast (ref, setMsg)
                      // Small delay to let cast process
                      Fable.Beam.Timer.sleep 10
                      let value = gen_server.call (ref, box (Erlang.binaryToAtom "get"))
                      assertThat (Erlang.exactEquals value (box 99)) (isEqualTo true)
                      gen_server.stop ref
                  | Error _ -> failwith "start should succeed"
          )

          test (
              "stop with reason and timeout",
              fun _ ->
                  let result = gen_server.start (Erlang.binaryToAtom "test_counter_server", box 0, [])

                  match result with
                  | Ok pid ->
                      let ref = ServerRef pid
                      assertThat (Erlang.isProcessAlive pid) (isEqualTo true)
                      gen_server.stop (ref, Erlang.binaryToAtom "normal", U2.Case1 5000)
                      // Process should be dead after stop
                      Fable.Beam.Timer.sleep 10
                      assertThat (Erlang.isProcessAlive pid) (isEqualTo false)
                  | Error _ -> failwith "start should succeed"
          ) ]
    )
