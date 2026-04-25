module Fable.Beam.Tests.Binary

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam
open Fable.Beam.Binary
open Fable.Beam.Lists
#endif

[<Fact>]
let ``test binary.copy makes a copy`` () =
#if FABLE_COMPILER
    let b = "hello"
    binary.copy b |> equal "hello"
#else
    ()
#endif

[<Fact>]
let ``test binary.copy N times concatenates`` () =
#if FABLE_COMPILER
    binary.copy ("ab", 3) |> equal "ababab"
#else
    ()
#endif

[<Fact>]
let ``test binary.at returns byte at position`` () =
#if FABLE_COMPILER
    // 'A' = 65, 'B' = 66
    binary.at ("AB", 0) |> equal 65
    binary.at ("AB", 1) |> equal 66
#else
    ()
#endif

[<Fact>]
let ``test binary.first returns first byte`` () =
#if FABLE_COMPILER
    // 'h' = 104
    binary.first "hello" |> equal 104
#else
    ()
#endif

[<Fact>]
let ``test binary.last returns last byte`` () =
#if FABLE_COMPILER
    // 'o' = 111
    binary.last "hello" |> equal 111
#else
    ()
#endif

[<Fact>]
let ``test binary.part extracts subbinary`` () =
#if FABLE_COMPILER
    binary.part ("hello world", 6, 5) |> equal "world"
#else
    ()
#endif

[<Fact>]
let ``test matchFirst returns Some on match`` () =
#if FABLE_COMPILER
    matchFirst "hello" "ll" |> equal (Some(2, 2))
#else
    ()
#endif

[<Fact>]
let ``test matchFirst returns None when not found`` () =
#if FABLE_COMPILER
    matchFirst "hello" "xyz" |> equal None
#else
    ()
#endif

[<Fact>]
let ``test matchAll returns all occurrences`` () =
#if FABLE_COMPILER
    let results = matchAll "abcabc" "b"
    Array.length results |> equal 2
#else
    ()
#endif

[<Fact>]
let ``test splitFirst splits on first occurrence`` () =
#if FABLE_COMPILER
    let parts = splitFirst "hello world" " "
    Array.length parts |> equal 2
    parts.[0] |> equal "hello"
    parts.[1] |> equal "world"
#else
    ()
#endif

[<Fact>]
let ``test splitAll splits on all occurrences`` () =
#if FABLE_COMPILER
    let parts = splitAll "a,b,c" ","
    Array.length parts |> equal 3
    parts.[0] |> equal "a"
    parts.[1] |> equal "b"
    parts.[2] |> equal "c"
#else
    ()
#endif

[<Fact>]
let ``test replaceFirst replaces first occurrence`` () =
#if FABLE_COMPILER
    replaceFirst "aabbaa" "aa" "XX" |> equal "XXbbaa"
#else
    ()
#endif

[<Fact>]
let ``test replaceAll replaces all occurrences`` () =
#if FABLE_COMPILER
    replaceAll "aabbaa" "aa" "XX" |> equal "XXbbXX"
#else
    ()
#endif

[<Fact>]
let ``test binary.longest_common_prefix`` () =
#if FABLE_COMPILER
    binary.longest_common_prefix ([ "foobar"; "foobaz"; "fooqux" ]) |> equal 4
#else
    ()
#endif

[<Fact>]
let ``test binary.longest_common_suffix`` () =
#if FABLE_COMPILER
    binary.longest_common_suffix ([ "foobar"; "bazbar"; "quuxbar" ]) |> equal 3
#else
    ()
#endif

[<Fact>]
let ``test binary.bin_to_list returns list of bytes`` () =
#if FABLE_COMPILER
    // "ABC" = [65, 66, 67]
    let bytes = binary.bin_to_list "ABC"
    lists.nth (1, bytes) |> equal 65
    lists.nth (2, bytes) |> equal 66
    lists.nth (3, bytes) |> equal 67
#else
    ()
#endif

[<Fact>]
let ``test binary.list_to_bin converts bytes to binary`` () =
#if FABLE_COMPILER
    // [104, 105] = "hi"
    let bytes: BeamList<int> = emitErlExpr () "[104, 105]"
    binary.list_to_bin bytes |> equal "hi"
#else
    ()
#endif

[<Fact>]
let ``test binary.bin_to_list and list_to_bin roundtrip`` () =
#if FABLE_COMPILER
    let original = "hello"
    let bytes = binary.bin_to_list original
    binary.list_to_bin bytes |> equal original
#else
    ()
#endif

[<Fact>]
let ``test binary.encode_unsigned and decode_unsigned roundtrip`` () =
#if FABLE_COMPILER
    let n = 12345
    let encoded = binary.encode_unsigned n
    binary.decode_unsigned encoded |> equal n
#else
    ()
#endif

[<Fact>]
let ``test binary.encode_unsigned of zero roundtrips`` () =
#if FABLE_COMPILER
    let encoded = binary.encode_unsigned 0
    binary.decode_unsigned encoded |> equal 0
#else
    ()
#endif

[<Fact>]
let ``test binary.decode_unsigned with little endian`` () =
#if FABLE_COMPILER
    let little = Erlang.binaryToAtom "little"
    let big = Erlang.binaryToAtom "big"
    // Big-endian encoding of 256 is <<1, 0>>.
    // Decoded as little-endian, those bytes read as 1.
    let encoded_big = binary.encode_unsigned (256, big)
    binary.decode_unsigned (encoded_big, little) |> equal 1
    // Roundtrip via little endian preserves the value.
    let encoded_little = binary.encode_unsigned (256, little)
    binary.decode_unsigned (encoded_little, little) |> equal 256
#else
    ()
#endif

[<Fact>]
let ``test binary.referenced_byte_size returns byte count`` () =
#if FABLE_COMPILER
    binary.referenced_byte_size "hello" |> equal 5
    binary.referenced_byte_size "" |> equal 0
#else
    ()
#endif
