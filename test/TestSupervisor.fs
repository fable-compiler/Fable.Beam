module Fable.Beam.Tests.Supervisor

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Beam.Supervisor
#endif

[<Fact>]
let ``test supervisor.which_children on non-existent catches error`` () =
#if FABLE_COMPILER
    try
        supervisor.which_children (box "nonexistent_sup_xyz") |> ignore
    with
    | _ -> ()
#else
    ()
#endif
