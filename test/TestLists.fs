module Fable.Beam.Tests.Lists

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Lists

[<Emit("erlang:length($0)")>]
let erlLength (xs: BeamList<'T>) : int = nativeOnly
#endif

[<Fact>]
let ``test lists.reverse works`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[1, 2, 3]"
    let expected: BeamList<int> = emitErlExpr () "[3, 2, 1]"
    lists.reverse xs |> equal expected
#else
    ()
#endif

[<Fact>]
let ``test lists.member works`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[1, 2, 3]"
    lists.``member`` (2, xs) |> equal true
    lists.``member`` (4, xs) |> equal false
#else
    ()
#endif

[<Fact>]
let ``test lists.sort works`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[3, 1, 2]"
    let expected: BeamList<int> = emitErlExpr () "[1, 2, 3]"
    lists.sort xs |> equal expected
#else
    ()
#endif

[<Fact>]
let ``test lists.append works`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[1, 2]"
    let ys: BeamList<int> = emitErlExpr () "[3, 4]"
    let expected: BeamList<int> = emitErlExpr () "[1, 2, 3, 4]"
    lists.append (xs, ys) |> equal expected
#else
    ()
#endif

[<Fact>]
let ``test lists.last works`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[1, 2, 3]"
    lists.last xs |> equal 3
#else
    ()
#endif

[<Fact>]
let ``test lists.nth works`` () =
#if FABLE_COMPILER
    // Erlang lists:nth is 1-based
    let xs: BeamList<int> = emitErlExpr () "[10, 20, 30]"
    lists.nth (1, xs) |> equal 10
#else
    ()
#endif

[<Fact>]
let ``test lists.flatten works`` () =
#if FABLE_COMPILER
    let xs: BeamList<BeamList<int>> = emitErlExpr () "[[1, 2], [3, 4]]"
    let expected: BeamList<int> = emitErlExpr () "[1, 2, 3, 4]"
    lists.flatten xs |> equal expected
#else
    ()
#endif

[<Fact>]
let ``test lists.usort removes duplicates`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[3, 1, 2, 1, 3]"
    let expected: BeamList<int> = emitErlExpr () "[1, 2, 3]"
    lists.usort xs |> equal expected
#else
    ()
#endif

[<Fact>]
let ``test lists.unzip returns tuple of two lists`` () =
#if FABLE_COMPILER
    let xs: BeamList<obj * obj> = emitErlExpr () "[{1, a}, {2, b}, {3, c}]"
    let (list1, list2) = lists.unzip xs
    erlLength list1 |> equal 3
    erlLength list2 |> equal 3
#else
    ()
#endif

[<Fact>]
let ``test lists.partition returns tuple of two lists`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[1, 2, 3, 4, 5]"
    let (matching, notMatching) = lists.partition (System.Func<_, _>(fun x -> x > 3), xs)
    erlLength matching |> equal 2
    erlLength notMatching |> equal 3
#else
    ()
#endif
