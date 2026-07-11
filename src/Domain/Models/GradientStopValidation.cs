#nullable enable

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides validation helpers for <see cref="GradientStop"/> instances.
/// </summary>
public static class GradientStopValidation
{
    /// <summary>
    /// Validates a <see cref="GradientStop"/> instance and returns a list of human-readable problems.
    /// </summary>
    /// <param name="value">The gradient stop to validate.</param>
    /// <returns>An empty list if valid; otherwise, a list of validation error messages.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<string> Validate(this GradientStop? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var errors = new List<string>();

        // Validate Position range
        if (float.IsNaN(value.Position) || float.IsInfinity(value.Position))
        {
            errors.Add("Position must be a valid number.");
        }
        else if (value.Position < 0f || value.Position > 1f)
        {
            errors.Add("Position must be in the [0, 1] range.");
        }

        // Validate Color (ARGB format - basic check)
        // Color is a uint representing ARGB (0xAARRGGBB)
        // We can't validate the actual color values without knowing the expected format
        // beyond ensuring it's a valid uint
        if (value.Color == 0u && value.Position != 0f)
        {
            // Only position 0 can reasonably have color 0 (transparent black)
            // This is a heuristic, not a strict rule
            errors.Add("Position 0 should use color 0 (transparent black) for gradient start.");
        }

        return errors.AsReadOnly();
    }

    /// <summary>
    /// Determines whether a <see cref="GradientStop"/> instance is valid.
    /// </summary>
    /// <param name="value">The gradient stop to check.</param>
    /// <returns>True if valid; otherwise, false.</returns>
    public static bool IsValid(this GradientStop? value) => value is not null && Validate(value).Count == 0;

    /// <summary>
    /// Ensures that a <see cref="GradientStop"/> instance is valid, throwing an exception if it is not.
    /// </summary>
    /// <param name="value">The gradient stop to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the gradient stop is invalid.</exception>
    public static void EnsureValid(this GradientStop? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var errors = Validate(value);
        if (errors.Count > 0)
        {
            throw new ArgumentException(
                $"GradientStop is invalid. {string.Join(" ", errors)}",
                nameof(value));
        }
    }
}
