// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Represents a single frame of audio data with timing and sample information.
/// </summary>
public class AudioFrame
{
    /// <summary>
    /// Unique identifier for this audio frame.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Raw audio samples as floats, normalized between -1.0 and 1.0.
    /// </summary>
    public float[] Samples { get; private set; } = [];

    /// <summary>
    /// Number of channels in this audio frame (mono=1, stereo=2).
    /// </summary>
    public int ChannelCount { get; set; }

    /// <summary>
    /// Sample rate of the audio (e.g., 44100 Hz, 48000 Hz).
    /// </summary>
    public int SampleRate { get; set; }

    /// <summary>
    /// Timestamp when this frame was captured.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Frame index in the audio stream sequence.
    /// </summary>
    public long FrameIndex { get; set; }

    /// <summary>
    /// Duration of this frame in seconds.
    /// </summary>
    public double DurationSeconds { get; set; }

    /// <summary>
    /// Peak amplitude value in this frame (-1.0 to 1.0).
    /// </summary>
    public float PeakAmplitude { get; private set; }

    /// <summary>
    /// RMS (Root Mean Square) energy value for this frame.
    /// </summary>
    public float RmsEnergy { get; private set; }

    /// <summary>
    /// Initializes a new audio frame with the specified samples and metadata.
    /// </summary>
    public AudioFrame(float[] samples, int channelCount, int sampleRate, long frameIndex)
    {
        Samples = samples ?? throw new ArgumentNullException(nameof(samples));
        ChannelCount = channelCount;
        SampleRate = sampleRate;
        FrameIndex = frameIndex;
        DurationSeconds = (double)samples.Length / sampleRate;

        CalculateFrameMetrics();
    }

    /// <summary>
    /// Calculates peak amplitude and RMS energy for the frame.
    /// </summary>
    private void CalculateFrameMetrics()
    {
        if (Samples.Length == 0)
        {
            PeakAmplitude = 0f;
            RmsEnergy = 0f;
            return;
        }

        float maxAmplitude = 0f;
        double sumOfSquares = 0;

        foreach (var sample in Samples)
        {
            float absSample = Math.Abs(sample);
            if (absSample > maxAmplitude)
            {
                maxAmplitude = absSample;
            }
            sumOfSquares += sample * sample;
        }

        PeakAmplitude = maxAmplitude;
        RmsEnergy = (float)Math.Sqrt(sumOfSquares / Samples.Length);
    }

    /// <summary>
    /// Gets the audio data for a specific channel.
    /// </summary>
    public float[] GetChannelData(int channelIndex)
    {
        if (channelIndex < 0 || channelIndex >= ChannelCount)
            throw new ArgumentOutOfRangeException(nameof(channelIndex));

        var channelData = new float[Samples.Length / ChannelCount];
        for (int i = 0; i < channelData.Length; i++)
        {
            channelData[i] = Samples[i * ChannelCount + channelIndex];
        }
        return channelData;
    }

    /// <summary>
    /// Validates that the frame data is consistent and valid.
    /// </summary>
    public bool IsValid()
    {
        return Samples.Length > 0 &&
               ChannelCount > 0 &&
               SampleRate > 0 &&
               !float.IsNaN(PeakAmplitude) &&
               !float.IsNaN(RmsEnergy);
    }
}
