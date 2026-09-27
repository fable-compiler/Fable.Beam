/// Typed bindings for remote procedure calls between Erlang nodes.
/// See https://www.erlang.org/doc/apps/kernel/rpc
[<RequireQualifiedAccess>]
module Fable.Beam.Rpc

open Fable.Core
open Fable.Beam

/// Evaluates a function on `node` with a bounded timeout.
///
/// The successful value is dynamic because the module and function are selected
/// at runtime. Decode it with the `Decode` combinators. RPC failure reasons are
/// formatted as Erlang terms so structured reasons are not restricted to atoms.
[<Emit("(fun() -> case rpc:call($0, $1, $2, $3, $4) of {badrpc, RpcReason__} -> {error, erlang:iolist_to_binary(io_lib:format(\"~p\", [RpcReason__]))}; RpcResult__ -> {ok, RpcResult__} end end)()")>]
let call
    (node: Atom)
    (moduleName: Atom)
    (functionName: Atom)
    (arguments: Dynamic list)
    (timeoutMs: int)
    : Result<Dynamic, string> =
    nativeOnly

/// Resolves a registered process on another node with a bounded timeout.
/// Returns `Ok None` when the name is not registered and `Error` when the RPC
/// fails or returns a value other than a pid or `undefined`.
[<Emit("(fun() -> case rpc:call($0, erlang, whereis, [$1], $2) of {badrpc, RpcWhereisReason__} -> {error, erlang:iolist_to_binary(io_lib:format(\"~p\", [RpcWhereisReason__]))}; undefined -> {ok, undefined}; RpcWhereisPid__ when is_pid(RpcWhereisPid__) -> {ok, RpcWhereisPid__}; RpcWhereisOther__ -> {error, erlang:iolist_to_binary(io_lib:format(\"unexpected whereis result: ~p\", [RpcWhereisOther__]))} end end)()")>]
let whereis<'Msg> (node: Atom) (name: Atom) (timeoutMs: int) : Result<Pid<'Msg> option, string> = nativeOnly
