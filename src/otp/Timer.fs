/// Type bindings for OTP timer module
/// See https://www.erlang.org/doc/apps/stdlib/timer
module Fable.Beam.Timer

open Fable.Core
open Fable.Beam

/// Sends a message to a process after the delay in milliseconds.
[<Emit("timer:send_after($0, $1, $2)")>]
let sendAfter (time: int) (dest: Pid<'Msg>) (msg: 'Msg) : Result<TimerRef<'Msg>, Atom> = nativeOnly

/// Sends a message to a process repeatedly after the interval in milliseconds.
[<Emit("timer:send_interval($0, $1, $2)")>]
let sendInterval (time: int) (dest: Pid<'Msg>) (msg: 'Msg) : Result<TimerRef<'Msg>, Atom> = nativeOnly

/// Evaluates a function after the delay in milliseconds.
[<Emit("timer:apply_after($0, $1, $2, $3)")>]
let applyAfter (time: int) (``module``: Atom) (``function``: Atom) (args: obj list) : Result<TimerRef<'Msg>, Atom> =
    nativeOnly

/// Evaluates a function repeatedly after the interval in milliseconds.
[<Emit("timer:apply_interval($0, $1, $2, $3)")>]
let applyInterval (time: int) (``module``: Atom) (``function``: Atom) (args: obj list) : Result<TimerRef<'Msg>, Atom> =
    nativeOnly

/// Cancels a previously started timer.
[<Emit("timer:cancel($0)")>]
let cancel (timerRef: TimerRef<'Msg>) : Result<Atom, Atom> = nativeOnly

/// Suspends the process for the given number of milliseconds.
[<Emit("timer:sleep($0)")>]
let sleep (ms: int) : unit = nativeOnly

/// Converts hours to milliseconds.
[<Emit("timer:hours($0)")>]
let hours (value: int) : int = nativeOnly

/// Converts minutes to milliseconds.
[<Emit("timer:minutes($0)")>]
let minutes (value: int) : int = nativeOnly

/// Converts seconds to milliseconds.
[<Emit("timer:seconds($0)")>]
let seconds (value: int) : int = nativeOnly
