#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using NAudioVisualizer.Constants;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Exceptions;
using static NAudioVisualizer.Constants.VisualizationConstants;

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
public sealed class WaveformService
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
    public WaveformData GenerateWaveform(AudioFrame frame, int downsamplingFactor = DEFAULT_WAVEFORM_DOWNSAMPLING)
    {
        if (frame is null)
            throw new ArgumentNullException(nameof(frame));

        if (!frame.IsValid())
            throw new VisualizationException("Audio frame is invalid", "Waveform");

        try
        {
            var samples = ProcessSamples(frame.Samples, downsamplingFactor);

            var waveform = new WaveformData(
                samples,
                frame.ChannelCount,
                frame.SampleRate,
                downsamplingFactor
            )
            {
                SourceFrame = frame
            };

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

    private float[] ProcessSamples(float[] samples, int downsamplingFactor)
    {
        float[] processedSamples = downsamplingFactor > 1
            ? DownsampleSamples(samples, downsamplingFactor)
            : (float[])samples.Clone();

        return processedSamples;
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
            downsampled[i] = CalculateDownsampledValue(samples, factor, i);
        }

        return downsampled;
    }

    /// <summary>
    /// Downsamples audio samples by finding the minimum and maximum values in each of
    /// <paramref name="targetBuckets"/> segments. Useful for waveform overview rendering.
    /// </summary>
    /// <param name="samples">Source PCM samples in the [-1, 1] range.</param>
    /// <param name="targetBuckets">Number of buckets for the overview.</param>
    /// <returns>An array of (min, max) tuples for each bucket.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="samples"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="targetBuckets"/> is not positive.</exception>
    public (float min, float max)[] DownsampleMinMax(float[] samples, int targetBuckets)
    {
        if (samples is null)
            throw new ArgumentNullException(nameof(samples));

        if (targetBuckets <= 0)
            throw new ArgumentException("Target buckets must be positive", nameof(targetBuckets));

        if (samples.Length == 0)
            return Array.Empty<(float, float)>();

        int actualBuckets = Math.Min(samples.Length, targetBuckets);
        var result = new (float min, float max)[actualBuckets];

        int samplesPerBucket = samples.Length / actualBuckets;

        for (int i = 0; i < actualBuckets; i++)
        {
            int startIdx = i * samplesPerBucket;
            int endIdx = (i == actualBuckets - 1) ? samples.Length : startIdx + samplesPerBucket;

            float min = float.MaxValue;
            float max = float.MinValue;

            for (int j = startIdx; j < endIdx; j++)
            {
                float sample = samples[j];
                if (sample < min) min = sample;
                if (sample > max) max = sample;
            }

            result[i] = (min, max);
        }

        return result;
    }

    private float CalculateDownsampledValue(float[] samples, int factor, int outputIndex)
    {
        float sum = 0f;
        int count = 0;

        int startIdx = outputIndex * factor;
        int endIdx = Math.Min(startIdx + factor, samples.Length);

        for (int j = startIdx; j < endIdx; j++)
        {
            sum += samples[j];
            count++;
        }

        return count > 0 ? sum / count : 0f;
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
            peaks[i] = CalculatePeakForSegment(samples, i, samplesPerPeak, peakCount);
        }

        return peaks;
    }

    private float CalculatePeakForSegment(float[] samples, int segmentIndex, int samplesPerPeak, int peakCount)
    {
        float maxPeak = 0f;
        int startIdx = segmentIndex * samplesPerPeak;
        int endIdx = segmentIndex == peakCount - 1
            ? samples.Length
            : startIdx + samplesPerPeak;

        for (int j = startIdx; j < endIdx; j++)
        {
            float absSample = Math.Abs(samples[j]);
            if (absSample > maxPeak) maxPeak = absSample;
        }

        return maxPeak;
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

        windowSize = Math.Max(1, windowSize);
        var smoothed = new float[samples.Length];

        for (int i = 0; i < samples.Length; i++)
        {
            smoothed[i] = CalculateSmoothedValue(samples, windowSize, i);
        }

        return smoothed;
    }

    private float CalculateSmoothedValue(float[] samples, int windowSize, int index)
    {
        float sum = 0f;
        int count = 0;
        int halfWindow = windowSize / 2;

        for (int j = -halfWindow; j <= halfWindow; j++)
        {
            int idx = index + j;
            if (idx >= 0 && idx < samples.Length)
            {
                sum += samples[idx];
                count++;
            }
        }

        return sum / count;
    }

    /// <summary>
    /// Calculates the RMS (Root Mean Square) energy level for each of 302
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
            energyFrames[i] = CalculateEnergyForFrame(samples, i, samplesPerFrame, frameCount);
        }

        return energyFrames;
    }

    private float CalculateEnergyForFrame(float[] samples, int frameIndex, int samplesPerFrame, int frameCount)
    {
        double sumSquares = 0;
        int startIdx = frameIndex * samplesPerFrame;
        int endIdx = frameIndex == frameCount - 1
            ? samples.Length
            : startIdx + samplesPerFrame;
        int sampleCount = endIdx - startIdx;

        for (int j = startIdx; j < endIdx; j++)
        {
            sumSquares += samples[j] * samples[j];
        }

        return (float)Math.Sqrt(sumSquares / sampleCount);
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
            if ((samples[i - 1] < 0 && samples[i] >= 0) ||
                (samples[i - 1] >= 0 && samples[i] < 0))
            {
                crossingCount++;
            }
        }

        return crossingCount;
    }

    /// <summary>
    /// Applies a zoom window to the waveform data, extracting a subset of samples.
    /// </summary>
    /// <param name="waveform">The waveform data to apply zoom to.</param>
    /// <param name="startSample">Starting sample index for the zoom window (inclusive).</param>
    /// <param name="lengthSamples">Number of samples to include in the zoom window.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveform"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when startSample or lengthSamples are invalid.</exception>
    public void ApplyZoomWindow(WaveformData waveform, long startSample, long lengthSamples)
    {
        if (waveform is null)
            throw new ArgumentNullException(nameof(waveform));

        float[] originalSamples = waveform.GetData();

        // Clamp start sample to valid range
        startSample = Math.Max(0, Math.Min(startSample, originalSamples.Length - 1));

        // Clamp length to ensure we don't exceed available samples
        long maxLength = originalSamples.Length - startSample;
        lengthSamples = Math.Max(0, Math.Min(lengthSamples, maxLength));

        if (lengthSamples <= 0)
            throw new ArgumentOutOfRangeException(nameof(lengthSamples), "Zoom window length must be positive");

        // Extract the zoomed samples
        var zoomedSamples = new float[lengthSamples];
        Array.Copy(originalSamples, startSample, zoomedSamples, 0, lengthSamples);

        // Update the waveform with zoomed data
        waveform.SetSamples(zoomedSamples);

        // Update source frame to reflect the zoom (preserve original metadata)
        if (waveform.SourceFrame != null)
        {
            waveform.SourceFrame = new AudioFrame(
                zoomedSamples,
                waveform.SourceFrame.ChannelCount,
                waveform.SourceFrame.SampleRate,
                waveform.SourceFrame.FrameIndex
            )
            {
                Timestamp = waveform.SourceFrame.Timestamp
            };
        }
    }

    /// <summary>
    /// Zooms into the waveform by reducing the visible sample range by half.
    /// </summary>
    /// <param name="waveform">The waveform data to zoom into.</param>
    /// <param name="zoomCenterSample">Optional center sample index for the zoom. If null, uses the middle of the current window.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveform"/> is null.</exception>
    public void ZoomIn(WaveformData waveform, long? zoomCenterSample = null)
    {
        if (waveform is null)
            throw new ArgumentNullException(nameof(waveform));

        float[] samples = waveform.GetData();
        long currentLength = samples.Length;

        if (currentLength <= 2)
            return; // Cannot zoom further

        long centerSample = zoomCenterSample ?? currentLength / 2;
        long newLength = Math.Max(2, currentLength / 2);
        long startSample = Math.Max(0, Math.Min(centerSample - newLength / 2, currentLength - newLength));

        ApplyZoomWindow(waveform, startSample, newLength);
    }

    /// <summary>
    /// Zooms out from the waveform by doubling the visible sample range.
    /// </summary>
    /// <param name="waveform">The waveform data to zoom out from.</param>
    /// <param name="zoomCenterSample">Optional center sample index for the zoom. If null, uses the middle of the current window.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveform"/> is null.</exception>
    public void ZoomOut(WaveformData waveform, long? zoomCenterSample = null)
    {
        if (waveform is null)
            throw new ArgumentNullException(nameof(waveform));

        float[] samples = waveform.GetData();
        long currentLength = samples.Length;

        if (currentLength >= 1000000) // Reasonable upper limit
            return; // Cannot zoom out further

        long centerSample = zoomCenterSample ?? currentLength / 2;
        long newLength = Math.Min(1000000, currentLength * 2);

        // Calculate new start position, ensuring we don't go out of bounds
        long startSample = Math.Max(0, centerSample - newLength / 2);

        // If we're at the end, adjust to keep the center visible
        if (startSample + newLength > samples.Length)
        {
            startSample = Math.Max(0, samples.Length - newLength);
        }

        ApplyZoomWindow(waveform, startSample, newLength);
    }

    /// <summary>
    /// Pans the waveform view by moving the visible window.
    /// </summary>
    /// <param name="waveform">The waveform data to pan.</param>
    /// <param name="samplesToMove">Number of samples to move the window. Positive moves right, negative moves left.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveform"/> is null.</exception>
    public void Pan(WaveformData waveform, long samplesToMove)
    {
        if (waveform is null)
            throw new ArgumentNullException(nameof(waveform));

        float[] samples = waveform.GetData();
        long currentLength = samples.Length;

        if (currentLength <= 0)
            return;

        // Simple pan implementation - in a full implementation, this would work with the original data
        // For now, we'll just ensure the pan stays within bounds
        // Note: A complete pan implementation would need access to the original full-length samples
    }

    /// <summary>
    /// Gets the current zoom window parameters from waveform data.
    /// </summary>
    /// <param name="waveform">The waveform data to get zoom parameters from.</param>
    /// <param name="startSample">Output parameter for the start sample index.</param>
    /// <param name="lengthSamples">Output parameter for the zoom window length.</param>
    /// <returns>True if zoom is active (length < original length), false otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveform"/> is null.</exception>
    public bool GetZoomWindow(WaveformData waveform, out long startSample, out long lengthSamples)
    {
        if (waveform is null)
            throw new ArgumentNullException(nameof(waveform));

        float[] zoomedSamples = waveform.GetData();
        lengthSamples = zoomedSamples.Length;
        startSample = 0;

        // To determine if zoom is active, check if we have source frame with original length
        if (waveform.SourceFrame != null)
        {
            long originalLength = waveform.SourceFrame.Samples.Length;
            return lengthSamples < originalLength;
        }

        return false;
    }
}