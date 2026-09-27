/// Typed bindings for Erlang distributed-node connections.
/// See https://www.erlang.org/doc/apps/kernel/net_kernel
[<RequireQualifiedAccess>]
module Fable.Beam.NetKernel

open Fable.Core
open Fable.Beam

/// Outcome from an explicit distributed-node connection attempt.
type ConnectNodeResult =
    | Connected
    | ConnectionFailed
    | NotAlive

/// Establishes a connection to a node.
///
/// `Connected` also covers an already-connected peer and the local node.
/// `NotAlive` means distribution has not been started on the local node.
[<Emit("(fun() -> case net_kernel:connect_node($0) of true -> connected; false -> connection_failed; ignored -> not_alive end end)()")>]
let connectNode (node: Atom) : ConnectNodeResult = nativeOnly
