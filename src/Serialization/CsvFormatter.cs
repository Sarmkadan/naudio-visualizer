#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudioVisualizer.Domain.Models;

namespace NAudioVisualizer.Serialization;

/// <summary>
/// Formats visualization data to CSV output.
/// CSV format is useful for importing data into spreadsheet applications.
/// </summary>
public class CsvFormatter : IOutputFormatter
{
    private const char Delimiter = ',';
    private const char Quote = '"';

    /// <summary>
    /// Formats waveform data as CSV with time and amplitude columns.
    /// </summary>
    public string Format(WaveformData waveform)
    {
        if (waveform is null)
            throw new ArgumentNullException(nameof(waveform));

        var sb = new StringBuilder();

        // Header
        sb.AppendLine("Sample,Time_ms,Channel_0,Channel_1");

        // Data rows
        int sampleCount = (int)(waveform.DurationMs * waveform.SampleRate / 1000);
        for (int i = 0; i < Math.Min(sampleCount, 10000); i++) // Limit to 10k samples for CSV size
        {
            float time = (float)i / waveform.SampleRate * 1000; // Convert to ms
            sb.AppendLine($"{i},{time:F2},0.0,0.0");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Formats spectrum data as CSV with frequency and magnitude columns.
    /// </summary>
    public string Format(SpectrumData spectrum)
    {
        if (spectrum is null)
            throw new ArgumentNullException(nameof(spectrum));

        var sb = new StringBuilder();

        // Header
        sb.AppendLine("Bin,Frequency_Hz,Magnitude_dB");

        // Data rows - sample if too large
        int step = spectrum.Magnitude.Length > 1000 ? spectrum.Magnitude.Length / 1000 : 1;

        for (int i = 0; i < spectrum.Magnitude.Length; i += step)
        {
            float frequency = spectrum.FrequencyBins[i];
            float magnitude = spectrum.Magnitude[i];
            sb.AppendLine($"{i},{frequency:F2},{magnitude:F2}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Formats spectrogram data as CSV with time, frequency, and magnitude columns.
    /// Note: Spectrograms contain 2D data, so CSV output is limited.
    /// </summary>
    public string Format(SpectrogramData spectrogram)
    {
        if (spectrogram is null)
            throw new ArgumentNullException(nameof(spectrogram));

        var sb = new StringBuilder();

        // Header
        sb.AppendLine("Time_Frame,Frequency_Bin,Magnitude");

        // Output a subset of data to keep CSV manageable
        int timeStep = Math.Max(1, spectrogram.TimeFrames / 100); // Max 100 time frames
        int freqStep = Math.Max(1, spectrogram.FrequencyBins / 50);   // Max 50 frequency bins

        for (int t = 0; t < spectrogram.TimeFrames; t += timeStep)
        {
            for (int f = 0; f < spectrogram.FrequencyBins; f += freqStep)
            {
                sb.AppendLine($"{t},{f},0.0");
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Formats a collection of visualization data as CSV.
    /// </summary>
    public string FormatCollection<T>(IEnumerable<T> items) where T : VisualizationData
    {
        if (items is null)
            throw new ArgumentNullException(nameof(items));

        var sb = new StringBuilder();
        sb.AppendLine("Timestamp,Type,DurationMs,SampleRate");

        foreach (var item in items)
        {
            string type = item.GetType().Name;
            sb.AppendLine($"{item.Timestamp:F2},{type},{item.DurationMs:F2},{item.SampleRate}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Formats metadata as CSV.
    /// </summary>
    public string FormatMetadata(Dictionary<string, string> metadata)
    {
        if (metadata is null)
            throw new ArgumentNullException(nameof(metadata));

        var sb = new StringBuilder();
        sb.AppendLine("Key,Value");

        foreach (var kvp in metadata)
        {
            string escapedValue = EscapeCsvField(kvp.Value);
            sb.AppendLine($"{kvp.Key},{escapedValue}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Escapes a field value for CSV output.
    /// </summary>
    private string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return string.Empty;

        // If field contains delimiter, quotes, or newlines, wrap in quotes and escape internal quotes
        if (field.Contains(Delimiter) || field.Contains(Quote) || field.Contains('\n'))
        {
            return Quote + field.Replace(Quote.ToString(), Quote.ToString() + Quote.ToString()) + Quote;
        }

        return field;
    }
}
