/// Type bindings for Erlang os module
/// See https://www.erlang.org/doc/apps/kernel/os
module Fable.Beam.Os

open Fable.Core

// fsharplint:disable MemberNames

// ============================================================================
// Environment variables
// ============================================================================
// WORKAROUND: Emit expressions wrapped in (fun() -> ... end)() to prevent
// Erlang "unsafe variable" errors. Remove IIFEs once fixed in Fable.

/// Gets an environment variable. Returns None if not set
/// (os:getenv returns the atom `false` when unset).
/// Handles binary_to_list/list_to_binary conversion.
[<Emit("(fun() -> case os:getenv(binary_to_list($0)) of false -> undefined; Value -> erlang:list_to_binary(Value) end end)()")>]
let getenv (name: string) : string option = nativeOnly

/// Sets an environment variable.
[<Emit("os:putenv(binary_to_list($0), binary_to_list($1))")>]
let putenv (name: string) (value: string) : unit = nativeOnly

/// Unsets an environment variable.
[<Emit("os:unsetenv(binary_to_list($0))")>]
let unsetenv (name: string) : unit = nativeOnly

// ============================================================================
// System commands
// ============================================================================

/// Executes a command in the OS shell. Returns the output as a string.
[<Emit("erlang:list_to_binary(os:cmd(binary_to_list($0)))")>]
let cmd (command: string) : string = nativeOnly

/// Returns the OS type as a tuple {Family, Name}.
[<Emit("os:type()")>]
let osType () : obj = nativeOnly

/// Returns the OS version as a tuple {Major, Minor, Release}.
[<Emit("os:version()")>]
let version () : obj = nativeOnly

// ============================================================================
// Time
// ============================================================================

/// Returns the current OS system time in the given unit.
[<Emit("os:system_time($0)")>]
let systemTime (unit: obj) : int = nativeOnly

/// Returns the current OS system time in seconds.
[<Emit("os:system_time(second)")>]
let systemTimeSeconds () : int = nativeOnly

/// Returns the current OS system time in milliseconds.
[<Emit("os:system_time(millisecond)")>]
let systemTimeMs () : int = nativeOnly
