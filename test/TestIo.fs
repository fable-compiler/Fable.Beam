module Fable.Beam.Tests.Io

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Beam.Io
#endif

[<Fact>]
let ``test io.put_chars works`` () =
#if FABLE_COMPILER
    io.put_chars "test output\n"
#else
    ()
#endif
