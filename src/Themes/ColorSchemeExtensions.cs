using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Utilities;

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
    /// Returns a new color scheme with gradient stops and positions reversed.
    /// </summary>
    public static ColorScheme Reversed(this ColorScheme scheme)
    {
        ArgumentNullException.ThrowIfNull(scheme);
        var theme = scheme.Theme;

        var newWaveformGradient = theme.WaveformGradient
            .Select(s => new GradientStop(1.0f - s.Position, s.Color))
            .OrderBy(s => s.Position)
            .ToList();

        var newSpectrogramPalette = theme.SpectrogramPalette
            .Select(s => new GradientStop(1.0f - s.Position, s.Color))
            .OrderBy(s => s.Position)
            .ToList();

        var newTheme = new VisualizerTheme(theme.Name + " Reversed", theme.BackgroundColor, newWaveformGradient, newSpectrogramPalette);
        return new ColorScheme(scheme.Name + " Reversed", newTheme);
    }

    /// <summary>
    /// Returns a new color scheme with adjusted brightness.
    /// </summary>
    public static ColorScheme WithBrightness(this ColorScheme scheme, float factor)
    {
        ArgumentNullException.ThrowIfNull(scheme);
        var theme = scheme.Theme;

        uint AdjustColor(uint color) => UintFromColor(ColorUtility.AdjustBrightness(ColorFromUint(color), factor));

        var newWaveformGradient = theme.WaveformGradient
            .Select(s => new GradientStop(s.Position, AdjustColor(s.Color)))
            .ToList();

        var newSpectrogramPalette = theme.SpectrogramPalette
            .Select(s => new GradientStop(s.Position, AdjustColor(s.Color)))
            .ToList();

        uint newBackgroundColor = AdjustColor(theme.BackgroundColor);

        var newTheme = new VisualizerTheme(theme.Name + (factor > 1.0f ? " Brighter" : " Darker"), newBackgroundColor, newWaveformGradient, newSpectrogramPalette);
        return new ColorScheme(scheme.Name + (factor > 1.0f ? " Brighter" : " Darker"), newTheme);
    }

    /// <summary>
    /// Returns a new color scheme interpolated between two schemes.
    /// </summary>
    public static ColorScheme Lerp(this ColorScheme scheme, ColorScheme other, float t)
    {
        ArgumentNullException.ThrowIfNull(scheme);
        ArgumentNullException.ThrowIfNull(other);
        
        var theme1 = scheme.Theme;
        var theme2 = other.Theme;

        uint LerpColor(uint c1, uint c2) => UintFromColor(ColorUtility.LerpColor(ColorFromUint(c1), ColorFromUint(c2), t));

        var countW = Math.Min(theme1.WaveformGradient.Count, theme2.WaveformGradient.Count);
        var newWaveformGradient = theme1.WaveformGradient.Take(countW)
            .Zip(theme2.WaveformGradient.Take(countW), (s1, s2) => new GradientStop(
                s1.Position + (s2.Position - s1.Position) * t,
                LerpColor(s1.Color, s2.Color)
            ))
            .ToList();

        var countS = Math.Min(theme1.SpectrogramPalette.Count, theme2.SpectrogramPalette.Count);
        var newSpectrogramPalette = theme1.SpectrogramPalette.Take(countS)
            .Zip(theme2.SpectrogramPalette.Take(countS), (s1, s2) => new GradientStop(
                s1.Position + (s2.Position - s1.Position) * t,
                LerpColor(s1.Color, s2.Color)
            ))
            .ToList();

        uint newBackgroundColor = LerpColor(theme1.BackgroundColor, theme2.BackgroundColor);

        var newTheme = new VisualizerTheme(theme1.Name + " Lerp", newBackgroundColor, newWaveformGradient, newSpectrogramPalette);
        return new ColorScheme(theme1.Name + " Lerp", newTheme);
    }

    private static Color ColorFromUint(uint color)
    {
        return Color.FromArgb((int)((color >> 24) & 0xFF), (int)((color >> 16) & 0xFF), (int)((color >> 8) & 0xFF), (int)(color & 0xFF));
    }

    private static uint UintFromColor(Color color)
    {
        return ((uint)color.A << 24) | ((uint)color.R << 16) | ((uint)color.G << 8) | (uint)color.B;
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
