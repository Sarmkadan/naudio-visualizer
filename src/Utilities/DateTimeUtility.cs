#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Globalization;

namespace NAudioVisualizer.Utilities;

/// <summary>
/// Provides utility methods for date and time operations.
/// Includes formatting, calculations, and time zone handling.
/// </summary>
public static class DateTimeUtility
{
    /// <summary>
    /// Gets the current time in milliseconds since epoch.
    /// </summary>
    public static long GetCurrentTimestampMs()
    {
        return (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalMilliseconds;
    }

    /// <summary>
    /// Converts a DateTime to milliseconds since epoch.
    /// </summary>
    public static long ToTimestampMs(DateTime dateTime)
    {
        return (long)(dateTime - DateTime.UnixEpoch).TotalMilliseconds;
    }

    /// <summary>
    /// Converts milliseconds since epoch to a DateTime.
    /// </summary>
    public static DateTime FromTimestampMs(long timestampMs)
    {
        return DateTime.UnixEpoch.AddMilliseconds(timestampMs);
    }

    /// <summary>
    /// Formats a DateTime with ISO 8601 format.
    /// </summary>
    public static string ToIso8601(DateTime dateTime)
    {
        return dateTime.ToUniversalTime().ToString("o");
    }

    /// <summary>
    /// Parses an ISO 8601 formatted string to DateTime.
    /// </summary>
    public static DateTime FromIso8601(string isoString)
    {
        if (!DateTime.TryParse(isoString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result))
            throw new FormatException($"Invalid ISO 8601 format: {isoString}");

        return result;
    }

    /// <summary>
    /// Formats a TimeSpan to a human-readable string.
    /// </summary>
    public static string FormatDuration(TimeSpan duration)
    {
        if (duration.TotalSeconds < 1)
            return $"{duration.TotalMilliseconds:F0}ms";

        if (duration.TotalMinutes < 1)
            return $"{duration.TotalSeconds:F1}s";

        if (duration.TotalHours < 1)
            return $"{duration.TotalMinutes:F1}m";

        return $"{duration.TotalHours:F1}h";
    }

    /// <summary>
    /// Gets the number of days between two dates.
    /// </summary>
    public static int DaysBetween(DateTime date1, DateTime date2)
    {
        return Math.Abs((date2.Date - date1.Date).Days);
    }

    /// <summary>
    /// Checks if a date is today.
    /// </summary>
    public static bool IsToday(DateTime dateTime)
    {
        return dateTime.Date == DateTime.Today;
    }

    /// <summary>
    /// Checks if a date is in the past.
    /// </summary>
    public static bool IsInPast(DateTime dateTime)
    {
        return dateTime < DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if a date is in the future.
    /// </summary>
    public static bool IsInFuture(DateTime dateTime)
    {
        return dateTime > DateTime.UtcNow;
    }

    /// <summary>
    /// Gets a human-readable relative time string (e.g., "2 hours ago").
    /// </summary>
    public static string GetRelativeTime(DateTime dateTime)
    {
        var now = DateTime.UtcNow;
        var diff = now - dateTime;

        if (diff.TotalSeconds < 60)
            return $"{diff.TotalSeconds:F0} seconds ago";

        if (diff.TotalMinutes < 60)
            return $"{diff.TotalMinutes:F0} minutes ago";

        if (diff.TotalHours < 24)
            return $"{diff.TotalHours:F0} hours ago";

        if (diff.TotalDays < 7)
            return $"{diff.TotalDays:F0} days ago";

        if (diff.TotalDays < 30)
            return $"{diff.TotalDays / 7:F0} weeks ago";

        if (diff.TotalDays < 365)
            return $"{diff.TotalDays / 30:F0} months ago";

        return $"{diff.TotalDays / 365:F0} years ago";
    }

    /// <summary>
    /// Gets the start of the day for a given date.
    /// </summary>
    public static DateTime GetStartOfDay(DateTime dateTime)
    {
        return dateTime.Date;
    }

    /// <summary>
    /// Gets the end of the day for a given date.
    /// </summary>
    public static DateTime GetEndOfDay(DateTime dateTime)
    {
        return dateTime.Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// Gets the start of the week (Monday).
    /// </summary>
    public static DateTime GetStartOfWeek(DateTime dateTime)
    {
        int daysToMonday = ((int)dateTime.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return dateTime.Date.AddDays(-daysToMonday);
    }

    /// <summary>
    /// Gets the start of the month.
    /// </summary>
    public static DateTime GetStartOfMonth(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, 1);
    }

    /// <summary>
    /// Gets the end of the month.
    /// </summary>
    public static DateTime GetEndOfMonth(DateTime dateTime)
    {
        var lastDay = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
        return new DateTime(dateTime.Year, dateTime.Month, lastDay);
    }

    /// <summary>
    /// Calculates the age in years from a birth date.
    /// </summary>
    public static int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        int age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age))
            age--;

        return age;
    }

    /// <summary>
    /// Gets the weekday name for a date.
    /// </summary>
    public static string GetDayName(DateTime dateTime)
    {
        return dateTime.ToString("dddd");
    }

    /// <summary>
    /// Checks if a year is a leap year.
    /// </summary>
    public static bool IsLeapYear(int year)
    {
        return DateTime.IsLeapYear(year);
    }
}
