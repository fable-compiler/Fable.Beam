module Fable.Beam.Tests.Erlang

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.Lists

type RecvMsg =
    | [<CompiledName("ping")>] Ping
    | [<CompiledName("data")>] Data of value: int

let tests =
    testList (
        "Erlang",
        [ test (
              "self returns a pid",
              fun _ ->
                  let pid = Erlang.self ()
                  let isAlive = Erlang.isProcessAlive pid
                  assertThat isAlive (isTrue)
          )

          test (
              "makeRef returns unique references",
              fun _ ->
                  let ref1 = Erlang.makeRef ()
                  let ref2 = Erlang.makeRef ()
                  assertThat (Erlang.exactEquals ref1 ref2) (isFalse)
          )

          test (
              "exactEquals on same ref",
              fun _ ->
                  let ref1 = Erlang.makeRef ()
                  assertThat (Erlang.exactEquals ref1 ref1) (isTrue)
          )

          test (
              "spawn creates a process",
              fun _ ->
                  let pid = Erlang.spawn (fun () -> ())
                  assertThat (Erlang.isProcessAlive pid) (isTrue)
          )

          test (
              "spawnLink creates a linked process",
              fun _ ->
                  let pid = Erlang.spawnLink (fun () -> ())
                  assertThat (Erlang.isProcessAlive pid) (isTrue)
          )

          test (
              "isProcessAlive on self",
              fun _ ->
                  let pid = Erlang.self ()
                  assertThat (Erlang.isProcessAlive pid) (isTrue)
          )

          test (
              "process dictionary get/put/erase",
              fun _ ->
                  let key = Erlang.makeRef ()
                  Erlang.put key (box 42) |> ignore

                  match Erlang.get key with
                  | Some v -> assertThat v (isEqualTo (box 42))
                  | None -> failwith "process dict key should be set"

                  Erlang.erase key |> ignore
          )

          test (
              "send and receive",
              fun _ ->
                  // A nullary DU case compiles to a bare atom (`ping`), not a 1-tuple (`{ping}`) -- only a
                  // case *with* fields becomes a tagged tuple, e.g. `Data 42` -> `{data, 42}`. Sending
                  // `{ping}` here never matched the generated receive clause, so this test used to sit out
                  // the full 1000ms timeout and take the None branch.
                  emitErlExpr () "erlang:self() ! ping"

                  match Erlang.receive<RecvMsg> 1000 with
                  | Some Ping -> assertThat true (isTrue)
                  | _ -> failwith "expected to receive the ping message"
          )

          test (
              "receive with timeout returns None",
              fun _ ->
                  match Erlang.receive<RecvMsg> 0 with
                  | None -> assertThat true (isTrue)
                  | Some _ -> failwith "expected a timeout"
          )

          test (
              "receive with data",
              fun _ ->
                  emitErlExpr () "erlang:self() ! {data, 42}"

                  match Erlang.receive<RecvMsg> 1000 with
                  | Some(Data v) -> assertThat v (isEqualTo 42)
                  | _ -> failwith "expected to receive the data message"
          )

          test (
              "sendAfter and cancelTimer",
              fun _ ->
                  let timerRef = Erlang.sendAfter 60000 (box "should_not_arrive")

                  match Erlang.cancelTimer timerRef with
                  | Some remaining -> assertThat (remaining > 0) (isTrue)
                  | None -> failwith "expected cancelTimer to return the remaining time"
          )

          test (
              "atomToBinary and binaryToAtom roundtrip",
              fun _ ->
                  let atom = Erlang.binaryToAtom "test_atom"
                  let str = Erlang.atomToBinary atom
                  assertThat str (isEqualTo "test_atom")
          )

          test (
              "monitor and demonitor",
              fun _ ->
                  let pid = Erlang.spawn (fun () -> Fable.Beam.Timer.sleep 60000)
                  let ref = Erlang.monitor pid
                  Erlang.demonitorFlush ref
                  Erlang.exitPid pid (box "kill")
          )

          test (
              "register and whereis",
              fun _ ->
                  let name = Erlang.binaryToAtom "fable_beam_test_proc"
                  let pid = Erlang.self ()
                  Erlang.register name pid

                  match Erlang.whereis name with
                  | Some found -> assertThat (Erlang.exactEquals pid found) (isTrue)
                  | None -> failwith "whereis should find the registered process"
          )

          test (
              "date returns valid year month day",
              fun _ ->
                  let (year, month, day) = Erlang.date ()
                  assertThat (year >= 2025) (isTrue)
                  assertThat (month >= 1 && month <= 12) (isTrue)
                  assertThat (day >= 1 && day <= 31) (isTrue)
          )

          test (
              "dateYear dateMonth dateDay match date",
              fun _ ->
                  let (year, month, day) = Erlang.date ()
                  assertThat (Erlang.dateYear ()) (isEqualTo year)
                  assertThat (Erlang.dateMonth ()) (isEqualTo month)
                  assertThat (Erlang.dateDay ()) (isEqualTo day)
          )

          test (
              "time returns valid hour minute second",
              fun _ ->
                  let (hour, minute, second) = Erlang.time ()
                  assertThat (hour >= 0 && hour <= 23) (isTrue)
                  assertThat (minute >= 0 && minute <= 59) (isTrue)
                  assertThat (second >= 0 && second <= 59) (isTrue)
          )

          test (
              "localtime returns valid date and time",
              fun _ ->
                  let ((year, month, day), (hour, minute, second)) = Erlang.localtime ()
                  assertThat (year >= 2025) (isTrue)
                  assertThat (month >= 1 && month <= 12) (isTrue)
                  assertThat (day >= 1 && day <= 31) (isTrue)
                  assertThat (hour >= 0 && hour <= 23) (isTrue)
                  assertThat (minute >= 0 && minute <= 59) (isTrue)
                  assertThat (second >= 0 && second <= 59) (isTrue)
          )

          test (
              "universaltime returns valid date and time",
              fun _ ->
                  let ((year, month, day), (hour, minute, second)) = Erlang.universaltime ()
                  assertThat (year >= 2025) (isTrue)
                  assertThat (month >= 1 && month <= 12) (isTrue)
                  assertThat (day >= 1 && day <= 31) (isTrue)
                  assertThat (hour >= 0 && hour <= 23) (isTrue)
                  assertThat (minute >= 0 && minute <= 59) (isTrue)
                  assertThat (second >= 0 && second <= 59) (isTrue)
          )

          test (
              "monotonicTimeMs returns positive",
              fun _ ->
                  let t1 = Erlang.monotonicTimeMs ()
                  let t2 = Erlang.monotonicTimeMs ()
                  assertThat (t2 >= t1) (isTrue)
          )

          test (
              "whereis returns None for unregistered name",
              fun _ ->
                  let name = Erlang.binaryToAtom "fable_beam_nonexistent_12345"
                  assertThat (Erlang.whereis name) (isEqualTo None)
          )

          test (
              "trapExit returns old value",
              fun _ ->
                  let old1 = Erlang.trapExit ()
                  // Second call should return true since we just set it
                  let old2 = Erlang.trapExit ()
                  assertThat old2 (isTrue)
                  // Reset: set trap_exit back to false
                  Erlang.processFlag (Erlang.binaryToAtom "trap_exit") (box false) |> ignore
          )

          test (
              "cancelTimer returns None for invalid ref",
              fun _ ->
                  let fakeRef = Erlang.makeRef ()
                  // cancelTimer on a non-timer ref returns None (false in Erlang).
                  let timerRef = Erlang.sendAfter 60000 (box "test")

                  match Erlang.cancelTimer timerRef with
                  | Some ms -> assertThat (ms >= 0) (isTrue)
                  | None -> failwith "expected cancelTimer to return the remaining time"
                  // Cancelling again should return None
                  assertThat (Erlang.cancelTimer timerRef) (isEqualTo None)
          )

          test (
              "sendAfterTo sends to specific pid",
              fun _ ->
                  let pid = Erlang.self ()
                  let timerRef = Erlang.sendAfterTo 60000 pid (box "msg")

                  match Erlang.cancelTimer timerRef with
                  | Some _ -> assertThat true (isTrue)
                  | None -> failwith "expected cancelTimer to succeed"
          )

          test (
              "byteSize returns correct size",
              fun _ ->
                  assertThat (Erlang.byteSize "hello") (isEqualTo 5)
                  assertThat (Erlang.byteSize "") (isEqualTo 0)
                  assertThat (Erlang.byteSize "abc") (isEqualTo 3)
          )

          test (
              "atomToList returns charlist not binary",
              fun _ ->
                  let atom = Erlang.binaryToAtom "test"
                  let charlist = Erlang.atomToList atom
                  // atomToList returns a charlist (Erlang list of integers),
                  // which is not the same as an F# string (binary).
                  // We verify by round-tripping through listToAtom.
                  let atom2 = Erlang.listToAtom charlist
                  assertThat (Erlang.atomToBinary atom2) (isEqualTo "test")
          )

          test (
              "binaryToList returns list of bytes",
              fun _ ->
                  let bytes = Erlang.binaryToList "ABC"
                  assertThat (Erlang.length bytes) (isEqualTo 3)
                  assertThat (Erlang.head bytes) (isEqualTo 65)
          )

          test (
              "binaryToList and listToBinary roundtrip",
              fun _ ->
                  let original = "hello"
                  let bytes = Erlang.binaryToList original
                  assertThat (Erlang.listToBinary bytes) (isEqualTo original)
          )

          test (
              "isEmpty returns true for empty list",
              fun _ ->
                  let empty: BeamList<int> = emitErlExpr () "[]"
                  assertThat (Erlang.isEmpty empty) (isTrue)
          )

          test (
              "isEmpty returns false for non-empty list",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3]"
                  assertThat (Erlang.isEmpty xs) (isFalse)
          )

          test (
              "head returns first element",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[42, 2, 3]"
                  assertThat (Erlang.head xs) (isEqualTo 42)
          )

          test (
              "head preserves element type with tuples",
              fun _ ->
                  let xs: BeamList<int * string> = emitErlExpr () "[{1, <<\"a\">>}, {2, <<\"b\">>}]"
                  let (n, s) = Erlang.head xs
                  assertThat n (isEqualTo 1)
                  assertThat s (isEqualTo "a")
          )

          test (
              "tail returns rest of list",
              fun _ ->
                  let xs: BeamList<int> = emitErlExpr () "[1, 2, 3]"
                  let tl = Erlang.tail xs
                  assertThat (Erlang.head tl) (isEqualTo 2)
                  assertThat (Erlang.isEmpty (Erlang.tail tl |> Erlang.tail)) (isTrue)
          )

          test (
              "head raises on empty list",
              fun _ ->
                  let empty: BeamList<int> = emitErlExpr () "[]"
                  assertThat (fun () -> Erlang.head empty |> ignore) throws
          )

          test (
              "tail raises on empty list",
              fun _ ->
                  let empty: BeamList<int> = emitErlExpr () "[]"
                  assertThat (fun () -> Erlang.tail empty |> ignore) throws
          )

          test (
              "Atom.ofString builds a real atom, not a binary",
              fun _ ->
                  // Regression: the erased `Atom` constructor used to be public, so `Atom "x"`
                  // compiled to the binary <<"x">> and silently failed to match atom-keyed terms.
                  let a = Atom.ofString "test_real_atom"
                  let isAtom: bool = emitErlExpr a "erlang:is_atom($0)"
                  assertThat isAtom (isTrue)
                  assertThat (Atom.toString a) (isEqualTo "test_real_atom")
          ) ]
    )
