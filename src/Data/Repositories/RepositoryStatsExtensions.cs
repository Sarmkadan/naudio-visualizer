#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;

namespace NAudioVisualizer.Data.Repositories
{
    /// <summary>
    /// Extension methods for <see cref="RepositoryStats"/>.
    /// </summary>
    public static class RepositoryStatsExtensions
    {
        /// <summary>
        /// Returns a summary string of the repository statistics.
        /// </summary>
        /// <param name="stats">The repository statistics.</param>
        /// <returns>A string summarizing the counts.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stats"/> is null.</exception>
        public static string ToSummaryString(this RepositoryStats stats)
        {
            if (stats is null)
                throw new ArgumentNullException(nameof(stats));

            return $"Waveform: {stats.WaveformCount}, Spectrum: {stats.SpectrumCount}, Spectrogram: {stats.SpectrogramCount}, Sessions: {stats.SessionCount}";
        }

        /// <summary>
        /// Determines whether the repository statistics is empty.
        /// </summary>
        /// <param name="stats">The repository statistics.</param>
        /// <returns>true if the total count is zero; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stats"/> is null.</exception>
        public static bool IsEmpty(this RepositoryStats stats)
        {
            if (stats is null)
                throw new ArgumentNullException(nameof(stats));

            return stats.TotalCount == 0;
        }

        /// <summary>
        /// Returns the percentage breakdown of visualization types.
        /// </summary>
        /// <param name="stats">The repository statistics.</param>
        /// <returns>A string representing the percentage of each visualization type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stats"/> is null.</exception>
        public static string ToPercentageString(this RepositoryStats stats)
        {
            if (stats is null)
                throw new ArgumentNullException(nameof(stats));

            if (stats.TotalCount == 0)
                return "Waveform: 0%, Spectrum: 0%, Spectrogram: 0%";

            double waveformPct = (double)stats.WaveformCount / stats.TotalCount * 100;
            double spectrumPct = (double)stats.SpectrumCount / stats.TotalCount * 100;
            double spectrogramPct = (double)stats.SpectrogramCount / stats.TotalCount * 100;

            return $"Waveform: {waveformPct:F1}%, Spectrum: {spectrumPct:F1}%, Spectrogram: {spectrogramPct:F1}%";
        }
    }
}