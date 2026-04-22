/// Type bindings for Erlang re module (regular expressions, PCRE-compatible)
/// See https://www.erlang.org/doc/apps/stdlib/re
///
/// Note on encoding: F# strings compile to Erlang binaries (UTF-8). Erlang's `re`
/// defaults to Latin-1 (byte-oriented) matching. Literal-byte patterns still work,
/// but character classes like `.`, `\w`, and anchors operate byte-by-byte. For
/// UTF-8 correctness with multi-byte characters, pass the `unicode` flag via the
/// *With variants, or use an inline PCRE modifier such as `(*UTF)` in the pattern.
module Fable.Beam.Re

open Fable.Core
open Fable.Beam

// ============================================================================
// Types
// ============================================================================

/// An opaque compiled regular expression produced by Re.compile.
/// Reusing a compiled pattern avoids re-parsing on every call.
[<Erase>]
type MP = MP of obj

// ============================================================================
// Option flags
// ----------------------------------------------------------------------------
// Pass a list of these atoms to any *With function. See the `re` module docs
// for the full set of supported options; the flags below cover the common ones.
// ============================================================================

/// Case-insensitive matching.
[<Emit("caseless")>]
let caseless: Atom = nativeOnly

/// ^ and $ match line boundaries as well as subject boundaries.
[<Emit("multiline")>]
let multiline: Atom = nativeOnly

/// . matches newline characters (normally it does not).
[<Emit("dotall")>]
let dotall: Atom = nativeOnly

/// Ignore unescaped whitespace and # comments in the pattern.
[<Emit("extended")>]
let extended: Atom = nativeOnly

/// The pattern only matches at the start of the subject.
[<Emit("anchored")>]
let anchored: Atom = nativeOnly

/// Invert greediness of quantifiers: `*` becomes lazy, `*?` becomes greedy.
[<Emit("ungreedy")>]
let ungreedy: Atom = nativeOnly

/// Treat subject and pattern as UTF-8. Required for `.`, `\w`, and anchors
/// to work on multi-byte characters.
[<Emit("unicode")>]
let unicode: Atom = nativeOnly

// ============================================================================
// Pattern compilation
// ============================================================================

/// Compiles a regex Pattern.
/// Returns Ok MP on success, or Error message if the pattern is invalid.
[<Emit("(fun() -> case re:compile($0) of {ok, ReCompileMP__} -> {ok, ReCompileMP__}; {error, {ReCompileErr__, _}} -> {error, iolist_to_binary(ReCompileErr__)} end end)()")>]
let compile (pattern: string) : Result<MP, string> = nativeOnly

/// Compiles a regex Pattern with the given options (e.g. `[caseless; unicode]`).
/// Returns Ok MP on success, or Error message if the pattern is invalid.
[<Emit("(fun() -> case re:compile($0, $1) of {ok, ReCompileMP__} -> {ok, ReCompileMP__}; {error, {ReCompileErr__, _}} -> {error, iolist_to_binary(ReCompileErr__)} end end)()")>]
let compileWith (pattern: string) (options: Atom list) : Result<MP, string> = nativeOnly

// ============================================================================
// Matching
// ============================================================================

/// Returns true if Subject contains a match for the regex Pattern.
[<Emit("(fun() -> case re:run($0, $1) of nomatch -> false; _ -> true end end)()")>]
let isMatch (subject: string) (pattern: string) : bool = nativeOnly

/// Returns true if Subject contains a match for Pattern with the given options.
[<Emit("(fun() -> case re:run($0, $1, $2) of nomatch -> false; _ -> true end end)()")>]
let isMatchWith (subject: string) (pattern: string) (options: Atom list) : bool = nativeOnly

/// Returns true if Subject contains a match for the compiled pattern MP.
[<Emit("(fun() -> case re:run($0, $1) of nomatch -> false; _ -> true end end)()")>]
let isMatchMP (subject: string) (mp: MP) : bool = nativeOnly

/// Runs Pattern against Subject, capturing all groups as binary strings.
/// Returns Some array of captures (index 0 is the whole match, 1..N are groups),
/// or None if Subject does not match.
[<Emit("(fun() -> case re:run($0, $1, [{capture, all, binary}]) of nomatch -> undefined; {match, ReRunCaptures__} -> fable_utils:new_ref(ReRunCaptures__) end end)()")>]
let run (subject: string) (pattern: string) : string array option = nativeOnly

/// Runs Pattern against Subject with options, capturing all groups.
[<Emit("(fun() -> case re:run($0, $1, [{capture, all, binary} | $2]) of nomatch -> undefined; {match, ReRunCaptures__} -> fable_utils:new_ref(ReRunCaptures__) end end)()")>]
let runWith (subject: string) (pattern: string) (options: Atom list) : string array option = nativeOnly

/// Runs a compiled pattern against Subject. Returns Some captures or None.
[<Emit("(fun() -> case re:run($0, $1, [{capture, all, binary}]) of nomatch -> undefined; {match, ReRunMPCaptures__} -> fable_utils:new_ref(ReRunMPCaptures__) end end)()")>]
let runMP (subject: string) (mp: MP) : string array option = nativeOnly

// ============================================================================
// Replace
// ============================================================================

/// Replaces the first match of Pattern in Subject with Replacement.
/// In Replacement, & refers to the whole match; \1..\9 refer to capture groups.
[<Emit("re:replace($0, $1, $2, [{return, binary}])")>]
let replaceFirst (subject: string) (pattern: string) (replacement: string) : string = nativeOnly

/// Replaces the first match of Pattern in Subject with Replacement, with options.
[<Emit("re:replace($0, $1, $2, [{return, binary} | $3])")>]
let replaceFirstWith (subject: string) (pattern: string) (replacement: string) (options: Atom list) : string =
    nativeOnly

/// Replaces the first match using a compiled pattern.
[<Emit("re:replace($0, $1, $2, [{return, binary}])")>]
let replaceFirstMP (subject: string) (mp: MP) (replacement: string) : string = nativeOnly

/// Replaces all matches of Pattern in Subject with Replacement.
/// In Replacement, & refers to the whole match; \1..\9 refer to capture groups.
[<Emit("re:replace($0, $1, $2, [global, {return, binary}])")>]
let replaceAll (subject: string) (pattern: string) (replacement: string) : string = nativeOnly

/// Replaces all matches of Pattern in Subject with Replacement, with options.
[<Emit("re:replace($0, $1, $2, [global, {return, binary} | $3])")>]
let replaceAllWith (subject: string) (pattern: string) (replacement: string) (options: Atom list) : string = nativeOnly

/// Replaces all matches using a compiled pattern.
[<Emit("re:replace($0, $1, $2, [global, {return, binary}])")>]
let replaceAllMP (subject: string) (mp: MP) (replacement: string) : string = nativeOnly

// ============================================================================
// Split
// ============================================================================

/// Splits Subject at each match of Pattern. Returns an array of substrings.
[<Emit("fable_utils:new_ref(re:split($0, $1, [{return, binary}]))")>]
let split (subject: string) (pattern: string) : string array = nativeOnly

/// Splits Subject at each match of Pattern, with options.
[<Emit("fable_utils:new_ref(re:split($0, $1, [{return, binary} | $2]))")>]
let splitWith (subject: string) (pattern: string) (options: Atom list) : string array = nativeOnly

/// Splits Subject at each match of a compiled pattern.
[<Emit("fable_utils:new_ref(re:split($0, $1, [{return, binary}]))")>]
let splitMP (subject: string) (mp: MP) : string array = nativeOnly

/// Splits Subject at matches of Pattern, returning at most Parts substrings.
/// The last element contains the remainder of the string.
[<Emit("fable_utils:new_ref(re:split($0, $1, [{return, binary}, {parts, $2}]))")>]
let splitParts (subject: string) (pattern: string) (parts: int) : string array = nativeOnly
