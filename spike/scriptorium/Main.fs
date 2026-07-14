module Main

open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test
open type Scriptorium.Quill.Runner

open Fable.Beam.Maps

/// Spike: run the Scriptorium test framework (Nib assertions + the Quill runner) on the BEAM,
/// against real Fable.Beam bindings.
///
/// Unlike test/, there is no Erlang test_runner.erl and no [<Fact>] marker: Quill's runner *is*
/// the entry point. Fable emits [<EntryPoint>] as main:main/1, Quill runs the suite and halts the
/// VM with the exit code, so `erl` returns non-zero on failure.
let tests =
    [ testList (
          "Nib assertions on the BEAM",
          [ test ("isEqualTo", fun _ -> assertThat 42 (isEqualTo 42))

            test ("chained comparisons", fun _ -> assertThat 42 (isGreaterThan 40 >> isLessThan 50))

            // F# strings compile to Erlang binaries (<<"hello">>), not charlists.
            test ("strings", fun _ -> assertThat "hello" (isEqualTo "hello"))

            test ("booleans", fun _ -> assertThat true isTrue)

            test ("lists", fun _ -> assertThat [ 1; 2; 3 ] (hasSize 3 >> contain 2))

            // Structural equality across the Fable runtime, not just primitives.
            test ("options", fun _ -> assertThat (Some "cowboy") (isEqualTo (Some "cowboy"))) ]
      )

      testList (
          "Fable.Beam maps bindings",
          [ test (
                "maps.new_ creates an empty map",
                fun _ ->
                    let m: BeamMap<string, int> = maps.new_ ()
                    assertThat (maps.size m) (isEqualTo 0)
            )

            test (
                "maps.put and maps.get round-trip",
                fun _ ->
                    let m: BeamMap<string, string> = maps.new_ ()
                    let m = maps.put ("key", "value", m)
                    assertThat (maps.get ("key", m)) (isEqualTo "value")
            )

            test (
                "maps.get falls back to the default for a missing key",
                fun _ ->
                    let m: BeamMap<string, int> = maps.new_ ()
                    assertThat (maps.get ("missing", m, 42)) (isEqualTo 42)
            )

            test (
                "tryFind maps presence onto an option",
                fun _ ->
                    let m: BeamMap<string, int> = maps.put ("x", 99, maps.new_ ())
                    assertThat (tryFind "x" m) (isEqualTo (Some 99))
                    assertThat (tryFind "nope" m) (isEqualTo None)
            )

            test (
                "ofList builds a map from a literal list",
                fun _ ->
                    let headers: BeamMap<string, string> =
                        ofList [ "content-type", "text/html"; "server", "cowboy" ]

                    assertThat (maps.size headers) (isEqualTo 2)
                    assertThat (tryFind "server" headers) (isEqualTo (Some "cowboy"))
            ) ]
      ) ]

[<EntryPoint>]
let main _ = runTests tests
