/// Type bindings for OTP logger module
/// See https://www.erlang.org/doc/apps/kernel/logger
module Fable.Beam.Logger

open Fable.Core

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Log an emergency message.
    abstract emergency: msg: string -> unit
    /// Log an emergency message with metadata or format args.
    abstract emergency: msg: string * metadataOrArgs: obj -> unit
    /// Log an alert message.
    abstract alert: msg: string -> unit
    /// Log an alert message with metadata or format args.
    abstract alert: msg: string * metadataOrArgs: obj -> unit
    /// Log a critical message.
    abstract critical: msg: string -> unit
    /// Log a critical message with metadata or format args.
    abstract critical: msg: string * metadataOrArgs: obj -> unit
    /// Log an error message.
    abstract error: msg: string -> unit
    /// Log an error message with metadata or format args.
    abstract error: msg: string * metadataOrArgs: obj -> unit
    /// Log a warning message.
    abstract warning: msg: string -> unit
    /// Log a warning message with metadata or format args.
    abstract warning: msg: string * metadataOrArgs: obj -> unit
    /// Log a notice message.
    abstract notice: msg: string -> unit
    /// Log a notice message with metadata or format args.
    abstract notice: msg: string * metadataOrArgs: obj -> unit
    /// Log an info message.
    abstract info: msg: string -> unit
    /// Log an info message with metadata or format args.
    abstract info: msg: string * metadataOrArgs: obj -> unit
    /// Log a debug message.
    abstract debug: msg: string -> unit
    /// Log a debug message with metadata or format args.
    abstract debug: msg: string * metadataOrArgs: obj -> unit

/// logger module
[<ImportAll("logger")>]
let logger: IExports = nativeOnly
