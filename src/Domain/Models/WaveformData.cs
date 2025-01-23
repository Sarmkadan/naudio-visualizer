// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Linq;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Represents waveform visualization data derived from audio samples.
/// </summary>
public class WaveformData : VisualizationData
{
    /// <summary>
    /// Waveform amplitude values, normalized between -1.0 and 1.0.
    /// </summary>
    private float[] _waveformSamples = [];

    /// <summary>
    /// Sample rate of the source audio.
    /// </summary>
    public int SampleRate { get; set; }

    /// <summary>
    /// Number of channels (mono=1, stereo=2).
    /// </summary>
    public int ChannelCount { get; set; }

    /// <summary>
    /// Downsampling factor applied to create this waveform.
    /// </summary>
    public int DownsamplingFactor { get; set; } = 1;

    /// <summary>
    /// Peak values for left channel (for stereo displays).
    /// </summary>
    public float[]? LeftChannelPeaks { get; private set; }

    /// <summary>
    /// Peak values for right channel (for stereo displays).
    /// </summary>
    public float[]? RightChannelPeaks { get; private set; }

    /// <summary>
    /// Initializes a new waveform data with samples.
    /// </summary>
    public WaveformData(float[] samples, int channelCount, int sampleRate, int downsamplingFactor = 1)
    {
        _waveformSamples = samples ?? throw new ArgumentNullException(nameof(samples));
        ChannelCount = channelCount;
        SampleRate = sampleRate;
        DownsamplingFactor = downsamplingFactor;
        DataPointCount = samples.Length;
        VisualizationType = VisualizationType.Waveform;

        CalculateMinMax();
    }

    /// <summary>
    /// Gets the waveform samples.
    /// </summary>
    public override float[] GetData() => _waveformSamples;

    /// <summary>
    /// Sets the waveform samples and recalculates metrics.
    /// </summary>
    public void SetSamples(float[] samples)
    {
        _waveformSamples = samples ?? throw new ArgumentNullException(nameof(samples));
        DataPointCount = samples.Length;
        CalculateMinMax();
    }

    /// <summary>
    /// Normalizes the waveform data to the 0-1 range.
    /// </summary>
    public override void Normalize()
    {
        if (IsNormalized || _waveformSamples.Length == 0)
            return;

        float range = MaxValue - MinValue;
        if (Math.Abs(range) < 0.0001f)
            return;

        for (int i = 0; i < _waveformSamples.Length; i++)
        {
            _waveformSamples[i] = (_waveformSamples[i] - MinValue) / range;
        }

        MinValue = 0f;
        MaxValue = 1f;
        IsNormalized = true;
    }

    /// <summary>
    /// Calculates peak values for stereo channel separation.
    /// </summary>
    public void CalculateStereoChannels()
    {
        if (ChannelCount != 2 || _waveformSamples.Length < 2)
            return;

        LeftChannelPeaks = new float[_waveformSamples.Length / 2];
        RightChannelPeaks = new float[_waveformSamples.Length / 2];

        for (int i = 0; i < LeftChannelPeaks.Length; i++)
        {
            LeftChannelPeaks[i] = _waveformSamples[i * 2];
            RightChannelPeaks[i] = _waveformSamples[i * 2 + 1];
        }
    }

    /// <summary>
    /// Downsamples the waveform data to reduce point count.
    /// </summary>
    public void Downsample(int factor)
    {
        if (factor <= 1 || _waveformSamples.Length <= factor)
            return;

        var downsampled = new float[_waveformSamples.Length / factor];
        for (int i = 0; i < downsampled.Length; i++)
        {
            downsampled[i] = _waveformSamples[i * factor];
        }

        _waveformSamples = downsampled;
        DownsamplingFactor = factor;
        DataPointCount = downsampled.Length;
        CalculateMinMax();
    }

    /// <summary>
    /// Calculates minimum and maximum values in the waveform.
    /// </summary>
    private void CalculateMinMax()
    {
        if (_waveformSamples.Length == 0)
        {
            MinValue = 0f;
            MaxValue = 1f;
            return;
        }

        MinValue = _waveformSamples.Min();
        MaxValue = _waveformSamples.Max();
    }

    /// <summary>
    /// Validates the waveform data integrity.
    /// </summary>
    public override bool IsValid()
    {
        return _waveformSamples.Length > 0 &&
               ChannelCount > 0 &&
               SampleRate > 0 &&
               !float.IsNaN(MinValue) &&
               !float.IsNaN(MaxValue);
    }
}
