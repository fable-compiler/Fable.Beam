/// Type bindings for Erlang io module
/// See https://www.erlang.org/doc/apps/stdlib/io
module Fable.Beam.Io

open Fable.Core
open Fable.Beam.Erlang

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Writes a formatted string to standard output.
    abstract format: format: string * args: obj list -> unit
    /// Writes a formatted string to a device.
    abstract format: device: Pid * format: string * args: obj list -> unit
    /// Reads a line from standard input (raw — returns eof atom on EOF).
    abstract get_line: prompt: string -> string
    /// Writes output to standard output.
    abstract put_chars: chars: string -> unit

/// io module (raw bindings)
[<ImportAll("io")>]
let io: IExports = nativeOnly

// ============================================================================
// Typed API with eof handling
// ============================================================================
// WORKAROUND: Emit expressions wrapped in (fun() -> ... end)() to prevent
// Erlang "unsafe variable" errors. Remove IIFEs once fixed in Fable.

/// Reads a line from standard input. Returns None on EOF (Ctrl+D),
/// Some with the line (including trailing newline) otherwise.
/// Handles the eof atom that io:get_line returns on end-of-input.
[<Emit("(fun() -> case io:get_line($0) of eof -> undefined; IoGetLine__ -> erlang:list_to_binary(IoGetLine__) end end)()")>]
let getLine (prompt: string) : string option = nativeOnly

/// Writes a string to standard output.
[<Emit("io:put_chars($0)")>]
let putChars (s: string) : unit = nativeOnly

/// Writes a formatted string to standard output.
/// The format string uses Erlang io:format syntax (e.g., "~s ~p~n").
[<Emit("io:format($0, $1)")>]
let format (fmt: string) (args: obj list) : unit = nativeOnly

/// Set IO options (e.g., encoding).
[<Emit("io:setopts($0)")>]
let setopts (opts: obj) : unit = nativeOnly

/// Enable Unicode encoding on standard IO.
[<Emit("io:setopts([{encoding, unicode}])")>]
let setUnicodeEncoding () : unit = nativeOnly
