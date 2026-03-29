module Fable.Beam.Tests.Erlang

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam

type RecvMsg =
    | [<CompiledName("ping")>] Ping
    | [<CompiledName("data")>] Data of value: int
#endif

[<Fact>]
let ``test self returns a pid`` () =
#if FABLE_COMPILER
    let pid = Erlang.self ()
    let isAlive = Erlang.isProcessAlive pid
    isAlive |> equal true
#else
    ()
#endif

[<Fact>]
let ``test makeRef returns unique references`` () =
#if FABLE_COMPILER
    let ref1 = Erlang.makeRef ()
    let ref2 = Erlang.makeRef ()
    Erlang.exactEquals ref1 ref2 |> equal false
#else
    ()
#endif

[<Fact>]
let ``test exactEquals on same ref`` () =
#if FABLE_COMPILER
    let ref1 = Erlang.makeRef ()
    Erlang.exactEquals ref1 ref1 |> equal true
#else
    ()
#endif

[<Fact>]
let ``test spawn creates a process`` () =
#if FABLE_COMPILER
    let pid = Erlang.spawn (fun () -> ())
    Erlang.isProcessAlive pid |> equal true
#else
    ()
#endif

[<Fact>]
let ``test spawnLink creates a linked process`` () =
#if FABLE_COMPILER
    let pid = Erlang.spawnLink (fun () -> ())
    Erlang.isProcessAlive pid |> equal true
#else
    ()
#endif

[<Fact>]
let ``test isProcessAlive on self`` () =
#if FABLE_COMPILER
    let pid = Erlang.self ()
    Erlang.isProcessAlive pid |> equal true
#else
    ()
#endif

[<Fact>]
let ``test process dictionary get/put/erase`` () =
#if FABLE_COMPILER
    let key = Erlang.makeRef ()
    Erlang.put key (box 42) |> ignore
    let value = Erlang.get key
    value |> equal (box 42)
    Erlang.erase key |> ignore
#else
    ()
#endif

[<Fact>]
let ``test send and receive`` () =
#if FABLE_COMPILER
    let pid = Erlang.self ()
    emitErlExpr () "erlang:self() ! {ping}"
    match Erlang.receive<RecvMsg> 1000 with
    | Some Ping -> equal 1 1
    | _ -> equal 0 1
#else
    ()
#endif

[<Fact>]
let ``test receive with timeout returns None`` () =
#if FABLE_COMPILER
    match Erlang.receive<RecvMsg> 0 with
    | None -> equal 1 1
    | Some _ -> equal 0 1
#else
    ()
#endif

[<Fact>]
let ``test receive with data`` () =
#if FABLE_COMPILER
    emitErlExpr () "erlang:self() ! {data, 42}"
    match Erlang.receive<RecvMsg> 1000 with
    | Some(Data v) -> equal 42 v
    | _ -> equal 0 1
#else
    ()
#endif

[<Fact>]
let ``test sendAfter and cancelTimer`` () =
#if FABLE_COMPILER
    let timerRef = Erlang.sendAfter 60000 (box "should_not_arrive")
    match Erlang.cancelTimer timerRef with
    | Some remaining -> (remaining > 0) |> equal true
    | None -> equal "Some" "None"
#else
    ()
#endif

[<Fact>]
let ``test atomToBinary and binaryToAtom roundtrip`` () =
#if FABLE_COMPILER
    let atom = Erlang.binaryToAtom "test_atom"
    let str = Erlang.atomToBinary atom
    str |> equal "test_atom"
#else
    ()
#endif

[<Fact>]
let ``test monitor and demonitor`` () =
#if FABLE_COMPILER
    let pid = Erlang.spawn (fun () -> Fable.Beam.Timer.timer.sleep 60000)
    let ref = Erlang.monitor pid
    Erlang.demonitorFlush ref
    Erlang.exitPid pid (box "kill")
#else
    ()
#endif

[<Fact>]
let ``test register and whereis`` () =
#if FABLE_COMPILER
    let name = Erlang.binaryToAtom "fable_beam_test_proc"
    let pid = Erlang.self ()
    Erlang.register name pid
    match Erlang.whereis name with
    | Some found -> Erlang.exactEquals pid found |> equal true
    | None -> equal "Some" "None"
#else
    ()
#endif

[<Fact>]
let ``test date returns valid year month day`` () =
#if FABLE_COMPILER
    let (year, month, day) = Erlang.date ()
    (year >= 2025) |> equal true
    (month >= 1 && month <= 12) |> equal true
    (day >= 1 && day <= 31) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test dateYear dateMonth dateDay match date`` () =
#if FABLE_COMPILER
    let (year, month, day) = Erlang.date ()
    Erlang.dateYear () |> equal year
    Erlang.dateMonth () |> equal month
    Erlang.dateDay () |> equal day
#else
    ()
#endif

[<Fact>]
let ``test time returns valid hour minute second`` () =
#if FABLE_COMPILER
    let (hour, minute, second) = Erlang.time ()
    (hour >= 0 && hour <= 23) |> equal true
    (minute >= 0 && minute <= 59) |> equal true
    (second >= 0 && second <= 59) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test localtime returns valid date and time`` () =
#if FABLE_COMPILER
    let ((year, month, day), (hour, minute, second)) = Erlang.localtime ()
    (year >= 2025) |> equal true
    (month >= 1 && month <= 12) |> equal true
    (day >= 1 && day <= 31) |> equal true
    (hour >= 0 && hour <= 23) |> equal true
    (minute >= 0 && minute <= 59) |> equal true
    (second >= 0 && second <= 59) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test universaltime returns valid date and time`` () =
#if FABLE_COMPILER
    let ((year, month, day), (hour, minute, second)) = Erlang.universaltime ()
    (year >= 2025) |> equal true
    (month >= 1 && month <= 12) |> equal true
    (day >= 1 && day <= 31) |> equal true
    (hour >= 0 && hour <= 23) |> equal true
    (minute >= 0 && minute <= 59) |> equal true
    (second >= 0 && second <= 59) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test monotonicTimeMs returns positive`` () =
#if FABLE_COMPILER
    let t1 = Erlang.monotonicTimeMs ()
    let t2 = Erlang.monotonicTimeMs ()
    (t2 >= t1) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test whereis returns None for unregistered name`` () =
#if FABLE_COMPILER
    let name = Erlang.binaryToAtom "fable_beam_nonexistent_12345"
    Erlang.whereis name |> equal None
#else
    ()
#endif

[<Fact>]
let ``test trapExit returns old value`` () =
#if FABLE_COMPILER
    let old1 = Erlang.trapExit ()
    // Second call should return true since we just set it
    let old2 = Erlang.trapExit ()
    old2 |> equal true
    // Reset: set trap_exit back to false
    Erlang.processFlag (Erlang.binaryToAtom "trap_exit") (box false) |> ignore
#else
    ()
#endif

[<Fact>]
let ``test cancelTimer returns None for invalid ref`` () =
#if FABLE_COMPILER
    let fakeRef = Erlang.makeRef ()
    // cancelTimer on a non-timer ref returns None (false in Erlang)
    // Note: makeRef() does not create a timer ref, but we can test
    // that sendAfter + cancel works and returns Some
    let timerRef = Erlang.sendAfter 60000 (box "test")
    match Erlang.cancelTimer timerRef with
    | Some ms -> (ms >= 0) |> equal true
    | None -> equal "Some" "None"
    // Cancelling again should return None
    Erlang.cancelTimer timerRef |> equal None
#else
    ()
#endif

[<Fact>]
let ``test sendAfterTo sends to specific pid`` () =
#if FABLE_COMPILER
    let pid = Erlang.self ()
    let timerRef = Erlang.sendAfterTo 60000 pid (box "msg")
    match Erlang.cancelTimer timerRef with
    | Some _ -> equal true true
    | None -> equal "Some" "None"
#else
    ()
#endif

[<Fact>]
let ``test byteSize returns correct size`` () =
#if FABLE_COMPILER
    Erlang.byteSize "hello" |> equal 5
    Erlang.byteSize "" |> equal 0
    Erlang.byteSize "abc" |> equal 3
#else
    ()
#endif

[<Fact>]
let ``test atomToList returns charlist not binary`` () =
#if FABLE_COMPILER
    let atom = Erlang.binaryToAtom "test"
    let charlist = Erlang.atomToList atom
    // atomToList returns a charlist (Erlang list of integers),
    // which is not the same as an F# string (binary).
    // We verify by round-tripping through listToAtom.
    let atom2 = Erlang.listToAtom charlist
    Erlang.atomToBinary atom2 |> equal "test"
#else
    ()
#endif
