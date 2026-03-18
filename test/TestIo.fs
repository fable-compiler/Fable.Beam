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

[<Fact>]
let ``test putChars does not crash`` () =
#if FABLE_COMPILER
    putChars "typed putChars test\n"
#else
    ()
#endif

[<Fact>]
let ``test format does not crash`` () =
#if FABLE_COMPILER
    format "hello ~s~n" [box "beam"]
#else
    ()
#endif
