#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Represents spectrogram visualization data (time-frequency representation).
/// </summary>
public class SpectrogramData : VisualizationData
{
    /// <summary>
    /// 2D array of magnitude values [time_bins][frequency_bins].
    /// </summary>
    private float[][] _spectrogramMatrix = [];

    /// <summary>
    /// List of spectrum frames over time.
    /// </summary>
    private List<SpectrumData> _spectrumFrames = [];

    /// <summary>
    /// Number of frequency bins.
    /// </summary>
    public int FrequencyBins { get; set; }

    /// <summary>
    /// Number of time frames.
    /// </summary>
    public int TimeFrames { get; private set; }

    /// <summary>
    /// Sample rate of the source audio.
    /// </summary>
    public int SampleRate { get; set; }

    /// <summary>
    /// FFT size used for each spectrum frame.
    /// </summary>
    public int FftSize { get; set; }

    /// <summary>
    /// Hop size (overlap) between consecutive frames.
    /// </summary>
    public int HopSize { get; set; }

    /// <summary>
    /// Time per frame in seconds.
    /// </summary>
    public float TimePerFrame { get; private set; }

    /// <summary>
    /// Frequency resolution (Hz per bin).
    /// </summary>
    public float FrequencyResolution { get; private set; }

    /// <summary>
    /// Color mapping mode for visualization.
    /// </summary>
    public ColormapType ColormapType { get; set; } = ColormapType.Viridis;

    /// <summary>
    /// Initializes a new spectrogram with 2D magnitude matrix.
    /// </summary>
    public SpectrogramData(float[][] spectrogramMatrix, int sampleRate, int fftSize, int hopSize)
    {
        _spectrogramMatrix = spectrogramMatrix ?? throw new ArgumentNullException(nameof(spectrogramMatrix));
        SampleRate = sampleRate;
        FftSize = fftSize;
        HopSize = hopSize;
        FrequencyBins = spectrogramMatrix.Length > 0 ? spectrogramMatrix[0].Length : 0;
        TimeFrames = spectrogramMatrix.Length;
        DataPointCount = FrequencyBins * TimeFrames;
        VisualizationType = VisualizationType.Spectrogram;

        TimePerFrame = (float)hopSize / sampleRate;
        FrequencyResolution = (float)sampleRate / fftSize;
        CalculateMinMax();
    }

    /// <summary>
    /// Gets the full spectrogram matrix as a flattened array.
    /// </summary>
    public override float[] GetData()
    {
        var flattened = new float[FrequencyBins * TimeFrames];
        int idx = 0;

        for (int t = 0; t < TimeFrames; t++)
        {
            for (int f = 0; f < FrequencyBins; f++)
            {
                flattened[idx++] = _spectrogramMatrix[t][f];
            }
        }

        return flattened;
    }

    /// <summary>
    /// Gets a single time frame as a spectrum.
    /// </summary>
    public float[] GetTimeFrame(int timeIndex)
    {
        if (timeIndex < 0 || timeIndex >= TimeFrames)
            throw new ArgumentOutOfRangeException(nameof(timeIndex));

        return (float[])_spectrogramMatrix[timeIndex].Clone();
    }

    /// <summary>
    /// Gets a frequency slice across all time frames.
    /// </summary>
    public float[] GetFrequencySlice(int frequencyIndex)
    {
        if (frequencyIndex < 0 || frequencyIndex >= FrequencyBins)
            throw new ArgumentOutOfRangeException(nameof(frequencyIndex));

        var slice = new float[TimeFrames];
        for (int t = 0; t < TimeFrames; t++)
        {
            slice[t] = _spectrogramMatrix[t][frequencyIndex];
        }

        return slice;
    }

    /// <summary>
    /// Adds a spectrum frame to the spectrogram.
    /// </summary>
    public void AddSpectrumFrame(SpectrumData spectrum)
    {
        if (spectrum is null)
            throw new ArgumentNullException(nameof(spectrum));

        _spectrumFrames.Add(spectrum);
    }

    /// <summary>
    /// Gets all spectrum frames.
    /// </summary>
    public IReadOnlyList<SpectrumData> GetSpectrumFrames() => _spectrumFrames.AsReadOnly();

    /// <summary>
    /// Normalizes spectrogram data to 0-1 range.
    /// </summary>
    public override void Normalize()
    {
        if (IsNormalized)
            return;

        float range = MaxValue - MinValue;
        if (Math.Abs(range) < 0.0001f)
            return;

        for (int t = 0; t < TimeFrames; t++)
        {
            for (int f = 0; f < FrequencyBins; f++)
            {
                _spectrogramMatrix[t][f] = (_spectrogramMatrix[t][f] - MinValue) / range;
            }
        }

        MinValue = 0f;
        MaxValue = 1f;
        IsNormalized = true;
    }

    /// <summary>
    /// Applies logarithmic scaling to spectrogram magnitudes.
    /// </summary>
    public void ApplyLogScale(float referenceValue = 1f)
    {
        for (int t = 0; t < TimeFrames; t++)
        {
            for (int f = 0; f < FrequencyBins; f++)
            {
                float magnitude = Math.Max(_spectrogramMatrix[t][f], 0.00001f);
                _spectrogramMatrix[t][f] = 20f * (float)Math.Log10(magnitude / referenceValue);
            }
        }

        CalculateMinMax();
    }

    /// <summary>
    /// Calculates minimum and maximum magnitude values across all frames.
    /// </summary>
    private void CalculateMinMax()
    {
        if (TimeFrames == 0 || FrequencyBins == 0)
        {
            MinValue = 0f;
            MaxValue = 1f;
            return;
        }

        float min = _spectrogramMatrix[0][0];
        float max = _spectrogramMatrix[0][0];

        for (int t = 0; t < TimeFrames; t++)
        {
            for (int f = 0; f < FrequencyBins; f++)
            {
                float value = _spectrogramMatrix[t][f];
                if (value < min) min = value;
                if (value > max) max = value;
            }
        }

        MinValue = min;
        MaxValue = max;
    }

    /// <summary>
    /// Validates the spectrogram data integrity.
    /// </summary>
    public override bool IsValid()
    {
        return _spectrogramMatrix.Length > 0 &&
               FrequencyBins > 0 &&
               TimeFrames > 0 &&
               SampleRate > 0 &&
               FftSize > 0 &&
               !float.IsNaN(MinValue) &&
               !float.IsNaN(MaxValue);
    }
}

/// <summary>
/// Color mapping types for spectrogram visualization.
/// </summary>
public enum ColormapType
{
    Viridis = 0,
    Plasma = 1,
    Inferno = 2,
    Magma = 3,
    Turbo = 4,
    Hot = 5
}
