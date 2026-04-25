/// Type bindings for Erlang binary module
/// See https://www.erlang.org/doc/apps/stdlib/binary
module Fable.Beam.Binary

open Fable.Core
open Fable.Beam
open Fable.Beam.Lists

// fsharplint:disable MemberNames

// ============================================================================
// Raw bindings
// ============================================================================

[<Erase>]
type IExports =
    /// Returns a copy of the binary.
    abstract copy: subject: string -> string
    /// Returns N copies of the binary concatenated.
    abstract copy: subject: string * n: int -> string
    /// Returns the byte at 0-based position Pos in the binary.
    abstract at: subject: string * pos: int -> int
    /// Returns the first byte of the binary.
    abstract first: subject: string -> int
    /// Returns the last byte of the binary.
    abstract last: subject: string -> int
    /// Returns a subbinary starting at Pos with Len bytes.
    abstract part: subject: string * pos: int * len: int -> string
    /// Decodes a binary as a big-endian unsigned integer.
    abstract decode_unsigned: subject: string -> int
    /// Decodes a binary as an unsigned integer with the given endianness (big or little).
    abstract decode_unsigned: subject: string * endianness: Atom -> int
    /// Encodes an unsigned integer as a binary (big-endian).
    abstract encode_unsigned: n: int -> string
    /// Encodes an unsigned integer as a binary with the given endianness (big or little).
    abstract encode_unsigned: n: int * endianness: Atom -> string
    /// Converts a binary to a list of bytes (integers in 0..255).
    abstract bin_to_list: subject: string -> BeamList<int>
    /// Converts a list of bytes (integers in 0..255) to a binary.
    abstract list_to_bin: byteList: BeamList<int> -> string
    /// Returns the length of the longest common prefix of a list of binaries.
    abstract longest_common_prefix: binaries: string list -> int
    /// Returns the length of the longest common suffix of a list of binaries.
    abstract longest_common_suffix: binaries: string list -> int
    /// Returns the byte size of the underlying memory referenced by the binary.
    abstract referenced_byte_size: subject: string -> int

/// binary module
[<ImportAll("binary")>]
let binary: IExports = nativeOnly

// ============================================================================
// Typed API — match, matches, split, replace
// ============================================================================
// WORKAROUND: Emit expressions wrapped in (fun() -> ... end)() to prevent
// Erlang "unsafe variable" errors. Remove IIFEs once fixed in Fable.

/// Searches for Pattern in Subject.
/// Returns Some (startPos, length) if found, or None if not found.
[<Emit("(fun() -> case binary:match($0, $1) of nomatch -> undefined; {BinMatchStart__, BinMatchLen__} -> {BinMatchStart__, BinMatchLen__} end end)()")>]
let matchFirst (subject: string) (pattern: string) : (int * int) option = nativeOnly

/// Returns all occurrences of Pattern in Subject as an array of (startPos, length) tuples.
[<Emit("fable_utils:new_ref(binary:matches($0, $1))")>]
let matchAll (subject: string) (pattern: string) : (int * int) array = nativeOnly

/// Splits Subject on the first occurrence of Pattern.
/// Returns an array of two binaries: the part before and the part after the pattern.
[<Emit("fable_utils:new_ref(binary:split($0, $1))")>]
let splitFirst (subject: string) (pattern: string) : string array = nativeOnly

/// Splits Subject on all occurrences of Pattern.
/// Returns an array of all parts between occurrences.
[<Emit("fable_utils:new_ref(binary:split($0, $1, [global]))")>]
let splitAll (subject: string) (pattern: string) : string array = nativeOnly

/// Replaces the first occurrence of Pattern in Subject with Replacement.
[<Emit("binary:replace($0, $1, $2)")>]
let replaceFirst (subject: string) (pattern: string) (replacement: string) : string = nativeOnly

/// Replaces all occurrences of Pattern in Subject with Replacement.
[<Emit("binary:replace($0, $1, $2, [global])")>]
let replaceAll (subject: string) (pattern: string) (replacement: string) : string = nativeOnly
