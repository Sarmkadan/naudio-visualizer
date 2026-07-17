using System;
using System.Collections.Generic;
using NAudioVisualizer.Domain.Models;

namespace NAudioVisualizer.Themes;

/// <summary>
/// Provides extension methods for <see cref="ColorScheme"/> to enable fluent queries and formatting.
/// </summary>
public static class ColorSchemeExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="ColorScheme"/> is the predefined dark scheme.
    /// </summary>
    /// <param name="scheme">The color scheme to evaluate.</param>
    /// <returns><c>true</c> if <paramref name="scheme"/> is <see cref="ColorScheme.Dark"/>; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="scheme"/> is <c>null</c>.</exception>
    public static bool IsDark(this ColorScheme scheme)
    {
        ArgumentNullException.ThrowIfNull(scheme);
        return ReferenceEquals(scheme, ColorScheme.Dark);
    }

    /// <summary>
    /// Determines whether the specified <see cref="ColorScheme"/> is one of the predefined schemes.
    /// </summary>
    /// <param name="scheme">The color scheme to evaluate.</param>
    /// <returns><c>true</c> if the scheme is predefined; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="scheme"/> is <c>null</c>.</exception>
    public static bool IsPredefined(this ColorScheme scheme)
    {
        ArgumentNullException.ThrowIfNull(scheme);
        return scheme switch
        {
            { } s when ReferenceEquals(s, ColorScheme.Dark) => true,
            { } s when ReferenceEquals(s, ColorScheme.Light) => true,
            { } s when ReferenceEquals(s, ColorScheme.Neon) => true,
            { } s when ReferenceEquals(s, ColorScheme.Grayscale) => true,
            _ => false
        };
    }

    /// <summary>
    /// Returns a human‑readable description of the color scheme, combining its name and the associated theme.
    /// </summary>
    /// <param name="scheme">The color scheme to describe.</param>
    /// <returns>A string in the format <c>"{Name} ({Theme})"</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="scheme"/> is <c>null</c>.</exception>
    public static string ToDisplayString(this ColorScheme scheme)
    {
        ArgumentNullException.ThrowIfNull(scheme);
        return $"{scheme.Name} ({scheme.Theme})";
    }

    /// <summary>
    /// Retrieves a read‑only list containing all predefined color schemes.
    /// </summary>
    /// <param name="_">An unused instance of <see cref="ColorScheme"/>; the method is provided as an extension for convenience.</param>
    /// <returns>An <see cref="IReadOnlyList{T}"/> of the predefined schemes.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="_"/> is <c>null</c>.</exception>
    public static IReadOnlyList<ColorScheme> GetPredefinedSchemes(this ColorScheme _)
    {
        ArgumentNullException.ThrowIfNull(_);
        return Array.AsReadOnly(new[] { ColorScheme.Dark, ColorScheme.Light, ColorScheme.Neon, ColorScheme.Grayscale });
    }
}
