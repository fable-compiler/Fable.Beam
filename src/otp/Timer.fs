/// Type bindings for OTP timer module
/// See https://www.erlang.org/doc/apps/stdlib/timer
module Fable.Beam.Timer

open Fable.Core

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Sends Msg to Dest after Time milliseconds.
    abstract send_after: time: int * dest: obj * msg: obj -> obj
    /// Sends Msg to Dest repeatedly every Time milliseconds.
    abstract send_interval: time: int * dest: obj * msg: obj -> obj
    /// Evaluates Fun after Time milliseconds.
    abstract apply_after: time: int * ``module``: obj * ``function``: obj * args: obj list -> obj
    /// Evaluates Fun repeatedly every Time milliseconds.
    abstract apply_interval: time: int * ``module``: obj * ``function``: obj * args: obj list -> obj
    /// Cancels a previously started timer.
    abstract cancel: timerRef: obj -> obj
    /// Suspends the process for Time milliseconds.
    abstract sleep: time: int -> unit
    /// Converts hours to milliseconds.
    abstract hours: hours: int -> int
    /// Converts minutes to milliseconds.
    abstract minutes: minutes: int -> int
    /// Converts seconds to milliseconds.
    abstract seconds: seconds: int -> int

/// timer module
[<ImportAll("timer")>]
let timer: IExports = nativeOnly
