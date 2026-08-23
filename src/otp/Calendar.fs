/// Type bindings for Erlang calendar module (date, time, and Gregorian conversions)
/// See https://www.erlang.org/doc/apps/stdlib/calendar
///
/// Note: this module exposes type aliases named `Date`, `Time`, and `DateTime` that
/// collide with `System.DateTime` and friends. If you also have `open System` in
/// scope, qualify with `Fable.Beam.Calendar.DateTime` to disambiguate.
module Fable.Beam.Calendar

open Fable.Core
open Fable.Beam

// fsharplint:disable MemberNames

// ============================================================================
// Type aliases
// ============================================================================

/// Erlang date tuple: (Year, Month, Day). Month is 1–12, Day is 1–31.
type Date = int * int * int

/// Erlang time tuple: (Hour, Min, Sec). Hour is 0–23, Min is 0–59, Sec is 0–59 (60 with leap second).
type Time = int * int * int

/// Erlang datetime: (Date, Time) = ((Year,Month,Day),(Hour,Min,Sec)).
type DateTime = Date * Time

/// Returns the current local datetime as ((Year,Month,Day),(Hour,Min,Sec)).
[<Emit("calendar:local_time()")>]
let localTime () : DateTime = nativeOnly

/// Returns the current UTC datetime as ((Year,Month,Day),(Hour,Min,Sec)).
[<Emit("calendar:universal_time()")>]
let universalTime () : DateTime = nativeOnly

/// Returns the day of the week: 1 = Monday, 7 = Sunday.
[<Emit("calendar:day_of_the_week($0, $1, $2)")>]
let dayOfWeek (year: int) (month: int) (day: int) : int = nativeOnly

/// Returns true if Year is a leap year.
[<Emit("calendar:is_leap_year($0)")>]
let isLeapYear (year: int) : bool = nativeOnly

/// Returns the last day of Month in Year (e.g. 28, 29, 30, or 31).
[<Emit("calendar:last_day_of_the_month($0, $1)")>]
let lastDayOfMonth (year: int) (month: int) : int = nativeOnly

/// Converts a date to the number of days since 0000-01-01 in the proleptic Gregorian calendar.
[<Emit("calendar:date_to_gregorian_days($0, $1, $2)")>]
let dateToGregorianDays (year: int) (month: int) (day: int) : int = nativeOnly

/// Converts a Gregorian day count back to (Year, Month, Day).
[<Emit("calendar:gregorian_days_to_date($0)")>]
let gregorianDaysToDate (days: int) : Date = nativeOnly

/// Converts a Gregorian second count back to ((Year,Month,Day),(Hour,Min,Sec)).
[<Emit("calendar:gregorian_seconds_to_datetime($0)")>]
let gregorianSecondsToDateTime (seconds: int64) : DateTime = nativeOnly

/// Converts an OS/system time to local datetime using the matching unit.
[<Emit("calendar:system_time_to_local_time($0, $1)")>]
let systemTimeToLocalTime (time: int64) (unit: TimeUnit) : DateTime = nativeOnly

/// Converts an OS/system time to UTC datetime using the matching unit.
[<Emit("calendar:system_time_to_universal_time($0, $1)")>]
let systemTimeToUniversalTime (time: int64) (unit: TimeUnit) : DateTime = nativeOnly

// ============================================================================
// Single-tuple-argument functions via Emit
// ----------------------------------------------------------------------------
// Erlang's calendar module takes tuple args (e.g. {Y,M,D} or {{Y,M,D},{H,Mi,S}}).
// Fable BEAM unpacks F# tuple-typed params into separate $N placeholders, so the
// Emit string must reconstruct the tuple inline. For DateTime, the outer 2-tuple
// is unpacked to two inner-tuple placeholders ($0 = Date, $1 = Time); for Time,
// the 3-tuple is unpacked to three int placeholders ($0..$2).
// ============================================================================

/// Converts a datetime to the total number of Gregorian seconds since 0000-01-01 00:00:00.
/// Accepts a datetime tuple: e.g. datetimeToGregorianSeconds ((2024, 1, 1), (12, 0, 0))
[<Emit("calendar:datetime_to_gregorian_seconds({$0, $1})")>]
let datetimeToGregorianSeconds (datetime: DateTime) : int64 = nativeOnly

/// Converts a time tuple (Hour, Min, Sec) to the number of seconds since midnight.
/// Accepts a time tuple: e.g. timeToSeconds (12, 30, 0)
[<Emit("calendar:time_to_seconds({$0, $1, $2})")>]
let timeToSeconds (time: Time) : int = nativeOnly

/// Converts a number of seconds since midnight to a time tuple (Hour, Min, Sec).
[<Emit("calendar:seconds_to_time($0)")>]
let secondsToTime (seconds: int) : Time = nativeOnly

/// Converts a local datetime to UTC.
/// Note: The result depends on the system's time zone configuration. For dates
/// in the DST gap or overlap, prefer calendar:local_time_to_universal_time_dst/1.
[<Emit("calendar:local_time_to_universal_time({$0, $1})")>]
let localTimeToUniversalTime (datetime: DateTime) : DateTime = nativeOnly

/// Converts a UTC datetime to local time.
/// Note: The result depends on the system's time zone configuration.
[<Emit("calendar:universal_time_to_local_time({$0, $1})")>]
let universalTimeToLocalTime (datetime: DateTime) : DateTime = nativeOnly
