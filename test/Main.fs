module Fable.Beam.Tests.Main

open Scriptorium.Quill
open type Scriptorium.Quill.Runner

// Quill is the entry point here (not an Erlang test_runner): Fable emits this as main:main/1, and
// Quill runs the registered suites then halts the VM with its exit code -- non-zero on failure.
// This is a BEAM-only subset for now: only the modules converted off [<Fact>] are registered.
// Add more `.tests` below as each remaining file migrates over to Scriptorium.
[<EntryPoint>]
let main _ =
    runTests
        [ Timer.tests
          Maps.tests
          GenServer.tests
          Base64.tests
          Math.tests
          Io.tests
          IoLib.tests
          Rand.tests
          Proplists.tests
          Binary.tests
          Calendar.tests
          Queue.tests
          Lists.tests
          String.tests
          Erlang.tests
          Re.tests
          Dynamic.tests
          UriString.tests
          Callbacks.tests
          Os.tests
          Port.tests
          Supervisor.tests
          Logger.tests
          File.tests
          Ets.tests
          Jsx.tests ]
