/// Type bindings for Erlang math module
/// See https://www.erlang.org/doc/apps/stdlib/math
module Fable.Beam.Math

open Fable.Core

// fsharplint:disable MemberNames

/// Returns the sine of X (radians).
[<Emit("math:sin($0)")>]
let sin (x: float) : float = nativeOnly

/// Returns the cosine of X (radians).
[<Emit("math:cos($0)")>]
let cos (x: float) : float = nativeOnly

/// Returns the tangent of X (radians).
[<Emit("math:tan($0)")>]
let tan (x: float) : float = nativeOnly

/// Returns the arcsine of X in radians.
[<Emit("math:asin($0)")>]
let asin (x: float) : float = nativeOnly

/// Returns the arccosine of X in radians.
[<Emit("math:acos($0)")>]
let acos (x: float) : float = nativeOnly

/// Returns the arctangent of X in radians.
[<Emit("math:atan($0)")>]
let atan (x: float) : float = nativeOnly

/// Returns the arctangent of Y/X, using the signs of both to determine the quadrant.
[<Emit("math:atan2($0, $1)")>]
let atan2 (y: float) (x: float) : float = nativeOnly

/// Returns the hyperbolic sine of X.
[<Emit("math:sinh($0)")>]
let sinh (x: float) : float = nativeOnly

/// Returns the hyperbolic cosine of X.
[<Emit("math:cosh($0)")>]
let cosh (x: float) : float = nativeOnly

/// Returns the hyperbolic tangent of X.
[<Emit("math:tanh($0)")>]
let tanh (x: float) : float = nativeOnly

/// Returns the inverse hyperbolic sine of X. (OTP 20+)
[<Emit("math:asinh($0)")>]
let asinh (x: float) : float = nativeOnly

/// Returns the inverse hyperbolic cosine of X. (OTP 20+)
[<Emit("math:acosh($0)")>]
let acosh (x: float) : float = nativeOnly

/// Returns the inverse hyperbolic tangent of X. (OTP 20+)
[<Emit("math:atanh($0)")>]
let atanh (x: float) : float = nativeOnly

/// Returns e raised to the power of X.
[<Emit("math:exp($0)")>]
let exp (x: float) : float = nativeOnly

/// Returns the natural logarithm (base e) of X.
[<Emit("math:log($0)")>]
let log (x: float) : float = nativeOnly

/// Returns the base-2 logarithm of X. (OTP 18+)
[<Emit("math:log2($0)")>]
let log2 (x: float) : float = nativeOnly

/// Returns the base-10 logarithm of X.
[<Emit("math:log10($0)")>]
let log10 (x: float) : float = nativeOnly

/// Returns X raised to the power of Y.
[<Emit("math:pow($0, $1)")>]
let pow (x: float) (y: float) : float = nativeOnly

/// Returns the non-negative square root of X.
[<Emit("math:sqrt($0)")>]
let sqrt (x: float) : float = nativeOnly

/// Returns the ceiling of X as a float. (OTP 20+)
[<Emit("math:ceil($0)")>]
let ceil (x: float) : float = nativeOnly

/// Returns the floor of X as a float. (OTP 20+)
[<Emit("math:floor($0)")>]
let floor (x: float) : float = nativeOnly

/// Returns the floating-point remainder of X/Y.
[<Emit("math:fmod($0, $1)")>]
let fmod (x: float) (y: float) : float = nativeOnly

/// Returns the value of pi (3.14159...).
[<Emit("math:pi()")>]
let pi () : float = nativeOnly
