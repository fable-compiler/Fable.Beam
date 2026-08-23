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
        [ test (
              "get_value returns value when key found",
              fun _ ->
                  // [{name, <<"alice">>}, {age, 30}]
                  let pl: BeamList<obj> = emitErlExpr () "[{name, <<\"alice\">>}, {age, 30}]"
                  let key = Erlang.binaryToAtom "name"
                  assertThat (tryFind key pl) (isEqualTo (Some "alice"))
          )

          test (
              "get_value returns None when key missing",
              fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{name, <<\"alice\">>}]"
                  let key = Erlang.binaryToAtom "missing"
                  let result: string option = tryFind key pl
                  assertThat result (isEqualTo None)
          )

          test (
              "get_value with default returns value when key found",
              fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{port, 443}]"
                  let portKey = Erlang.binaryToAtom "port"
                  let timeoutKey = Erlang.binaryToAtom "timeout"
                  assertThat (getOrDefault portKey pl 80) (isEqualTo 443)
                  assertThat (getOrDefault timeoutKey pl 5000) (isEqualTo 5000)
          )

          test (
              "is_defined returns correct bool",
              fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{ssl, true}, {port, 443}]"
                  let sslKey = Erlang.binaryToAtom "ssl"
                  let missingKey = Erlang.binaryToAtom "missing"
                  assertThat (containsKey sslKey pl) (isEqualTo true)
                  assertThat (containsKey missingKey pl) (isEqualTo false)
          )

          test (
              "delete removes all entries with key",
              fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{a, 1}, {b, 2}, {a, 3}]"
                  let aKey = Erlang.binaryToAtom "a"
                  let bKey = Erlang.binaryToAtom "b"
                  let result = remove aKey pl
                  assertThat (containsKey aKey result) (isEqualTo false)
                  assertThat (containsKey bKey result) (isEqualTo true)
          )

          test (
              "get_all_values returns all values for key",
              fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{x, 1}, {y, 2}, {x, 3}]"
                  let xKey = Erlang.binaryToAtom "x"
                  let vs: BeamList<int> = getAllValues xKey pl
                  let expected: BeamList<int> = emitErlExpr () "[1, 3]"
                  assertThat vs (isEqualTo expected)
          )

          test (
              "to_map converts proplist to map",
              fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{a, 1}, {b, 2}]"
                  let m: BeamMap<Atom, int> = toMap pl
                  let aKey = Erlang.binaryToAtom "a"
                  let bKey = Erlang.binaryToAtom "b"
                  assertThat (Maps.get aKey m) (isEqualTo 1)
                  assertThat (Maps.get bKey m) (isEqualTo 2)
          )

          test (
              "unfold expands bare atoms to {Atom, true}",
              fun _ ->
                  // [ssl, {port, 443}] -> [{ssl, true}, {port, 443}]
                  let pl: BeamList<obj> = emitErlExpr () "[ssl, {port, 443}]"
                  let result = unfold pl
                  let expected: BeamList<obj> = emitErlExpr () "[{ssl, true}, {port, 443}]"
                  assertThat result (isEqualTo expected)
          )

          test (
              "compact collapses {Atom, true} to bare atoms",
              fun _ ->
                  // [{ssl, true}, {port, 443}] -> [ssl, {port, 443}]
                  let pl: BeamList<obj> = emitErlExpr () "[{ssl, true}, {port, 443}]"
                  let result = compact pl
                  let expected: BeamList<obj> = emitErlExpr () "[ssl, {port, 443}]"
                  assertThat result (isEqualTo expected)
          )

          test (
              "get_keys returns deduplicated keys",
              fun _ ->
                  // [{a, 1}, {b, 2}, {a, 3}] -> [a, b] (unordered, no duplicates)
                  let pl: BeamList<obj> = emitErlExpr () "[{a, 1}, {b, 2}, {a, 3}]"
                  let ks: Atom array = keys pl
                  assertThat (ks |> Array.length) (isEqualTo 2)
                  let aKey = Erlang.binaryToAtom "a"
                  let bKey = Erlang.binaryToAtom "b"
                  assertThat (ks |> Array.contains aKey) (isTrue)
                  assertThat (ks |> Array.contains bKey) (isTrue)
          )

          test (
              "from_map converts map to proplist",
              fun _ ->
                  let m: BeamMap<Atom, int> = Maps.empty ()
                  let aKey = Erlang.binaryToAtom "a"
                  let bKey = Erlang.binaryToAtom "b"
                  let m = Maps.put aKey 1 m
                  let m = Maps.put bKey 2 m
                  let pl: BeamList<obj> = ofMap m
                  assertThat (containsKey aKey pl) (isEqualTo true)
                  assertThat (containsKey bKey pl) (isEqualTo true)
                  let aVal: int option = tryFind aKey pl
                  assertThat aVal (isEqualTo (Some 1))
          ) ]
    )
