module Fable.Beam.Tests.Maps

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Beam.Maps
#endif

[<Fact>]
let ``test maps.new_ creates empty map`` () =
#if FABLE_COMPILER
    let m = maps.new_ ()
    maps.size m |> equal 0
#else
    ()
#endif

[<Fact>]
let ``test maps.put and get`` () =
#if FABLE_COMPILER
    let m = maps.new_ ()
    let m = maps.put (box "key", box "value", m)
    maps.get (box "key", m) |> equal (box "value")
#else
    ()
#endif

[<Fact>]
let ``test maps.is_key works`` () =
#if FABLE_COMPILER
    let m = maps.new_ ()
    let m = maps.put (box "a", box 1, m)
    maps.is_key (box "a", m) |> equal true
    maps.is_key (box "b", m) |> equal false
#else
    ()
#endif

[<Fact>]
let ``test maps.remove works`` () =
#if FABLE_COMPILER
    let m = maps.new_ ()
    let m = maps.put (box "a", box 1, m)
    let m = maps.remove (box "a", m)
    maps.size m |> equal 0
#else
    ()
#endif

[<Fact>]
let ``test maps.size works`` () =
#if FABLE_COMPILER
    let m = maps.new_ ()
    let m = maps.put (box "a", box 1, m)
    let m = maps.put (box "b", box 2, m)
    maps.size m |> equal 2
#else
    ()
#endif

[<Fact>]
let ``test maps.merge works`` () =
#if FABLE_COMPILER
    let m1 = maps.put (box "a", box 1, maps.new_ ())
    let m2 = maps.put (box "b", box 2, maps.new_ ())
    let merged = maps.merge (m1, m2)
    maps.size merged |> equal 2
#else
    ()
#endif

[<Fact>]
let ``test maps.find returns tagged result`` () =
#if FABLE_COMPILER
    let m = maps.put (box "key", box 42, maps.new_ ())
    // maps.find returns {ok, Value} or the atom error
    let result = maps.find (box "key", m)
    let notFound = maps.find (box "missing", m)
    // result should be {ok, 42}, notFound should be the atom error
    Fable.Beam.Erlang.exactEquals notFound (box (Fable.Beam.Erlang.binaryToAtom "error")) |> equal true
    Fable.Beam.Erlang.exactEquals result (box (Fable.Beam.Erlang.binaryToAtom "error")) |> equal false
#else
    ()
#endif
