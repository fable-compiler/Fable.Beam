/// Type bindings for Erlang BIFs (Built-in Functions)
/// See https://www.erlang.org/doc/apps/erts/erlang
[<RequireQualifiedAccess>]
module Fable.Beam.Erlang

open Fable.Core
open Fable.Beam

// Note: For selective receive, use Fable.Core.BeamInterop.Erlang.receive<'T>
// which is provided by Fable.Core and handled by the compiler.

// ============================================================================
// Process management
// ============================================================================

/// Get the current process's pid.
[<Emit("erlang:self()")>]
let self () : Pid = nativeOnly

/// Spawn a new process that executes the given function.
[<Emit("erlang:spawn(fun() -> $0(ok) end)")>]
let spawn (f: unit -> unit) : Pid = nativeOnly

/// Spawn a linked process.
[<Emit("erlang:spawn_link(fun() -> $0(ok) end)")>]
let spawnLink (f: unit -> unit) : Pid = nativeOnly

/// Send a message to a process (Pid ! Msg).
[<Emit("$0 ! $1")>]
let send (pid: Pid) (msg: obj) : unit = nativeOnly

/// Enable trap_exit so EXIT signals become messages. Returns the old value.
[<Emit("erlang:process_flag(trap_exit, true)")>]
let trapExit () : bool = nativeOnly

/// Set a process flag.
[<Emit("erlang:process_flag($0, $1)")>]
let processFlag (flag: Atom) (value: obj) : obj = nativeOnly

/// Exit the current process with a reason.
[<Emit("erlang:exit($0)")>]
let exit (reason: obj) : unit = nativeOnly

/// Send an exit signal to a process.
[<Emit("erlang:exit($0, $1)")>]
let exitPid (pid: Pid) (reason: obj) : unit = nativeOnly

/// Link to another process.
[<Emit("erlang:link($0)")>]
let link (pid: Pid) : unit = nativeOnly

/// Unlink from a process.
[<Emit("erlang:unlink($0)")>]
let unlink (pid: Pid) : unit = nativeOnly

/// Monitor a process. Returns a monitor reference.
[<Emit("erlang:monitor(process, $0)")>]
let monitor (pid: Pid) : Ref = nativeOnly

/// Demonitor a process.
[<Emit("erlang:demonitor($0)")>]
let demonitor (ref: Ref) : unit = nativeOnly

/// Demonitor a process with flush option.
[<Emit("erlang:demonitor($0, [flush])")>]
let demonitorFlush (ref: Ref) : unit = nativeOnly

/// Register a name for the calling process.
[<Emit("erlang:register($0, $1)")>]
let register (name: Atom) (pid: Pid) : unit = nativeOnly

/// Look up a registered process name. Returns None if not registered.
[<Emit("(fun() -> case erlang:whereis($0) of undefined -> undefined; WhereIsPid__ -> WhereIsPid__ end end)()")>]
let whereis (name: Atom) : Pid option = nativeOnly

/// Check if a process is alive.
[<Emit("erlang:is_process_alive($0)")>]
let isProcessAlive (pid: Pid) : bool = nativeOnly

// ============================================================================
// References and identity
// ============================================================================

/// Create a unique reference.
[<Emit("erlang:make_ref()")>]
let makeRef () : Ref = nativeOnly

/// Exact equality comparison (=:=).
[<Emit("$0 =:= $1")>]
let exactEquals (a: obj) (b: obj) : bool = nativeOnly

// ============================================================================
// Time
// ============================================================================

/// Get the current monotonic time in milliseconds.
[<Emit("erlang:monotonic_time(millisecond)")>]
let monotonicTimeMs () : int = nativeOnly

/// Get system time in the given unit.
[<Emit("erlang:system_time($0)")>]
let systemTime (unit: Atom) : int = nativeOnly

/// Get system time in seconds (Unix epoch).
[<Emit("erlang:system_time(second)")>]
let systemTimeSec () : int = nativeOnly

/// Hash a term to a value in [0, Range). Useful for partitioning or load balancing.
[<Emit("erlang:phash2($0, $1)")>]
let phash2 (term: obj) (range: int) : int = nativeOnly

/// Schedule a message to be sent to self after Ms milliseconds.
[<Emit("erlang:send_after($0, erlang:self(), $1)")>]
let sendAfter (ms: int) (msg: obj) : TimerRef = nativeOnly

/// Schedule a message to be sent to the given process after Ms milliseconds.
[<Emit("erlang:send_after($0, $1, $2)")>]
let sendAfterTo (ms: int) (dest: Pid) (msg: obj) : TimerRef = nativeOnly

/// Cancel a timer. Returns the remaining time in ms, or None if the timer was not found.
[<Emit("(fun() -> case erlang:cancel_timer($0) of false -> undefined; CancelTimerMs__ -> CancelTimerMs__ end end)()")>]
let cancelTimer (timerRef: TimerRef) : int option = nativeOnly

// ============================================================================
// Process dictionary
// ============================================================================

/// Get a value from the process dictionary.
[<Emit("erlang:get($0)")>]
let get (key: obj) : obj = nativeOnly

/// Put a value in the process dictionary.
[<Emit("erlang:put($0, $1)")>]
let put (key: obj) (value: obj) : obj = nativeOnly

/// Erase a key from the process dictionary.
[<Emit("erlang:erase($0)")>]
let erase (key: obj) : obj = nativeOnly

// ============================================================================
// Date and time
// ============================================================================

/// Returns the current date as {Year, Month, Day}.
[<Emit("erlang:date()")>]
let date () : int * int * int = nativeOnly

/// Returns the current year.
[<Emit("element(1, erlang:date())")>]
let dateYear () : int = nativeOnly

/// Returns the current month (1-12).
[<Emit("element(2, erlang:date())")>]
let dateMonth () : int = nativeOnly

/// Returns the current day of month (1-31).
[<Emit("element(3, erlang:date())")>]
let dateDay () : int = nativeOnly

/// Returns the current time as {Hour, Minute, Second}.
[<Emit("erlang:time()")>]
let time () : int * int * int = nativeOnly

/// Returns the current local date and time as {{Year,Month,Day},{Hour,Minute,Second}}.
[<Emit("erlang:localtime()")>]
let localtime () : (int * int * int) * (int * int * int) = nativeOnly

/// Returns the current UTC date and time as {{Year,Month,Day},{Hour,Minute,Second}}.
[<Emit("erlang:universaltime()")>]
let universaltime () : (int * int * int) * (int * int * int) = nativeOnly

// ============================================================================
// Guards and BIFs for raw Erlang types
// ============================================================================

/// Returns the length of an Erlang list.
/// Use this instead of .Length when working with raw Erlang lists from OTP calls,
/// since F# .Length expects a ref-wrapped array, not a native Erlang list.
[<Emit("erlang:length($0)")>]
let length (list: obj) : int = nativeOnly

/// Returns the size of a tuple.
[<Emit("erlang:tuple_size($0)")>]
let tupleSize (tuple: obj) : int = nativeOnly

/// Returns the size of a binary (string).
[<Emit("erlang:byte_size($0)")>]
let byteSize (bin: string) : int = nativeOnly

/// Returns the element at position N (1-based) in a tuple.
[<Emit("erlang:element($0, $1)")>]
let element (n: int) (tuple: obj) : obj = nativeOnly

// ============================================================================
// Type checks and conversions
// ============================================================================

/// Convert a term to a binary string for display.
[<Emit("erlang:term_to_binary($0)")>]
let termToBinary (term: obj) : obj = nativeOnly

/// Convert a binary back to a term.
[<Emit("erlang:binary_to_term($0)")>]
let binaryToTerm (bin: obj) : obj = nativeOnly

/// Convert a list to a binary.
[<Emit("erlang:list_to_binary($0)")>]
let listToBinary (list: obj) : obj = nativeOnly

/// Convert an atom to a binary string.
[<Emit("erlang:atom_to_binary($0)")>]
let atomToBinary (atom: Atom) : string = nativeOnly

/// Convert a binary string to an atom.
[<Emit("erlang:binary_to_atom($0)")>]
let binaryToAtom (s: string) : Atom = nativeOnly

/// Convert an atom to a charlist. Note: returns a charlist (Erlang string()),
/// not an F# string (binary). Use atomToBinary for F# string conversion.
[<Emit("erlang:atom_to_list($0)")>]
let atomToList (atom: Atom) : obj = nativeOnly

/// Convert a charlist to an atom. Note: expects a charlist (Erlang string()),
/// not an F# string (binary). Use binaryToAtom for F# string conversion.
[<Emit("erlang:list_to_atom($0)")>]
let listToAtom (charlist: obj) : Atom = nativeOnly

/// Format a term as a readable string.
[<Emit("erlang:list_to_binary(io_lib:format(<<\"~p\">>, [$0]))")>]
let formatTerm (term: obj) : string = nativeOnly
