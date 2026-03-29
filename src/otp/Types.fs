/// Shared opaque types for Erlang/OTP bindings.
namespace Fable.Beam

open Fable.Core

/// Erlang process identifier.
[<Erase>]
type Pid = Pid of obj

/// Erlang reference (from make_ref, monitor, etc.).
[<Erase>]
type Ref = Ref of obj

/// Erlang atom.
[<Erase>]
type Atom = Atom of obj

/// Erlang timer reference (from send_after, etc.).
[<Erase>]
type TimerRef = TimerRef of obj
