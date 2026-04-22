/// Type bindings for Erlang re module (regular expressions, PCRE-compatible)
/// See https://www.erlang.org/doc/apps/stdlib/re
module Fable.Beam.Re

open Fable.Core

// ============================================================================
// Types
// ============================================================================

/// An opaque compiled regular expression produced by Re.compile.
/// Reusing a compiled pattern avoids re-parsing on every call.
[<Erase>]
type MP = | MP of obj

// ============================================================================
// Pattern compilation
// ============================================================================

/// Compiles a regex Pattern.
/// Returns Ok MP on success, or Error message if the pattern is invalid.
[<Emit("(fun() -> case re:compile($0) of {ok, ReCompileMP__} -> {ok, ReCompileMP__}; {error, {ReCompileErr__, _}} -> {error, iolist_to_binary(ReCompileErr__)} end end)()")>]
let compile (pattern: string) : Result<MP, string> = nativeOnly

// ============================================================================
// Matching
// ============================================================================

/// Returns true if Subject contains a match for the regex Pattern.
[<Emit("(fun() -> case re:run($0, $1) of nomatch -> false; _ -> true end end)()")>]
let isMatch (subject: string) (pattern: string) : bool = nativeOnly

/// Returns true if Subject contains a match for the compiled pattern MP.
[<Emit("(fun() -> case re:run($0, $1) of nomatch -> false; _ -> true end end)()")>]
let isMatchMP (subject: string) (mp: MP) : bool = nativeOnly

/// Runs Pattern against Subject, capturing all groups as binary strings.
/// Returns Some array of captures (index 0 is the whole match, 1..N are groups),
/// or None if Subject does not match.
[<Emit("(fun() -> case re:run($0, $1, [{capture, all, binary}]) of nomatch -> undefined; {match, ReRunCaptures__} -> fable_utils:new_ref(ReRunCaptures__) end end)()")>]
let run (subject: string) (pattern: string) : string array option = nativeOnly

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

/// Replaces all matches of Pattern in Subject with Replacement.
/// In Replacement, & refers to the whole match; \1..\9 refer to capture groups.
[<Emit("re:replace($0, $1, $2, [global, {return, binary}])")>]
let replaceAll (subject: string) (pattern: string) (replacement: string) : string = nativeOnly

// ============================================================================
// Split
// ============================================================================

/// Splits Subject at each match of Pattern. Returns an array of substrings.
[<Emit("fable_utils:new_ref(re:split($0, $1, [{return, binary}]))")>]
let split (subject: string) (pattern: string) : string array = nativeOnly

/// Splits Subject at matches of Pattern, returning at most Parts substrings.
/// The last element contains the remainder of the string.
[<Emit("fable_utils:new_ref(re:split($0, $1, [{return, binary}, {parts, $2}]))")>]
let splitParts (subject: string) (pattern: string) (parts: int) : string array = nativeOnly
