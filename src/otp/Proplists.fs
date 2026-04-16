/// Type bindings for Erlang proplists module
/// See https://www.erlang.org/doc/apps/stdlib/proplists
module Fable.Beam.Proplists

open Fable.Core
open Fable.Beam
open Fable.Beam.Lists
open Fable.Beam.Maps

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Returns the value for Key if {Key, Value} is found in List; true if the bare atom Key
    /// is present; or undefined (None) if Key is not in List.
    abstract get_value: key: 'K * list: BeamList<obj> -> 'V option

    /// Returns the value for Key if found in List, or Default otherwise.
    abstract get_value: key: 'K * list: BeamList<obj> * ``default``: 'V -> 'V

    /// Returns a list of all values associated with Key in List.
    abstract get_all_values: key: 'K * list: BeamList<obj> -> BeamList<'V>

    /// Returns true if Key is present in List, otherwise false.
    abstract is_defined: key: 'K * list: BeamList<obj> -> bool

    /// Deletes all entries with the given Key from List.
    abstract delete: key: 'K * list: BeamList<obj> -> BeamList<obj>

    /// Expands all bare atom entries in List to {Atom, true} pairs.
    abstract unfold: list: BeamList<obj> -> BeamList<obj>

    /// Minimizes List by removing duplicate keys (keeping first) and entries for boolean
    /// keys whose value is false.
    abstract compact: list: BeamList<obj> -> BeamList<obj>

    /// Returns a list of all keys in List without duplicates, preserving first-occurrence order.
    [<Emit("fable_utils:new_ref(proplists:keys($0))")>]
    abstract keys: list: BeamList<obj> -> 'K array

    /// Converts a property list to a map. Requires OTP 24+.
    abstract to_map: list: BeamList<obj> -> BeamMap<'K, 'V>

    /// Converts a map to a property list. Requires OTP 24+.
    abstract from_map: map: BeamMap<'K, 'V> -> BeamList<obj>

/// proplists module
[<ImportAll("proplists")>]
let proplists: IExports = nativeOnly
