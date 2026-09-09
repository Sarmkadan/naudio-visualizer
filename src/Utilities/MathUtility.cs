#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;

namespace NAudioVisualizer.Utilities;

/// <summary>
/// Provides mathematical utility functions for audio processing.
/// Includes functions for frequency conversions, dB calculations, and signal processing.
/// </summary>
public static class MathUtility
{
    /// <summary>
    /// Converts frequency in Hz to MIDI note number.
    /// MIDI note 69 = 440 Hz (A4)
    /// </summary>
    /// <param name="frequency">The frequency in hertz.</param>
    /// <returns>The nearest MIDI note number, or 0 when <paramref name="frequency"/> is non-positive.</returns>
    public static int FrequencyToMidiNote(float frequency)
    {
        if (frequency <= 0)
            return 0;

        // MIDI = 69 + 12 * log2(f / 440)
        double noteNumber = 69.0 + 12.0 * Math.Log2(frequency / 440.0);
        return (int)Math.Round(noteNumber);
    }

    /// <summary>
    /// Converts MIDI note number to frequency in Hz.
    /// </summary>
    /// <param name="midiNote">The MIDI note number to convert.</param>
    /// <returns>The corresponding frequency in hertz.</returns>
    public static float MidiNoteToFrequency(int midiNote)
    {
        // f = 440 * 2^((midi - 69) / 12)
        double frequency = 440.0 * Math.Pow(2.0, (midiNote - 69.0) / 12.0);
        return (float)frequency;
    }

    /// <summary>
    /// Converts linear amplitude to decibels.
    /// Reference value: 1.0 = 0 dB
    /// </summary>
    /// <param name="amplitude">The linear amplitude to convert.</param>
    /// <returns>The amplitude in decibels, or negative infinity when <paramref name="amplitude"/> is non-positive.</returns>
    public static float AmplitudeToDb(float amplitude)
    {
        if (amplitude <= 0)
            return float.NegativeInfinity;

        return 20f * MathF.Log10(Math.Abs(amplitude));
    }

    /// <summary>
    /// Converts decibels to linear amplitude.
    /// 0 dB = 1.0 amplitude
    /// </summary>
    /// <param name="db">The value in decibels.</param>
    /// <returns>The corresponding linear amplitude.</returns>
    public static float DbToAmplitude(float db)
    {
        return MathF.Pow(10f, db / 20f);
    }

    /// <summary>
    /// Calculates the root mean square (RMS) of a signal.
    /// RMS is a measure of signal energy.
    /// </summary>
    /// <param name="signal">The signal samples to measure.</param>
    /// <returns>The root mean square of the samples, or 0 when <paramref name="signal"/> is null or empty.</returns>
    public static float CalculateRms(float[] signal)
    {
        if (signal is null || signal.Length == 0)
            return 0f;

        float sum = 0f;
        for (int i = 0; i < signal.Length; i++)
        {
            sum += signal[i] * signal[i];
        }

        return MathF.Sqrt(sum / signal.Length);
    }

    /// <summary>
    /// Calculates peak amplitude in a signal.
    /// </summary>
    /// <param name="signal">The signal samples to measure.</param>
    /// <returns>The greatest absolute sample value, or 0 when <paramref name="signal"/> is null or empty.</returns>
    public static float CalculatePeak(float[] signal)
    {
        if (signal is null || signal.Length == 0)
            return 0f;

        float peak = 0f;
        for (int i = 0; i < signal.Length; i++)
        {
            float abs = Math.Abs(signal[i]);
            if (abs > peak)
                peak = abs;
        }

        return peak;
    }

    /// <summary>
    /// Applies logarithmic scaling to a value for visualization.
    /// Useful for converting linear frequency to logarithmic scale.
    /// </summary>
    /// <param name="value">The value to scale.</param>
    /// <param name="minValue">The minimum value used as the logarithm argument.</param>
    /// <returns>The base-10 logarithm of the greater of <paramref name="value"/> and <paramref name="minValue"/>, or 0 when <paramref name="value"/> is non-positive.</returns>
    public static float LogScale(float value, float minValue = 1f)
    {
        if (value <= 0)
            return 0f;

        return MathF.Log10(Math.Max(value, minValue));
    }

    /// <summary>
    /// Applies power law scaling (gamma correction).
    /// Used for perceptual loudness scaling.
    /// </summary>
    /// <param name="value">The value to scale.</param>
    /// <param name="gamma">The exponent divisor used for the power-law transformation.</param>
    /// <returns><paramref name="value"/> raised to the reciprocal of <paramref name="gamma"/>, or 0 when <paramref name="value"/> is negative.</returns>
    public static float PowerScale(float value, float gamma = 2.0f)
    {
        if (value < 0)
            return 0f;

        return MathF.Pow(value, 1f / gamma);
    }

    /// <summary>
    /// Applies a Hann window function to a signal.
    /// </summary>
    /// <param name="signal">The signal samples to modify in place.</param>
    /// <returns>No value is returned.</returns>
    public static void ApplyHannWindow(float[] signal)
    {
        if (signal is null || signal.Length == 0)
            return;

        int n = signal.Length;
        for (int i = 0; i < n; i++)
        {
            float window = 0.5f * (1f - MathF.Cos(2f * MathF.PI * i / (n - 1)));
            signal[i] *= window;
        }
    }

    /// <summary>
    /// Applies a Hamming window function to a signal.
    /// </summary>
    /// <param name="signal">The signal samples to modify in place.</param>
    /// <returns>No value is returned.</returns>
    public static void ApplyHammingWindow(float[] signal)
    {
        if (signal is null || signal.Length == 0)
            return;

        int n = signal.Length;
        for (int i = 0; i < n; i++)
        {
            float window = 0.54f - 0.46f * MathF.Cos(2f * MathF.PI * i / (n - 1));
            signal[i] *= window;
        }
    }

    /// <summary>
    /// Calculates next power of 2 greater than or equal to n.
    /// Used for FFT size calculations.
    /// </summary>
    /// <param name="n">The value for which to find a power of 2.</param>
    /// <returns>The smallest power of 2 greater than or equal to <paramref name="n"/>, or 1 when <paramref name="n"/> is non-positive.</returns>
    public static int NextPowerOf2(int n)
    {
        if (n <= 0)
            return 1;

        int power = 1;
        while (power < n)
            power *= 2;

        return power;
    }

    /// <summary>
    /// Checks if a number is a power of 2.
    /// </summary>
    /// <param name="n">The number to check.</param>
    /// <returns><see langword="true"/> when <paramref name="n"/> is a positive power of 2; otherwise, <see langword="false"/>.</returns>
    public static bool IsPowerOf2(int n)
    {
        return n > 0 && (n & (n - 1)) == 0;
    }

    /// <summary>
    /// Linearly interpolates between two values.
    /// </summary>
    /// <param name="a">The value returned when <paramref name="t"/> is 0.</param>
    /// <param name="b">The value returned when <paramref name="t"/> is 1.</param>
    /// <param name="t">The interpolation amount, clamped to the range from 0 to 1.</param>
    /// <returns>The linearly interpolated value between <paramref name="a"/> and <paramref name="b"/>.</returns>
    public static float Lerp(float a, float b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return a + (b - a) * t;
    }

    /// <summary>
    /// Maps a value from one range to another.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="fromMin">The start of the source range.</param>
    /// <param name="fromMax">The end of the source range.</param>
    /// <param name="toMin">The start of the destination range.</param>
    /// <param name="toMax">The end of the destination range.</param>
    /// <returns>The value mapped to the destination range, or <paramref name="toMin"/> when the source range has zero length.</returns>
    public static float MapRange(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        if (fromMax == fromMin)
            return toMin;

        float normalized = (value - fromMin) / (fromMax - fromMin);
        return toMin + normalized * (toMax - toMin);
    }

    /// <summary>
    /// Calculates distance between two points.
    /// </summary>
    /// <param name="x1">The x-coordinate of the first point.</param>
    /// <param name="y1">The y-coordinate of the first point.</param>
    /// <param name="x2">The x-coordinate of the second point.</param>
    /// <param name="y2">The y-coordinate of the second point.</param>
    /// <returns>The Euclidean distance between the two points.</returns>
    public static float Distance(float x1, float y1, float x2, float y2)
    {
        float dx = x2 - x1;
        float dy = y2 - y1;
        return MathF.Sqrt(dx * dx + dy * dy);
    }
}
