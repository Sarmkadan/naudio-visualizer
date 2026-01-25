// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using NAudioVisualizer.Domain.Models;

namespace NAudioVisualizer.Infrastructure;

/// <summary>
/// Utility class for converting and formatting audio data.
/// </summary>
public static class AudioDataConverter
{
    /// <summary>
    /// Converts decibels to linear amplitude.
    /// </summary>
    public static float DbToLinear(float db, float referenceLevel = 1f)
    {
        return referenceLevel * (float)Math.Pow(10f, db / 20f);
    }

    /// <summary>
    /// Converts linear amplitude to decibels.
    /// </summary>
    public static float LinearToDb(float linear, float referenceLevel = 1f)
    {
        if (linear <= 0)
            return float.NegativeInfinity;

        return 20f * (float)Math.Log10(linear / referenceLevel);
    }

    /// <summary>
    /// Formats a frequency value as a human-readable string.
    /// </summary>
    public static string FormatFrequency(float frequencyHz)
    {
        if (frequencyHz < 1000)
            return $"{frequencyHz:F1} Hz";

        return $"{frequencyHz / 1000:F2} kHz";
    }

    /// <summary>
    /// Formats a time duration as a human-readable string.
    /// </summary>
    public static string FormatDuration(double seconds)
    {
        if (seconds < 60)
            return $"{seconds:F2}s";

        var timespan = TimeSpan.FromSeconds(seconds);
        return timespan.ToString(@"hh\:mm\:ss");
    }

    /// <summary>
    /// Formats an audio level as a percentage string.
    /// </summary>
    public static string FormatAudioLevel(float level)
    {
        return $"{Math.Clamp(level * 100, 0, 100):F1}%";
    }

    /// <summary>
    /// Formats an audio level in decibels.
    /// </summary>
    public static string FormatAudioLevelDb(float linear)
    {
        float db = LinearToDb(linear);
        if (float.IsNegativeInfinity(db))
            return "-∞ dB";

        return $"{db:F2} dB";
    }

    /// <summary>
    /// Converts float samples to 16-bit PCM byte array.
    /// </summary>
    public static byte[] FloatToInt16Pcm(float[] samples)
    {
        if (samples == null)
            throw new ArgumentNullException(nameof(samples));

        var bytes = new byte[samples.Length * 2];

        for (int i = 0; i < samples.Length; i++)
        {
            // Clamp to -1.0 to 1.0
            float sample = Math.Clamp(samples[i], -1f, 1f);

            // Convert to 16-bit integer
            short value = (short)(sample * 32767);

            // Convert to bytes (little-endian)
            bytes[i * 2] = (byte)(value & 0xFF);
            bytes[i * 2 + 1] = (byte)((value >> 8) & 0xFF);
        }

        return bytes;
    }

    /// <summary>
    /// Converts 16-bit PCM byte array to float samples.
    /// </summary>
    public static float[] Int16PcmToFloat(byte[] bytes)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));

        int sampleCount = bytes.Length / 2;
        var samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            // Read 16-bit value (little-endian)
            short value = (short)((bytes[i * 2 + 1] << 8) | bytes[i * 2]);

            // Convert to float
            samples[i] = value / 32768f;
        }

        return samples;
    }

    /// <summary>
    /// Extracts a single channel from interleaved multi-channel audio.
    /// </summary>
    public static float[] ExtractChannel(float[] interleavedSamples, int channelIndex, int channelCount)
    {
        if (interleavedSamples == null)
            throw new ArgumentNullException(nameof(interleavedSamples));

        if (channelIndex < 0 || channelIndex >= channelCount)
            throw new ArgumentOutOfRangeException(nameof(channelIndex));

        int sampleCount = interleavedSamples.Length / channelCount;
        var channelData = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            channelData[i] = interleavedSamples[i * channelCount + channelIndex];
        }

        return channelData;
    }

    /// <summary>
    /// Interleaves multiple channel arrays into a single sample array.
    /// </summary>
    public static float[] InterleaveChannels(float[][] channels)
    {
        if (channels == null || channels.Length == 0)
            throw new ArgumentException("Channels array cannot be null or empty", nameof(channels));

        int samplesPerChannel = channels[0].Length;
        int totalSamples = samplesPerChannel * channels.Length;
        var interleavedSamples = new float[totalSamples];

        for (int i = 0; i < samplesPerChannel; i++)
        {
            for (int c = 0; c < channels.Length; c++)
            {
                interleavedSamples[i * channels.Length + c] = channels[c][i];
            }
        }

        return interleavedSamples;
    }

    /// <summary>
    /// Calculates RMS (Root Mean Square) level of audio samples.
    /// </summary>
    public static float CalculateRmsLevel(float[] samples)
    {
        if (samples == null || samples.Length == 0)
            return 0f;

        double sumSquares = 0;
        foreach (var sample in samples)
        {
            sumSquares += sample * sample;
        }

        return (float)Math.Sqrt(sumSquares / samples.Length);
    }

    /// <summary>
    /// Calculates peak (maximum absolute value) of audio samples.
    /// </summary>
    public static float CalculatePeakLevel(float[] samples)
    {
        if (samples == null || samples.Length == 0)
            return 0f;

        float peak = 0f;
        foreach (var sample in samples)
        {
            float abs = Math.Abs(sample);
            if (abs > peak)
                peak = abs;
        }

        return peak;
    }

    /// <summary>
    /// Normalizes audio samples to prevent clipping.
    /// </summary>
    public static float[] NormalizeSamples(float[] samples)
    {
        if (samples == null || samples.Length == 0)
            return samples;

        float peak = CalculatePeakLevel(samples);
        if (peak == 0 || peak >= 1f)
            return samples;

        var normalized = new float[samples.Length];
        for (int i = 0; i < samples.Length; i++)
        {
            normalized[i] = samples[i] / peak;
        }

        return normalized;
    }

    /// <summary>
    /// Applies gain (amplification) to audio samples.
    /// </summary>
    public static float[] ApplyGain(float[] samples, float gainDb)
    {
        if (samples == null || samples.Length == 0)
            return samples;

        float gainLinear = DbToLinear(gainDb);
        var gainedSamples = new float[samples.Length];

        for (int i = 0; i < samples.Length; i++)
        {
            gainedSamples[i] = Math.Clamp(samples[i] * gainLinear, -1f, 1f);
        }

        return gainedSamples;
    }
}
