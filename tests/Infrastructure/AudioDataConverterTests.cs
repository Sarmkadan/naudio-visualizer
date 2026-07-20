using System;
using NAudioVisualizer.Infrastructure;
using Xunit;

namespace NAudioVisualizer.Tests.Infrastructure
{
    public class AudioDataConverterTests
    {
        [Fact]
        public void FloatToInt16Pcm_And_Int16PcmToFloat_RoundTrip_ShouldPreserveValues()
        {
            // Arrange: a set of representative samples including min, max and zero
            float[] originalSamples = new[] { -1f, -0.5f, 0f, 0.5f, 1f };

            // Act
            byte[] pcmBytes = AudioDataConverter.FloatToInt16Pcm(originalSamples);
            float[] roundTripSamples = AudioDataConverter.Int16PcmToFloat(pcmBytes);

            // Assert
            Assert.Equal(originalSamples.Length, roundTripSamples.Length);

            // The conversion loses a tiny amount of precision (1/32768), so allow a small tolerance
            const float tolerance = 1f / 32768f;
            for (int i = 0; i < originalSamples.Length; i++)
            {
                Assert.InRange(roundTripSamples[i], originalSamples[i] - tolerance, originalSamples[i] + tolerance);
            }
        }

        [Fact]
        public void Int16PcmToFloat_OddLengthByteArray_ShouldIgnoreTrailingByte()
        {
            // Arrange: 3 bytes – the last byte does not form a complete 16‑bit sample
            byte[] oddLengthBytes = new byte[] { 0x01, 0x00, 0xFF };

            // Act
            float[] samples = AudioDataConverter.Int16PcmToFloat(oddLengthBytes);

            // Assert
            // Only the first two bytes form a valid sample (value = 1)
            Assert.Single(samples);
            Assert.Equal(1f / 32768f, samples[0], precision: 5);
        }

        [Fact]
        public void InterleaveChannels_TwoChannels_ShouldInterleaveCorrectly()
        {
            // Arrange
            float[] leftChannel  = new[] { 0.1f, 0.2f, 0.3f };
            float[] rightChannel = new[] { 0.4f, 0.5f, 0.6f };
            float[][] channels = new[] { leftChannel, rightChannel };

            // Act
            float[] interleaved = AudioDataConverter.InterleaveChannels(channels);

            // Assert
            float[] expected = new[] { 0.1f, 0.4f, 0.2f, 0.5f, 0.3f, 0.6f };
            Assert.Equal(expected, interleaved);
        }
    }
}
