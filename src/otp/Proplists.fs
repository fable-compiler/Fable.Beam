/// Type bindings for Erlang proplists module
/// See https://www.erlang.org/doc/apps/stdlib/proplists
module Fable.Beam.Proplists

open Fable.Core
open Fable.Beam
open Fable.Beam.Lists
open Fable.Beam.Maps

/// Returns the value for Key if {Key, Value} is found in List, or None otherwise.
[<Emit("proplists:get_value($0, $1)")>]
let tryFind (key: 'K) (list: BeamList<obj>) : 'V option = nativeOnly

/// Returns the value for Key if found in List, or Default otherwise.
[<Emit("proplists:get_value($0, $1, $2)")>]
let getOrDefault (key: 'K) (list: BeamList<obj>) (defaultValue: 'V) : 'V = nativeOnly

/// Returns a list of all values associated with Key in List.
[<Emit("proplists:get_all_values($0, $1)")>]
let getAllValues (key: 'K) (list: BeamList<obj>) : BeamList<'V> = nativeOnly

/// Returns true if Key is present in List, otherwise false.
[<Emit("proplists:is_defined($0, $1)")>]
let containsKey (key: 'K) (list: BeamList<obj>) : bool = nativeOnly

/// Deletes all entries with the given Key from List.
[<Emit("proplists:delete($0, $1)")>]
let remove (key: 'K) (list: BeamList<obj>) : BeamList<obj> = nativeOnly

/// Expands all bare atom entries in List to {Atom, true} pairs.
[<Emit("proplists:unfold($0)")>]
let unfold (list: BeamList<obj>) : BeamList<obj> = nativeOnly

/// Minimizes List by collapsing each {Key, true} pair (where Key is an atom) to bare atoms.
[<Emit("proplists:compact($0)")>]
let compact (list: BeamList<obj>) : BeamList<obj> = nativeOnly

/// Returns an unordered array of all keys in List, without duplicates.
[<Emit("fable_utils:new_ref(proplists:get_keys($0))")>]
let keys (list: BeamList<obj>) : 'K array = nativeOnly

/// Converts a property list to a map. Requires OTP 24+.
[<Emit("proplists:to_map($0)")>]
let toMap (list: BeamList<obj>) : BeamMap<'K, 'V> = nativeOnly

/// Converts a map to a property list. Requires OTP 24+.
[<Emit("proplists:from_map($0)")>]
let ofMap (map: BeamMap<'K, 'V>) : BeamList<obj> = nativeOnly

/// Like `proplists.get_keys`, but returns the native Erlang list instead of an F# array.
/// See "Dual API" in BINDINGS-GUIDE.md.
[<Emit("proplists:get_keys($0)")>]
let getKeysRaw (list: BeamList<obj>) : BeamList<'K> = nativeOnly
