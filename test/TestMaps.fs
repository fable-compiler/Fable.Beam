module Fable.Beam.Tests.Maps

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Beam.Lists

module BMaps = Fable.Beam.Maps

[<Emit("length($0)")>]
let private listLen (xs: BeamList<'T>) : int = nativeOnly

let tests =
    testList (
        "Maps",
        [ test (
              "new_ creates an empty map",
              fun _ ->
                  let m: BMaps.BeamMap<string, int> = BMaps.empty ()
                  assertThat (BMaps.size m) (isEqualTo 0)
          )

          test (
              "put and get round-trip",
              fun _ ->
                  let m: BMaps.BeamMap<string, string> = BMaps.empty ()
                  let m = BMaps.put "key" "value" m
                  assertThat (BMaps.get "key" m) (isEqualTo "value")
          )

          test (
              "is_key works",
              fun _ ->
                  let m: BMaps.BeamMap<string, int> = BMaps.empty ()
                  let m = BMaps.put "a" 1 m
                  assertThat (BMaps.containsKey "a" m) (isEqualTo true)
                  assertThat (BMaps.containsKey "b" m) (isEqualTo false)
          )

          test (
              "remove works",
              fun _ ->
                  let m: BMaps.BeamMap<string, int> = BMaps.empty ()
                  let m = BMaps.put "a" 1 m
                  let m = BMaps.remove "a" m
                  assertThat (BMaps.size m) (isEqualTo 0)
          )

          test (
              "size works",
              fun _ ->
                  let m: BMaps.BeamMap<string, int> = BMaps.empty ()
                  let m = BMaps.put "a" 1 m
                  let m = BMaps.put "b" 2 m
                  assertThat (BMaps.size m) (isEqualTo 2)
          )

          test (
              "merge works",
              fun _ ->
                  let m1: BMaps.BeamMap<string, int> = BMaps.put "a" 1 (BMaps.empty ())
                  let m2 = BMaps.put "b" 2 (BMaps.empty ())
                  let merged = BMaps.merge m1 m2
                  assertThat (BMaps.size merged) (isEqualTo 2)
          )

          test (
              "keys and values",
              fun _ ->
                  let m: BMaps.BeamMap<string, int> = BMaps.empty ()
                  let m = BMaps.put "a" 1 m
                  let m = BMaps.put "b" 2 m
                  assertThat (BMaps.keys m |> Array.length) (isEqualTo 2)
                  assertThat (BMaps.values m |> Array.length) (isEqualTo 2)
          )

          test (
              "get with default",
              fun _ ->
                  let m: BMaps.BeamMap<string, int> = BMaps.empty ()
                  assertThat (BMaps.getOrDefault "missing" m 42) (isEqualTo 42)
          )

          test (
              "to_list and from_list",
              fun _ ->
                  let m: BMaps.BeamMap<string, int> = BMaps.empty ()
                  let m = BMaps.put "a" 1 m
                  let lst = BMaps.toArray m
                  assertThat (Array.length lst) (isEqualTo 1)
                  let m2 = BMaps.ofArray lst
                  assertThat (BMaps.size m2) (isEqualTo 1)
          )

          test (
              "tryFind returns Some for existing key",
              fun _ ->
                  let m: BMaps.BeamMap<string, int> = BMaps.put "x" 99 (BMaps.empty ())
                  assertThat (BMaps.tryFind "x" m) (isEqualTo (Some 99))
          )

          test (
              "tryFind returns None for missing key",
              fun _ ->
                  let m: BMaps.BeamMap<string, int> = BMaps.empty ()
                  assertThat (BMaps.tryFind "missing" m) (isEqualTo None)
          )

          test (
              "ofList builds a map from a literal list",
              fun _ ->
                  let headers: BMaps.BeamMap<string, string> =
                      BMaps.ofList [ "content-type", "text/html"; "server", "cowboy" ]

                  assertThat (BMaps.size headers) (isEqualTo 2)
                  assertThat (BMaps.get "content-type" headers) (isEqualTo "text/html")
                  assertThat (BMaps.tryFind "server" headers) (isEqualTo (Some "cowboy"))
          )

          test (
              "keysRaw and valuesRaw return native lists matching keys and values",
              fun _ ->
                  let m: BMaps.BeamMap<string, int> = BMaps.ofList [ "a", 1; "b", 2; "c", 3 ]
                  // native lists carry the same data as the array-returning members, without the ref-wrap
                  assertThat (BMaps.keysRaw m |> listLen) (isEqualTo (BMaps.keys m |> Array.length))
                  assertThat (BMaps.valuesRaw m |> listLen) (isEqualTo (BMaps.values m |> Array.length))
                  assertThat (BMaps.keysRaw m |> listLen) (isEqualTo 3)
          )

          test (
              "toListRaw returns native list of pairs",
              fun _ ->
                  let m: BMaps.BeamMap<string, int> = BMaps.ofList [ "a", 1; "b", 2 ]
                  assertThat (BMaps.toListRaw m |> listLen) (isEqualTo 2)
          )

          test (
              "maps.fold accumulates over key-value pairs",
              fun _ ->
                  // maps:fold/3 applies F(K, V, Acc) — the only 3-arity callback in the bindings.
                  let m: BMaps.BeamMap<string, int> = BMaps.ofList [ ("a", 1); ("b", 2); ("c", 3) ]
                  assertThat (BMaps.fold (fun _k v acc -> v + acc) 0 m) (isEqualTo 6)
          )

          test (
              "maps.map transforms each value",
              fun _ ->
                  let m: BMaps.BeamMap<string, int> = BMaps.ofList [ ("a", 1); ("b", 2) ]
                  let doubled = BMaps.map (fun _k v -> v * 2) m
                  assertThat (BMaps.get "a" doubled) (isEqualTo 2)
                  assertThat (BMaps.get "b" doubled) (isEqualTo 4)
          )

          test (
              "maps.filter keeps matching pairs",
              fun _ ->
                  let m: BMaps.BeamMap<string, int> = BMaps.ofList [ ("a", 1); ("b", 2); ("c", 3) ]
                  let big = BMaps.filter (fun _k v -> v > 1) m
                  assertThat (BMaps.size big) (isEqualTo 2)
                  assertThat (BMaps.containsKey "a" big) (isEqualTo false)
          ) ]
    )
