#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides extension methods for sampling colors from a <see cref="VisualizerTheme"/>.
/// </summary>
public static class VisualizerThemeExtensions
{
    private const int BrightnessMidpoint = 128;
    private const double RedLumaCoefficient = 0.299;
    private const double GreenLumaCoefficient = 0.587;
    private const double BlueLumaCoefficient = 0.114;

    /// <summary>
    /// Samples the waveform gradient at a normalized position, linearly
    /// interpolating the ARGB color between the two neighbouring gradient stops.
    /// </summary>
    /// <param name="theme">The theme whose waveform gradient is sampled.</param>
    /// <param name="position">Normalized position in [0, 1]. 0 is the bottom of the waveform view; 1 is the top.</param>
    /// <returns>The interpolated ARGB color (0xAARRGGBB).</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="theme"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="position"/> is outside [0, 1].</exception>
    public static uint SampleWaveformColor(this VisualizerTheme theme, float position)
    {
        ArgumentNullException.ThrowIfNull(theme);
        return SampleColor(theme.WaveformGradient, position);
    }

    /// <summary>
    /// Samples the spectrogram palette at a normalized intensity, linearly
    /// interpolating the ARGB color between the two neighbouring gradient stops.
    /// </summary>
    /// <param name="theme">The theme whose spectrogram palette is sampled.</param>
    /// <param name="position">Normalized intensity in [0, 1]. 0 is silence; 1 is the loudest signal.</param>
    /// <returns>The interpolated ARGB color (0xAARRGGBB).</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="theme"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="position"/> is outside [0, 1].</exception>
    public static uint SampleSpectrogramColor(this VisualizerTheme theme, float position)
    {
        ArgumentNullException.ThrowIfNull(theme);
        return SampleColor(theme.SpectrogramPalette, position);
    }

    /// <summary>
    /// Determines whether the theme uses a dark background, based on the
    /// perceived luminance of <see cref="VisualizerTheme.BackgroundColor"/>.
    /// </summary>
    /// <param name="theme">The theme to inspect.</param>
    /// <returns><see langword="true"/> if the background is dark; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="theme"/> is null.</exception>
    public static bool IsDarkTheme(this VisualizerTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        var red = (byte)((theme.BackgroundColor >> 16) & 0xFF);
        var green = (byte)((theme.BackgroundColor >> 8) & 0xFF);
        var blue = (byte)(theme.BackgroundColor & 0xFF);

        var luminance = RedLumaCoefficient * red + GreenLumaCoefficient * green + BlueLumaCoefficient * blue;
        return luminance < BrightnessMidpoint;
    }

    private static uint SampleColor(IReadOnlyList<GradientStop> stops, float position)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(position, 0.0f);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(position, 1.0f);

        if (position <= stops[0].Position)
            return stops[0].Color;

        if (position >= stops[^1].Position)
            return stops[^1].Color;

        for (int i = 0; i < stops.Count - 1; i++)
        {
            var lower = stops[i];
            var upper = stops[i + 1];

            if (position < upper.Position)
            {
                var span = upper.Position - lower.Position;
                var t = span <= 0f ? 0f : (position - lower.Position) / span;
                return lower.Interpolate(upper, t).Color;
            }
        }

        return stops[^1].Color;
    }
}