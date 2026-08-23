module Fable.Beam.Tests.Port

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Port

#if FABLE_COMPILER
/// A single-case message used to place an unrelated message in the test process's
/// mailbox and verify that selective port receive leaves it there. The nullary
/// case compiles to the bare Erlang atom `unrelated_probe`, matching how the
/// message is injected below.
type MailboxProbe = | [<CompiledName("unrelated_probe")>] Unrelated

let private options arguments maxLineLength =
    { PortOptions.defaultOptions with
        arguments = arguments
        maxLineLength = maxLineLength }
#endif

let tests =
    testList (
        "Port",
        [ test ("port streams a complete line and exit status", fun _ ->
                  // This is a deliberately small fixture: sh reads one stdin line, echoes it,
                  // and exits. The script is an argument to a fixed executable, not a command
                  // assembled by the port binding.
                  let script = "read line; printf '%s\n' \"$line\"; exit 7"

                  let port =
                      match startAbsolute "/bin/sh" (options [ "-c"; script ] 128) with
                      | Ok port -> port
                      | Error reason -> failwithf "could not start port fixture: %s" reason

                  assertThat (send port "hello from port\n") (isTrue)

                  match receive port 1000 with
                  | Some(Line data) -> assertThat data (isEqualTo "hello from port")
                  | Some message -> failwithf "expected line data, got %A" message
                  | None -> failwith "timed out waiting for port line"

                  match receive port 1000 with
                  | Some(ExitStatus status) -> assertThat status (isEqualTo 7)
                  | Some message -> failwithf "expected exit status, got %A" message
                  | None -> failwith "timed out waiting for port exit status"
                  )

          test ("port path lookup and close", fun _ ->
                  let port =
                      match startOnPath "cat" (options [] 128) with
                      | Ok port -> port
                      | Error reason -> failwithf "could not find cat on PATH: %s" reason

                  assertThat (send port "close probe\n") (isTrue)

                  match receive port 1000 with
                  | Some(Line data) -> assertThat data (isEqualTo "close probe")
                  | Some message -> failwithf "expected line data, got %A" message
                  | None -> failwith "timed out waiting for port line"

                  close port
                  )

          test ("port delivers an incomplete line when the process ends mid-line", fun _ ->
                  // `printf` with no trailing newline: the process ends mid-line. With `exit_status`
                  // enabled, ERTS delivers both the pending partial line (`{data, {noeol, ...}}`) and
                  // the exit status. Their relative order is an ERTS implementation detail — on this
                  // runtime the exit status arrives first — so assert on the two trailing messages
                  // without depending on which one is delivered first.
                  let port =
                      match startAbsolute "/bin/sh" (options [ "-c"; "printf 'partial'; exit 3" ] 128) with
                      | Ok port -> port
                      | Error reason -> failwithf "could not start port fixture: %s" reason

                  let trailing =
                      [ receive port 1000; receive port 1000 ]
                      |> List.filter (fun m -> m <> None)
                      |> List.map Option.get

                  assertThat ((trailing |> List.exists (function | IncompleteLine d -> d = "partial" | _ -> false))) (isTrue)
                  assertThat ((trailing |> List.exists (function | ExitStatus s -> s = 3 | _ -> false))) (isTrue)
                  )

          test ("port receive leaves unrelated mailbox messages behind", fun _ ->
                  // Inject an unrelated message into this process's mailbox before the port reads,
                  // to verify the selective port receive skips it and leaves it for the mailbox owner.
                  emitErlExpr () "erlang:self() ! unrelated_probe"

                  let port =
                      match startAbsolute "/bin/sh" (options [ "-c"; "read line; printf '%s\n' \"$line\"; exit 0" ] 128) with
                      | Ok port -> port
                      | Error reason -> failwithf "could not start port fixture: %s" reason

                  assertThat (send port "selective probe\n") (isTrue)

                  // Selective receive must skip the unrelated message and deliver the port line.
                  match receive port 1000 with
                  | Some(Line data) -> assertThat data (isEqualTo "selective probe")
                  | Some message -> failwithf "expected line data, got %A" message
                  | None -> failwith "timed out waiting for port line"

                  // Drain the port's exit status; selective receive still skips the unrelated message.
                  match receive port 1000 with
                  | Some(ExitStatus status) -> assertThat status (isEqualTo 0)
                  | Some message -> failwithf "expected exit status, got %A" message
                  | None -> failwith "timed out waiting for exit status"

                  // The unrelated message must still be in the mailbox, undisturbed.
                  match Erlang.receive<MailboxProbe> 1000 with
                  | Some Unrelated -> assertThat 1 (isEqualTo 1)
                  | _ -> failwith "unrelated mailbox message was lost by selective receive"
                  )

          test ("port startAbsolute rejects a relative path", fun _ ->
                  match startAbsolute "some/relative/path" (options [] 128) with
                  | Error reason -> assertThat reason (isEqualTo "path must be absolute, maxLineLength must be positive, and stderrToStdout requires useStdio")
                  | Ok _ -> failwith "expected an error for a relative path"
                  )

          test ("port startOnPath errors when the executable is missing", fun _ ->
                  match startOnPath "definitely_not_a_real_executable_xyz" (options [] 128) with
                  | Error reason -> assertThat reason (isEqualTo "executable not found on PATH")
                  | Ok _ -> failwith "expected an error for a missing executable"
                  )

          test ("port rejects a non-positive maxLineLength", fun _ ->
                  match startAbsolute "/bin/sh" (options [] 0) with
                  | Error reason -> assertThat reason (isEqualTo "path must be absolute, maxLineLength must be positive, and stderrToStdout requires useStdio")
                  | Ok _ -> failwith "expected an error for maxLineLength 0"

                  match startOnPath "cat" (options [] 0) with
                  | Error reason -> assertThat reason (isEqualTo "maxLineLength must be positive and stderrToStdout requires useStdio")
                  | Ok _ -> failwith "expected an error for maxLineLength 0"
                  )

          test ("port options redirect stderr and apply child environment", fun _ ->
                  // The shell script writes two lines to stdout (one with the configured env value
                  // and working dir) and a diagnostic line to stderr, merged via stderrToStdout.
                  let script = "printf '%s:%s\\n' \"$PORT_TEST_VALUE\" \"$PWD\"; printf 'diagnostic\\n' >&2"

                  let portOptions =
                      { PortOptions.defaultOptions with
                            arguments = [ "-c"; script ]
                            maxLineLength = 128
                            stderrToStdout = true
                            workingDirectory = Some "/tmp"
                            environment = [ "PORT_TEST_VALUE", "configured" ] }

                  let port =
                      match startAbsolute "/bin/sh" portOptions with
                      | Ok port -> port
                      | Error reason -> failwithf "could not start configured port fixture: %s" reason

                  let lines =
                      receiveUntil port 1000
                      |> List.filter (function
                          | Line _ -> true
                          | _ -> false)
                      |> List.map (function
                          | Line data -> data
                          | _ -> failwith "unreachable")

                  assertThat lines (isEqualTo [ "configured:/tmp"; "diagnostic" ])
                  )

          test ("port lifecycle operations are non-throwing after exit", fun _ ->
                  let port =
                      match startAbsolute "/bin/sh" (options [ "-c"; "exit 0" ] 128) with
                      | Ok port -> port
                      | Error reason -> failwithf "could not start port fixture: %s" reason

                  receiveUntil port 1000 |> ignore

                  match trySend port "too late\n" with
                  | Error _ -> assertThat true (isTrue)
                  | Ok () -> failwith "sending to an exited port should fail"

                  match tryClose port with
                  | Error _ -> assertThat true (isTrue)
                  | Ok () -> failwith "closing an exited port should fail"
                  )

          test ("port foldMessages keeps trailing oversized JSONL fragments after exit", fun _ ->
                  let port =
                      match startAbsolute "/bin/sh" (options [ "-c"; "printf '{\\\"value\\\":\\\"0123456789\\\"}'" ] 8) with
                      | Ok port -> port
                      | Error reason -> failwithf "could not start port fixture: %s" reason

                  let messages =
                      foldMessages port 1000 (fun messages message -> message :: messages) []
                      |> List.rev

                  assertThat ((messages |> List.exists (function | ExitStatus 0 -> true | _ -> false))) (isTrue)
                  assertThat ((messages |> List.exists (function | IncompleteLine data -> data.Contains "0123456789" || data.Length = 8 | _ -> false))) (isTrue)
                  )

          test ("monitored port emits a typed down notification", fun _ ->
                  let port =
                      match startAbsolute "/bin/sh" (options [ "-c"; "exit 0" ] 128) with
                      | Ok port -> port
                      | Error reason -> failwithf "could not start port fixture: %s" reason

                  let portMonitor = monitor port

                  match receiveDown port portMonitor 1000 with
                  | Some(Down reason) -> assertThat reason (isEqualTo "normal")
                  | None -> failwith "timed out waiting for port down notification"
                  ) ]
    )
