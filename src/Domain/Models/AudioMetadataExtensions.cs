using System;
using System.Collections.Generic;
using System.Globalization;

namespace NAudioVisualizer.Domain.Models
{
    /// <summary>
    /// Extension methods that provide additional insight and utility operations for <see cref="AudioMetadata"/>.
    /// </summary>
    public static class AudioMetadataExtensions
    {
        /// <summary>
        /// Returns the current captured duration formatted as a human‑readable string (hh:mm:ss.fff).
        /// </summary>
        /// <param name="metadata">The <see cref="AudioMetadata"/> instance.</param>
        /// <returns>A formatted duration string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="metadata"/> is <c>null</c>.</exception>
        public static string GetFormattedDuration(this AudioMetadata metadata)
        {
            ArgumentNullException.ThrowIfNull(metadata);
            var ts = TimeSpan.FromSeconds(metadata.CurrentDurationSeconds);
            return ts.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Retrieves the level metrics (current, peak and average) as a read‑only dictionary.
        /// </summary>
        /// <param name="metadata">The <see cref="AudioMetadata"/> instance.</param>
        /// <returns>An <see cref="IReadOnlyDictionary{TKey,TValue}"/> containing the level values.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="metadata"/> is <c>null</c>.</exception>
        public static IReadOnlyDictionary<string, float> GetLevelMetrics(this AudioMetadata metadata)
        {
            ArgumentNullException.ThrowIfNull(metadata);
            var dict = new Dictionary<string, float>(capacity: 3)
            {
                ["Current"] = metadata.CurrentLevel,
                ["Peak"]    = metadata.PeakLevel,
                ["Average"] = metadata.AverageLevel
            };
            return dict;
        }

        /// <summary>
        /// Determines whether the capture session is experiencing performance issues.
        /// </summary>
        /// <param name="metadata">The <see cref="AudioMetadata"/> instance.</param>
        /// <returns>
        /// <c>true</c> if CPU usage exceeds 80 % or any buffer underruns have been recorded; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="metadata"/> is <c>null</c>.</exception>
        public static bool HasPerformanceIssues(this AudioMetadata metadata)
        {
            ArgumentNullException.ThrowIfNull(metadata);
            return metadata.CpuUsagePercent > 80f || metadata.BufferUnderruns > 0;
        }

        /// <summary>
        /// Returns a concise description of the associated audio device, or a placeholder if none is set.
        /// </summary>
        /// <param name="metadata">The <see cref="AudioMetadata"/> instance.</param>
        /// <returns>A string describing the audio device.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="metadata"/> is <c>null</c>.</exception>
        public static string GetDeviceDescription(this AudioMetadata metadata)
        {
            ArgumentNullException.ThrowIfNull(metadata);
            return metadata.AudioDevice?.ToString() ?? "No audio device assigned";
        }
    }
}
