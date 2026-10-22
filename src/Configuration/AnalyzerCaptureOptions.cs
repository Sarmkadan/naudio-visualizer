#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using NAudioVisualizer.Constants;

namespace NAudioVisualizer.Configuration;

/// <summary>
/// Options shared by the spectrum analyzer and the audio capture service.
/// Validated with <see cref="AnalyzerCaptureOptionsValidation.EnsureValid"/> when a consumer is constructed,
/// so invalid configuration fails at startup instead of being silently replaced at runtime.
/// </summary>
public sealed class AnalyzerCaptureOptions
{
    /// <summary>
    /// Default buffer length in milliseconds.
    /// </summary>
    public const int DEFAULT_BUFFER_MILLISECONDS = 1000;

    /// <summary>
    /// Default smoothing factor in the range [0, 1].
    /// </summary>
    public const float DEFAULT_SMOOTHING_FACTOR = 0.5f;

    /// <summary>
    /// FFT window size in samples. Must be a power of two within
    /// [<see cref="AudioConstants.FFT_MINIMUM"/>, <see cref="AudioConstants.FFT_MAXIMUM"/>].
    /// </summary>
    public int FftSize { get; set; } = AudioConstants.DEFAULT_FFT_SIZE;

    /// <summary>
    /// Sample rate in Hz. Must be within [<see cref="AudioConstants.MIN_SAMPLE_RATE"/>, <see cref="AudioConstants.MAX_SAMPLE_RATE"/>].
    /// </summary>
    public int SampleRate { get; set; } = AudioConstants.DEFAULT_SAMPLE_RATE;

    /// <summary>
    /// Number of audio channels. Must be 1 (mono) or 2 (stereo).
    /// </summary>
    public int Channels { get; set; } = AudioConstants.DEFAULT_CHANNEL_COUNT;

    /// <summary>
    /// Length of the capture buffer in milliseconds. Must be greater than zero.
    /// </summary>
    public int BufferMilliseconds { get; set; } = DEFAULT_BUFFER_MILLISECONDS;

    /// <summary>
    /// Smoothing factor applied to the analysis output. Must be within [0, 1] (NaN is rejected).
    /// </summary>
    public float SmoothingFactor { get; set; } = DEFAULT_SMOOTHING_FACTOR;
}
