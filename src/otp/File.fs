/// Type bindings for Erlang file module
/// See https://www.erlang.org/doc/apps/kernel/file
module Fable.Beam.File

open Fable.Core

// fsharplint:disable MemberNames

// ============================================================================
// Raw bindings (internal escape hatch; returns obj — prefer the typed API below)
// ============================================================================

[<Erase>]
type internal IExports =
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
let internal file: IExports = nativeOnly

// ============================================================================
// Typed API with charlist conversion and Result returns
// ============================================================================
// NOTE: the Emit expressions below are wrapped in (fun() -> ... end)(). As of
// Fable 5.0.0 this is no longer required — the BEAM backend auto-wraps every
// case-containing Emit, so clause variables are isolated even when the same Emit
// is inlined twice into one function. The wrappers are kept for explicitness and
// are safe to remove.

/// Kind of filesystem entry reported by OTP's `file_info` record.
type FileKind =
    | Device
    | Directory
    | Other
    | Regular
    | Symlink

/// Selected portable metadata from OTP's `file_info` record.
///
/// Fields that OTP may report as `undefined` are represented as `None`.
/// `majorDevice` identifies the filesystem; `minorDevice` is meaningful for
/// Unix character devices; and `inode` is zero or unavailable on filesystems
/// without Unix-style inode identity.
type FileInfo =
    {
        kind: FileKind
        mode: int option
        majorDevice: int option
        minorDevice: int option
        inode: int option
    }

/// OTP's private `#file_info{}` record. Keep the native tuple opaque so callers
/// cannot depend on its positional representation.
[<Erase>]
type private NativeFileInfo = NativeFileInfo of obj

[<Emit("file:read_file_info(binary_to_list($0))")>]
let private readFileInfoRaw (path: string) : Result<NativeFileInfo, Atom> = nativeOnly

[<Emit("file:read_link_info(binary_to_list($0))")>]
let private readLinkInfoRaw (path: string) : Result<NativeFileInfo, Atom> = nativeOnly

[<Emit("erlang:element(3, $0)")>]
let private fileInfoKind (info: NativeFileInfo) : FileKind option = nativeOnly

[<Emit("erlang:element(8, $0)")>]
let private fileInfoMode (info: NativeFileInfo) : int option = nativeOnly

[<Emit("erlang:element(10, $0)")>]
let private fileInfoMajorDevice (info: NativeFileInfo) : int option = nativeOnly

[<Emit("erlang:element(11, $0)")>]
let private fileInfoMinorDevice (info: NativeFileInfo) : int option = nativeOnly

[<Emit("erlang:element(12, $0)")>]
let private fileInfoInode (info: NativeFileInfo) : int option = nativeOnly

let private toFileInfo (info: NativeFileInfo) : FileInfo =
    {
        kind = fileInfoKind info |> Option.defaultValue FileKind.Other
        mode = fileInfoMode info
        majorDevice = fileInfoMajorDevice info
        minorDevice = fileInfoMinorDevice info
        inode = fileInfoInode info
    }

let private mapFileInfoResult (result: Result<NativeFileInfo, Atom>) : Result<FileInfo, string> =
    result |> Result.map toFileInfo |> Result.mapError Atom.toString

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

/// Reads metadata for a filesystem entry, following symbolic links.
let readFileInfo (path: string) : Result<FileInfo, string> =
    readFileInfoRaw path |> mapFileInfoResult

/// Reads metadata for a filesystem entry without following a symbolic link.
/// A missing path is returned as `Error "enoent"`, consistently with the other
/// typed file functions.
let readLinkInfo (path: string) : Result<FileInfo, string> =
    readLinkInfoRaw path |> mapFileInfoResult

/// Renames (moves) a file. Handles binary_to_list conversion for both paths.
[<Emit("(fun() -> case file:rename(binary_to_list($0), binary_to_list($1)) of ok -> {ok, ok}; {error, FileRenameReason__} -> {error, erlang:atom_to_binary(FileRenameReason__)} end end)()")>]
let rename (source: string) (destination: string) : Result<unit, string> = nativeOnly

/// Returns the current working directory as a binary string.
[<Emit("(fun() -> case file:get_cwd() of {ok, FileGetCwdDir__} -> {ok, erlang:list_to_binary(FileGetCwdDir__)}; {error, FileGetCwdReason__} -> {error, erlang:atom_to_binary(FileGetCwdReason__)} end end)()")>]
let getCwd () : Result<string, string> = nativeOnly

/// Checks if a file or directory exists at the given path.
[<Emit("(fun() -> case file:read_file_info(binary_to_list($0)) of {ok, _} -> true; {error, _} -> false end end)()")>]
let exists (path: string) : bool = nativeOnly

/// Sets the current working directory. Handles binary_to_list conversion for path.
[<Emit("(fun() -> case file:set_cwd(binary_to_list($0)) of ok -> {ok, ok}; {error, FileSetCwdReason__} -> {error, erlang:atom_to_binary(FileSetCwdReason__)} end end)()")>]
let setCwd (path: string) : Result<unit, string> = nativeOnly
