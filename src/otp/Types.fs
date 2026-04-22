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

/// Erlang timer reference (from send_after, etc.).
/// The phantom parameter 'Msg captures the message type that will be delivered
/// when the timer fires — analogous to Gleam's Subject(message). Erased at runtime.
[<Erase>]
type TimerRef<'Msg> = TimerRef of obj
