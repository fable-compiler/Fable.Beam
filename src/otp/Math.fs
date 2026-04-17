/// Type bindings for Erlang math module
/// See https://www.erlang.org/doc/apps/stdlib/math
module Fable.Beam.Math

open Fable.Core

// fsharplint:disable MemberNames

[<Erase>]
type IExports =
    /// Returns the sine of X (radians).
    abstract sin: x: float -> float
    /// Returns the cosine of X (radians).
    abstract cos: x: float -> float
    /// Returns the tangent of X (radians).
    abstract tan: x: float -> float
    /// Returns the arcsine of X in radians.
    abstract asin: x: float -> float
    /// Returns the arccosine of X in radians.
    abstract acos: x: float -> float
    /// Returns the arctangent of X in radians.
    abstract atan: x: float -> float
    /// Returns the arctangent of Y/X in radians, using the signs of both to determine the quadrant.
    abstract atan2: y: float * x: float -> float
    /// Returns the hyperbolic sine of X.
    abstract sinh: x: float -> float
    /// Returns the hyperbolic cosine of X.
    abstract cosh: x: float -> float
    /// Returns the hyperbolic tangent of X.
    abstract tanh: x: float -> float
    /// Returns the inverse hyperbolic sine of X. (OTP 20+)
    abstract asinh: x: float -> float
    /// Returns the inverse hyperbolic cosine of X. (OTP 20+)
    abstract acosh: x: float -> float
    /// Returns the inverse hyperbolic tangent of X. (OTP 20+)
    abstract atanh: x: float -> float
    /// Returns e raised to the power of X.
    abstract exp: x: float -> float
    /// Returns the natural logarithm (base e) of X.
    abstract log: x: float -> float
    /// Returns the base-2 logarithm of X. (OTP 18+)
    abstract log2: x: float -> float
    /// Returns the base-10 logarithm of X.
    abstract log10: x: float -> float
    /// Returns X raised to the power of Y.
    abstract pow: x: float * y: float -> float
    /// Returns the non-negative square root of X.
    abstract sqrt: x: float -> float
    /// Returns the ceiling of X as a float. (OTP 20+)
    abstract ceil: x: float -> float
    /// Returns the floor of X as a float. (OTP 20+)
    abstract floor: x: float -> float
    /// Returns the floating-point remainder of X/Y.
    abstract fmod: x: float * y: float -> float
    /// Returns the value of pi (3.14159...).
    abstract pi: unit -> float

/// math module
[<ImportAll("math")>]
let math: IExports = nativeOnly
