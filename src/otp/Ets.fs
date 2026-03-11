/// Type bindings for Erlang Term Storage (ETS)
/// See https://www.erlang.org/doc/apps/stdlib/ets
module Fable.Beam.Ets

open Fable.Core

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Creates a new ETS table.
    abstract new_: name: obj * options: obj list -> obj
    /// Inserts a tuple or list of tuples into the table.
    abstract insert: table: obj * objects: obj -> bool
    /// Looks up elements with the given key.
    abstract lookup: table: obj * key: obj -> obj array
    /// Deletes an entire table.
    abstract delete: table: obj -> unit
    /// Deletes all objects with key from the table.
    abstract delete: table: obj * key: obj -> unit
    /// Returns a list of all objects in the table.
    abstract tab2list: table: obj -> obj array
    /// Returns information about the table.
    abstract info: table: obj -> obj
    /// Matches objects in the table against a pattern.
    abstract ``match``: table: obj * pattern: obj -> obj array
    /// Selects objects using a match specification.
    abstract select: table: obj * matchSpec: obj -> obj array
    /// Returns the first key in the table.
    abstract first: table: obj -> obj
    /// Returns the next key after the given key.
    abstract next: table: obj * key: obj -> obj

/// ets module
[<ImportAll("ets")>]
let ets: IExports = nativeOnly
