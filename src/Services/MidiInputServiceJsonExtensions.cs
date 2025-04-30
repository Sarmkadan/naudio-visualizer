#nullable enable

// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Text.Json;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Events;

namespace NAudioVisualizer.Services;

/// <summary>
/// Represents the serializable state of a <see cref="MidiInputService"/> instance.
/// </summary>
/// <param name="IsDisposed">Indicates whether the service has been disposed.</param>
/// <param name="ActiveDeviceIndex">The index of the currently active MIDI device, or -1 if none.</param>
internal sealed record MidiInputServiceState(bool IsDisposed, int ActiveDeviceIndex);

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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static string ToJson(this MidiInputService value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        // Serialize only the essential state that can be meaningfully represented.
        // MidiInputService contains disposable resources and event handlers that cannot be serialized.
        var state = new MidiInputServiceState(value.IsDisposed, value.ActiveDeviceIndex);

        var options = indented ? new JsonSerializerOptions(_jsonOptions) { WriteIndented = true } : _jsonOptions;
        return JsonSerializer.Serialize(state, options);
    }

    /// <summary>
    /// Deserializes a JSON string to a <see cref="MidiInputService"/> instance.
    /// </summary>
    /// <remarks>
    /// Note: <see cref="MidiInputService"/> contains disposable resources and event handlers that cannot be deserialized.
    /// This method creates a new instance regardless of the JSON content.
    /// </remarks>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized service instance, or null if deserialization fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="json"/> is empty or whitespace.</exception>
    public static MidiInputService? FromJson(string json)
    {
        ArgumentNullException.ThrowIfNull(json);

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON string cannot be empty or whitespace.", nameof(json));
        }

        try
        {
            // MidiInputService is not meant to be fully deserialized from JSON
            // as it contains disposable resources and event handlers.
            // This method creates a new instance regardless of JSON content.
            return new MidiInputService();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to deserialize a JSON string to a <see cref="MidiInputService"/> instance.
    /// </summary>
    /// <remarks>
    /// Note: <see cref="MidiInputService"/> contains disposable resources and event handlers that cannot be serialized.
    /// This method creates a new instance regardless of the JSON content.
    /// </remarks>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">The deserialized service instance, or null if deserialization fails.</param>
    /// <returns>True if deserialization succeeds; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is null.</exception>
    public static bool TryFromJson(string json, out MidiInputService? value)
    {
        ArgumentNullException.ThrowIfNull(json);

        try
        {
            value = FromJson(json);
            return value is not null;
        }
        catch
        {
            value = null;
            return false;
        }
    }
}