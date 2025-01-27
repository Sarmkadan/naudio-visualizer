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
    public static float DbToAmplitude(float db)
    {
        return MathF.Pow(10f, db / 20f);
    }

    /// <summary>
    /// Calculates the root mean square (RMS) of a signal.
    /// RMS is a measure of signal energy.
    /// </summary>
    public static float CalculateRms(float[] signal)
    {
        if (signal == null || signal.Length == 0)
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
    public static float CalculatePeak(float[] signal)
    {
        if (signal == null || signal.Length == 0)
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
    public static float PowerScale(float value, float gamma = 2.0f)
    {
        if (value < 0)
            return 0f;

        return MathF.Pow(value, 1f / gamma);
    }

    /// <summary>
    /// Applies a Hann window function to a signal.
    /// </summary>
    public static void ApplyHannWindow(float[] signal)
    {
        if (signal == null || signal.Length == 0)
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
    public static void ApplyHammingWindow(float[] signal)
    {
        if (signal == null || signal.Length == 0)
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
    public static bool IsPowerOf2(int n)
    {
        return n > 0 && (n & (n - 1)) == 0;
    }

    /// <summary>
    /// Linearly interpolates between two values.
    /// </summary>
    public static float Lerp(float a, float b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return a + (b - a) * t;
    }

    /// <summary>
    /// Maps a value from one range to another.
    /// </summary>
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
    public static float Distance(float x1, float y1, float x2, float y2)
    {
        float dx = x2 - x1;
        float dy = y2 - y1;
        return MathF.Sqrt(dx * dx + dy * dy);
    }
}
