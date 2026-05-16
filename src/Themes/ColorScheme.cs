#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using NAudioVisualizer.Domain.Models;

namespace NAudioVisualizer.Themes;

/// <summary>
/// Provides predefined color scheme presets for audio visualization.
/// Each preset bundles a <see cref="VisualizerTheme"/> with a descriptive name
/// and a suggested <see cref="BackgroundStyle"/> hint.
/// </summary>
public sealed class ColorScheme
{
    /// <summary>
    /// Human-readable name of the color scheme.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The <see cref="VisualizerTheme"/> that defines the gradient colors.
    /// </summary>
    public VisualizerTheme Theme { get; }

    private ColorScheme(string name, VisualizerTheme theme)
    {
        Name = name;
        Theme = theme;
    }

    /// <summary>
    /// Dark background with a blue-to-white gradient — suited for dark UIs.
    /// </summary>
    public static ColorScheme Dark { get; } = new(
        name: "Dark",
        theme: new VisualizerTheme(
            name: "Dark",
            backgroundColor: 0xFF000000,
            waveformGradient: new[]
            {
                new GradientStop(0.00f, 0xFF00004D),
                new GradientStop(0.50f, 0xFF0080FF),
                new GradientStop(1.00f, 0xFFFFFFFF)
            },
            spectrogramPalette: new[]
            {
                new GradientStop(0.00f, 0xFF000000),
                new GradientStop(0.40f, 0xFF00004D),
                new GradientStop(0.70f, 0xFF0080FF),
                new GradientStop(1.00f, 0xFFFFFFFF)
            }));

    /// <summary>
    /// Light background with a gray-to-blue gradient — suited for light UIs.
    /// </summary>
    public static ColorScheme Light { get; } = new(
        name: "Light",
        theme: new VisualizerTheme(
            name: "Light",
            backgroundColor: 0xFFFFFFFF,
            waveformGradient: new[]
            {
                new GradientStop(0.00f, 0xFF808080),
                new GradientStop(1.00f, 0xFF0055AA)
            },
            spectrogramPalette: new[]
            {
                new GradientStop(0.00f, 0xFFFFFFFF),
                new GradientStop(0.40f, 0xFFCCCCCC),
                new GradientStop(0.70f, 0xFF6699CC),
                new GradientStop(1.00f, 0xFF0033AA)
            }));

    /// <summary>
    /// Black background with a green-yellow-red gradient — classic VU meter style.
    /// </summary>
    public static ColorScheme Neon { get; } = new(
        name: "Neon",
        theme: new VisualizerTheme(
            name: "Neon",
            backgroundColor: 0xFF000000,
            waveformGradient: new[]
            {
                new GradientStop(0.00f, 0xFF00FF00),
                new GradientStop(0.60f, 0xFFFFFF00),
                new GradientStop(1.00f, 0xFFFF0000)
            },
            spectrogramPalette: new[]
            {
                new GradientStop(0.00f, 0xFF000000),
                new GradientStop(0.33f, 0xFF00FF00),
                new GradientStop(0.66f, 0xFFFFFF00),
                new GradientStop(1.00f, 0xFFFF0000)
            }));

    /// <summary>
    /// White background with a black-to-white gradient — optimised for print and screenshots.
    /// </summary>
    public static ColorScheme Grayscale { get; } = new(
        name: "Grayscale",
        theme: new VisualizerTheme(
            name: "Grayscale",
            backgroundColor: 0xFFFFFFFF,
            waveformGradient: new[]
            {
                new GradientStop(0.00f, 0xFF000000),
                new GradientStop(1.00f, 0xFF888888)
            },
            spectrogramPalette: new[]
            {
                new GradientStop(0.00f, 0xFFFFFFFF),
                new GradientStop(0.50f, 0xFF888888),
                new GradientStop(1.00f, 0xFF000000)
            }));
}
