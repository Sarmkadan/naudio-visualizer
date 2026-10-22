#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using NAudioVisualizer.Constants;

namespace NAudioVisualizer.Configuration;

/// <summary>
/// Validation rules for <see cref="AnalyzerCaptureOptions"/>.
/// </summary>
public static class AnalyzerCaptureOptionsValidation
{
    /// <summary>
    /// Validates all rules and collects every violation, not just the first one.
    /// </summary>
    /// <param name="value">The options to validate.</param>
    /// <returns>
    /// An empty list if the options are valid; otherwise, one human-readable message per violated rule.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<string> Validate(this AnalyzerCaptureOptions? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = new List<string>();

        if (!IsPowerOfTwo(value.FftSize))
        {
            problems.Add($"FftSize must be a power of two (got {value.FftSize}).");
        }
        else if (value.FftSize < AudioConstants.FFT_MINIMUM || value.FftSize > AudioConstants.FFT_MAXIMUM)
        {
            problems.Add($"FftSize must be between {AudioConstants.FFT_MINIMUM} and {AudioConstants.FFT_MAXIMUM} (got {value.FftSize}).");
        }

        if (value.SampleRate < AudioConstants.MIN_SAMPLE_RATE || value.SampleRate > AudioConstants.MAX_SAMPLE_RATE)
        {
            problems.Add($"SampleRate must be between {AudioConstants.MIN_SAMPLE_RATE} and {AudioConstants.MAX_SAMPLE_RATE} Hz (got {value.SampleRate}).");
        }

        if (value.Channels != 1 && value.Channels != 2)
        {
            problems.Add($"Channels must be 1 (mono) or 2 (stereo) (got {value.Channels}).");
        }

        if (value.BufferMilliseconds <= 0)
        {
            problems.Add($"BufferMilliseconds must be greater than 0 (got {value.BufferMilliseconds}).");
        }

        // Written as a negated range check so NaN is rejected as well.
        if (!(value.SmoothingFactor >= 0f && value.SmoothingFactor <= 1f))
        {
            problems.Add($"SmoothingFactor must be within [0, 1] (got {value.SmoothingFactor}).");
        }

        return problems.AsReadOnly();
    }

    /// <summary>
    /// Determines whether the options are valid.
    /// </summary>
    /// <param name="value">The options to check.</param>
    /// <returns><see langword="true"/> if all rules pass; otherwise, <see langword="false"/>.</returns>
    public static bool IsValid(this AnalyzerCaptureOptions? value) => value?.Validate().Count == 0;

    /// <summary>
    /// Throws if any rule is violated. The exception message lists every violation.
    /// </summary>
    /// <param name="value">The options to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when one or more rules are violated.</exception>
    public static void EnsureValid(this AnalyzerCaptureOptions? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = value.Validate();

        if (problems.Count > 0)
        {
            throw new ArgumentException(
                $"AnalyzerCaptureOptions validation failed:{Environment.NewLine}- {string.Join($"{Environment.NewLine}- ", problems)}",
                "options"
            );
        }
    }

    private static bool IsPowerOfTwo(int value) => value > 0 && (value & (value - 1)) == 0;
}
