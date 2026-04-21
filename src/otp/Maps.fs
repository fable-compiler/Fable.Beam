/// Type bindings for Erlang maps module
/// See https://www.erlang.org/doc/apps/stdlib/maps
module Fable.Beam.Maps

open Fable.Core

// fsharplint:disable MemberNames

/// Erlang map with typed keys and values.
[<Erase>]
type BeamMap<'K, 'V> = BeamMap of obj

[<Erase>]
type IExports =
    /// Returns a new empty map.
    abstract new_: unit -> BeamMap<'K, 'V>
    /// Gets the value associated with key.
    abstract get: key: 'K * map: BeamMap<'K, 'V> -> 'V
    /// Returns the value associated with key, or default if not found.
    abstract get: key: 'K * map: BeamMap<'K, 'V> * ``default``: 'V -> 'V
    /// Associates key with value in the map.
    abstract put: key: 'K * value: 'V * map: BeamMap<'K, 'V> -> BeamMap<'K, 'V>
    /// Removes a key from the map.
    abstract remove: key: 'K * map: BeamMap<'K, 'V> -> BeamMap<'K, 'V>
    /// Returns true if the map contains key.
    abstract is_key: key: 'K * map: BeamMap<'K, 'V> -> bool

    /// Returns an array of all keys in the map.
    [<Emit("fable_utils:new_ref(maps:keys($0))")>]
    abstract keys: map: BeamMap<'K, 'V> -> 'K array

    /// Returns an array of all values in the map.
    [<Emit("fable_utils:new_ref(maps:values($0))")>]
    abstract values: map: BeamMap<'K, 'V> -> 'V array

    /// Returns the number of key-value pairs in the map.
    abstract size: map: BeamMap<'K, 'V> -> int

    /// Converts a list of key-value pairs to a map.
    [<Emit("maps:from_list(erlang:get($0))")>]
    abstract from_list: list: ('K * 'V) array -> BeamMap<'K, 'V>

    /// Converts a map to an array of key-value pairs.
    [<Emit("fable_utils:new_ref(maps:to_list($0))")>]
    abstract to_list: map: BeamMap<'K, 'V> -> ('K * 'V) array

    /// Merges two maps.
    abstract merge: map1: BeamMap<'K, 'V> * map2: BeamMap<'K, 'V> -> BeamMap<'K, 'V>
    /// Applies a function to each key-value pair.
    abstract fold: f: System.Func<'K, 'V, 'Acc, 'Acc> * init: 'Acc * map: BeamMap<'K, 'V> -> 'Acc
    /// Applies a function to each value, returning a new map.
    abstract map: f: System.Func<'K, 'V, 'V2> * map: BeamMap<'K, 'V> -> BeamMap<'K, 'V2>
    /// Filters key-value pairs by a predicate.
    abstract filter: pred: System.Func<'K, 'V, bool> * map: BeamMap<'K, 'V> -> BeamMap<'K, 'V>
    /// Returns {ok, Value} if key is in the map, or the atom error if not.
    /// Prefer tryFind for type-safe optional lookup.
    abstract find: key: 'K * map: BeamMap<'K, 'V> -> obj

/// maps module
[<ImportAll("maps")>]
let maps: IExports = nativeOnly

/// Returns Some value if key is in the map, or None if not found.
/// Type-safe wrapper around maps:find/2.
[<Emit("(fun() -> case maps:find($0, $1) of error -> undefined; {ok, MapsFindVal__} -> MapsFindVal__ end end)()")>]
let tryFind (key: 'K) (map: BeamMap<'K, 'V>) : 'V option = nativeOnly
