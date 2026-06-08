module Fable.Beam.Tests.Logger

open Fable.Beam.Testing

open Fable.Core

#if FABLE_COMPILER
open Fable.Core.BeamInterop
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
    let timeKey = Erlang.binaryToAtom "fable_test_seen_time"

    let filter =
        System.Func<_, _, _>(fun (ev: Filter.LogEvent) _extra ->
            // Exercise the accessors, then record what we saw and discard the event
            // (returning `stop` keeps test output clean). `meta` values are Dynamic;
            // OTP always stamps a `time` (system time in microseconds) onto the event.
            Filter.msg ev |> ignore
            let m = Filter.meta ev
            Erlang.put timeKey (maps.get (Erlang.binaryToAtom "time", m)) |> ignore
            Erlang.put seenKey (Filter.level ev) |> ignore
            Filter.stop)

    Filter.addPrimary filterId filter (Erlang.binaryToAtom "ok") |> equal (Ok())

    // `error` is above the default primary level (`notice`), so it reaches the filter.
    logger.error "trigger for filter"

    match Erlang.get<Atom, LogLevel> seenKey with
    | Some lvl -> lvl |> equal LogLevel.Error
    | None -> equal "filter saw the event" "filter did not run"

    // The metadata `time` came out as Dynamic and decodes to a positive integer.
    match Erlang.get<Atom, Dynamic> timeKey with
    | Some d ->
        match Decode.int d with
        | Ok t -> (t > 0) |> equal true
        | Error _ -> equal "time decodes to int" "decode failed"
    | None -> equal "time present in meta" "time missing"

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

[<Fact>]
let ``test raw add_primary_filter ok path is not swallowed`` () =
#if FABLE_COMPILER
    // The opaque {FilterFun, Extra} tuple that Filter.addPrimary builds for you.
    // Exercises the bare-ok success path of the raw IExports binding.
    let filterId = Erlang.binaryToAtom "fable_test_raw_filter"

    let filterTuple: obj =
        emitErlExpr () "{fun(RawLogEvent__, _) -> RawLogEvent__ end, ok}"

    logger.add_primary_filter (filterId, filterTuple) |> equal (Ok())
    Filter.removePrimary filterId |> equal (Ok())
#else
    ()
#endif

// ============================================================================
// Primary config
// ============================================================================

[<Fact>]
let ``test set_primary_config ok path is not swallowed`` () =
#if FABLE_COMPILER
    // Setting filter_default to its default (`log`) is behaviourally a no-op but
    // exercises the bare-ok success path — a missing wrapper would fall through.
    logger.set_primary_config (Erlang.binaryToAtom "filter_default", Erlang.binaryToAtom "log")
    |> equal (Ok())
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
