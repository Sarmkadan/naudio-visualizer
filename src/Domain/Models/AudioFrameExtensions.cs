using System;
using System.Collections.Generic;
using System.Globalization;

namespace NAudioVisualizer.Domain.Models
{
    /// <summary>
    /// Extension methods for <see cref="AudioFrame"/>.
    /// </summary>
    public static class AudioFrameExtensions
    {
        /// <summary>
        /// Calculates the average amplitude of an <see cref="AudioFrame"/>.
        /// </summary>
        /// <param name="frame">The <see cref="AudioFrame"/> to calculate the average amplitude for.</param>
        /// <returns>The average amplitude.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="frame"/> is null.</exception>
        public static float CalculateAverageAmplitude(this AudioFrame frame)
        {
            ArgumentNullException.ThrowIfNull(frame);

            float sum = 0;
            foreach (float sample in frame.Samples)
            {
                sum += Math.Abs(sample);
            }
            return sum / frame.Samples.Length;
        }

        /// <summary>
        /// Gets the channel data as a dictionary where the key is the channel index and the value is the channel data.
        /// </summary>
        /// <param name="frame">The <see cref="AudioFrame"/> to get the channel data for.</param>
        /// <returns>A dictionary where the key is the channel index and the value is the channel data.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="frame"/> is null.</exception>
        public static IReadOnlyDictionary<int, float[]> GetChannelDataDictionary(this AudioFrame frame)
        {
            ArgumentNullException.ThrowIfNull(frame);

            var channelData = new Dictionary<int, float[]>();
            for (int i = 0; i < frame.ChannelCount; i++)
            {
                channelData.Add(i, frame.GetChannelData(i));
            }
            return channelData;
        }

        /// <summary>
        /// Formats the <see cref="AudioFrame"/> as a string.
        /// </summary>
        /// <param name="frame">The <see cref="AudioFrame"/> to format.</param>
        /// <param name="cultureInfo">The culture info to use for formatting.</param>
        /// <returns>A string representation of the <see cref="AudioFrame"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="frame"/> or <paramref name="cultureInfo"/> is null.</exception>
        public static string Format(this AudioFrame frame, CultureInfo cultureInfo)
        {
            ArgumentNullException.ThrowIfNull(frame);
            ArgumentNullException.ThrowIfNull(cultureInfo);

            return $"AudioFrame(Id: {frame.Id}, Timestamp: {frame.Timestamp}, DurationSeconds: {frame.DurationSeconds:F3}, PeakAmplitude: {frame.PeakAmplitude:F3}, RmsEnergy: {frame.RmsEnergy:F3})";
        }
    }
}
