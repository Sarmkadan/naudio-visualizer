#nullable enable

// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Linq;
using FluentAssertions;
using NAudioVisualizer.Configuration;
using Xunit;

namespace NAudioVisualizer.Tests;

/// <summary>
/// Unit tests for <see cref="AnalyzerCaptureOptions"/> and its <see cref="AnalyzerCaptureOptionsValidation"/> rules.
/// </summary>
public class AnalyzerCaptureOptionsTests
{
    private static AnalyzerCaptureOptions CreateValidOptions() => new();

    [Fact]
    public void Validate_DefaultOptions_ReturnsNoProblems()
    {
        // Arrange
        var options = CreateValidOptions();

        // Act
        var problems = options.Validate();

        // Assert
        problems.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2048)]
    [InlineData(1000)]
    [InlineData(2047)]
    [InlineData(3000)]
    public void Validate_FftSizeNotPowerOfTwo_ReportsFftSize(int fftSize)
    {
        // Arrange
        var options = CreateValidOptions();
        options.FftSize = fftSize;

        // Act
        var problems = options.Validate();

        // Assert
        problems.Should().ContainSingle()
            .Which.Should().Contain("FftSize must be a power of two");
    }

    [Theory]
    [InlineData(128)]
    [InlineData(32768)]
    public void Validate_FftSizePowerOfTwoOutsideRange_ReportsFftSizeRange(int fftSize)
    {
        // Arrange
        var options = CreateValidOptions();
        options.FftSize = fftSize;

        // Act
        var problems = options.Validate();

        // Assert
        problems.Should().ContainSingle()
            .Which.Should().Contain("FftSize must be between 256 and 16384");
    }

    [Theory]
    [InlineData(256)]
    [InlineData(2048)]
    [InlineData(16384)]
    public void Validate_FftSizePowerOfTwoInRange_IsValid(int fftSize)
    {
        // Arrange
        var options = CreateValidOptions();
        options.FftSize = fftSize;

        // Act
        var problems = options.Validate();

        // Assert
        problems.Should().BeEmpty();
    }

    [Theory]
    [InlineData(7999)]
    [InlineData(0)]
    [InlineData(-44100)]
    [InlineData(192001)]
    [InlineData(384000)]
    public void Validate_SampleRateOutsideRange_ReportsSampleRate(int sampleRate)
    {
        // Arrange
        var options = CreateValidOptions();
        options.SampleRate = sampleRate;

        // Act
        var problems = options.Validate();

        // Assert
        problems.Should().ContainSingle()
            .Which.Should().Contain("SampleRate must be between 8000 and 192000 Hz");
    }

    [Theory]
    [InlineData(8000)]
    [InlineData(44100)]
    [InlineData(192000)]
    public void Validate_SampleRateInRange_IsValid(int sampleRate)
    {
        // Arrange
        var options = CreateValidOptions();
        options.SampleRate = sampleRate;

        // Act
        var problems = options.Validate();

        // Assert
        problems.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(-1)]
    [InlineData(6)]
    public void Validate_ChannelsNotMonoOrStereo_ReportsChannels(int channels)
    {
        // Arrange
        var options = CreateValidOptions();
        options.Channels = channels;

        // Act
        var problems = options.Validate();

        // Assert
        problems.Should().ContainSingle()
            .Which.Should().Contain("Channels must be 1 (mono) or 2 (stereo)");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void Validate_ChannelsMonoOrStereo_IsValid(int channels)
    {
        // Arrange
        var options = CreateValidOptions();
        options.Channels = channels;

        // Act
        var problems = options.Validate();

        // Assert
        problems.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void Validate_BufferMillisecondsNotPositive_ReportsBufferMilliseconds(int bufferMilliseconds)
    {
        // Arrange
        var options = CreateValidOptions();
        options.BufferMilliseconds = bufferMilliseconds;

        // Act
        var problems = options.Validate();

        // Assert
        problems.Should().ContainSingle()
            .Which.Should().Contain("BufferMilliseconds must be greater than 0");
    }

    [Fact]
    public void Validate_BufferMillisecondsOne_IsValid()
    {
        // Arrange
        var options = CreateValidOptions();
        options.BufferMilliseconds = 1;

        // Act
        var problems = options.Validate();

        // Assert
        problems.Should().BeEmpty();
    }

    [Theory]
    [InlineData(-0.01f)]
    [InlineData(1.01f)]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    public void Validate_SmoothingFactorOutsideRange_ReportsSmoothingFactor(float smoothingFactor)
    {
        // Arrange
        var options = CreateValidOptions();
        options.SmoothingFactor = smoothingFactor;

        // Act
        var problems = options.Validate();

        // Assert
        problems.Should().ContainSingle()
            .Which.Should().Contain("SmoothingFactor must be within [0, 1]");
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(0.5f)]
    [InlineData(1f)]
    public void Validate_SmoothingFactorInRange_IsValid(float smoothingFactor)
    {
        // Arrange
        var options = CreateValidOptions();
        options.SmoothingFactor = smoothingFactor;

        // Act
        var problems = options.Validate();

        // Assert
        problems.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EveryRuleViolated_ReportsEveryProblem()
    {
        // Arrange
        var options = new AnalyzerCaptureOptions
        {
            FftSize = 1000,
            SampleRate = 4000,
            Channels = 6,
            BufferMilliseconds = 0,
            SmoothingFactor = 2f
        };

        // Act
        var problems = options.Validate();

        // Assert
        problems.Should().HaveCount(5);
        problems.Should().Contain(p => p.Contains("FftSize"));
        problems.Should().Contain(p => p.Contains("SampleRate"));
        problems.Should().Contain(p => p.Contains("Channels"));
        problems.Should().Contain(p => p.Contains("BufferMilliseconds"));
        problems.Should().Contain(p => p.Contains("SmoothingFactor"));
    }

    [Fact]
    public void Validate_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        AnalyzerCaptureOptions? options = null;

        // Act
        Action act = () => options.Validate();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsValid_NullOptions_ReturnsFalse()
    {
        // Arrange
        AnalyzerCaptureOptions? options = null;

        // Act
        var isValid = options.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void EnsureValid_ValidOptions_DoesNotThrow()
    {
        // Arrange
        var options = CreateValidOptions();

        // Act
        Action act = () => options.EnsureValid();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureValid_InvalidOptions_ThrowsArgumentExceptionListingAllProblems()
    {
        // Arrange
        var options = new AnalyzerCaptureOptions
        {
            FftSize = 1000,
            Channels = 3
        };

        // Act
        Action act = () => options.EnsureValid();

        // Assert
        var exception = act.Should().Throw<ArgumentException>().Which;
        exception.Message.Should().Contain("AnalyzerCaptureOptions validation failed");
        exception.Message.Should().Contain("FftSize must be a power of two (got 1000).");
        exception.Message.Should().Contain("Channels must be 1 (mono) or 2 (stereo) (got 3).");
        exception.Message.Split(Environment.NewLine).Count(line => line.Contains("- ")).Should().Be(2);
    }

    [Fact]
    public void EnsureValid_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        AnalyzerCaptureOptions? options = null;

        // Act
        Action act = () => options.EnsureValid();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
