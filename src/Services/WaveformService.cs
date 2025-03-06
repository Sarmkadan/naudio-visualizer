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
/// <example>
/// <code>
/// var service = new WaveformService();
/// var waveform = service.GenerateWaveform(audioFrame, downsamplingFactor: 4);
/// float[] peaks = service.CalculatePeakValues(waveform.GetData(), peakCount: 512);
/// </code>
/// </example>
public class WaveformService
{
    /// <summary>
    /// Generates waveform visualization from an audio frame.
    /// </summary>
    /// <param name="frame">
    /// The captured audio frame containing interleaved PCM samples in the [-1, 1] range.
    /// </param>
    /// <param name="downsamplingFactor">
    /// Ratio by which the sample count is reduced for rendering performance.
    /// For example, a value of 4 averages every 4 input samples into one output sample.
    /// Valid range: 1 (no downsampling) to any positive integer. Defaults to
    /// <see cref="AudioConstants.DEFAULT_WAVEFORM_DOWNSAMPLING"/> (4).
    /// </param>
    /// <returns>A <see cref="WaveformData"/> instance ready for rendering.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="frame"/> is null.</exception>
    /// <exception cref="VisualizationException">
    /// Thrown when <paramref name="frame"/> fails validation or sample processing fails.
    /// </exception>
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
        catch (InvalidOperationException ex)
        {
            throw new VisualizationException(
                $"Failed to generate waveform: {ex.Message}",
                "Waveform",
                ex
            );
        }
    }

    /// <summary>
    /// Downsamples audio samples by averaging groups of <paramref name="factor"/> consecutive
    /// samples, reducing the array length by approximately that factor.
    /// </summary>
    /// <param name="samples">Source PCM samples in the [-1, 1] range.</param>
    /// <param name="factor">
    /// Number of input samples to average into each output sample.
    /// A value of 1 returns a clone of the input unchanged.
    /// </param>
    /// <returns>A new array whose length is approximately <c>samples.Length / factor</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="samples"/> is null.</exception>
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
    /// Applies normalization to waveform data, scaling all amplitude values to the [-1, 1] range.
    /// </summary>
    /// <param name="waveform">The waveform data to normalize in place.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveform"/> is null.</exception>
    public void NormalizeWaveform(WaveformData waveform)
    {
        if (waveform is null)
            throw new ArgumentNullException(nameof(waveform));

        waveform.Normalize();
    }

    /// <summary>
    /// Calculates the peak absolute amplitude within each of <paramref name="peakCount"/>
    /// equally-sized segments of <paramref name="samples"/>.
    /// Useful for drawing a compact bar representation of a long waveform.
    /// </summary>
    /// <param name="samples">Input PCM samples.</param>
    /// <param name="peakCount">
    /// Number of output peak values (i.e. the number of bars in the rendered waveform).
    /// Must be greater than zero.
    /// </param>
    /// <returns>
    /// An array of <paramref name="peakCount"/> non-negative peak values,
    /// or a clone of <paramref name="samples"/> when it is shorter than
    /// <paramref name="peakCount"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="samples"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="peakCount"/> is not positive.</exception>
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
    /// Applies a moving-average smoothing filter to reduce high-frequency visual noise.
    /// </summary>
    /// <param name="samples">Input PCM samples to smooth.</param>
    /// <param name="windowSize">
    /// Number of samples in the averaging window. Must be ≥ 1.
    /// Larger values produce a smoother but more latent result. Defaults to 3.
    /// </param>
    /// <returns>A new array of the same length with smoothed values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="samples"/> is null.</exception>
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
    /// Calculates the RMS (Root Mean Square) energy level for each of
    /// <paramref name="frameCount"/> equally-sized segments of <paramref name="samples"/>.
    /// RMS energy correlates with perceived loudness and is suitable for loudness meters.
    /// </summary>
    /// <param name="samples">Input PCM samples in the [-1, 1] range.</param>
    /// <param name="frameCount">
    /// Number of segments to divide the samples into. Must be greater than zero.
    /// </param>
    /// <returns>
    /// An array of <paramref name="frameCount"/> non-negative RMS values in the [0, 1] range.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="samples"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="frameCount"/> is not positive.</exception>
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
    /// Counts zero-crossings in <paramref name="samples"/>, i.e. positions where the signal
    /// transitions from positive to negative or vice versa. The zero-crossing rate is a
    /// simple proxy for the fundamental frequency content of the signal.
    /// </summary>
    /// <param name="samples">Input PCM samples. Requires at least two elements for any crossings to be detected.</param>
    /// <returns>Number of zero crossings found, or 0 when fewer than two samples are provided.</returns>
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
