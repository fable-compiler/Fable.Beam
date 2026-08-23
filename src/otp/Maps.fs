/// Type bindings for Erlang maps module
/// See https://www.erlang.org/doc/apps/stdlib/maps
module Fable.Beam.Maps

open Fable.Core
open Fable.Beam.Lists

// fsharplint:disable MemberNames

/// Erlang map with typed keys and values.
[<Erase>]
type BeamMap<'K, 'V> = BeamMap of obj

/// Returns a new empty map.
[<Emit("maps:new()")>]
let empty<'K, 'V> () : BeamMap<'K, 'V> = nativeOnly

/// Gets the value associated with key.
[<Emit("maps:get($0, $1)")>]
let get (key: 'K) (map: BeamMap<'K, 'V>) : 'V = nativeOnly

/// Returns the value associated with key, or default if not found.
[<Emit("maps:get($0, $1, $2)")>]
let getOrDefault (key: 'K) (map: BeamMap<'K, 'V>) (defaultValue: 'V) : 'V = nativeOnly

/// Associates key with value in the map.
[<Emit("maps:put($0, $1, $2)")>]
let put (key: 'K) (value: 'V) (map: BeamMap<'K, 'V>) : BeamMap<'K, 'V> = nativeOnly

/// Removes a key from the map.
[<Emit("maps:remove($0, $1)")>]
let remove (key: 'K) (map: BeamMap<'K, 'V>) : BeamMap<'K, 'V> = nativeOnly

/// Returns true if the map contains key.
[<Emit("maps:is_key($0, $1)")>]
let containsKey (key: 'K) (map: BeamMap<'K, 'V>) : bool = nativeOnly

/// Returns an array of all keys in the map.
[<Emit("fable_utils:new_ref(maps:keys($0))")>]
let keys (map: BeamMap<'K, 'V>) : 'K array = nativeOnly

/// Returns an array of all values in the map.
[<Emit("fable_utils:new_ref(maps:values($0))")>]
let values (map: BeamMap<'K, 'V>) : 'V array = nativeOnly

/// Returns the number of key-value pairs in the map.
[<Emit("maps:size($0)")>]
let size (map: BeamMap<'K, 'V>) : int = nativeOnly

/// Converts an array of key-value pairs to a map.
[<Emit("maps:from_list(erlang:get($0))")>]
let ofArray (pairs: ('K * 'V) array) : BeamMap<'K, 'V> = nativeOnly

/// Converts a map to an array of key-value pairs.
[<Emit("fable_utils:new_ref(maps:to_list($0))")>]
let toArray (map: BeamMap<'K, 'V>) : ('K * 'V) array = nativeOnly

/// Merges two maps.
[<Emit("maps:merge($0, $1)")>]
let merge (map1: BeamMap<'K, 'V>) (map2: BeamMap<'K, 'V>) : BeamMap<'K, 'V> = nativeOnly

/// Applies a function to each key-value pair.
[<Emit("maps:fold($0, $1, $2)")>]
let fold (folder: 'K -> 'V -> 'Acc -> 'Acc) (initial: 'Acc) (map: BeamMap<'K, 'V>) : 'Acc = nativeOnly

/// Applies a function to each value, returning a new map.
[<Emit("maps:map($0, $1)")>]
let map (mapping: 'K -> 'V -> 'V2) (map: BeamMap<'K, 'V>) : BeamMap<'K, 'V2> = nativeOnly

/// Filters key-value pairs by a predicate.
[<Emit("maps:filter($0, $1)")>]
let filter (predicate: 'K -> 'V -> bool) (map: BeamMap<'K, 'V>) : BeamMap<'K, 'V> = nativeOnly

/// Returns Some value if key is in the map, or None if not found.
/// Type-safe wrapper around maps:find/2.
[<Emit("(fun() -> case maps:find($0, $1) of error -> undefined; {ok, MapsFindVal__} -> MapsFindVal__ end end)()")>]
let tryFind (key: 'K) (map: BeamMap<'K, 'V>) : 'V option = nativeOnly

/// Builds a map from a list of key-value pairs. Unlike `maps.from_list` (which
/// takes an F# array and round-trips through a process-dictionary ref via
/// `erlang:get(fable_utils:new_ref(...))`), this takes an F# list and lowers to a
/// direct `maps:from_list([...])` — ideal for small literal maps such as Cowboy
/// response headers, e.g. `ofList [ "content-type", "text/html" ]`.
[<Emit("maps:from_list($0)")>]
let ofList (pairs: ('K * 'V) list) : BeamMap<'K, 'V> = nativeOnly

// Raw-list variants of keys/values/to_list: the `maps.*` members ref-wrap the Erlang list into an
// F# array (for Array.* ops); these return the native BeamList for BEAM-side use without the
// round-trip. See "Dual API" in BINDINGS-GUIDE.md.

/// Like `maps.keys`, but returns the native Erlang list instead of an F# array.
[<Emit("maps:keys($0)")>]
let keysRaw (map: BeamMap<'K, 'V>) : BeamList<'K> = nativeOnly

/// Like `maps.values`, but returns the native Erlang list instead of an F# array.
[<Emit("maps:values($0)")>]
let valuesRaw (map: BeamMap<'K, 'V>) : BeamList<'V> = nativeOnly

/// Like `maps.to_list`, but returns the native Erlang list instead of an F# array.
[<Emit("maps:to_list($0)")>]
let toListRaw (map: BeamMap<'K, 'V>) : BeamList<'K * 'V> = nativeOnly
