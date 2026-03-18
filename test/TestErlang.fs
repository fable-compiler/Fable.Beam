module Fable.Beam.Tests.Erlang

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Erlang

type RecvMsg =
    | [<CompiledName("ping")>] Ping
    | [<CompiledName("data")>] Data of value: int
#endif

[<Fact>]
let ``test self returns a pid`` () =
#if FABLE_COMPILER
    let pid = self ()
    let isAlive = isProcessAlive pid
    isAlive |> equal true
#else
    ()
#endif

[<Fact>]
let ``test makeRef returns unique references`` () =
#if FABLE_COMPILER
    let ref1 = makeRef ()
    let ref2 = makeRef ()
    exactEquals ref1 ref2 |> equal false
#else
    ()
#endif

[<Fact>]
let ``test exactEquals on same ref`` () =
#if FABLE_COMPILER
    let ref1 = makeRef ()
    exactEquals ref1 ref1 |> equal true
#else
    ()
#endif

[<Fact>]
let ``test spawn creates a process`` () =
#if FABLE_COMPILER
    let pid = spawn (fun () -> ())
    isProcessAlive pid |> equal true
#else
    ()
#endif

[<Fact>]
let ``test spawnLink creates a linked process`` () =
#if FABLE_COMPILER
    let pid = spawnLink (fun () -> ())
    isProcessAlive pid |> equal true
#else
    ()
#endif

[<Fact>]
let ``test isProcessAlive on self`` () =
#if FABLE_COMPILER
    let pid = self ()
    isProcessAlive pid |> equal true
#else
    ()
#endif

[<Fact>]
let ``test process dictionary get/put/erase`` () =
#if FABLE_COMPILER
    let key = makeRef ()
    put key (box 42) |> ignore
    let value = get key
    value |> equal (box 42)
    erase key |> ignore
#else
    ()
#endif

[<Fact>]
let ``test send and receive`` () =
#if FABLE_COMPILER
    let pid = self ()
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
    let timerRef = sendAfter 60000 (box "should_not_arrive")
    cancelTimer timerRef
#else
    ()
#endif

[<Fact>]
let ``test atomToBinary and binaryToAtom roundtrip`` () =
#if FABLE_COMPILER
    let atom = binaryToAtom "test_atom"
    let str = atomToBinary atom
    str |> equal "test_atom"
#else
    ()
#endif

[<Fact>]
let ``test monitor and demonitor`` () =
#if FABLE_COMPILER
    let pid = spawn (fun () -> Fable.Beam.Timer.timer.sleep 60000)
    let ref = monitor pid
    demonitorFlush ref
    exitPid pid (box "kill")
#else
    ()
#endif

[<Fact>]
let ``test register and whereis`` () =
#if FABLE_COMPILER
    let name = binaryToAtom "fable_beam_test_proc"
    let pid = self ()
    register name pid
    let found = whereis name
    exactEquals pid found |> equal true
#else
    ()
#endif

[<Fact>]
let ``test date returns valid year month day`` () =
#if FABLE_COMPILER
    let (year, month, day) = date ()
    (year >= 2025) |> equal true
    (month >= 1 && month <= 12) |> equal true
    (day >= 1 && day <= 31) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test dateYear dateMonth dateDay match date`` () =
#if FABLE_COMPILER
    let (year, month, day) = date ()
    dateYear () |> equal year
    dateMonth () |> equal month
    dateDay () |> equal day
#else
    ()
#endif

[<Fact>]
let ``test time returns valid hour minute second`` () =
#if FABLE_COMPILER
    let (hour, minute, second) = time ()
    (hour >= 0 && hour <= 23) |> equal true
    (minute >= 0 && minute <= 59) |> equal true
    (second >= 0 && second <= 59) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test localtime returns valid date and time`` () =
#if FABLE_COMPILER
    let ((year, month, day), (hour, minute, second)) = localtime ()
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
    let ((year, month, day), (hour, minute, second)) = universaltime ()
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
    let t1 = monotonicTimeMs ()
    let t2 = monotonicTimeMs ()
    (t2 >= t1) |> equal true
#else
    ()
#endif
