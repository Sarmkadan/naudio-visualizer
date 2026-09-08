#nullable enable

using System;
using System.Linq;
using System.Text.Json;

namespace NAudioVisualizer.Themes;

/// <summary>
/// Provides JSON serialization extensions for <see cref="ColorScheme"/>.
/// </summary>
public static class ColorSchemeJsonExtensions
{
    /// <summary>
    /// Serializes the color scheme and its theme colors and gradients to JSON.
    /// </summary>
    /// <param name="scheme">The color scheme to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation.</param>
    /// <returns>A JSON representation of the color scheme.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="scheme"/> is <see langword="null"/>.</exception>
    public static string ToJson(this ColorScheme scheme, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(scheme);

        var value = new
        {
            scheme.Name,
            Theme = new
            {
                scheme.Theme.Name,
                scheme.Theme.BackgroundColor,
                WaveformGradient = scheme.Theme.WaveformGradient.Select(stop => new
                {
                    stop.Position,
                    stop.Color
                }),
                SpectrogramPalette = scheme.Theme.SpectrogramPalette.Select(stop => new
                {
                    stop.Position,
                    stop.Color
                })
            }
        };

        return JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = indented
        });
    }
}
