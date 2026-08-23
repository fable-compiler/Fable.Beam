/// Type bindings for Erlang Term Storage (ETS)
/// See https://www.erlang.org/doc/apps/stdlib/ets
module Fable.Beam.Ets

open Fable.Core
open Fable.Beam

// fsharplint:disable MemberNames

/// Opaque ETS table identifier.
/// The phantom 'Key and 'Row capture the types of keys and stored rows —
/// analogous to BeamMap<'K,'V>. Erased at runtime.
[<Erase>]
type TableId<'Key, 'Row> = TableId of obj

/// ETS table type. Determines key uniqueness and ordering.
type EtsTableType =
    | Set
    | [<CompiledName("ordered_set")>] OrderedSet
    | Bag
    | [<CompiledName("duplicate_bag")>] DuplicateBag

/// ETS table access level. Determines which processes can read/write the table.
type EtsAccess =
    | Public
    | Protected
    | Private

/// Creates a new ETS table.
[<Emit("ets:new($0, $1)")>]
let create (name: Atom) (options: Atom list) : TableId<'Key, 'Row> = nativeOnly

/// Inserts a tuple or list of tuples into the table.
[<Emit("ets:insert($0, $1)")>]
let insert (table: TableId<'Key, 'Row>) (objects: 'Row) : bool = nativeOnly

/// Looks up elements with the given key.
[<Emit("fable_utils:new_ref(ets:lookup($0, $1))")>]
let lookup (table: TableId<'Key, 'Row>) (key: 'Key) : 'Row array = nativeOnly

/// Deletes an entire table.
[<Emit("ets:delete($0)")>]
let delete (table: TableId<'Key, 'Row>) : unit = nativeOnly

/// Deletes all objects with a key from the table.
[<Emit("ets:delete($0, $1)")>]
let deleteKey (table: TableId<'Key, 'Row>) (key: 'Key) : unit = nativeOnly

/// Returns all objects in the table.
[<Emit("fable_utils:new_ref(ets:tab2list($0))")>]
let toArray (table: TableId<'Key, 'Row>) : 'Row array = nativeOnly

/// Returns information about the table.
[<Emit("ets:info($0)")>]
let info (table: TableId<'Key, 'Row>) : obj = nativeOnly

/// Matches objects in the table against a pattern.
[<Emit("fable_utils:new_ref(ets:'match'($0, $1))")>]
let matchRows (table: TableId<'Key, 'Row>) (pattern: obj) : obj array = nativeOnly

/// Selects objects using a match specification.
[<Emit("fable_utils:new_ref(ets:select($0, $1))")>]
let select (table: TableId<'Key, 'Row>) (matchSpec: obj) : obj array = nativeOnly

/// Returns the first key in the table, or None if the table is empty.
[<Emit("(fun() -> case ets:first($0) of '$end_of_table' -> undefined; FirstKey__ -> FirstKey__ end end)()")>]
let first (table: TableId<'Key, 'Row>) : 'Key option = nativeOnly

/// Returns the next key after the given key, or None if at the end.
[<Emit("(fun() -> case ets:next($0, $1) of '$end_of_table' -> undefined; NextKey__ -> NextKey__ end end)()")>]
let next (table: TableId<'Key, 'Row>) (key: 'Key) : 'Key option = nativeOnly

// ============================================================================
// Typed ets:info/2 accessors for common fields
// ============================================================================

/// Number of objects stored in the table.
[<Emit("ets:info($0, size)")>]
let size (table: TableId<'K, 'R>) : int = nativeOnly

/// Name of the table (as given to ets:new/2).
[<Emit("ets:info($0, name)")>]
let tableName (table: TableId<'K, 'R>) : Atom = nativeOnly

/// Table type (set / ordered_set / bag / duplicate_bag).
[<Emit("ets:info($0, type)")>]
let tableType (table: TableId<'K, 'R>) : EtsTableType = nativeOnly

/// Access level (public / protected / private).
[<Emit("ets:info($0, protection)")>]
let access (table: TableId<'K, 'R>) : EtsAccess = nativeOnly

/// Pid of the process that owns this table.
[<Emit("ets:info($0, owner)")>]
let owner (table: TableId<'K, 'R>) : Pid<obj> = nativeOnly

/// Number of words allocated to the table in memory.
[<Emit("ets:info($0, memory)")>]
let memory (table: TableId<'K, 'R>) : int = nativeOnly

/// Position of the key element in each row tuple (1-based).
[<Emit("ets:info($0, keypos)")>]
let keypos (table: TableId<'K, 'R>) : int = nativeOnly
