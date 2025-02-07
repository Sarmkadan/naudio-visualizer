#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Describes a single color stop in a gradient, combining a normalized position
/// in the [0, 1] range with an ARGB color value.
/// </summary>
public sealed class GradientStop
{
    /// <summary>
    /// Normalized position within the gradient, in the range [0, 1].
    /// 0 is the start of the gradient; 1 is the end.
    /// </summary>
    public float Position { get; }

    /// <summary>
    /// Color at this stop in ARGB (0xAARRGGBB) format.
    /// </summary>
    public uint Color { get; }

    /// <param name="position">Normalized position in [0, 1].</param>
    /// <param name="color">ARGB color value (0xAARRGGBB).</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="position"/> is outside [0, 1].
    /// </exception>
    public GradientStop(float position, uint color)
    {
        if (position < 0f || position > 1f)
            throw new ArgumentOutOfRangeException(nameof(position), "Position must be in the [0, 1] range.");

        Position = position;
        Color = color;
    }
}

/// <summary>
/// Defines the color scheme used to render waveform and spectrogram views.
/// Supply a two-stop <see cref="WaveformGradient"/> and a multi-stop
/// <see cref="SpectrogramPalette"/> to fully customize the visualizer appearance.
/// </summary>
/// <example>
/// Create a custom blue-to-white waveform theme:
/// <code>
/// var theme = new VisualizerTheme(
///     name: "Blue Ocean",
///     waveformGradient: new[]
///     {
///         new GradientStop(0f, 0xFF003366),
///         new GradientStop(1f, 0xFFCCEEFF)
///     },
///     spectrogramPalette: VisualizerTheme.Presets.Classic.SpectrogramPalette);
/// </code>
/// </example>
public sealed class VisualizerTheme
{
    /// <summary>
    /// Human-readable name for the theme (e.g. "Classic", "Accessible").
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Background fill color in ARGB (0xAARRGGBB) format.
    /// </summary>
    public uint BackgroundColor { get; }

    /// <summary>
    /// Two or more gradient stops that define the waveform line color from
    /// the bottom (position 0) to the top (position 1) of the waveform view.
    /// At minimum a start and end stop must be provided.
    /// </summary>
    public IReadOnlyList<GradientStop> WaveformGradient { get; }

    /// <summary>
    /// Ordered list of gradient stops that map normalized spectrogram intensity
    /// values to colors. Position 0 corresponds to silence (minimum dB) and
    /// position 1 corresponds to the loudest signal (maximum dB).
    /// </summary>
    public IReadOnlyList<GradientStop> SpectrogramPalette { get; }

    /// <param name="name">Human-readable theme name.</param>
    /// <param name="backgroundColor">Background color in ARGB format.</param>
    /// <param name="waveformGradient">
    /// At least two gradient stops for waveform coloring.
    /// </param>
    /// <param name="spectrogramPalette">
    /// At least two gradient stops for spectrogram intensity mapping.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="name"/>, <paramref name="waveformGradient"/>,
    /// or <paramref name="spectrogramPalette"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when fewer than two stops are supplied for either gradient.
    /// </exception>
    public VisualizerTheme(
        string name,
        uint backgroundColor,
        IReadOnlyList<GradientStop> waveformGradient,
        IReadOnlyList<GradientStop> spectrogramPalette)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        if (waveformGradient is null || waveformGradient.Count < 2)
            throw new ArgumentException("Waveform gradient requires at least two stops.", nameof(waveformGradient));

        if (spectrogramPalette is null || spectrogramPalette.Count < 2)
            throw new ArgumentException("Spectrogram palette requires at least two stops.", nameof(spectrogramPalette));

        Name = name;
        BackgroundColor = backgroundColor;
        WaveformGradient = waveformGradient;
        SpectrogramPalette = spectrogramPalette;
    }

    /// <summary>
    /// Built-in theme presets.
    /// </summary>
    public static class Presets
    {
        /// <summary>
        /// Classic green-on-black waveform with a heat-map spectrogram.
        /// Matches the default hardcoded appearance from earlier releases.
        /// </summary>
        public static VisualizerTheme Classic { get; } = new(
            name: "Classic",
            backgroundColor: 0xFF000000,
            waveformGradient: new[]
            {
                new GradientStop(0f, 0xFF003300),
                new GradientStop(1f, 0xFF00FF00)
            },
            spectrogramPalette: new[]
            {
                new GradientStop(0.00f, 0xFF000000),
                new GradientStop(0.25f, 0xFF0000FF),
                new GradientStop(0.50f, 0xFFFF0000),
                new GradientStop(0.75f, 0xFFFFFF00),
                new GradientStop(1.00f, 0xFFFFFFFF)
            });

        /// <summary>
        /// High-contrast blue-and-yellow palette optimised for broadcast and
        /// accessibility contexts where standard red/green combinations may be
        /// difficult to distinguish.
        /// </summary>
        public static VisualizerTheme Accessible { get; } = new(
            name: "Accessible",
            backgroundColor: 0xFF0A0A2E,
            waveformGradient: new[]
            {
                new GradientStop(0f, 0xFF003E7A),
                new GradientStop(1f, 0xFFFFD700)
            },
            spectrogramPalette: new[]
            {
                new GradientStop(0.00f, 0xFF0A0A2E),
                new GradientStop(0.33f, 0xFF0055AA),
                new GradientStop(0.66f, 0xFFFFAA00),
                new GradientStop(1.00f, 0xFFFFFFDD)
            });

        /// <summary>
        /// Greyscale theme suitable for print, dark-room monitoring, and
        /// contexts where color should carry no semantic meaning.
        /// </summary>
        public static VisualizerTheme Monochrome { get; } = new(
            name: "Monochrome",
            backgroundColor: 0xFF000000,
            waveformGradient: new[]
            {
                new GradientStop(0f, 0xFF333333),
                new GradientStop(1f, 0xFFFFFFFF)
            },
            spectrogramPalette: new[]
            {
                new GradientStop(0.00f, 0xFF000000),
                new GradientStop(0.50f, 0xFF808080),
                new GradientStop(1.00f, 0xFFFFFFFF)
            });
    }
}
