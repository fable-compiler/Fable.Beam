module Fable.Beam.Tests.Process

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam

type WorkerMessage = Finish

type ReadyMessage = WorkerReady of Pid<WorkerMessage>

type MonitorMessage = | [<CompiledName("DOWN")>] Down of Ref<Pid<WorkerMessage>> * Atom * Pid<WorkerMessage> * Atom

type AliasMessage = AliasValue of int

type BarrierMessage = Barrier

type UnrelatedMessage = Unrelated of string

type RevocationMessage = RevocationResult of bool

let private assertDown pid monitorRef reason =
    match Erlang.receive<MonitorMessage> 5000 with
    | Some(Down(actualRef, kind, actualPid, actualReason)) ->
        assertThat (Erlang.exactEquals actualRef monitorRef) (isTrue)
        assertThat (Erlang.exactEquals actualPid pid) (isTrue)
        assertThat (Atom.toString kind) (isEqualTo "process")
        assertThat (Atom.toString actualReason) (isEqualTo reason)
    | None -> failwith "expected the monitored worker's DOWN message"

let private assertValue expected =
    match Erlang.receive<AliasMessage> 5000 with
    | Some(AliasValue value) -> assertThat value (isEqualTo expected)
    | None -> failwith "expected a message sent through the alias"

let private assertNoAliasMessage () =
    assertThat (Erlang.receive<AliasMessage> 0) (isEqualTo None)

let private awaitBarrier () =
    match Erlang.receive<BarrierMessage> 5000 with
    | Some Barrier -> ()
    | None -> failwith "expected the sender's barrier"

let private exitImmediately () = ()

let private spawnCallback (callback: unit -> unit) =
    Erlang.spawnMonitor<WorkerMessage> callback

// invariant: a barrier follows alias sends from the same process so absence checks do not race delivery
let private sendValues alias =
    let owner = Erlang.self<BarrierMessage>()

    let pid, monitorRef =
        Erlang.spawnMonitor<WorkerMessage>(fun () ->
            Erlang.sendAlias alias (AliasValue 1)
            Erlang.sendAlias alias (AliasValue 2)
            Erlang.send owner Barrier)

    awaitBarrier ()
    assertDown pid monitorRef "normal"

let tests =
    testList (
        "Process",
        [
            test (
                "spawnMonitor observes a worker exiting after a handshake",
                fun _ ->
                    let owner = Erlang.self<ReadyMessage>()

                    let pid, monitorRef =
                        Erlang.spawnMonitor<WorkerMessage>(fun () ->
                            Erlang.send owner (WorkerReady(Erlang.self<WorkerMessage>()))

                            match Erlang.receive<WorkerMessage> 5000 with
                            | Some Finish -> ()
                            | None -> Erlang.exit (Atom.ofString "worker_timeout"))

                    match Erlang.receive<ReadyMessage> 5000 with
                    | Some(WorkerReady readyPid) -> assertThat (Erlang.exactEquals readyPid pid) (isTrue)
                    | None -> failwith "expected the worker's ready message"

                    Erlang.send pid Finish
                    assertDown pid monitorRef "normal"
            )

            test (
                "spawnMonitor observes immediate exit of a named F# callback",
                fun _ ->
                    let pid, monitorRef = Erlang.spawnMonitor<WorkerMessage> exitImmediately
                    assertDown pid monitorRef "normal"
                    assertThat (Erlang.isProcessAlive pid) (isFalse)
            )

            test (
                "spawnMonitor preserves an immediate worker's exit reason",
                fun _ ->
                    let pid, monitorRef =
                        Erlang.spawnMonitor<WorkerMessage>(fun () -> Erlang.exit (Atom.ofString "worker_failed"))

                    assertDown pid monitorRef "worker_failed"
            )

            test (
                "spawnMonitor accepts a forwarded F# callback with a captured alias",
                fun _ ->
                    let alias = Erlang.alias<AliasMessage>()
                    let value = 42

                    let pid, monitorRef =
                        spawnCallback (fun () -> Erlang.sendAlias alias (AliasValue value))

                    assertValue value
                    assertDown pid monitorRef "normal"
                    assertThat (Erlang.unalias alias) (isTrue)
            )

            test (
                "alias delivers messages from another process to its creator",
                fun _ ->
                    let alias = Erlang.alias<AliasMessage>()
                    sendValues alias
                    assertValue 1
                    assertValue 2
                    assertThat (Erlang.unalias alias) (isTrue)
            )

            test (
                "unalias returns true once and false afterward",
                fun _ ->
                    let alias = Erlang.alias<AliasMessage>()
                    assertThat (Erlang.unalias alias) (isTrue)
                    assertThat (Erlang.unalias alias) (isFalse)
            )

            test (
                "revocation drops later delivery and preserves queued and unrelated messages",
                fun _ ->
                    let alias = Erlang.alias<AliasMessage>()
                    Erlang.sendAlias alias (AliasValue 42)
                    Erlang.send (Erlang.self<UnrelatedMessage>()) (Unrelated "keep")
                    Erlang.send (Erlang.self<BarrierMessage>()) Barrier
                    awaitBarrier ()
                    assertThat (Erlang.unalias alias) (isTrue)
                    sendValues alias
                    assertValue 42
                    assertThat (Erlang.receive<UnrelatedMessage> 0) (isEqualTo (Some(Unrelated "keep")))
                    assertNoAliasMessage ()
            )

            test (
                "empty alias options use explicit deactivation",
                fun _ ->
                    let alias = Erlang.aliasWithOptions<AliasMessage> []
                    sendValues alias
                    assertValue 1
                    assertValue 2
                    assertThat (Erlang.unalias alias) (isTrue)
            )

            test (
                "ExplicitUnalias stays active across multiple messages until revoked",
                fun _ ->
                    let alias =
                        Erlang.aliasWithOptions<AliasMessage> [ Erlang.AliasOption.ExplicitUnalias ]

                    sendValues alias
                    assertValue 1
                    assertValue 2
                    assertThat (Erlang.unalias alias) (isTrue)
                    sendValues alias
                    assertNoAliasMessage ()
            )

            test (
                "Reply deactivates on first delivery before the message is consumed",
                fun _ ->
                    let alias = Erlang.aliasWithOptions<AliasMessage> [ Erlang.AliasOption.Reply ]
                    sendValues alias
                    assertThat (Erlang.unalias alias) (isFalse)
                    assertValue 1
                    assertNoAliasMessage ()
            )

            test (
                "Reply allows explicit revocation before a reply arrives",
                fun _ ->
                    let alias = Erlang.aliasWithOptions<AliasMessage> [ Erlang.AliasOption.Reply ]
                    assertThat (Erlang.unalias alias) (isTrue)
                    sendValues alias
                    assertNoAliasMessage ()
            )

            test (
                "another process cannot revoke the creator's alias",
                fun _ ->
                    let alias = Erlang.alias<AliasMessage>()
                    let owner = Erlang.self<RevocationMessage>()

                    let pid, monitorRef =
                        Erlang.spawnMonitor<WorkerMessage>(fun () ->
                            Erlang.send owner (RevocationResult(Erlang.unalias alias))
                            Erlang.sendAlias alias (AliasValue 42))

                    assertThat (Erlang.receive<RevocationMessage> 5000) (isEqualTo (Some(RevocationResult false)))
                    assertValue 42
                    assertDown pid monitorRef "normal"
                    assertThat (Erlang.unalias alias) (isTrue)
            )
        ]
    )
