module Fable.Beam.Tests.IoLib

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Beam
open Fable.Beam.IoLib

// The raw variant returns unflattened chardata (a deep iolist), i.e. a list, not a binary.
[<Emit("is_list($0)")>]
let private isList (x: BeamChardata) : bool = nativeOnly
#endif

[<Fact>]
let ``test io_lib format renders a string`` () =
#if FABLE_COMPILER
    format "~s-~p" [ box "x"; box 42 ] |> equal "x-42"
#else
    ()
#endif

[<Fact>]
let ``test io_lib formatRaw returns unflattened chardata that flattens to format`` () =
#if FABLE_COMPILER
    let raw = formatRaw "~s-~p" [ box "x"; box 42 ]
    isList raw |> equal true
    BeamChardata.toString raw |> equal "x-42"
    BeamChardata.toString raw |> equal (format "~s-~p" [ box "x"; box 42 ])
#else
    ()
#endif
