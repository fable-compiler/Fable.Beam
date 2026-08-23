module Fable.Beam.Tests.Proplists

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.Lists
open Fable.Beam.Maps
open Fable.Beam.Proplists

let tests =
    testList (
        "Proplists",
        [ test ("get_value returns value when key found", fun _ ->
                  // [{name, <<"alice">>}, {age, 30}]
                  let pl: BeamList<obj> = emitErlExpr () "[{name, <<\"alice\">>}, {age, 30}]"
                  let key = Erlang.binaryToAtom "name"
                  assertThat (proplists.get_value (key, pl)) (isEqualTo (Some "alice")))

          test ("get_value returns None when key missing", fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{name, <<\"alice\">>}]"
                  let key = Erlang.binaryToAtom "missing"
                  let result: string option = proplists.get_value (key, pl)
                  assertThat result (isEqualTo None))

          test ("get_value with default returns value when key found", fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{port, 443}]"
                  let portKey = Erlang.binaryToAtom "port"
                  let timeoutKey = Erlang.binaryToAtom "timeout"
                  assertThat (proplists.get_value (portKey, pl, 80)) (isEqualTo 443)
                  assertThat (proplists.get_value (timeoutKey, pl, 5000)) (isEqualTo 5000))

          test ("is_defined returns correct bool", fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{ssl, true}, {port, 443}]"
                  let sslKey = Erlang.binaryToAtom "ssl"
                  let missingKey = Erlang.binaryToAtom "missing"
                  assertThat (proplists.is_defined (sslKey, pl)) (isEqualTo true)
                  assertThat (proplists.is_defined (missingKey, pl)) (isEqualTo false))

          test ("delete removes all entries with key", fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{a, 1}, {b, 2}, {a, 3}]"
                  let aKey = Erlang.binaryToAtom "a"
                  let bKey = Erlang.binaryToAtom "b"
                  let result = proplists.delete (aKey, pl)
                  assertThat (proplists.is_defined (aKey, result)) (isEqualTo false)
                  assertThat (proplists.is_defined (bKey, result)) (isEqualTo true))

          test ("get_all_values returns all values for key", fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{x, 1}, {y, 2}, {x, 3}]"
                  let xKey = Erlang.binaryToAtom "x"
                  let vs: BeamList<int> = proplists.get_all_values (xKey, pl)
                  let expected: BeamList<int> = emitErlExpr () "[1, 3]"
                  assertThat vs (isEqualTo expected))

          test ("to_map converts proplist to map", fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{a, 1}, {b, 2}]"
                  let m: BeamMap<Atom, int> = proplists.to_map pl
                  let aKey = Erlang.binaryToAtom "a"
                  let bKey = Erlang.binaryToAtom "b"
                  assertThat (maps.get (aKey, m)) (isEqualTo 1)
                  assertThat (maps.get (bKey, m)) (isEqualTo 2))

          test ("unfold expands bare atoms to {Atom, true}", fun _ ->
                  // [ssl, {port, 443}] -> [{ssl, true}, {port, 443}]
                  let pl: BeamList<obj> = emitErlExpr () "[ssl, {port, 443}]"
                  let result = proplists.unfold pl
                  let expected: BeamList<obj> = emitErlExpr () "[{ssl, true}, {port, 443}]"
                  assertThat result (isEqualTo expected))

          test ("compact collapses {Atom, true} to bare atoms", fun _ ->
                  // [{ssl, true}, {port, 443}] -> [ssl, {port, 443}]
                  let pl: BeamList<obj> = emitErlExpr () "[{ssl, true}, {port, 443}]"
                  let result = proplists.compact pl
                  let expected: BeamList<obj> = emitErlExpr () "[ssl, {port, 443}]"
                  assertThat result (isEqualTo expected))

          test ("get_keys returns deduplicated keys", fun _ ->
                  // [{a, 1}, {b, 2}, {a, 3}] -> [a, b] (unordered, no duplicates)
                  let pl: BeamList<obj> = emitErlExpr () "[{a, 1}, {b, 2}, {a, 3}]"
                  let ks: Atom array = proplists.get_keys pl
                  assertThat (ks |> Array.length) (isEqualTo 2)
                  let aKey = Erlang.binaryToAtom "a"
                  let bKey = Erlang.binaryToAtom "b"
                  assertThat (ks |> Array.contains aKey) (isTrue)
                  assertThat (ks |> Array.contains bKey) (isTrue))

          test ("from_map converts map to proplist", fun _ ->
                  let m: BeamMap<Atom, int> = maps.new_ ()
                  let aKey = Erlang.binaryToAtom "a"
                  let bKey = Erlang.binaryToAtom "b"
                  let m = maps.put (aKey, 1, m)
                  let m = maps.put (bKey, 2, m)
                  let pl: BeamList<obj> = proplists.from_map m
                  assertThat (proplists.is_defined (aKey, pl)) (isEqualTo true)
                  assertThat (proplists.is_defined (bKey, pl)) (isEqualTo true)
                  let aVal: int option = proplists.get_value (aKey, pl)
                  assertThat aVal (isEqualTo (Some 1))) ]
    )
