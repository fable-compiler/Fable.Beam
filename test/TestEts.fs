module Fable.Beam.Tests.Ets

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam

module BEts = Fable.Beam.Ets

let tests =
    testList (
        "Ets",
        [ test (
              "ets create and delete",
              fun _ ->
                  let table =
                      BEts.create (Erlang.binaryToAtom "test_table") [ Erlang.binaryToAtom "set" ]

                  BEts.delete table
          )

          test (
              "ets insert and lookup",
              fun _ ->
                  let table =
                      BEts.create (Erlang.binaryToAtom "lookup_table") [ Erlang.binaryToAtom "set" ]

                  let tuple: obj = emitErlExpr () "{1, <<\"hello\">>}"
                  assertThat (BEts.insert table tuple) (isTrue)
                  let result = BEts.lookup table (box 1)
                  assertThat (Array.length result) (isEqualTo 1)
                  BEts.delete table
          )

          test (
              "ets tab2list",
              fun _ ->
                  let table =
                      BEts.create (Erlang.binaryToAtom "list_table") [ Erlang.binaryToAtom "set" ]

                  let t1: obj = emitErlExpr () "{1, <<\"a\">>}"
                  let t2: obj = emitErlExpr () "{2, <<\"b\">>}"
                  BEts.insert table t1 |> ignore
                  BEts.insert table t2 |> ignore
                  let all = BEts.toArray table
                  assertThat (Array.length all) (isEqualTo 2)
                  BEts.delete table
          )

          test (
              "ets typed info accessors",
              fun _ ->
                  let table =
                      BEts.create (Erlang.binaryToAtom "info_table") [ Erlang.binaryToAtom "set" ]

                  let tuple: obj = emitErlExpr () "{1, <<\"hello\">>}"
                  BEts.insert table tuple |> ignore

                  assertThat (BEts.size table) (isEqualTo 1)
                  assertThat (BEts.tableType table) (isEqualTo BEts.Set)
                  assertThat (BEts.access table) (isEqualTo BEts.Protected) // default access
                  assertThat (BEts.keypos table) (isEqualTo 1) // default keypos

                  BEts.delete table
          )

          test (
              "ets typed info with ordered_set CompiledName",
              fun _ ->
                  let table =
                      BEts.create (Erlang.binaryToAtom "ordered_table") [ Erlang.binaryToAtom "ordered_set" ]

                  assertThat (BEts.tableType table) (isEqualTo BEts.OrderedSet)
                  BEts.delete table
          ) ]
    )
