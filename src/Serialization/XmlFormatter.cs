// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NAudioVisualizer.Domain.Models;

namespace NAudioVisualizer.Serialization;

/// <summary>
/// Formats visualization data to XML output.
/// XML format provides structured, hierarchical representation of audio data.
/// </summary>
public class XmlFormatter : IOutputFormatter
{
    private const string RootElementName = "VisualizationData";
    private const string MetadataElementName = "Metadata";

    /// <summary>
    /// Formats waveform data as XML.
    /// </summary>
    public string Format(WaveformData waveform)
    {
        if (waveform == null)
            throw new ArgumentNullException(nameof(waveform));

        var root = new XElement("Waveform",
            new XAttribute("timestamp", waveform.Timestamp),
            new XAttribute("duration_ms", waveform.DurationMs),
            new XAttribute("sample_rate", waveform.SampleRate),
            new XAttribute("channel_count", waveform.ChannelCount),
            new XElement("Channels",
                CreateChannelElements(waveform.ChannelCount)
            )
        );

        return root.ToString();
    }

    /// <summary>
    /// Formats spectrum data as XML.
    /// </summary>
    public string Format(SpectrumData spectrum)
    {
        if (spectrum == null)
            throw new ArgumentNullException(nameof(spectrum));

        var root = new XElement("Spectrum",
            new XAttribute("timestamp", spectrum.Timestamp),
            new XAttribute("frequency_bins", spectrum.FrequencyBins.Length),
            new XAttribute("sample_rate", spectrum.SampleRate),
            new XAttribute("peak_frequency", spectrum.PeakFrequency),
            new XElement("FrequencyResolution", spectrum.FrequencyResolution),
            new XElement("Magnitude",
                CreateMagnitudeElements(spectrum.Magnitude, spectrum.FrequencyBins)
            )
        );

        return root.ToString();
    }

    /// <summary>
    /// Formats spectrogram data as XML.
    /// </summary>
    public string Format(SpectrogramData spectrogram)
    {
        if (spectrogram == null)
            throw new ArgumentNullException(nameof(spectrogram));

        var root = new XElement("Spectrogram",
            new XAttribute("timestamp", spectrogram.Timestamp),
            new XAttribute("time_frames", spectrogram.TimeFrames),
            new XAttribute("frequency_bins", spectrogram.FrequencyBins),
            new XAttribute("sample_rate", spectrogram.SampleRate),
            new XElement("Dimensions",
                new XElement("Width", spectrogram.TimeFrames),
                new XElement("Height", spectrogram.FrequencyBins)
            ),
            new XElement("Properties",
                new XElement("FrequencyResolution", spectrogram.FrequencyResolution),
                new XElement("TimeResolutionMs", spectrogram.TimeResolutionMs)
            )
        );

        return root.ToString();
    }

    /// <summary>
    /// Formats a collection of visualization data as XML.
    /// </summary>
    public string FormatCollection<T>(IEnumerable<T> items) where T : VisualizationData
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        var elements = new List<XElement>();
        foreach (var item in items)
        {
            elements.Add(new XElement("Item",
                new XAttribute("type", item.GetType().Name),
                new XAttribute("timestamp", item.Timestamp),
                new XAttribute("duration_ms", item.DurationMs),
                new XAttribute("sample_rate", item.SampleRate)
            ));
        }

        var root = new XElement("VisualizationCollection", elements);
        return root.ToString();
    }

    /// <summary>
    /// Formats metadata as XML.
    /// </summary>
    public string FormatMetadata(Dictionary<string, string> metadata)
    {
        if (metadata == null)
            throw new ArgumentNullException(nameof(metadata));

        var elements = new List<XElement>();
        foreach (var kvp in metadata)
        {
            elements.Add(new XElement("Property",
                new XAttribute("name", kvp.Key),
                new XText(kvp.Value)
            ));
        }

        var root = new XElement(MetadataElementName, elements);
        return root.ToString();
    }

    /// <summary>
    /// Helper method to create channel elements for waveform data.
    /// </summary>
    private IEnumerable<XElement> CreateChannelElements(int channelCount)
    {
        for (int i = 0; i < channelCount; i++)
        {
            yield return new XElement("Channel",
                new XAttribute("index", i),
                new XElement("Min", "-1.0"),
                new XElement("Max", "1.0"),
                new XElement("RMS", "0.5")
            );
        }
    }

    /// <summary>
    /// Helper method to create magnitude elements for spectrum data.
    /// </summary>
    private IEnumerable<XElement> CreateMagnitudeElements(float[] magnitude, float[] frequencies)
    {
        int step = magnitude.Length > 500 ? magnitude.Length / 500 : 1;

        for (int i = 0; i < magnitude.Length; i += step)
        {
            yield return new XElement("Bin",
                new XAttribute("index", i),
                new XAttribute("frequency", $"{frequencies[i]:F2}"),
                new XText($"{magnitude[i]:F2}")
            );
        }
    }

    /// <summary>
    /// Parses XML string back to a waveform object.
    /// </summary>
    public WaveformData? ParseWaveform(string xml)
    {
        try
        {
            var root = XElement.Parse(xml);
            var waveform = new WaveformData
            {
                Timestamp = double.Parse(root.Attribute("timestamp")?.Value ?? "0"),
                DurationMs = double.Parse(root.Attribute("duration_ms")?.Value ?? "0"),
                SampleRate = int.Parse(root.Attribute("sample_rate")?.Value ?? "44100"),
                ChannelCount = int.Parse(root.Attribute("channel_count")?.Value ?? "2")
            };

            return waveform;
        }
        catch (Exception ex)
        {
            throw new FormatException("Failed to parse waveform XML.", ex);
        }
    }

    /// <summary>
    /// Parses XML string back to a spectrum object.
    /// </summary>
    public SpectrumData? ParseSpectrum(string xml)
    {
        try
        {
            var root = XElement.Parse(xml);
            var spectrum = new SpectrumData
            {
                Timestamp = double.Parse(root.Attribute("timestamp")?.Value ?? "0"),
                SampleRate = int.Parse(root.Attribute("sample_rate")?.Value ?? "44100")
            };

            return spectrum;
        }
        catch (Exception ex)
        {
            throw new FormatException("Failed to parse spectrum XML.", ex);
        }
    }
}
