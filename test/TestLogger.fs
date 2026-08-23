module Fable.Beam.Tests.Logger

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.Maps
open Fable.Beam.Logger

let tests =
    testList (
        "Logger",
        [ test ("logger.info works", fun _ ->
                  logger.info "test info message"
                  )

          test ("logger.warning works", fun _ ->
                  logger.warning "test warning message"
                  )

          test ("logger.debug works", fun _ ->
                  logger.debug "test debug message"
                  )

          test ("logger.info with format args", fun _ ->
                  // The 2-arg overload accepts both metadata maps and format args lists
                  logger.info ("test ~p message", U2.Case2 [ box 42 ])
                  )

          test ("logger add and remove handler", fun _ ->
                  // Round-trip a handler through add_handler/3 and remove_handler/1, asserting the
                  // ok | {error, term()} result maps to Ok () (and is not swallowed).
                  let handlerId = Erlang.binaryToAtom "test_handler"
                  let modle = Erlang.binaryToAtom "logger_std_h"

                  let config: BeamMap<Atom, obj> =
                      Maps.ofList [ (Erlang.binaryToAtom "level", box (Erlang.binaryToAtom "info")) ]

                  assertThat (logger.add_handler (handlerId, modle, config)) (isEqualTo (Ok ()))
                  assertThat (logger.remove_handler handlerId) (isEqualTo (Ok ()))
                  )

          test ("Filter.addPrimary receives the event and can stop it", fun _ ->
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
                          Erlang.put timeKey (Maps.get (Erlang.binaryToAtom "time") m) |> ignore
                          Erlang.put seenKey (Filter.level ev) |> ignore
                          Filter.stop)

                  assertThat (Filter.addPrimary filterId filter (Erlang.binaryToAtom "ok")) (isEqualTo (Ok ()))

                  // `error` is above the default primary level (`notice`), so it reaches the filter.
                  logger.error "trigger for filter"

                  match Erlang.get<Atom, LogLevel> seenKey with
                  | Some lvl -> assertThat lvl (isEqualTo LogLevel.Error)
                  | None -> failwith "filter saw the event"

                  // The metadata `time` came out as Dynamic and decodes to a positive integer.
                  match Erlang.get<Atom, Dynamic> timeKey with
                  | Some d ->
                      match Decode.int d with
                      | Ok t -> assertThat ((t > 0)) (isTrue)
                      | Error _ -> failwith "time decodes to int"
                  | None -> failwith "time present in meta"

                  assertThat (Filter.removePrimary filterId) (isEqualTo (Ok ()))
                  )

          test ("Filter.removePrimary on unknown id returns Error", fun _ ->
                  match Filter.removePrimary (Erlang.binaryToAtom "fable_test_no_such_filter") with
                  | Error _ -> assertThat true (isTrue)
                  | Ok () -> failwith "Error"
                  )

          test ("raw add_primary_filter ok path is not swallowed", fun _ ->
                  // The opaque {FilterFun, Extra} tuple that Filter.addPrimary builds for you.
                  // Exercises the bare-ok success path of the raw IExports binding.
                  let filterId = Erlang.binaryToAtom "fable_test_raw_filter"

                  let filterTuple: obj =
                      emitErlExpr () "{fun(RawLogEvent__, _) -> RawLogEvent__ end, ok}"

                  assertThat (logger.add_primary_filter (filterId, filterTuple)) (isEqualTo (Ok ()))
                  assertThat (Filter.removePrimary filterId) (isEqualTo (Ok ()))
                  )

          test ("set_primary_config ok path is not swallowed", fun _ ->
                  // Setting filter_default to its default (`log`) is behaviourally a no-op but
                  // exercises the bare-ok success path — a missing wrapper would fall through.
                  assertThat (logger.set_primary_config (Erlang.binaryToAtom "filter_default", Erlang.binaryToAtom "log")) (isEqualTo (Ok ()))
                  )

          test ("Formatter.setTemplate updates a handler's formatter", fun _ ->
                  let handlerId = Erlang.binaryToAtom "fable_test_fmt_handler"
                  let modle = Erlang.binaryToAtom "logger_std_h"

                  let config: BeamMap<Atom, obj> =
                      Maps.ofList [ (Erlang.binaryToAtom "level", box (Erlang.binaryToAtom "info")) ]

                  assertThat (logger.add_handler (handlerId, modle, config)) (isEqualTo (Ok ()))

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

                  assertThat (Formatter.setTemplate handlerId true template) (isEqualTo (Ok ()))

                  assertThat (logger.remove_handler handlerId) (isEqualTo (Ok ()))
                  ) ]
    )
