module Fable.Beam.Tests.Maps

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Beam.Lists
open Fable.Beam.Maps

[<Emit("length($0)")>]
let private listLen (xs: BeamList<'T>) : int = nativeOnly

let tests =
    testList (
        "Maps",
        [ test (
              "new_ creates an empty map",
              fun _ ->
                  let m: BeamMap<string, int> = empty ()
                  assertThat (size m) (isEqualTo 0)
          )

          test (
              "put and get round-trip",
              fun _ ->
                  let m: BeamMap<string, string> = empty ()
                  let m = put "key" "value" m
                  assertThat (get "key" m) (isEqualTo "value")
          )

          test (
              "is_key works",
              fun _ ->
                  let m: BeamMap<string, int> = empty ()
                  let m = put "a" 1 m
                  assertThat (containsKey "a" m) (isEqualTo true)
                  assertThat (containsKey "b" m) (isEqualTo false)
          )

          test (
              "remove works",
              fun _ ->
                  let m: BeamMap<string, int> = empty ()
                  let m = put "a" 1 m
                  let m = remove "a" m
                  assertThat (size m) (isEqualTo 0)
          )

          test (
              "size works",
              fun _ ->
                  let m: BeamMap<string, int> = empty ()
                  let m = put "a" 1 m
                  let m = put "b" 2 m
                  assertThat (size m) (isEqualTo 2)
          )

          test (
              "merge works",
              fun _ ->
                  let m1: BeamMap<string, int> = put "a" 1 (empty ())
                  let m2 = put "b" 2 (empty ())
                  let merged = merge m1 m2
                  assertThat (size merged) (isEqualTo 2)
          )

          test (
              "keys and values",
              fun _ ->
                  let m: BeamMap<string, int> = empty ()
                  let m = put "a" 1 m
                  let m = put "b" 2 m
                  assertThat (keys m |> Array.length) (isEqualTo 2)
                  assertThat (values m |> Array.length) (isEqualTo 2)
          )

          test (
              "get with default",
              fun _ ->
                  let m: BeamMap<string, int> = empty ()
                  assertThat (getOrDefault "missing" m 42) (isEqualTo 42)
          )

          test (
              "to_list and from_list",
              fun _ ->
                  let m: BeamMap<string, int> = empty ()
                  let m = put "a" 1 m
                  let lst = toArray m
                  assertThat (Array.length lst) (isEqualTo 1)
                  let m2 = ofArray lst
                  assertThat (size m2) (isEqualTo 1)
          )

          test (
              "tryFind returns Some for existing key",
              fun _ ->
                  let m: BeamMap<string, int> = put "x" 99 (empty ())
                  assertThat (tryFind "x" m) (isEqualTo (Some 99))
          )

          test (
              "tryFind returns None for missing key",
              fun _ ->
                  let m: BeamMap<string, int> = empty ()
                  assertThat (tryFind "missing" m) (isEqualTo None)
          )

          test (
              "ofList builds a map from a literal list",
              fun _ ->
                  let headers: BeamMap<string, string> =
                      ofList [ "content-type", "text/html"; "server", "cowboy" ]

                  assertThat (size headers) (isEqualTo 2)
                  assertThat (get "content-type" headers) (isEqualTo "text/html")
                  assertThat (tryFind "server" headers) (isEqualTo (Some "cowboy"))
          )

          test (
              "keysRaw and valuesRaw return native lists matching keys and values",
              fun _ ->
                  let m: BeamMap<string, int> = ofList [ "a", 1; "b", 2; "c", 3 ]
                  // native lists carry the same data as the array-returning members, without the ref-wrap
                  assertThat (keysRaw m |> listLen) (isEqualTo (keys m |> Array.length))
                  assertThat (valuesRaw m |> listLen) (isEqualTo (values m |> Array.length))
                  assertThat (keysRaw m |> listLen) (isEqualTo 3)
          )

          test (
              "toListRaw returns native list of pairs",
              fun _ ->
                  let m: BeamMap<string, int> = ofList [ "a", 1; "b", 2 ]
                  assertThat (toListRaw m |> listLen) (isEqualTo 2)
          )

          test (
              "maps.fold accumulates over key-value pairs",
              fun _ ->
                  // maps:fold/3 applies F(K, V, Acc) — the only 3-arity callback in the bindings.
                  let m: BeamMap<string, int> = ofList [ ("a", 1); ("b", 2); ("c", 3) ]
                  assertThat (fold (fun _k v acc -> v + acc) 0 m) (isEqualTo 6)
          )

          test (
              "maps.map transforms each value",
              fun _ ->
                  let m: BeamMap<string, int> = ofList [ ("a", 1); ("b", 2) ]
                  let doubled = map (fun _k v -> v * 2) m
                  assertThat (get "a" doubled) (isEqualTo 2)
                  assertThat (get "b" doubled) (isEqualTo 4)
          )

          test (
              "maps.filter keeps matching pairs",
              fun _ ->
                  let m: BeamMap<string, int> = ofList [ ("a", 1); ("b", 2); ("c", 3) ]
                  let big = filter (fun _k v -> v > 1) m
                  assertThat (size big) (isEqualTo 2)
                  assertThat (containsKey "a" big) (isEqualTo false)
          ) ]
    )
