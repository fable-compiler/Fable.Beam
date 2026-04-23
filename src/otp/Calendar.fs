/// Type bindings for Erlang calendar module (date, time, and Gregorian conversions)
/// See https://www.erlang.org/doc/apps/stdlib/calendar
module Fable.Beam.Calendar

open Fable.Core

// fsharplint:disable MemberNames

// ============================================================================
// Type aliases
// ============================================================================

/// Erlang date tuple: (Year, Month, Day). Month is 1–12, Day is 1–31.
type Date = int * int * int

/// Erlang time tuple: (Hour, Min, Sec). Hour is 0–23, Sec is 0–59.
type Time = int * int * int

/// Erlang datetime: (Date, Time) = ((Year,Month,Day),(Hour,Min,Sec)).
type DateTime = Date * Time

// ============================================================================
// Zero-arg and simple-arg functions via ImportAll
// ============================================================================

[<Erase>]
type IExports =
    /// Returns the current local date as (Year, Month, Day).
    abstract date: unit -> Date

    /// Returns the current local time as (Hour, Min, Sec).
    abstract time: unit -> Time

    /// Returns the current local datetime as ((Year,Month,Day),(Hour,Min,Sec)).
    abstract local_time: unit -> DateTime

    /// Returns the current UTC datetime as ((Year,Month,Day),(Hour,Min,Sec)).
    abstract universal_time: unit -> DateTime

    /// Returns the day of the week: 1 = Monday, 7 = Sunday.
    abstract day_of_the_week: year: int * month: int * day: int -> int

    /// Returns true if Year is a leap year.
    abstract is_leap_year: year: int -> bool

    /// Returns the last day of Month in Year (e.g. 28, 29, 30, or 31).
    abstract last_day_of_the_month: year: int * month: int -> int

    /// Converts a date to the number of days since 0000-01-01 in the proleptic Gregorian calendar.
    /// Note: Erlang uses 0000-01-01 as epoch (not the ISO 8601 epoch of 0001-01-01).
    abstract date_to_gregorian_days: year: int * month: int * day: int -> int

    /// Converts a Gregorian day count back to (Year, Month, Day).
    abstract gregorian_days_to_date: days: int -> Date

    /// Converts a Gregorian second count back to ((Year,Month,Day),(Hour,Min,Sec)).
    abstract gregorian_seconds_to_datetime: seconds: int64 -> DateTime

/// calendar module
[<ImportAll("calendar")>]
let calendar: IExports = nativeOnly

// ============================================================================
// Single-tuple-argument functions via Emit
// ============================================================================

/// Converts a datetime to the total number of Gregorian seconds since 0000-01-01 00:00:00.
/// Accepts a datetime tuple: e.g. datetimeToGregorianSeconds ((2024, 1, 1), (12, 0, 0))
[<Emit("calendar:datetime_to_gregorian_seconds($0)")>]
let datetimeToGregorianSeconds (datetime: DateTime) : int64 = nativeOnly

/// Converts a time tuple (Hour, Min, Sec) to the number of seconds since midnight.
/// Accepts a time tuple: e.g. timeToSeconds (12, 30, 0)
[<Emit("calendar:time_to_seconds($0)")>]
let timeToSeconds (time: Time) : int = nativeOnly

/// Converts a number of seconds since midnight to a time tuple (Hour, Min, Sec).
[<Emit("calendar:seconds_to_time($0)")>]
let secondsToTime (seconds: int) : Time = nativeOnly

/// Converts a local datetime to UTC.
/// Note: The result depends on the system's time zone configuration.
[<Emit("calendar:local_time_to_universal_time($0)")>]
let localTimeToUniversalTime (datetime: DateTime) : DateTime = nativeOnly

/// Converts a UTC datetime to local time.
/// Note: The result depends on the system's time zone configuration.
[<Emit("calendar:universal_time_to_local_time($0)")>]
let universalTimeToLocalTime (datetime: DateTime) : DateTime = nativeOnly
