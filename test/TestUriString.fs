module Fable.Beam.Tests.UriString

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.UriString

let tests =
    testList (
        "UriString",
        [ test (
              "parse full uri",
              fun _ ->
                  match parse "https://user:pass@example.com:8080/path?q=hello#frag" with
                  | Ok uri ->
                      assertThat (scheme uri) (isEqualTo (Some "https"))
                      assertThat (userinfo uri) (isEqualTo (Some "user:pass"))
                      assertThat (host uri) (isEqualTo (Some "example.com"))
                      assertThat (port uri) (isEqualTo (Some 8080))
                      assertThat (path uri) (isEqualTo (Some "/path"))
                      assertThat (query uri) (isEqualTo (Some "q=hello"))
                      assertThat (fragment uri) (isEqualTo (Some "frag"))
                  | Error _ -> failwith "expected a parsed uri"
          )

          test (
              "parse minimal uri",
              fun _ ->
                  match parse "https://example.com" with
                  | Ok uri ->
                      assertThat (scheme uri) (isEqualTo (Some "https"))
                      assertThat (host uri) (isEqualTo (Some "example.com"))
                      assertThat (port uri) (isEqualTo None)
                      assertThat (query uri) (isEqualTo None)
                      assertThat (fragment uri) (isEqualTo None)
                  | Error _ -> failwith "expected a parsed uri"
          )

          test (
              "parse relative uri has no scheme or host",
              fun _ ->
                  match parse "/relative/path" with
                  | Ok uri ->
                      assertThat (scheme uri) (isEqualTo None)
                      assertThat (host uri) (isEqualTo None)
                      assertThat (path uri) (isEqualTo (Some "/relative/path"))
                  | Error _ -> failwith "expected a parsed uri"
          )

          test (
              "parse path only uri",
              fun _ ->
                  match parse "just/a/path" with
                  | Ok uri ->
                      assertThat (scheme uri) (isEqualTo None)
                      assertThat (host uri) (isEqualTo None)
                      assertThat (path uri) (isEqualTo (Some "just/a/path"))
                  | Error _ -> failwith "expected a parsed uri"
          )

          test (
              "normalize lowercases scheme and host",
              fun _ -> assertThat (normalize "HTTP://EXAMPLE.COM/path") (isEqualTo (Ok "http://example.com/path"))
          )

          test (
              "normalize removes default http port",
              fun _ -> assertThat (normalize "http://example.com:80/path") (isEqualTo (Ok "http://example.com/path"))
          )

          test (
              "normalize removes default https port",
              fun _ -> assertThat (normalize "https://example.com:443/path") (isEqualTo (Ok "https://example.com/path"))
          )

          test (
              "normalize resolves dot segments",
              fun _ -> assertThat (normalize "http://example.com/a/b/../c") (isEqualTo (Ok "http://example.com/a/c"))
          )

          test (
              "resolve absolute path reference",
              fun _ ->
                  assertThat (resolve "/new" "https://example.com/old/page") (isEqualTo (Ok "https://example.com/new"))
          )

          test (
              "resolve relative path reference",
              fun _ ->
                  assertThat
                      (resolve "new" "https://example.com/old/page")
                      (isEqualTo (Ok "https://example.com/old/new"))
          )

          test (
              "resolve full uri preserves reference",
              fun _ ->
                  assertThat
                      (resolve "https://other.com/path" "https://example.com/base")
                      (isEqualTo (Ok "https://other.com/path"))
          )

          test (
              "dissect query parses key value pairs",
              fun _ -> assertThat (dissectQuery "q=hello&lang=en") (isEqualTo [ ("q", "hello"); ("lang", "en") ])
          )

          test ("dissect query empty string", fun _ -> assertThat (dissectQuery "") (isEqualTo []))

          test (
              "compose query builds query string",
              fun _ -> assertThat (composeQuery [ ("q", "search"); ("page", "1") ]) (isEqualTo "q=search&page=1")
          )

          test ("compose query empty list", fun _ -> assertThat (composeQuery []) (isEqualTo ""))

          test (
              "dissect and compose query roundtrip",
              fun _ ->
                  let original = "name=Alice&role=admin"
                  assertThat (original |> dissectQuery |> composeQuery) (isEqualTo original)
          )

          test (
              "percent decode decodes encoded chars",
              fun _ -> assertThat (percentDecode "hello%20world") (isEqualTo (Ok "hello world"))
          )

          test (
              "percent decode passthrough for plain string",
              fun _ -> assertThat (percentDecode "hello") (isEqualTo (Ok "hello"))
          )

          test (
              "percent decode returns error for malformed encoding",
              fun _ ->
                  match percentDecode "invalid%GG" with
                  | Error _ -> assertThat true (isTrue)
                  | Ok _ -> failwith "expected a malformed-encoding error"
          )

          test (
              "quote encodes spaces and slashes",
              fun _ -> assertThat (quote "hello world") (isEqualTo "hello%20world")
          )

          test (
              "quote with safe chars preserves slash",
              fun _ -> assertThat (quoteWith "hello/world" "/") (isEqualTo "hello/world")
          )

          test (
              "unquote decodes percent encoded string",
              fun _ -> assertThat (unquote "hello%20world") (isEqualTo "hello world")
          )

          test ("unquote passthrough for plain string", fun _ -> assertThat (unquote "hello") (isEqualTo "hello")) ]
    )
