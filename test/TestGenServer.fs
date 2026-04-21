module Fable.Beam.Tests.GenServer

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.GenServer
#endif

[<Fact>]
let ``test gen_server.stop on non-existent catches error`` () =
#if FABLE_COMPILER
    try
        gen_server.stop (ServerRef "nonexistent_process_xyz")
    with _ ->
        ()
#else
    ()
#endif

[<Fact>]
let ``test gen_server.start_link returns ok with pid`` () =
#if FABLE_COMPILER
    let result =
        gen_server.start_link (Erlang.binaryToAtom "test_counter_server", box 0, [])

    match result with
    | Ok pid -> Erlang.isProcessAlive pid |> equal true
    | Error _ -> failwith "start_link should succeed"
#else
    ()
#endif

[<Fact>]
let ``test gen_server.start returns ok with pid`` () =
#if FABLE_COMPILER
    let result = gen_server.start (Erlang.binaryToAtom "test_counter_server", box 0, [])

    match result with
    | Ok pid ->
        Erlang.isProcessAlive pid |> equal true
        gen_server.stop (ServerRef pid)
    | Error _ -> failwith "start should succeed"
#else
    ()
#endif

[<Fact>]
let ``test gen_server.call gets state`` () =
#if FABLE_COMPILER
    let result =
        gen_server.start (Erlang.binaryToAtom "test_counter_server", box 42, [])

    match result with
    | Ok pid ->
        let value = gen_server.call (ServerRef pid, box (Erlang.binaryToAtom "get"))
        Erlang.exactEquals value (box 42) |> equal true
        gen_server.stop (ServerRef pid)
    | Error _ -> failwith "start should succeed"
#else
    ()
#endif

[<Fact>]
let ``test gen_server.call increment`` () =
#if FABLE_COMPILER
    let result = gen_server.start (Erlang.binaryToAtom "test_counter_server", box 0, [])

    match result with
    | Ok pid ->
        let ref = ServerRef pid
        let v1 = gen_server.call (ref, box (Erlang.binaryToAtom "increment"))
        Erlang.exactEquals v1 (box 1) |> equal true
        let v2 = gen_server.call (ref, box (Erlang.binaryToAtom "increment"))
        Erlang.exactEquals v2 (box 2) |> equal true
        gen_server.stop ref
    | Error _ -> failwith "start should succeed"
#else
    ()
#endif

[<Fact>]
let ``test gen_server.call with timeout`` () =
#if FABLE_COMPILER
    let result =
        gen_server.start (Erlang.binaryToAtom "test_counter_server", box 10, [])

    match result with
    | Ok pid ->
        let ref = ServerRef pid
        let value = gen_server.call (ref, box (Erlang.binaryToAtom "get"), U2.Case1 5000)
        Erlang.exactEquals value (box 10) |> equal true
        gen_server.stop ref
    | Error _ -> failwith "start should succeed"
#else
    ()
#endif

[<Fact>]
let ``test gen_server.cast updates state`` () =
#if FABLE_COMPILER
    let result = gen_server.start (Erlang.binaryToAtom "test_counter_server", box 0, [])

    match result with
    | Ok pid ->
        let ref = ServerRef pid
        let setMsg: obj = emitErlExpr () "{set, 99}"
        gen_server.cast (ref, setMsg)
        // Small delay to let cast process
        Fable.Beam.Timer.sleep 10
        let value = gen_server.call (ref, box (Erlang.binaryToAtom "get"))
        Erlang.exactEquals value (box 99) |> equal true
        gen_server.stop ref
    | Error _ -> failwith "start should succeed"
#else
    ()
#endif

[<Fact>]
let ``test gen_server.stop with reason and timeout`` () =
#if FABLE_COMPILER
    let result = gen_server.start (Erlang.binaryToAtom "test_counter_server", box 0, [])

    match result with
    | Ok pid ->
        let ref = ServerRef pid
        Erlang.isProcessAlive pid |> equal true
        gen_server.stop (ref, Erlang.binaryToAtom "normal", U2.Case1 5000)
        // Process should be dead after stop
        Fable.Beam.Timer.sleep 10
        Erlang.isProcessAlive pid |> equal false
    | Error _ -> failwith "start should succeed"
#else
    ()
#endif
