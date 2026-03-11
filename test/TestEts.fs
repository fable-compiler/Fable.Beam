module Fable.Beam.Tests.Ets

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Ets
open Fable.Beam.Erlang

[<Emit("erlang:length($0)")>]
let erlLength (xs: obj) : int = nativeOnly
#endif

[<Fact>]
let ``test ets create and delete`` () =
#if FABLE_COMPILER
    let table = ets.new_ (binaryToAtom "test_table", [ box (binaryToAtom "set") ])
    ets.delete table
#else
    ()
#endif

[<Fact>]
let ``test ets insert and lookup`` () =
#if FABLE_COMPILER
    let table = ets.new_ (binaryToAtom "lookup_table", [ box (binaryToAtom "set") ])
    let tuple: obj = emitErlExpr () "{1, <<\"hello\">>}"
    ets.insert (table, tuple) |> equal true
    let result = ets.lookup (table, box 1)
    erlLength (box result) |> equal 1
    ets.delete table
#else
    ()
#endif

[<Fact>]
let ``test ets tab2list`` () =
#if FABLE_COMPILER
    let table = ets.new_ (binaryToAtom "list_table", [ box (binaryToAtom "set") ])
    let t1: obj = emitErlExpr () "{1, <<\"a\">>}"
    let t2: obj = emitErlExpr () "{2, <<\"b\">>}"
    ets.insert (table, t1) |> ignore
    ets.insert (table, t2) |> ignore
    let all = ets.tab2list table
    erlLength (box all) |> equal 2
    ets.delete table
#else
    ()
#endif
