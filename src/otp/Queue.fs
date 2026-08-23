/// Type bindings for Erlang queue module
/// See https://www.erlang.org/doc/apps/stdlib/queue
module Fable.Beam.Queue

open Fable.Core

// fsharplint:disable MemberNames

/// Opaque Erlang functional queue.
[<Erase>]
type Queue<'a> = Queue of obj

/// Creates a new empty queue.
[<Emit("queue:new()")>]
let empty<'a> () : Queue<'a> = nativeOnly

/// Tests if Q is an empty queue.
[<Emit("queue:is_empty($0)")>]
let isEmpty (queue: Queue<'a>) : bool = nativeOnly

/// Tests if Term is a queue.
[<Emit("queue:is_queue($0)")>]
let isQueue (term: obj) : bool = nativeOnly

/// Returns the number of elements in Q. O(1) time.
[<Emit("queue:len($0)")>]
let length (queue: Queue<'a>) : int = nativeOnly

/// Inserts an item at the rear. O(1) time.
[<Emit("queue:in($0, $1)")>]
let enqueue (item: 'a) (queue: Queue<'a>) : Queue<'a> = nativeOnly

/// Inserts an item at the front. O(1) time.
[<Emit("queue:in_r($0, $1)")>]
let enqueueFront (item: 'a) (queue: Queue<'a>) : Queue<'a> = nativeOnly

/// Returns the front element. Raises an error on an empty queue — use peek for a safe alternative.
[<Emit("queue:head($0)")>]
let head (queue: Queue<'a>) : 'a = nativeOnly

/// Returns the rear element. Raises an error on an empty queue — use peekRear for a safe alternative.
[<Emit("queue:last($0)")>]
let last (queue: Queue<'a>) : 'a = nativeOnly

/// Returns Q with the front element removed. Raises an error on an empty queue.
[<Emit("queue:tail($0)")>]
let tail (queue: Queue<'a>) : Queue<'a> = nativeOnly

/// Returns Q with the rear element removed. Raises an error on an empty queue.
[<Emit("queue:init($0)")>]
let init (queue: Queue<'a>) : Queue<'a> = nativeOnly

/// Converts the queue to a list, front first.
[<Emit("queue:to_list($0)")>]
let toList (queue: Queue<'a>) : 'a list = nativeOnly

/// Builds a queue from a list. The head of the list becomes the front element.
[<Emit("queue:from_list($0)")>]
let ofList (items: 'a list) : Queue<'a> = nativeOnly

/// Returns true if Item is a member of Q, otherwise false. O(n) time.
[<Emit("queue:member($0, $1)")>]
let contains (item: 'a) (queue: Queue<'a>) : bool = nativeOnly

/// Returns a queue with the reverse ordering.
[<Emit("queue:reverse($0)")>]
let reverse (queue: Queue<'a>) : Queue<'a> = nativeOnly

/// Joins Q1 and Q2. The rear of Q1 becomes the front of Q2. O(1) time.
[<Emit("queue:join($0, $1)")>]
let join (first: Queue<'a>) (second: Queue<'a>) : Queue<'a> = nativeOnly

/// Returns a queue of all elements for which the predicate returns true.
[<Emit("queue:filter($0, $1)")>]
let filter (predicate: 'a -> bool) (queue: Queue<'a>) : Queue<'a> = nativeOnly

// ============================================================================
// Typed API — functions with non-trivial Erlang return values
// ============================================================================
// NOTE: the (fun() -> ... end)() wrappers on the case Emits below are no longer
// required — Fable (>= 5.0.0) auto-wraps case-containing Emits for variable scoping.
// Kept for explicitness; safe to remove.

/// Removes the front element of queue Q.
/// Returns (Some element, newQueue) if Q is non-empty, or (None, Q) if empty. O(1) amortized.
[<Emit("(fun() -> case queue:out($0) of {empty, QueueOutQ__} -> {undefined, QueueOutQ__}; {{value, QueueOutVal__}, QueueOutQ__} -> {QueueOutVal__, QueueOutQ__} end end)()")>]
let out (q: Queue<'a>) : 'a option * Queue<'a> = nativeOnly

/// Removes the rear element of queue Q.
/// Returns (Some element, newQueue) if Q is non-empty, or (None, Q) if empty. O(1) amortized.
[<Emit("(fun() -> case queue:out_r($0) of {empty, QueueOutRQ__} -> {undefined, QueueOutRQ__}; {{value, QueueOutRVal__}, QueueOutRQ__} -> {QueueOutRVal__, QueueOutRQ__} end end)()")>]
let outRear (q: Queue<'a>) : 'a option * Queue<'a> = nativeOnly

/// Returns the front element of queue Q without removing it. O(1) time.
/// Returns Some element, or None if Q is empty.
[<Emit("(fun() -> case queue:peek($0) of empty -> undefined; {value, QueuePeekVal__} -> QueuePeekVal__ end end)()")>]
let peek (q: Queue<'a>) : 'a option = nativeOnly

/// Returns the rear element of queue Q without removing it. O(1) time.
/// Returns Some element, or None if Q is empty.
[<Emit("(fun() -> case queue:peek_r($0) of empty -> undefined; {value, QueuePeekRVal__} -> QueuePeekRVal__ end end)()")>]
let peekRear (q: Queue<'a>) : 'a option = nativeOnly

/// Splits Q into (Q1, Q2) where Q1 has the front N elements and Q2 has the rest.
/// Raises a runtime error if N > len(Q).
[<Emit("queue:split($0, $1)")>]
let split (n: int) (q: Queue<'a>) : Queue<'a> * Queue<'a> = nativeOnly
