module Fable.Beam.Tests.GenServer

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Beam.GenServer
#endif

[<Fact>]
let ``test gen_server.stop on non-existent catches error`` () =
#if FABLE_COMPILER
    try
        gen_server.stop (box "nonexistent_process_xyz")
    with
    | _ -> ()
#else
    ()
#endif
