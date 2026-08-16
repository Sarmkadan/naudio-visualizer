using System;
using System.Linq;

namespace NaudioVisualizer.Utilities
{
    /// <summary>
    /// Extension methods for working with raw audio buffers represented as <c>float[]</c>.
    /// </summary>
    public static class AudioBufferExtensions
    {
        private const float MinAmplitude = 1e-12f; // Prevent log(0)

        /// <summary>
        /// Returns the peak amplitude of the buffer expressed in decibels (dBFS).
        /// </summary>
        /// <param name="buffer">The audio buffer.</param>
        /// <returns>Peak level in dB.</returns>
        public static float PeakDb(this float[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (buffer.Length == 0) return float.NegativeInfinity;

            float peak = buffer.Max(v => Math.Abs(v));
            peak = Math.Max(peak, MinAmplitude);
            return 20f * (float)Math.Log10(peak);
        }

        /// <summary>
        /// Returns the RMS (root‑mean‑square) level of the buffer expressed in decibels (dBFS).
        /// </summary>
        /// <param name="buffer">The audio buffer.</param>
        /// <returns>RMS level in dB.</returns>
        public static float RmsDb(this float[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (buffer.Length == 0) return float.NegativeInfinity;

            double sumSquares = 0.0;
            foreach (var sample in buffer)
            {
                sumSquares += sample * sample;
            }

            double rms = Math.Sqrt(sumSquares / buffer.Length);
            rms = Math.Max(rms, MinAmplitude);
            return 20f * (float)Math.Log10(rms);
        }

        /// <summary>
        /// Normalises the buffer in‑place so that its peak amplitude matches the specified target.
        /// </summary>
        /// <param name="buffer">The audio buffer to normalise.</param>
        /// <param name="targetPeak">The desired peak amplitude (linear, not dB).</param>
        public static void NormalizeInPlace(this float[] buffer, float targetPeak)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (targetPeak <= 0f) throw new ArgumentOutOfRangeException(nameof(targetPeak), "Target peak must be positive.");

            if (buffer.Length == 0) return;

            float currentPeak = buffer.Max(v => Math.Abs(v));
            if (currentPeak <= 0f) return; // Silent buffer – nothing to scale

            float scale = targetPeak / currentPeak;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] *= scale;
            }
        }
    }
}
