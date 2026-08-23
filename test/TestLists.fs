module Fable.Beam.Tests.Lists

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Lists

module BLists = Fable.Beam.Lists

[<Emit("erlang:length($0)")>]
let erlLength (xs: BeamList<'T>) : int = nativeOnly

let tests =
    testList (
        "Lists",
        [ test (
              "reverse works",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3]"
                  let expected: BeamList<int> = emitErlExpr () "[3, 2, 1]"
                  assertThat (BLists.reverse xs) (isEqualTo expected)
          )

          test (
              "member works",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3]"
                  assertThat (BLists.``member`` 2 xs) (isTrue)
                  assertThat (BLists.``member`` 4 xs) (isFalse)
          )

          test (
              "sort works",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[3, 1, 2]"
                  let expected: BeamList<int> = emitErlExpr () "[1, 2, 3]"
                  assertThat (BLists.sort xs) (isEqualTo expected)
          )

          test (
              "append works",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2]"
                  let ys: BeamList<int> = emitErlExpr () "[3, 4]"
                  let expected: BeamList<int> = emitErlExpr () "[1, 2, 3, 4]"
                  assertThat (BLists.append xs ys) (isEqualTo expected)
          )

          test (
              "last works",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3]"
                  assertThat (BLists.last xs) (isEqualTo 3)
          )

          test (
              "nth works",
              fun _ ->
                  // Erlang lists:nth is 1-based
                  let xs: BeamList<int> = emitErlExpr () "[10, 20, 30]"
                  assertThat (BLists.nth 1 xs) (isEqualTo 10)
          )

          test (
              "flatten works",
              fun _ ->
                  let xs: BeamList<BeamList<int>> = emitErlExpr () "[[1, 2], [3, 4]]"
                  let expected: BeamList<int> = emitErlExpr () "[1, 2, 3, 4]"
                  assertThat (BLists.flatten xs) (isEqualTo expected)
          )

          test (
              "usort removes duplicates",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[3, 1, 2, 1, 3]"
                  let expected: BeamList<int> = emitErlExpr () "[1, 2, 3]"
                  assertThat (BLists.usort xs) (isEqualTo expected)
          )

          test (
              "unzip returns tuple of two lists",
              fun _ ->
                  let xs: BeamList<obj * obj> = emitErlExpr () "[{1, a}, {2, b}, {3, c}]"
                  let (list1, list2) = BLists.unzip xs
                  assertThat (erlLength list1) (isEqualTo 3)
                  assertThat (erlLength list2) (isEqualTo 3)
          )

          test (
              "partition returns tuple of two lists",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3, 4, 5]"

                  let (matching, notMatching) = BLists.partition (fun x -> x > 3) xs

                  assertThat (erlLength matching) (isEqualTo 2)
                  assertThat (erlLength notMatching) (isEqualTo 3)
          )

          test (
              "sum returns sum",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3, 4]"
                  assertThat (BLists.sumInt xs) (isEqualTo 10)
          )

          test (
              "sum returns float sum",
              fun _ ->
                  let xs: BeamList<float> = emitErlExpr () "[1.5, 2.5, 3.0]"
                  assertThat (BLists.sumFloat xs) (isEqualTo 7.0)
          )

          test (
              "max returns maximum",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[3, 1, 4, 1, 5, 9, 2]"
                  assertThat (BLists.max xs) (isEqualTo 9)
          )

          test (
              "min returns minimum",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[3, 1, 4, 1, 5, 9, 2]"
                  assertThat (BLists.min xs) (isEqualTo 1)
          )

          test (
              "seq generates integer sequence",
              fun _ ->
                  let xs = BLists.seq 1 5
                  assertThat (erlLength xs) (isEqualTo 5)
                  assertThat (BLists.nth 1 xs) (isEqualTo 1)
                  assertThat (BLists.nth 5 xs) (isEqualTo 5)
          )

          test (
              "seq with step generates sequence",
              fun _ ->
                  let xs = BLists.seqStep 0 10 2
                  assertThat (erlLength xs) (isEqualTo 6)
                  assertThat (BLists.nth 1 xs) (isEqualTo 0)
                  assertThat (BLists.nth 2 xs) (isEqualTo 2)
          )

          test (
              "duplicate creates repeated list",
              fun _ ->
                  let xs: BeamList<int> = BLists.duplicate 3 7
                  assertThat (erlLength xs) (isEqualTo 3)
                  assertThat (BLists.nth 1 xs) (isEqualTo 7)
                  assertThat (BLists.nth 3 xs) (isEqualTo 7)
          )

          test (
              "takewhile takes while predicate holds",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3, 4, 5]"
                  let result = BLists.takeWhile (fun x -> x < 4) xs
                  assertThat (erlLength result) (isEqualTo 3)
                  assertThat (BLists.nth 3 result) (isEqualTo 3)
          )

          test (
              "dropwhile drops while predicate holds",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3, 4, 5]"
                  let result = BLists.dropWhile (fun x -> x < 4) xs
                  assertThat (erlLength result) (isEqualTo 2)
                  assertThat (BLists.nth 1 result) (isEqualTo 4)
          )

          test (
              "splitwith splits at predicate boundary",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3, 4, 5]"
                  let (before, after) = BLists.splitWith (fun x -> x < 3) xs
                  assertThat (erlLength before) (isEqualTo 2)
                  assertThat (erlLength after) (isEqualTo 3)
          )

          test (
              "delete removes first occurrence",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3, 2, 1]"
                  let result = BLists.delete 2 xs
                  assertThat (erlLength result) (isEqualTo 4)
                  assertThat (BLists.``member`` 2 result) (isTrue)
          )

          test (
              "subtract removes elements",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3, 4, 5]"
                  let ys: BeamList<int> = emitErlExpr () "[2, 4]"
                  let result = BLists.subtract xs ys
                  assertThat (erlLength result) (isEqualTo 3)
                  assertThat (BLists.``member`` 2 result) (isFalse)
          )

          test (
              "keysort sorts by Nth element",
              fun _ ->
                  let xs: BeamList<int * string> = emitErlExpr () "[{3, c}, {1, a}, {2, b}]"
                  let sorted = BLists.keySort 1 xs
                  let first: int * string = BLists.nth 1 sorted
                  assertThat (fst first) (isEqualTo 1)
          )

          test (
              "keydelete removes first matching tuple",
              fun _ ->
                  let xs: BeamList<string * int> =
                      emitErlExpr () "[{<<\"a\">>, 1}, {<<\"b\">>, 2}, {<<\"a\">>, 3}]"

                  let result = BLists.keyDelete "a" 1 xs
                  assertThat (erlLength result) (isEqualTo 2)
          )

          test (
              "keymember checks for key",
              fun _ ->
                  let xs: BeamList<string * int> = emitErlExpr () "[{<<\"a\">>, 1}, {<<\"b\">>, 2}]"
                  assertThat (BLists.keyMember "a" 1 xs) (isTrue)
                  assertThat (BLists.keyMember "c" 1 xs) (isFalse)
          )

          test (
              "keyFind returns Some for existing key",
              fun _ ->
                  let xs: BeamList<string * int> =
                      emitErlExpr () "[{<<\"a\">>, 1}, {<<\"b\">>, 2}, {<<\"c\">>, 3}]"

                  let found = BLists.keyFind "b" 1 xs
                  assertThat found (isEqualTo (Some("b", 2)))
          )

          test (
              "keyFind returns None for missing key",
              fun _ ->
                  let xs: BeamList<string * int> = emitErlExpr () "[{<<\"a\">>, 1}, {<<\"b\">>, 2}]"
                  assertThat (BLists.keyFind "z" 1 xs) (isEqualTo None)
          )

          test (
              "keyreplace replaces first matching tuple",
              fun _ ->
                  let xs: BeamList<string * int> =
                      emitErlExpr () "[{<<\"a\">>, 1}, {<<\"b\">>, 2}, {<<\"a\">>, 3}]"

                  let result = BLists.keyReplace "a" 1 xs ("a", 99)
                  assertThat (BLists.nth 1 result) (isEqualTo ("a", 99))
                  assertThat (erlLength result) (isEqualTo 3)
          )

          test (
              "mapfoldl maps and folds left",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3]"

                  let (mapped, acc) = BLists.mapFoldLeft (fun x s -> (x * 2, s + x)) 0 xs

                  assertThat mapped (isEqualTo (emitErlExpr () "[2, 4, 6]"))
                  assertThat acc (isEqualTo 6)
          )

          test (
              "mapfoldr maps and folds right",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3]"

                  let (mapped, acc) = BLists.mapFoldRight (fun x s -> (x * 2, s + x)) 0 xs

                  assertThat mapped (isEqualTo (emitErlExpr () "[2, 4, 6]"))
                  assertThat acc (isEqualTo 6)
          )

          test (
              "map applies a function to each element",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3]"
                  assertThat (BLists.map (fun x -> x * 2) xs) (isEqualTo (emitErlExpr () "[2, 4, 6]"))
          )

          test (
              "filter keeps matching elements",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3, 4]"
                  assertThat (BLists.filter (fun x -> x % 2 = 0) xs) (isEqualTo (emitErlExpr () "[2, 4]"))
          )

          test (
              "foldl folds from the left",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3]"
                  assertThat (BLists.foldl (fun x acc -> acc + x) 0 xs) (isEqualTo 6)
          )

          test (
              "foldr folds from the right",
              fun _ ->
                  // Subtraction is not associative, so this pins the direction: 1-(2-(3-0)) = 2.
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3]"
                  assertThat (BLists.foldr (fun x acc -> x - acc) 0 xs) (isEqualTo 2)
          )

          test (
              "all and any check predicates",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[2, 4, 6]"
                  assertThat (BLists.all (fun x -> x % 2 = 0) xs) (isTrue)
                  assertThat (BLists.any (fun x -> x > 5) xs) (isTrue)
                  assertThat (BLists.any (fun x -> x > 10) xs) (isFalse)
          )

          test (
              "sort with a comparison function",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[3, 1, 2]"
                  // Descending: the comparator returns true when A should come before B.
                  assertThat (BLists.sortWith (fun a b -> a >= b) xs) (isEqualTo (emitErlExpr () "[3, 2, 1]"))
          ) ]
    )
