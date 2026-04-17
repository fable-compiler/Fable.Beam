module Fable.Beam.Tests.Math

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Math
#endif

[<Fact>]
let ``test math.pi returns pi`` () =
#if FABLE_COMPILER
    let pi = math.pi ()
    // pi is approximately 3.14159
    (pi > 3.14 && pi < 3.15) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test math.sin of zero is zero`` () =
#if FABLE_COMPILER
    math.sin 0.0 |> equal 0.0
#else
    ()
#endif

[<Fact>]
let ``test math.cos of zero is one`` () =
#if FABLE_COMPILER
    math.cos 0.0 |> equal 1.0
#else
    ()
#endif

[<Fact>]
let ``test math.sqrt of four is two`` () =
#if FABLE_COMPILER
    math.sqrt 4.0 |> equal 2.0
#else
    ()
#endif

[<Fact>]
let ``test math.pow computes power`` () =
#if FABLE_COMPILER
    math.pow (2.0, 10.0) |> equal 1024.0
#else
    ()
#endif

[<Fact>]
let ``test math.exp of zero is one`` () =
#if FABLE_COMPILER
    math.exp 0.0 |> equal 1.0
#else
    ()
#endif

[<Fact>]
let ``test math.log of e is one`` () =
#if FABLE_COMPILER
    let e = math.exp 1.0
    let result = math.log e
    // result should be approximately 1.0
    (result > 0.9999 && result < 1.0001) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test math.log2 of eight is three`` () =
#if FABLE_COMPILER
    let result = math.log2 8.0
    (result > 2.9999 && result < 3.0001) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test math.log10 of one hundred is two`` () =
#if FABLE_COMPILER
    let result = math.log10 100.0
    (result > 1.9999 && result < 2.0001) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test math.floor rounds down`` () =
#if FABLE_COMPILER
    math.floor 3.9 |> equal 3.0
#else
    ()
#endif

[<Fact>]
let ``test math.ceil rounds up`` () =
#if FABLE_COMPILER
    math.ceil 3.1 |> equal 4.0
#else
    ()
#endif

[<Fact>]
let ``test math.atan2 quadrant`` () =
#if FABLE_COMPILER
    // atan2(1, 1) = pi/4 approximately 0.785
    let result = math.atan2 (1.0, 1.0)
    (result > 0.78 && result < 0.79) |> equal true
#else
    ()
#endif
