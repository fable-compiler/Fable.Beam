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

    let (matching, notMatching) =
        lists.partition (System.Func<_, _>(fun x -> x > 3), xs)

    erlLength matching |> equal 2
    erlLength notMatching |> equal 3
#else
    ()
#endif

[<Fact>]
let ``test lists.sum returns sum`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[1, 2, 3, 4]"
    lists.sum xs |> equal 10
#else
    ()
#endif

[<Fact>]
let ``test lists.sum returns float sum`` () =
#if FABLE_COMPILER
    let xs: BeamList<float> = emitErlExpr () "[1.5, 2.5, 3.0]"
    lists.sum xs |> equal 7.0
#else
    ()
#endif

[<Fact>]
let ``test lists.max returns maximum`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[3, 1, 4, 1, 5, 9, 2]"
    lists.max xs |> equal 9
#else
    ()
#endif

[<Fact>]
let ``test lists.min returns minimum`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[3, 1, 4, 1, 5, 9, 2]"
    lists.min xs |> equal 1
#else
    ()
#endif

[<Fact>]
let ``test lists.seq generates integer sequence`` () =
#if FABLE_COMPILER
    let xs = lists.seq (1, 5)
    erlLength xs |> equal 5
    lists.nth (1, xs) |> equal 1
    lists.nth (5, xs) |> equal 5
#else
    ()
#endif

[<Fact>]
let ``test lists.seq with step generates sequence`` () =
#if FABLE_COMPILER
    let xs = lists.seq (0, 10, 2)
    erlLength xs |> equal 6
    lists.nth (1, xs) |> equal 0
    lists.nth (2, xs) |> equal 2
#else
    ()
#endif

[<Fact>]
let ``test lists.duplicate creates repeated list`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = lists.duplicate (3, 7)
    erlLength xs |> equal 3
    lists.nth (1, xs) |> equal 7
    lists.nth (3, xs) |> equal 7
#else
    ()
#endif

[<Fact>]
let ``test lists.takewhile takes while predicate holds`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[1, 2, 3, 4, 5]"
    let result = lists.takewhile (System.Func<_, _>(fun x -> x < 4), xs)
    erlLength result |> equal 3
    lists.nth (3, result) |> equal 3
#else
    ()
#endif

[<Fact>]
let ``test lists.dropwhile drops while predicate holds`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[1, 2, 3, 4, 5]"
    let result = lists.dropwhile (System.Func<_, _>(fun x -> x < 4), xs)
    erlLength result |> equal 2
    lists.nth (1, result) |> equal 4
#else
    ()
#endif

[<Fact>]
let ``test lists.splitwith splits at predicate boundary`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[1, 2, 3, 4, 5]"
    let (before, after) = lists.splitwith (System.Func<_, _>(fun x -> x < 3), xs)
    erlLength before |> equal 2
    erlLength after |> equal 3
#else
    ()
#endif

[<Fact>]
let ``test lists.delete removes first occurrence`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[1, 2, 3, 2, 1]"
    let result = lists.delete (2, xs)
    erlLength result |> equal 4
    lists.``member`` (2, result) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test lists.subtract removes elements`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[1, 2, 3, 4, 5]"
    let ys: BeamList<int> = emitErlExpr () "[2, 4]"
    let result = lists.subtract (xs, ys)
    erlLength result |> equal 3
    lists.``member`` (2, result) |> equal false
#else
    ()
#endif

[<Fact>]
let ``test lists.keysort sorts by Nth element`` () =
#if FABLE_COMPILER
    let xs: BeamList<int * string> = emitErlExpr () "[{3, c}, {1, a}, {2, b}]"
    let sorted = lists.keysort (1, xs)
    let first: int * string = lists.nth (1, sorted)
    fst first |> equal 1
#else
    ()
#endif

[<Fact>]
let ``test lists.keydelete removes first matching tuple`` () =
#if FABLE_COMPILER
    let xs: BeamList<string * int> =
        emitErlExpr () "[{<<\"a\">>, 1}, {<<\"b\">>, 2}, {<<\"a\">>, 3}]"

    let result = lists.keydelete ("a", 1, xs)
    erlLength result |> equal 2
#else
    ()
#endif

[<Fact>]
let ``test lists.keymember checks for key`` () =
#if FABLE_COMPILER
    let xs: BeamList<string * int> = emitErlExpr () "[{<<\"a\">>, 1}, {<<\"b\">>, 2}]"
    lists.keymember ("a", 1, xs) |> equal true
    lists.keymember ("c", 1, xs) |> equal false
#else
    ()
#endif

[<Fact>]
let ``test keyFind returns Some for existing key`` () =
#if FABLE_COMPILER
    let xs: BeamList<string * int> =
        emitErlExpr () "[{<<\"a\">>, 1}, {<<\"b\">>, 2}, {<<\"c\">>, 3}]"

    keyFind "b" 1 xs |> equal (Some("b", 2))
#else
    ()
#endif

[<Fact>]
let ``test keyFind returns None for missing key`` () =
#if FABLE_COMPILER
    let xs: BeamList<string * int> = emitErlExpr () "[{<<\"a\">>, 1}, {<<\"b\">>, 2}]"
    keyFind "z" 1 xs |> equal None
#else
    ()
#endif

[<Fact>]
let ``test lists.keyreplace replaces first matching tuple`` () =
#if FABLE_COMPILER
    let xs: BeamList<string * int> =
        emitErlExpr () "[{<<\"a\">>, 1}, {<<\"b\">>, 2}, {<<\"a\">>, 3}]"

    let result = lists.keyreplace ("a", 1, xs, ("a", 99))
    lists.nth (1, result) |> equal ("a", 99)
    erlLength result |> equal 3
#else
    ()
#endif

[<Fact>]
let ``test lists.mapfoldl maps and folds left`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[1, 2, 3]"

    let (mapped, acc) =
        lists.mapfoldl (System.Func<_, _, _>(fun x s -> (x * 2, s + x)), 0, xs)

    mapped |> equal (emitErlExpr () "[2, 4, 6]")
    acc |> equal 6
#else
    ()
#endif

[<Fact>]
let ``test lists.mapfoldr maps and folds right`` () =
#if FABLE_COMPILER
    let xs: BeamList<int> = emitErlExpr () "[1, 2, 3]"

    let (mapped, acc) =
        lists.mapfoldr (System.Func<_, _, _>(fun x s -> (x * 2, s + x)), 0, xs)

    mapped |> equal (emitErlExpr () "[2, 4, 6]")
    acc |> equal 6
#else
    ()
#endif
