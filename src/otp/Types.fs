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

/// Erlang `unicode:chardata()` — a binary, a charlist (a list of Unicode codepoints), or a nested
/// iolist of any of these. The OTP `string`/`uri_string` modules return chardata, and it is valid
/// anywhere a binary or iodata is accepted (`io:format`, `gen_tcp:send`, Cowboy response bodies),
/// so a value can be passed straight on without first flattening it to a binary. Erased at runtime.
///
/// Prefer this over the flattened `string` results when authoring BEAM output: keeping intermediate
/// results as chardata avoids repeatedly copying binaries, which is the idiomatic Erlang way to
/// build output and flatten just once at the I/O boundary.
[<Erase>]
type BeamChardata = BeamChardata of obj

/// Conversions between F# strings (Erlang binaries) and chardata.
[<RequireQualifiedAccess>]
module BeamChardata =
    /// An F# string is an Erlang binary, which is already valid chardata — zero cost.
    [<Emit("$0")>]
    let ofString (s: string) : BeamChardata = nativeOnly

    /// Flatten chardata into a single binary (F# string).
    [<Emit("unicode:characters_to_binary($0)")>]
    let toString (chardata: BeamChardata) : string = nativeOnly
