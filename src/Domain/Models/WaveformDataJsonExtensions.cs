// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Text.Json;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides System.Text.Json serialization extensions for WaveformData.
/// </summary>
public static class WaveformDataJsonExtensions
{
    /// <summary>
    /// Converts a WaveformData instance to a JSON string.
    /// </summary>
    /// <param name="waveform">The WaveformData to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation.</param>
    /// <returns>A JSON string representation of the WaveformData.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveform"/> is null.</exception>
    public static string ToJson(this WaveformData waveform, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(waveform);

        var value = new
        {
            waveform.SampleRate,
            waveform.ChannelCount,
            waveform.DownsamplingFactor,
            waveform.DataPointCount,
            waveform.IsNormalized,
            Data = waveform.GetData()
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = indented
        };

        return JsonSerializer.Serialize(value, options);
    }
}
