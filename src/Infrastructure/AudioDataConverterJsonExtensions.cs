#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Text.Json;
using NAudioVisualizer.Domain.Models;

namespace NAudioVisualizer.Infrastructure;

/// <summary>
/// Provides System.Text.Json serialization extensions for AudioDataConverter.
/// </summary>
public static class AudioDataConverterJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Serializes the AudioDataConverter instance to a JSON string.
    /// </summary>
    /// <param name="value">The AudioDataConverter instance to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation for readability.</param>
    /// <returns>A JSON string representation of the AudioDataConverter.</returns>
    public static string ToJson(this AudioDataConverter value, bool indented = false)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        var options = indented
            ? new JsonSerializerOptions(_jsonOptions)
            {
                WriteIndented = true
            }
            : _jsonOptions;

        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// Deserializes a JSON string to an AudioDataConverter instance.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>An AudioDataConverter instance, or null if the JSON is null or empty.</returns>
    public static AudioDataConverter? FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        return JsonSerializer.Deserialize<AudioDataConverter>(json, _jsonOptions);
    }

    /// <summary>
    /// Attempts to deserialize a JSON string to an AudioDataConverter instance.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">Receives the deserialized AudioDataConverter instance, or null if deserialization fails.</param>
    /// <returns>True if deserialization succeeded; otherwise, false.</returns>
    public static bool TryFromJson(string json, out AudioDataConverter? value)
    {
        value = null;

        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            value = JsonSerializer.Deserialize<AudioDataConverter>(json, _jsonOptions);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}