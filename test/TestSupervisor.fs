module Fable.Beam.Tests.Supervisor

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Beam
open Fable.Beam.Supervisor
#endif

[<Fact>]
let ``test supervisor.which_children on non-existent catches error`` () =
#if FABLE_COMPILER
    try
        supervisor.which_children (fromName (Fable.Beam.Erlang.binaryToAtom "nonexistent_sup_xyz"))
        |> ignore
    with _ ->
        ()
#else
    ()
#endif

#if FABLE_COMPILER
// Starts a fresh test_basic_sup supervisor (one temporary `counter` child) and
// returns a SupRef to it.
let private startSup () : SupRef =
    match supervisor.start_link (Erlang.binaryToAtom "test_basic_sup", []) with
    | Ok pid -> fromPid pid
    | Error _ -> failwith "test_basic_sup should start"
#endif

[<Fact>]
let ``test supervisor.terminate_child succeeds for a running child`` () =
#if FABLE_COMPILER
    let sup = startSup ()

    // Bare `ok` from OTP must surface as Ok () on the F# side.
    match supervisor.terminate_child (sup, Erlang.binaryToAtom "counter") with
    | Ok() -> ()
    | Error _ -> failwith "terminate_child should succeed for a known child"
#else
    ()
#endif

[<Fact>]
let ``test supervisor.terminate_child returns Error not_found for unknown id`` () =
#if FABLE_COMPILER
    let sup = startSup ()

    match supervisor.terminate_child (sup, Erlang.binaryToAtom "nope") with
    | Ok() -> failwith "terminate_child should fail for an unknown child"
    | Error reason -> reason |> equal (Erlang.binaryToAtom "not_found")
#else
    ()
#endif

[<Fact>]
let ``test supervisor.delete_child succeeds after terminate`` () =
#if FABLE_COMPILER
    let sup = startSup ()
    let counter = Erlang.binaryToAtom "counter"

    // A child spec can only be deleted once the child is terminated.
    supervisor.terminate_child (sup, counter) |> ignore

    match supervisor.delete_child (sup, counter) with
    | Ok() -> ()
    | Error _ -> failwith "delete_child should succeed for a terminated child"
#else
    ()
#endif

[<Fact>]
let ``test supervisor.delete_child returns Error not_found for unknown id`` () =
#if FABLE_COMPILER
    let sup = startSup ()

    match supervisor.delete_child (sup, Erlang.binaryToAtom "nope") with
    | Ok() -> failwith "delete_child should fail for an unknown child"
    | Error reason -> reason |> equal (Erlang.binaryToAtom "not_found")
#else
    ()
#endif
