#nullable enable

using System;
using System.Text.Json;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides JSON serialization extensions for <see cref="SpectrogramData"/>.
/// </summary>
public static class SpectrogramDataJsonExtensions
{
    /// <summary>
    /// Serializes the spectrogram metadata and, optionally, its flattened frame data.
    /// </summary>
    /// <param name="spectrogram">The spectrogram to serialize.</param>
    /// <param name="includeMatrix">Whether to include the flattened data returned by <see cref="VisualizationData.GetData"/>.</param>
    /// <returns>A JSON representation of the spectrogram.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="spectrogram"/> is null.</exception>
    public static string ToJson(this SpectrogramData spectrogram, bool includeMatrix = false)
    {
        ArgumentNullException.ThrowIfNull(spectrogram);

        var value = new
        {
            spectrogram.TimeFrames,
            spectrogram.FrequencyBins,
            spectrogram.SampleRate,
            spectrogram.FftSize,
            spectrogram.HopSize,
            spectrogram.ColormapType,
            Frames = includeMatrix ? spectrogram.GetData() : null
        };

        return JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
    }
}
