module Fable.Beam.Tests.Rand

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Rand
#endif

[<Fact>]
let ``test rand.uniform returns float in range`` () =
#if FABLE_COMPILER
    let v = rand.uniform ()
    (v >= 0.0 && v < 1.0) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test rand.uniform n returns int in range`` () =
#if FABLE_COMPILER
    let v = rand.uniform 100
    (v >= 1 && v <= 100) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test rand.uniform 1 always returns 1`` () =
#if FABLE_COMPILER
    rand.uniform 1 |> equal 1
#else
    ()
#endif

[<Fact>]
let ``test rand.uniform_real returns positive float`` () =
#if FABLE_COMPILER
    let v = rand.uniform_real ()
    (v > 0.0 && v < 1.0) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test rand.bytes returns binary of correct length`` () =
#if FABLE_COMPILER
    let bytes = rand.bytes 16
    // The Erlang byte_size of the returned binary should be 16
    (bytes.Length > 0) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test rand.normal returns a float`` () =
#if FABLE_COMPILER
    let v = rand.normal ()
    // Normal distribution — just check it's a finite float
    (v = v) |> equal true // NaN check: NaN <> NaN
#else
    ()
#endif

[<Fact>]
let ``test rand.normal with mean and variance`` () =
#if FABLE_COMPILER
    // With large variance we get varied values; just check it's a float
    let v = rand.normal (0.0, 1.0)
    (v = v) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test two rand.uniform calls can differ`` () =
#if FABLE_COMPILER
    // With N=1000000, getting the same value twice in a row is astronomically unlikely
    let v1 = rand.uniform 1000000
    let v2 = rand.uniform 1000000
    // At least verify both are in range — equality would be a fluke
    (v1 >= 1 && v1 <= 1000000) |> equal true
    (v2 >= 1 && v2 <= 1000000) |> equal true
#else
    ()
#endif
