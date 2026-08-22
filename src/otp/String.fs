/// Type bindings for Erlang string module (OTP 20+ binary mode)
/// See https://www.erlang.org/doc/apps/stdlib/string
module Fable.Beam.String

open Fable.Core
open Fable.Beam
open Fable.Beam.Lists

/// Returns true if String contains no grapheme clusters, otherwise false.
[<Emit("string:is_empty($0)")>]
let isEmpty (s: string) : bool = nativeOnly

/// Returns the number of grapheme clusters in String.
/// Note: this is Unicode grapheme count, not byte length — use erlang.byteSize for bytes.
[<Emit("string:length($0)")>]
let length (s: string) : int = nativeOnly

/// Converts String to lowercase.
[<Emit("string:lowercase($0)")>]
let lowercase (s: string) : string = nativeOnly

/// Converts String to uppercase.
[<Emit("string:uppercase($0)")>]
let uppercase (s: string) : string = nativeOnly

/// Converts the first grapheme cluster of String to uppercase and the rest to lowercase.
[<Emit("string:titlecase($0)")>]
let titlecase (s: string) : string = nativeOnly

/// Converts String to a case-folded form suitable for case-insensitive comparisons.
[<Emit("string:casefold($0)")>]
let casefold (s: string) : string = nativeOnly

/// Returns a string slice from grapheme position Start to end of String.
[<Emit("string:slice($0, $1)")>]
let slice (s: string) (start: int) : string = nativeOnly

/// Returns a string slice of at most Length graphemes starting at position Start.
[<Emit("string:slice($0, $1, $2)")>]
let sliceLen (s: string) (start: int) (length: int) : string = nativeOnly

/// Strips leading and trailing Unicode whitespace.
[<Emit("string:trim($0)")>]
let trim (s: string) : string = nativeOnly

/// Strips leading Unicode whitespace.
[<Emit("string:trim($0, leading)")>]
let trimStart (s: string) : string = nativeOnly

/// Strips trailing Unicode whitespace.
[<Emit("string:trim($0, trailing)")>]
let trimEnd (s: string) : string = nativeOnly

/// Returns true if S1 and S2 are equal (ordinal).
[<Emit("string:equal($0, $1)")>]
let equal (s1: string) (s2: string) : bool = nativeOnly

/// Returns true if S1 and S2 are equal after Unicode case folding.
[<Emit("string:equal($0, $1, true)")>]
let equalCaseInsensitive (s1: string) (s2: string) : bool = nativeOnly

// NOTE: `string:concat/2` is bound to Erlang's `++` operator, which expects
// charlists — it raises `badarg` when called with binaries. Since F# strings
// compile to binaries, concatenate F# strings directly (for example, `s1 + s2`).

// NOTE: the (fun() -> ... end)() wrappers on the case Emits below are no longer
// required — Fable (>= 5.0.0) auto-wraps case-containing Emits for variable scoping.
// Kept for explicitness; safe to remove.

/// Searches for the first occurrence of SearchPattern in String, searching left to right.
/// Returns Some suffix (the tail of String from the match start) or None if not found.
[<Emit("(fun() -> case string:find($0, $1) of nomatch -> undefined; StringFindResult__ -> StringFindResult__ end end)()")>]
let find (s: string) (pattern: string) : string option = nativeOnly

/// Searches for the last occurrence of SearchPattern in String.
/// Returns Some suffix from the match start or None if not found.
[<Emit("(fun() -> case string:find($0, $1, trailing) of nomatch -> undefined; StringFindResult__ -> StringFindResult__ end end)()")>]
let findLast (s: string) (pattern: string) : string option = nativeOnly

/// Checks if Prefix is a prefix of String.
/// Returns Some rest (String with Prefix stripped) or None if String does not start with Prefix.
[<Emit("(fun() -> case string:prefix($0, $1) of nomatch -> undefined; StringPrefixRest__ -> StringPrefixRest__ end end)()")>]
let prefix (s: string) (pre: string) : string option = nativeOnly

/// Splits String at the first occurrence of SearchPattern.
/// Returns an array of at most 2 parts.
[<Emit("fable_utils:new_ref(string:split($0, $1))")>]
let splitFirst (s: string) (pattern: string) : string array = nativeOnly

/// Like `splitFirst`, but returns the native Erlang list instead of an F# array. See "Dual API".
[<Emit("string:split($0, $1)")>]
let splitFirstRaw (s: string) (pattern: string) : BeamList<string> = nativeOnly

/// Splits String at all occurrences of SearchPattern.
/// Returns an array of all parts between (and around) occurrences.
[<Emit("fable_utils:new_ref(string:split($0, $1, all))")>]
let splitAll (s: string) (pattern: string) : string array = nativeOnly

/// Like `splitAll`, but returns the native Erlang list instead of an F# array. See "Dual API".
[<Emit("string:split($0, $1, all)")>]
let splitAllRaw (s: string) (pattern: string) : BeamList<string> = nativeOnly

// The OTP `string` module returns *chardata* (an iolist, or a charlist of codepoints) from these
// functions, not a binary. Each is exposed twice:
//   * the default (e.g. `padEnd`) flattens with `unicode:characters_to_binary/1` to an F# `string` —
//     what F# code wants, where the result is compared, stored, or pattern-matched;
//   * the `*Raw` variant (e.g. `padEndRaw`) returns the chardata as `BeamChardata`, unflattened — for
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
let padEnd (s: string) (length: int) : string = nativeOnly

/// Like `padEnd`, but returns the raw chardata without flattening. See `BeamChardata`.
[<Emit("string:pad($0, $1)")>]
let padEndRaw (s: string) (length: int) : BeamChardata = nativeOnly

/// Pads String on the leading side to at least Length grapheme clusters.
[<Emit("unicode:characters_to_binary(string:pad($0, $1, leading))")>]
let padStart (s: string) (length: int) : string = nativeOnly

/// Like `padStart`, but returns the raw chardata without flattening. See `BeamChardata`.
[<Emit("string:pad($0, $1, leading)")>]
let padStartRaw (s: string) (length: int) : BeamChardata = nativeOnly

/// Pads both sides of String to at least Length grapheme clusters.
[<Emit("unicode:characters_to_binary(string:pad($0, $1, both))")>]
let padBoth (s: string) (length: int) : string = nativeOnly

/// Like `padBoth`, but returns the raw chardata without flattening. See `BeamChardata`.
[<Emit("string:pad($0, $1, both)")>]
let padBothRaw (s: string) (length: int) : BeamChardata = nativeOnly

/// Pads String on the trailing side with the grapheme cluster Char.
[<Emit("unicode:characters_to_binary(string:pad($0, $1, trailing, $2))")>]
let padEndWith (s: string) (length: int) (char: string) : string = nativeOnly

/// Like `padEndWith`, but returns the raw chardata without flattening. See `BeamChardata`.
[<Emit("string:pad($0, $1, trailing, $2)")>]
let padEndWithRaw (s: string) (length: int) (char: string) : BeamChardata = nativeOnly

/// Pads String on the leading side with the grapheme cluster Char.
[<Emit("unicode:characters_to_binary(string:pad($0, $1, leading, $2))")>]
let padStartWith (s: string) (length: int) (char: string) : string = nativeOnly

/// Like `padStartWith`, but returns the raw chardata without flattening. See `BeamChardata`.
[<Emit("string:pad($0, $1, leading, $2)")>]
let padStartWithRaw (s: string) (length: int) (char: string) : BeamChardata = nativeOnly

/// Pads both sides of String with the grapheme cluster Char.
[<Emit("unicode:characters_to_binary(string:pad($0, $1, both, $2))")>]
let padBothWith (s: string) (length: int) (char: string) : string = nativeOnly

/// Like `padBothWith`, but returns the raw chardata without flattening. See `BeamChardata`.
[<Emit("string:pad($0, $1, both, $2)")>]
let padBothWithRaw (s: string) (length: int) (char: string) : BeamChardata = nativeOnly

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
