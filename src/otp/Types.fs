/// Shared opaque types for Erlang/OTP bindings.
namespace Fable.Beam

open Fable.Core

/// Erlang process identifier.
/// The phantom parameter 'Msg captures the type of messages this process accepts —
/// analogous to Gleam's Subject(message). Erased at runtime.
[<Erase>]
type Pid<'Msg> = Pid of obj

/// Erlang reference (from make_ref, monitor, etc.).
/// The phantom parameter 'Tag identifies what the ref refers to — e.g., a
/// monitor ref is Ref<Pid>, a request/response tag is Ref<Reply>. Erased at runtime.
[<Erase>]
type Ref<'Tag> = Ref of obj

/// Erlang atom.
[<Erase>]
type Atom = Atom of obj

/// Erlang time unit (`erlang:time_unit()`), used by `erlang:system_time`,
/// `os:system_time`, and `calendar:system_time_to_*`. Each case compiles to its
/// atom (`perf_counter` via `[<CompiledName>]` for the snake_case name).
/// Note: the rare `pos_integer()` parts-per-second form is not modelled here.
type TimeUnit =
    | Second
    | Millisecond
    | Microsecond
    | Nanosecond
    | Native
    | [<CompiledName("perf_counter")>] PerfCounter

/// Erlang timer reference (from send_after, etc.).
/// The phantom parameter 'Msg captures the message type that will be delivered
/// when the timer fires — analogous to Gleam's Subject(message). Erased at runtime.
[<Erase>]
type TimerRef<'Msg> = TimerRef of obj
