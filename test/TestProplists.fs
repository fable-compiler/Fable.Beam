module Fable.Beam.Tests.Proplists

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.Lists
open Fable.Beam.Maps
open Fable.Beam.Proplists
#endif

[<Fact>]
let ``test proplists.get_value returns value when key found`` () =
#if FABLE_COMPILER
    // [{name, <<"alice">>}, {age, 30}]
    let pl: BeamList<obj> = emitErlExpr () "[{name, <<\"alice\">>}, {age, 30}]"
    let key = Erlang.binaryToAtom "name"
    proplists.get_value (key, pl) |> equal (Some "alice")
#else
    ()
#endif

[<Fact>]
let ``test proplists.get_value returns None when key missing`` () =
#if FABLE_COMPILER
    let pl: BeamList<obj> = emitErlExpr () "[{name, <<\"alice\">>}]"
    let key = Erlang.binaryToAtom "missing"
    let result: string option = proplists.get_value (key, pl)
    result |> equal None
#else
    ()
#endif

[<Fact>]
let ``test proplists.get_value with default returns value when key found`` () =
#if FABLE_COMPILER
    let pl: BeamList<obj> = emitErlExpr () "[{port, 443}]"
    let portKey = Erlang.binaryToAtom "port"
    let timeoutKey = Erlang.binaryToAtom "timeout"
    proplists.get_value (portKey, pl, 80) |> equal 443
    proplists.get_value (timeoutKey, pl, 5000) |> equal 5000
#else
    ()
#endif

[<Fact>]
let ``test proplists.is_defined returns correct bool`` () =
#if FABLE_COMPILER
    let pl: BeamList<obj> = emitErlExpr () "[{ssl, true}, {port, 443}]"
    let sslKey = Erlang.binaryToAtom "ssl"
    let missingKey = Erlang.binaryToAtom "missing"
    proplists.is_defined (sslKey, pl) |> equal true
    proplists.is_defined (missingKey, pl) |> equal false
#else
    ()
#endif

[<Fact>]
let ``test proplists.delete removes all entries with key`` () =
#if FABLE_COMPILER
    let pl: BeamList<obj> = emitErlExpr () "[{a, 1}, {b, 2}, {a, 3}]"
    let aKey = Erlang.binaryToAtom "a"
    let bKey = Erlang.binaryToAtom "b"
    let result = proplists.delete (aKey, pl)
    proplists.is_defined (aKey, result) |> equal false
    proplists.is_defined (bKey, result) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test proplists.get_all_values returns all values for key`` () =
#if FABLE_COMPILER
    let pl: BeamList<obj> = emitErlExpr () "[{x, 1}, {y, 2}, {x, 3}]"
    let xKey = Erlang.binaryToAtom "x"
    let vs: BeamList<int> = proplists.get_all_values (xKey, pl)
    let expected: BeamList<int> = emitErlExpr () "[1, 3]"
    vs |> equal expected
#else
    ()
#endif

[<Fact>]
let ``test proplists.to_map converts proplist to map`` () =
#if FABLE_COMPILER
    let pl: BeamList<obj> = emitErlExpr () "[{a, 1}, {b, 2}]"
    let m: BeamMap<Atom, int> = proplists.to_map pl
    let aKey = Erlang.binaryToAtom "a"
    let bKey = Erlang.binaryToAtom "b"
    maps.get (aKey, m) |> equal 1
    maps.get (bKey, m) |> equal 2
#else
    ()
#endif

[<Fact>]
let ``test proplists.unfold expands bare atoms to {Atom, true}`` () =
#if FABLE_COMPILER
    // [ssl, {port, 443}] -> [{ssl, true}, {port, 443}]
    let pl: BeamList<obj> = emitErlExpr () "[ssl, {port, 443}]"
    let result = proplists.unfold pl
    let expected: BeamList<obj> = emitErlExpr () "[{ssl, true}, {port, 443}]"
    result |> equal expected
#else
    ()
#endif

[<Fact>]
let ``test proplists.compact collapses {Atom, true} to bare atoms`` () =
#if FABLE_COMPILER
    // [{ssl, true}, {port, 443}] -> [ssl, {port, 443}]
    let pl: BeamList<obj> = emitErlExpr () "[{ssl, true}, {port, 443}]"
    let result = proplists.compact pl
    let expected: BeamList<obj> = emitErlExpr () "[ssl, {port, 443}]"
    result |> equal expected
#else
    ()
#endif

[<Fact>]
let ``test proplists.get_keys returns deduplicated keys`` () =
#if FABLE_COMPILER
    // [{a, 1}, {b, 2}, {a, 3}] -> [a, b] (unordered, no duplicates)
    let pl: BeamList<obj> = emitErlExpr () "[{a, 1}, {b, 2}, {a, 3}]"
    let ks: Atom array = proplists.get_keys pl
    ks |> Array.length |> equal 2
    let aKey = Erlang.binaryToAtom "a"
    let bKey = Erlang.binaryToAtom "b"
    ks |> Array.contains aKey |> equal true
    ks |> Array.contains bKey |> equal true
#else
    ()
#endif

[<Fact>]
let ``test proplists.from_map converts map to proplist`` () =
#if FABLE_COMPILER
    let m: BeamMap<Atom, int> = maps.new_ ()
    let aKey = Erlang.binaryToAtom "a"
    let bKey = Erlang.binaryToAtom "b"
    let m = maps.put (aKey, 1, m)
    let m = maps.put (bKey, 2, m)
    let pl: BeamList<obj> = proplists.from_map m
    proplists.is_defined (aKey, pl) |> equal true
    proplists.is_defined (bKey, pl) |> equal true
    let aVal: int option = proplists.get_value (aKey, pl)
    aVal |> equal (Some 1)
#else
    ()
#endif
