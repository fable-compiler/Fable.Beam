/// Type bindings for Erlang file module
/// See https://www.erlang.org/doc/apps/kernel/file
module Fable.Beam.File

open Fable.Core

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Reads the contents of a file.
    abstract read_file: filename: string -> obj
    /// Writes data to a file.
    abstract write_file: filename: string * data: obj -> obj
    /// Deletes a file.
    abstract delete: filename: string -> obj
    /// Makes a directory.
    abstract make_dir: dir: string -> obj
    /// Deletes a directory.
    abstract del_dir: dir: string -> obj
    /// Lists files in a directory.
    abstract list_dir: dir: string -> obj
    /// Returns file info.
    abstract read_file_info: filename: string -> obj
    /// Renames a file.
    abstract rename: source: string * destination: string -> obj
    /// Returns the current working directory.
    abstract get_cwd: unit -> obj
    /// Sets the current working directory.
    abstract set_cwd: dir: string -> obj

/// file module
[<ImportAll("file")>]
let file: IExports = nativeOnly
