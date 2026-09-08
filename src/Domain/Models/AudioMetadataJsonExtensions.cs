using System;
using System.Text.Json;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides JSON serialization extensions for <see cref="AudioMetadata"/>.
/// </summary>
public static class AudioMetadataJsonExtensions
{
    /// <summary>
    /// Serializes the specified audio metadata to a JSON string using camel-case property names.
    /// </summary>
    /// <param name="metadata">The audio metadata to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation.</param>
    /// <returns>A JSON representation of the audio metadata.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="metadata"/> is <c>null</c>.</exception>
    public static string ToJson(this AudioMetadata metadata, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        return JsonSerializer.Serialize(metadata, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = indented
        });
    }
}
