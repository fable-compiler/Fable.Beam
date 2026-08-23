/// Type bindings for Erlang lists module
/// See https://www.erlang.org/doc/apps/stdlib/lists
module Fable.Beam.Lists

open Fable.Core

// fsharplint:disable MemberNames

/// Erlang list with typed elements.
[<Erase>]
type BeamList<'T> = BeamList of obj

/// Appends two lists.
[<Emit("lists:append($0, $1)")>]
let append (list1: BeamList<'T>) (list2: BeamList<'T>) : BeamList<'T> = nativeOnly

[<Emit("lists:flatten($0)")>]
let flatten (deepList: BeamList<BeamList<'T>>) : BeamList<'T> = nativeOnly

[<Emit("lists:flatlength($0)")>]
let flatlength (list: BeamList<'T>) : int = nativeOnly

[<Emit("lists:member($0, $1)")>]
let ``member`` (elem: 'T) (list: BeamList<'T>) : bool = nativeOnly

[<Emit("lists:reverse($0)")>]
let reverse (list: BeamList<'T>) : BeamList<'T> = nativeOnly

[<Emit("lists:sort($0)")>]
let sort (list: BeamList<'T>) : BeamList<'T> = nativeOnly

[<Emit("lists:sort($0, $1)")>]
let sortWith (f: 'T -> 'T -> bool) (list: BeamList<'T>) : BeamList<'T> = nativeOnly

[<Emit("lists:nth($0, $1)")>]
let nth (n: int) (list: BeamList<'T>) : 'T = nativeOnly

[<Emit("lists:last($0)")>]
let last (list: BeamList<'T>) : 'T = nativeOnly

[<Emit("lists:map($0, $1)")>]
let map (f: 'T -> 'U) (list: BeamList<'T>) : BeamList<'U> = nativeOnly

[<Emit("lists:filter($0, $1)")>]
let filter (pred: 'T -> bool) (list: BeamList<'T>) : BeamList<'T> = nativeOnly

[<Emit("lists:foldl($0, $1, $2)")>]
let foldl (f: 'T -> 'Acc -> 'Acc) (acc: 'Acc) (list: BeamList<'T>) : 'Acc = nativeOnly

[<Emit("lists:foldr($0, $1, $2)")>]
let foldr (f: 'T -> 'Acc -> 'Acc) (acc: 'Acc) (list: BeamList<'T>) : 'Acc = nativeOnly

[<Emit("lists:foreach($0, $1)")>]
let foreach (f: System.Action<'T>) (list: BeamList<'T>) : unit = nativeOnly

[<Emit("lists:zip($0, $1)")>]
let zip (list1: BeamList<'A>) (list2: BeamList<'B>) : BeamList<'A * 'B> = nativeOnly

[<Emit("lists:unzip($0)")>]
let unzip (list: BeamList<'A * 'B>) : BeamList<'A> * BeamList<'B> = nativeOnly

[<Emit("lists:partition($0, $1)")>]
let partition (pred: 'T -> bool) (list: BeamList<'T>) : BeamList<'T> * BeamList<'T> = nativeOnly

[<Emit("lists:usort($0)")>]
let usort (list: BeamList<'T>) : BeamList<'T> = nativeOnly

[<Emit("lists:sublist($0, $1)")>]
let sublist (list: BeamList<'T>) (length: int) : BeamList<'T> = nativeOnly

[<Emit("lists:all($0, $1)")>]
let all (pred: 'T -> bool) (list: BeamList<'T>) : bool = nativeOnly

[<Emit("lists:any($0, $1)")>]
let any (pred: 'T -> bool) (list: BeamList<'T>) : bool = nativeOnly

[<Emit("lists:sum($0)")>]
let sumInt (list: BeamList<int>) : int = nativeOnly

[<Emit("lists:sum($0)")>]
let sumFloat (list: BeamList<float>) : float = nativeOnly

[<Emit("lists:max($0)")>]
let max (list: BeamList<'T>) : 'T = nativeOnly

[<Emit("lists:min($0)")>]
let min (list: BeamList<'T>) : 'T = nativeOnly

[<Emit("lists:seq($0, $1)")>]
let seq (from: int) (``to``: int) : BeamList<int> = nativeOnly

[<Emit("lists:seq($0, $1, $2)")>]
let seqStep (from: int) (``to``: int) (increment: int) : BeamList<int> = nativeOnly

[<Emit("lists:duplicate($0, $1)")>]
let duplicate (n: int) (elem: 'T) : BeamList<'T> = nativeOnly

[<Emit("lists:takewhile($0, $1)")>]
let takeWhile (pred: 'T -> bool) (list: BeamList<'T>) : BeamList<'T> = nativeOnly

[<Emit("lists:dropwhile($0, $1)")>]
let dropWhile (pred: 'T -> bool) (list: BeamList<'T>) : BeamList<'T> = nativeOnly

[<Emit("lists:splitwith($0, $1)")>]
let splitWith (pred: 'T -> bool) (list: BeamList<'T>) : BeamList<'T> * BeamList<'T> = nativeOnly

[<Emit("lists:delete($0, $1)")>]
let delete (elem: 'T) (list: BeamList<'T>) : BeamList<'T> = nativeOnly

[<Emit("lists:subtract($0, $1)")>]
let subtract (list1: BeamList<'T>) (list2: BeamList<'T>) : BeamList<'T> = nativeOnly

[<Emit("lists:keysort($0, $1)")>]
let keySort (n: int) (list: BeamList<'T>) : BeamList<'T> = nativeOnly

[<Emit("lists:keydelete($0, $1, $2)")>]
let keyDelete (key: 'Key) (n: int) (list: BeamList<'T>) : BeamList<'T> = nativeOnly

[<Emit("lists:keymember($0, $1, $2)")>]
let keyMember (key: 'Key) (n: int) (list: BeamList<'T>) : bool = nativeOnly

[<Emit("lists:keyreplace($0, $1, $2, $3)")>]
let keyReplace (key: 'Key) (n: int) (list: BeamList<'T>) (newTuple: 'T) : BeamList<'T> = nativeOnly

[<Emit("lists:mapfoldl($0, $1, $2)")>]
let mapFoldLeft (f: 'T -> 'Acc -> 'U * 'Acc) (acc: 'Acc) (list: BeamList<'T>) : BeamList<'U> * 'Acc = nativeOnly

[<Emit("lists:mapfoldr($0, $1, $2)")>]
let mapFoldRight (f: 'T -> 'Acc -> 'U * 'Acc) (acc: 'Acc) (list: BeamList<'T>) : BeamList<'U> * 'Acc = nativeOnly

/// Searches a list of tuples for the first one whose Nth element (1-based) equals Key.
/// Returns Some(tuple) if found, or None if not found.
[<Emit("(fun() -> case lists:keyfind($0, $1, $2) of false -> undefined; KeyFindTuple__ -> KeyFindTuple__ end end)()")>]
let keyFind (key: 'Key) (n: int) (list: BeamList<'T>) : 'T option = nativeOnly
