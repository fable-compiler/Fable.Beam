/// Type bindings for Erlang rand module
/// See https://www.erlang.org/doc/apps/stdlib/rand
module Fable.Beam.Rand

open Fable.Core

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Returns a random float uniformly distributed in the value range 0.0 =< X < 1.0.
    abstract uniform: unit -> float

    /// Returns a random integer uniformly distributed in the value range 1 =< X =< N.
    abstract uniform: n: int -> int

    /// Returns a random float in the value range 2.2e-308 < X < 1.0.
    /// This variant has better properties for float use cases than uniform().
    abstract uniform_real: unit -> float

    /// Returns N random bytes as a binary. (OTP 24+)
    abstract bytes: n: int -> string

    /// Returns a float from a normal (Gaussian) distribution with mean 0.0 and variance 1.0.
    abstract normal: unit -> float

    /// Returns a float from a normal (Gaussian) distribution with the given Mean and Variance.
    abstract normal: mean: float * variance: float -> float

/// rand module
[<ImportAll("rand")>]
let rand: IExports = nativeOnly
