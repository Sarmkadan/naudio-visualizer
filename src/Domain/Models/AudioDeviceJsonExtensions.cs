using System;
using System.Text.Json;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides JSON serialization extensions for <see cref="AudioDevice"/>.
/// </summary>
public static class AudioDeviceJsonExtensions
{
    /// <summary>
    /// Serializes the device's essential audio properties to JSON.
    /// </summary>
    /// <param name="device">The audio device to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation.</param>
    /// <returns>A JSON representation of the audio device.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="device"/> is null.</exception>
    public static string ToJson(this AudioDevice device, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(device);

        return JsonSerializer.Serialize(
            new
            {
                device.Name,
                device.DeviceIndex,
                device.ChannelCount,
                device.DefaultSampleRate,
                device.SupportedSampleRates,
                device.IsAvailable
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = indented
            });
    }
}
