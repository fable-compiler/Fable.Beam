module Fable.Beam.Tests.GenTcp

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core

module BGenTcp = Fable.Beam.GenTcp

[<Emit("test_tcp_server:start()")>]
let private startServer () : int = nativeOnly

let tests =
    testList (
        "GenTcp",
        [
            test (
                "passive line client times out, sends, receives, and observes close",
                fun _ ->
                    let port = startServer ()

                    let socket =
                        match BGenTcp.connectLineClient "127.0.0.1" port with
                        | Ok socket -> socket
                        | Error reason -> failwithf "TCP connection failed: %s" reason

                    assertThat (BGenTcp.recv socket 0 20) (isEqualTo (Error "timeout"))
                    assertThat (BGenTcp.send socket "hello from gen_tcp\n") (isEqualTo (Ok()))

                    assertThat (BGenTcp.recv socket 0 1000) (isEqualTo (Ok "hello from gen_tcp\n"))

                    assertThat (BGenTcp.recv socket 0 1000) (isEqualTo (Error "closed"))
                    BGenTcp.close socket
            )

            test (
                "connect rejects a negative timeout before calling OTP",
                fun _ ->
                    let options =
                        { BGenTcp.ConnectOptions.defaultOptions with
                            connectTimeoutMs = Some -1
                        }

                    assertThat
                        (BGenTcp.connect "127.0.0.1" 1 options)
                        (isEqualTo (Error "connect timeout must be non-negative"))
            )
        ]
    )
