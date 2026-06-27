// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using FluentAssertions;
using NAudioVisualizer.Services;
using Xunit;
using System;

namespace NAudioVisualizer.Tests;

public class WaveformServiceTests
{
    private readonly WaveformService _service;

    public WaveformServiceTests()
    {
        _service = new WaveformService();
    }

    [Fact]
    public void DownsampleSamples_ReturnsCorrectLengthAndAverages()
    {
        // Arrange
        var samples = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };
        int factor = 2;

        // Act
        var result = _service.DownsampleSamples(samples, factor);

        // Assert
        result.Should().HaveCount(2);
        result[0].Should().BeApproximately(0.15f, 0.0001f); // (0.1+0.2)/2
        result[1].Should().BeApproximately(0.35f, 0.0001f); // (0.3+0.4)/2
    }

    [Fact]
    public void CalculatePeakValues_ReturnsCorrectPeaks()
    {
        // Arrange
        var samples = new float[] { 0.1f, 0.8f, 0.2f, 0.5f };
        int peakCount = 2;

        // Act
        var result = _service.CalculatePeakValues(samples, peakCount);

        // Assert
        result.Should().HaveCount(2);
        result[0].Should().Be(0.8f); // Max of {0.1, 0.8}
        result[1].Should().Be(0.5f); // Max of {0.2, 0.5}
    }

    [Fact]
    public void CalculatePeakValues_InvalidPeakCount_ThrowsArgumentException()
    {
        // Arrange
        var samples = new float[] { 0.1f, 0.2f };
        int peakCount = 0;

        // Act
        Action act = () => _service.CalculatePeakValues(samples, peakCount);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Peak count must be positive*");
    }

    [Fact]
    public void ApplySmoothingFilter_SmoothesSignal()
    {
        // Arrange
        var samples = new float[] { 0.0f, 1.0f, 0.0f };
        int windowSize = 3;

        // Act
        var result = _service.ApplySmoothingFilter(samples, windowSize);

        // Assert
        result.Should().HaveCount(3);
        // (0.0+1.0)/2 = 0.5
        // (0.0+1.0+0.0)/3 = 0.333
        // (1.0+0.0)/2 = 0.5
        result[0].Should().BeApproximately(0.5f, 0.001f);
        result[1].Should().BeApproximately(0.333f, 0.001f);
        result[2].Should().BeApproximately(0.5f, 0.001f);
    }

    [Fact]
    public void CalculateFrameEnergy_ReturnsCorrectRms()
    {
        // Arrange
        var samples = new float[] { 0.5f, 0.5f, 0.5f, 0.5f };
        int frameCount = 2;

        // Act
        var result = _service.CalculateFrameEnergy(samples, frameCount);

        // Assert
        result.Should().HaveCount(2);
        // Segment 1: {0.5, 0.5}, RMS = sqrt((0.25+0.25)/2) = sqrt(0.25) = 0.5
        result[0].Should().BeApproximately(0.5f, 0.0001f);
        result[1].Should().BeApproximately(0.5f, 0.0001f);
    }

    [Fact]
    public void CountZeroCrossings_ReturnsCorrectCount()
    {
        // Arrange
        var samples = new float[] { -0.1f, 0.1f, -0.1f, 0.1f };

        // Act
        var count = _service.CountZeroCrossings(samples);

        // Assert
        count.Should().Be(3); // -0.1 -> 0.1, 0.1 -> -0.1, -0.1 -> 0.1
    }
}
