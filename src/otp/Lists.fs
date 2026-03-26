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

/// lists module
[<ImportAll("lists")>]
let lists: IExports = nativeOnly
