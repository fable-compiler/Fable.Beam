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

    /// Reverses grapheme clusters in String (Unicode-aware, unlike binary:reverse).
    abstract reverse: s: string -> string

    /// Concatenates two strings. Equivalent to S1 ++ S2 in Erlang.
    abstract concat: s1: string * s2: string -> string

    /// Returns a string slice from grapheme position Start to end of String.
    abstract slice: s: string * start: int -> string

    /// Returns a string slice of at most Length graphemes starting at position Start.
    abstract slice: s: string * start: int * length: int -> string

    /// Strips leading and trailing whitespace (graphemes matching the Unicode "White_Space" property).
    abstract trim: s: string -> string

    /// Strips whitespace from the direction Dir of String.
    /// Dir must be the atom leading, trailing, or both (use erlang.binaryToAtom).
    abstract trim: s: string * dir: Atom -> string

    /// Strips Characters from the direction Dir of String.
    /// Dir must be the atom leading, trailing, or both. Characters is a string of chars to strip.
    abstract trim: s: string * dir: Atom * characters: string -> string

    /// Pads String on the trailing side to at least Length grapheme clusters.
    abstract pad: s: string * length: int -> string

    /// Pads String on the given side Dir to at least Length grapheme clusters.
    /// Dir must be the atom leading, trailing, or both (use erlang.binaryToAtom).
    abstract pad: s: string * length: int * dir: Atom -> string

    /// Pads String on the given side Dir with the grapheme cluster Char.
    abstract pad: s: string * length: int * dir: Atom * char: string -> string

    /// Returns true if S1 and S2 are equal (ordinal).
    abstract equal: s1: string * s2: string -> bool

    /// Returns true if S1 and S2 are equal; if IgnoreCase is true, the comparison is
    /// normalised to NFC before comparing (Unicode case-insensitive).
    abstract equal: s1: string * s2: string * ignoreCase: bool -> bool

/// string module
[<ImportAll("string")>]
let string: IExports = nativeOnly

// ============================================================================
// Typed API — functions with non-trivial Erlang return values
// ============================================================================
// WORKAROUND: Emit expressions wrapped in (fun() -> ... end)() to prevent
// Erlang "unsafe variable" errors.

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

/// Replaces the first occurrence of SearchPattern in String with Replacement.
[<Emit("string:replace($0, $1, $2)")>]
let replaceFirst (s: string) (pattern: string) (replacement: string) : string = nativeOnly

/// Replaces all occurrences of SearchPattern in String with Replacement.
[<Emit("string:replace($0, $1, $2, all)")>]
let replaceAll (s: string) (pattern: string) (replacement: string) : string = nativeOnly

/// Parses an integer from the start of String.
/// Returns Ok (integer, rest) on success, or Error reason on failure.
[<Emit("(fun() -> case string:to_integer($0) of {error, StringToIntReason__} -> {error, atom_to_binary(StringToIntReason__)}; {StringToIntVal__, StringToIntRest__} -> {ok, {StringToIntVal__, StringToIntRest__}} end end)()")>]
let toInteger (s: string) : Result<int * string, string> = nativeOnly

/// Parses a float from the start of String.
/// Returns Ok (float, rest) on success, or Error reason on failure.
[<Emit("(fun() -> case string:to_float($0) of {error, StringToFloatReason__} -> {error, atom_to_binary(StringToFloatReason__)}; {StringToFloatVal__, StringToFloatRest__} -> {ok, {StringToFloatVal__, StringToFloatRest__}} end end)()")>]
let toFloat (s: string) : Result<float * string, string> = nativeOnly

/// Returns the grapheme clusters of String as an array.
[<Emit("fable_utils:new_ref(string:to_graphemes($0))")>]
let toGraphemes (s: string) : string array = nativeOnly
