// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using FluentAssertions;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Services;
using Xunit;

namespace NAudioVisualizer.Tests;

/// <summary>
/// Tests for the SpectrogramAnalyzer class.
/// </summary>
public class SpectrogramAnalyzerTests
{
    private readonly SpectrogramAnalyzer _analyzer;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpectrogramAnalyzerTests"/> class.
    /// </summary>
    public SpectrogramAnalyzerTests()
    {
        _analyzer = new SpectrogramAnalyzer();
    }

    /// <summary>
    /// Verifies that SetBufferSize rejects non-positive sizes.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SetBufferSize_NonPositiveSize_ThrowsArgumentException(int maxFrames)
    {
        // Act
        Action act = () => _analyzer.SetBufferSize(maxFrames);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("maxFrames");
    }

    /// <summary>
    /// Verifies that adding spectrum frames respects the configured maximum buffer size.
    /// </summary>
    [Fact]
    public void AddSpectrumFrame_MoreThanMaximum_KeepsBufferAtMaximumSize()
    {
        // Arrange
        _analyzer.SetBufferSize(2);

        // Act
        _analyzer.AddSpectrumFrame(CreateSpectrum(1f));
        _analyzer.AddSpectrumFrame(CreateSpectrum(2f));
        _analyzer.AddSpectrumFrame(CreateSpectrum(3f));

        // Assert
        _analyzer.GetBufferFrameCount().Should().Be(2);
    }

    /// <summary>
    /// Verifies that ClearBuffer removes all buffered spectrum frames.
    /// </summary>
    [Fact]
    public void ClearBuffer_WithBufferedFrames_EmptiesBuffer()
    {
        // Arrange
        _analyzer.AddSpectrumFrame(CreateSpectrum(1f));
        _analyzer.AddSpectrumFrame(CreateSpectrum(2f));

        // Act
        _analyzer.ClearBuffer();

        // Assert
        _analyzer.GetBufferFrameCount().Should().Be(0);
    }

    /// <summary>
    /// Verifies that GetCurrentSpectrogram reflects whether frames are buffered.
    /// </summary>
    [Fact]
    public void GetCurrentSpectrogram_BeforeAndAfterAddingFrame_ReturnsExpectedResult()
    {
        // Assert
        _analyzer.GetCurrentSpectrogram().Should().BeNull();

        // Act
        _analyzer.AddSpectrumFrame(CreateSpectrum(1f));

        // Assert
        _analyzer.GetCurrentSpectrogram().Should().NotBeNull();
    }

    /// <summary>
    /// Verifies that identical consecutive frames have zero spectral flux.
    /// </summary>
    [Fact]
    public void CalculateSpectralFlux_IdenticalConsecutiveFrames_ReturnsZeroFlux()
    {
        // Arrange
        _analyzer.AddSpectrumFrame(CreateSpectrum(1f));
        _analyzer.AddSpectrumFrame(CreateSpectrum(1f));
        var spectrogram = _analyzer.GetCurrentSpectrogram();

        // Act
        var flux = _analyzer.CalculateSpectralFlux(spectrogram!);

        // Assert
        flux.Should().Equal(0f, 0f);
    }

    /// <summary>
    /// Verifies that increasing magnitudes produce positive spectral flux.
    /// </summary>
    [Fact]
    public void CalculateSpectralFlux_IncreasingMagnitudes_ReturnsPositiveFlux()
    {
        // Arrange
        _analyzer.AddSpectrumFrame(CreateSpectrum(1f));
        _analyzer.AddSpectrumFrame(CreateSpectrum(2f));
        var spectrogram = _analyzer.GetCurrentSpectrogram();

        // Act
        var flux = _analyzer.CalculateSpectralFlux(spectrogram!);

        // Assert
        flux[1].Should().BePositive();
    }

    /// <summary>
    /// Verifies that a spike in spectral magnitude is detected as a transient.
    /// </summary>
    [Fact]
    public void DetectTransients_SpikeFrame_ReturnsSpikeFrameIndex()
    {
        // Arrange
        _analyzer.AddSpectrumFrame(CreateSpectrum(0f));
        _analyzer.AddSpectrumFrame(CreateSpectrum(10f));
        _analyzer.AddSpectrumFrame(CreateSpectrum(10f));
        var spectrogram = _analyzer.GetCurrentSpectrogram();

        // Act
        var transients = _analyzer.DetectTransients(spectrogram!);

        // Assert
        transients.Should().ContainSingle().Which.Should().Be(1);
    }

    private static SpectrumData CreateSpectrum(float magnitude)
    {
        var magnitudes = new[] { magnitude, magnitude, magnitude, magnitude };
        var frequencies = new[] { 0f, 6000f, 12000f, 18000f };
        return new SpectrumData(magnitudes, frequencies, 48000, 8);
    }
}
