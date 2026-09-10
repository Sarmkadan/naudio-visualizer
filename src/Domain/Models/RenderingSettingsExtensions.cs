#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;

namespace NAudioVisualizer.Domain.Models
{
    /// <summary>
    /// Extension methods for rendering settings classes providing deep-copy cloning and validation.
    /// </summary>
    public static class RenderingSettingsExtensions
    {
        /// <summary>
        /// Creates a deep copy of the waveform rendering settings.
        /// </summary>
        /// <param name="settings">The settings to clone.</param>
        /// <returns>A new instance with the same property values.</returns>
        public static WaveformRenderingSettings Clone(this WaveformRenderingSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            return new WaveformRenderingSettings
            {
                LineColor = settings.LineColor,
                LineThickness = settings.LineThickness,
                ShowStereoSeparate = settings.ShowStereoSeparate,
                AmplitudeZoom = settings.AmplitudeZoom,
                DownsamplingFactor = settings.DownsamplingFactor,
                ShowPeakIndicators = settings.ShowPeakIndicators
            };
        }

        /// <summary>
        /// Validates the waveform rendering settings.
        /// </summary>
        /// <param name="settings">The settings to validate.</param>
        /// <returns>True if all settings are within valid ranges; otherwise false.</returns>
        public static bool IsValid(this WaveformRenderingSettings settings)
        {
            if (settings == null)
                return false;

            return settings.LineThickness > 0 &&
                   settings.AmplitudeZoom >= 0.1f && settings.AmplitudeZoom <= 10.0f &&
                   settings.DownsamplingFactor >= 1;
        }

        /// <summary>
        /// Creates a deep copy of the spectrum rendering settings.
        /// </summary>
        /// <param name="settings">The settings to clone.</param>
        /// <returns>A new instance with the same property values.</returns>
        public static SpectrumRenderingSettings Clone(this SpectrumRenderingSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            return new SpectrumRenderingSettings
            {
                BarColor = settings.BarColor,
                UseLogFrequencyScale = settings.UseLogFrequencyScale,
                UseLogMagnitudeScale = settings.UseLogMagnitudeScale,
                FrequencySmoothing = settings.FrequencySmoothing,
                TemporalSmoothing = settings.TemporalSmoothing,
                ShowFrequencyGrid = settings.ShowFrequencyGrid,
                BarGap = settings.BarGap
            };
        }

        /// <summary>
        /// Validates the spectrum rendering settings.
        /// </summary>
        /// <param name="settings">The settings to validate.</param>
        /// <returns>True if all settings are within valid ranges; otherwise false.</returns>
        public static bool IsValid(this SpectrumRenderingSettings settings)
        {
            if (settings == null)
                return false;

            return settings.FrequencySmoothing >= 0 && settings.FrequencySmoothing <= 10 &&
                   settings.TemporalSmoothing >= 0 && settings.TemporalSmoothing <= 10 &&
                   settings.BarGap >= 0;
        }

        /// <summary>
        /// Creates a deep copy of the spectrogram rendering settings.
        /// </summary>
        /// <param name="settings">The settings to clone.</param>
        /// <returns>A new instance with the same property values.</returns>
        public static SpectrogramRenderingSettings Clone(this SpectrogramRenderingSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            return new SpectrogramRenderingSettings
            {
                Colormap = settings.Colormap,
                UseLogFrequencyScale = settings.UseLogFrequencyScale,
                UseLogMagnitudeScale = settings.UseLogMagnitudeScale,
                TimeWindowSeconds = settings.TimeWindowSeconds,
                Brightness = settings.Brightness,
                Contrast = settings.Contrast,
                ShowIntensityScale = settings.ShowIntensityScale
            };
        }

        /// <summary>
        /// Validates the spectrogram rendering settings.
        /// </summary>
        /// <param name="settings">The settings to validate.</param>
        /// <returns>True if all settings are within valid ranges; otherwise false.</returns>
        public static bool IsValid(this SpectrogramRenderingSettings settings)
        {
            if (settings == null)
                return false;

            return settings.TimeWindowSeconds > 0 &&
                   settings.Brightness >= 0.1f && settings.Brightness <= 3.0f &&
                   settings.Contrast >= 0.5f && settings.Contrast <= 2.0f;
        }
    }
}