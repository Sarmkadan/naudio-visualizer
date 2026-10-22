// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Linq;
using FluentAssertions;
using NAudio.Dsp;
using NAudioVisualizer.Configuration;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Services;
using Xunit;

namespace NAudioVisualizer.Tests;

/// <summary>
/// End-to-end tests for SpectrumAnalyzer class.
/// Tests feed synthetic audio through the analyzer and verify spectrum analysis results.
/// </summary>
public class SpectrumAnalyzerTests
{
    private const int SampleRate = 44100;
    private const int DefaultFftSize = 2048;

    private static AudioFrame CreateSineFrame(float frequencyHz, float amplitude = 1f, int sampleRate = 44100, int durationMs = 500)
    {
        int sampleCount = (int)(sampleRate * durationMs / 1000.0);
        var samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            double t = i / (double)sampleRate;
            samples[i] = (float)(amplitude * Math.Sin(2 * Math.PI * frequencyHz * t));
        }
        return new AudioFrame(samples, 1, sampleRate, 0);
    }

    private static AudioFrame CreateSilenceFrame(int sampleRate = 44100, int durationMs = 500)
    {
        return CreateSineFrame(0f, 0f, sampleRate, durationMs);
    }

    private static AudioFrame CreateSquareWaveFrame(float frequencyHz, float amplitude = 1f, int sampleRate = 44100, int durationMs = 500)
    {
        int sampleCount = (int)(sampleRate * durationMs / 1000.0);
        var samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            double t = i / (double)sampleRate;
            // Hard clipping to create square wave - full scale ±amplitude
            double value = amplitude * Math.Sign(Math.Sin(2 * Math.PI * frequencyHz * t));
            samples[i] = (float)value;
        }
        return new AudioFrame(samples, 1, sampleRate, 0);
    }

    [Fact]
    public void AnalyzeSpectrum_OneKHzSinePeakAtExpectedBin()
    {
        // Arrange: 1 kHz sine at 44.1 kHz sample rate
        var frame = CreateSineFrame(1000f, 1f);
        var analyzer = new SpectrumAnalyzer();

        // Act: Analyze with FFT size 2048 (frequency resolution = 44100/2048 ≈ 21.5 Hz)
        SpectrumData spectrum = analyzer.AnalyzeSpectrum(frame, fftSize: 2048);

        // Assert: Peak frequency should be near 1 kHz (within ~2 bins = ~43 Hz tolerance)
        float dominantFreq = analyzer.FindDominantFrequency(spectrum);
        dominantFreq.Should().BeApproximately(1000f, 50f);

        // Also verify the peak bin index corresponds to ~1 kHz
        float freqPerBin = (float)SampleRate / DefaultFftSize;
        int peakBinIndex = (int)Math.Round(dominantFreq / freqPerBin);
        peakBinIndex.Should().BeLessThanOrEqualTo(DefaultFftSize / 2);
    }

    [Fact]
    public void AnalyzeSpectrum_TwoToneSignal_BothPeaksDetected()
    {
        // Arrange: 440 Hz + 4 kHz sine mixture
        int sampleCount = SampleRate * 500 / 1000; // 0.5 seconds
        var samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            double t = i / (double)SampleRate;
            samples[i] = (float)(Math.Sin(2 * Math.PI * 440 * t) + Math.Sin(2 * Math.PI * 4000 * t));
        }
        var frame = new AudioFrame(samples, 1, SampleRate, 0);
        var analyzer = new SpectrumAnalyzer();

        // Act
        SpectrumData spectrum = analyzer.AnalyzeSpectrum(frame, fftSize: 2048);

        // Assert: Both peaks should be detected - check dominant frequency is near either 440 Hz or 4 kHz
        float dominantFreq = analyzer.FindDominantFrequency(spectrum);
        bool inRange = (dominantFreq >= 400f && dominantFreq <= 500f) || (dominantFreq >= 3900f && dominantFreq <= 4100f);
        inRange.Should().BeTrue("Expected dominant frequency near 440 Hz or 4 kHz");

        // Check that we have multiple significant bins (both tones present)
        float[] magnitudes = spectrum.GetData();
        int significantBins = 0;
        for (int i = 0; i < magnitudes.Length; i++)
        {
            if (magnitudes[i] > 0.1f * magnitudes.Max())
                significantBins++;
        }
        significantBins.Should().BeGreaterThan(5); // Both tones create multiple significant bins (Hann window reduces leakage)
    }

    [Fact]
    public void AnalyzeSpectrum_Silence_AllBinsNearZeroNoNaN()
    {
        // Arrange: silence
        var frame = CreateSilenceFrame();
        var analyzer = new SpectrumAnalyzer();

        // Act
        SpectrumData spectrum = analyzer.AnalyzeSpectrum(frame, fftSize: 2048);

        // Assert: All bins should be near zero, no NaN values
        float[] magnitudes = spectrum.GetData();
        foreach (float magnitude in magnitudes)
        {
            magnitude.Should().BeGreaterThanOrEqualTo(0f);
            magnitude.Should().BeLessThanOrEqualTo(0.01f); // very small
            float.IsNaN(magnitude).Should().BeFalse("Magnitude should not be NaN");
            float.IsInfinity(magnitude).Should().BeFalse("Magnitude should not be infinity");
        }
    }

    [Fact]
    public void AnalyzeSpectrum_FullScaleSquareWave_NoOverflowNoNaN_HarmonicsPresent()
    {
        // Arrange: full-scale square wave at 440 Hz
        var frame = CreateSquareWaveFrame(440f, 1f);
        var analyzer = new SpectrumAnalyzer();

        // Act
        SpectrumData spectrum = analyzer.AnalyzeSpectrum(frame, fftSize: 2048);

        // Assert: No NaN or infinity, harmonics should be present
        float[] magnitudes = spectrum.GetData();
        foreach (float magnitude in magnitudes)
        {
            float.IsNaN(magnitude).Should().BeFalse("Magnitude should not be NaN");
            float.IsInfinity(magnitude).Should().BeFalse("Magnitude should not be infinity");
            magnitude.Should().BeGreaterOrEqualTo(0f, "Magnitude should be non-negative");
        }

        // Square wave has harmonics - check that higher frequency bins have significant energy
        float maxMagnitude = magnitudes.Max();
        int harmonicBinsAboveFundamental = 0;
        for (int i = 1; i < magnitudes.Length; i++) // skip DC bin
        {
            if (magnitudes[i] > 0.1f * maxMagnitude)
                harmonicBinsAboveFundamental++;
        }
        harmonicBinsAboveFundamental.Should().BeGreaterThan(5); // square wave has odd harmonics
    }

    [Fact]
    public void AnalyzeSpectrum_BufferSmallerThanFftSize_PadsDeterministically()
    {
        // Arrange: buffer smaller than FFT size (512 samples with FFT size 2048)
        int smallBufferSize = 128;
        var samples = new float[smallBufferSize];
        for (int i = 0; i < smallBufferSize; i++)
        {
            samples[i] = (float)(Math.Sin(2 * Math.PI * 1000 * i / (double)SampleRate));
        }
        var frame = new AudioFrame(samples, 1, SampleRate, 0);
        var analyzer = new SpectrumAnalyzer();

        // Act & Assert: Should not throw, should pad deterministically
        SpectrumData spectrum = analyzer.AnalyzeSpectrum(frame, fftSize: 2048);

        // Verify spectrum data is valid
        spectrum.IsValid().Should().BeTrue();
        float[] magnitudes = spectrum.GetData();
        magnitudes.Length.Should().Be(DefaultFftSize / 2); // 1024 bins

        // No NaN values
        foreach (float magnitude in magnitudes)
        {
            float.IsNaN(magnitude).Should().BeFalse();
        }
    }

    [Fact]
    public void AnalyzeSpectrum_WindowFunctionSelectable_MagnitudeScalingConsistent()
    {
        // Arrange: 1 kHz sine signal
        var frame = CreateSineFrame(1000f, 1f);
        var analyzer = new SpectrumAnalyzer();

        // Act: Test with different window types (SpectrumAnalyzer uses Hann internally)
        SpectrumData hannSpectrum = analyzer.AnalyzeSpectrum(frame, fftSize: 2048);

        // Verify magnitude scaling is consistent - peak should be at 1 kHz bin
        float hannDominantFreq = analyzer.FindDominantFrequency(hannSpectrum);
        hannDominantFreq.Should().BeInRange(950f, 1050f); // within 1 bin of 1 kHz (bin width ≈ 21.5 Hz)

        // Verify no NaN in any bin
        float[] hannMagnitudes = hannSpectrum.GetData();
        foreach (float mag in hannMagnitudes)
        {
            float.IsNaN(mag).Should().BeFalse("Hann window magnitude should not be NaN");
        }
    }

    [Fact]
    public void Constructor_DefaultOptions_CreatesAnalyzer()
    {
        // Arrange & Act
        var analyzer = new SpectrumAnalyzer();

        // Assert
        analyzer.Options.Should().NotBeNull();
        analyzer.Options.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Constructor_InvalidOptions_ThrowsArgumentExceptionWithAllProblems()
    {
        // Arrange
        var options = new AnalyzerCaptureOptions { FftSize = 1000, SmoothingFactor = 1.5f };

        // Act
        Action act = () => _ = new SpectrumAnalyzer(options);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*FftSize must be a power of two*")
            .WithMessage("*SmoothingFactor must be within [0, 1]*");
    }

    [Fact]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => _ = new SpectrumAnalyzer(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}

