/// Type bindings for OTP supervisor behaviour
/// See https://www.erlang.org/doc/apps/stdlib/supervisor
module Fable.Beam.Supervisor

open Fable.Core
open Fable.Beam
open Fable.Beam.GenServer

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Starts a supervisor process.
    abstract start_link: ``module``: Atom * args: 'Args -> obj
    /// Starts a named supervisor process.
    abstract start_link: name: ServerName * ``module``: Atom * args: 'Args -> obj
    /// Dynamically adds a child specification to a supervisor.
    abstract start_child: supRef: obj * childSpec: obj -> obj
    /// Terminates a child process.
    abstract terminate_child: supRef: obj * id: Atom -> obj
    /// Restarts a child process.
    abstract restart_child: supRef: obj * id: Atom -> obj
    /// Deletes a child specification.
    abstract delete_child: supRef: obj * id: Atom -> obj
    /// Returns a list of all child specifications and child processes.
    abstract which_children: supRef: obj -> obj array
    /// Returns a list with counts for each child specification.
    abstract count_children: supRef: obj -> obj

/// supervisor module
[<ImportAll("supervisor")>]
let supervisor: IExports = nativeOnly
