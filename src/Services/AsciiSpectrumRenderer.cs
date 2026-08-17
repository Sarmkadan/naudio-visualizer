using System;
using System.Text;
using NAudioVisualizer.Configuration;

namespace NaudioVisualizer.Services
{
    /// <summary>
    /// Renders a float spectrum frame as an ASCII bar chart.
    /// </summary>
    public sealed class AsciiSpectrumRenderer
    {
        private readonly int _width;
        private readonly int _height;
        private readonly bool _logScale;

        // Peak‑hold support
        private readonly bool _peakHoldEnabled;
        private readonly float _peakFallRateDbPerSec;
        private readonly float[] _peakValues;
        private DateTime _lastRenderTime;
        private const char PeakChar = '^';

        /// <summary>
        /// Characters used for drawing bars from bottom to top.
        /// </summary>
        private static readonly char[] BarChars = new[]
        {
            ' ', '▁', '▂', '▃', '▄', '▅', '▆', '▇', '█'
        };

        /// <summary>
        /// Creates a new renderer.
        /// </summary>
        /// <param name="width">Number of columns in the output.</param>
        /// <param name="height">Number of rows in the output.</param>
        /// <param name="logScale">If true, values are log‑scaled before rendering.</param>
        /// <param name="config">Optional configuration manager for peak‑hold settings.</param>
        public AsciiSpectrumRenderer(int width = 80, int height = 20, bool logScale = false, ConfigurationManager? config = null)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            _width = width;
            _height = height;
            _logScale = logScale;

            // Initialise peak‑hold state
            _peakHoldEnabled = config?.GetValue<bool>("visualization.peakHoldEnabled", false) ?? false;
            _peakFallRateDbPerSec = config?.GetValue<float>("visualization.peakHoldFallRateDbPerSec", 10f) ?? 10f;
            _peakValues = new float[_width];
            _lastRenderTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Renders the given spectrum frame as an ASCII bar chart.
        /// </summary>
        /// <param name="spectrumFrame">Array of spectrum magnitudes.</param>
        /// <returns>String containing the rendered chart.</returns>
        public string Render(float[] spectrumFrame)
        {
            if (spectrumFrame == null) throw new ArgumentNullException(nameof(spectrumFrame));
            if (spectrumFrame.Length == 0) return string.Empty;

            // Determine current console size and clamp rendering dimensions
            int effectiveWidth = Math.Min(_width, Console.WindowWidth);
            int effectiveHeight = Math.Min(_height, Console.WindowHeight);

            // Map the spectrum to the desired width (clamped)
            float[] mapped = MapToWidth(spectrumFrame, effectiveWidth);

            // Find maximum value for scaling
            float max = 0f;
            foreach (var v in mapped)
            {
                float val = _logScale ? LogScale(v) : v;
                if (val > max) max = val;
            }

            // Avoid division by zero
            if (max <= 0f) max = 1f;

            // Update peak‑hold values if enabled
            if (_peakHoldEnabled)
                UpdatePeakValues(mapped, max);

            // Build the chart line by line
            var sb = new StringBuilder();
            for (int row = 0; row < effectiveHeight; row++)
            {
                int level = effectiveHeight - row; // 1‑based level from bottom
                for (int col = 0; col < effectiveWidth; col++)
                {
                    float val = _logScale ? LogScale(mapped[col]) : mapped[col];
                    int barHeight = (int)Math.Round((val / max) * effectiveHeight);
                    int peakHeight = (int)Math.Round((_peakValues[col] / max) * effectiveHeight);

                    char ch;
                    if (_peakHoldEnabled && peakHeight >= level && level > barHeight)
                    {
                        // Peak marker sits above the current bar
                        ch = PeakChar;
                    }
                    else if (barHeight >= level)
                    {
                        ch = BarChars[Math.Min(barHeight, BarChars.Length - 1)];
                    }
                    else
                    {
                        ch = ' ';
                    }

                    sb.Append(ch);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Updates the stored peak values, applying decay based on elapsed time.
        /// </summary>
        private void UpdatePeakValues(float[] currentValues, float max)
        {
            DateTime now = DateTime.UtcNow;
            double elapsedSec = (now - _lastRenderTime).TotalSeconds;
            _lastRenderTime = now;

            // Convert fall rate from dB/s to a linear decay factor
            double decayFactor = Math.Pow(10.0, -_peakFallRateDbPerSec * elapsedSec / 20.0);

            for (int i = 0; i < currentValues.Length; i++)
            {
                float current = currentValues[i];
                if (current > _peakValues[i])
                {
                    _peakValues[i] = current;
                }
                else
                {
                    _peakValues[i] = (float)(_peakValues[i] * decayFactor);
                }

                // Clamp to zero to avoid negative values due to rounding
                if (_peakValues[i] < 0f) _peakValues[i] = 0f;
            }
        }

        /// <summary>
        /// Maps the input array to the target width by averaging blocks.
        /// </summary>
        private static float[] MapToWidth(float[] input, int targetWidth)
        {
            if (input.Length <= targetWidth)
            {
                // Pad with zeros if needed
                var padded = new float[targetWidth];
                Array.Copy(input, padded, input.Length);
                return padded;
            }

            float[] result = new float[targetWidth];
            int blockSize = input.Length / targetWidth;
            int remainder = input.Length % targetWidth;
            int index = 0;

            for (int i = 0; i < targetWidth; i++)
            {
                int count = blockSize + (i < remainder ? 1 : 0);
                float sum = 0f;
                for (int j = 0; j < count; j++)
                {
                    sum += input[index++];
                }
                result[i] = sum / count;
            }

            return result;
        }

        /// <summary>
        /// Logarithmic scaling (base 10) with a small offset to avoid log(0).
        /// </summary>
        private static float LogScale(float value)
        {
            const float offset = 1e-6f;
            return (float)Math.Log10(value + offset);
        }
    }
}
