/// Type bindings for Erlang lists module
/// See https://www.erlang.org/doc/apps/stdlib/lists
module Fable.Beam.Lists

open Fable.Core

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Appends two lists.
    abstract append: list1: obj * list2: obj -> obj
    /// Flattens a list of lists.
    abstract flatten: deepList: obj -> obj
    /// Returns the length of a list. Note: prefer erlang:length/1 for BIF performance.
    abstract flatlength: list: obj -> int
    /// Returns true if Elem is in the list.
    abstract ``member``: elem: obj * list: obj -> bool
    /// Reverses a list.
    abstract reverse: list: obj -> obj
    /// Sorts a list.
    abstract sort: list: obj -> obj
    /// Sorts a list using a comparison function.
    abstract sort: f: obj * list: obj -> obj
    /// Returns the Nth element (1-based).
    abstract nth: n: int * list: obj -> obj
    /// Returns the last element.
    abstract last: list: obj -> obj
    /// Applies a function to each element (map).
    abstract map: f: obj * list: obj -> obj
    /// Filters elements by a predicate.
    abstract filter: pred: obj * list: obj -> obj
    /// Left fold over a list.
    abstract foldl: f: obj * acc: obj * list: obj -> obj
    /// Right fold over a list.
    abstract foldr: f: obj * acc: obj * list: obj -> obj
    /// Applies a function to each element for side effects.
    abstract foreach: f: obj * list: obj -> unit
    /// Zips two lists into a list of tuples.
    abstract zip: list1: obj * list2: obj -> obj
    /// Unzips a list of tuples into a tuple of two lists.
    abstract unzip: list: obj -> obj * obj
    /// Returns a tuple of {Satisfying, NotSatisfying} elements.
    abstract partition: pred: obj * list: obj -> obj * obj
    /// Removes duplicate elements.
    abstract usort: list: obj -> obj
    /// Returns a sublist (first N elements).
    abstract sublist: list: obj * len: int -> obj
    /// Returns true if all elements satisfy the predicate.
    abstract all: pred: obj * list: obj -> bool
    /// Returns true if any element satisfies the predicate.
    abstract any: pred: obj * list: obj -> bool

/// lists module
[<ImportAll("lists")>]
let lists: IExports = nativeOnly
