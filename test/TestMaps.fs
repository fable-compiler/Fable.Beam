module Fable.Beam.Tests.Maps

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Beam.Maps
#endif

[<Fact>]
let ``test maps.new_ creates empty map`` () =
#if FABLE_COMPILER
    let m: BeamMap<string, int> = maps.new_ ()
    maps.size m |> equal 0
#else
    ()
#endif

[<Fact>]
let ``test maps.put and get`` () =
#if FABLE_COMPILER
    let m: BeamMap<string, string> = maps.new_ ()
    let m = maps.put ("key", "value", m)
    maps.get ("key", m) |> equal "value"
#else
    ()
#endif

[<Fact>]
let ``test maps.is_key works`` () =
#if FABLE_COMPILER
    let m: BeamMap<string, int> = maps.new_ ()
    let m = maps.put ("a", 1, m)
    maps.is_key ("a", m) |> equal true
    maps.is_key ("b", m) |> equal false
#else
    ()
#endif

[<Fact>]
let ``test maps.remove works`` () =
#if FABLE_COMPILER
    let m: BeamMap<string, int> = maps.new_ ()
    let m = maps.put ("a", 1, m)
    let m = maps.remove ("a", m)
    maps.size m |> equal 0
#else
    ()
#endif

[<Fact>]
let ``test maps.size works`` () =
#if FABLE_COMPILER
    let m: BeamMap<string, int> = maps.new_ ()
    let m = maps.put ("a", 1, m)
    let m = maps.put ("b", 2, m)
    maps.size m |> equal 2
#else
    ()
#endif

[<Fact>]
let ``test maps.merge works`` () =
#if FABLE_COMPILER
    let m1: BeamMap<string, int> = maps.put ("a", 1, maps.new_ ())
    let m2 = maps.put ("b", 2, maps.new_ ())
    let merged = maps.merge (m1, m2)
    maps.size merged |> equal 2
#else
    ()
#endif

[<Fact>]
let ``test maps.keys and values`` () =
#if FABLE_COMPILER
    let m: BeamMap<string, int> = maps.new_ ()
    let m = maps.put ("a", 1, m)
    let m = maps.put ("b", 2, m)
    maps.keys m |> Array.length |> equal 2
    maps.values m |> Array.length |> equal 2
#else
    ()
#endif

[<Fact>]
let ``test maps.get with default`` () =
#if FABLE_COMPILER
    let m: BeamMap<string, int> = maps.new_ ()
    maps.get ("missing", m, 42) |> equal 42
#else
    ()
#endif

[<Fact>]
let ``test maps.to_list and from_list`` () =
#if FABLE_COMPILER
    let m: BeamMap<string, int> = maps.new_ ()
    let m = maps.put ("a", 1, m)
    let lst = maps.to_list m
    Array.length lst |> equal 1
    let m2 = maps.from_list lst
    maps.size m2 |> equal 1
#else
    ()
#endif
