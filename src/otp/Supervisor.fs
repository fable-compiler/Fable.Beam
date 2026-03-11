/// Type bindings for OTP supervisor behaviour
/// See https://www.erlang.org/doc/apps/stdlib/supervisor
module Fable.Beam.Supervisor

open Fable.Core

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Starts a supervisor process.
    abstract start_link: ``module``: obj * args: obj -> obj
    /// Starts a named supervisor process.
    abstract start_link: name: obj * ``module``: obj * args: obj -> obj
    /// Dynamically adds a child specification to a supervisor.
    abstract start_child: supRef: obj * childSpec: obj -> obj
    /// Terminates a child process.
    abstract terminate_child: supRef: obj * id: obj -> obj
    /// Restarts a child process.
    abstract restart_child: supRef: obj * id: obj -> obj
    /// Deletes a child specification.
    abstract delete_child: supRef: obj * id: obj -> obj
    /// Returns a list of all child specifications and child processes.
    abstract which_children: supRef: obj -> obj array
    /// Returns a list with counts for each child specification.
    abstract count_children: supRef: obj -> obj

/// supervisor module
[<ImportAll("supervisor")>]
let supervisor: IExports = nativeOnly
