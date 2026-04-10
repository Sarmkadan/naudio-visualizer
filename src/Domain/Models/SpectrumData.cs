#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Linq;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Represents frequency spectrum visualization data (FFT magnitude spectrum).
/// </summary>
public class SpectrumData : VisualizationData
{
    /// <summary>
    /// FFT magnitude values for each frequency bin.
    /// </summary>
    private float[] _magnitudes = [];

    /// <summary>
    /// Frequency values corresponding to each magnitude bin (in Hz).
    /// </summary>
    private float[] _frequencies = [];

    /// <summary>
    /// Sample rate of the source audio.
    /// </summary>
    public int SampleRate { get; set; }

    /// <summary>
    /// FFT size used for spectrum analysis.
    /// </summary>
    public int FftSize { get; set; }

    /// <summary>
    /// Window function applied to samples (Hann, Hamming, etc.).
    /// </summary>
    public WindowType WindowType { get; set; } = WindowType.Hann;

    /// <summary>
    /// Whether magnitude values are in logarithmic scale (dB).
    /// </summary>
    public bool IsLogScale { get; private set; }

    /// <summary>
    /// Frequency resolution (Hz per bin).
    /// </summary>
    public float FrequencyResolution { get; private set; }

    /// <summary>
    /// Peak frequency detected in the spectrum.
    /// </summary>
    public float PeakFrequency { get; private set; }

    /// <summary>
    /// Magnitude at the peak frequency.
    /// </summary>
    public float PeakMagnitude { get; private set; }

    /// <summary>
    /// Initializes a new spectrum data with magnitude and frequency values.
    /// </summary>
    public SpectrumData(float[] magnitudes, float[] frequencies, int sampleRate, int fftSize)
    {
        _magnitudes = magnitudes ?? throw new ArgumentNullException(nameof(magnitudes));
        _frequencies = frequencies ?? throw new ArgumentNullException(nameof(frequencies));
        SampleRate = sampleRate;
        FftSize = fftSize;
        DataPointCount = magnitudes.Length;
        VisualizationType = VisualizationType.Spectrum;

        FrequencyResolution = (float)sampleRate / fftSize;
        CalculateMinMax();
        CalculatePeakFrequency();
    }

    /// <summary>
    /// Gets the magnitude spectrum data.
    /// </summary>
    public override float[] GetData() => _magnitudes;

    /// <summary>
    /// Gets the frequency values for each magnitude bin.
    /// </summary>
    public float[] GetFrequencies() => _frequencies;

    /// <summary>
    /// Converts magnitude values to logarithmic scale (dB).
    /// </summary>
    public void ConvertToLogScale(float referenceValue = 1f)
    {
        if (IsLogScale)
            return;

        for (int i = 0; i < _magnitudes.Length; i++)
        {
            float magnitude = Math.Max(_magnitudes[i], 0.00001f);
            _magnitudes[i] = 20f * (float)Math.Log10(magnitude / referenceValue);
        }

        IsLogScale = true;
        CalculateMinMax();
    }

    /// <summary>
    /// Normalizes spectrum data to 0-1 range.
    /// </summary>
    public override void Normalize()
    {
        if (IsNormalized || _magnitudes.Length == 0)
            return;

        float range = MaxValue - MinValue;
        if (Math.Abs(range) < 0.0001f)
            return;

        for (int i = 0; i < _magnitudes.Length; i++)
        {
            _magnitudes[i] = (_magnitudes[i] - MinValue) / range;
        }

        MinValue = 0f;
        MaxValue = 1f;
        IsNormalized = true;
    }

    /// <summary>
    /// Applies smoothing to the spectrum using moving average.
    /// </summary>
    public void SmoothSpectrum(int windowSize = 3)
    {
        if (windowSize <= 1 || _magnitudes.Length <= windowSize)
            return;

        var smoothed = new float[_magnitudes.Length];
        int half = windowSize / 2;

        for (int i = 0; i < _magnitudes.Length; i++)
        {
            float sum = 0f;
            int count = 0;

            for (int j = -half; j <= half; j++)
            {
                int idx = i + j;
                if (idx >= 0 && idx < _magnitudes.Length)
                {
                    sum += _magnitudes[idx];
                    count++;
                }
            }

            smoothed[i] = count > 0 ? sum / count : _magnitudes[i];
        }

        _magnitudes = smoothed;
        CalculateMinMax();
    }

    /// <summary>
    /// Finds the peak frequency and magnitude.
    /// </summary>
    private void CalculatePeakFrequency()
    {
        if (_magnitudes.Length == 0 || _frequencies.Length == 0)
            return;

        int peakIndex = 0;
        float peakValue = _magnitudes[0];

        for (int i = 1; i < _magnitudes.Length; i++)
        {
            if (_magnitudes[i] > peakValue)
            {
                peakValue = _magnitudes[i];
                peakIndex = i;
            }
        }

        PeakMagnitude = peakValue;
        PeakFrequency = _frequencies[peakIndex];
    }

    /// <summary>
    /// Calculates minimum and maximum magnitude values.
    /// </summary>
    private void CalculateMinMax()
    {
        if (_magnitudes.Length == 0)
        {
            MinValue = 0f;
            MaxValue = 1f;
            return;
        }

        MinValue = _magnitudes.Min();
        MaxValue = _magnitudes.Max();
    }

    /// <summary>
    /// Validates the spectrum data integrity.
    /// </summary>
    public override bool IsValid()
    {
        return _magnitudes.Length > 0 &&
               _frequencies.Length == _magnitudes.Length &&
               SampleRate > 0 &&
               FftSize > 0 &&
               !float.IsNaN(MinValue) &&
               !float.IsNaN(MaxValue);
    }
}

/// <summary>
/// Window functions for FFT analysis.
/// </summary>
public enum WindowType
{
    Hann = 0,
    Hamming = 1,
    Blackman = 2,
    Rectangular = 3
}
