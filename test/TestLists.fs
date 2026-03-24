module Fable.Beam.Tests.Lists

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Lists

[<Emit("erlang:length($0)")>]
let erlLength (xs: obj) : int = nativeOnly
#endif

[<Fact>]
let ``test lists.reverse works`` () =
#if FABLE_COMPILER
    let xs: obj = emitErlExpr () "[1, 2, 3]"
    let expected: obj = emitErlExpr () "[3, 2, 1]"
    lists.reverse xs |> equal expected
#else
    ()
#endif

[<Fact>]
let ``test lists.member works`` () =
#if FABLE_COMPILER
    let xs: obj = emitErlExpr () "[1, 2, 3]"
    lists.``member`` (box 2, xs) |> equal true
    lists.``member`` (box 4, xs) |> equal false
#else
    ()
#endif

[<Fact>]
let ``test lists.sort works`` () =
#if FABLE_COMPILER
    let xs: obj = emitErlExpr () "[3, 1, 2]"
    let expected: obj = emitErlExpr () "[1, 2, 3]"
    lists.sort xs |> equal expected
#else
    ()
#endif

[<Fact>]
let ``test lists.append works`` () =
#if FABLE_COMPILER
    let xs: obj = emitErlExpr () "[1, 2]"
    let ys: obj = emitErlExpr () "[3, 4]"
    let expected: obj = emitErlExpr () "[1, 2, 3, 4]"
    lists.append (xs, ys) |> equal expected
#else
    ()
#endif

[<Fact>]
let ``test lists.last works`` () =
#if FABLE_COMPILER
    let xs: obj = emitErlExpr () "[1, 2, 3]"
    lists.last xs |> equal (box 3)
#else
    ()
#endif

[<Fact>]
let ``test lists.nth works`` () =
#if FABLE_COMPILER
    // Erlang lists:nth is 1-based
    let xs: obj = emitErlExpr () "[10, 20, 30]"
    lists.nth (1, xs) |> equal (box 10)
#else
    ()
#endif

[<Fact>]
let ``test lists.flatten works`` () =
#if FABLE_COMPILER
    let xs: obj = emitErlExpr () "[[1, 2], [3, 4]]"
    let expected: obj = emitErlExpr () "[1, 2, 3, 4]"
    lists.flatten xs |> equal expected
#else
    ()
#endif

[<Fact>]
let ``test lists.usort removes duplicates`` () =
#if FABLE_COMPILER
    let xs: obj = emitErlExpr () "[3, 1, 2, 1, 3]"
    let expected: obj = emitErlExpr () "[1, 2, 3]"
    lists.usort xs |> equal expected
#else
    ()
#endif

[<Fact>]
let ``test lists.unzip returns tuple of two lists`` () =
#if FABLE_COMPILER
    let xs: obj = emitErlExpr () "[{1, a}, {2, b}, {3, c}]"
    let (list1, list2) = lists.unzip xs
    erlLength list1 |> equal 3
    erlLength list2 |> equal 3
#else
    ()
#endif

[<Fact>]
let ``test lists.partition returns tuple of two lists`` () =
#if FABLE_COMPILER
    let xs: obj = emitErlExpr () "[1, 2, 3, 4, 5]"
    let pred: obj = emitErlExpr () "fun(X) -> X > 3 end"
    let (matching, notMatching) = lists.partition (pred, xs)
    erlLength matching |> equal 2
    erlLength notMatching |> equal 3
#else
    ()
#endif
