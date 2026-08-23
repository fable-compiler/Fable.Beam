module Fable.Beam.Tests.Supervisor

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Beam

module BSupervisor = Fable.Beam.Supervisor

#if FABLE_COMPILER
// Starts a fresh test_basic_sup supervisor (one temporary `counter` child) and
// returns a SupRef to it.
let private startSup () : BSupervisor.SupRef =
    match BSupervisor.startLink (Erlang.binaryToAtom "test_basic_sup") [] with
    | Ok pid -> BSupervisor.fromPid pid
    | Error _ -> failwith "test_basic_sup should start"
#endif

let tests =
    testList (
        "Supervisor",
        [ test (
              "supervisor.which_children on non-existent catches error",
              fun _ ->
                  try
                      BSupervisor.children (BSupervisor.fromName (Erlang.binaryToAtom "nonexistent_sup_xyz"))
                      |> ignore
                  with _ ->
                      ()
          )

          test (
              "supervisor.terminate_child succeeds for a running child",
              fun _ ->
                  let sup = startSup ()

                  // Bare `ok` from OTP must surface as Ok () on the F# side.
                  match BSupervisor.terminateChild sup (Erlang.binaryToAtom "counter") with
                  | Ok() -> assertThat true (isTrue)
                  | Error _ -> failwith "terminate_child should succeed for a known child"
          )

          test (
              "supervisor.terminate_child returns Error not_found for unknown id",
              fun _ ->
                  let sup = startSup ()

                  match BSupervisor.terminateChild sup (Erlang.binaryToAtom "nope") with
                  | Ok() -> failwith "terminate_child should fail for an unknown child"
                  | Error reason -> assertThat reason (isEqualTo (Erlang.binaryToAtom "not_found"))
          )

          test (
              "supervisor.delete_child succeeds after terminate",
              fun _ ->
                  let sup = startSup ()
                  let counter = Erlang.binaryToAtom "counter"

                  // A child spec can only be deleted once the child is terminated.
                  BSupervisor.terminateChild sup counter |> ignore

                  match BSupervisor.deleteChild sup counter with
                  | Ok() -> assertThat true (isTrue)
                  | Error _ -> failwith "delete_child should succeed for a terminated child"
          )

          test (
              "supervisor.delete_child returns Error not_found for unknown id",
              fun _ ->
                  let sup = startSup ()

                  match BSupervisor.deleteChild sup (Erlang.binaryToAtom "nope") with
                  | Ok() -> failwith "delete_child should fail for an unknown child"
                  | Error reason -> assertThat reason (isEqualTo (Erlang.binaryToAtom "not_found"))
          ) ]
    )
