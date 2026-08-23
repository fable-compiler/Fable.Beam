module Fable.Beam.Tests.Dynamic

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam

let tests =
    testList (
        "Dynamic",
        [ test (
              "Decode.int succeeds on integer",
              fun _ ->
                  let d: Dynamic = emitErlExpr () "42"

                  match Decode.int d with
                  | Ok v -> assertThat v (isEqualTo 42)
                  | Error e -> failwithf "expected Ok, got Error %s" e
          )

          test (
              "Decode.int fails on non-integer",
              fun _ ->
                  let d: Dynamic = emitErlExpr () "<<\"hello\">>"

                  match Decode.int d with
                  | Ok _ -> failwith "expected Error"
                  | Error _ -> assertThat true (isTrue)
          )

          test (
              "Decode.string succeeds on binary",
              fun _ ->
                  let d: Dynamic = emitErlExpr () "<<\"hello\">>"

                  match Decode.string d with
                  | Ok v -> assertThat v (isEqualTo "hello")
                  | Error e -> failwithf "expected Ok, got Error %s" e
          )

          test (
              "Decode.atom succeeds on atom",
              fun _ ->
                  let d: Dynamic = emitErlExpr () "ok"

                  match Decode.atom d with
                  // don't compare Atom values across the boundary, just ensure decode succeeded
                  | Ok _ -> assertThat true (isTrue)
                  | Error e -> failwithf "expected Ok, got Error %s" e
          )

          test (
              "Decode.bool succeeds on true",
              fun _ ->
                  let d: Dynamic = emitErlExpr () "true"

                  match Decode.bool d with
                  | Ok v -> assertThat v (isEqualTo true)
                  | Error e -> failwithf "expected Ok, got Error %s" e
          )

          test (
              "Decode.field extracts map value",
              fun _ ->
                  let d: Dynamic = emitErlExpr () "#{name => <<\"alice\">>, age => 30}"
                  let nameKey = Erlang.binaryToAtom "name"
                  let ageKey = Erlang.binaryToAtom "age"

                  match Decode.field nameKey Decode.string d with
                  | Ok name -> assertThat name (isEqualTo "alice")
                  | Error e -> failwithf "expected Ok name, got Error %s" e

                  match Decode.field ageKey Decode.int d with
                  | Ok age -> assertThat age (isEqualTo 30)
                  | Error e -> failwithf "expected Ok age, got Error %s" e
          )

          test (
              "Decode.field errors on missing key",
              fun _ ->
                  let d: Dynamic = emitErlExpr () "#{name => <<\"alice\">>}"
                  let missingKey = Erlang.binaryToAtom "nonexistent"

                  match Decode.field missingKey Decode.string d with
                  | Ok _ -> failwith "expected Error on missing field"
                  | Error _ -> assertThat true (isTrue)
          )

          test (
              "Decode.list decodes homogeneous list",
              fun _ ->
                  let d: Dynamic = emitErlExpr () "[1, 2, 3, 4]"

                  match Decode.list Decode.int d with
                  | Ok arr ->
                      assertThat (Array.length arr) (isEqualTo 4)
                      assertThat arr.[0] (isEqualTo 1)
                      assertThat arr.[3] (isEqualTo 4)
                  | Error e -> failwithf "expected Ok, got Error %s" e
          )

          test (
              "Decode.list short-circuits on first decode error",
              fun _ ->
                  let d: Dynamic = emitErlExpr () "[1, 2, <<\"not_int\">>, 4]"

                  match Decode.list Decode.int d with
                  | Ok _ -> failwith "expected Error"
                  | Error _ -> assertThat true (isTrue)
          )

          test (
              "Decode.optional returns None for undefined",
              fun _ ->
                  let d: Dynamic = emitErlExpr () "undefined"

                  match Decode.optional Decode.int d with
                  | Ok None -> assertThat true (isTrue)
                  | Ok(Some v) -> failwithf "expected None, got Some %d" v
                  | Error e -> failwithf "expected Ok None, got Error %s" e
          )

          test (
              "Decode.optional returns Some for value",
              fun _ ->
                  let d: Dynamic = emitErlExpr () "42"

                  match Decode.optional Decode.int d with
                  | Ok(Some v) -> assertThat v (isEqualTo 42)
                  | Ok None -> failwith "expected Some"
                  | Error e -> failwithf "expected Ok, got Error %s" e
          )

          test (
              "Decode.tuple2 decodes a pair",
              fun _ ->
                  let d: Dynamic = emitErlExpr () "{<<\"alice\">>, 30}"

                  match Decode.tuple2 Decode.string Decode.int d with
                  | Ok(name, age) ->
                      assertThat name (isEqualTo "alice")
                      assertThat age (isEqualTo 30)
                  | Error e -> failwithf "expected Ok, got Error %s" e
          )

          test (
              "Decode.field with Atom.ofString key finds an atom-keyed field",
              fun _ ->
                  // Regression for the documented decoder example: the key must be a real atom.
                  // A binary key (what `Atom "name"` used to produce) never matches #{name => ...}.
                  let d: Dynamic = emitErlExpr () "#{name => <<\"alice\">>, age => 30}"

                  match Decode.field (Atom.ofString "name") Decode.string d with
                  | Ok name -> assertThat name (isEqualTo "alice")
                  | Error e -> failwithf "expected Ok name, got Error %s" e
          )

          test (
              "Decode combinators accept a plain lambda decoder",
              fun _ ->
                  // No System.Func wrapper: a single-argument F# function compiles to the
                  // 1-arity Erlang fun the Emit applies.
                  let d: Dynamic = emitErlExpr () "[1, 2, 3]"
                  let doubled = fun (x: Dynamic) -> Decode.int x |> Result.map (fun n -> n * 2)

                  match Decode.list doubled d with
                  | Ok arr ->
                      assertThat (Array.length arr) (isEqualTo 3)
                      assertThat arr.[0] (isEqualTo 2)
                      assertThat arr.[2] (isEqualTo 6)
                  | Error e -> failwithf "expected Ok, got Error %s" e
          ) ]
    )
