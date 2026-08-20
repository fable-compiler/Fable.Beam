/// Typed bindings for OTP ports used to communicate with external OS processes.
/// See https://www.erlang.org/doc/apps/erts/erlang#open_port-2
module Fable.Beam.Port

open Fable.Core

// fsharplint:disable MemberNames

/// A running OTP port. This opaque value is valid only on the BEAM runtime.
///
/// The API type-checks on .NET, but opening, sending to, closing, and receiving
/// from ports are BEAM runtime operations implemented by the Fable compiler.
[<Erase>]
type Port = private Port of obj

/// A message delivered by a port opened with line mode and `exit_status`.
///
/// `IncompleteLine` is an output fragment delivered when a line exceeds the
/// maximum length supplied at start-up, or when the process ends a line without
/// a newline. The data is always an Erlang binary, represented as F# `string`.
///
/// Note: when the process ends mid-line, ERTS delivers the `ExitStatus` message
/// *before* the pending `IncompleteLine` fragment. So `ExitStatus` is not a
/// terminal marker — an `IncompleteLine` may still arrive after it. If you need
/// the trailing fragment, keep receiving once more after `ExitStatus`.
type Message =
    | [<CompiledName("line")>] Line of data: string
    | [<CompiledName("incomplete_line")>] IncompleteLine of data: string
    | [<CompiledName("exit_status")>] ExitStatus of status: int

// The emitted code is deliberately the only raw OTP boundary. It constructs
// `open_port` option tuples and decodes native port mailbox tuples before they
// reach consuming F# code.

/// Starts an executable at an absolute path with separate argument values.
///
/// The process is connected through stdin/stdout in binary, line-delimited mode
/// and delivers its exit status. `maxLineLength` must be positive. Arguments are
/// passed as an OTP argument vector, never interpolated into a shell command.
[<Emit("(fun() -> case $2 > 0 andalso filename:pathtype(binary_to_list($0)) =:= absolute of false -> {error, <<\"path must be absolute and maxLineLength must be positive\">>}; true -> try {ok, erlang:open_port({spawn_executable, binary_to_list($0)}, [binary, {line, $2}, exit_status, use_stdio, {args, [binary_to_list(PortArg__) || PortArg__ <- $1]}])} catch error:PortStartReason__ -> {error, erlang:iolist_to_binary(io_lib:format(\"~p\", [PortStartReason__]))} end end end)()")>]
let startAbsolute (path: string) (arguments: string list) (maxLineLength: int) : Result<Port, string> = nativeOnly

/// Finds an executable on `PATH` and starts it with separate argument values.
///
/// Resolution uses `os:find_executable/1`; the resolved file is then started
/// with `spawn_executable`, so this does not invoke a shell or interpolate
/// argument text. See `startAbsolute` for runtime and line-mode details.
[<Emit("(fun() -> case $2 > 0 of false -> {error, <<\"maxLineLength must be positive\">>}; true -> case os:find_executable(binary_to_list($0)) of false -> {error, <<\"executable not found on PATH\">>}; PortPath__ -> try {ok, erlang:open_port({spawn_executable, PortPath__}, [binary, {line, $2}, exit_status, use_stdio, {args, [binary_to_list(PortArg__) || PortArg__ <- $1]}])} catch error:PortStartReason__ -> {error, erlang:iolist_to_binary(io_lib:format(\"~p\", [PortStartReason__]))} end end end end)()")>]
let startOnPath (name: string) (arguments: string list) (maxLineLength: int) : Result<Port, string> = nativeOnly

/// Sends binary data to the process's standard input.
///
/// F# strings compile to Erlang binaries. Returns true when OTP accepted the
/// command for the port; it raises if the port is no longer valid.
[<Emit("erlang:port_command($0, $1)")>]
let send (port: Port) (data: string) : bool = nativeOnly

/// Closes the port and its OS process.
[<Emit("erlang:port_close($0)")>]
let close (port: Port) : unit = nativeOnly

/// Selectively receives the next message from this port, or None after timeout.
///
/// Messages unrelated to this port remain in the current process mailbox, so
/// this composes with Fable.Actor and other process protocols without exposing
/// raw Erlang tuples to callers.
[<Emit("(fun() -> receive {$0, {data, {eol, PortLineData__}}} -> {line, PortLineData__}; {$0, {data, {noeol, PortFragmentData__}}} -> {incomplete_line, PortFragmentData__}; {$0, {exit_status, PortExitStatus__}} -> {exit_status, PortExitStatus__} after $1 -> undefined end end)()")>]
let receive (port: Port) (timeoutMs: int) : Message option = nativeOnly
