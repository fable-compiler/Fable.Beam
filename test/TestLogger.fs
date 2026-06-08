module Fable.Beam.Tests.Logger

open Fable.Beam.Testing

open Fable.Core

#if FABLE_COMPILER
open Fable.Beam
open Fable.Beam.Maps
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
    // ok | {error, term()} result maps to Ok () (and is not swallowed).
    let handlerId = Erlang.binaryToAtom "test_handler"
    let modle = Erlang.binaryToAtom "logger_std_h"

    let config: BeamMap<Atom, obj> =
        Maps.ofList [ (Erlang.binaryToAtom "level", box (Erlang.binaryToAtom "info")) ]

    logger.add_handler (handlerId, modle, config) |> equal (Ok())
    logger.remove_handler handlerId |> equal (Ok())
#else
    ()
#endif

// ============================================================================
// Filter helpers
// ============================================================================

[<Fact>]
let ``test Filter.addPrimary receives the event and can stop it`` () =
#if FABLE_COMPILER
    // Primary filters run in the logging (client) process, so the filter can record
    // what it saw in this process's dictionary for us to assert on afterwards.
    let filterId = Erlang.binaryToAtom "fable_test_filter"
    let seenKey = Erlang.binaryToAtom "fable_test_seen_level"

    let filter =
        System.Func<_, _, _>(fun (ev: Filter.LogEvent) _extra ->
            // Exercise the accessors, then record the level and discard the event
            // (returning `stop` keeps test output clean).
            Filter.meta ev |> ignore
            Filter.msg ev |> ignore
            Erlang.put seenKey (Filter.level ev) |> ignore
            Filter.stop)

    Filter.addPrimary filterId filter (Erlang.binaryToAtom "ok") |> equal (Ok())

    // `error` is above the default primary level (`notice`), so it reaches the filter.
    logger.error "trigger for filter"

    match Erlang.get<Atom, Atom> seenKey with
    | Some lvl -> Erlang.exactEquals lvl (Erlang.binaryToAtom "error") |> equal true
    | None -> equal "filter saw the event" "filter did not run"

    Filter.removePrimary filterId |> equal (Ok())
#else
    ()
#endif

[<Fact>]
let ``test Filter.removePrimary on unknown id returns Error`` () =
#if FABLE_COMPILER
    match Filter.removePrimary (Erlang.binaryToAtom "fable_test_no_such_filter") with
    | Error _ -> equal true true
    | Ok() -> equal "Error" "Ok"
#else
    ()
#endif

// ============================================================================
// Formatter helpers
// ============================================================================

[<Fact>]
let ``test Formatter.setTemplate updates a handler's formatter`` () =
#if FABLE_COMPILER
    let handlerId = Erlang.binaryToAtom "fable_test_fmt_handler"
    let modle = Erlang.binaryToAtom "logger_std_h"

    let config: BeamMap<Atom, obj> =
        Maps.ofList [ (Erlang.binaryToAtom "level", box (Erlang.binaryToAtom "info")) ]

    logger.add_handler (handlerId, modle, config) |> equal (Ok())

    // Compact template exercising key / text / cond template items.
    let sp = Formatter.text " "

    let template =
        [ Formatter.key (Erlang.binaryToAtom "time")
          sp
          Formatter.key (Erlang.binaryToAtom "level")
          Formatter.text ": "
          Formatter.cond
              (Erlang.binaryToAtom "pid")
              [ Formatter.text "["
                Formatter.key (Erlang.binaryToAtom "pid")
                Formatter.text "] " ]
              []
          Formatter.key (Erlang.binaryToAtom "msg")
          Formatter.text "\n" ]

    Formatter.setTemplate handlerId true template |> equal (Ok())

    logger.remove_handler handlerId |> equal (Ok())
#else
    ()
#endif
