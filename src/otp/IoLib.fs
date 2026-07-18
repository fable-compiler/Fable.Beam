/// Type bindings for Erlang io_lib module (formatting to chardata, not to a device)
/// See https://www.erlang.org/doc/apps/stdlib/io_lib
module Fable.Beam.IoLib

open Fable.Core
open Fable.Beam

// fsharplint:disable MemberNames

// `io_lib:format/2` builds output the same way `io:format/2` does, but *returns* it as chardata
// (a deep iolist) instead of writing it to a device. That makes it the canonical way to build a
// string from a format template. The result is chardata, so — like the `string` module — it comes
// in two forms (see "Dual API" in BINDINGS-GUIDE.md): a flattened `string` default, and a `*Raw`
// variant that hands back the unflattened `BeamChardata` for cheap, passable BEAM output.

/// Formats Args per the Erlang format string (io:format syntax, e.g. "~s ~p~n"),
/// returning the result as a string.
[<Emit("unicode:characters_to_binary(io_lib:format($0, $1))")>]
let format (fmt: string) (args: obj list) : string = nativeOnly

/// Like `format`, but returns the raw chardata without flattening. See `BeamChardata`.
[<Emit("io_lib:format($0, $1)")>]
let formatRaw (fmt: string) (args: obj list) : BeamChardata = nativeOnly
