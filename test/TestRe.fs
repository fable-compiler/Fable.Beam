module Fable.Beam.Tests.Re

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Re
open Fable.Beam.Lists

let tests =
    testList (
        "Re",
        [ test ("isMatch returns true for matching pattern", fun _ ->
                  assertThat (isMatch "hello world" "hello") (isTrue))

          test ("isMatch returns false for non-matching pattern", fun _ ->
                  assertThat (isMatch "hello world" "xyz") (isFalse))

          test ("isMatch with digit pattern", fun _ ->
                  assertThat (isMatch "abc123" "\\d+") (isTrue))

          test ("isMatch anchored no match", fun _ ->
                  assertThat (isMatch "hello" "^world") (isFalse))

          test ("compile returns Ok for valid pattern", fun _ ->
                  match compile "hello" with
                  | Ok _ -> assertThat true (isTrue)
                  | Error _ -> failwith "expected compile to succeed"
                  )

          test ("compile returns Error for invalid pattern", fun _ ->
                  match compile "[invalid" with
                  | Ok _ -> failwith "expected compile to fail"
                  | Error msg -> assertThat (msg.Length > 0) (isTrue)
                  )

          test ("isMatchMP with compiled pattern", fun _ ->
                  match compile "\\d+" with
                  | Ok mp -> assertThat (isMatchMP "abc123" mp) (isTrue)
                  | Error _ -> failwith "expected compile to succeed"
                  )

          test ("isMatchMP with compiled pattern no match", fun _ ->
                  match compile "\\d+" with
                  | Ok mp -> assertThat (isMatchMP "abcdef" mp) (isFalse)
                  | Error _ -> failwith "expected compile to succeed"
                  )

          test ("run returns Some with whole match at index 0", fun _ ->
                  match run "hello world" "hello" with
                  | Some captures -> assertThat captures.[0] (isEqualTo "hello")
                  | None -> failwith "expected a match"
                  )

          test ("run returns Some with capture groups", fun _ ->
                  match run "hello world" "h(e)(l+)o" with
                  | Some captures ->
                      assertThat captures.[0] (isEqualTo "hello")
                      assertThat captures.[1] (isEqualTo "e")
                      assertThat captures.[2] (isEqualTo "ll")
                  | None -> failwith "expected a match"
                  )

          test ("run returns None for no match", fun _ ->
                  assertThat (run "hello world" "xyz") (isEqualTo None))

          test ("runMP returns captures for compiled pattern", fun _ ->
                  match compile "(\\d+)" with
                  | Ok mp ->
                      match runMP "abc123def" mp with
                      | Some captures ->
                          assertThat captures.[0] (isEqualTo "123")
                          assertThat captures.[1] (isEqualTo "123")
                      | None -> failwith "expected a capture"
                  | Error _ -> failwith "expected compile to succeed"
                  )

          test ("replaceFirst replaces only first occurrence", fun _ ->
                  assertThat (replaceFirst "aabbaa" "a+" "X") (isEqualTo "Xbbaa"))

          test ("replaceFirstWith caseless replaces first case-insensitively", fun _ ->
                  assertThat (replaceFirstWith "Aabbaa" "a+" "X" [ caseless ]) (isEqualTo "Xbbaa"))

          test ("replaceAll replaces all occurrences", fun _ ->
                  assertThat (replaceAll "aabbaa" "a+" "X") (isEqualTo "XbbX"))

          test ("replaceAll with digit pattern", fun _ ->
                  assertThat (replaceAll "abc123def456" "\\d+" "N") (isEqualTo "abcNdefN"))

          test ("split on comma", fun _ ->
                  let parts = split "one,two,three" ","
                  assertThat parts.[0] (isEqualTo "one")
                  assertThat parts.[1] (isEqualTo "two")
                  assertThat parts.[2] (isEqualTo "three")
                  )

          test ("split on whitespace pattern", fun _ ->
                  let parts = split "a b  c" "\\s+"
                  assertThat parts.[0] (isEqualTo "a")
                  assertThat parts.[1] (isEqualTo "b")
                  assertThat parts.[2] (isEqualTo "c")
                  )

          test ("splitParts limits result count", fun _ ->
                  let parts = splitParts "one,two,three,four" "," 2
                  assertThat parts.[0] (isEqualTo "one")
                  assertThat parts.[1] (isEqualTo "two,three,four")
                  )

          test ("isMatchWith caseless option matches different case", fun _ ->
                  assertThat (isMatchWith "HELLO" "hello" [ caseless ]) (isTrue)
                  // Sanity check: default is case-sensitive
                  assertThat (isMatch "HELLO" "hello") (isFalse)
                  )

          test ("isMatchWith multiline option matches after newline", fun _ ->
                  // ^world only matches at line starts in multiline mode
                  assertThat (isMatchWith "hello\nworld" "^world" [ multiline ]) (isTrue)
                  assertThat (isMatch "hello\nworld" "^world") (isFalse)
                  )

          test ("isMatchWith unicode option handles multi-byte characters", fun _ ->
                  // "é" is 2 bytes in UTF-8. Without unicode, ^.$ expects exactly 1 byte — no match.
                  // With unicode, ^.$ expects exactly 1 codepoint — matches.
                  assertThat (isMatch "é" "^.$") (isFalse)
                  assertThat (isMatchWith "é" "^.$" [ unicode ]) (isTrue)
                  )

          test ("runWith caseless returns original-case captures", fun _ ->
                  match runWith "HELLO world" "hello" [ caseless ] with
                  | Some captures -> assertThat captures.[0] (isEqualTo "HELLO")
                  | None -> failwith "expected a match"
                  )

          test ("compileWith caseless produces case-insensitive MP", fun _ ->
                  match compileWith "hello" [ caseless ] with
                  | Ok mp ->
                      assertThat (isMatchMP "HELLO" mp) (isTrue)
                      assertThat (isMatchMP "hello" mp) (isTrue)
                  | Error _ -> failwith "expected compile to succeed"
                  )

          test ("replaceAllWith caseless replaces all cases", fun _ ->
                  assertThat (replaceAllWith "Hello HELLO hello" "hello" "X" [ caseless ]) (isEqualTo "X X X"))

          test ("splitWith caseless splits on either case", fun _ ->
                  let parts = splitWith "aXbxc" "x" [ caseless ]
                  assertThat parts.[0] (isEqualTo "a")
                  assertThat parts.[1] (isEqualTo "b")
                  assertThat parts.[2] (isEqualTo "c")
                  )

          test ("replaceFirstMP with compiled pattern", fun _ ->
                  match compile "a+" with
                  | Ok mp -> assertThat (replaceFirstMP "aabbaa" mp "X") (isEqualTo "Xbbaa")
                  | Error _ -> failwith "expected compile to succeed"
                  )

          test ("replaceAllMP with compiled pattern", fun _ ->
                  match compile "\\d+" with
                  | Ok mp -> assertThat (replaceAllMP "abc123def456" mp "N") (isEqualTo "abcNdefN")
                  | Error _ -> failwith "expected compile to succeed"
                  )

          test ("splitMP with compiled pattern", fun _ ->
                  match compile "," with
                  | Ok mp ->
                      let parts = splitMP "one,two,three" mp
                      assertThat parts.[0] (isEqualTo "one")
                      assertThat parts.[1] (isEqualTo "two")
                      assertThat parts.[2] (isEqualTo "three")
                  | Error _ -> failwith "expected compile to succeed"
                  )

          test ("isMatch with empty subject", fun _ ->
                  // Empty pattern matches empty subject (zero-width match at position 0)
                  assertThat (isMatch "" "") (isTrue)
                  assertThat (isMatch "" "a") (isFalse)
                  )

          test ("run with empty subject and optional group returns Some empty", fun _ ->
                  match run "" "a*" with
                  | Some captures -> assertThat captures.[0] (isEqualTo "")
                  | None -> failwith "expected a match"
                  )

          test ("splitRaw returns the native list form of split", fun _ ->
                  let parts: BeamList<string> = splitRaw "a1b2c" "[0-9]"
                  let expected: BeamList<string> = emitErlExpr () "[<<\"a\">>, <<\"b\">>, <<\"c\">>]"
                  assertThat parts (isEqualTo expected)
                  ) ]
    )
