using System;
using System.Text;

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
        public AsciiSpectrumRenderer(int width = 80, int height = 20, bool logScale = false)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            _width = width;
            _height = height;
            _logScale = logScale;
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

            // Map the spectrum to the desired width
            float[] mapped = MapToWidth(spectrumFrame, _width);

            // Find maximum value for scaling
            float max = 0f;
            foreach (var v in mapped)
            {
                float val = _logScale ? LogScale(v) : v;
                if (val > max) max = val;
            }

            // Avoid division by zero
            if (max <= 0f) max = 1f;

            // Build the chart line by line
            var sb = new StringBuilder();
            for (int row = 0; row < _height; row++)
            {
                int level = _height - row; // 1-based level from bottom
                for (int col = 0; col < _width; col++)
                {
                    float val = _logScale ? LogScale(mapped[col]) : mapped[col];
                    int barHeight = (int)Math.Round((val / max) * _height);
                    char ch = barHeight >= level ? BarChars[Math.Min(barHeight, BarChars.Length - 1)] : ' ';
                    sb.Append(ch);
                }
                sb.AppendLine();
            }

            return sb.ToString();
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
