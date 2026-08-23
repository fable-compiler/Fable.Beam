module Fable.Beam.Tests.Os

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Beam
open Fable.Beam.Os

let tests =
    testList (
        "Os",
        [ test (
              "getenv returns None for unset var",
              fun _ -> assertThat (getenv "FABLE_BEAM_TEST_UNSET_12345") (isEqualTo None)
          )

          test (
              "putenv and getenv roundtrip",
              fun _ ->
                  putenv "FABLE_BEAM_TEST_VAR" "hello_beam"
                  assertThat (getenv "FABLE_BEAM_TEST_VAR") (isEqualTo (Some "hello_beam"))
                  unsetenv "FABLE_BEAM_TEST_VAR"
          )

          test (
              "unsetenv removes a variable",
              fun _ ->
                  putenv "FABLE_BEAM_TEST_UNSET" "temp"
                  unsetenv "FABLE_BEAM_TEST_UNSET"
                  assertThat (getenv "FABLE_BEAM_TEST_UNSET") (isEqualTo None)
          )

          test (
              "getenv returns Some for HOME",
              fun _ ->
                  match getenv "HOME" with
                  | Some home -> assertThat ((String.length home > 0)) (isTrue)
                  | None ->
                      // HOME should be set on any unix system
                      failwith "expected HOME to be set"
          )

          test (
              "cmd runs a command",
              fun _ ->
                  let result = cmd "echo hello"
                  assertThat result (isEqualTo "hello\n")
          )

          test (
              "systemTimeSeconds returns positive",
              fun _ ->
                  let t = systemTimeSeconds ()
                  assertThat ((t > 0)) (isTrue)
          )

          test (
              "systemTime with TimeUnit returns a sensible value",
              fun _ ->
                  // Exercises the TimeUnit DU: each case compiles to its time-unit atom.
                  let secs = systemTime TimeUnit.Second
                  assertThat ((secs > 1_000_000_000L)) (isTrue)
                  let micros = systemTime TimeUnit.Microsecond
                  assertThat ((micros > secs)) (isTrue)
          )

          test (
              "systemTimeMs is monotonically increasing",
              fun _ ->
                  let t1 = systemTimeMs ()
                  let t2 = systemTimeMs ()
                  assertThat ((t2 >= t1)) (isTrue)
          )

          test (
              "systemTimeMs returns int64 value above 32-bit range",
              fun _ ->
                  let t = systemTimeMs ()
                  // Millisecond timestamps are around 1.7 * 10^12, well above int32 max (~2.1 * 10^9)
                  assertThat ((t > 1_000_000_000_000L)) (isTrue)
          )

          test (
              "systemTimeSeconds returns int64",
              fun _ ->
                  let t = systemTimeSeconds ()
                  // Unix epoch seconds are around 1.7 * 10^9
                  assertThat ((t > 1_000_000_000L)) (isTrue)
          )

          test (
              "osType returns a string tuple",
              fun _ ->
                  let (family, _name) = osType ()
                  // Should be "unix" on Linux/macOS or "win32" on Windows
                  assertThat ((family = "unix" || family = "win32")) (isTrue)
          )

          test (
              "version returns an int tuple",
              fun _ ->
                  let (major, minor, release) = version ()
                  assertThat ((major >= 0)) (isTrue)
                  assertThat ((minor >= 0)) (isTrue)
                  assertThat ((release >= 0)) (isTrue)
          ) ]
    )
