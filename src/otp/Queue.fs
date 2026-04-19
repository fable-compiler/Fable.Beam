/// Type bindings for Erlang queue module
/// See https://www.erlang.org/doc/apps/stdlib/queue
module Fable.Beam.Queue

open Fable.Core

// fsharplint:disable MemberNames

/// Opaque Erlang functional queue.
[<Erase>]
type Queue<'a> = Queue of obj

[<Erase>]
type IExports =
    /// Creates a new empty queue.
    abstract ``new``: unit -> Queue<'a>

    /// Tests if Q is an empty queue.
    abstract is_empty: q: Queue<'a> -> bool

    /// Tests if Term is a queue.
    abstract is_queue: term: obj -> bool

    /// Returns the number of elements in queue Q. O(1) time.
    abstract len: q: Queue<'a> -> int

    /// Inserts Item at the rear of queue Q. Returns the resulting queue. O(1) time.
    abstract ``in``: item: 'a * q: Queue<'a> -> Queue<'a>

    /// Inserts Item at the front of queue Q. Returns the resulting queue. O(1) time.
    abstract in_r: item: 'a * q: Queue<'a> -> Queue<'a>

    /// Returns the front element of Q without removing it.
    /// Raises an error on an empty queue — use peek for a safe alternative.
    abstract head: q: Queue<'a> -> 'a

    /// Returns the rear element of Q without removing it.
    /// Raises an error on an empty queue — use peekRear for a safe alternative.
    abstract last: q: Queue<'a> -> 'a

    /// Returns Q with the front element removed.
    /// Raises an error on an empty queue.
    abstract tail: q: Queue<'a> -> Queue<'a>

    /// Returns Q with the rear element removed.
    /// Raises an error on an empty queue.
    abstract init: q: Queue<'a> -> Queue<'a>

    /// Converts the queue to a list, front first.
    abstract to_list: q: Queue<'a> -> 'a list

    /// Builds a queue from a list. The head of the list becomes the front element.
    abstract from_list: l: 'a list -> Queue<'a>

    /// Returns true if Item is a member of Q, otherwise false. O(n) time.
    abstract ``member``: item: 'a * q: Queue<'a> -> bool

    /// Returns a queue Q2 that is the reverse of Q1.
    abstract reverse: q: Queue<'a> -> Queue<'a>

    /// Joins Q1 and Q2. The rear of Q1 becomes the front of Q2. O(1) time.
    abstract join: q1: Queue<'a> * q2: Queue<'a> -> Queue<'a>

    /// Returns a queue of all elements of Q for which Pred(Elem) returns true.
    abstract filter: pred: System.Func<'a, bool> * q: Queue<'a> -> Queue<'a>

/// queue module
[<ImportAll("queue")>]
let queue: IExports = nativeOnly

// ============================================================================
// Typed API — functions with non-trivial Erlang return values
// ============================================================================
// WORKAROUND: Emit expressions wrapped in (fun() -> ... end)() to prevent
// Erlang "unsafe variable" errors.

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
