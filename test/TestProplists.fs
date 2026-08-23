module Fable.Beam.Tests.Proplists

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.Lists

module BMaps = Fable.Beam.Maps
module BProplists = Fable.Beam.Proplists

let tests =
    testList (
        "Proplists",
        [ test (
              "get_value returns value when key found",
              fun _ ->
                  // [{name, <<"alice">>}, {age, 30}]
                  let pl: BeamList<obj> = emitErlExpr () "[{name, <<\"alice\">>}, {age, 30}]"
                  let key = Erlang.binaryToAtom "name"
                  assertThat (BProplists.tryFind key pl) (isEqualTo (Some "alice"))
          )

          test (
              "get_value returns None when key missing",
              fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{name, <<\"alice\">>}]"
                  let key = Erlang.binaryToAtom "missing"
                  let result: string option = BProplists.tryFind key pl
                  assertThat result (isEqualTo None)
          )

          test (
              "get_value with default returns value when key found",
              fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{port, 443}]"
                  let portKey = Erlang.binaryToAtom "port"
                  let timeoutKey = Erlang.binaryToAtom "timeout"
                  assertThat (BProplists.getOrDefault portKey pl 80) (isEqualTo 443)
                  assertThat (BProplists.getOrDefault timeoutKey pl 5000) (isEqualTo 5000)
          )

          test (
              "is_defined returns correct bool",
              fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{ssl, true}, {port, 443}]"
                  let sslKey = Erlang.binaryToAtom "ssl"
                  let missingKey = Erlang.binaryToAtom "missing"
                  assertThat (BProplists.containsKey sslKey pl) (isEqualTo true)
                  assertThat (BProplists.containsKey missingKey pl) (isEqualTo false)
          )

          test (
              "delete removes all entries with key",
              fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{a, 1}, {b, 2}, {a, 3}]"
                  let aKey = Erlang.binaryToAtom "a"
                  let bKey = Erlang.binaryToAtom "b"
                  let result = BProplists.remove aKey pl
                  assertThat (BProplists.containsKey aKey result) (isEqualTo false)
                  assertThat (BProplists.containsKey bKey result) (isEqualTo true)
          )

          test (
              "get_all_values returns all values for key",
              fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{x, 1}, {y, 2}, {x, 3}]"
                  let xKey = Erlang.binaryToAtom "x"
                  let vs: BeamList<int> = BProplists.getAllValues xKey pl
                  let expected: BeamList<int> = emitErlExpr () "[1, 3]"
                  assertThat vs (isEqualTo expected)
          )

          test (
              "to_map converts proplist to map",
              fun _ ->
                  let pl: BeamList<obj> = emitErlExpr () "[{a, 1}, {b, 2}]"
                  let m: BMaps.BeamMap<Atom, int> = BProplists.toMap pl
                  let aKey = Erlang.binaryToAtom "a"
                  let bKey = Erlang.binaryToAtom "b"
                  assertThat (BMaps.get aKey m) (isEqualTo 1)
                  assertThat (BMaps.get bKey m) (isEqualTo 2)
          )

          test (
              "unfold expands bare atoms to {Atom, true}",
              fun _ ->
                  // [ssl, {port, 443}] -> [{ssl, true}, {port, 443}]
                  let pl: BeamList<obj> = emitErlExpr () "[ssl, {port, 443}]"
                  let result = BProplists.unfold pl
                  let expected: BeamList<obj> = emitErlExpr () "[{ssl, true}, {port, 443}]"
                  assertThat result (isEqualTo expected)
          )

          test (
              "compact collapses {Atom, true} to bare atoms",
              fun _ ->
                  // [{ssl, true}, {port, 443}] -> [ssl, {port, 443}]
                  let pl: BeamList<obj> = emitErlExpr () "[{ssl, true}, {port, 443}]"
                  let result = BProplists.compact pl
                  let expected: BeamList<obj> = emitErlExpr () "[ssl, {port, 443}]"
                  assertThat result (isEqualTo expected)
          )

          test (
              "get_keys returns deduplicated keys",
              fun _ ->
                  // [{a, 1}, {b, 2}, {a, 3}] -> [a, b] (unordered, no duplicates)
                  let pl: BeamList<obj> = emitErlExpr () "[{a, 1}, {b, 2}, {a, 3}]"
                  let ks: Atom array = BProplists.keys pl
                  assertThat (ks |> Array.length) (isEqualTo 2)
                  let aKey = Erlang.binaryToAtom "a"
                  let bKey = Erlang.binaryToAtom "b"
                  assertThat (ks |> Array.contains aKey) (isTrue)
                  assertThat (ks |> Array.contains bKey) (isTrue)
          )

          test (
              "from_map converts map to proplist",
              fun _ ->
                  let m: BMaps.BeamMap<Atom, int> = BMaps.empty ()
                  let aKey = Erlang.binaryToAtom "a"
                  let bKey = Erlang.binaryToAtom "b"
                  let m = BMaps.put aKey 1 m
                  let m = BMaps.put bKey 2 m
                  let pl: BeamList<obj> = BProplists.ofMap m
                  assertThat (BProplists.containsKey aKey pl) (isEqualTo true)
                  assertThat (BProplists.containsKey bKey pl) (isEqualTo true)
                  let aVal: int option = BProplists.tryFind aKey pl
                  assertThat aVal (isEqualTo (Some 1))
          ) ]
    )
