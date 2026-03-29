/// Type bindings for Erlang os module
/// See https://www.erlang.org/doc/apps/kernel/os
module Fable.Beam.Os

open Fable.Core
open Fable.Beam

// fsharplint:disable MemberNames

// ============================================================================
// Environment variables
// ============================================================================
// WORKAROUND: Emit expressions wrapped in (fun() -> ... end)() to prevent
// Erlang "unsafe variable" errors. Remove IIFEs once fixed in Fable.

/// Gets an environment variable. Returns None if not set
/// (os:getenv returns the atom `false` when unset).
/// Handles binary_to_list/list_to_binary conversion.
[<Emit("(fun() -> case os:getenv(binary_to_list($0)) of false -> undefined; OsGetEnv__ -> erlang:list_to_binary(OsGetEnv__) end end)()")>]
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

/// Returns the OS type as a tuple {Family, Name} where Family is unix or win32.
[<Emit("(fun() -> {OsTypeF__, OsTypeN__} = os:type(), {erlang:atom_to_binary(OsTypeF__), erlang:atom_to_binary(OsTypeN__)} end)()")>]
let osType () : string * string = nativeOnly

/// Returns the OS version as a tuple {Major, Minor, Release}.
[<Emit("os:version()")>]
let version () : int * int * int = nativeOnly

// ============================================================================
// Time
// ============================================================================

/// Returns the current OS system time in the given unit (e.g., second, millisecond).
[<Emit("os:system_time($0)")>]
let systemTime (unit: Atom) : int64 = nativeOnly

/// Returns the current OS system time in seconds.
[<Emit("os:system_time(second)")>]
let systemTimeSeconds () : int64 = nativeOnly

/// Returns the current OS system time in milliseconds.
[<Emit("os:system_time(millisecond)")>]
let systemTimeMs () : int64 = nativeOnly
