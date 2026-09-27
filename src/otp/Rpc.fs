/// Typed bindings for remote procedure calls between Erlang nodes.
/// See https://www.erlang.org/doc/apps/kernel/rpc
[<RequireQualifiedAccess>]
module Fable.Beam.Rpc

open Fable.Core
open Fable.Beam

[<Emit("case rpc:call($0, $1, $2, $3, $4) of {badrpc, RpcReason__} -> {error, RpcReason__}; RpcResult__ -> {ok, RpcResult__} end")>]
let private callRaw
    (node: Atom)
    (moduleName: Atom)
    (functionName: Atom)
    (arguments: Dynamic list)
    (timeoutMs: int)
    : Result<Dynamic, Dynamic> =
    nativeOnly

[<Emit("erlang:iolist_to_binary(io_lib:format(\"~p\", [$0]))")>]
let private formatTerm (value: Dynamic) : string = nativeOnly

[<Emit("case $0 of undefined -> {ok, undefined}; RpcWhereisPid__ when is_pid(RpcWhereisPid__) -> {ok, RpcWhereisPid__}; RpcWhereisOther__ -> {error, RpcWhereisOther__} end")>]
let private decodeWhereisRaw<'Msg> (value: Dynamic) : Result<Pid<'Msg> option, Dynamic> = nativeOnly

[<Emit("erlang:iolist_to_binary(io_lib:format(\"unexpected whereis result: ~p\", [$0]))")>]
let private formatUnexpectedWhereis (value: Dynamic) : string = nativeOnly

/// Evaluates a function on `node` with a bounded timeout.
///
/// The successful value is dynamic because the module and function are selected
/// at runtime. Decode it with the `Decode` combinators. RPC failure reasons are
/// formatted as Erlang terms so structured reasons are not restricted to atoms.
let call
    (node: Atom)
    (moduleName: Atom)
    (functionName: Atom)
    (arguments: Dynamic list)
    (timeoutMs: int)
    : Result<Dynamic, string> =
    callRaw node moduleName functionName arguments timeoutMs
    |> Result.mapError formatTerm

/// Resolves a registered process on another node with a bounded timeout.
/// Returns `Ok None` when the name is not registered and `Error` when the RPC
/// fails or returns a value other than a pid or `undefined`.
let whereis<'Msg> (node: Atom) (name: Atom) (timeoutMs: int) : Result<Pid<'Msg> option, string> =
    call node (Atom.ofString "erlang") (Atom.ofString "whereis") [ Dynamic.ofValue name ] timeoutMs
    |> Result.bind (fun value -> decodeWhereisRaw<'Msg> value |> Result.mapError formatUnexpectedWhereis)
