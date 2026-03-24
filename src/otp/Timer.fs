/// Type bindings for OTP timer module
/// See https://www.erlang.org/doc/apps/stdlib/timer
module Fable.Beam.Timer

open Fable.Core
open Fable.Beam.Erlang

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Sends Msg to Dest after Time milliseconds. Returns {ok, TRef} | {error, Reason}.
    abstract send_after: time: int * dest: Pid * msg: obj -> obj
    /// Sends Msg to Dest repeatedly every Time milliseconds. Returns {ok, TRef} | {error, Reason}.
    abstract send_interval: time: int * dest: Pid * msg: obj -> obj
    /// Evaluates Fun after Time milliseconds. Returns {ok, TRef} | {error, Reason}.
    abstract apply_after: time: int * ``module``: Atom * ``function``: Atom * args: obj list -> obj
    /// Evaluates Fun repeatedly every Time milliseconds. Returns {ok, TRef} | {error, Reason}.
    abstract apply_interval: time: int * ``module``: Atom * ``function``: Atom * args: obj list -> obj
    /// Cancels a previously started timer. Returns {ok, cancel} | {error, Reason}.
    abstract cancel: timerRef: TimerRef -> obj
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

/// Suspends the process for the given number of milliseconds.
[<Emit("timer:sleep($0)")>]
let sleep (ms: int) : unit = nativeOnly
