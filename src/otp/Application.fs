/// Type bindings for OTP application module
/// See https://www.erlang.org/doc/apps/kernel/application
module Fable.Beam.Application

open Fable.Core
open Fable.Beam

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Starts an application and all its dependencies.
    abstract ensure_all_started: app: Atom -> obj
    /// Starts an application.
    abstract start: app: Atom -> obj
    /// Stops an application.
    abstract stop: app: Atom -> obj
    /// Gets an application environment variable.
    abstract get_env: app: Atom * key: Atom -> obj
    /// Gets an application environment variable with default.
    abstract get_env: app: Atom * key: Atom * defaultValue: obj -> obj
    /// Sets an application environment variable.
    abstract set_env: app: Atom * key: Atom * value: obj -> obj
    /// Returns a list of all running applications.
    abstract which_applications: unit -> obj

/// application module
[<ImportAll("application")>]
let application: IExports = nativeOnly
