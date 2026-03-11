/// Type bindings for Erlang maps module
/// See https://www.erlang.org/doc/apps/stdlib/maps
module Fable.Beam.Maps

open Fable.Core

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Returns a new empty map.
    abstract new_: unit -> obj
    /// Gets the value associated with key.
    abstract get: key: obj * map: obj -> obj
    /// Returns the value associated with key, or default if not found.
    abstract get: key: obj * map: obj * ``default``: obj -> obj
    /// Associates key with value in the map.
    abstract put: key: obj * value: obj * map: obj -> obj
    /// Removes a key from the map.
    abstract remove: key: obj * map: obj -> obj
    /// Returns true if the map contains key.
    abstract is_key: key: obj * map: obj -> bool
    /// Returns a list of all keys in the map.
    abstract keys: map: obj -> obj array
    /// Returns a list of all values in the map.
    abstract values: map: obj -> obj array
    /// Returns the number of key-value pairs in the map.
    abstract size: map: obj -> int
    /// Converts a list of key-value pairs to a map.
    abstract from_list: list: obj -> obj
    /// Converts a map to a list of key-value pairs.
    abstract to_list: map: obj -> obj array
    /// Merges two maps.
    abstract merge: map1: obj * map2: obj -> obj
    /// Applies a function to each key-value pair.
    abstract fold: f: obj * init: obj * map: obj -> obj
    /// Applies a function to each value, returning a new map.
    abstract map: f: obj * map: obj -> obj
    /// Filters key-value pairs by a predicate.
    abstract filter: pred: obj * map: obj -> obj
    /// Returns the value for key, or calls fun if not found.
    abstract find: key: obj * map: obj -> obj

/// maps module
[<ImportAll("maps")>]
let maps: IExports = nativeOnly
