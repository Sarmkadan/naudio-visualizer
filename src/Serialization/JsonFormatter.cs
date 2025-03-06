#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using NAudioVisualizer.Domain.Models;

namespace NAudioVisualizer.Serialization;

/// <summary>
/// Formats visualization data to JSON output.
/// Uses System.Text.Json for high-performance serialization.
/// </summary>
public class JsonFormatter : IOutputFormatter
{
    private readonly JsonSerializerOptions _options;
    private const int IndentSize = 2;

    /// <summary>
    /// Initializes a new instance of the JSON formatter with default options.
    /// </summary>
    public JsonFormatter(bool prettyPrint = true)
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = prettyPrint,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <summary>
    /// Formats waveform data as JSON.
    /// </summary>
    public string Format(WaveformData waveform)
    {
        if (waveform is null)
            throw new ArgumentNullException(nameof(waveform));

        var json = new Dictionary<string, object>
        {
            ["type"] = "waveform",
            ["timestamp"] = waveform.Timestamp,
            ["duration_ms"] = waveform.DurationMs,
            ["sample_rate"] = waveform.SampleRate,
            ["channel_count"] = waveform.ChannelCount,
            ["channels"] = FormatChannels(waveform)
        };

        return JsonSerializer.Serialize(json, _options);
    }

    /// <summary>
    /// Formats spectrum data as JSON.
    /// </summary>
    public string Format(SpectrumData spectrum)
    {
        if (spectrum is null)
            throw new ArgumentNullException(nameof(spectrum));

        var json = new Dictionary<string, object>
        {
            ["type"] = "spectrum",
            ["timestamp"] = spectrum.Timestamp,
            ["frequency_bins"] = spectrum.FrequencyBins.Length,
            ["sample_rate"] = spectrum.SampleRate,
            ["frequency_resolution"] = spectrum.FrequencyResolution,
            ["magnitude"] = FormatMagnitude(spectrum.Magnitude),
            ["peak_frequency"] = spectrum.PeakFrequency
        };

        return JsonSerializer.Serialize(json, _options);
    }

    /// <summary>
    /// Formats spectrogram data as JSON.
    /// </summary>
    public string Format(SpectrogramData spectrogram)
    {
        if (spectrogram is null)
            throw new ArgumentNullException(nameof(spectrogram));

        var json = new Dictionary<string, object>
        {
            ["type"] = "spectrogram",
            ["timestamp"] = spectrogram.Timestamp,
            ["time_frames"] = spectrogram.TimeFrames,
            ["frequency_bins"] = spectrogram.FrequencyBins,
            ["sample_rate"] = spectrogram.SampleRate,
            ["frequency_resolution"] = spectrogram.FrequencyResolution,
            ["time_resolution_ms"] = spectrogram.TimeResolutionMs,
            ["dimensions"] = new { width = spectrogram.TimeFrames, height = spectrogram.FrequencyBins }
        };

        return JsonSerializer.Serialize(json, _options);
    }

    /// <summary>
    /// Formats multiple visualization data items as a JSON array.
    /// </summary>
    public string FormatCollection<T>(IEnumerable<T> items) where T : VisualizationData
    {
        if (items is null)
            throw new ArgumentNullException(nameof(items));

        var array = new List<object>();
        foreach (var item in items)
        {
            array.Add(JsonSerializer.Deserialize<object>(Format(item))!);
        }

        return JsonSerializer.Serialize(array, _options);
    }

    /// <summary>
    /// Formats metadata about a visualization session.
    /// </summary>
    public string FormatMetadata(Dictionary<string, string> metadata)
    {
        return JsonSerializer.Serialize(metadata, _options);
    }

    /// <summary>
    /// Parses JSON string back to a waveform object.
    /// </summary>
    public WaveformData? ParseWaveform(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            int channelCount = root.GetProperty("channel_count").GetInt32();
            var channelsElement = root.GetProperty("channels");

            var waveform = new WaveformData
            {
                ChannelCount = channelCount,
                SampleRate = root.GetProperty("sample_rate").GetInt32(),
                DurationMs = root.GetProperty("duration_ms").GetDouble(),
                Timestamp = root.GetProperty("timestamp").GetDouble()
            };

            return waveform;
        }
        catch (JsonException ex)
        {
            throw new FormatException("Failed to parse waveform JSON.", ex);
        }
    }

    /// <summary>
    /// Parses JSON string back to a spectrum object.
    /// </summary>
    public SpectrumData? ParseSpectrum(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var spectrum = new SpectrumData
            {
                SampleRate = root.GetProperty("sample_rate").GetInt32(),
                Timestamp = root.GetProperty("timestamp").GetDouble()
            };

            return spectrum;
        }
        catch (JsonException ex)
        {
            throw new FormatException("Failed to parse spectrum JSON.", ex);
        }
    }

    /// <summary>
    /// Helper method to format channel data.
    /// </summary>
    private object FormatChannels(WaveformData waveform)
    {
        var channels = new List<object>();
        for (int i = 0; i < waveform.ChannelCount; i++)
        {
            channels.Add(new { channel = i, min = -1.0, max = 1.0, rms = 0.5 });
        }
        return channels;
    }

    /// <summary>
    /// Helper method to format magnitude data with reduced precision for smaller output.
    /// </summary>
    private object FormatMagnitude(float[] magnitude)
    {
        if (magnitude.Length > 1000)
        {
            // Sample for large arrays
            int step = magnitude.Length / 500;
            var sampled = new List<float>();
            for (int i = 0; i < magnitude.Length; i += step)
                sampled.Add(magnitude[i]);
            return sampled;
        }

        return magnitude;
    }
}

/// <summary>
/// Interface for output formatters.
/// </summary>
public interface IOutputFormatter
{
    string Format(WaveformData waveform);
    string Format(SpectrumData spectrum);
    string Format(SpectrogramData spectrogram);
    string FormatCollection<T>(IEnumerable<T> items) where T : VisualizationData;
    string FormatMetadata(Dictionary<string, string> metadata);
}
