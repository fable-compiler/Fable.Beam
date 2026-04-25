/// Fable bindings for the jsx JSON library.
/// See: https://github.com/talentdeficit/jsx
module Fable.Beam.Jsx.Jsx

open Fable.Core

// fsharplint:disable MemberNames

// ============================================================================
// Options
// ============================================================================

/// Opaque jsx option passed to decode, encode, format, is_json, and is_term.
[<Erase>]
type JsxOpt = JsxOpt of obj

/// Return maps instead of proplists for JSON objects (default since jsx 3.0).
[<Emit("return_maps")>]
let returnMaps: JsxOpt = nativeOnly

/// Enable strict JSON parsing (no comments, no trailing commas).
[<Emit("strict")>]
let strict: JsxOpt = nativeOnly

/// Enable streaming mode for incremental parsing.
[<Emit("stream")>]
let stream: JsxOpt = nativeOnly

/// Escape unicode characters to \\uXXXX sequences.
[<Emit("uescape")>]
let uescape: JsxOpt = nativeOnly

/// How JSON object keys are decoded. Cases compile to Erlang atoms.
[<RequireQualifiedAccess>]
type LabelMode =
    /// Decode keys as UTF-8 binaries (the jsx default).
    | Binary
    /// Decode keys as atoms; fails if a key cannot be converted.
    | Atom
    /// Decode keys as existing atoms; fails if a key is not already an atom in the VM.
    | ExistingAtom
    /// Decode keys as atoms when possible, falling back to binaries otherwise.
    | AttemptAtom

/// Control how JSON object keys are decoded.
[<Emit("{labels, $0}")>]
let labels (mode: LabelMode) : JsxOpt = nativeOnly

/// Set the number of spaces for indentation when formatting.
[<Emit("{indent, $0}")>]
let indent (n: int) : JsxOpt = nativeOnly

/// Set the number of spaces after colon/comma when formatting.
[<Emit("{space, $0}")>]
let space (n: int) : JsxOpt = nativeOnly

// ============================================================================
// Module binding
// ============================================================================

[<Erase>]
type IExports =
    /// Decode a UTF-8 JSON binary into Erlang terms.
    abstract decode: json: string -> 'T
    /// Decode a UTF-8 JSON binary into Erlang terms with options.
    abstract decode: json: string * opts: JsxOpt list -> 'T
    /// Encode an Erlang term into a UTF-8 JSON binary.
    abstract encode: term: 'T -> string
    /// Encode an Erlang term into a UTF-8 JSON binary with options.
    abstract encode: term: 'T * opts: JsxOpt list -> string
    /// Format a JSON binary.
    abstract format: json: string -> string
    /// Format a JSON binary with options.
    abstract format: json: string * opts: JsxOpt list -> string
    /// Minify a JSON binary (remove whitespace).
    abstract minify: json: string -> string
    /// Prettify a JSON binary (add readable formatting).
    abstract prettify: json: string -> string
    /// Check if a binary is valid JSON.
    abstract is_json: json: string -> bool
    /// Check if a binary is valid JSON with options.
    abstract is_json: json: string * opts: JsxOpt list -> bool
    /// Check if an Erlang term is a valid JSON-representable term.
    abstract is_term: term: 'T -> bool
    /// Check if an Erlang term is a valid JSON-representable term with options.
    abstract is_term: term: 'T * opts: JsxOpt list -> bool

/// jsx module
[<ImportAll("jsx")>]
let jsx: IExports = nativeOnly
