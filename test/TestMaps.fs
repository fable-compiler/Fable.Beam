module Fable.Beam.Tests.Maps

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Beam.Lists
open Fable.Beam.Maps

[<Emit("length($0)")>]
let private listLen (xs: BeamList<'T>) : int = nativeOnly
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

[<Fact>]
let ``test tryFind returns Some for existing key`` () =
#if FABLE_COMPILER
    let m: BeamMap<string, int> = maps.put ("x", 99, maps.new_ ())
    tryFind "x" m |> equal (Some 99)
#else
    ()
#endif

[<Fact>]
let ``test tryFind returns None for missing key`` () =
#if FABLE_COMPILER
    let m: BeamMap<string, int> = maps.new_ ()
    tryFind "missing" m |> equal None
#else
    ()
#endif

[<Fact>]
let ``test ofList builds a map from a literal list`` () =
#if FABLE_COMPILER
    let headers: BeamMap<string, string> =
        ofList [ "content-type", "text/html"; "server", "cowboy" ]

    maps.size headers |> equal 2
    maps.get ("content-type", headers) |> equal "text/html"
    tryFind "server" headers |> equal (Some "cowboy")
#else
    ()
#endif

[<Fact>]
let ``test keysRaw and valuesRaw return native lists matching keys and values`` () =
#if FABLE_COMPILER
    let m: BeamMap<string, int> = ofList [ "a", 1; "b", 2; "c", 3 ]
    // native lists carry the same data as the array-returning members, without the ref-wrap
    keysRaw m |> listLen |> equal (maps.keys m |> Array.length)
    valuesRaw m |> listLen |> equal (maps.values m |> Array.length)
    keysRaw m |> listLen |> equal 3
#else
    ()
#endif

[<Fact>]
let ``test toListRaw returns native list of pairs`` () =
#if FABLE_COMPILER
    let m: BeamMap<string, int> = ofList [ "a", 1; "b", 2 ]
    toListRaw m |> listLen |> equal 2
#else
    ()
#endif
