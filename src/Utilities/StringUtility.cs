#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Text;

namespace NAudioVisualizer.Utilities;

/// <summary>
/// Provides utility methods for string manipulation and formatting.
/// Includes truncation, padding, and case conversion utilities.
/// </summary>
public static class StringUtility
{
    /// <summary>
    /// Truncates a string to a maximum length with optional ellipsis.
    /// </summary>
    public static string Truncate(string? text, int maxLength, bool addEllipsis = true)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        if (text.Length <= maxLength)
            return text;

        int length = addEllipsis ? maxLength - 3 : maxLength;
        string truncated = text.Substring(0, Math.Max(0, length));

        return addEllipsis ? truncated + "..." : truncated;
    }

    /// <summary>
    /// Repeats a string N times.
    /// </summary>
    public static string Repeat(string text, int count)
    {
        if (string.IsNullOrEmpty(text) || count <= 0)
            return string.Empty;

        var sb = new StringBuilder();
        for (int i = 0; i < count; i++)
            sb.Append(text);

        return sb.ToString();
    }

    /// <summary>
    /// Pads a string to a specific width with a character.
    /// </summary>
    public static string PadCenter(string text, int totalWidth, char paddingChar = ' ')
    {
        if (text.Length >= totalWidth)
            return text;

        int totalPadding = totalWidth - text.Length;
        int leftPadding = totalPadding / 2;
        int rightPadding = totalPadding - leftPadding;

        return new string(paddingChar, leftPadding) + text + new string(paddingChar, rightPadding);
    }

    /// <summary>
    /// Converts a number of bytes to human-readable format.
    /// </summary>
    public static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }

    /// <summary>
    /// Converts milliseconds to a formatted time string.
    /// </summary>
    public static string FormatMilliseconds(long milliseconds)
    {
        if (milliseconds < 0)
            return "Invalid";

        if (milliseconds < 1000)
            return $"{milliseconds}ms";

        var timespan = TimeSpan.FromMilliseconds(milliseconds);

        if (timespan.TotalHours > 1)
            return $"{timespan.Hours}h {timespan.Minutes}m {timespan.Seconds}s";

        if (timespan.TotalMinutes > 1)
            return $"{timespan.Minutes}m {timespan.Seconds}s";

        return $"{timespan.Seconds}s {timespan.Milliseconds}ms";
    }

    /// <summary>
    /// Converts a number to a human-readable format with suffix (K, M, B, etc).
    /// </summary>
    public static string FormatLargeNumber(long number)
    {
        if (number < 1000)
            return number.ToString();

        string[] suffixes = { "", "K", "M", "B", "T" };
        double value = number;
        int suffixIndex = 0;

        while (value >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            value /= 1000;
            suffixIndex++;
        }

        return $"{value:0.##}{suffixes[suffixIndex]}";
    }

    /// <summary>
    /// Capitalizes the first letter of each word.
    /// </summary>
    public static string ToTitleCase(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(text.ToLower());
    }

    /// <summary>
    /// Converts a string to snake_case format.
    /// </summary>
    public static string ToSnakeCase(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var sb = new StringBuilder();
        bool previousWasUpper = false;

        foreach (char c in text)
        {
            if (char.IsUpper(c))
            {
                if (sb.Length > 0 && !previousWasUpper)
                    sb.Append('_');
                sb.Append(char.ToLower(c));
                previousWasUpper = true;
            }
            else
            {
                sb.Append(c);
                previousWasUpper = false;
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts a string to camelCase format.
    /// </summary>
    public static string ToCamelCase(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var parts = text.Split('_', ' ', '-');
        if (parts.Length == 0)
            return text;

        var sb = new StringBuilder();
        sb.Append(char.ToLower(parts[0][0]));
        sb.Append(parts[0].Substring(1));

        for (int i = 1; i < parts.Length; i++)
        {
            if (parts[i].Length > 0)
            {
                sb.Append(char.ToUpper(parts[i][0]));
                sb.Append(parts[i].Substring(1));
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Removes all whitespace from a string.
    /// </summary>
    public static string RemoveWhitespace(string text)
    {
        return string.IsNullOrEmpty(text)
            ? text
            : System.Text.RegularExpressions.Regex.Replace(text, @"\s+", "");
    }

    /// <summary>
    /// Checks if a string contains only alphanumeric characters.
    /// </summary>
    public static bool IsAlphanumeric(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        foreach (char c in text)
        {
            if (!char.IsLetterOrDigit(c))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Counts the occurrences of a substring in a string.
    /// </summary>
    public static int CountOccurrences(string text, string substring)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(substring))
            return 0;

        int count = 0;
        int index = 0;

        while ((index = text.IndexOf(substring, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += substring.Length;
        }

        return count;
    }
}
