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

[<Erase>]
type IExports =
    /// Starts a supervisor process. Returns the supervisor pid or an error term.
    abstract start_link: ``module``: Atom * args: 'Args -> Result<Pid<obj>, Dynamic>
    /// Starts a named supervisor process.
    abstract start_link: name: ServerName * ``module``: Atom * args: 'Args -> Result<Pid<obj>, Dynamic>
    /// Dynamically adds and starts a child. Returns the child pid or an error term.
    abstract start_child: supRef: SupRef * childSpec: ChildSpec -> Result<Pid<obj>, Dynamic>

    /// Terminates a running child by id. Error is an atom (e.g. 'not_found').
    /// The Emit wrapper bridges OTP's bare `ok` into Fable's `{ok, ok}` Result form.
    [<Emit("(fun() -> case supervisor:terminate_child($0, $1) of ok -> {ok, ok}; {error, SupTerminateChildReason__} -> {error, SupTerminateChildReason__} end end)()")>]
    abstract terminate_child: supRef: SupRef * id: Atom -> Result<unit, Atom>

    /// Restarts a previously-terminated child by id. Returns the new child pid.
    abstract restart_child: supRef: SupRef * id: Atom -> Result<Pid<obj>, Dynamic>

    /// Deletes a child specification by id. Error is an atom (e.g. 'not_found').
    /// The Emit wrapper bridges OTP's bare `ok` into Fable's `{ok, ok}` Result form.
    [<Emit("(fun() -> case supervisor:delete_child($0, $1) of ok -> {ok, ok}; {error, SupDeleteChildReason__} -> {error, SupDeleteChildReason__} end end)()")>]
    abstract delete_child: supRef: SupRef * id: Atom -> Result<unit, Atom>

    /// Returns the children as a dynamic list of `{Id, Child, Type, Modules}`
    /// tuples — decode with `Fable.Beam.Decode`.
    abstract which_children: supRef: SupRef -> Dynamic
    /// Returns child counts as a dynamic proplist
    /// (`specs`, `active`, `supervisors`, `workers`) — decode with `Fable.Beam.Decode`.
    abstract count_children: supRef: SupRef -> Dynamic

/// supervisor module
[<ImportAll("supervisor")>]
let supervisor: IExports = nativeOnly
