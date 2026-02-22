#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using FluentAssertions;
using NAudioVisualizer.Utilities;
using Xunit;

namespace NAudioVisualizer.Tests;

public class MathUtilityTests
{
    [Fact]
    public void FrequencyToMidiNote_A4Frequency_Returns69()
    {
        var result = MathUtility.FrequencyToMidiNote(440f);

        result.Should().Be(69);
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(-100f)]
    public void FrequencyToMidiNote_NonPositiveFrequency_ReturnsZero(float frequency)
    {
        var result = MathUtility.FrequencyToMidiNote(frequency);

        result.Should().Be(0);
    }

    [Fact]
    public void AmplitudeToDb_ZeroAmplitude_ReturnsNegativeInfinity()
    {
        var result = MathUtility.AmplitudeToDb(0f);

        result.Should().Be(float.NegativeInfinity);
    }

    [Fact]
    public void AmplitudeToDb_UnitAmplitude_ReturnsZeroDb()
    {
        var result = MathUtility.AmplitudeToDb(1f);

        result.Should().BeApproximately(0f, precision: 0.001f);
    }

    [Fact]
    public void CalculateRms_UniformSignal_ReturnsUnitValue()
    {
        // Arrange
        var signal = new float[] { 1f, 1f, 1f, 1f };

        // Act
        var result = MathUtility.CalculateRms(signal);

        // Assert
        result.Should().BeApproximately(1f, precision: 0.001f);
    }

    [Fact]
    public void CalculateRms_EmptyArray_ReturnsZero()
    {
        var result = MathUtility.CalculateRms([]);

        result.Should().Be(0f);
    }

    [Fact]
    public void CalculatePeak_SignalWithNegativeValues_ReturnsAbsoluteMaximum()
    {
        // Arrange
        var signal = new float[] { 0.3f, -0.9f, 0.5f, -0.1f };

        // Act
        var result = MathUtility.CalculatePeak(signal);

        // Assert
        result.Should().BeApproximately(0.9f, precision: 0.001f);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 4)]
    [InlineData(7, 8)]
    [InlineData(8, 8)]
    [InlineData(9, 16)]
    public void NextPowerOf2_VariousInputs_ReturnsNextPower(int input, int expected)
    {
        var result = MathUtility.NextPowerOf2(input);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(4, true)]
    [InlineData(1024, true)]
    [InlineData(3, false)]
    [InlineData(6, false)]
    [InlineData(0, false)]
    public void IsPowerOf2_VariousInputs_ReturnsExpectedResult(int input, bool expected)
    {
        var result = MathUtility.IsPowerOf2(input);

        result.Should().Be(expected);
    }

    [Fact]
    public void Lerp_TBeyondUpperBound_ClampsToEndValue()
    {
        var result = MathUtility.Lerp(0f, 10f, 2f);

        result.Should().Be(10f);
    }

    [Fact]
    public void MapRange_MidpointValue_ReturnsMidpointOfTargetRange()
    {
        var result = MathUtility.MapRange(5f, 0f, 10f, 0f, 100f);

        result.Should().BeApproximately(50f, precision: 0.001f);
    }

    [Fact]
    public void MapRange_EqualSourceBounds_ReturnsTargetMinimum()
    {
        var result = MathUtility.MapRange(5f, 5f, 5f, 0f, 100f);

        result.Should().Be(0f);
    }
}
