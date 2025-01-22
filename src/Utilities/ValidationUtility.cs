#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace NAudioVisualizer.Utilities;

/// <summary>
/// Provides utility methods for common validation scenarios.
/// Centralizes validation logic to ensure consistency across the application.
/// </summary>
public static class ValidationUtility
{
    /// <summary>
    /// Validates that a sample rate is within acceptable audio range.
    /// Valid sample rates are typically 8000 Hz to 192000 Hz.
    /// </summary>
    public static bool ValidateSampleRate(int sampleRate)
    {
        return sampleRate >= 8000 && sampleRate <= 192000 && sampleRate % 100 == 0;
    }

    /// <summary>
    /// Validates that an FFT size is a power of 2 and within range.
    /// </summary>
    public static bool ValidateFftSize(int fftSize)
    {
        // FFT size must be power of 2 and between 256 and 16384
        if (fftSize < 256 || fftSize > 16384)
            return false;

        // Check if power of 2
        return (fftSize & (fftSize - 1)) == 0;
    }

    /// <summary>
    /// Validates that channel count is 1 (mono) or 2 (stereo).
    /// </summary>
    public static bool ValidateChannelCount(int channels)
    {
        return channels == 1 || channels == 2;
    }

    /// <summary>
    /// Validates that FPS is within a reasonable range.
    /// </summary>
    public static bool ValidateFps(int fps)
    {
        return fps >= 15 && fps <= 240;
    }

    /// <summary>
    /// Validates that a frequency value is within the human hearing range.
    /// </summary>
    public static bool ValidateFrequency(float frequency)
    {
        return frequency >= 20f && frequency <= 20000f;
    }

    /// <summary>
    /// Validates that audio data array is not null or empty.
    /// </summary>
    public static bool ValidateAudioData(float[]? data)
    {
        return data is not null && data.Length > 0;
    }

    /// <summary>
    /// Validates that amplitude values are within -1.0 to 1.0 range.
    /// </summary>
    public static bool ValidateAmplitude(float amplitude)
    {
        return amplitude >= -1.0f && amplitude <= 1.0f;
    }

    /// <summary>
    /// Validates that a file path is not null or whitespace.
    /// </summary>
    public static bool ValidateFilePath(string? path)
    {
        return !string.IsNullOrWhiteSpace(path);
    }

    /// <summary>
    /// Validates that a duration in seconds is positive.
    /// </summary>
    public static bool ValidateDuration(float durationSeconds)
    {
        return durationSeconds > 0f && durationSeconds < 3600f; // Up to 1 hour
    }

    /// <summary>
    /// Validates that a device index is non-negative.
    /// </summary>
    public static bool ValidateDeviceIndex(int deviceIndex)
    {
        return deviceIndex >= 0;
    }

    /// <summary>
    /// Validates a time value in milliseconds.
    /// </summary>
    public static bool ValidateTimeInMs(int timeMs)
    {
        return timeMs > 0 && timeMs < int.MaxValue;
    }

    /// <summary>
    /// Validates a normalization factor (should be positive and not zero).
    /// </summary>
    public static bool ValidateNormalization(float factor)
    {
        return !float.IsNaN(factor) && !float.IsInfinity(factor) && factor > 0f;
    }

    /// <summary>
    /// Validates that all required parameters are provided.
    /// </summary>
    public static bool ValidateRequiredParameters(params object?[] parameters)
    {
        return parameters.All(p => p is not null);
    }

    /// <summary>
    /// Validates that a collection is not null or empty.
    /// </summary>
    public static bool ValidateCollection<T>(IEnumerable<T>? collection)
    {
        return collection is not null && collection.Any();
    }

    /// <summary>
    /// Throws an ArgumentException if a value doesn't meet criteria.
    /// </summary>
    public static void ThrowIfInvalid(bool isValid, string parameterName, string reason)
    {
        if (!isValid)
            throw new ArgumentException($"{parameterName}: {reason}", parameterName);
    }

    /// <summary>
    /// Throws an ArgumentNullException if a value is null.
    /// </summary>
    public static void ThrowIfNull(object? value, string parameterName)
    {
        if (value is null)
            throw new ArgumentNullException(parameterName);
    }

    /// <summary>
    /// Throws an ArgumentException if a string is null or whitespace.
    /// </summary>
    public static void ThrowIfNullOrWhitespace(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", parameterName);
    }

    /// <summary>
    /// Throws an ArgumentOutOfRangeException if a value is outside the specified range.
    /// </summary>
    public static void ThrowIfOutOfRange(int value, int min, int max, string parameterName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be between {min} and {max}.");
    }

    /// <summary>
    /// Throws an ArgumentOutOfRangeException if a float value is outside range.
    /// </summary>
    public static void ThrowIfOutOfRange(float value, float min, float max, string parameterName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be between {min} and {max}.");
    }
}
