#nullable enable

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.Services;

/// <summary>
/// Provides validation helpers for <see cref="SpectrogramAnalyzer"/> instances.
/// Validates public members and ensures they are in valid states.
/// </summary>
public static class SpectrogramAnalyzerValidation
{
    /// <summary>
    /// Validates the state of a <see cref="SpectrogramAnalyzer"/> instance.
    /// </summary>
    /// <param name="value">The analyzer instance to validate.</param>
    /// <returns>
    /// An empty list if the analyzer is valid; otherwise, a list of human-readable
    /// problem descriptions.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    public static IReadOnlyList<string> Validate(this SpectrogramAnalyzer? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = new List<string>();

        // Validate internal state via public API
        try
        {
            int bufferFrameCount = value.GetBufferFrameCount();
            if (bufferFrameCount < 0)
            {
                problems.Add("Buffer frame count is negative");
            }
        }
        catch (Exception ex)
        {
            problems.Add($"GetBufferFrameCount() threw exception: {ex.Message}");
        }

        return problems.AsReadOnly();
    }

    /// <summary>
    /// Determines whether the specified <see cref="SpectrogramAnalyzer"/> instance is valid.
    /// </summary>
    /// <param name="value">The analyzer instance to check.</param>
    /// <returns>
    /// <see langword="true"/> if the analyzer is valid; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsValid(this SpectrogramAnalyzer? value) => value?.Validate().Count == 0;

    /// <summary>
    /// Ensures that the specified <see cref="SpectrogramAnalyzer"/> instance is valid.
    /// </summary>
    /// <param name="value">The analyzer instance to validate.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the analyzer instance contains validation problems.
    /// The exception message lists all detected problems.
    /// </exception>
    public static void EnsureValid(this SpectrogramAnalyzer? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = value.Validate();

        if (problems.Count > 0)
        {
            throw new ArgumentException(
                $"SpectrogramAnalyzer validation failed:{Environment.NewLine}- {string.Join($"{Environment.NewLine}- ", problems)}",
                nameof(value)
            );
        }
    }
}