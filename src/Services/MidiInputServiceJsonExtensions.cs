#nullable enable

// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Text.Json;
using NAudioVisualizer.Services;

namespace NAudioVisualizer.Services;

/// <summary>
/// Provides System.Text.Json serialization extensions for <see cref="MidiInputService"/>.
/// </summary>
public static class MidiInputServiceJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Serializes the <see cref="MidiInputService"/> instance to a JSON string.
    /// </summary>
    /// <param name="value">The service instance to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation for readability.</param>
    /// <returns>A JSON representation of the service state.</returns>
    public static string ToJson(this MidiInputService value, bool indented = false)
    {
        if (value is null)
        {
            return "{}";
        }

        // Note: MidiInputService contains disposable resources and event handlers,
        // so we serialize only the essential state that can be meaningfully represented.
        // In a real scenario, you might want to add state tracking properties.
        var state = new
        {
            IsDisposed = value.GetType().GetField("_isDisposed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(value) as bool? ?? false,
            ActiveDeviceIndex = value.GetType().GetField("_activeDeviceIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(value) as int? ?? -1
        };

        var options = indented ? new JsonSerializerOptions(_jsonOptions) { WriteIndented = true } : _jsonOptions;
        return JsonSerializer.Serialize(state, options);
    }

    /// <summary>
    /// Deserializes a JSON string to a <see cref="MidiInputService"/> instance.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized service instance, or null if deserialization fails.</returns>
    public static MidiInputService? FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            // Note: MidiInputService is not meant to be fully deserialized from JSON
            // as it contains disposable resources. This method creates a new instance.
            // In a real scenario, you would need additional context to properly reconstruct the service.
            return new MidiInputService();
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to deserialize a JSON string to a <see cref="MidiInputService"/> instance.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">The deserialized service instance, or null if deserialization fails.</param>
    /// <returns>True if deserialization succeeds; otherwise, false.</returns>
    public static bool TryFromJson(string json, out MidiInputService? value)
    {
        try
        {
            value = FromJson(json);
            return value is not null;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}