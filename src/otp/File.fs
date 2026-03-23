/// Type bindings for Erlang file module
/// See https://www.erlang.org/doc/apps/kernel/file
module Fable.Beam.File

open Fable.Core

// fsharplint:disable MemberNames

// ============================================================================
// Raw bindings (returns obj, caller must handle tuples)
// ============================================================================

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

/// file module (raw, no charlist conversion — prefer typed functions below)
[<ImportAll("file")>]
let file: IExports = nativeOnly

// ============================================================================
// Typed API with charlist conversion and Result returns
// ============================================================================
// WORKAROUND: All Emit expressions below are wrapped in (fun() -> ... end)()
// to prevent Erlang "unsafe variable" errors when multiple Emit calls appear
// in the same function. This is a Fable BEAM backend bug — Emit inlines case
// expressions without scoping variables. Remove IIFEs once fixed in Fable.

/// Reads the contents of a file. Handles binary_to_list conversion for path.
/// Returns Ok with file contents as binary, or Error with reason as string.
[<Emit("(fun() -> case file:read_file(binary_to_list($0)) of {ok, FileReadData__} -> {ok, FileReadData__}; {error, FileReadReason__} -> {error, erlang:atom_to_binary(FileReadReason__)} end end)()")>]
let readFile (path: string) : Result<string, string> = nativeOnly

/// Writes data to a file. Handles binary_to_list conversion for path.
/// Returns Ok unit or Error with reason as string.
[<Emit("(fun() -> case file:write_file(binary_to_list($0), $1) of ok -> {ok, ok}; {error, FileWriteReason__} -> {error, erlang:atom_to_binary(FileWriteReason__)} end end)()")>]
let writeFile (path: string) (data: string) : Result<unit, string> = nativeOnly

/// Deletes a file. Handles binary_to_list conversion for path.
[<Emit("(fun() -> case file:delete(binary_to_list($0)) of ok -> {ok, ok}; {error, FileDeleteReason__} -> {error, erlang:atom_to_binary(FileDeleteReason__)} end end)()")>]
let delete (path: string) : Result<unit, string> = nativeOnly

/// Creates a directory. Handles binary_to_list conversion for path.
[<Emit("(fun() -> case file:make_dir(binary_to_list($0)) of ok -> {ok, ok}; {error, FileMkDirReason__} -> {error, erlang:atom_to_binary(FileMkDirReason__)} end end)()")>]
let makeDir (path: string) : Result<unit, string> = nativeOnly

/// Deletes a directory. Handles binary_to_list conversion for path.
[<Emit("(fun() -> case file:del_dir(binary_to_list($0)) of ok -> {ok, ok}; {error, FileDelDirReason__} -> {error, erlang:atom_to_binary(FileDelDirReason__)} end end)()")>]
let delDir (path: string) : Result<unit, string> = nativeOnly

/// Lists files in a directory. Converts charlist filenames to binaries.
[<Emit("(fun() -> case file:list_dir(binary_to_list($0)) of {ok, FileListFiles__} -> {ok, [erlang:list_to_binary(FileListF__) || FileListF__ <- FileListFiles__]}; {error, FileListReason__} -> {error, erlang:atom_to_binary(FileListReason__)} end end)()")>]
let listDir (path: string) : Result<string list, string> = nativeOnly

/// Renames (moves) a file. Handles binary_to_list conversion for both paths.
[<Emit("(fun() -> case file:rename(binary_to_list($0), binary_to_list($1)) of ok -> {ok, ok}; {error, FileRenameReason__} -> {error, erlang:atom_to_binary(FileRenameReason__)} end end)()")>]
let rename (source: string) (destination: string) : Result<unit, string> = nativeOnly

/// Returns the current working directory as a binary string.
[<Emit("(fun() -> case file:get_cwd() of {ok, FileGetCwdDir__} -> {ok, erlang:list_to_binary(FileGetCwdDir__)}; {error, FileGetCwdReason__} -> {error, erlang:atom_to_binary(FileGetCwdReason__)} end end)()")>]
let getCwd () : Result<string, string> = nativeOnly

/// Checks if a file or directory exists at the given path.
[<Emit("(fun() -> case file:read_file_info(binary_to_list($0)) of {ok, _} -> true; {error, _} -> false end end)()")>]
let exists (path: string) : bool = nativeOnly
