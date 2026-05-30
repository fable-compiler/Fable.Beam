/// Fable bindings for Cowboy's cowboy_router module.
/// See: https://ninenines.eu/docs/en/cowboy/2.14/manual/cowboy_router/
module Fable.Beam.Cowboy.CowboyRouter

open Fable.Core
open Fable.Beam

/// A compiled Cowboy dispatch table, produced by `compile` and passed to
/// `Cowboy.protocolOpts`. Erased at runtime.
[<Erase>]
type Dispatch = Dispatch of obj

/// A single route: `{Path, Handler, InitialState}`. Erased at runtime.
[<Erase>]
type Route = Route of obj

/// A host rule pairing a host matcher with its routes. Erased at runtime.
[<Erase>]
type HostRule = HostRule of obj

/// A host matcher: a host pattern or the wildcard `'_'`. Erased at runtime.
[<Erase>]
type HostMatch = HostMatch of obj

/// Wildcard host matcher (`'_'`) — matches any host.
[<Emit("'_'")>]
let wildcard: HostMatch = nativeOnly

/// Match a specific host pattern, e.g. "example.com" or ":subdomain.example.org".
[<Emit("$0")>]
let host (pattern: string) : HostMatch = nativeOnly

/// Build a route: `{Path, Handler, InitialState}`.
/// Handler is the module atom that implements the cowboy_handler behaviour.
[<Emit("{$0, $1, $2}")>]
let route (path: string) (handler: Atom) (state: 'State) : Route = nativeOnly

/// Build a host rule pairing a host matcher with its routes.
[<Emit("{$0, $1}")>]
let hostRule (host: HostMatch) (routes: Route list) : HostRule = nativeOnly

/// Compile host rules into a dispatch table.
[<Emit("cowboy_router:compile($0)")>]
let compile (rules: HostRule list) : Dispatch = nativeOnly
