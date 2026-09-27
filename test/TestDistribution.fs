module Fable.Beam.Tests.Distribution

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Beam

[<Emit("node()")>]
let private currentNode () : Atom = nativeOnly

let tests =
    testList (
        "Distribution",
        [
            test (
                "connectNode reports that a non-distributed node is not alive",
                fun _ ->
                    let result = NetKernel.connectNode (currentNode ())
                    assertThat result (isEqualTo NetKernel.ConnectNodeResult.NotAlive)
            )

            test (
                "rpc call returns and decodes a remote result",
                fun _ ->
                    let argument = Dynamic.ofValue (Atom.ofString "ok")

                    let result =
                        Rpc.call (currentNode ()) (Atom.ofString "erlang") (Atom.ofString "is_atom") [ argument ] 1000

                    match result with
                    | Error reason -> failwithf "rpc call failed: %s" reason
                    | Ok value ->
                        match Decode.bool value with
                        | Ok isAtom -> assertThat isAtom (isTrue)
                        | Error reason -> failwithf "could not decode rpc result: %s" reason
            )

            test (
                "rpc call maps badrpc to Error",
                fun _ ->
                    let result =
                        Rpc.call
                            (currentNode ())
                            (Atom.ofString "erlang")
                            (Atom.ofString "definitely_missing_function")
                            []
                            1000

                    match result with
                    | Error _ -> assertThat true (isTrue)
                    | Ok _ -> failwith "missing remote function should return Error"
            )

            test (
                "rpc whereis preserves registered and missing results",
                fun _ ->
                    let registeredName = Atom.ofString "fable_beam_rpc_test_process"
                    let missingName = Atom.ofString "fable_beam_rpc_missing_process"
                    let currentPid: Pid<string> = Fable.Beam.Erlang.self ()
                    Fable.Beam.Erlang.register registeredName currentPid

                    match Rpc.whereis<string> (currentNode ()) registeredName 1000 with
                    | Ok(Some registeredPid) ->
                        assertThat (Fable.Beam.Erlang.exactEquals registeredPid currentPid) (isTrue)
                    | Ok None -> failwith "registered remote process was not found"
                    | Error reason -> failwithf "remote whereis failed: %s" reason

                    match Rpc.whereis<string> (currentNode ()) missingName 1000 with
                    | Ok None -> assertThat true (isTrue)
                    | Ok(Some _) -> failwith "missing remote process unexpectedly resolved"
                    | Error reason -> failwithf "remote whereis failed: %s" reason
            )
        ]
    )
