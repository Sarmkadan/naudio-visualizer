using System;
using System.Collections.Generic;
using System.Linq;

namespace NAudioVisualizer.Domain.Models
{
    public static class SpectrogramDataExtensions
    {
        /// <summary>
        /// Creates a new spectrogram with normalized frequency bins, ensuring consistent scaling across different audio clips.
        /// </summary>
        /// <param name="spectrogram">The source spectrogram data</param>
        /// <param name="targetMaxDb">Target maximum decibel level for normalization (default: -3dB)</param>
        /// <returns>A new spectrogram with normalized frequency bins</returns>
        public static SpectrogramData NormalizeFrequencyBins(this SpectrogramData spectrogram, float targetMaxDb = -3.0f)
        {
            if (spectrogram == null)
                throw new ArgumentNullException(nameof(spectrogram));

            if (!spectrogram.IsValid())
                throw new InvalidOperationException("Cannot normalize invalid spectrogram data");

            // Create normalized 2D array
            var normalizedMatrix = new float[spectrogram.TimeFrames][];
            float maxValue = float.MinValue;
            float minValue = float.MaxValue;

            for (int t = 0; t < spectrogram.TimeFrames; t++)
            {
                var frame = spectrogram.GetTimeFrame(t);
                var normalizedFrame = new float[spectrogram.FrequencyBins];

                for (int f = 0; f < spectrogram.FrequencyBins; f++)
                {
                    float value = frame[f];
                    normalizedFrame[f] = value;

                    if (value > maxValue) maxValue = value;
                    if (value < minValue) minValue = value;
                }

                normalizedMatrix[t] = normalizedFrame;
            }

            // Apply normalization
            var range = maxValue - minValue;
            if (range > float.Epsilon)
            {
                for (int t = 0; t < spectrogram.TimeFrames; t++)
                {
                    for (int f = 0; f < spectrogram.FrequencyBins; f++)
                    {
                        float value = normalizedMatrix[t][f];
                        // Convert to dB scale and normalize
                        float dbValue = 20 * (float)Math.Log10(Math.Max(float.Epsilon, value));
                        float normalizedDb = (dbValue - minValue) / range * targetMaxDb;
                        normalizedMatrix[t][f] = (float)Math.Pow(10, normalizedDb / 20);
                    }
                }
            }

            return new SpectrogramData(normalizedMatrix, spectrogram.SampleRate, spectrogram.FftSize, spectrogram.HopSize)
            {
                ColormapType = spectrogram.ColormapType
            };
        }

        /// <summary>
        /// Extracts a rectangular region from the spectrogram, useful for focusing on specific frequency ranges or time segments.
        /// </summary>
        /// <param name="spectrogram">The source spectrogram data</param>
        /// <param name="startFrequencyBin">Starting frequency bin index (0-based)</param>
        /// <param name="frequencyBinCount">Number of frequency bins to extract</param>
        /// <param name="startTimeFrame">Starting time frame index (0-based)</param>
        /// <param name="timeFrameCount">Number of time frames to extract</param>
        /// <returns>A new spectrogram containing only the specified region</returns>
        public static SpectrogramData ExtractRegion(this SpectrogramData spectrogram, int startFrequencyBin, int frequencyBinCount, int startTimeFrame, int timeFrameCount)
        {
            if (spectrogram == null)
                throw new ArgumentNullException(nameof(spectrogram));

            if (!spectrogram.IsValid())
                throw new InvalidOperationException("Cannot extract region from invalid spectrogram data");

            // Validate parameters
            startFrequencyBin = Math.Clamp(startFrequencyBin, 0, spectrogram.FrequencyBins - 1);
            frequencyBinCount = Math.Clamp(frequencyBinCount, 1, spectrogram.FrequencyBins - startFrequencyBin);
            startTimeFrame = Math.Clamp(startTimeFrame, 0, spectrogram.TimeFrames - 1);
            timeFrameCount = Math.Clamp(timeFrameCount, 1, spectrogram.TimeFrames - startTimeFrame);

            // Create region matrix
            var regionMatrix = new float[timeFrameCount][];

            for (int t = 0; t < timeFrameCount; t++)
            {
                var sourceFrame = spectrogram.GetTimeFrame(startTimeFrame + t);
                var regionFrame = new float[frequencyBinCount];

                for (int f = 0; f < frequencyBinCount; f++)
                {
                    regionFrame[f] = sourceFrame[startFrequencyBin + f];
                }

                regionMatrix[t] = regionFrame;
            }

            return new SpectrogramData(regionMatrix, spectrogram.SampleRate, spectrogram.FftSize, spectrogram.HopSize)
            {
                ColormapType = spectrogram.ColormapType
            };
        }

        /// <summary>
        /// Applies a frequency mask to the spectrogram, zeroing out specified frequency bins.
        /// Useful for filtering out unwanted frequency ranges (e.g., DC offset, hum, or specific noise bands).
        /// </summary>
        /// <param name="spectrogram">The source spectrogram data</param>
        /// <param name="frequencyBinIndices">Array of frequency bin indices to mask (set to zero)</param>
        /// <returns>A new spectrogram with the specified frequency bins zeroed out</returns>
        public static SpectrogramData ApplyFrequencyMask(this SpectrogramData spectrogram, int[] frequencyBinIndices)
        {
            if (spectrogram == null)
                throw new ArgumentNullException(nameof(spectrogram));

            if (frequencyBinIndices == null)
                throw new ArgumentNullException(nameof(frequencyBinIndices));

            if (!spectrogram.IsValid())
                throw new InvalidOperationException("Cannot apply mask to invalid spectrogram data");

            // Create masked matrix
            var maskedMatrix = new float[spectrogram.TimeFrames][];

            // Create a hash set for O(1) lookups
            var maskSet = new HashSet<int>(frequencyBinIndices);

            for (int t = 0; t < spectrogram.TimeFrames; t++)
            {
                var sourceFrame = spectrogram.GetTimeFrame(t);
                var maskedFrame = new float[spectrogram.FrequencyBins];

                for (int f = 0; f < spectrogram.FrequencyBins; f++)
                {
                    maskedFrame[f] = maskSet.Contains(f) ? 0f : sourceFrame[f];
                }

                maskedMatrix[t] = maskedFrame;
            }

            return new SpectrogramData(maskedMatrix, spectrogram.SampleRate, spectrogram.FftSize, spectrogram.HopSize)
            {
                ColormapType = spectrogram.ColormapType
            };
        }

        /// <summary>
        /// Calculates the spectral centroid for each time frame, providing a measure of the "center of mass" of the spectrum.
        /// Useful for analyzing the dominant frequency content over time.
        /// </summary>
        /// <param name="spectrogram">The source spectrogram data</param>
        /// <returns>Array of spectral centroid values (in Hz) for each time frame</returns>
        public static float[] CalculateSpectralCentroids(this SpectrogramData spectrogram)
        {
            if (spectrogram == null)
                throw new ArgumentNullException(nameof(spectrogram));

            if (!spectrogram.IsValid())
                throw new InvalidOperationException("Cannot calculate centroids from invalid spectrogram data");

            var centroids = new float[spectrogram.TimeFrames];
            float binWidth = spectrogram.FrequencyResolution;

            for (int t = 0; t < spectrogram.TimeFrames; t++)
            {
                var frame = spectrogram.GetTimeFrame(t);
                float sumMagnitude = 0f;
                float weightedSum = 0f;

                for (int f = 0; f < spectrogram.FrequencyBins; f++)
                {
                    float magnitude = frame[f];
                    sumMagnitude += magnitude;
                    weightedSum += magnitude * (f * binWidth);
                }

                if (sumMagnitude > float.Epsilon)
                {
                    centroids[t] = weightedSum / sumMagnitude;
                }
                else
                {
                    centroids[t] = 0f;
                }
            }

            return centroids;
        }
    }
}
