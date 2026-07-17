using System;
using System.Collections.Generic;
using System.Globalization;

namespace NAudioVisualizer.Domain.Models
{
    /// <summary>
    /// Provides extension methods for <see cref="AudioFrame"/>.
    /// </summary>
    public static class AudioFrameExtensions
    {
        /// <summary>
        /// Calculates the average amplitude of an <see cref="AudioFrame"/>.
        /// </summary>
        /// <param name="frame">The <see cref="AudioFrame"/> to calculate the average amplitude for.</param>
        /// <returns>The average amplitude, calculated as the mean of absolute sample values.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="frame"/> is <see langword="null"/>.</exception>
        public static float CalculateAverageAmplitude(this AudioFrame frame)
        {
            ArgumentNullException.ThrowIfNull(frame);

            if (frame.Samples.Length == 0)
            {
                return 0f;
            }

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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="frame"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="frame"/>.ChannelCount is less than 1.</exception>
        public static IReadOnlyDictionary<int, float[]> GetChannelDataDictionary(this AudioFrame frame)
        {
            ArgumentNullException.ThrowIfNull(frame);

            if (frame.ChannelCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(frame), "ChannelCount must be greater than 0.");
            }

            var channelData = new Dictionary<int, float[]>(frame.ChannelCount);
            for (int i = 0; i < frame.ChannelCount; i++)
            {
                channelData[i] = frame.GetChannelData(i);
            }

            return channelData;
        }

        /// <summary>
        /// Formats the <see cref="AudioFrame"/> as a string using the invariant culture.
        /// </summary>
        /// <param name="frame">The <see cref="AudioFrame"/> to format.</param>
        /// <returns>A string representation of the <see cref="AudioFrame"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="frame"/> is <see langword="null"/>.</exception>
        public static string Format(this AudioFrame frame)
        {
            ArgumentNullException.ThrowIfNull(frame);

            return string.Create(
                CultureInfo.InvariantCulture,
                $"AudioFrame(Id: {frame.Id}, Timestamp: {frame.Timestamp:O}, DurationSeconds: {frame.DurationSeconds:F3}, PeakAmplitude: {frame.PeakAmplitude:F3}, RmsEnergy: {frame.RmsEnergy:F3}, ChannelCount: {frame.ChannelCount}, SampleRate: {frame.SampleRate})"
            );
        }

        /// <summary>
        /// Formats the <see cref="AudioFrame"/> as a string using the specified culture.
        /// </summary>
        /// <param name="frame">The <see cref="AudioFrame"/> to format.</param>
        /// <param name="cultureInfo">The culture info to use for formatting.</param>
        /// <returns>A string representation of the <see cref="AudioFrame"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="frame"/> is <see langword="null"/>.
        /// Thrown if <paramref name="cultureInfo"/> is <see langword="null"/>.
        /// </exception>
        public static string Format(this AudioFrame frame, CultureInfo cultureInfo)
        {
            ArgumentNullException.ThrowIfNull(frame);
            ArgumentNullException.ThrowIfNull(cultureInfo);

            return string.Create(
                cultureInfo,
                $"AudioFrame(Id: {frame.Id}, Timestamp: {frame.Timestamp:O}, DurationSeconds: {frame.DurationSeconds:F3}, PeakAmplitude: {frame.PeakAmplitude:F3}, RmsEnergy: {frame.RmsEnergy:F3}, ChannelCount: {frame.ChannelCount}, SampleRate: {frame.SampleRate})"
            );
        }
    }
}
