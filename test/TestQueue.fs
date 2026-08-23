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
        [ test ("new creates empty queue", fun _ ->
                  let q = queue.``new`` ()
                  assertThat (queue.is_empty q) (isTrue))

          test ("is_queue returns true for queue", fun _ ->
                  let q = queue.``new`` ()
                  assertThat (queue.is_queue q) (isTrue))

          test ("is_queue returns false for non-queue", fun _ ->
                  assertThat (queue.is_queue (box 42)) (isFalse))

          test ("len returns zero for empty queue", fun _ ->
                  let q = queue.``new`` ()
                  assertThat (queue.len q) (isEqualTo 0))

          test ("in adds element at rear", fun _ ->
                  let q = queue.``new`` ()
                  let q1 = queue.``in`` (1, q)
                  let q2 = queue.``in`` (2, q1)
                  assertThat (queue.len q2) (isEqualTo 2))

          test ("in_r adds element at front", fun _ ->
                  let q = queue.``new`` ()
                  let q1 = queue.``in`` (1, q)
                  let q2 = queue.in_r (99, q1)
                  assertThat (queue.head q2) (isEqualTo 99))

          test ("head returns front element", fun _ ->
                  let q = queue.``new`` ()
                  let q1 = queue.``in`` (10, q)
                  let q2 = queue.``in`` (20, q1)
                  assertThat (queue.head q2) (isEqualTo 10))

          test ("last returns rear element", fun _ ->
                  let q = queue.``new`` ()
                  let q1 = queue.``in`` (10, q)
                  let q2 = queue.``in`` (20, q1)
                  assertThat (queue.last q2) (isEqualTo 20))

          test ("tail removes front element", fun _ ->
                  let q = queue.``new`` ()
                  let q1 = queue.``in`` (1, q)
                  let q2 = queue.``in`` (2, q1)
                  let q3 = queue.tail q2
                  assertThat (queue.len q3) (isEqualTo 1)
                  assertThat (queue.head q3) (isEqualTo 2))

          test ("init removes rear element", fun _ ->
                  let q = queue.``new`` ()
                  let q1 = queue.``in`` (1, q)
                  let q2 = queue.``in`` (2, q1)
                  let q3 = queue.init q2
                  assertThat (queue.len q3) (isEqualTo 1)
                  assertThat (queue.last q3) (isEqualTo 1))

          test ("to_list returns elements front first", fun _ ->
                  let q = queue.``new`` ()
                  let q1 = queue.``in`` (1, q)
                  let q2 = queue.``in`` (2, q1)
                  let q3 = queue.``in`` (3, q2)
                  assertThat (queue.to_list q3) (isEqualTo [ 1; 2; 3 ]))

          test ("from_list builds queue from list", fun _ ->
                  let q = queue.from_list [ 1; 2; 3 ]
                  assertThat (queue.len q) (isEqualTo 3)
                  assertThat (queue.head q) (isEqualTo 1)
                  assertThat (queue.last q) (isEqualTo 3))

          test ("member returns true when element present", fun _ ->
                  let q = queue.from_list [ 1; 2; 3 ]
                  assertThat (queue.``member`` (2, q)) (isTrue))

          test ("member returns false when element absent", fun _ ->
                  let q = queue.from_list [ 1; 2; 3 ]
                  assertThat (queue.``member`` (99, q)) (isFalse))

          test ("reverse reverses order", fun _ ->
                  let q = queue.from_list [ 1; 2; 3 ]
                  let r = queue.reverse q
                  assertThat (queue.to_list r) (isEqualTo [ 3; 2; 1 ]))

          test ("join appends two queues", fun _ ->
                  let q1 = queue.from_list [ 1; 2 ]
                  let q2 = queue.from_list [ 3; 4 ]
                  let q3 = queue.join (q1, q2)
                  assertThat (queue.to_list q3) (isEqualTo [ 1; 2; 3; 4 ]))

          test ("filter keeps matching elements", fun _ ->
                  let q = queue.from_list [ 1; 2; 3; 4; 5 ]
                  let evens = queue.filter ((fun x -> x % 2 = 0), q)
                  assertThat (queue.to_list evens) (isEqualTo [ 2; 4 ]))

          test ("out removes front element", fun _ ->
                  let q = queue.from_list [ 10; 20; 30 ]
                  let (item, q2) = out q
                  assertThat item (isEqualTo (Some 10))
                  assertThat (queue.len q2) (isEqualTo 2))

          test ("out returns None for empty queue", fun _ ->
                  let q = queue.``new`` ()
                  let (item, _) = out q
                  assertThat item (isEqualTo None))

          test ("outRear removes rear element", fun _ ->
                  let q = queue.from_list [ 10; 20; 30 ]
                  let (item, q2) = outRear q
                  assertThat item (isEqualTo (Some 30))
                  assertThat (queue.len q2) (isEqualTo 2))

          test ("outRear returns None for empty queue", fun _ ->
                  let q = queue.``new`` ()
                  let (item, _) = outRear q
                  assertThat item (isEqualTo None))

          test ("peek returns front element without removing", fun _ ->
                  let q = queue.from_list [ 10; 20 ]
                  assertThat (peek q) (isEqualTo (Some 10))
                  assertThat (queue.len q) (isEqualTo 2))

          test ("peek returns None for empty queue", fun _ ->
                  let q = queue.``new`` ()
                  assertThat (peek q) (isEqualTo None))

          test ("peekRear returns rear element without removing", fun _ ->
                  let q = queue.from_list [ 10; 20; 30 ]
                  assertThat (peekRear q) (isEqualTo (Some 30))
                  assertThat (queue.len q) (isEqualTo 3))

          test ("split divides queue at position", fun _ ->
                  let q = queue.from_list [ 1; 2; 3; 4; 5 ]
                  let (q1, q2) = split 3 q
                  assertThat (queue.to_list q1) (isEqualTo [ 1; 2; 3 ])
                  assertThat (queue.to_list q2) (isEqualTo [ 4; 5 ]))

          test ("split at zero yields empty front", fun _ ->
                  let q = queue.from_list [ 1; 2; 3 ]
                  let (q1, q2) = split 0 q
                  assertThat (queue.is_empty q1) (isTrue)
                  assertThat (queue.to_list q2) (isEqualTo [ 1; 2; 3 ]))

          test ("fifo ordering is preserved", fun _ ->
                  // Enqueue 1, 2, 3 — dequeue should yield 1, 2, 3
                  let q0 = queue.``new`` ()
                  let q1 = queue.``in`` (1, q0)
                  let q2 = queue.``in`` (2, q1)
                  let q3 = queue.``in`` (3, q2)
                  let (a, q4) = out q3
                  let (b, q5) = out q4
                  let (c, _) = out q5
                  assertThat a (isEqualTo (Some 1))
                  assertThat b (isEqualTo (Some 2))
                  assertThat c (isEqualTo (Some 3))) ]
    )
