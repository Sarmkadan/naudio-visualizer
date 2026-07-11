#nullable enable

// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =====================================================================

using System;
using NAudioVisualizer.Domain.Models;

namespace NAudioVisualizer.Events;

/// <summary>
/// Provides useful extension methods for <see cref="AudioCaptureStartedEvent"/> to
/// enhance the event with additional functionality for audio capture scenarios.
/// </summary>
public static class AudioCaptureStartedEventExtensions
{
    /// <summary>
    /// Creates a human-readable description of the audio capture session.
    /// </summary>
    /// <param name="event">The audio capture started event.</param>
    /// <returns>A formatted string describing the capture session.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="event"/> is <see langword="null"/></exception>
    public static string GetSessionDescription(this AudioCaptureStartedEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event);

        return $"Audio Capture Started [Device: {@event.DeviceId}, Sample Rate: {@event.SampleRate}Hz, Channels: {@event.ChannelCount}] at {@event.StartTime:yyyy-MM-dd HH:mm:ss}";
    }

    /// <summary>
    /// Calculates the estimated memory usage in bytes for the audio buffer based on
    /// the capture parameters.
    /// </summary>
    /// <param name="event">The audio capture started event.</param>
    /// <param name="seconds">Number of seconds to estimate for. Must be non-negative.</param>
    /// <returns>Estimated memory usage in bytes (16-bit samples).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="event"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="seconds"/> is negative.</exception>
    public static long EstimateMemoryUsage(this AudioCaptureStartedEvent @event, int seconds)
    {
        ArgumentNullException.ThrowIfNull(@event);
        ArgumentOutOfRangeException.ThrowIfNegative(seconds);

        // 16-bit samples, 2 bytes per sample
        const int bytesPerSample = 2;
        long samplesPerSecond = @event.SampleRate * @event.ChannelCount;
        long totalSamples = samplesPerSecond * seconds;

        return totalSamples * bytesPerSample;
    }

    /// <summary>
    /// Determines if the capture settings match a target configuration.
    /// </summary>
    /// <param name="event">The audio capture started event.</param>
    /// <param name="targetSampleRate">The target sample rate to match.</param>
    /// <param name="targetChannelCount">The target channel count to match.</param>
    /// <returns>True if the settings match; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="event"/> is <see langword="null"/></exception>
    public static bool MatchesConfiguration(this AudioCaptureStartedEvent @event, int targetSampleRate, int targetChannelCount)
    {
        ArgumentNullException.ThrowIfNull(@event);

        return @event.SampleRate == targetSampleRate && @event.ChannelCount == targetChannelCount;
    }

    /// <summary>
    /// Gets a display-friendly label for the audio device.
    /// </summary>
    /// <param name="event">The audio capture started event.</param>
    /// <returns>A formatted device label.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="event"/> is <see langword="null"/></exception>
    public static string GetDeviceLabel(this AudioCaptureStartedEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event);

        return $"Device {@event.DeviceId} - {@event.SampleRate}Hz/{@event.ChannelCount}ch";
    }
}
