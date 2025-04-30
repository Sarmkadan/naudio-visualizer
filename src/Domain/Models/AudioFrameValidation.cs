#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =====================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides validation helpers for AudioFrame instances.
/// </summary>
public static class AudioFrameValidation
{
    /// <summary>
    /// Validates an AudioFrame and returns a list of human-readable problems.
    /// </summary>
    /// <param name="value">The AudioFrame to validate.</param>
    /// <returns>List of validation problems; empty if valid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<string> Validate(this AudioFrame value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = new List<string>();

        // Validate Id
        if (value.Id == Guid.Empty)
        {
            problems.Add("AudioFrame.Id must not be empty (Guid.Empty).");
        }

        // Validate Samples
        if (value.Samples == null)
        {
            problems.Add("AudioFrame.Samples cannot be null.");
        }
        else if (value.Samples.Length == 0)
        {
            problems.Add("AudioFrame.Samples cannot be empty.");
        }
        else if (value.Samples.Any(float.IsNaN))
        {
            problems.Add("AudioFrame.Samples contains NaN values.");
        }
        else if (value.Samples.Any(s => float.IsInfinity(s)))
        {
            problems.Add("AudioFrame.Samples contains infinite values.");
        }

        // Validate ChannelCount
        if (value.ChannelCount <= 0)
        {
            problems.Add("AudioFrame.ChannelCount must be greater than 0.");
        }
        else if (value.Samples != null && value.Samples.Length % value.ChannelCount != 0)
        {
            problems.Add("AudioFrame.Samples length must be divisible by ChannelCount.");
        }

        // Validate SampleRate
        if (value.SampleRate <= 0)
        {
            problems.Add("AudioFrame.SampleRate must be greater than 0.");
        }

        // Validate Timestamp
        if (value.Timestamp == default)
        {
            problems.Add("AudioFrame.Timestamp must not be default (DateTime.MinValue).");
        }
        else if (value.Timestamp.Kind != DateTimeKind.Utc)
        {
            problems.Add("AudioFrame.Timestamp must be in UTC format.");
        }

        // Validate FrameIndex
        if (value.FrameIndex < 0)
        {
            problems.Add("AudioFrame.FrameIndex must not be negative.");
        }

        // Validate DurationSeconds
        if (value.DurationSeconds <= 0)
        {
            problems.Add("AudioFrame.DurationSeconds must be greater than 0.");
        }

        // Validate PeakAmplitude
        if (float.IsNaN(value.PeakAmplitude))
        {
            problems.Add("AudioFrame.PeakAmplitude cannot be NaN.");
        }
        else if (float.IsInfinity(value.PeakAmplitude))
        {
            problems.Add("AudioFrame.PeakAmplitude cannot be infinite.");
        }
        else if (Math.Abs(value.PeakAmplitude) > 1.0f)
        {
            problems.Add("AudioFrame.PeakAmplitude must be between -1.0 and 1.0.");
        }

        // Validate RmsEnergy
        if (float.IsNaN(value.RmsEnergy))
        {
            problems.Add("AudioFrame.RmsEnergy cannot be NaN.");
        }
        else if (float.IsInfinity(value.RmsEnergy))
        {
            problems.Add("AudioFrame.RmsEnergy cannot be infinite.");
        }
        else if (value.RmsEnergy < 0)
        {
            problems.Add("AudioFrame.RmsEnergy cannot be negative.");
        }

        return problems.AsReadOnly();
    }

    /// <summary>
    /// Determines whether an AudioFrame is valid.
    /// </summary>
    /// <param name="value">The AudioFrame to check.</param>
    /// <returns>True if valid; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static bool IsValid(this AudioFrame value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Validate(value).Count == 0;
    }

    /// <summary>
    /// Ensures that an AudioFrame is valid, throwing an exception if not.
    /// </summary>
    /// <param name="value">The AudioFrame to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the AudioFrame is invalid.</exception>
    public static void EnsureValid(this AudioFrame value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = Validate(value);

        if (problems.Count > 0)
        {
            throw new ArgumentException(
                $"AudioFrame validation failed:{Environment.NewLine}{string.Join(Environment.NewLine, problems)}");
        }
    }
}