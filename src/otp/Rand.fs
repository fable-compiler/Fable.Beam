/// Type bindings for Erlang rand module
/// See https://www.erlang.org/doc/apps/stdlib/rand
module Fable.Beam.Rand

open Fable.Core
open Fable.Beam

// fsharplint:disable MemberNames

/// Pseudo-random number generator algorithm. These compile to Erlang atoms.
/// See https://www.erlang.org/doc/apps/stdlib/rand#type-builtin_alg
type RandAlg =
    | Exsss
    | Exro928ss
    | Exs1024s
    | Exsplus
    | Exs64

/// Seeds the random number generator with the given algorithm.
[<Emit("rand:seed($0)")>]
let seed (algorithm: RandAlg) : obj = nativeOnly

/// Returns a random float uniformly distributed in the value range 0.0 =< X < 1.0.
[<Emit("rand:uniform()")>]
let uniform () : float = nativeOnly

/// Returns a random integer uniformly distributed in the value range 1 =< X =< N.
[<Emit("rand:uniform($0)")>]
let uniformInt (maximum: int) : int = nativeOnly

/// Returns a random float in the value range 2.2e-308 < X < 1.0.
/// This variant has better properties for float use cases than uniform().
[<Emit("rand:uniform_real()")>]
let uniformReal () : float = nativeOnly

/// Returns N random bytes as a binary. (OTP 24+)
[<Emit("rand:bytes($0)")>]
let bytes (count: int) : string = nativeOnly

/// Returns a float from a normal (Gaussian) distribution with mean 0.0 and variance 1.0.
[<Emit("rand:normal()")>]
let normal () : float = nativeOnly

/// Returns a float from a normal (Gaussian) distribution with the given mean and variance.
[<Emit("rand:normal($0, $1)")>]
let normalWith (mean: float) (variance: float) : float = nativeOnly
