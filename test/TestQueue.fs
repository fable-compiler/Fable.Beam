module Fable.Beam.Tests.Queue

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.Queue

let tests =
    testList (
        "Queue",
        [ test (
              "new creates empty queue",
              fun _ ->
                  let q = empty ()
                  assertThat (isEmpty q) (isTrue)
          )

          test (
              "is_queue returns true for queue",
              fun _ ->
                  let q = empty ()
                  assertThat (isQueue q) (isTrue)
          )

          test ("is_queue returns false for non-queue", fun _ -> assertThat (isQueue (box 42)) (isFalse))

          test (
              "len returns zero for empty queue",
              fun _ ->
                  let q = empty ()
                  assertThat (length q) (isEqualTo 0)
          )

          test (
              "in adds element at rear",
              fun _ ->
                  let q = empty ()
                  let q1 = enqueue 1 q
                  let q2 = enqueue 2 q1
                  assertThat (length q2) (isEqualTo 2)
          )

          test (
              "in_r adds element at front",
              fun _ ->
                  let q = empty ()
                  let q1 = enqueue 1 q
                  let q2 = enqueueFront 99 q1
                  assertThat (head q2) (isEqualTo 99)
          )

          test (
              "head returns front element",
              fun _ ->
                  let q = empty ()
                  let q1 = enqueue 10 q
                  let q2 = enqueue 20 q1
                  assertThat (head q2) (isEqualTo 10)
          )

          test (
              "last returns rear element",
              fun _ ->
                  let q = empty ()
                  let q1 = enqueue 10 q
                  let q2 = enqueue 20 q1
                  assertThat (last q2) (isEqualTo 20)
          )

          test (
              "tail removes front element",
              fun _ ->
                  let q = empty ()
                  let q1 = enqueue 1 q
                  let q2 = enqueue 2 q1
                  let q3 = tail q2
                  assertThat (length q3) (isEqualTo 1)
                  assertThat (head q3) (isEqualTo 2)
          )

          test (
              "init removes rear element",
              fun _ ->
                  let q = empty ()
                  let q1 = enqueue 1 q
                  let q2 = enqueue 2 q1
                  let q3 = init q2
                  assertThat (length q3) (isEqualTo 1)
                  assertThat (last q3) (isEqualTo 1)
          )

          test (
              "to_list returns elements front first",
              fun _ ->
                  let q = empty ()
                  let q1 = enqueue 1 q
                  let q2 = enqueue 2 q1
                  let q3 = enqueue 3 q2
                  assertThat (toList q3) (isEqualTo [ 1; 2; 3 ])
          )

          test (
              "from_list builds queue from list",
              fun _ ->
                  let q = ofList [ 1; 2; 3 ]
                  assertThat (length q) (isEqualTo 3)
                  assertThat (head q) (isEqualTo 1)
                  assertThat (last q) (isEqualTo 3)
          )

          test (
              "member returns true when element present",
              fun _ ->
                  let q = ofList [ 1; 2; 3 ]
                  assertThat (contains 2 q) (isTrue)
          )

          test (
              "member returns false when element absent",
              fun _ ->
                  let q = ofList [ 1; 2; 3 ]
                  assertThat (contains 99 q) (isFalse)
          )

          test (
              "reverse reverses order",
              fun _ ->
                  let q = ofList [ 1; 2; 3 ]
                  let r = reverse q
                  assertThat (toList r) (isEqualTo [ 3; 2; 1 ])
          )

          test (
              "join appends two queues",
              fun _ ->
                  let q1 = ofList [ 1; 2 ]
                  let q2 = ofList [ 3; 4 ]
                  let q3 = join q1 q2
                  assertThat (toList q3) (isEqualTo [ 1; 2; 3; 4 ])
          )

          test (
              "filter keeps matching elements",
              fun _ ->
                  let q = ofList [ 1; 2; 3; 4; 5 ]
                  let evens = filter (fun x -> x % 2 = 0) q
                  assertThat (toList evens) (isEqualTo [ 2; 4 ])
          )

          test (
              "out removes front element",
              fun _ ->
                  let q = ofList [ 10; 20; 30 ]
                  let (item, q2) = out q
                  assertThat item (isEqualTo (Some 10))
                  assertThat (length q2) (isEqualTo 2)
          )

          test (
              "out returns None for empty queue",
              fun _ ->
                  let q = empty ()
                  let (item, _) = out q
                  assertThat item (isEqualTo None)
          )

          test (
              "outRear removes rear element",
              fun _ ->
                  let q = ofList [ 10; 20; 30 ]
                  let (item, q2) = outRear q
                  assertThat item (isEqualTo (Some 30))
                  assertThat (length q2) (isEqualTo 2)
          )

          test (
              "outRear returns None for empty queue",
              fun _ ->
                  let q = empty ()
                  let (item, _) = outRear q
                  assertThat item (isEqualTo None)
          )

          test (
              "peek returns front element without removing",
              fun _ ->
                  let q = ofList [ 10; 20 ]
                  assertThat (peek q) (isEqualTo (Some 10))
                  assertThat (length q) (isEqualTo 2)
          )

          test (
              "peek returns None for empty queue",
              fun _ ->
                  let q = empty ()
                  assertThat (peek q) (isEqualTo None)
          )

          test (
              "peekRear returns rear element without removing",
              fun _ ->
                  let q = ofList [ 10; 20; 30 ]
                  assertThat (peekRear q) (isEqualTo (Some 30))
                  assertThat (length q) (isEqualTo 3)
          )

          test (
              "split divides queue at position",
              fun _ ->
                  let q = ofList [ 1; 2; 3; 4; 5 ]
                  let (q1, q2) = split 3 q
                  assertThat (toList q1) (isEqualTo [ 1; 2; 3 ])
                  assertThat (toList q2) (isEqualTo [ 4; 5 ])
          )

          test (
              "split at zero yields empty front",
              fun _ ->
                  let q = ofList [ 1; 2; 3 ]
                  let (q1, q2) = split 0 q
                  assertThat (isEmpty q1) (isTrue)
                  assertThat (toList q2) (isEqualTo [ 1; 2; 3 ])
          )

          test (
              "fifo ordering is preserved",
              fun _ ->
                  // Enqueue 1, 2, 3 — dequeue should yield 1, 2, 3
                  let q0 = empty ()
                  let q1 = enqueue 1 q0
                  let q2 = enqueue 2 q1
                  let q3 = enqueue 3 q2
                  let (a, q4) = out q3
                  let (b, q5) = out q4
                  let (c, _) = out q5
                  assertThat a (isEqualTo (Some 1))
                  assertThat b (isEqualTo (Some 2))
                  assertThat c (isEqualTo (Some 3))
          ) ]
    )
