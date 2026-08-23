/// Type bindings for OTP supervisor behaviour
/// See https://www.erlang.org/doc/apps/stdlib/supervisor
module Fable.Beam.Supervisor

open Fable.Core
open Fable.Beam
open Fable.Beam.GenServer

// fsharplint:disable MemberNames

// ============================================================================
// Child specification
// ============================================================================

/// Restart strategy for a child process.
[<RequireQualifiedAccess>]
type Restart =
    | [<CompiledName("permanent")>] Permanent
    | [<CompiledName("transient")>] Transient
    | [<CompiledName("temporary")>] Temporary

/// Whether a child is a worker or a (sub)supervisor.
[<RequireQualifiedAccess>]
type ChildType =
    | [<CompiledName("worker")>] Worker
    | [<CompiledName("supervisor")>] Supervisor

/// How long to wait for a child to terminate. Erased at runtime.
[<Erase>]
type Shutdown = Shutdown of obj

/// Wait up to the given milliseconds for a graceful shutdown.
[<Emit("$0")>]
let shutdownTimeout (ms: int) : Shutdown = nativeOnly

/// Terminate the child immediately with exit(kill).
[<Emit("brutal_kill")>]
let brutalKill: Shutdown = nativeOnly

/// Wait indefinitely for shutdown (typically for supervisor children).
[<Emit("infinity")>]
let shutdownInfinity: Shutdown = nativeOnly

/// A child specification (map form). `Start` is the `{Module, Function, Args}`
/// entry point that returns `{ok, Pid}`. Compiles to an Erlang child-spec map.
type ChildSpec =
    { Id: Atom
      Start: Atom * Atom * obj list
      Restart: Restart
      Shutdown: Shutdown
      Type: ChildType }

// ============================================================================
// Supervisor reference
// ============================================================================

/// Reference to a running supervisor: its pid or registered name. Erased.
[<Erase>]
type SupRef = SupRef of obj

/// Reference a supervisor by pid.
[<Emit("$0")>]
let fromPid (pid: Pid<obj>) : SupRef = nativeOnly

/// Reference a supervisor by its locally-registered name.
[<Emit("$0")>]
let fromName (name: Atom) : SupRef = nativeOnly

// ============================================================================
// Bindings
// ============================================================================

/// Starts a supervisor process. Returns the supervisor pid or an error term.
[<Emit("supervisor:start_link($0, $1)")>]
let startLink (``module``: Atom) (args: 'Args) : Result<Pid<obj>, Dynamic> = nativeOnly

/// Starts a named supervisor process.
[<Emit("supervisor:start_link($0, $1, $2)")>]
let startLinkNamed (name: ServerName) (``module``: Atom) (args: 'Args) : Result<Pid<obj>, Dynamic> = nativeOnly

/// Dynamically adds and starts a child.
[<Emit("supervisor:start_child($0, $1)")>]
let startChild (supRef: SupRef) (childSpec: ChildSpec) : Result<Pid<obj>, Dynamic> = nativeOnly

/// Terminates a running child by id.
[<Emit("(fun() -> case supervisor:terminate_child($0, $1) of ok -> {ok, ok}; {error, SupTerminateChildReason__} -> {error, SupTerminateChildReason__} end end)()")>]
let terminateChild (supRef: SupRef) (id: Atom) : Result<unit, Atom> = nativeOnly

/// Restarts a previously-terminated child by id.
[<Emit("supervisor:restart_child($0, $1)")>]
let restartChild (supRef: SupRef) (id: Atom) : Result<Pid<obj>, Dynamic> = nativeOnly

/// Deletes a child specification by id.
[<Emit("(fun() -> case supervisor:delete_child($0, $1) of ok -> {ok, ok}; {error, SupDeleteChildReason__} -> {error, SupDeleteChildReason__} end end)()")>]
let deleteChild (supRef: SupRef) (id: Atom) : Result<unit, Atom> = nativeOnly

/// Returns children as `{Id, Child, Type, Modules}` tuples.
[<Emit("supervisor:which_children($0)")>]
let children (supRef: SupRef) : Dynamic = nativeOnly

/// Returns child counts (`specs`, `active`, `supervisors`, `workers`).
[<Emit("supervisor:count_children($0)")>]
let childCounts (supRef: SupRef) : Dynamic = nativeOnly
