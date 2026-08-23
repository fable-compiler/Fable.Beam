module Fable.Beam.Tests.Queue

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
module BQueue = Fable.Beam.Queue

let tests =
    testList (
        "Queue",
        [ test (
              "new creates empty queue",
              fun _ ->
                  let q = BQueue.empty ()
                  assertThat (BQueue.isEmpty q) (isTrue)
          )

          test (
              "is_queue returns true for queue",
              fun _ ->
                  let q = BQueue.empty ()
                  assertThat (BQueue.isQueue q) (isTrue)
          )

          test ("is_queue returns false for non-queue", fun _ -> assertThat (BQueue.isQueue (box 42)) (isFalse))

          test (
              "len returns zero for empty queue",
              fun _ ->
                  let q = BQueue.empty ()
                  assertThat (BQueue.length q) (isEqualTo 0)
          )

          test (
              "in adds element at rear",
              fun _ ->
                  let q = BQueue.empty ()
                  let q1 = BQueue.enqueue 1 q
                  let q2 = BQueue.enqueue 2 q1
                  assertThat (BQueue.length q2) (isEqualTo 2)
          )

          test (
              "in_r adds element at front",
              fun _ ->
                  let q = BQueue.empty ()
                  let q1 = BQueue.enqueue 1 q
                  let q2 = BQueue.enqueueFront 99 q1
                  assertThat (BQueue.head q2) (isEqualTo 99)
          )

          test (
              "head returns front element",
              fun _ ->
                  let q = BQueue.empty ()
                  let q1 = BQueue.enqueue 10 q
                  let q2 = BQueue.enqueue 20 q1
                  assertThat (BQueue.head q2) (isEqualTo 10)
          )

          test (
              "last returns rear element",
              fun _ ->
                  let q = BQueue.empty ()
                  let q1 = BQueue.enqueue 10 q
                  let q2 = BQueue.enqueue 20 q1
                  assertThat (BQueue.last q2) (isEqualTo 20)
          )

          test (
              "tail removes front element",
              fun _ ->
                  let q = BQueue.empty ()
                  let q1 = BQueue.enqueue 1 q
                  let q2 = BQueue.enqueue 2 q1
                  let q3 = BQueue.tail q2
                  assertThat (BQueue.length q3) (isEqualTo 1)
                  assertThat (BQueue.head q3) (isEqualTo 2)
          )

          test (
              "init removes rear element",
              fun _ ->
                  let q = BQueue.empty ()
                  let q1 = BQueue.enqueue 1 q
                  let q2 = BQueue.enqueue 2 q1
                  let q3 = BQueue.init q2
                  assertThat (BQueue.length q3) (isEqualTo 1)
                  assertThat (BQueue.last q3) (isEqualTo 1)
          )

          test (
              "to_list returns elements front first",
              fun _ ->
                  let q = BQueue.empty ()
                  let q1 = BQueue.enqueue 1 q
                  let q2 = BQueue.enqueue 2 q1
                  let q3 = BQueue.enqueue 3 q2
                  assertThat (BQueue.toList q3) (isEqualTo [ 1; 2; 3 ])
          )

          test (
              "from_list builds queue from list",
              fun _ ->
                  let q = BQueue.ofList [ 1; 2; 3 ]
                  assertThat (BQueue.length q) (isEqualTo 3)
                  assertThat (BQueue.head q) (isEqualTo 1)
                  assertThat (BQueue.last q) (isEqualTo 3)
          )

          test (
              "member returns true when element present",
              fun _ ->
                  let q = BQueue.ofList [ 1; 2; 3 ]
                  assertThat (BQueue.contains 2 q) (isTrue)
          )

          test (
              "member returns false when element absent",
              fun _ ->
                  let q = BQueue.ofList [ 1; 2; 3 ]
                  assertThat (BQueue.contains 99 q) (isFalse)
          )

          test (
              "reverse reverses order",
              fun _ ->
                  let q = BQueue.ofList [ 1; 2; 3 ]
                  let r = BQueue.reverse q
                  assertThat (BQueue.toList r) (isEqualTo [ 3; 2; 1 ])
          )

          test (
              "join appends two queues",
              fun _ ->
                  let q1 = BQueue.ofList [ 1; 2 ]
                  let q2 = BQueue.ofList [ 3; 4 ]
                  let q3 = BQueue.join q1 q2
                  assertThat (BQueue.toList q3) (isEqualTo [ 1; 2; 3; 4 ])
          )

          test (
              "filter keeps matching elements",
              fun _ ->
                  let q = BQueue.ofList [ 1; 2; 3; 4; 5 ]
                  let evens = BQueue.filter (fun x -> x % 2 = 0) q
                  assertThat (BQueue.toList evens) (isEqualTo [ 2; 4 ])
          )

          test (
              "out removes front element",
              fun _ ->
                  let q = BQueue.ofList [ 10; 20; 30 ]
                  let (item, q2) = BQueue.out q
                  assertThat item (isEqualTo (Some 10))
                  assertThat (BQueue.length q2) (isEqualTo 2)
          )

          test (
              "out returns None for empty queue",
              fun _ ->
                  let q = BQueue.empty ()
                  let (item, _) = BQueue.out q
                  assertThat item (isEqualTo None)
          )

          test (
              "outRear removes rear element",
              fun _ ->
                  let q = BQueue.ofList [ 10; 20; 30 ]
                  let (item, q2) = BQueue.outRear q
                  assertThat item (isEqualTo (Some 30))
                  assertThat (BQueue.length q2) (isEqualTo 2)
          )

          test (
              "outRear returns None for empty queue",
              fun _ ->
                  let q = BQueue.empty ()
                  let (item, _) = BQueue.outRear q
                  assertThat item (isEqualTo None)
          )

          test (
              "peek returns front element without removing",
              fun _ ->
                  let q = BQueue.ofList [ 10; 20 ]
                  assertThat (BQueue.peek q) (isEqualTo (Some 10))
                  assertThat (BQueue.length q) (isEqualTo 2)
          )

          test (
              "peek returns None for empty queue",
              fun _ ->
                  let q = BQueue.empty ()
                  assertThat (BQueue.peek q) (isEqualTo None)
          )

          test (
              "peekRear returns rear element without removing",
              fun _ ->
                  let q = BQueue.ofList [ 10; 20; 30 ]
                  assertThat (BQueue.peekRear q) (isEqualTo (Some 30))
                  assertThat (BQueue.length q) (isEqualTo 3)
          )

          test (
              "split divides queue at position",
              fun _ ->
                  let q = BQueue.ofList [ 1; 2; 3; 4; 5 ]
                  let (q1, q2) = BQueue.split 3 q
                  assertThat (BQueue.toList q1) (isEqualTo [ 1; 2; 3 ])
                  assertThat (BQueue.toList q2) (isEqualTo [ 4; 5 ])
          )

          test (
              "split at zero yields empty front",
              fun _ ->
                  let q = BQueue.ofList [ 1; 2; 3 ]
                  let (q1, q2) = BQueue.split 0 q
                  assertThat (BQueue.isEmpty q1) (isTrue)
                  assertThat (BQueue.toList q2) (isEqualTo [ 1; 2; 3 ])
          )

          test (
              "fifo ordering is preserved",
              fun _ ->
                  // Enqueue 1, 2, 3 — dequeue should yield 1, 2, 3
                  let q0 = BQueue.empty ()
                  let q1 = BQueue.enqueue 1 q0
                  let q2 = BQueue.enqueue 2 q1
                  let q3 = BQueue.enqueue 3 q2
                  let (a, q4) = BQueue.out q3
                  let (b, q5) = BQueue.out q4
                  let (c, _) = BQueue.out q5
                  assertThat a (isEqualTo (Some 1))
                  assertThat b (isEqualTo (Some 2))
                  assertThat c (isEqualTo (Some 3))
          ) ]
    )
