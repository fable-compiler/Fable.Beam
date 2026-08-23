/// Type bindings for Erlang binary module
/// See https://www.erlang.org/doc/apps/stdlib/binary
module Fable.Beam.Binary

open Fable.Core
open Fable.Beam
open Fable.Beam.Lists

// fsharplint:disable MemberNames

/// Returns a copy of the binary.
[<Emit("binary:copy($0)")>]
let copy (subject: string) : string = nativeOnly

/// Returns N copies of the binary concatenated.
[<Emit("binary:copy($0, $1)")>]
let copyN (subject: string) (count: int) : string = nativeOnly

/// Returns the byte at 0-based position Pos in the binary.
[<Emit("binary:at($0, $1)")>]
let at (subject: string) (position: int) : int = nativeOnly

/// Returns the first byte of the binary.
[<Emit("binary:first($0)")>]
let first (subject: string) : int = nativeOnly

/// Returns the last byte of the binary.
[<Emit("binary:last($0)")>]
let last (subject: string) : int = nativeOnly

/// Returns a subbinary starting at Pos with Len bytes.
[<Emit("binary:part($0, $1, $2)")>]
let part (subject: string) (position: int) (length: int) : string = nativeOnly

/// Decodes a binary as a big-endian unsigned integer.
[<Emit("binary:decode_unsigned($0)")>]
let decodeUnsigned (subject: string) : int = nativeOnly

/// Decodes a binary as an unsigned integer with the given endianness (big or little).
[<Emit("binary:decode_unsigned($0, $1)")>]
let decodeUnsignedWithEndianness (subject: string) (endianness: Atom) : int = nativeOnly

/// Encodes an unsigned integer as a binary (big-endian).
[<Emit("binary:encode_unsigned($0)")>]
let encodeUnsigned (value: int) : string = nativeOnly

/// Encodes an unsigned integer as a binary with the given endianness (big or little).
[<Emit("binary:encode_unsigned($0, $1)")>]
let encodeUnsignedWithEndianness (value: int) (endianness: Atom) : string = nativeOnly

/// Converts a binary to a list of bytes (integers in 0..255).
[<Emit("binary:bin_to_list($0)")>]
let toByteList (subject: string) : BeamList<int> = nativeOnly

/// Converts a list of bytes (integers in 0..255) to a binary.
[<Emit("binary:list_to_bin($0)")>]
let ofByteList (bytes: BeamList<int>) : string = nativeOnly

/// Returns the length of the longest common prefix of a list of binaries.
[<Emit("binary:longest_common_prefix($0)")>]
let longestCommonPrefix (binaries: string list) : int = nativeOnly

/// Returns the length of the longest common suffix of a list of binaries.
[<Emit("binary:longest_common_suffix($0)")>]
let longestCommonSuffix (binaries: string list) : int = nativeOnly

/// Returns the byte size of the underlying memory referenced by the binary.
[<Emit("binary:referenced_byte_size($0)")>]
let referencedByteSize (subject: string) : int = nativeOnly

// ============================================================================
// Typed API — match, matches, split, replace
// ============================================================================
// NOTE: the (fun() -> ... end)() wrappers on the case Emits below are no longer
// required — Fable (>= 5.0.0) auto-wraps case-containing Emits for variable scoping.
// Kept for explicitness; safe to remove.

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

/// Like `splitFirst`, but returns the native Erlang list instead of an F# array. See "Dual API".
[<Emit("binary:split($0, $1)")>]
let splitFirstRaw (subject: string) (pattern: string) : BeamList<string> = nativeOnly

/// Splits Subject on all occurrences of Pattern.
/// Returns an array of all parts between occurrences.
[<Emit("fable_utils:new_ref(binary:split($0, $1, [global]))")>]
let splitAll (subject: string) (pattern: string) : string array = nativeOnly

/// Like `splitAll`, but returns the native Erlang list instead of an F# array. See "Dual API".
[<Emit("binary:split($0, $1, [global])")>]
let splitAllRaw (subject: string) (pattern: string) : BeamList<string> = nativeOnly

/// Replaces the first occurrence of Pattern in Subject with Replacement.
[<Emit("binary:replace($0, $1, $2)")>]
let replaceFirst (subject: string) (pattern: string) (replacement: string) : string = nativeOnly

/// Replaces all occurrences of Pattern in Subject with Replacement.
[<Emit("binary:replace($0, $1, $2, [global])")>]
let replaceAll (subject: string) (pattern: string) (replacement: string) : string = nativeOnly
