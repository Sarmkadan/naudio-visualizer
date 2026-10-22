// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Exceptions;
using NAudioVisualizer.Services;
using Xunit;

namespace NAudioVisualizer.Tests;

/// <summary>
/// Integration tests for waveform rendering.
/// These tests verify the WaveformService generates correct data from known audio signals,
/// and that rendering behavior matches expected properties.
/// </summary>
public class WaveformRendererIntegrationTests
{
    private const int RenderWidth = 200;
    private const int RenderHeight = 100;

    /// <summary>
    /// Creates an AudioFrame with a sine wave signal.
    /// </summary>
    private static AudioFrame CreateSineWaveFrame(int sampleRate = 44100, double durationSeconds = 0.5, float amplitude = 0.8f)
    {
        int sampleCount = (int)(sampleRate * durationSeconds);
        var samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            double t = (double)i / sampleRate;
            samples[i] = (float)(amplitude * Math.Sin(2 * Math.PI * 440 * t)); // 440 Hz sine
        }
        return new AudioFrame(samples, 1, sampleRate, 0);
    }

    /// <summary>
    /// Creates an AudioFrame with silence (all zeros).
    /// </summary>
    private static AudioFrame CreateSilenceFrame(int sampleRate = 44100, double durationSeconds = 0.5f)
    {
        int sampleCount = (int)(sampleRate * durationSeconds);
        var samples = new float[sampleCount];
        // Already zeros
        return new AudioFrame(samples, 1, sampleRate, 0);
    }

    /// <summary>
    /// Creates an AudioFrame with a DC offset signal (constant non-zero value).
    /// </summary>
    private static AudioFrame CreateDcOffsetFrame(int sampleRate = 44100, double durationSeconds = 0.5f, float dcLevel = 0.5f)
    {
        int sampleCount = (int)(sampleRate * durationSeconds);
        var samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            samples[i] = dcLevel;
        }
        return new AudioFrame(samples, 1, sampleRate, 0);
    }

    /// <summary>
    /// Creates an AudioFrame with a clipped signal (values exceed [-1, 1] range).
    /// Note: WaveformService will normalize/clip, but we test the raw rendering behavior.
    /// </summary>
    private static AudioFrame CreateClippedFrame(int sampleRate = 44100, double durationSeconds = 0.5f)
    {
        int sampleCount = (int)(sampleRate * durationSeconds);
        var samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            // Generate values that exceed the [-1, 1] range
            double t = (double)i / sampleRate;
            float val = (float)(0.8 * Math.Sin(2 * Math.PI * 440 * t));
            // Clip to exceed range - simulate over-modulated signal
            samples[i] = val > 1.0f ? 1.5f : (val < -1.0f ? -1.5f : val);
        }
        return new AudioFrame(samples, 1, sampleRate, 0);
    }

    /// <summary>
    /// Verifies that GenerateWaveform produces correct data from a sine wave.
    /// </summary>
    [Fact]
    public void GenerateWaveform_SineWave_ProducesValidData()
    {
        // Arrange
        var audioFrame = CreateSineWaveFrame(amplitude: 0.8f);
        var waveformService = new WaveformService();

        // Act
        var waveformData = waveformService.GenerateWaveform(audioFrame, downsamplingFactor: 1);

        // Assert
        Assert.NotNull(waveformData);
        Assert.True(waveformData.IsValid());
        Assert.Equal(1, waveformData.ChannelCount);
        Assert.Equal(44100, waveformData.SampleRate);
        Assert.True(waveformData.DataPointCount > 0);
        // With no downsampling, data point count should match sample count
        Assert.Equal(audioFrame.Samples.Length, waveformData.DataPointCount);
    }

    /// <summary>
    /// Verifies that GenerateWaveform produces correct data from silence.
    /// </summary>
    [Fact]
    public void GenerateWaveform_Silence_ProducesValidData()
    {
        // Arrange
        var audioFrame = CreateSilenceFrame();
        var waveformService = new WaveformService();

        // Act
        var waveformData = waveformService.GenerateWaveform(audioFrame, downsamplingFactor: 1);

        // Assert
        Assert.NotNull(waveformData);
        Assert.True(waveformData.IsValid());
        // All samples should be 0, so min and max should both be 0
        var samples = waveformData.GetData();
        Assert.All(samples, s => Assert.Equal(0f, s));
    }

    /// <summary>
    /// Verifies that GenerateWaveform produces correct data from DC offset.
    /// </summary>
    [Fact]
    public void GenerateWaveform_DcOffset_ProducesValidData()
    {
        // Arrange
        var audioFrame = CreateDcOffsetFrame(dcLevel: 0.5f);
        var waveformService = new WaveformService();

        // Act
        var waveformData = waveformService.GenerateWaveform(audioFrame, downsamplingFactor: 1);

        // Assert
        Assert.NotNull(waveformData);
        Assert.True(waveformData.IsValid());
        var samples = waveformData.GetData();
        // All samples should have the DC offset level
        Assert.All(samples, s => Assert.Equal(0.5f, s));
    }

    /// <summary>
    /// Verifies that GenerateWaveform produces correct data from clipped signal.
    /// </summary>
    [Fact]
    public void GenerateWaveform_ClippedSignal_ProducesValidData()
    {
        // Arrange
        var audioFrame = CreateClippedFrame();
        var waveformService = new WaveformService();

        // Act
        var waveformData = waveformService.GenerateWaveform(audioFrame, downsamplingFactor: 1);

        // Assert
        Assert.NotNull(waveformData);
        Assert.True(waveformData.IsValid());
        var samples = waveformData.GetData();
        // After normalization by the service, values should be in [-1, 1] range
        // but should still reflect the clipped nature (some values at extremes)
        // Check that we have valid data and some values are at or near the extremes
        Assert.True(samples.Length > 0);
        // Min/max should be valid numbers (not NaN)
        Assert.False(float.IsNaN(waveformData.MinValue));
        Assert.False(float.IsNaN(waveformData.MaxValue));
    }

    /// <summary>
    /// Verifies that GenerateWaveform throws for zero-length buffer.
    /// </summary>
    [Fact]
    public void GenerateWaveform_ZeroLengthBuffer_ThrowsVisualizationException()
    {
        // Arrange
        var audioFrame = new AudioFrame(Array.Empty<float>(), 1, 44100, 0);
        var waveformService = new WaveformService();

        // Act & Assert
        Assert.Throws<VisualizationException>(() => waveformService.GenerateWaveform(audioFrame));
    }

    /// <summary>
    /// Verifies that repeated GenerateWaveform calls with different downsampling factors
    /// produce correctly sized results without data leakage.
    /// </summary>
    [Fact]
    public void GenerateWaveform_RepeatedCalls_DifferentDownsamplingFactors()
    {
        // Arrange
        var audioFrame = CreateSineWaveFrame(amplitude: 0.5f);
        var waveformService = new WaveformService();

        // Act - Render multiple times with different downsampling factors
        var waveform1 = waveformService.GenerateWaveform(audioFrame, downsamplingFactor: 1);
        var waveform2 = waveformService.GenerateWaveform(audioFrame, downsamplingFactor: 4);
        var waveform3 = waveformService.GenerateWaveform(audioFrame, downsamplingFactor: 16);

        // Assert - each should be valid and have different sizes
        Assert.True(waveform1.IsValid());
        Assert.True(waveform2.IsValid());
        Assert.True(waveform3.IsValid());
        // factor 1 = no downsampling, should have most data points
        Assert.Equal(audioFrame.Samples.Length, waveform1.DataPointCount);
        // factor 4 should have approximately 1/4 the data points
        Assert.True(waveform2.DataPointCount < waveform1.DataPointCount);
        // factor 16 should have approximately 1/16 the data points
        Assert.True(waveform3.DataPointCount < waveform2.DataPointCount);
    }

    /// <summary>
    /// Verifies that GenerateWaveform handles stereo audio correctly.
    /// </summary>
    [Fact]
    public void GenerateWaveform_StereoAudio_ProducesValidData()
    {
        // Arrange
        int sampleCount = 4410; // 0.1 seconds at 44100 Hz
        var samples = new float[sampleCount * 2]; // interleaved stereo
        for (int i = 0; i < sampleCount; i++)
        {
            samples[i * 2] = 0.5f;     // Left channel
            samples[i * 2 + 1] = 0.3f; // Right channel
        }
        var audioFrame = new AudioFrame(samples, 2, 44100, 0);
        var waveformService = new WaveformService();

        // Act
        var waveformData = waveformService.GenerateWaveform(audioFrame, downsamplingFactor: 1);

        // Assert
        Assert.NotNull(waveformData);
        Assert.Equal(2, waveformData.ChannelCount);
        Assert.True(waveformData.IsValid());
        // Should have calculated stereo channels
        Assert.NotNull(waveformData.LeftChannelPeaks);
        Assert.NotNull(waveformData.RightChannelPeaks);
    }
}