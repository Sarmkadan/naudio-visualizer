using System;
using System.Collections.Generic;
using System.Globalization;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides extension methods for <see cref="AudioBuffer"/> to enable common audio processing scenarios.
/// </summary>
public static class AudioBufferExtensions
{
    /// <summary>
    /// Copies samples from the buffer to a target array, handling channel separation and interleaving.
    /// </summary>
    /// <param name="buffer">The audio buffer.</param>
    /// <param name="target">The target array to copy samples to.</param>
    /// <param name="targetChannel">The target channel index (0-based).</param>
    /// <param name="sourceChannel">The source channel index (0-based).</param>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> or <paramref name="target"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Channel indices are invalid.</exception>
    public static void CopyToChannel(
        this AudioBuffer buffer,
        float[] target,
        int targetChannel,
        int sourceChannel)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(target);

        if (targetChannel < 0 || targetChannel >= buffer.ChannelCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(targetChannel),
                $"Target channel {targetChannel} is out of range [0, {buffer.ChannelCount - 1}]");
        }

        if (sourceChannel < 0 || sourceChannel >= buffer.ChannelCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sourceChannel),
                $"Source channel {sourceChannel} is out of range [0, {buffer.ChannelCount - 1}]");
        }

        lock (buffer.GetLock())
        {
            // Calculate stride between channels
            int stride = buffer.ChannelCount;
            int availableSamples = buffer.Count;

            // Copy samples for the specified channel
            for (int i = 0; i < availableSamples; i++)
            {
                int sourceIndex = i * stride + sourceChannel;
                int targetIndex = i * stride + targetChannel;

                if (targetIndex < target.Length)
                {
                    target[targetIndex] = buffer.Peek(availableSamples)[sourceIndex];
                }
            }
        }
    }

    /// <summary>
    /// Converts the audio buffer to a normalized float array (values in range [-1, 1]).
    /// </summary>
    /// <param name="buffer">The audio buffer.</param>
    /// <returns>An array containing normalized audio samples.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
    public static float[] ToNormalizedArray(this AudioBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        lock (buffer.GetLock())
        {
            float[] samples = buffer.GetAll();
            float[] normalized = new float[samples.Length];

            // Find maximum absolute value for normalization
            float max = 0f;
            for (int i = 0; i < samples.Length; i++)
            {
                float abs = Math.Abs(samples[i]);
                if (abs > max)
                {
                    max = abs;
                }
            }

            // Normalize to [-1, 1] range
            float scale = max > float.Epsilon ? 1f / max : 1f;
            for (int i = 0; i < samples.Length; i++)
            {
                normalized[i] = samples[i] * scale;
            }

            return normalized;
        }
    }

    /// <summary>
    /// Creates a new <see cref="AudioBuffer"/> with the same configuration but empty content.
    /// </summary>
    /// <param name="buffer">The audio buffer to clone.</param>
    /// <returns>A new empty buffer with identical capacity, sample rate, and channel count.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
    public static AudioBuffer CloneEmpty(this AudioBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        return new AudioBuffer(buffer.Capacity, buffer.SampleRate, buffer.ChannelCount);
    }

    /// <summary>
    /// Gets the buffer fill percentage as a string formatted for display.
    /// </summary>
    /// <param name="buffer">The audio buffer.</param>
    /// <param name="format">The format string for the percentage (default: "P2").</param>
    /// <returns>A formatted string representing the fill percentage.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
    public static string GetFillPercentageString(
        this AudioBuffer buffer,
        string format = "P2")
    {
        ArgumentNullException.ThrowIfNull(buffer);

        lock (buffer.GetLock())
        {
            var stats = buffer.GetStats();
            return stats.FillPercentage.ToString(format, CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Gets the internal lock object for synchronized access.
    /// </summary>
    /// <param name="buffer">The audio buffer.</param>
    /// <returns>The lock object used for thread synchronization.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
    internal static object GetLock(this AudioBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        return buffer.GetType().GetField("_lock", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(buffer);
    }
}