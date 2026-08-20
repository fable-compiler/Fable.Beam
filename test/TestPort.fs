module Fable.Beam.Tests.Port

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Port

/// A single-case message used to place an unrelated message in the test process's
/// mailbox and verify that selective port receive leaves it there. The nullary
/// case compiles to the bare Erlang atom `unrelated_probe`, matching how the
/// message is injected below.
type MailboxProbe = | [<CompiledName("unrelated_probe")>] Unrelated
#endif

[<Fact>]
let ``test port streams a complete line and exit status`` () =
#if FABLE_COMPILER
    // This is a deliberately small fixture: sh reads one stdin line, echoes it,
    // and exits. The script is an argument to a fixed executable, not a command
    // assembled by the port binding.
    let script = "read line; printf '%s\n' \"$line\"; exit 7"

    let port =
        match startAbsolute "/bin/sh" [ "-c"; script ] 128 with
        | Ok port -> port
        | Error reason -> failwithf "could not start port fixture: %s" reason

    send port "hello from port\n" |> equal true

    match receive port 1000 with
    | Some(Line data) -> equal "hello from port" data
    | Some message -> failwithf "expected line data, got %A" message
    | None -> failwith "timed out waiting for port line"

    match receive port 1000 with
    | Some(ExitStatus status) -> equal 7 status
    | Some message -> failwithf "expected exit status, got %A" message
    | None -> failwith "timed out waiting for port exit status"
#else
    ()
#endif

[<Fact>]
let ``test port path lookup and close`` () =
#if FABLE_COMPILER
    let port =
        match startOnPath "cat" [] 128 with
        | Ok port -> port
        | Error reason -> failwithf "could not find cat on PATH: %s" reason

    send port "close probe\n" |> equal true

    match receive port 1000 with
    | Some(Line data) -> equal "close probe" data
    | Some message -> failwithf "expected line data, got %A" message
    | None -> failwith "timed out waiting for port line"

    close port
#else
    ()
#endif

[<Fact>]
let ``test port delivers an incomplete line when the process ends mid-line`` () =
#if FABLE_COMPILER
    // `printf` with no trailing newline: the process ends mid-line. With `exit_status`
    // enabled, ERTS delivers both the pending partial line (`{data, {noeol, ...}}`) and
    // the exit status. Their relative order is an ERTS implementation detail — on this
    // runtime the exit status arrives first — so assert on the two trailing messages
    // without depending on which one is delivered first.
    let port =
        match startAbsolute "/bin/sh" [ "-c"; "printf 'partial'; exit 3" ] 128 with
        | Ok port -> port
        | Error reason -> failwithf "could not start port fixture: %s" reason

    let trailing =
        [ receive port 1000; receive port 1000 ]
        |> List.filter (fun m -> m <> None)
        |> List.map Option.get

    (trailing
     |> List.exists (function
         | IncompleteLine d -> d = "partial"
         | _ -> false))
    |> equal true

    (trailing
     |> List.exists (function
         | ExitStatus s -> s = 3
         | _ -> false))
    |> equal true
#else
    ()
#endif

[<Fact>]
let ``test port receive leaves unrelated mailbox messages behind`` () =
#if FABLE_COMPILER
    // Inject an unrelated message into this process's mailbox before the port reads,
    // to verify the selective port receive skips it and leaves it for the mailbox owner.
    emitErlExpr () "erlang:self() ! unrelated_probe"

    let port =
        match startAbsolute "/bin/sh" [ "-c"; "read line; printf '%s\n' \"$line\"; exit 0" ] 128 with
        | Ok port -> port
        | Error reason -> failwithf "could not start port fixture: %s" reason

    send port "selective probe\n" |> equal true

    // Selective receive must skip the unrelated message and deliver the port line.
    match receive port 1000 with
    | Some(Line data) -> equal "selective probe" data
    | Some message -> failwithf "expected line data, got %A" message
    | None -> failwith "timed out waiting for port line"

    // Drain the port's exit status; selective receive still skips the unrelated message.
    match receive port 1000 with
    | Some(ExitStatus status) -> equal 0 status
    | Some message -> failwithf "expected exit status, got %A" message
    | None -> failwith "timed out waiting for exit status"

    // The unrelated message must still be in the mailbox, undisturbed.
    match Erlang.receive<MailboxProbe> 1000 with
    | Some Unrelated -> equal 1 1
    | _ -> failwith "unrelated mailbox message was lost by selective receive"
#else
    ()
#endif

[<Fact>]
let ``test port startAbsolute rejects a relative path`` () =
#if FABLE_COMPILER
    match startAbsolute "some/relative/path" [] 128 with
    | Error reason -> equal "path must be absolute and maxLineLength must be positive" reason
    | Ok _ -> failwith "expected an error for a relative path"
#else
    ()
#endif

[<Fact>]
let ``test port startOnPath errors when the executable is missing`` () =
#if FABLE_COMPILER
    match startOnPath "definitely_not_a_real_executable_xyz" [] 128 with
    | Error reason -> equal "executable not found on PATH" reason
    | Ok _ -> failwith "expected an error for a missing executable"
#else
    ()
#endif

[<Fact>]
let ``test port rejects a non-positive maxLineLength`` () =
#if FABLE_COMPILER
    match startAbsolute "/bin/sh" [] 0 with
    | Error reason -> equal "path must be absolute and maxLineLength must be positive" reason
    | Ok _ -> failwith "expected an error for maxLineLength 0"

    match startOnPath "cat" [] 0 with
    | Error reason -> equal "maxLineLength must be positive" reason
    | Ok _ -> failwith "expected an error for maxLineLength 0"
#else
    ()
#endif
