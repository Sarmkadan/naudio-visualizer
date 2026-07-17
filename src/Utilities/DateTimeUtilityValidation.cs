#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.Utilities;

/// <summary>
/// Provides validation helpers for DateTimeUtility operations.
/// Validates input parameters and ensures data integrity before processing.
/// </summary>
public static class DateTimeUtilityValidation
{
    /// <summary>
    /// Validates that the provided DateTime is not the default (uninitialized) value.
    /// </summary>
    /// <param name="dateTime">The DateTime to validate.</param>
    /// <param name="paramName">Name of the parameter for exception messages.</param>
    /// <exception cref="ArgumentException">Thrown if the date is default/MinValue.</exception>
    public static void ValidateDateTime(DateTime dateTime, string paramName = "dateTime")
    {
        if (dateTime == default)
        {
            throw new ArgumentException("DateTime cannot be default (uninitialized).", paramName);
        }
    }

    /// <summary>
    /// Validates that the provided DateTime is not MinValue.
    /// </summary>
    /// <param name="dateTime">The DateTime to validate.</param>
    /// <param name="paramName">Name of the parameter for exception messages.</param>
    /// <exception cref="ArgumentException">Thrown if the date is DateTime.MinValue.</exception>
    public static void ValidateDateTimeNotMinValue(DateTime dateTime, string paramName = "dateTime")
    {
        if (dateTime == DateTime.MinValue)
        {
            throw new ArgumentException("DateTime cannot be DateTime.MinValue.", paramName);
        }
    }

    /// <summary>
    /// Validates that the provided DateTime is not MaxValue.
    /// </summary>
    /// <param name="dateTime">The DateTime to validate.</param>
    /// <param name="paramName">Name of the parameter for exception messages.</param>
    /// <exception cref="ArgumentException">Thrown if the date is DateTime.MaxValue.</exception>
    public static void ValidateDateTimeNotMaxValue(DateTime dateTime, string paramName = "dateTime")
    {
        if (dateTime == DateTime.MaxValue)
        {
            throw new ArgumentException("DateTime cannot be DateTime.MaxValue.", paramName);
        }
    }

    /// <summary>
    /// Validates that the provided DateTime is a valid date (not MinValue or MaxValue).
    /// </summary>
    /// <param name="dateTime">The DateTime to validate.</param>
    /// <param name="paramName">Name of the parameter for exception messages.</param>
    /// <exception cref="ArgumentException">Thrown if the date is invalid.</exception>
    public static void ValidateDateTimeIsValid(DateTime dateTime, string paramName = "dateTime")
    {
        if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue)
        {
            throw new ArgumentException("DateTime must be a valid date (not MinValue or MaxValue).", paramName);
        }
    }

    /// <summary>
    /// Validates that the provided string is not null or empty for ISO 8601 parsing.
    /// </summary>
    /// <param name="isoString">The ISO 8601 string to validate.</param>
    /// <param name="paramName">Name of the parameter for exception messages.</param>
    /// <exception cref="ArgumentNullException">Thrown if the string is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the string is empty.</exception>
    public static void ValidateIso8601String(string? isoString, string paramName = "isoString")
    {
        ArgumentException.ThrowIfNullOrEmpty(isoString, paramName);
    }

    /// <summary>
    /// Validates that the provided birth date is valid for age calculation.
    /// </summary>
    /// <param name="birthDate">The birth date to validate.</param>
    /// <param name="paramName">Name of the parameter for exception messages.</param>
    /// <exception cref="ArgumentException">Thrown if the birth date is in the future or invalid.</exception>
    public static void ValidateBirthDate(DateTime birthDate, string paramName = "birthDate")
    {
        if (birthDate > DateTime.Today)
        {
            throw new ArgumentException("Birth date cannot be in the future.", paramName);
        }

        if (birthDate == DateTime.MinValue)
        {
            throw new ArgumentException("Birth date cannot be DateTime.MinValue.", paramName);
        }
    }

    /// <summary>
    /// Validates that both dates are valid for comparison operations.
    /// </summary>
    /// <param name="date1">The first date to validate.</param>
    /// <param name="date2">The second date to validate.</param>
    /// <param name="paramName1">Name of the first parameter for exception messages.</param>
    /// <param name="paramName2">Name of the second parameter for exception messages.</param>
    /// <exception cref="ArgumentException">Thrown if either date is invalid.</exception>
    public static void ValidateDatesForComparison(DateTime date1, DateTime date2,
        string paramName1 = "date1", string paramName2 = "date2")
    {
        ValidateDateTimeIsValid(date1, paramName1);
        ValidateDateTimeIsValid(date2, paramName2);
    }

    /// <summary>
    /// Validates that the provided TimeSpan is not negative.
    /// </summary>
    /// <param name="duration">The TimeSpan to validate.</param>
    /// <param name="paramName">Name of the parameter for exception messages.</param>
    /// <exception cref="ArgumentException">Thrown if the duration is negative.</exception>
    public static void ValidateDurationNonNegative(TimeSpan duration, string paramName = "duration")
    {
        if (duration < TimeSpan.Zero)
        {
            throw new ArgumentException("Duration cannot be negative.", paramName);
        }
    }

    /// <summary>
    /// Validates that the provided year is a reasonable value.
    /// </summary>
    /// <param name="year">The year to validate.</param>
    /// <param name="paramName">Name of the parameter for exception messages.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the year is out of valid range.</exception>
    public static void ValidateYear(int year, string paramName = "year")
    {
        const int MinYear = 1;
        const int MaxYear = 9999;

        if (year < MinYear || year > MaxYear)
        {
            throw new ArgumentOutOfRangeException(paramName,
                $"Year must be between {MinYear} and {MaxYear}.");
        }
    }

    /// <summary>
    /// Validates that the provided timestamp is reasonable (not negative and not too large).
    /// </summary>
    /// <param name="timestampMs">The timestamp in milliseconds to validate.</param>
    /// <param name="paramName">Name of the parameter for exception messages.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the timestamp is invalid.</exception>
    public static void ValidateTimestampMs(long timestampMs, string paramName = "timestampMs")
    {
        const long MinReasonableTimestamp = 0; // Unix epoch
        const long MaxReasonableTimestamp = 325036799999999; // Year 2999

        if (timestampMs < MinReasonableTimestamp || timestampMs > MaxReasonableTimestamp)
        {
            throw new ArgumentOutOfRangeException(paramName,
                $"Timestamp must be between {MinReasonableTimestamp} and {MaxReasonableTimestamp} milliseconds.");
        }
    }

    /// <summary>
    /// Determines whether all DateTimeUtility operations are valid.
    /// </summary>
    /// <returns>true if all validation methods work correctly; otherwise, false.</returns>
    public static bool IsValid()
    {
        try
        {
            // Test basic validation methods
            ValidateDateTime(DateTime.UtcNow);
            ValidateDateTimeNotMinValue(DateTime.UtcNow);
            ValidateDateTimeNotMaxValue(DateTime.UtcNow);
            ValidateDateTimeIsValid(DateTime.UtcNow);
            ValidateIso8601String(DateTime.UtcNow.ToString("o"));
            ValidateBirthDate(DateTime.UtcNow.AddYears(-30));
            ValidateDatesForComparison(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);
            ValidateDurationNonNegative(TimeSpan.FromHours(1));
            ValidateYear(2024);
            ValidateTimestampMs(DateTimeUtility.GetCurrentTimestampMs());

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Ensures that all DateTimeUtility operations are valid.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if validation methods themselves are not valid.</exception>
    public static void EnsureValid()
    {
        if (!IsValid())
        {
            throw new InvalidOperationException(
                "DateTimeUtilityValidation methods are not functioning correctly.");
        }
    }
}