/// Type bindings for Erlang Term Storage (ETS)
/// See https://www.erlang.org/doc/apps/stdlib/ets
module Fable.Beam.Ets

open Fable.Core
open Fable.Beam

// fsharplint:disable MemberNames

/// Opaque ETS table identifier.
[<Erase>]
type TableId = TableId of obj

[<Erase>]
type IExports =
    /// Creates a new ETS table.
    abstract new_: name: Atom * options: Atom list -> TableId
    /// Inserts a tuple or list of tuples into the table.
    abstract insert: table: TableId * objects: obj -> bool
    /// Looks up elements with the given key.
    [<Emit("fable_utils:new_ref(ets:lookup($0, $1))")>]
    abstract lookup: table: TableId * key: obj -> obj array
    /// Deletes an entire table.
    abstract delete: table: TableId -> unit
    /// Deletes all objects with key from the table.
    abstract delete: table: TableId * key: obj -> unit
    /// Returns a list of all objects in the table.
    [<Emit("fable_utils:new_ref(ets:tab2list($0))")>]
    abstract tab2list: table: TableId -> obj array
    /// Returns information about the table.
    abstract info: table: TableId -> obj
    /// Matches objects in the table against a pattern.
    [<Emit("fable_utils:new_ref(ets:'match'($0, $1))")>]
    abstract ``match``: table: TableId * pattern: obj -> obj array
    /// Selects objects using a match specification.
    [<Emit("fable_utils:new_ref(ets:select($0, $1))")>]
    abstract select: table: TableId * matchSpec: obj -> obj array
    /// Returns the first key in the table.
    abstract first: table: TableId -> obj
    /// Returns the next key after the given key.
    abstract next: table: TableId * key: obj -> obj

/// ets module
[<ImportAll("ets")>]
let ets: IExports = nativeOnly
