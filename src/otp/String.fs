/// Type bindings for Erlang string module (OTP 20+ binary mode)
/// See https://www.erlang.org/doc/apps/stdlib/string
module Fable.Beam.String

open Fable.Core
open Fable.Beam

// fsharplint:disable MemberNames

// ============================================================================
// Raw bindings — simple direct mappings via ImportAll
// ============================================================================

[<Erase>]
type IExports =
    /// Returns true if String contains no grapheme clusters, otherwise false.
    abstract is_empty: s: string -> bool

    /// Returns the number of grapheme clusters in String.
    /// Note: this is Unicode grapheme count, not byte length — use erlang.byteSize for bytes.
    abstract length: s: string -> int

    /// Converts String to lowercase.
    abstract lowercase: s: string -> string

    /// Converts String to uppercase.
    abstract uppercase: s: string -> string

    /// Converts the first grapheme cluster of String to uppercase and the rest to lowercase.
    abstract titlecase: s: string -> string

    /// Converts String to a case-folded form suitable for case-insensitive comparisons.
    abstract casefold: s: string -> string

    // `string:reverse/1` returns chardata, not a binary, so it can't be an ImportAll member here.
    // See `reverse` (-> string) and `reverseRaw` (-> BeamChardata) in the typed API below.

    // NOTE: `string:concat/2` is bound to Erlang's `++` operator, which expects
    // charlists — it raises `badarg` when called with binaries. Since F# strings
    // compile to binaries on BEAM, this binding cannot be used. Concatenate
    // F# strings directly (e.g., `s1 + s2`) instead.
    // abstract concat: s1: string * s2: string -> string

    /// Returns a string slice from grapheme position Start to end of String.
    abstract slice: s: string * start: int -> string

    /// Returns a string slice of at most Length graphemes starting at position Start.
    abstract slice: s: string * start: int * length: int -> string

    /// Strips leading and trailing whitespace (graphemes matching the Unicode "White_Space" property).
    abstract trim: s: string -> string

    /// Strips whitespace from the direction Dir of String.
    /// Dir must be the atom leading, trailing, or both (use erlang.binaryToAtom).
    abstract trim: s: string * dir: Atom -> string

    // NOTE: `string:trim/3` requires `Characters` to be a list of grapheme
    // clusters (`[char() | [char()]]`), not a binary. Passing an F# string
    // directly raises `function_clause`. A typed Emit helper that converts
    // the chars via `binary_to_list` would be needed — left unbound for now.
    // abstract trim: s: string * dir: Atom * characters: string -> string

    // Every `string:pad` arity returns chardata (an iolist), not a binary, so they can't be
    // ImportAll members here. See `pad`/`padDir`/`padWith` (-> string) and their `*Raw` variants
    // (-> BeamChardata) in the typed API below.

    /// Returns true if S1 and S2 are equal (ordinal).
    abstract equal: s1: string * s2: string -> bool

    /// Returns true if S1 and S2 are equal; if IgnoreCase is true, the comparison is
    /// normalised to NFC before comparing (Unicode case-insensitive).
    abstract equal: s1: string * s2: string * ignoreCase: bool -> bool

/// string module (named `str` to avoid shadowing F#'s built-in `string` conversion function)
[<ImportAll("string")>]
let str: IExports = nativeOnly

// ============================================================================
// Typed API — functions with non-trivial Erlang return values
// ============================================================================
// NOTE: the (fun() -> ... end)() wrappers on the case Emits below are no longer
// required — Fable (>= 5.0.0) auto-wraps case-containing Emits for variable scoping.
// Kept for explicitness; safe to remove.

/// Searches for the first occurrence of SearchPattern in String, searching left to right.
/// Returns Some suffix (the tail of String from the match start) or None if not found.
[<Emit("(fun() -> case string:find($0, $1) of nomatch -> undefined; StringFindResult__ -> StringFindResult__ end end)()")>]
let find (s: string) (pattern: string) : string option = nativeOnly

/// Searches for the first occurrence of SearchPattern in String in the given direction.
/// Dir must be the atom leading or trailing (use erlang.binaryToAtom).
/// Returns Some suffix/prefix or None if not found.
[<Emit("(fun() -> case string:find($0, $1, $2) of nomatch -> undefined; StringFindResult__ -> StringFindResult__ end end)()")>]
let findFrom (s: string) (pattern: string) (dir: Atom) : string option = nativeOnly

/// Checks if Prefix is a prefix of String.
/// Returns Some rest (String with Prefix stripped) or None if String does not start with Prefix.
[<Emit("(fun() -> case string:prefix($0, $1) of nomatch -> undefined; StringPrefixRest__ -> StringPrefixRest__ end end)()")>]
let prefix (s: string) (pre: string) : string option = nativeOnly

/// Splits String at the first occurrence of SearchPattern.
/// Returns an array of at most 2 parts.
[<Emit("fable_utils:new_ref(string:split($0, $1))")>]
let splitFirst (s: string) (pattern: string) : string array = nativeOnly

/// Splits String at all occurrences of SearchPattern.
/// Returns an array of all parts between (and around) occurrences.
[<Emit("fable_utils:new_ref(string:split($0, $1, all))")>]
let splitAll (s: string) (pattern: string) : string array = nativeOnly

// The OTP `string` module returns *chardata* (an iolist, or a charlist of codepoints) from these
// functions, not a binary. Each is exposed twice:
//   * the default (e.g. `pad`) flattens with `unicode:characters_to_binary/1` to an F# `string` —
//     what F# code wants, where the result is compared, stored, or pattern-matched;
//   * the `*Raw` variant (e.g. `padRaw`) returns the chardata as `BeamChardata`, unflattened — for
//     building BEAM output cheaply and handing it straight to io:format/gen_tcp/Cowboy.
// The default is a lie without the flatten: `string:pad("hi", 5)` is `[<<"hi">>,32,32,32]`, which
// compares unequal to <<"hi   ">>.

/// Reverses grapheme clusters in String (Unicode-aware, unlike binary:reverse).
[<Emit("unicode:characters_to_binary(string:reverse($0))")>]
let reverse (s: string) : string = nativeOnly

/// Like `reverse`, but returns the raw chardata without flattening. See `BeamChardata`.
[<Emit("string:reverse($0)")>]
let reverseRaw (s: string) : BeamChardata = nativeOnly

/// Pads String on the trailing side to at least Length grapheme clusters.
[<Emit("unicode:characters_to_binary(string:pad($0, $1))")>]
let pad (s: string) (length: int) : string = nativeOnly

/// Like `pad`, but returns the raw chardata without flattening. See `BeamChardata`.
[<Emit("string:pad($0, $1)")>]
let padRaw (s: string) (length: int) : BeamChardata = nativeOnly

/// Pads String on the given side Dir to at least Length grapheme clusters.
/// Dir must be the atom leading, trailing, or both (use erlang.binaryToAtom).
[<Emit("unicode:characters_to_binary(string:pad($0, $1, $2))")>]
let padDir (s: string) (length: int) (dir: Atom) : string = nativeOnly

/// Like `padDir`, but returns the raw chardata without flattening. See `BeamChardata`.
[<Emit("string:pad($0, $1, $2)")>]
let padDirRaw (s: string) (length: int) (dir: Atom) : BeamChardata = nativeOnly

/// Pads String on the given side Dir with the grapheme cluster Char.
/// Dir must be the atom leading, trailing, or both (use erlang.binaryToAtom).
[<Emit("unicode:characters_to_binary(string:pad($0, $1, $2, $3))")>]
let padWith (s: string) (length: int) (dir: Atom) (char: string) : string = nativeOnly

/// Like `padWith`, but returns the raw chardata without flattening. See `BeamChardata`.
[<Emit("string:pad($0, $1, $2, $3)")>]
let padWithRaw (s: string) (length: int) (dir: Atom) (char: string) : BeamChardata = nativeOnly

/// Replaces the first occurrence of SearchPattern in String with Replacement.
[<Emit("unicode:characters_to_binary(string:replace($0, $1, $2))")>]
let replaceFirst (s: string) (pattern: string) (replacement: string) : string = nativeOnly

/// Like `replaceFirst`, but returns the raw chardata without flattening. See `BeamChardata`.
[<Emit("string:replace($0, $1, $2)")>]
let replaceFirstRaw (s: string) (pattern: string) (replacement: string) : BeamChardata = nativeOnly

/// Replaces all occurrences of SearchPattern in String with Replacement.
[<Emit("unicode:characters_to_binary(string:replace($0, $1, $2, all))")>]
let replaceAll (s: string) (pattern: string) (replacement: string) : string = nativeOnly

/// Like `replaceAll`, but returns the raw chardata without flattening. See `BeamChardata`.
[<Emit("string:replace($0, $1, $2, all)")>]
let replaceAllRaw (s: string) (pattern: string) (replacement: string) : BeamChardata = nativeOnly

/// Parses an integer from the start of String.
/// Returns Ok (integer, rest) on success, or Error reason on failure.
[<Emit("(fun() -> case string:to_integer($0) of {error, StringToIntReason__} -> {error, atom_to_binary(StringToIntReason__)}; {StringToIntVal__, StringToIntRest__} -> {ok, {StringToIntVal__, StringToIntRest__}} end end)()")>]
let toInteger (s: string) : Result<int * string, string> = nativeOnly

/// Parses a float from the start of String.
/// Returns Ok (float, rest) on success, or Error reason on failure.
[<Emit("(fun() -> case string:to_float($0) of {error, StringToFloatReason__} -> {error, atom_to_binary(StringToFloatReason__)}; {StringToFloatVal__, StringToFloatRest__} -> {ok, {StringToFloatVal__, StringToFloatRest__}} end end)()")>]
let toFloat (s: string) : Result<float * string, string> = nativeOnly

/// Returns the grapheme clusters of String as an array.
/// `string:to_graphemes/1` yields codepoints (97) or codepoint lists for multi-codepoint clusters,
/// so each cluster is converted back to a binary to match the `string array` signature.
[<Emit("fable_utils:new_ref([unicode:characters_to_binary([StringGrapheme__]) || StringGrapheme__ <- string:to_graphemes($0)])")>]
let toGraphemes (s: string) : string array = nativeOnly
