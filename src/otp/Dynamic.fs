/// A dynamically-typed Erlang term with composable decoders for extracting
/// typed values. Use this instead of bare `obj` when the runtime shape is
/// unknown and must be validated.
///
/// Inspired by Gleam's `gleam/dynamic/decode` module.
namespace Fable.Beam

open Fable.Core

/// An Erlang term of unknown static type. Construct from untyped Erlang
/// responses and narrow with the combinators in the `Decode` module.
[<Erase>]
type Dynamic = Dynamic of obj

/// Decoder combinators for extracting typed values from a `Dynamic`.
/// Each decoder returns `Result<'T, string>` where the error is a human-readable message.
///
/// WORKAROUND: Emit expressions wrapped in `(fun() -> ... end)()` to prevent
/// Erlang "unsafe variable" errors. Remove IIFEs once fixed in Fable.
module Decode =

    /// Decode a Dynamic as an integer.
    [<Emit("(fun() -> case erlang:is_integer($0) of true -> {ok, $0}; false -> {error, <<\"expected integer\">>} end end)()")>]
    let int (d: Dynamic) : Result<int, string> = nativeOnly

    /// Decode a Dynamic as a float.
    [<Emit("(fun() -> case erlang:is_float($0) of true -> {ok, $0}; false -> {error, <<\"expected float\">>} end end)()")>]
    let float (d: Dynamic) : Result<float, string> = nativeOnly

    /// Decode a Dynamic as a boolean (atoms `true` or `false`).
    [<Emit("(fun() -> case erlang:is_boolean($0) of true -> {ok, $0}; false -> {error, <<\"expected boolean\">>} end end)()")>]
    let bool (d: Dynamic) : Result<bool, string> = nativeOnly

    /// Decode a Dynamic as an atom.
    [<Emit("(fun() -> case erlang:is_atom($0) of true -> {ok, $0}; false -> {error, <<\"expected atom\">>} end end)()")>]
    let atom (d: Dynamic) : Result<Atom, string> = nativeOnly

    /// Decode a Dynamic as a binary string.
    [<Emit("(fun() -> case erlang:is_binary($0) of true -> {ok, $0}; false -> {error, <<\"expected binary string\">>} end end)()")>]
    let string (d: Dynamic) : Result<string, string> = nativeOnly

    /// Return the Dynamic unchanged. Useful as a terminal decoder in `field`
    /// chains when you want to hand off further decoding to the caller.
    [<Emit("{ok, $0}")>]
    let dynamic (d: Dynamic) : Result<Dynamic, string> = nativeOnly

    /// Extract a field from an Erlang map and decode it with the given decoder.
    /// Returns Error if the map doesn't contain the key or the inner decoder fails.
    /// Uses System.Func for the decoder — matches the Lists.fs convention for
    /// callbacks and avoids curried-arg substitution issues with Emit.
    [<Emit("(fun() -> case maps:find($0, $2) of {ok, FieldVal__} -> $1(FieldVal__); error -> {error, erlang:list_to_binary(io_lib:format(<<\"missing field ~p\">>, [$0]))} end end)()")>]
    let field (key: Atom) (decoder: System.Func<Dynamic, Result<'V, string>>) (d: Dynamic) : Result<'V, string> =
        nativeOnly

    /// Decode a Dynamic as a list of values, decoding each element with `decoder`.
    /// Short-circuits on the first decode error.
    [<Emit("(fun() -> case erlang:is_list($1) of true -> DecodeListFold__ = fun (_, {error, _} = E__) -> E__; (Elem__, {ok, Acc__}) -> case $0(Elem__) of {ok, V__} -> {ok, [V__ | Acc__]}; {error, _} = E__ -> E__ end end, case lists:foldl(DecodeListFold__, {ok, []}, $1) of {ok, Rev__} -> {ok, fable_utils:new_ref(lists:reverse(Rev__))}; {error, _} = E__ -> E__ end; false -> {error, <<\"expected list\">>} end end)()")>]
    let list (decoder: System.Func<Dynamic, Result<'V, string>>) (d: Dynamic) : Result<'V array, string> = nativeOnly

    /// Decode a Dynamic that may be the atom `undefined` (mapped to None) or
    /// a value decoded by the inner decoder (mapped to Some).
    [<Emit("(fun() -> case $1 of undefined -> {ok, undefined}; V__ -> case $0(V__) of {ok, V2__} -> {ok, V2__}; {error, _} = E__ -> E__ end end end)()")>]
    let optional (decoder: System.Func<Dynamic, Result<'V, string>>) (d: Dynamic) : Result<'V option, string> =
        nativeOnly

    /// Decode a 2-tuple by applying each decoder to the corresponding element.
    [<Emit("(fun() -> case erlang:is_tuple($2) andalso erlang:tuple_size($2) =:= 2 of true -> case $0(erlang:element(1, $2)) of {ok, A__} -> case $1(erlang:element(2, $2)) of {ok, B__} -> {ok, {A__, B__}}; {error, _} = E__ -> E__ end; {error, _} = E__ -> E__ end; false -> {error, <<\"expected 2-tuple\">>} end end)()")>]
    let tuple2
        (decA: System.Func<Dynamic, Result<'A, string>>)
        (decB: System.Func<Dynamic, Result<'B, string>>)
        (d: Dynamic)
        : Result<'A * 'B, string> =
        nativeOnly

    /// Lift a plain value into the Result monad. Useful for building records
    /// after extracting all fields successfully.
    let inline succeed (value: 'V) : Result<'V, string> = Ok value

    /// Map the successful value of a decode result through a function.
    let inline map (f: 'A -> 'B) (r: Result<'A, string>) : Result<'B, string> = Result.map f r

    /// Chain decode results: run the first, and if it succeeds, run the second
    /// with its value.
    let inline andThen (f: 'A -> Result<'B, string>) (r: Result<'A, string>) : Result<'B, string> = Result.bind f r
