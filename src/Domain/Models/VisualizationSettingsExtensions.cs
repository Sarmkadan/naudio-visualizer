#nullable enable

// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Drawing;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Themes;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Extension methods for <see cref="VisualizationSettings"/> that provide
/// convenient configuration and validation utilities.
/// </summary>
public static class VisualizationSettingsExtensions
{
    /// <summary>
    /// Creates a new instance of VisualizationSettings with default values.
    /// </summary>
    /// <returns>A new VisualizationSettings instance.</returns>
    public static VisualizationSettings CreateDefault()
    {
        return new VisualizationSettings
        {
            Theme = VisualizerTheme.Presets.Classic,
            RenderingQuality = 85,
            TargetFPS = 60,
            EnableAntiAliasing = true,
            BackgroundColor = 0xFF1a1a1a,
            ShowGrid = true,
            ShowFrequencyLabels = true,
            ShowTimeLabels = true,
            TimeScale = 50f,
            MaxFrequencyDisplay = 20000f,
            WaveformSettings = new WaveformRenderingSettings
            {
                LineColor = 0xFF00D9FF,
                LineThickness = 1.5f,
                ShowStereoSeparate = true,
                AmplitudeZoom = 1.0f,
                DownsamplingFactor = 4
            },
            SpectrumSettings = new SpectrumRenderingSettings
            {
                BarColor = 0xFF00FF00,
                UseLogFrequencyScale = true,
                UseLogMagnitudeScale = true,
                FrequencySmoothing = 3,
                TemporalSmoothing = 5,
                ShowFrequencyGrid = true,
                BarGap = 1
            },
            SpectrogramSettings = new SpectrogramRenderingSettings
            {
                Colormap = ColormapType.Viridis,
                UseLogFrequencyScale = true,
                UseLogMagnitudeScale = true,
                TimeWindowSeconds = 10f,
                Brightness = 1.0f,
                Contrast = 1.0f,
                ShowIntensityScale = true
            }
        };
    }

    /// <summary>
    /// Clones the current VisualizationSettings instance, creating a deep copy.
    /// </summary>
    /// <param name="settings">The settings to clone.</param>
    /// <returns>A new VisualizationSettings instance with the same values.</returns>
    public static VisualizationSettings Clone(this VisualizationSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        return new VisualizationSettings
        {
            Theme = settings.Theme,
            Id = settings.Id,
            RenderingQuality = settings.RenderingQuality,
            TargetFPS = settings.TargetFPS,
            EnableAntiAliasing = settings.EnableAntiAliasing,
            BackgroundColor = settings.BackgroundColor,
            ShowGrid = settings.ShowGrid,
            ShowFrequencyLabels = settings.ShowFrequencyLabels,
            ShowTimeLabels = settings.ShowTimeLabels,
            TimeScale = settings.TimeScale,
            MaxFrequencyDisplay = settings.MaxFrequencyDisplay,
            WaveformSettings = new WaveformRenderingSettings
            {
                LineColor = settings.WaveformSettings.LineColor,
                LineThickness = settings.WaveformSettings.LineThickness,
                ShowStereoSeparate = settings.WaveformSettings.ShowStereoSeparate,
                AmplitudeZoom = settings.WaveformSettings.AmplitudeZoom,
                DownsamplingFactor = settings.WaveformSettings.DownsamplingFactor
            },
            SpectrumSettings = new SpectrumRenderingSettings
            {
                BarColor = settings.SpectrumSettings.BarColor,
                UseLogFrequencyScale = settings.SpectrumSettings.UseLogFrequencyScale,
                UseLogMagnitudeScale = settings.SpectrumSettings.UseLogMagnitudeScale,
                FrequencySmoothing = settings.SpectrumSettings.FrequencySmoothing,
                TemporalSmoothing = settings.SpectrumSettings.TemporalSmoothing,
                ShowFrequencyGrid = settings.SpectrumSettings.ShowFrequencyGrid,
                BarGap = settings.SpectrumSettings.BarGap
            },
            SpectrogramSettings = new SpectrogramRenderingSettings
            {
                Colormap = settings.SpectrogramSettings.Colormap,
                UseLogFrequencyScale = settings.SpectrogramSettings.UseLogFrequencyScale,
                UseLogMagnitudeScale = settings.SpectrogramSettings.UseLogMagnitudeScale,
                TimeWindowSeconds = settings.SpectrogramSettings.TimeWindowSeconds,
                Brightness = settings.SpectrogramSettings.Brightness,
                Contrast = settings.SpectrogramSettings.Contrast,
                ShowIntensityScale = settings.SpectrogramSettings.ShowIntensityScale
            }
        };
    }

    /// <summary>
    /// Sets the visualization to a high-performance mode by adjusting
    /// rendering parameters for better performance.
    /// </summary>
    /// <param name="settings">The settings to modify.</param>
    /// <param name="downsamplingFactor">Optional downsampling factor (default: 8).</param>
    /// <param name="targetFPS">Optional target FPS (default: 120).</param>
    /// <returns>The modified VisualizationSettings instance for fluent chaining.</returns>
    public static VisualizationSettings WithHighPerformance(this VisualizationSettings settings,
        int downsamplingFactor = 8, int targetFPS = 120)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        settings.RenderingQuality = 60;
        settings.TargetFPS = targetFPS;
        settings.EnableAntiAliasing = false;
        settings.WaveformSettings.DownsamplingFactor = downsamplingFactor;
        settings.SpectrumSettings.FrequencySmoothing = 1;
        settings.SpectrumSettings.TemporalSmoothing = 2;
        settings.SpectrogramSettings.TimeWindowSeconds = 5f;

        return settings;
    }

    /// <summary>
    /// Sets the visualization to a high-quality mode by adjusting
    /// rendering parameters for maximum visual fidelity.
    /// </summary>
    /// <param name="settings">The settings to modify.</param>
    /// <param name="lineThickness">Optional line thickness for waveforms (default: 2.5).</param>
    /// <returns>The modified VisualizationSettings instance for fluent chaining.</returns>
    public static VisualizationSettings WithHighQuality(this VisualizationSettings settings,
        float lineThickness = 2.5f)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        settings.RenderingQuality = 100;
        settings.TargetFPS = 60;
        settings.EnableAntiAliasing = true;
        settings.WaveformSettings.LineThickness = lineThickness;
        settings.WaveformSettings.AmplitudeZoom = 1.5f;
        settings.SpectrumSettings.FrequencySmoothing = 5;
        settings.SpectrumSettings.TemporalSmoothing = 8;
        settings.SpectrogramSettings.Brightness = 1.5f;
        settings.SpectrogramSettings.Contrast = 1.3f;

        return settings;
    }

    /// <summary>
    /// Validates the settings and throws an exception if invalid.
    /// </summary>
    /// <param name="settings">The settings to validate.</param>
    /// <exception cref="ArgumentException">Thrown when settings are invalid.</exception>
    public static void Validate(this VisualizationSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        if (!settings.IsValid())
        {
            throw new ArgumentException(
                "VisualizationSettings are invalid. Ensure RenderingQuality (0-100), " +
                "TargetFPS (>0), TimeScale (>0), and MaxFrequencyDisplay (>0) are valid.");
        }

        if (settings.WaveformSettings.DownsamplingFactor < 1)
        {
            throw new ArgumentException("DownsamplingFactor must be at least 1.");
        }

        if (settings.WaveformSettings.LineThickness <= 0)
        {
            throw new ArgumentException("LineThickness must be positive.");
        }

        if (settings.WaveformSettings.AmplitudeZoom <= 0)
        {
            throw new ArgumentException("AmplitudeZoom must be positive.");
        }

        if (settings.SpectrumSettings.FrequencySmoothing < 0 ||
            settings.SpectrumSettings.FrequencySmoothing > 10)
        {
            throw new ArgumentException("FrequencySmoothing must be between 0 and 10.");
        }

        if (settings.SpectrumSettings.TemporalSmoothing < 0 ||
            settings.SpectrumSettings.TemporalSmoothing > 10)
        {
            throw new ArgumentException("TemporalSmoothing must be between 0 and 10.");
        }
    }

    /// <summary>
    /// Gets the background color as a Color structure.
    /// </summary>
    /// <param name="settings">The settings containing the background color.</param>
    /// <returns>The background color as a Color.</returns>
    public static Color GetBackgroundColor(this VisualizationSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        return Color.FromArgb((int)settings.BackgroundColor);
    }

    /// <summary>
    /// Gets the waveform line color as a Color structure.
    /// </summary>
    /// <param name="settings">The settings containing the waveform settings.</param>
    /// <returns>The waveform line color as a Color.</returns>
    public static Color GetWaveformLineColor(this VisualizationSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        return Color.FromArgb((int)settings.WaveformSettings.LineColor);
    }

    /// <summary>
    /// Gets the spectrum bar color as a Color structure.
    /// </summary>
    /// <param name="settings">The settings containing the spectrum settings.</param>
    /// <returns>The spectrum bar color as a Color.</returns>
    public static Color GetSpectrumBarColor(this VisualizationSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        return Color.FromArgb((int)settings.SpectrumSettings.BarColor);
    }
}