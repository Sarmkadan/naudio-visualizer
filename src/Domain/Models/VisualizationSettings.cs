#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Configuration settings for audio visualization rendering.
/// </summary>
public class VisualizationSettings
{
    /// <summary>
    /// Unique settings identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Waveform rendering settings.
    /// </summary>
    public WaveformRenderingSettings WaveformSettings { get; set; } = new();

    /// <summary>
    /// Spectrum rendering settings.
    /// </summary>
    public SpectrumRenderingSettings SpectrumSettings { get; set; } = new();

    /// <summary>
    /// Spectrogram rendering settings.
    /// </summary>
    public SpectrogramRenderingSettings SpectrogramSettings { get; set; } = new();

    /// <summary>
    /// General rendering quality (0-100).
    /// </summary>
    public int RenderingQuality { get; set; } = 85;

    /// <summary>
    /// Target frames per second for visualization updates.
    /// </summary>
    public int TargetFPS { get; set; } = 60;

    /// <summary>
    /// Whether to enable anti-aliasing.
    /// </summary>
    public bool EnableAntiAliasing { get; set; } = true;

    /// <summary>
    /// Background color (ARGB format).
    /// </summary>
    public uint BackgroundColor { get; set; } = 0xFF1a1a1a;

    /// <summary>
    /// Whether to show grid lines.
    /// </summary>
    public bool ShowGrid { get; set; } = true;

    /// <summary>
    /// Whether to show frequency labels.
    /// </summary>
    public bool ShowFrequencyLabels { get; set; } = true;

    /// <summary>
    /// Whether to show time labels.
    /// </summary>
    public bool ShowTimeLabels { get; set; } = true;

    /// <summary>
    /// Display scale: how many pixels per second of audio.
    /// </summary>
    public float TimeScale { get; set; } = 50f;

    /// <summary>
    /// Maximum frequency to display (Hz).
    /// </summary>
    public float MaxFrequencyDisplay { get; set; } = 20000f;

    /// <summary>
    /// Validates the settings are valid.
    /// </summary>
    public bool IsValid()
    {
        return RenderingQuality >= 0 && RenderingQuality <= 100 &&
               TargetFPS > 0 && TargetFPS <= 240 &&
               TimeScale > 0 &&
               MaxFrequencyDisplay > 0;
    }
}

/// <summary>
/// Waveform-specific rendering settings.
/// </summary>
public class WaveformRenderingSettings
{
    /// <summary>
    /// Waveform line color (ARGB format).
    /// </summary>
    public uint LineColor { get; set; } = 0xFF00D9FF;

    /// <summary>
    /// Line thickness in pixels.
    /// </summary>
    public float LineThickness { get; set; } = 1.5f;

    /// <summary>
    /// Whether to show stereo channels separately.
    /// </summary>
    public bool ShowStereoSeparate { get; set; } = true;

    /// <summary>
    /// Vertical zoom level for waveform amplitude (0.1 to 10.0).
    /// </summary>
    public float AmplitudeZoom { get; set; } = 1.0f;

    /// <summary>
    /// Downsampling factor for performance.
    /// </summary>
    public int DownsamplingFactor { get; set; } = 4;

    /// <summary>
    /// Whether to show peak indicators.
    /// </summary>
    public bool ShowPeakIndicators { get; set; } = true;
}

/// <summary>
/// Spectrum-specific rendering settings.
/// </summary>
public class SpectrumRenderingSettings
{
    /// <summary>
    /// Spectrum bar color (ARGB format).
    /// </summary>
    public uint BarColor { get; set; } = 0xFF00FF00;

    /// <summary>
    /// Whether to use logarithmic frequency scale.
    /// </summary>
    public bool UseLogFrequencyScale { get; set; } = true;

    /// <summary>
    /// Whether to use logarithmic magnitude scale (dB).
    /// </summary>
    public bool UseLogMagnitudeScale { get; set; } = true;

    /// <summary>
    /// Frequency smoothing amount (0-10).
    /// </summary>
    public int FrequencySmoothing { get; set; } = 3;

    /// <summary>
    /// Temporal smoothing amount (0-10).
    /// </summary>
    public int TemporalSmoothing { get; set; } = 5;

    /// <summary>
    /// Whether to show frequency grid.
    /// </summary>
    public bool ShowFrequencyGrid { get; set; } = true;

    /// <summary>
    /// Bar gap size in pixels.
    /// </summary>
    public int BarGap { get; set; } = 1;
}

/// <summary>
/// Spectrogram-specific rendering settings.
/// </summary>
public class SpectrogramRenderingSettings
{
    /// <summary>
    /// Colormap type for spectrogram visualization.
    /// </summary>
    public ColormapType Colormap { get; set; } = ColormapType.Viridis;

    /// <summary>
    /// Whether to use logarithmic frequency scale.
    /// </summary>
    public bool UseLogFrequencyScale { get; set; } = true;

    /// <summary>
    /// Whether to use logarithmic magnitude scale.
    /// </summary>
    public bool UseLogMagnitudeScale { get; set; } = true;

    /// <summary>
    /// Time window size in seconds for spectrogram.
    /// </summary>
    public float TimeWindowSeconds { get; set; } = 10f;

    /// <summary>
    /// Brightness adjustment (0.1 to 3.0).
    /// </summary>
    public float Brightness { get; set; } = 1.0f;

    /// <summary>
    /// Contrast adjustment (0.5 to 2.0).
    /// </summary>
    public float Contrast { get; set; } = 1.0f;

    /// <summary>
    /// Whether to show intensity scale bar.
    /// </summary>
    public bool ShowIntensityScale { get; set; } = true;
}
