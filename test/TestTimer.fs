module Fable.Beam.Tests.Timer

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Beam.Timer
#endif

[<Fact>]
let ``test timer.hours converts correctly`` () =
#if FABLE_COMPILER
    timer.hours 1 |> equal 3600000
#else
    ()
#endif

[<Fact>]
let ``test timer.minutes converts correctly`` () =
#if FABLE_COMPILER
    timer.minutes 1 |> equal 60000
#else
    ()
#endif

[<Fact>]
let ``test timer.seconds converts correctly`` () =
#if FABLE_COMPILER
    timer.seconds 1 |> equal 1000
#else
    ()
#endif

[<Fact>]
let ``test timer.sleep works`` () =
#if FABLE_COMPILER
    timer.sleep 10
#else
    ()
#endif
