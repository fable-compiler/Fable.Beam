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
    /// Returns the value for Key if {Key, Value} is found in List; the atom 'true' if
    /// the bare atom Key is present (in that case 'V must accept a bool, or the result
    /// is a runtime type mismatch); or undefined (None) if Key is not in List.
    /// Note: a stored value equal to the atom 'undefined' is indistinguishable from "missing".
    abstract get_value: key: 'K * list: BeamList<obj> -> 'V option

    /// Returns the value for Key if found in List, or Default otherwise.
    /// Note: same bare-atom and 'undefined' caveats as the 2-arity overload apply;
    /// a stored atom 'undefined' is treated as "not found" and yields Default.
    abstract get_value: key: 'K * list: BeamList<obj> * ``default``: 'V -> 'V

    /// Returns a list of all values associated with Key in List.
    abstract get_all_values: key: 'K * list: BeamList<obj> -> BeamList<'V>

    /// Returns true if Key is present in List, otherwise false.
    abstract is_defined: key: 'K * list: BeamList<obj> -> bool

    /// Deletes all entries with the given Key from List.
    abstract delete: key: 'K * list: BeamList<obj> -> BeamList<obj>

    /// Expands all bare atom entries in List to {Atom, true} pairs.
    abstract unfold: list: BeamList<obj> -> BeamList<obj>

    /// Minimizes List by collapsing each {Key, true} pair (where Key is an atom) to
    /// the bare atom Key. Inverse of unfold. Does not deduplicate or drop false values.
    abstract compact: list: BeamList<obj> -> BeamList<obj>

    /// Returns an unordered list of all keys in List, without duplicates.
    [<Emit("fable_utils:new_ref(proplists:get_keys($0))")>]
    abstract get_keys: list: BeamList<obj> -> 'K array

    /// Converts a property list to a map. Requires OTP 24+.
    abstract to_map: list: BeamList<obj> -> BeamMap<'K, 'V>

    /// Converts a map to a property list. Requires OTP 24+.
    abstract from_map: map: BeamMap<'K, 'V> -> BeamList<obj>

/// proplists module
[<ImportAll("proplists")>]
let proplists: IExports = nativeOnly
