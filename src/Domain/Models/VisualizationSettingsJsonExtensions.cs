#nullable enable

using System.Text.Json;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// JSON serialization extensions for <see cref="VisualizationSettings"/>.
/// </summary>
public static class VisualizationSettingsJsonExtensions
{
    /// <summary>
    /// Serializes the visualization settings to JSON using camel-case property names.
    /// </summary>
    /// <param name="settings">The settings to serialize.</param>
    /// <param name="indented">Whether the JSON should be indented.</param>
    /// <returns>The serialized JSON.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="settings"/> is null.</exception>
    public static string ToJson(this VisualizationSettings settings, bool indented = true)
    {
        ArgumentNullException.ThrowIfNull(settings);
        return JsonSerializer.Serialize(settings, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = indented
        });
    }
}
