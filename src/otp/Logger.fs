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

/// OTP log severity levels (logger:level/0). Each case compiles to the matching
/// Erlang atom (`emergency`..`debug`). `RequireQualifiedAccess` avoids the clash
/// between the `Error` case and the `Result.Error` constructor.
[<RequireQualifiedAccess>]
type LogLevel =
    | Emergency
    | Alert
    | Critical
    | Error
    | Warning
    | Notice
    | Info
    | Debug

// ============================================================================
// Typed filters
// ----------------------------------------------------------------------------
// OTP filters are an opaque {FilterFun, Extra} tuple where FilterFun is an
// arity-2 fun `(log_event(), Extra) -> log_event() | stop | ignore`. These
// helpers let callers stay in F#: an F# `System.Func<LogEvent, 'Extra,
// FilterReturn>` compiles to the required arity-2 Erlang fun, and the return
// constructors (`pass`/`stop`/`ignore`) emit the raw terms OTP expects (a DU
// would compile to a tagged tuple and break the contract).
// See https://www.erlang.org/doc/apps/kernel/logger#type-filter
// ============================================================================

/// Helpers for typed logger filters.
module Filter =

    /// An OTP log event map `#{level := level(), msg := ..., meta := metadata()}`.
    /// Opaque — read it with the accessors below.
    [<Erase>]
    type LogEvent = LogEvent of obj

    /// The result of a filter function: pass the (possibly rewritten) event on,
    /// `stop` to discard it, or `ignore` to defer to the configured filter_default.
    [<Erase>]
    type FilterReturn = FilterReturn of obj

    /// The event's severity level (e.g. `LogLevel.Info` or `LogLevel.Error`).
    [<Emit("maps:get(level, $0)")>]
    let level (event: LogEvent) : LogLevel = nativeOnly

    /// The event's metadata map.
    [<Emit("maps:get(meta, $0)")>]
    let meta (event: LogEvent) : BeamMap<Atom, obj> = nativeOnly

    /// The event's message term — one of `{Format, Args}`, `{report, Report}`,
    /// or `{string, Chardata}`. Narrow it with the `Decode` combinators.
    [<Emit("maps:get(msg, $0)")>]
    let msg (event: LogEvent) : Dynamic = nativeOnly

    /// Pass the event on to the next filter / handler. Return the event unchanged
    /// or one you have rewritten (e.g. with added metadata).
    [<Emit("$0")>]
    let pass (event: LogEvent) : FilterReturn = nativeOnly

    /// Discard the event immediately (Erlang `stop`).
    [<Emit("stop")>]
    let stop: FilterReturn = nativeOnly

    /// Express no opinion; defer to the configured filter_default (Erlang `ignore`).
    [<Emit("ignore")>]
    let ignore: FilterReturn = nativeOnly

    /// Add a primary filter built from an F# function. The filter receives each
    /// event plus `extra` and returns `pass`/`stop`/`ignore`.
    /// Wraps logger:add_primary_filter/2; returns `Ok ()` or `Error reason`
    /// (e.g. `{invalid_filter, _}`, or already-present).
    [<Emit("(fun() -> case logger:add_primary_filter($0, {$1, $2}) of ok -> {ok, ok}; {error, AddPrimaryFilterReason__} -> {error, AddPrimaryFilterReason__} end end)()")>]
    let addPrimary
        (id: Atom)
        (filter: System.Func<LogEvent, 'Extra, FilterReturn>)
        (extra: 'Extra)
        : Result<unit, Dynamic> =
        nativeOnly

    /// Remove a primary filter by id. logger:remove_primary_filter/1.
    /// Returns `Ok ()` or `Error reason` (e.g. `{not_found, FilterId}`).
    [<Emit("(fun() -> case logger:remove_primary_filter($0) of ok -> {ok, ok}; {error, RemovePrimaryFilterReason__} -> {error, RemovePrimaryFilterReason__} end end)()")>]
    let removePrimary (id: Atom) : Result<unit, Dynamic> = nativeOnly

// ============================================================================
// logger_formatter templates
// ----------------------------------------------------------------------------
// A logger_formatter template is a heterogeneous list of metadata keys, literal
// strings, and `{Key, IfExists, Else}` conditionals. An F# list literal compiles
// to a native Erlang proper list, and each erased `Item` constructor emits its
// raw element, so `Item list` drops straight into the formatter config.
// See https://www.erlang.org/doc/apps/kernel/logger_formatter#type-template
// ============================================================================

/// Helpers for building logger_formatter templates and applying them.
module Formatter =

    /// One element of a logger_formatter template.
    [<Erase>]
    type Item = Item of obj

    /// A metadata key placeholder, replaced by its value (e.g. `time`, `level`, `msg`).
    [<Emit("$0")>]
    let key (metaKey: Atom) : Item = nativeOnly

    /// A nested metadata key path, e.g. `[mfa]` or `[my; nested; field]`.
    [<Emit("$0")>]
    let path (keys: Atom list) : Item = nativeOnly

    /// A literal string, printed verbatim.
    [<Emit("$0")>]
    let text (s: string) : Item = nativeOnly

    /// Conditional: if `metaKey` exists in the metadata, render `ifExists`,
    /// otherwise render `orElse`.
    [<Emit("{$0, $1, $2}")>]
    let cond (metaKey: Atom) (ifExists: Item list) (orElse: Item list) : Item = nativeOnly

    /// Merge a template (and the single-line flag) into a handler's logger_formatter
    /// config. Wraps logger:update_formatter_config/2; returns `Ok ()` or `Error reason`.
    [<Emit("(fun() -> case logger:update_formatter_config($0, #{single_line => $1, template => $2}) of ok -> {ok, ok}; {error, UpdateFormatterCfgReason__} -> {error, UpdateFormatterCfgReason__} end end)()")>]
    let setTemplate (handlerId: Atom) (singleLine: bool) (template: Item list) : Result<unit, Dynamic> = nativeOnly
