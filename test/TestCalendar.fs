module Fable.Beam.Tests.Calendar

open Scriptorium.Quill
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test

open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam

module BCalendar = Fable.Beam.Calendar

let tests =
    testList (
        "Calendar",
        [ test ("is_leap_year returns true for 2000", fun _ -> assertThat (BCalendar.isLeapYear 2000) (isTrue))

          test ("is_leap_year returns true for 2024", fun _ -> assertThat (BCalendar.isLeapYear 2024) (isTrue))

          test (
              "is_leap_year returns false for 1900",
              fun _ ->
                  // 1900 is divisible by 100 but not 400 — not a leap year
                  assertThat (BCalendar.isLeapYear 1900) (isFalse)
          )

          test ("is_leap_year returns false for 2023", fun _ -> assertThat (BCalendar.isLeapYear 2023) (isFalse))

          test (
              "last_day_of_the_month returns 31 for January",
              fun _ -> assertThat (BCalendar.lastDayOfMonth 2024 1) (isEqualTo 31)
          )

          test (
              "last_day_of_the_month returns 29 for February in leap year",
              fun _ -> assertThat (BCalendar.lastDayOfMonth 2024 2) (isEqualTo 29)
          )

          test (
              "last_day_of_the_month returns 28 for February in non-leap year",
              fun _ -> assertThat (BCalendar.lastDayOfMonth 2023 2) (isEqualTo 28)
          )

          test (
              "last_day_of_the_month returns 30 for April",
              fun _ -> assertThat (BCalendar.lastDayOfMonth 2024 4) (isEqualTo 30)
          )

          test (
              "day_of_the_week returns 1 for Monday",
              fun _ ->
                  // 2024-01-01 is a Monday
                  assertThat (BCalendar.dayOfWeek 2024 1 1) (isEqualTo 1)
          )

          test (
              "day_of_the_week returns 7 for Sunday",
              fun _ ->
                  // 2024-01-07 is a Sunday
                  assertThat (BCalendar.dayOfWeek 2024 1 7) (isEqualTo 7)
          )

          test (
              "day_of_the_week returns 5 for Friday",
              fun _ ->
                  // 2024-01-05 is a Friday
                  assertThat (BCalendar.dayOfWeek 2024 1 5) (isEqualTo 5)
          )

          test (
              "date_to_gregorian_days for known date",
              fun _ ->
                  // Erlang epoch: 0000-01-01. Days to 2000-01-01 = 730485
                  assertThat (BCalendar.dateToGregorianDays 2000 1 1) (isEqualTo 730485)
          )

          test (
              "gregorian_days_to_date roundtrip",
              fun _ ->
                  let days = BCalendar.dateToGregorianDays 2024 3 15
                  let (y, m, d) = BCalendar.gregorianDaysToDate days
                  assertThat y (isEqualTo 2024)
                  assertThat m (isEqualTo 3)
                  assertThat d (isEqualTo 15)
          )

          test (
              "gregorian_days_to_date for known days",
              fun _ ->
                  let (y, m, d) = BCalendar.gregorianDaysToDate 730485
                  assertThat y (isEqualTo 2000)
                  assertThat m (isEqualTo 1)
                  assertThat d (isEqualTo 1)
          )

          test ("timeToSeconds midnight is zero", fun _ -> assertThat (BCalendar.timeToSeconds (0, 0, 0)) (isEqualTo 0))

          test (
              "timeToSeconds for noon",
              fun _ ->
                  // 12:00:00 = 12 * 3600 = 43200 seconds
                  assertThat (BCalendar.timeToSeconds (12, 0, 0)) (isEqualTo 43200)
          )

          test (
              "timeToSeconds for 1:30:30",
              fun _ ->
                  // 1*3600 + 30*60 + 30 = 5430
                  assertThat (BCalendar.timeToSeconds (1, 30, 30)) (isEqualTo 5430)
          )

          test (
              "secondsToTime roundtrip",
              fun _ ->
                  let (h, m, s) = BCalendar.secondsToTime 5430
                  assertThat h (isEqualTo 1)
                  assertThat m (isEqualTo 30)
                  assertThat s (isEqualTo 30)
          )

          test (
              "secondsToTime for noon",
              fun _ ->
                  let (h, m, s) = BCalendar.secondsToTime 43200
                  assertThat h (isEqualTo 12)
                  assertThat m (isEqualTo 0)
                  assertThat s (isEqualTo 0)
          )

          test (
              "datetimeToGregorianSeconds and back roundtrip",
              fun _ ->
                  let dt: BCalendar.DateTime = (2024, 3, 15), (10, 30, 0)
                  let secs = BCalendar.datetimeToGregorianSeconds dt
                  let ((y, mo, d), (h, mi, s)) = BCalendar.gregorianSecondsToDateTime secs
                  assertThat y (isEqualTo 2024)
                  assertThat mo (isEqualTo 3)
                  assertThat d (isEqualTo 15)
                  assertThat h (isEqualTo 10)
                  assertThat mi (isEqualTo 30)
                  assertThat s (isEqualTo 0)
          )

          test (
              "datetimeToGregorianSeconds for known value",
              fun _ ->
                  // 2000-01-01 00:00:00 = 730485 days * 86400 s/day = 63113904000
                  let secs = BCalendar.datetimeToGregorianSeconds ((2000, 1, 1), (0, 0, 0))
                  assertThat secs (isEqualTo 63113904000L)
          )

          test (
              "local_time returns plausible datetime",
              fun _ ->
                  let ((y, mo, d), (h, mi, s)) = BCalendar.localTime ()
                  assertThat (y >= 2024) (isTrue)
                  assertThat (mo >= 1 && mo <= 12) (isTrue)
                  assertThat (d >= 1 && d <= 31) (isTrue)
                  assertThat (h >= 0 && h <= 23) (isTrue)
                  assertThat (mi >= 0 && mi <= 59) (isTrue)
                  assertThat (s >= 0 && s <= 60) (isTrue)
          )

          test (
              "universal_time returns plausible datetime",
              fun _ ->
                  let ((y, _, _), _) = BCalendar.universalTime ()
                  assertThat (y >= 2024) (isTrue)
          )

          test (
              "localTimeToUniversalTime returns plausible datetime",
              fun _ ->
                  let ((y, mo, d), (h, mi, s)) =
                      BCalendar.localTimeToUniversalTime ((2024, 6, 15), (12, 0, 0))
                  // Crossing tz can shift the date by one day, so we allow the year to differ by 1.
                  assertThat (y >= 2023 && y <= 2025) (isTrue)
                  assertThat (mo >= 1 && mo <= 12) (isTrue)
                  assertThat (d >= 1 && d <= 31) (isTrue)
                  assertThat (h >= 0 && h <= 23) (isTrue)
                  assertThat (mi >= 0 && mi <= 59) (isTrue)
                  assertThat (s >= 0 && s <= 60) (isTrue)
          )

          test (
              "universalTimeToLocalTime returns plausible datetime",
              fun _ ->
                  let ((y, mo, d), (h, mi, s)) =
                      BCalendar.universalTimeToLocalTime ((2024, 6, 15), (12, 0, 0))

                  assertThat (y >= 2023 && y <= 2025) (isTrue)
                  assertThat (mo >= 1 && mo <= 12) (isTrue)
                  assertThat (d >= 1 && d <= 31) (isTrue)
                  assertThat (h >= 0 && h <= 23) (isTrue)
                  assertThat (mi >= 0 && mi <= 59) (isTrue)
                  assertThat (s >= 0 && s <= 60) (isTrue)
          )

          test (
              "localTimeToUniversalTime then back roundtrips",
              fun _ ->
                  let original: BCalendar.DateTime = (2024, 6, 15), (12, 0, 0)
                  let utc = BCalendar.localTimeToUniversalTime original
                  let roundtrip = BCalendar.universalTimeToLocalTime utc
                  assertThat roundtrip (isEqualTo original)
          )

          test (
              "system_time_to_universal_time for unix epoch",
              fun _ ->
                  // Unix epoch 0 seconds = 1970-01-01 00:00:00 UTC
                  let ((y, mo, d), (h, mi, s)) =
                      BCalendar.systemTimeToUniversalTime 0L TimeUnit.Second

                  assertThat y (isEqualTo 1970)
                  assertThat mo (isEqualTo 1)
                  assertThat d (isEqualTo 1)
                  assertThat h (isEqualTo 0)
                  assertThat mi (isEqualTo 0)
                  assertThat s (isEqualTo 0)
          )

          test (
              "system_time_to_universal_time for known second",
              fun _ ->
                  // 1700000000 seconds since the Unix epoch = 2023-11-14 22:13:20 UTC
                  let ((y, mo, d), (h, mi, s)) =
                      BCalendar.systemTimeToUniversalTime 1700000000L TimeUnit.Second

                  assertThat y (isEqualTo 2023)
                  assertThat mo (isEqualTo 11)
                  assertThat d (isEqualTo 14)
                  assertThat h (isEqualTo 22)
                  assertThat mi (isEqualTo 13)
                  assertThat s (isEqualTo 20)
          )

          test (
              "system_time_to_local_time returns plausible datetime",
              fun _ ->
                  // Local time depends on the system time zone, so only assert structural validity.
                  let ((y, mo, d), (h, mi, s)) =
                      BCalendar.systemTimeToLocalTime 1700000000L TimeUnit.Second

                  assertThat (y >= 2023 && y <= 2024) (isTrue)
                  assertThat (mo >= 1 && mo <= 12) (isTrue)
                  assertThat (d >= 1 && d <= 31) (isTrue)
                  assertThat (h >= 0 && h <= 23) (isTrue)
                  assertThat (mi >= 0 && mi <= 59) (isTrue)
                  assertThat (s >= 0 && s <= 60) (isTrue)
          ) ]
    )
