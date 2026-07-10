#nullable enable

// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides extension methods for <see cref="WaveformData"/> to enable common waveform operations
/// and conversions without modifying the original class.
/// </summary>
public static class WaveformDataExtensions
{
    /// <summary>
    /// Creates a new <see cref="WaveformData"/> instance by combining left and right channel samples.
    /// </summary>
    /// <param name="leftSamples">Left channel samples (must not be null or empty)</param>
    /// <param name="rightSamples">Right channel samples (must not be null or empty)</param>
    /// <param name="sampleRate">Sample rate of the audio</param>
    /// <param name="downsamplingFactor">Downsampling factor to apply</param>
    /// <returns>A new <see cref="WaveformData"/> instance containing interleaved stereo samples</returns>
    /// <exception cref="ArgumentNullException">Thrown if leftSamples or rightSamples is null</exception>
    /// <exception cref="ArgumentException">Thrown if leftSamples or rightSamples is empty or arrays have different lengths</exception>
    public static WaveformData ToStereoWaveform(this float[] leftSamples, float[] rightSamples, int sampleRate, int downsamplingFactor = 1)
    {
        ArgumentNullException.ThrowIfNull(leftSamples);
        ArgumentNullException.ThrowIfNull(rightSamples);

        if (leftSamples.Length == 0)
            throw new ArgumentException("Left channel samples cannot be empty", nameof(leftSamples));

        if (rightSamples.Length == 0)
            throw new ArgumentException("Right channel samples cannot be empty", nameof(rightSamples));

        if (leftSamples.Length != rightSamples.Length)
            throw new ArgumentException("Left and right channel samples must have the same length");

        var interleaved = new float[leftSamples.Length * 2];
        for (int i = 0; i < leftSamples.Length; i++)
        {
            interleaved[i * 2] = leftSamples[i];
            interleaved[i * 2 + 1] = rightSamples[i];
        }

        return new WaveformData(interleaved, channelCount: 2, sampleRate, downsamplingFactor);
    }

    /// <summary>
    /// Converts mono <see cref="WaveformData"/> to stereo by duplicating the mono channel.
    /// </summary>
    /// <param name="waveform">The mono waveform data to convert</param>
    /// <returns>A new <see cref="WaveformData"/> instance with stereo channels</returns>
    /// <exception cref="ArgumentNullException">Thrown if waveform is null</exception>
    /// <exception cref="InvalidOperationException">Thrown if waveform is already stereo</exception>
    public static WaveformData ToStereo(this WaveformData waveform)
    {
        ArgumentNullException.ThrowIfNull(waveform);

        if (waveform.ChannelCount != 1)
            throw new InvalidOperationException("Waveform must be mono (ChannelCount = 1) to convert to stereo");

        var samples = waveform.GetData();
        var stereoSamples = new float[samples.Length * 2];

        for (int i = 0; i < samples.Length; i++)
        {
            stereoSamples[i * 2] = samples[i];
            stereoSamples[i * 2 + 1] = samples[i];
        }

        return new WaveformData(
            stereoSamples,
            channelCount: 2,
            sampleRate: waveform.SampleRate,
            downsamplingFactor: waveform.DownsamplingFactor
        );
    }

    /// <summary>
    /// Gets the peak values for the specified channel.
    /// </summary>
    /// <param name="waveform">The waveform data</param>
    /// <param name="channelIndex">0 for left channel, 1 for right channel</param>
    /// <returns>An array of peak values for the specified channel, or null if not available</returns>
    /// <exception cref="ArgumentNullException">Thrown if waveform is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if channelIndex is not 0 or 1</exception>
    public static float[]? GetChannelPeaks(this WaveformData waveform, int channelIndex)
    {
        ArgumentNullException.ThrowIfNull(waveform);

        return channelIndex switch
        {
            0 => waveform.LeftChannelPeaks,
            1 => waveform.RightChannelPeaks,
            _ => throw new ArgumentOutOfRangeException(nameof(channelIndex), "Channel index must be 0 (left) or 1 (right)")
        };
    }

    /// <summary>
    /// Calculates the duration of the waveform in seconds.
    /// </summary>
    /// <param name="waveform">The waveform data</param>
    /// <returns>The duration in seconds, or 0 if invalid</returns>
    /// <exception cref="ArgumentNullException">Thrown if waveform is null</exception>
    public static double GetDurationSeconds(this WaveformData waveform)
    {
        ArgumentNullException.ThrowIfNull(waveform);

        if (waveform.SampleRate <= 0 || waveform.GetData().Length == 0)
            return 0.0;

        return (double)waveform.GetData().Length / waveform.SampleRate;
    }

    /// <summary>
    /// Gets the total number of samples across all channels.
    /// </summary>
    /// <param name="waveform">The waveform data</param>
    /// <returns>The total sample count</returns>
    /// <exception cref="ArgumentNullException">Thrown if waveform is null</exception>
    public static int GetTotalSampleCount(this WaveformData waveform)
    {
        ArgumentNullException.ThrowIfNull(waveform);

        return waveform.GetData().Length;
    }

    /// <summary>
    /// Gets the number of data points per channel.
    /// </summary>
    /// <param name="waveform">The waveform data</param>
    /// <returns>The number of data points per channel</returns>
    /// <exception cref="ArgumentNullException">Thrown if waveform is null</exception>
    public static int GetPointsPerChannel(this WaveformData waveform)
    {
        ArgumentNullException.ThrowIfNull(waveform);

        return waveform.GetData().Length / waveform.ChannelCount;
    }

    /// <summary>
    /// Creates a downsampled copy of the waveform with the specified factor.
    /// </summary>
    /// <param name="waveform">The waveform data to downsample</param>
    /// <param name="factor">Downsampling factor (must be greater than 1)</param>
    /// <returns>A new <see cref="WaveformData"/> instance with downsampled data</returns>
    /// <exception cref="ArgumentNullException">Thrown if waveform is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if factor is less than 2</exception>
    public static WaveformData Downsample(this WaveformData waveform, int factor)
    {
        ArgumentNullException.ThrowIfNull(waveform);

        if (factor < 2)
            throw new ArgumentOutOfRangeException(nameof(factor), "Factor must be 2 or greater");

        var samples = waveform.GetData();
        var downsampled = new float[samples.Length / factor];

        for (int i = 0; i < downsampled.Length; i++)
        {
            downsampled[i] = samples[i * factor];
        }

        return new WaveformData(
            downsampled,
            channelCount: waveform.ChannelCount,
            sampleRate: waveform.SampleRate / factor,
            downsamplingFactor: factor * waveform.DownsamplingFactor
        );
    }

    /// <summary>
    /// Gets the peak amplitude value from the waveform data.
    /// </summary>
    /// <param name="waveform">The waveform data</param>
    /// <returns>The peak amplitude value, or 0 if no data</returns>
    /// <exception cref="ArgumentNullException">Thrown if waveform is null</exception>
    public static float GetPeakAmplitude(this WaveformData waveform)
    {
        ArgumentNullException.ThrowIfNull(waveform);

        return waveform.GetData().Max(Math.Abs);
    }

    /// <summary>
    /// Gets the RMS (Root Mean Square) amplitude of the waveform.
    /// </summary>
    /// <param name="waveform">The waveform data</param>
    /// <returns>The RMS amplitude value</returns>
    /// <exception cref="ArgumentNullException">Thrown if waveform is null</exception>
    public static float GetRmsAmplitude(this WaveformData waveform)
    {
        ArgumentNullException.ThrowIfNull(waveform);

        var samples = waveform.GetData();
        if (samples.Length == 0)
            return 0f;

        double sumOfSquares = 0.0;
        foreach (var sample in samples)
        {
            sumOfSquares += sample * sample;
        }

        return (float)Math.Sqrt(sumOfSquares / samples.Length);
    }

    /// <summary>
    /// Gets the average amplitude of the waveform.
    /// </summary>
    /// <param name="waveform">The waveform data</param>
    /// <returns>The average amplitude value</returns>
    /// <exception cref="ArgumentNullException">Thrown if waveform is null</exception>
    public static float GetAverageAmplitude(this WaveformData waveform)
    {
        ArgumentNullException.ThrowIfNull(waveform);

        return waveform.GetData().Average();
    }

    /// <summary>
    /// Creates a normalized copy of the waveform.
    /// </summary>
    /// <param name="waveform">The waveform data to normalize</param>
    /// <returns>A new normalized <see cref="WaveformData"/> instance</returns>
    /// <exception cref="ArgumentNullException">Thrown if waveform is null</exception>
    public static WaveformData NormalizedCopy(this WaveformData waveform)
    {
        ArgumentNullException.ThrowIfNull(waveform);

        var normalizedSamples = (float[])waveform.GetData().Clone();
        var min = normalizedSamples.Min();
        var max = normalizedSamples.Max();
        var range = max - min;

        if (Math.Abs(range) > 0.0001f)
        {
            for (int i = 0; i < normalizedSamples.Length; i++)
            {
                normalizedSamples[i] = (normalizedSamples[i] - min) / range;
            }
        }

        var result = new WaveformData(
            normalizedSamples,
            channelCount: waveform.ChannelCount,
            sampleRate: waveform.SampleRate,
            downsamplingFactor: waveform.DownsamplingFactor
        );

        return result;
    }

    /// <summary>
    /// Splits stereo <see cref="WaveformData"/> into separate left and right channel arrays.
    /// </summary>
    /// <param name="waveform">The stereo waveform data to split</param>
    /// <returns>A tuple containing left and right channel samples, or null if not stereo</returns>
    /// <exception cref="ArgumentNullException">Thrown if waveform is null</exception>
    /// <exception cref="InvalidOperationException">Thrown if waveform is not stereo</exception>
    public static (float[] Left, float[] Right)? SplitStereoChannels(this WaveformData waveform)
    {
        ArgumentNullException.ThrowIfNull(waveform);

        if (waveform.ChannelCount != 2)
            throw new InvalidOperationException("Waveform must be stereo (ChannelCount = 2) to split channels");

        var samples = waveform.GetData();
        var left = new float[samples.Length / 2];
        var right = new float[samples.Length / 2];

        for (int i = 0; i < left.Length; i++)
        {
            left[i] = samples[i * 2];
            right[i] = samples[i * 2 + 1];
        }

        return (left, right);
    }

    /// <summary>
    /// Gets the sample at the specified index across all channels.
    /// </summary>
    /// <param name="waveform">The waveform data</param>
    /// <param name="index">The sample index</param>
    /// <returns>The sample value at the specified index</returns>
    /// <exception cref="ArgumentNullException">Thrown if waveform is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if index is out of range</exception>
    public static float GetSample(this WaveformData waveform, int index)
    {
        ArgumentNullException.ThrowIfNull(waveform);

        var samples = waveform.GetData();
        if (index < 0 || index >= samples.Length)
            throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {samples.Length - 1}");

        return samples[index];
    }

    /// <summary>
    /// Gets a range of samples from the waveform.
    /// </summary>
    /// <param name="waveform">The waveform data</param>
    /// <param name="startIndex">Starting index (inclusive)</param>
    /// <param name="count">Number of samples to retrieve</param>
    /// <returns>An array containing the requested samples</returns>
    /// <exception cref="ArgumentNullException">Thrown if waveform is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if startIndex or count are invalid</exception>
    public static float[] GetSampleRange(this WaveformData waveform, int startIndex, int count)
    {
        ArgumentNullException.ThrowIfNull(waveform);

        var samples = waveform.GetData();
        if (startIndex < 0 || startIndex >= samples.Length)
            throw new ArgumentOutOfRangeException(nameof(startIndex), $"Start index must be between 0 and {samples.Length - 1}");

        if (count < 0 || startIndex + count > samples.Length)
            throw new ArgumentOutOfRangeException(nameof(count), $"Count must be between 0 and {samples.Length - startIndex}");

        var result = new float[count];
        Array.Copy(samples, startIndex, result, 0, count);
        return result;
    }
}