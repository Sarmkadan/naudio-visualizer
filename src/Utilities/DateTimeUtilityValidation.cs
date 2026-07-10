#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Globalization;

namespace NAudioVisualizer.Utilities;

/// <summary>
/// Provides validation helpers for DateTimeUtility operations.
/// Validates input parameters and ensures data integrity before processing.
/// </summary>
public static class DateTimeUtilityValidation
{
    /// <summary>
    /// Validates DateTimeUtility operations and returns a list of human-readable problems.
    /// </summary>
    /// <returns>A read-only list of validation problems (empty if valid).</returns>
    public static IReadOnlyList<string> Validate()
    {
        var problems = new List<string>();

        // Validate timestamp-related operations
        try
        {
            // Test GetCurrentTimestampMs doesn't throw
            _ = DateTimeUtility.GetCurrentTimestampMs();
        }
        catch (Exception ex)
        {
            problems.Add($"GetCurrentTimestampMs() failed: {ex.Message}");
        }

        try
        {
            // Test ToTimestampMs with valid date
            var validDate = DateTime.UtcNow;
            _ = DateTimeUtility.ToTimestampMs(validDate);
        }
        catch (Exception ex)
        {
            problems.Add($"ToTimestampMs() failed with valid input: {ex.Message}");
        }

        try
        {
            // Test FromTimestampMs with reasonable value
            var reasonableTimestamp = DateTimeUtility.GetCurrentTimestampMs();
            _ = DateTimeUtility.FromTimestampMs(reasonableTimestamp);
        }
        catch (Exception ex)
        {
            problems.Add($"FromTimestampMs() failed with reasonable timestamp: {ex.Message}");
        }

        // Validate ISO 8601 operations
        try
        {
            var testDate = DateTime.UtcNow;
            var isoString = DateTimeUtility.ToIso8601(testDate);
            _ = DateTimeUtility.FromIso8601(isoString);
        }
        catch (Exception ex)
        {
            problems.Add($"ISO 8601 conversion failed: {ex.Message}");
        }

        // Validate Duration formatting
        try
        {
            var validDuration = TimeSpan.FromHours(2.5);
            _ = DateTimeUtility.FormatDuration(validDuration);
        }
        catch (Exception ex)
        {
            problems.Add($"FormatDuration() failed: {ex.Message}");
        }

        // Validate DaysBetween with valid dates
        try
        {
            var date1 = DateTime.UtcNow.AddDays(-5);
            var date2 = DateTime.UtcNow;
            _ = DateTimeUtility.DaysBetween(date1, date2);
        }
        catch (Exception ex)
        {
            problems.Add($"DaysBetween() failed: {ex.Message}");
        }

        // Validate date comparison methods
        try
        {
            var pastDate = DateTime.UtcNow.AddDays(-1);
            _ = DateTimeUtility.IsInPast(pastDate);
            _ = DateTimeUtility.IsInFuture(pastDate);

            var futureDate = DateTime.UtcNow.AddDays(1);
            _ = DateTimeUtility.IsInFuture(futureDate);
            _ = DateTimeUtility.IsInPast(futureDate);

            var today = DateTime.Today;
            _ = DateTimeUtility.IsToday(today);
        }
        catch (Exception ex)
        {
            problems.Add($"Date comparison methods failed: {ex.Message}");
        }

        // Validate GetRelativeTime
        try
        {
            var past = DateTime.UtcNow.AddHours(-2);
            _ = DateTimeUtility.GetRelativeTime(past);

            var future = DateTime.UtcNow.AddHours(2);
            _ = DateTimeUtility.GetRelativeTime(future);
        }
        catch (Exception ex)
        {
            problems.Add($"GetRelativeTime() failed: {ex.Message}");
        }

        // Validate date boundary methods
        try
        {
            var testDate = DateTime.UtcNow;
            _ = DateTimeUtility.GetStartOfDay(testDate);
            _ = DateTimeUtility.GetEndOfDay(testDate);
            _ = DateTimeUtility.GetStartOfWeek(testDate);
            _ = DateTimeUtility.GetStartOfMonth(testDate);
            _ = DateTimeUtility.GetEndOfMonth(testDate);
        }
        catch (Exception ex)
        {
            problems.Add($"Date boundary methods failed: {ex.Message}");
        }

        // Validate CalculateAge
        try
        {
            var birthDate = DateTime.UtcNow.AddYears(-30);
            _ = DateTimeUtility.CalculateAge(birthDate);
        }
        catch (Exception ex)
        {
            problems.Add($"CalculateAge() failed: {ex.Message}");
        }

        // Validate GetDayName
        try
        {
            var testDate = DateTime.UtcNow;
            _ = DateTimeUtility.GetDayName(testDate);
        }
        catch (Exception ex)
        {
            problems.Add($"GetDayName() failed: {ex.Message}");
        }

        // Validate IsLeapYear
        try
        {
            _ = DateTimeUtility.IsLeapYear(2024); // Leap year
            _ = DateTimeUtility.IsLeapYear(2023); // Not leap year
        }
        catch (Exception ex)
        {
            problems.Add($"IsLeapYear() failed: {ex.Message}");
        }

        return problems.AsReadOnly();
    }

    /// <summary>
    /// Determines whether DateTimeUtility operations are valid.
    /// </summary>
    /// <returns>true if DateTimeUtility operations are valid; otherwise, false.</returns>
    public static bool IsValid()
    {
        return Validate().Count == 0;
    }

    /// <summary>
    /// Ensures that DateTimeUtility operations are valid.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if DateTimeUtility operations are not valid, containing the validation problems.</exception>
    public static void EnsureValid()
    {
        var problems = Validate();

        if (problems.Count > 0)
        {
            throw new ArgumentException(
                $"DateTimeUtility operations are not valid. Problems: {string.Join(", ", problems)}");
        }
    }
}