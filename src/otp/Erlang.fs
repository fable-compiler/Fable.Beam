/// Type bindings for Erlang BIFs (Built-in Functions)
/// See https://www.erlang.org/doc/apps/erts/erlang
module Fable.Beam.Erlang

open Fable.Core

// Note: For selective receive, use Fable.Core.BeamInterop.Erlang.receive<'T>
// which is provided by Fable.Core and handled by the compiler.

// ============================================================================
// Opaque Erlang types
// ============================================================================

/// Erlang process identifier.
[<Erase>]
type Pid = Pid of obj

/// Erlang reference (from make_ref, monitor, etc.).
[<Erase>]
type Ref = Ref of obj

/// Erlang atom.
[<Erase>]
type Atom = Atom of obj

/// Erlang timer reference (from send_after, etc.).
[<Erase>]
type TimerRef = TimerRef of obj

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

/// Enable trap_exit so EXIT signals become messages.
[<Emit("erlang:process_flag(trap_exit, true)")>]
let trapExit () : unit = nativeOnly

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

/// Look up a registered process name.
[<Emit("erlang:whereis($0)")>]
let whereis (name: Atom) : Pid = nativeOnly

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

/// Schedule a message to be sent after Ms milliseconds.
[<Emit("erlang:send_after($0, erlang:self(), $1)")>]
let sendAfter (ms: int) (msg: obj) : TimerRef = nativeOnly

/// Cancel a timer.
[<Emit("erlang:cancel_timer($0)")>]
let cancelTimer (timerRef: TimerRef) : unit = nativeOnly

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

/// Returns the size of a binary.
[<Emit("erlang:byte_size($0)")>]
let byteSize (bin: obj) : int = nativeOnly

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

/// Convert an atom to a list (charlist).
[<Emit("erlang:atom_to_list($0)")>]
let atomToList (atom: Atom) : string = nativeOnly

/// Convert a list (charlist) to an atom.
[<Emit("erlang:list_to_atom($0)")>]
let listToAtom (s: string) : Atom = nativeOnly

/// Format a term as a readable string.
[<Emit("erlang:list_to_binary(io_lib:format(<<\"~p\">>, [$0]))")>]
let formatTerm (term: obj) : string = nativeOnly
