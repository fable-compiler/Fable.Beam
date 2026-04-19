module Fable.Beam.Tests.Queue

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.Queue
#endif

[<Fact>]
let ``test new creates empty queue`` () =
#if FABLE_COMPILER
    let q = queue.``new`` ()
    queue.is_empty q |> equal true
#else
    ()
#endif

[<Fact>]
let ``test is_queue returns true for queue`` () =
#if FABLE_COMPILER
    let q = queue.``new`` ()
    queue.is_queue q |> equal true
#else
    ()
#endif

[<Fact>]
let ``test is_queue returns false for non-queue`` () =
#if FABLE_COMPILER
    queue.is_queue (box 42) |> equal false
#else
    ()
#endif

[<Fact>]
let ``test len returns zero for empty queue`` () =
#if FABLE_COMPILER
    let q = queue.``new`` ()
    queue.len q |> equal 0
#else
    ()
#endif

[<Fact>]
let ``test in adds element at rear`` () =
#if FABLE_COMPILER
    let q = queue.``new`` ()
    let q1 = queue.``in`` (1, q)
    let q2 = queue.``in`` (2, q1)
    queue.len q2 |> equal 2
#else
    ()
#endif

[<Fact>]
let ``test in_r adds element at front`` () =
#if FABLE_COMPILER
    let q = queue.``new`` ()
    let q1 = queue.``in`` (1, q)
    let q2 = queue.in_r (99, q1)
    queue.head q2 |> equal 99
#else
    ()
#endif

[<Fact>]
let ``test head returns front element`` () =
#if FABLE_COMPILER
    let q = queue.``new`` ()
    let q1 = queue.``in`` (10, q)
    let q2 = queue.``in`` (20, q1)
    queue.head q2 |> equal 10
#else
    ()
#endif

[<Fact>]
let ``test last returns rear element`` () =
#if FABLE_COMPILER
    let q = queue.``new`` ()
    let q1 = queue.``in`` (10, q)
    let q2 = queue.``in`` (20, q1)
    queue.last q2 |> equal 20
#else
    ()
#endif

[<Fact>]
let ``test tail removes front element`` () =
#if FABLE_COMPILER
    let q = queue.``new`` ()
    let q1 = queue.``in`` (1, q)
    let q2 = queue.``in`` (2, q1)
    let q3 = queue.tail q2
    queue.len q3 |> equal 1
    queue.head q3 |> equal 2
#else
    ()
#endif

[<Fact>]
let ``test init removes rear element`` () =
#if FABLE_COMPILER
    let q = queue.``new`` ()
    let q1 = queue.``in`` (1, q)
    let q2 = queue.``in`` (2, q1)
    let q3 = queue.init q2
    queue.len q3 |> equal 1
    queue.last q3 |> equal 1
#else
    ()
#endif

[<Fact>]
let ``test to_list returns elements front first`` () =
#if FABLE_COMPILER
    let q = queue.``new`` ()
    let q1 = queue.``in`` (1, q)
    let q2 = queue.``in`` (2, q1)
    let q3 = queue.``in`` (3, q2)
    queue.to_list q3 |> equal [ 1; 2; 3 ]
#else
    ()
#endif

[<Fact>]
let ``test from_list builds queue from list`` () =
#if FABLE_COMPILER
    let q = queue.from_list [ 1; 2; 3 ]
    queue.len q |> equal 3
    queue.head q |> equal 1
    queue.last q |> equal 3
#else
    ()
#endif

[<Fact>]
let ``test member returns true when element present`` () =
#if FABLE_COMPILER
    let q = queue.from_list [ 1; 2; 3 ]
    queue.``member`` (2, q) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test member returns false when element absent`` () =
#if FABLE_COMPILER
    let q = queue.from_list [ 1; 2; 3 ]
    queue.``member`` (99, q) |> equal false
#else
    ()
#endif

[<Fact>]
let ``test reverse reverses order`` () =
#if FABLE_COMPILER
    let q = queue.from_list [ 1; 2; 3 ]
    let r = queue.reverse q
    queue.to_list r |> equal [ 3; 2; 1 ]
#else
    ()
#endif

[<Fact>]
let ``test join appends two queues`` () =
#if FABLE_COMPILER
    let q1 = queue.from_list [ 1; 2 ]
    let q2 = queue.from_list [ 3; 4 ]
    let q3 = queue.join (q1, q2)
    queue.to_list q3 |> equal [ 1; 2; 3; 4 ]
#else
    ()
#endif

[<Fact>]
let ``test filter keeps matching elements`` () =
#if FABLE_COMPILER
    let q = queue.from_list [ 1; 2; 3; 4; 5 ]
    let evens = queue.filter (System.Func<_, _>(fun x -> x % 2 = 0), q)
    queue.to_list evens |> equal [ 2; 4 ]
#else
    ()
#endif

[<Fact>]
let ``test out removes front element`` () =
#if FABLE_COMPILER
    let q = queue.from_list [ 10; 20; 30 ]
    let (item, q2) = out q
    item |> equal (Some 10)
    queue.len q2 |> equal 2
#else
    ()
#endif

[<Fact>]
let ``test out returns None for empty queue`` () =
#if FABLE_COMPILER
    let q = queue.``new`` ()
    let (item, _) = out q
    item |> equal None
#else
    ()
#endif

[<Fact>]
let ``test outRear removes rear element`` () =
#if FABLE_COMPILER
    let q = queue.from_list [ 10; 20; 30 ]
    let (item, q2) = outRear q
    item |> equal (Some 30)
    queue.len q2 |> equal 2
#else
    ()
#endif

[<Fact>]
let ``test outRear returns None for empty queue`` () =
#if FABLE_COMPILER
    let q = queue.``new`` ()
    let (item, _) = outRear q
    item |> equal None
#else
    ()
#endif

[<Fact>]
let ``test peek returns front element without removing`` () =
#if FABLE_COMPILER
    let q = queue.from_list [ 10; 20 ]
    peek q |> equal (Some 10)
    queue.len q |> equal 2
#else
    ()
#endif

[<Fact>]
let ``test peek returns None for empty queue`` () =
#if FABLE_COMPILER
    let q = queue.``new`` ()
    peek q |> equal None
#else
    ()
#endif

[<Fact>]
let ``test peekRear returns rear element without removing`` () =
#if FABLE_COMPILER
    let q = queue.from_list [ 10; 20; 30 ]
    peekRear q |> equal (Some 30)
    queue.len q |> equal 3
#else
    ()
#endif

[<Fact>]
let ``test split divides queue at position`` () =
#if FABLE_COMPILER
    let q = queue.from_list [ 1; 2; 3; 4; 5 ]
    let (q1, q2) = split 3 q
    queue.to_list q1 |> equal [ 1; 2; 3 ]
    queue.to_list q2 |> equal [ 4; 5 ]
#else
    ()
#endif

[<Fact>]
let ``test split at zero yields empty front`` () =
#if FABLE_COMPILER
    let q = queue.from_list [ 1; 2; 3 ]
    let (q1, q2) = split 0 q
    queue.is_empty q1 |> equal true
    queue.to_list q2 |> equal [ 1; 2; 3 ]
#else
    ()
#endif

[<Fact>]
let ``test fifo ordering is preserved`` () =
#if FABLE_COMPILER
    // Enqueue 1, 2, 3 — dequeue should yield 1, 2, 3
    let q0 = queue.``new`` ()
    let q1 = queue.``in`` (1, q0)
    let q2 = queue.``in`` (2, q1)
    let q3 = queue.``in`` (3, q2)
    let (a, q4) = out q3
    let (b, q5) = out q4
    let (c, _) = out q5
    a |> equal (Some 1)
    b |> equal (Some 2)
    c |> equal (Some 3)
#else
    ()
#endif
