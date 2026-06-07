module Fable.Beam.Tests.Logger

open Fable.Beam.Testing

open Fable.Core

#if FABLE_COMPILER
open Fable.Beam
open Fable.Beam.Logger
#endif

[<Fact>]
let ``test logger.info works`` () =
#if FABLE_COMPILER
    logger.info "test info message"
#else
    ()
#endif

[<Fact>]
let ``test logger.warning works`` () =
#if FABLE_COMPILER
    logger.warning "test warning message"
#else
    ()
#endif

[<Fact>]
let ``test logger.debug works`` () =
#if FABLE_COMPILER
    logger.debug "test debug message"
#else
    ()
#endif

[<Fact>]
let ``test logger.info with format args`` () =
#if FABLE_COMPILER
    // The 2-arg overload accepts both metadata maps and format args lists
    logger.info ("test ~p message", U2.Case2 [ box 42 ])
#else
    ()
#endif

[<Fact>]
let ``test logger add and remove handler`` () =
#if FABLE_COMPILER
    // Round-trip a handler through add_handler/3 and remove_handler/1, asserting the
    // ok | {error, term()} result is returned (not swallowed).
    let handlerId = Erlang.binaryToAtom "test_handler"
    let modle = Erlang.binaryToAtom "logger_std_h"
    let ok = box (Erlang.binaryToAtom "ok")

    let config =
        Maps.ofList [ (Erlang.binaryToAtom "level", box (Erlang.binaryToAtom "info")) ]

    logger.add_handler (handlerId, modle, box config) |> equal ok
    logger.remove_handler handlerId |> equal ok
#else
    ()
#endif
