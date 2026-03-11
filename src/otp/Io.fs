/// Type bindings for Erlang io module
/// See https://www.erlang.org/doc/apps/stdlib/io
module Fable.Beam.Io

open Fable.Core

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Writes a formatted string to standard output.
    abstract format: format: string * args: obj list -> unit
    /// Writes a formatted string to a device.
    abstract format: device: obj * format: string * args: obj list -> unit
    /// Reads a line from standard input.
    abstract get_line: prompt: string -> string
    /// Writes output to standard output.
    abstract put_chars: chars: string -> unit

/// io module
[<ImportAll("io")>]
let io: IExports = nativeOnly
