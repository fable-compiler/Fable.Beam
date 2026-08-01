module Fable.Beam.Tests.Dynamic

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
#endif

[<Fact>]
let ``test Decode.int succeeds on integer`` () =
#if FABLE_COMPILER
    let d: Dynamic = emitErlExpr () "42"

    match Decode.int d with
    | Ok v -> v |> equal 42
    | Error e -> failwithf "expected Ok, got Error %s" e
#else
    ()
#endif

[<Fact>]
let ``test Decode.int fails on non-integer`` () =
#if FABLE_COMPILER
    let d: Dynamic = emitErlExpr () "<<\"hello\">>"

    match Decode.int d with
    | Ok _ -> failwith "expected Error"
    | Error _ -> ()
#else
    ()
#endif

[<Fact>]
let ``test Decode.string succeeds on binary`` () =
#if FABLE_COMPILER
    let d: Dynamic = emitErlExpr () "<<\"hello\">>"

    match Decode.string d with
    | Ok v -> v |> equal "hello"
    | Error e -> failwithf "expected Ok, got Error %s" e
#else
    ()
#endif

[<Fact>]
let ``test Decode.atom succeeds on atom`` () =
#if FABLE_COMPILER
    let d: Dynamic = emitErlExpr () "ok"

    match Decode.atom d with
    | Ok _ -> () // don't compare Atom values across the boundary, just ensure decode succeeded
    | Error e -> failwithf "expected Ok, got Error %s" e
#else
    ()
#endif

[<Fact>]
let ``test Decode.bool succeeds on true`` () =
#if FABLE_COMPILER
    let d: Dynamic = emitErlExpr () "true"

    match Decode.bool d with
    | Ok v -> v |> equal true
    | Error e -> failwithf "expected Ok, got Error %s" e
#else
    ()
#endif

[<Fact>]
let ``test Decode.field extracts map value`` () =
#if FABLE_COMPILER
    let d: Dynamic = emitErlExpr () "#{name => <<\"alice\">>, age => 30}"
    let nameKey = Erlang.binaryToAtom "name"
    let ageKey = Erlang.binaryToAtom "age"

    match Decode.field nameKey Decode.string d with
    | Ok name -> name |> equal "alice"
    | Error e -> failwithf "expected Ok name, got Error %s" e

    match Decode.field ageKey Decode.int d with
    | Ok age -> age |> equal 30
    | Error e -> failwithf "expected Ok age, got Error %s" e
#else
    ()
#endif

[<Fact>]
let ``test Decode.field errors on missing key`` () =
#if FABLE_COMPILER
    let d: Dynamic = emitErlExpr () "#{name => <<\"alice\">>}"
    let missingKey = Erlang.binaryToAtom "nonexistent"

    match Decode.field missingKey Decode.string d with
    | Ok _ -> failwith "expected Error on missing field"
    | Error _ -> ()
#else
    ()
#endif

[<Fact>]
let ``test Decode.list decodes homogeneous list`` () =
#if FABLE_COMPILER
    let d: Dynamic = emitErlExpr () "[1, 2, 3, 4]"

    match Decode.list Decode.int d with
    | Ok arr ->
        Array.length arr |> equal 4
        arr.[0] |> equal 1
        arr.[3] |> equal 4
    | Error e -> failwithf "expected Ok, got Error %s" e
#else
    ()
#endif

[<Fact>]
let ``test Decode.list short-circuits on first decode error`` () =
#if FABLE_COMPILER
    let d: Dynamic = emitErlExpr () "[1, 2, <<\"not_int\">>, 4]"

    match Decode.list Decode.int d with
    | Ok _ -> failwith "expected Error"
    | Error _ -> ()
#else
    ()
#endif

[<Fact>]
let ``test Decode.optional returns None for undefined`` () =
#if FABLE_COMPILER
    let d: Dynamic = emitErlExpr () "undefined"

    match Decode.optional Decode.int d with
    | Ok None -> ()
    | Ok(Some v) -> failwithf "expected None, got Some %d" v
    | Error e -> failwithf "expected Ok None, got Error %s" e
#else
    ()
#endif

[<Fact>]
let ``test Decode.optional returns Some for value`` () =
#if FABLE_COMPILER
    let d: Dynamic = emitErlExpr () "42"

    match Decode.optional Decode.int d with
    | Ok(Some v) -> v |> equal 42
    | Ok None -> failwith "expected Some"
    | Error e -> failwithf "expected Ok, got Error %s" e
#else
    ()
#endif

[<Fact>]
let ``test Decode.tuple2 decodes a pair`` () =
#if FABLE_COMPILER
    let d: Dynamic = emitErlExpr () "{<<\"alice\">>, 30}"

    match Decode.tuple2 Decode.string Decode.int d with
    | Ok(name, age) ->
        name |> equal "alice"
        age |> equal 30
    | Error e -> failwithf "expected Ok, got Error %s" e
#else
    ()
#endif

[<Fact>]
let ``test Decode.field with Atom.ofString key finds an atom-keyed field`` () =
#if FABLE_COMPILER
    // Regression for the documented decoder example: the key must be a real atom.
    // A binary key (what `Atom "name"` used to produce) never matches #{name => ...}.
    let d: Dynamic = emitErlExpr () "#{name => <<\"alice\">>, age => 30}"

    match Decode.field (Atom.ofString "name") Decode.string d with
    | Ok name -> name |> equal "alice"
    | Error e -> failwithf "expected Ok name, got Error %s" e
#else
    ()
#endif

[<Fact>]
let ``test Decode combinators accept a plain lambda decoder`` () =
#if FABLE_COMPILER
    // No System.Func wrapper: a single-argument F# function compiles to the
    // 1-arity Erlang fun the Emit applies.
    let d: Dynamic = emitErlExpr () "[1, 2, 3]"
    let doubled = fun (x: Dynamic) -> Decode.int x |> Result.map (fun n -> n * 2)

    match Decode.list doubled d with
    | Ok arr ->
        Array.length arr |> equal 3
        arr.[0] |> equal 2
        arr.[2] |> equal 6
    | Error e -> failwithf "expected Ok, got Error %s" e
#else
    ()
#endif
