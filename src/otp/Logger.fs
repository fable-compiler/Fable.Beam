/// Type bindings for OTP logger module
/// See https://www.erlang.org/doc/apps/kernel/logger
module Fable.Beam.Logger

open Fable.Core
open Fable.Beam
open Fable.Beam.Maps

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Log an emergency message.
    abstract emergency: msg: string -> unit
    /// Log an emergency message with metadata or format args.
    abstract emergency: msg: string * metadataOrArgs: U2<BeamMap<Atom, obj>, obj list> -> unit
    /// Log an alert message.
    abstract alert: msg: string -> unit
    /// Log an alert message with metadata or format args.
    abstract alert: msg: string * metadataOrArgs: U2<BeamMap<Atom, obj>, obj list> -> unit
    /// Log a critical message.
    abstract critical: msg: string -> unit
    /// Log a critical message with metadata or format args.
    abstract critical: msg: string * metadataOrArgs: U2<BeamMap<Atom, obj>, obj list> -> unit
    /// Log an error message.
    abstract error: msg: string -> unit
    /// Log an error message with metadata or format args.
    abstract error: msg: string * metadataOrArgs: U2<BeamMap<Atom, obj>, obj list> -> unit
    /// Log a warning message.
    abstract warning: msg: string -> unit
    /// Log a warning message with metadata or format args.
    abstract warning: msg: string * metadataOrArgs: U2<BeamMap<Atom, obj>, obj list> -> unit
    /// Log a notice message.
    abstract notice: msg: string -> unit
    /// Log a notice message with metadata or format args.
    abstract notice: msg: string * metadataOrArgs: U2<BeamMap<Atom, obj>, obj list> -> unit
    /// Log an info message.
    abstract info: msg: string -> unit
    /// Log an info message with metadata or format args.
    abstract info: msg: string * metadataOrArgs: U2<BeamMap<Atom, obj>, obj list> -> unit
    /// Log a debug message.
    abstract debug: msg: string -> unit
    /// Log a debug message with metadata or format args.
    abstract debug: msg: string * metadataOrArgs: U2<BeamMap<Atom, obj>, obj list> -> unit
    /// Set the primary logger configuration. Common use: set_primary_config(atom "level", atom "debug")
    abstract set_primary_config: key: Atom * value: Atom -> unit
    /// Update a single key in a handler's configuration. logger:update_handler_config/3.
    /// Returns `Ok ()` or `Error reason` — OTP rejects some changes (e.g. changing a
    /// logger_std_h handler's type at runtime), so the result must not be swallowed.
    [<Emit("(fun() -> case logger:update_handler_config($0, $1, $2) of ok -> {ok, ok}; {error, LoggerUpdateHandlerCfgReason__} -> {error, LoggerUpdateHandlerCfgReason__} end end)()")>]
    abstract update_handler_config: handler: Atom * key: Atom * value: obj -> Result<unit, Dynamic>
    /// Add a primary filter. The filter is an opaque {FilterFun, Extra} tuple.
    abstract add_primary_filter: id: Atom * filter: obj -> unit
    /// Add a handler. logger:add_handler/3. Returns `Ok ()` or `Error reason`.
    /// config is an open handler-config map (e.g. logger_std_h / logger_formatter settings).
    [<Emit("(fun() -> case logger:add_handler($0, $1, $2) of ok -> {ok, ok}; {error, LoggerAddHandlerReason__} -> {error, LoggerAddHandlerReason__} end end)()")>]
    abstract add_handler: handlerId: Atom * modle: Atom * config: BeamMap<Atom, obj> -> Result<unit, Dynamic>
    /// Remove a handler. logger:remove_handler/1. Returns `Ok ()` or `Error reason`.
    [<Emit("(fun() -> case logger:remove_handler($0) of ok -> {ok, ok}; {error, LoggerRemoveHandlerReason__} -> {error, LoggerRemoveHandlerReason__} end end)()")>]
    abstract remove_handler: handlerId: Atom -> Result<unit, Dynamic>
    /// Get a handler's full configuration map. logger:get_handler_config/1.
    /// Returns `Ok config` or `Error reason` (e.g. {not_found, HandlerId}). OTP already
    /// returns {ok, Config} | {error, term()}, which matches Fable's Result representation.
    abstract get_handler_config: handlerId: Atom -> Result<BeamMap<Atom, obj>, Dynamic>
    /// Replace a handler's entire configuration. logger:set_handler_config/2. Returns `Ok ()` or `Error reason`.
    [<Emit("(fun() -> case logger:set_handler_config($0, $1) of ok -> {ok, ok}; {error, LoggerSetHandlerCfg2Reason__} -> {error, LoggerSetHandlerCfg2Reason__} end end)()")>]
    abstract set_handler_config: handlerId: Atom * config: BeamMap<Atom, obj> -> Result<unit, Dynamic>
    /// Set a single key in a handler's configuration. logger:set_handler_config/3. Returns `Ok ()` or `Error reason`.
    [<Emit("(fun() -> case logger:set_handler_config($0, $1, $2) of ok -> {ok, ok}; {error, LoggerSetHandlerCfg3Reason__} -> {error, LoggerSetHandlerCfg3Reason__} end end)()")>]
    abstract set_handler_config: handlerId: Atom * key: Atom * value: obj -> Result<unit, Dynamic>

/// logger module
[<ImportAll("logger")>]
let logger: IExports = nativeOnly
