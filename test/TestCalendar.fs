module Fable.Beam.Tests.Calendar

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam.Calendar
#endif

// ============================================================================
// is_leap_year
// ============================================================================

[<Fact>]
let ``test calendar.is_leap_year returns true for 2000`` () =
#if FABLE_COMPILER
    calendar.is_leap_year 2000 |> equal true
#else
    ()
#endif

[<Fact>]
let ``test calendar.is_leap_year returns true for 2024`` () =
#if FABLE_COMPILER
    calendar.is_leap_year 2024 |> equal true
#else
    ()
#endif

[<Fact>]
let ``test calendar.is_leap_year returns false for 1900`` () =
#if FABLE_COMPILER
    // 1900 is divisible by 100 but not 400 — not a leap year
    calendar.is_leap_year 1900 |> equal false
#else
    ()
#endif

[<Fact>]
let ``test calendar.is_leap_year returns false for 2023`` () =
#if FABLE_COMPILER
    calendar.is_leap_year 2023 |> equal false
#else
    ()
#endif

// ============================================================================
// last_day_of_the_month
// ============================================================================

[<Fact>]
let ``test calendar.last_day_of_the_month returns 31 for January`` () =
#if FABLE_COMPILER
    calendar.last_day_of_the_month (2024, 1) |> equal 31
#else
    ()
#endif

[<Fact>]
let ``test calendar.last_day_of_the_month returns 29 for February in leap year`` () =
#if FABLE_COMPILER
    calendar.last_day_of_the_month (2024, 2) |> equal 29
#else
    ()
#endif

[<Fact>]
let ``test calendar.last_day_of_the_month returns 28 for February in non-leap year`` () =
#if FABLE_COMPILER
    calendar.last_day_of_the_month (2023, 2) |> equal 28
#else
    ()
#endif

[<Fact>]
let ``test calendar.last_day_of_the_month returns 30 for April`` () =
#if FABLE_COMPILER
    calendar.last_day_of_the_month (2024, 4) |> equal 30
#else
    ()
#endif

// ============================================================================
// day_of_the_week
// ============================================================================

[<Fact>]
let ``test calendar.day_of_the_week returns 1 for Monday`` () =
#if FABLE_COMPILER
    // 2024-01-01 is a Monday
    calendar.day_of_the_week (2024, 1, 1) |> equal 1
#else
    ()
#endif

[<Fact>]
let ``test calendar.day_of_the_week returns 7 for Sunday`` () =
#if FABLE_COMPILER
    // 2024-01-07 is a Sunday
    calendar.day_of_the_week (2024, 1, 7) |> equal 7
#else
    ()
#endif

[<Fact>]
let ``test calendar.day_of_the_week returns 5 for Friday`` () =
#if FABLE_COMPILER
    // 2024-01-05 is a Friday
    calendar.day_of_the_week (2024, 1, 5) |> equal 5
#else
    ()
#endif

// ============================================================================
// date_to_gregorian_days / gregorian_days_to_date roundtrip
// ============================================================================

[<Fact>]
let ``test calendar.date_to_gregorian_days for known date`` () =
#if FABLE_COMPILER
    // Erlang epoch: 0000-01-01. Days to 2000-01-01 = 730485
    calendar.date_to_gregorian_days (2000, 1, 1) |> equal 730485
#else
    ()
#endif

[<Fact>]
let ``test calendar.gregorian_days_to_date roundtrip`` () =
#if FABLE_COMPILER
    let days = calendar.date_to_gregorian_days (2024, 3, 15)
    let (y, m, d) = calendar.gregorian_days_to_date days
    y |> equal 2024
    m |> equal 3
    d |> equal 15
#else
    ()
#endif

[<Fact>]
let ``test calendar.gregorian_days_to_date for known days`` () =
#if FABLE_COMPILER
    let (y, m, d) = calendar.gregorian_days_to_date 730485
    y |> equal 2000
    m |> equal 1
    d |> equal 1
#else
    ()
#endif

// ============================================================================
// timeToSeconds / secondsToTime roundtrip
// ============================================================================

[<Fact>]
let ``test calendar.timeToSeconds midnight is zero`` () =
#if FABLE_COMPILER
    timeToSeconds (0, 0, 0) |> equal 0
#else
    ()
#endif

[<Fact>]
let ``test calendar.timeToSeconds for noon`` () =
#if FABLE_COMPILER
    // 12:00:00 = 12 * 3600 = 43200 seconds
    timeToSeconds (12, 0, 0) |> equal 43200
#else
    ()
#endif

[<Fact>]
let ``test calendar.timeToSeconds for 1:30:30`` () =
#if FABLE_COMPILER
    // 1*3600 + 30*60 + 30 = 5430
    timeToSeconds (1, 30, 30) |> equal 5430
#else
    ()
#endif

[<Fact>]
let ``test calendar.secondsToTime roundtrip`` () =
#if FABLE_COMPILER
    let (h, m, s) = secondsToTime 5430
    h |> equal 1
    m |> equal 30
    s |> equal 30
#else
    ()
#endif

[<Fact>]
let ``test calendar.secondsToTime for noon`` () =
#if FABLE_COMPILER
    let (h, m, s) = secondsToTime 43200
    h |> equal 12
    m |> equal 0
    s |> equal 0
#else
    ()
#endif

// ============================================================================
// datetimeToGregorianSeconds / gregorian_seconds_to_datetime roundtrip
// ============================================================================

[<Fact>]
let ``test calendar.datetimeToGregorianSeconds and back roundtrip`` () =
#if FABLE_COMPILER
    let dt: DateTime = (2024, 3, 15), (10, 30, 0)
    let secs = datetimeToGregorianSeconds dt
    let ((y, mo, d), (h, mi, s)) = calendar.gregorian_seconds_to_datetime secs
    y |> equal 2024
    mo |> equal 3
    d |> equal 15
    h |> equal 10
    mi |> equal 30
    s |> equal 0
#else
    ()
#endif

[<Fact>]
let ``test calendar.datetimeToGregorianSeconds for known value`` () =
#if FABLE_COMPILER
    // 2000-01-01 00:00:00 = 730485 days * 86400 s/day = 63113904000
    let secs = datetimeToGregorianSeconds ((2000, 1, 1), (0, 0, 0))
    secs |> equal 63113904000L
#else
    ()
#endif

// ============================================================================
// date / time / local_time / universal_time (sanity checks)
// ============================================================================

[<Fact>]
let ``test calendar.date returns plausible year`` () =
#if FABLE_COMPILER
    let (y, m, d) = calendar.date ()
    (y >= 2024) |> equal true
    (m >= 1 && m <= 12) |> equal true
    (d >= 1 && d <= 31) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test calendar.time returns plausible time`` () =
#if FABLE_COMPILER
    let (h, m, s) = calendar.time ()
    (h >= 0 && h <= 23) |> equal true
    (m >= 0 && m <= 59) |> equal true
    (s >= 0 && s <= 60) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test calendar.local_time returns plausible datetime`` () =
#if FABLE_COMPILER
    let ((y, mo, d), (h, mi, s)) = calendar.local_time ()
    (y >= 2024) |> equal true
    (mo >= 1 && mo <= 12) |> equal true
    (d >= 1 && d <= 31) |> equal true
    (h >= 0 && h <= 23) |> equal true
    (mi >= 0 && mi <= 59) |> equal true
    (s >= 0 && s <= 60) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test calendar.universal_time returns plausible datetime`` () =
#if FABLE_COMPILER
    let ((y, _, _), _) = calendar.universal_time ()
    (y >= 2024) |> equal true
#else
    ()
#endif
