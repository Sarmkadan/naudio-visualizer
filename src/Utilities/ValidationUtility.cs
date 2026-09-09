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
    /// <param name="sampleRate">The sample rate, in hertz, to validate.</param>
    /// <returns><see langword="true"/> if the sample rate is valid; otherwise, <see langword="false"/>.</returns>
    public static bool ValidateSampleRate(int sampleRate)
    {
        return sampleRate >= 8000 && sampleRate <= 192000 && sampleRate % 100 == 0;
    }

    /// <summary>
    /// Validates that an FFT size is a power of 2 and within range.
    /// </summary>
    /// <param name="fftSize">The FFT size to validate.</param>
    /// <returns><see langword="true"/> if the FFT size is valid; otherwise, <see langword="false"/>.</returns>
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
    /// <param name="channels">The channel count to validate.</param>
    /// <returns><see langword="true"/> if the channel count is valid; otherwise, <see langword="false"/>.</returns>
    public static bool ValidateChannelCount(int channels)
    {
        return channels == 1 || channels == 2;
    }

    /// <summary>
    /// Validates that FPS is within a reasonable range.
    /// </summary>
    /// <param name="fps">The frames-per-second value to validate.</param>
    /// <returns><see langword="true"/> if the FPS value is valid; otherwise, <see langword="false"/>.</returns>
    public static bool ValidateFps(int fps)
    {
        return fps >= 15 && fps <= 240;
    }

    /// <summary>
    /// Validates that a frequency value is within the human hearing range.
    /// </summary>
    /// <param name="frequency">The frequency, in hertz, to validate.</param>
    /// <returns><see langword="true"/> if the frequency is valid; otherwise, <see langword="false"/>.</returns>
    public static bool ValidateFrequency(float frequency)
    {
        return frequency >= 20f && frequency <= 20000f;
    }

    /// <summary>
    /// Validates that audio data array is not null or empty.
    /// </summary>
    /// <param name="data">The audio data array to validate.</param>
    /// <returns><see langword="true"/> if the array is not <see langword="null"/> or empty; otherwise, <see langword="false"/>.</returns>
    public static bool ValidateAudioData(float[]? data)
    {
        return data is not null && data.Length > 0;
    }

    /// <summary>
    /// Validates that amplitude values are within -1.0 to 1.0 range.
    /// </summary>
    /// <param name="amplitude">The amplitude value to validate.</param>
    /// <returns><see langword="true"/> if the amplitude is valid; otherwise, <see langword="false"/>.</returns>
    public static bool ValidateAmplitude(float amplitude)
    {
        return amplitude >= -1.0f && amplitude <= 1.0f;
    }

    /// <summary>
    /// Validates that a file path is not null or whitespace.
    /// </summary>
    /// <param name="path">The file path to validate.</param>
    /// <returns><see langword="true"/> if the path is not <see langword="null"/>, empty, or whitespace; otherwise, <see langword="false"/>.</returns>
    public static bool ValidateFilePath(string? path)
    {
        return !string.IsNullOrWhiteSpace(path);
    }

    /// <summary>
    /// Validates that a duration in seconds is positive.
    /// </summary>
    /// <param name="durationSeconds">The duration, in seconds, to validate.</param>
    /// <returns><see langword="true"/> if the duration is valid; otherwise, <see langword="false"/>.</returns>
    public static bool ValidateDuration(float durationSeconds)
    {
        return durationSeconds > 0f && durationSeconds < 3600f; // Up to 1 hour
    }

    /// <summary>
    /// Validates that a device index is non-negative.
    /// </summary>
    /// <param name="deviceIndex">The device index to validate.</param>
    /// <returns><see langword="true"/> if the device index is valid; otherwise, <see langword="false"/>.</returns>
    public static bool ValidateDeviceIndex(int deviceIndex)
    {
        return deviceIndex >= 0;
    }

    /// <summary>
    /// Validates a time value in milliseconds.
    /// </summary>
    /// <param name="timeMs">The time value, in milliseconds, to validate.</param>
    /// <returns><see langword="true"/> if the time value is valid; otherwise, <see langword="false"/>.</returns>
    public static bool ValidateTimeInMs(int timeMs)
    {
        return timeMs > 0 && timeMs < int.MaxValue;
    }

    /// <summary>
    /// Validates a normalization factor (should be positive and not zero).
    /// </summary>
    /// <param name="factor">The normalization factor to validate.</param>
    /// <returns><see langword="true"/> if the normalization factor is valid; otherwise, <see langword="false"/>.</returns>
    public static bool ValidateNormalization(float factor)
    {
        return !float.IsNaN(factor) && !float.IsInfinity(factor) && factor > 0f;
    }

    /// <summary>
    /// Validates that all required parameters are provided.
    /// </summary>
    /// <param name="parameters">The parameter values to validate.</param>
    /// <returns><see langword="true"/> if every parameter is non-null; otherwise, <see langword="false"/>.</returns>
    public static bool ValidateRequiredParameters(params object?[] parameters)
    {
        return parameters.All(p => p is not null);
    }

    /// <summary>
    /// Validates that a collection is not null or empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to validate.</param>
    /// <returns><see langword="true"/> if the collection is not <see langword="null"/> or empty; otherwise, <see langword="false"/>.</returns>
    public static bool ValidateCollection<T>(IEnumerable<T>? collection)
    {
        return collection is not null && collection.Any();
    }

    /// <summary>
    /// Throws an ArgumentException if a value doesn't meet criteria.
    /// </summary>
    /// <param name="isValid">A value indicating whether the validation criteria are met.</param>
    /// <param name="parameterName">The name of the parameter being validated.</param>
    /// <param name="reason">The reason the value is invalid.</param>
    /// <exception cref="ArgumentException"><paramref name="isValid"/> is <see langword="false"/>.</exception>
    public static void ThrowIfInvalid(bool isValid, string parameterName, string reason)
    {
        if (!isValid)
            throw new ArgumentException($"{parameterName}: {reason}", parameterName);
    }

    /// <summary>
    /// Throws an ArgumentNullException if a value is null.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="parameterName">The name of the parameter being validated.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public static void ThrowIfNull(object? value, string parameterName)
    {
        if (value is null)
            throw new ArgumentNullException(parameterName);
    }

    /// <summary>
    /// Throws an ArgumentException if a string is null or whitespace.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <param name="parameterName">The name of the parameter being validated.</param>
    /// <exception cref="ArgumentException"><paramref name="value"/> is <see langword="null"/>, empty, or consists only of whitespace.</exception>
    public static void ThrowIfNullOrWhitespace(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", parameterName);
    }

    /// <summary>
    /// Throws an ArgumentOutOfRangeException if a value is outside the specified range.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="min">The inclusive minimum permitted value.</param>
    /// <param name="max">The inclusive maximum permitted value.</param>
    /// <param name="parameterName">The name of the parameter being validated.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than <paramref name="min"/> or greater than <paramref name="max"/>.</exception>
    public static void ThrowIfOutOfRange(int value, int min, int max, string parameterName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be between {min} and {max}.");
    }

    /// <summary>
    /// Throws an ArgumentOutOfRangeException if a float value is outside range.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="min">The inclusive minimum permitted value.</param>
    /// <param name="max">The inclusive maximum permitted value.</param>
    /// <param name="parameterName">The name of the parameter being validated.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than <paramref name="min"/> or greater than <paramref name="max"/>.</exception>
    public static void ThrowIfOutOfRange(float value, float min, float max, string parameterName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be between {min} and {max}.");
    }
}
