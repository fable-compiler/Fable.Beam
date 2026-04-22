/// Type bindings for OTP application module
/// See https://www.erlang.org/doc/apps/kernel/application
module Fable.Beam.Application

open Fable.Core
open Fable.Beam

// fsharplint:disable MemberNames

/// Raw low-level bindings. Prefer the typed helpers below for most use cases.
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
    abstract get_env: app: Atom * key: Atom * defaultValue: 'V -> 'V
    /// Sets an application environment variable.
    abstract set_env: app: Atom * key: Atom * value: 'V -> unit
    /// Returns a list of all running applications.
    abstract which_applications: unit -> obj

/// application module
[<ImportAll("application")>]
let application: IExports = nativeOnly

// ============================================================================
// Typed API
// ============================================================================
// WORKAROUND: Emit expressions wrapped in (fun() -> ... end)() to prevent
// Erlang "unsafe variable" errors. Remove IIFEs once fixed in Fable.

/// Gets an application environment variable. Returns None if unset.
/// The returned Dynamic must be decoded with `Fable.Beam.Decode` combinators.
[<Emit("(fun() -> case application:get_env($0, $1) of {ok, GetEnvVal__} -> GetEnvVal__; undefined -> undefined end end)()")>]
let getEnv (app: Atom) (key: Atom) : Dynamic option = nativeOnly

/// Starts an application and all its dependencies. Returns the list of apps
/// that were started (empty if everything was already running).
[<Emit("(fun() -> case application:ensure_all_started($0) of {ok, Started__} -> {ok, fable_utils:new_ref(Started__)}; {error, Reason__} -> {error, erlang:list_to_binary(io_lib:format(<<\"~p\">>, [Reason__]))} end end)()")>]
let ensureAllStarted (app: Atom) : Result<Atom array, string> = nativeOnly

/// Starts an application (without dependencies). Returns Ok on success.
[<Emit("(fun() -> case application:start($0) of ok -> {ok, ok}; {error, Reason__} -> {error, erlang:list_to_binary(io_lib:format(<<\"~p\">>, [Reason__]))} end end)()")>]
let start (app: Atom) : Result<unit, string> = nativeOnly

/// Stops an application. Returns Ok on success.
[<Emit("(fun() -> case application:stop($0) of ok -> {ok, ok}; {error, Reason__} -> {error, erlang:list_to_binary(io_lib:format(<<\"~p\">>, [Reason__]))} end end)()")>]
let stop (app: Atom) : Result<unit, string> = nativeOnly
