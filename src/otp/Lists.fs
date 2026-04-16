/// Type bindings for Erlang lists module
/// See https://www.erlang.org/doc/apps/stdlib/lists
module Fable.Beam.Lists

open Fable.Core

// fsharplint:disable MemberNames

/// Erlang list with typed elements.
[<Erase>]
type BeamList<'T> = BeamList of obj

[<Erase>]
type IExports =
    /// Appends two lists.
    abstract append: list1: BeamList<'T> * list2: BeamList<'T> -> BeamList<'T>
    /// Flattens a list of lists.
    abstract flatten: deepList: BeamList<BeamList<'T>> -> BeamList<'T>
    /// Returns the length of a flat list. Note: prefer erlang:length/1 for BIF performance.
    abstract flatlength: list: BeamList<'T> -> int
    /// Returns true if Elem is in the list.
    abstract ``member``: elem: 'T * list: BeamList<'T> -> bool
    /// Reverses a list.
    abstract reverse: list: BeamList<'T> -> BeamList<'T>
    /// Sorts a list.
    abstract sort: list: BeamList<'T> -> BeamList<'T>
    /// Sorts a list using a comparison function.
    abstract sort: f: System.Func<'T, 'T, bool> * list: BeamList<'T> -> BeamList<'T>
    /// Returns the Nth element (1-based).
    abstract nth: n: int * list: BeamList<'T> -> 'T
    /// Returns the last element.
    abstract last: list: BeamList<'T> -> 'T
    /// Applies a function to each element (map).
    abstract map: f: System.Func<'T, 'U> * list: BeamList<'T> -> BeamList<'U>
    /// Filters elements by a predicate.
    abstract filter: pred: System.Func<'T, bool> * list: BeamList<'T> -> BeamList<'T>
    /// Left fold over a list.
    abstract foldl: f: System.Func<'T, 'Acc, 'Acc> * acc: 'Acc * list: BeamList<'T> -> 'Acc
    /// Right fold over a list.
    abstract foldr: f: System.Func<'T, 'Acc, 'Acc> * acc: 'Acc * list: BeamList<'T> -> 'Acc
    /// Applies a function to each element for side effects.
    abstract foreach: f: System.Action<'T> * list: BeamList<'T> -> unit
    /// Zips two lists into a list of tuples.
    abstract zip: list1: BeamList<'A> * list2: BeamList<'B> -> BeamList<'A * 'B>
    /// Unzips a list of tuples into a tuple of two lists.
    abstract unzip: list: BeamList<'A * 'B> -> BeamList<'A> * BeamList<'B>
    /// Returns a tuple of {Satisfying, NotSatisfying} elements.
    abstract partition: pred: System.Func<'T, bool> * list: BeamList<'T> -> BeamList<'T> * BeamList<'T>
    /// Removes duplicate elements.
    abstract usort: list: BeamList<'T> -> BeamList<'T>
    /// Returns a sublist (first N elements).
    abstract sublist: list: BeamList<'T> * len: int -> BeamList<'T>
    /// Returns true if all elements satisfy the predicate.
    abstract all: pred: System.Func<'T, bool> * list: BeamList<'T> -> bool
    /// Returns true if any element satisfies the predicate.
    abstract any: pred: System.Func<'T, bool> * list: BeamList<'T> -> bool

    /// Returns the sum of all numbers in the list.
    abstract sum: list: BeamList<int> -> int
    /// Returns the sum of all numbers in the list.
    abstract sum: list: BeamList<float> -> float
    /// Returns the maximum element of a non-empty list.
    abstract max: list: BeamList<'T> -> 'T
    /// Returns the minimum element of a non-empty list.
    abstract min: list: BeamList<'T> -> 'T

    /// Generates a sequence of integers from From to To (inclusive).
    abstract seq: from: int * ``to``: int -> BeamList<int>
    /// Generates a sequence of integers from From to To with the given increment.
    abstract seq: from: int * ``to``: int * incr: int -> BeamList<int>
    /// Returns a list containing N copies of Elem.
    abstract duplicate: n: int * elem: 'T -> BeamList<'T>

    /// Returns elements from the front of the list as long as Pred returns true.
    abstract takewhile: pred: System.Func<'T, bool> * list: BeamList<'T> -> BeamList<'T>
    /// Drops elements from the front of the list while Pred returns true.
    abstract dropwhile: pred: System.Func<'T, bool> * list: BeamList<'T> -> BeamList<'T>
    /// Splits a list into {TakeWhile, Rest} at the first element for which Pred returns false.
    abstract splitwith: pred: System.Func<'T, bool> * list: BeamList<'T> -> BeamList<'T> * BeamList<'T>

    /// Deletes the first occurrence of Elem from the list.
    abstract delete: elem: 'T * list: BeamList<'T> -> BeamList<'T>
    /// Subtracts list2 from list1 by removing the first matching occurrence of each element in list2.
    abstract subtract: list1: BeamList<'T> * list2: BeamList<'T> -> BeamList<'T>

    /// Sorts a list of tuples by the Nth element (1-based).
    abstract keysort: n: int * list: BeamList<'T> -> BeamList<'T>
    /// Deletes the first tuple in List whose Nth element equals Key.
    abstract keydelete: key: obj * n: int * list: BeamList<'T> -> BeamList<'T>
    /// Returns true if any tuple in List has Key at position N (1-based).
    abstract keymember: key: obj * n: int * list: BeamList<'T> -> bool
    /// Replaces the first tuple whose Nth element equals Key with NewTuple.
    abstract keyreplace: key: obj * n: int * list: BeamList<'T> * newTuple: 'T -> BeamList<'T>

    /// Combines map and left fold: applies Fun to each element and an accumulator,
    /// returning a new list of the first Fun results and the final accumulator.
    abstract mapfoldl: f: System.Func<'T, 'Acc, 'U * 'Acc> * acc: 'Acc * list: BeamList<'T> -> BeamList<'U> * 'Acc
    /// Combines map and right fold.
    abstract mapfoldr: f: System.Func<'T, 'Acc, 'U * 'Acc> * acc: 'Acc * list: BeamList<'T> -> BeamList<'U> * 'Acc

/// lists module
[<ImportAll("lists")>]
let lists: IExports = nativeOnly

/// Searches a list of tuples for the first one whose Nth element (1-based) equals Key.
/// Returns Some(tuple) if found, or None if not found.
[<Emit("(fun() -> case lists:keyfind($0, $1, $2) of false -> undefined; KeyFindTuple__ -> KeyFindTuple__ end end)()")>]
let keyFind (key: obj) (n: int) (list: BeamList<'T>) : 'T option = nativeOnly
