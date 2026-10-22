#nullable enable

// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using FluentAssertions;
using NAudioVisualizer.Domain.Models;
using Xunit;

namespace NAudioVisualizer.Tests;

/// <summary>
/// Unit tests for <see cref="WaveformRenderingSettings"/> and its <see cref="WaveformRenderingSettingsValidation"/> rules.
/// </summary>
public class WaveformRenderingSettingsValidationTests
{
    [Fact]
    public void Validate_DefaultSettings_ReturnsNoProblems()
    {
        // Arrange
        var settings = new WaveformRenderingSettings();

        // Act
        var problems = settings.Validate();

        // Assert
        problems.Should().BeEmpty();
    }

    [Fact]
    public void Validate_NullSettings_ThrowsArgumentNullException()
    {
        // Arrange
        WaveformRenderingSettings? settings = null;

        // Act
        var act = () => settings.Validate();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0x00000000u)]
    [InlineData(0x00FFFFFFu)]
    public void Validate_FullyTransparentLineColor_ReportsProblem(uint color)
    {
        // Arrange
        var settings = new WaveformRenderingSettings { LineColor = color };

        // Act
        var problems = settings.Validate();

        // Assert
        problems.Should().ContainSingle().Which.Should().Contain("LineColor");
    }

    [Theory]
    [InlineData(0x01000000u)]
    [InlineData(0xFF000000u)]
    [InlineData(0xFF00D9FFu)]
    public void Validate_VisibleLineColor_ReturnsNoProblems(uint color)
    {
        // Arrange
        var settings = new WaveformRenderingSettings { LineColor = color };

        // Act
        var problems = settings.Validate();

        // Assert
        problems.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(-1f)]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    public void Validate_NonPositiveOrNonFiniteLineThickness_ReportsProblem(float thickness)
    {
        // Arrange
        var settings = new WaveformRenderingSettings { LineThickness = thickness };

        // Act
        var problems = settings.Validate();

        // Assert
        problems.Should().ContainSingle().Which.Should().Contain("LineThickness");
    }

    [Theory]
    [InlineData(0.1f)]
    [InlineData(1f)]
    [InlineData(10f)]
    public void Validate_AmplitudeZoomWithinRange_ReturnsNoProblems(float zoom)
    {
        // Arrange
        var settings = new WaveformRenderingSettings { AmplitudeZoom = zoom };

        // Act
        var problems = settings.Validate();

        // Assert
        problems.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0.09f)]
    [InlineData(10.01f)]
    [InlineData(float.NaN)]
    public void Validate_AmplitudeZoomOutOfRange_ReportsProblem(float zoom)
    {
        // Arrange
        var settings = new WaveformRenderingSettings { AmplitudeZoom = zoom };

        // Act
        var problems = settings.Validate();

        // Assert
        problems.Should().ContainSingle().Which.Should().Contain("AmplitudeZoom");
    }

    [Fact]
    public void Validate_ZeroDownsamplingFactor_ReportsProblem()
    {
        // Arrange
        var settings = new WaveformRenderingSettings { DownsamplingFactor = 0 };

        // Act
        var problems = settings.Validate();

        // Assert
        problems.Should().ContainSingle().Which.Should().Contain("DownsamplingFactor");
    }

    [Fact]
    public void Validate_MultipleViolations_ReportsEveryViolation()
    {
        // Arrange
        var settings = new WaveformRenderingSettings
        {
            LineColor = 0,
            LineThickness = -2f,
            AmplitudeZoom = 50f,
            DownsamplingFactor = 0
        };

        // Act
        var problems = settings.Validate();

        // Assert
        problems.Should().HaveCount(4);
    }

    [Fact]
    public void EnsureValid_ValidSettings_DoesNotThrow()
    {
        // Arrange
        var settings = new WaveformRenderingSettings();

        // Act
        var act = () => settings.EnsureValid();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureValid_MultipleViolations_ThrowsArgumentExceptionListingAll()
    {
        // Arrange
        var settings = new WaveformRenderingSettings
        {
            LineThickness = 0f,
            DownsamplingFactor = 0
        };

        // Act
        var act = () => settings.EnsureValid();

        // Assert
        act.Should().Throw<ArgumentException>()
            .Which.Message.Should().Contain("LineThickness").And.Contain("DownsamplingFactor");
    }

    [Fact]
    public void EnsureValid_NullSettings_ThrowsArgumentNullException()
    {
        // Arrange
        WaveformRenderingSettings? settings = null;

        // Act
        var act = () => settings.EnsureValid();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
