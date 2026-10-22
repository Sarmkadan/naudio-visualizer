#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Validation rules for <see cref="WaveformRenderingSettings"/>.
/// </summary>
public static class WaveformRenderingSettingsValidation
{
    /// <summary>
    /// Validates all rules and collects every violation, not just the first one.
    /// </summary>
    /// <param name="value">The settings to validate.</param>
    /// <returns>
    /// An empty list if the settings are valid; otherwise, one human-readable message per violated rule.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<string> Validate(this WaveformRenderingSettings? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = new List<string>();

        // Alpha 0 means the line is fully transparent, so nothing would be drawn.
        if ((value.LineColor >> 24) == 0)
        {
            problems.Add($"LineColor must not be fully transparent (alpha 0) (got 0x{value.LineColor:X8}).");
        }

        // Written as a negated range check so NaN and infinity are rejected as well.
        if (!(value.LineThickness > 0f && float.IsFinite(value.LineThickness)))
        {
            problems.Add($"LineThickness must be greater than 0 (got {value.LineThickness}).");
        }

        if (!(value.AmplitudeZoom >= 0.1f && value.AmplitudeZoom <= 10f))
        {
            problems.Add($"AmplitudeZoom must be within [0.1, 10] (got {value.AmplitudeZoom}).");
        }

        if (value.DownsamplingFactor < 1)
        {
            problems.Add($"DownsamplingFactor must be at least 1 (got {value.DownsamplingFactor}).");
        }

        return problems.AsReadOnly();
    }

    /// <summary>
    /// Throws if any rule is violated. The exception message lists every violation.
    /// </summary>
    /// <param name="value">The settings to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when one or more rules are violated.</exception>
    public static void EnsureValid(this WaveformRenderingSettings? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = value.Validate();

        if (problems.Count > 0)
        {
            throw new ArgumentException(
                $"WaveformRenderingSettings validation failed:{Environment.NewLine}- {string.Join($"{Environment.NewLine}- ", problems)}",
                "settings"
            );
        }
    }
}
