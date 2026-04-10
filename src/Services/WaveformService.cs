#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using NAudioVisualizer.Constants;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Exceptions;

namespace NAudioVisualizer.Services;

/// <summary>
/// Service for generating waveform visualization data from audio frames.
/// </summary>
public class WaveformService
{
    /// <summary>
    /// Generates waveform visualization from an audio frame.
    /// </summary>
    public WaveformData GenerateWaveform(AudioFrame frame, int downsamplingFactor = AudioConstants.DEFAULT_WAVEFORM_DOWNSAMPLING)
    {
        if (frame is null)
            throw new ArgumentNullException(nameof(frame));

        if (!frame.IsValid())
            throw new VisualizationException("Audio frame is invalid", "Waveform");

        try
        {
            // Get samples and optionally downsample
            var samples = frame.Samples;
            if (downsamplingFactor > 1)
            {
                samples = DownsampleSamples(samples, downsamplingFactor);
            }

            var waveform = new WaveformData(
                samples,
                frame.ChannelCount,
                frame.SampleRate,
                downsamplingFactor
            )
            {
                SourceFrame = frame
            };

            // Generate stereo channel data if stereo
            if (frame.ChannelCount == 2)
            {
                waveform.CalculateStereoChannels();
            }

            return waveform;
        }
        catch (Exception ex)
        {
            throw new VisualizationException(
                $"Failed to generate waveform: {ex.Message}",
                "Waveform",
                ex
            );
        }
    }

    /// <summary>
    /// Downsamples audio samples by a given factor.
    /// </summary>
    public float[] DownsampleSamples(float[] samples, int factor)
    {
        if (samples is null)
            throw new ArgumentNullException(nameof(samples));

        if (factor <= 1)
            return (float[])samples.Clone();

        int downsampledLength = (samples.Length + factor - 1) / factor;
        var downsampled = new float[downsampledLength];

        for (int i = 0; i < downsampledLength; i++)
        {
            // Average nearby samples for smoother downsampling
            float sum = 0f;
            int count = 0;

            for (int j = 0; j < factor; j++)
            {
                int idx = i * factor + j;
                if (idx < samples.Length)
                {
                    sum += samples[idx];
                    count++;
                }
            }

            downsampled[i] = count > 0 ? sum / count : 0f;
        }

        return downsampled;
    }

    /// <summary>
    /// Applies normalization to waveform data.
    /// </summary>
    public void NormalizeWaveform(WaveformData waveform)
    {
        if (waveform is null)
            throw new ArgumentNullException(nameof(waveform));

        waveform.Normalize();
    }

    /// <summary>
    /// Calculates peak values for each section of the waveform.
    /// </summary>
    public float[] CalculatePeakValues(float[] samples, int peakCount)
    {
        if (samples is null)
            throw new ArgumentNullException(nameof(samples));

        if (peakCount <= 0)
            throw new ArgumentException("Peak count must be positive", nameof(peakCount));

        if (samples.Length <= peakCount)
            return (float[])samples.Clone();

        var peaks = new float[peakCount];
        int samplesPerPeak = samples.Length / peakCount;

        for (int i = 0; i < peakCount; i++)
        {
            float maxPeak = 0f;
            int startIdx = i * samplesPerPeak;
            int endIdx = i == peakCount - 1 ? samples.Length : (i + 1) * samplesPerPeak;

            for (int j = startIdx; j < endIdx; j++)
            {
                float absSample = Math.Abs(samples[j]);
                if (absSample > maxPeak)
                    maxPeak = absSample;
            }

            peaks[i] = maxPeak;
        }

        return peaks;
    }

    /// <summary>
    /// Applies smoothing filter to reduce visual noise.
    /// </summary>
    public float[] ApplySmoothingFilter(float[] samples, int windowSize = 3)
    {
        if (samples is null)
            throw new ArgumentNullException(nameof(samples));

        if (windowSize < 1)
            windowSize = 1;

        var smoothed = new float[samples.Length];
        int halfWindow = windowSize / 2;

        for (int i = 0; i < samples.Length; i++)
        {
            float sum = 0f;
            int count = 0;

            for (int j = -halfWindow; j <= halfWindow; j++)
            {
                int idx = i + j;
                if (idx >= 0 && idx < samples.Length)
                {
                    sum += samples[idx];
                    count++;
                }
            }

            smoothed[i] = sum / count;
        }

        return smoothed;
    }

    /// <summary>
    /// Calculates RMS energy per frame segment.
    /// </summary>
    public float[] CalculateFrameEnergy(float[] samples, int frameCount)
    {
        if (samples is null)
            throw new ArgumentNullException(nameof(samples));

        if (frameCount <= 0)
            throw new ArgumentException("Frame count must be positive", nameof(frameCount));

        var energyFrames = new float[frameCount];
        int samplesPerFrame = samples.Length / frameCount;

        for (int i = 0; i < frameCount; i++)
        {
            double sumSquares = 0;
            int startIdx = i * samplesPerFrame;
            int endIdx = i == frameCount - 1 ? samples.Length : (i + 1) * samplesPerFrame;
            int sampleCount = endIdx - startIdx;

            for (int j = startIdx; j < endIdx; j++)
            {
                sumSquares += samples[j] * samples[j];
            }

            energyFrames[i] = (float)Math.Sqrt(sumSquares / sampleCount);
        }

        return energyFrames;
    }

    /// <summary>
    /// Detects zero crossings in audio signal (indicates frequency content).
    /// </summary>
    public int CountZeroCrossings(float[] samples)
    {
        if (samples is null || samples.Length < 2)
            return 0;

        int crossingCount = 0;

        for (int i = 1; i < samples.Length; i++)
        {
            // Check if sign changed from previous to current sample
            if ((samples[i - 1] < 0 && samples[i] >= 0) ||
                (samples[i - 1] >= 0 && samples[i] < 0))
            {
                crossingCount++;
            }
        }

        return crossingCount;
    }
}
