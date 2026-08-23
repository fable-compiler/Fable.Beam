module Fable.Beam.Tests.Ets

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Ets
open Fable.Beam

let tests =
    testList (
        "Ets",
        [ test ("ets create and delete", fun _ ->
                  let table =
                      ets.new_ (Erlang.binaryToAtom "test_table", [ Erlang.binaryToAtom "set" ])

                  ets.delete table
                  )

          test ("ets insert and lookup", fun _ ->
                  let table =
                      ets.new_ (Erlang.binaryToAtom "lookup_table", [ Erlang.binaryToAtom "set" ])

                  let tuple: obj = emitErlExpr () "{1, <<\"hello\">>}"
                  assertThat (ets.insert (table, tuple)) (isTrue)
                  let result = ets.lookup (table, box 1)
                  assertThat (Array.length result) (isEqualTo 1)
                  ets.delete table
                  )

          test ("ets tab2list", fun _ ->
                  let table =
                      ets.new_ (Erlang.binaryToAtom "list_table", [ Erlang.binaryToAtom "set" ])

                  let t1: obj = emitErlExpr () "{1, <<\"a\">>}"
                  let t2: obj = emitErlExpr () "{2, <<\"b\">>}"
                  ets.insert (table, t1) |> ignore
                  ets.insert (table, t2) |> ignore
                  let all = ets.tab2list table
                  assertThat (Array.length all) (isEqualTo 2)
                  ets.delete table
                  )

          test ("ets typed info accessors", fun _ ->
                  let table =
                      ets.new_ (Erlang.binaryToAtom "info_table", [ Erlang.binaryToAtom "set" ])

                  let tuple: obj = emitErlExpr () "{1, <<\"hello\">>}"
                  ets.insert (table, tuple) |> ignore

                  assertThat (size table) (isEqualTo 1)
                  assertThat (tableType table) (isEqualTo Set)
                  assertThat (access table) (isEqualTo Protected) // default access
                  assertThat (keypos table) (isEqualTo 1) // default keypos

                  ets.delete table
                  )

          test ("ets typed info with ordered_set CompiledName", fun _ ->
                  let table =
                      ets.new_ (Erlang.binaryToAtom "ordered_table", [ Erlang.binaryToAtom "ordered_set" ])

                  assertThat (tableType table) (isEqualTo OrderedSet)
                  ets.delete table
                  ) ]
    )
