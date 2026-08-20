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

/// A monitor installed for a port. It is owned by the process that called
/// `monitor`, and can be consumed with `receiveDown`.
[<Erase>]
type PortMonitor = private PortMonitor of obj

/// Launch and ownership settings for a port.
///
/// `arguments` is always an argument vector, never a shell command string.
/// `environment` supplies overrides to the child environment. `linkOwner`
/// opts into OTP link semantics; it is false by default so an unexpected child
/// exit cannot terminate the caller.
type PortOptions =
    { arguments: string list
      maxLineLength: int
      exitStatus: bool
      useStdio: bool
      stderrToStdout: bool
      workingDirectory: string option
      environment: (string * string) list
      linkOwner: bool }

/// Safe defaults for a line-oriented stdin/stdout process connection.
module PortOptions =
    let defaultOptions =
        { arguments = []
          maxLineLength = 8192
          exitStatus = true
          useStdio = true
          stderrToStdout = false
          workingDirectory = None
          environment = []
          linkOwner = false }

/// A message delivered by a port opened with line mode. `ExitStatus` is
/// delivered when `PortOptions.exitStatus` is enabled.
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

/// A down notification emitted for a monitored port.
type Down = Down of reason: string

// The emitted code is deliberately the only raw OTP boundary. It constructs
// `open_port` option tuples and decodes native port mailbox tuples before they
// reach consuming F# code.

/// Starts an executable at an absolute path using explicit port options.
/// `maxLineLength` must be positive. `stderrToStdout` requires `useStdio`.
[<Emit("(fun() -> case $2 > 0 andalso filename:pathtype(binary_to_list($0)) =:= absolute andalso (not $5 orelse $4) of false -> {error, <<\"path must be absolute, maxLineLength must be positive, and stderrToStdout requires useStdio\">>}; true -> PortDirectoryOpts__ = case $6 of undefined -> []; PortWorkingDirectory__ -> [{cd, binary_to_list(PortWorkingDirectory__)}] end, PortEnvironmentOpts__ = case $7 of [] -> []; PortEnvironment__ -> [{env, [{binary_to_list(PortEnvironmentKey__), binary_to_list(PortEnvironmentValue__)} || {PortEnvironmentKey__, PortEnvironmentValue__} <- PortEnvironment__]}] end, PortExitStatusOpts__ = case $3 of true -> [exit_status]; false -> [] end, PortStdioOpts__ = case $4 of true -> [use_stdio]; false -> [] end, PortStderrOpts__ = case $5 of true -> [stderr_to_stdout]; false -> [] end, PortLinkOpts__ = case $8 of true -> [link]; false -> [] end, PortOpts__ = [binary, {line, $2}, {args, [binary_to_list(PortArg__) || PortArg__ <- $1]}] ++ PortExitStatusOpts__ ++ PortStdioOpts__ ++ PortStderrOpts__ ++ PortDirectoryOpts__ ++ PortEnvironmentOpts__ ++ PortLinkOpts__, try {ok, erlang:open_port({spawn_executable, binary_to_list($0)}, PortOpts__)} catch error:PortStartReason__ -> {error, erlang:iolist_to_binary(io_lib:format(\"~p\", [PortStartReason__]))} end end end)()")>]
let private startAbsoluteImpl
    (path: string)
    (arguments: string list)
    (maxLineLength: int)
    (exitStatus: bool)
    (useStdio: bool)
    (stderrToStdout: bool)
    (workingDirectory: string option)
    (environment: (string * string) list)
    (linkOwner: bool)
    : Result<Port, string> =
    nativeOnly

let startAbsolute (path: string) (options: PortOptions) : Result<Port, string> =
    startAbsoluteImpl
        path
        options.arguments
        options.maxLineLength
        options.exitStatus
        options.useStdio
        options.stderrToStdout
        options.workingDirectory
        options.environment
        options.linkOwner

/// Finds an executable on `PATH` and starts it with separate argument values.
///
/// Resolution uses `os:find_executable/1`; the resolved file is then started
/// with `spawn_executable`, so this does not invoke a shell or interpolate
/// argument text. See `startAbsolute` for runtime and line-mode details.
[<Emit("(fun() -> case $2 > 0 andalso (not $5 orelse $4) of false -> {error, <<\"maxLineLength must be positive and stderrToStdout requires useStdio\">>}; true -> case os:find_executable(binary_to_list($0)) of false -> {error, <<\"executable not found on PATH\">>}; PortPath__ -> PortDirectoryOpts__ = case $6 of undefined -> []; PortWorkingDirectory__ -> [{cd, binary_to_list(PortWorkingDirectory__)}] end, PortEnvironmentOpts__ = case $7 of [] -> []; PortEnvironment__ -> [{env, [{binary_to_list(PortEnvironmentKey__), binary_to_list(PortEnvironmentValue__)} || {PortEnvironmentKey__, PortEnvironmentValue__} <- PortEnvironment__]}] end, PortExitStatusOpts__ = case $3 of true -> [exit_status]; false -> [] end, PortStdioOpts__ = case $4 of true -> [use_stdio]; false -> [] end, PortStderrOpts__ = case $5 of true -> [stderr_to_stdout]; false -> [] end, PortLinkOpts__ = case $8 of true -> [link]; false -> [] end, PortOpts__ = [binary, {line, $2}, {args, [binary_to_list(PortArg__) || PortArg__ <- $1]}] ++ PortExitStatusOpts__ ++ PortStdioOpts__ ++ PortStderrOpts__ ++ PortDirectoryOpts__ ++ PortEnvironmentOpts__ ++ PortLinkOpts__, try {ok, erlang:open_port({spawn_executable, PortPath__}, PortOpts__)} catch error:PortStartReason__ -> {error, erlang:iolist_to_binary(io_lib:format(\"~p\", [PortStartReason__]))} end end end end)()")>]
let private startOnPathImpl
    (name: string)
    (arguments: string list)
    (maxLineLength: int)
    (exitStatus: bool)
    (useStdio: bool)
    (stderrToStdout: bool)
    (workingDirectory: string option)
    (environment: (string * string) list)
    (linkOwner: bool)
    : Result<Port, string> =
    nativeOnly

let startOnPath (name: string) (options: PortOptions) : Result<Port, string> =
    startOnPathImpl
        name
        options.arguments
        options.maxLineLength
        options.exitStatus
        options.useStdio
        options.stderrToStdout
        options.workingDirectory
        options.environment
        options.linkOwner

/// Sends binary data to the process's standard input.
///
/// F# strings compile to Erlang binaries. Returns true when OTP accepted the
/// command for the port; it raises if the port is no longer valid.
[<Emit("erlang:port_command($0, $1)")>]
let send (port: Port) (data: string) : bool = nativeOnly

/// Sends data without raising when the port has already disappeared.
[<Emit("(fun() -> try case erlang:port_command($0, $1) of true -> {ok, []}; false -> {error, <<\"port command was not accepted\">>} end catch error:PortSendReason__ -> {error, erlang:iolist_to_binary(io_lib:format(\"~p\", [PortSendReason__]))} end end)()")>]
let trySend (port: Port) (data: string) : Result<unit, string> = nativeOnly

/// Closes the port and its OS process.
[<Emit("erlang:port_close($0)")>]
let close (port: Port) : unit = nativeOnly

/// Closes a port without raising when it has already disappeared.
[<Emit("(fun() -> try erlang:port_close($0), {ok, []} catch error:PortCloseReason__ -> {error, erlang:iolist_to_binary(io_lib:format(\"~p\", [PortCloseReason__]))} end end)()")>]
let tryClose (port: Port) : Result<unit, string> = nativeOnly

/// Monitors a port without linking it to the caller.
[<Emit("erlang:monitor(port, $0)")>]
let monitor (port: Port) : PortMonitor = nativeOnly

/// Selectively receives the down notification for a monitored port.
[<Emit("(fun() -> receive {'DOWN', $1, port, $0, PortDownReason__} -> {down, erlang:iolist_to_binary(io_lib:format(\"~p\", [PortDownReason__]))} after $2 -> undefined end end)()")>]
let receiveDown (port: Port) (monitor: PortMonitor) (timeoutMs: int) : Down option = nativeOnly

/// Selectively receives the next message from this port, or None after timeout.
///
/// Messages unrelated to this port remain in the current process mailbox, so
/// this composes with Fable.Actor and other process protocols without exposing
/// raw Erlang tuples to callers.
[<Emit("(fun() -> receive {$0, {data, {eol, PortLineData__}}} -> {line, PortLineData__}; {$0, {data, {noeol, PortFragmentData__}}} -> {incomplete_line, PortFragmentData__}; {$0, {exit_status, PortExitStatus__}} -> {exit_status, PortExitStatus__} after $1 -> undefined end end)()")>]
let receive (port: Port) (timeoutMs: int) : Message option = nativeOnly

/// Folds port messages until the port exits or the next receive times out.
///
/// The timeout applies to each receive. If ERTS delivers `ExitStatus` before
/// its final `IncompleteLine`, this function receives and folds that trailing
/// fragment before returning. With `exitStatus = false`, it returns when a
/// receive times out. This is the preferred line-mode consumption loop.
let foldMessages (port: Port) (timeoutMs: int) (folder: 'State -> Message -> 'State) (state: 'State) : 'State =
    let rec loop state =
        match receive port timeoutMs with
        | None -> state
        | Some(ExitStatus _ as message) ->
            let state = folder state message

            match receive port timeoutMs with
            | Some(IncompleteLine _ as trailing) -> folder state trailing
            | _ -> state
        | Some message -> loop (folder state message)

    loop state

/// Receives all messages through exit (and its possible trailing fragment).
let receiveUntil (port: Port) (timeoutMs: int) : Message list =
    foldMessages port timeoutMs (fun messages message -> message :: messages) []
    |> List.rev
