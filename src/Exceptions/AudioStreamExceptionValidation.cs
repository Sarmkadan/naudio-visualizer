#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =====================================================================

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.Exceptions;

/// <summary>
/// Provides validation helpers for <see cref="AudioStreamException"/> instances.
/// </summary>
public static class AudioStreamExceptionValidation
{
    /// <summary>
    /// Validates an <see cref="AudioStreamException"/> instance and returns a list of validation problems.
    /// </summary>
    /// <param name="value">The exception to validate.</param>
    /// <returns>A list of human-readable validation problems; empty if valid.</returns>
    public static IReadOnlyList<string> Validate(this AudioStreamException value)
    {
        if (value is null)
        {
            return new[] { "AudioStreamException cannot be null." };
        }

        var problems = new List<string>();

        // Validate ErrorCode
        if (!Enum.IsDefined(typeof(AudioStreamErrorCode), value.ErrorCode))
        {
            problems.Add("ErrorCode must be a defined AudioStreamErrorCode value.");
        }

        // Validate Message (from base Exception)
        if (string.IsNullOrWhiteSpace(value.Message))
        {
            problems.Add("Message cannot be null or whitespace.");
        }

        // Validate InnerException (from base Exception)
        if (value.InnerException is not null)
        {
            if (string.IsNullOrWhiteSpace(value.InnerException.Message))
            {
                problems.Add("InnerException.Message cannot be null or whitespace.");
            }
        }

        return problems;
    }

    /// <summary>
    /// Determines whether an <see cref="AudioStreamException"/> instance is valid.
    /// </summary>
    /// <param name="value">The exception to check.</param>
    /// <returns>True if the exception is valid; otherwise, false.</returns>
    public static bool IsValid(this AudioStreamException value)
    {
        return value is not null && Validate(value).Count == 0;
    }

    /// <summary>
    /// Ensures that an <see cref="AudioStreamException"/> instance is valid, throwing an <see cref="ArgumentException"/> if not.
    /// </summary>
    /// <param name="value">The exception to validate.</param>
    /// <exception cref="ArgumentException">Thrown when the exception is invalid, containing a list of validation problems.</exception>
    public static void EnsureValid(this AudioStreamException value)
    {
        if (value is null)
        {
            throw new ArgumentException("AudioStreamException cannot be null.", nameof(value));
        }

        var problems = Validate(value);
        if (problems.Count > 0)
        {
            throw new ArgumentException(
                $"AudioStreamException is invalid. Problems:\n{string.Join("\n", problems)}",
                nameof(value));
        }
    }
}
