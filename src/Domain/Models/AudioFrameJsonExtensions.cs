// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Text.Json;
using System.Text.Json.Serialization;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides System.Text.Json serialization extensions for <see cref="AudioFrame"/>.
/// </summary>
public static class AudioFrameJsonExtensions
{
    /// <summary>
    /// Converts an <see cref="AudioFrame"/> to a JSON string.
    /// </summary>
    /// <param name="frame">The audio frame to serialize.</param>
    /// <param name="includeSamples">
    /// <see langword="true"/> to include raw audio samples; otherwise, <see langword="false"/>.
    /// </param>
    /// <returns>A JSON string representation of the audio frame.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="frame"/> is null.</exception>
    public static string ToJson(this AudioFrame frame, bool includeSamples = false)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var dto = new
        {
            frame.Id,
            Samples = includeSamples ? frame.Samples : null,
            frame.ChannelCount,
            frame.SampleRate,
            frame.Timestamp,
            frame.FrameIndex,
            frame.DurationSeconds,
            frame.PeakAmplitude,
            frame.RmsEnergy
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        };

        return JsonSerializer.Serialize(dto, options);
    }
}
