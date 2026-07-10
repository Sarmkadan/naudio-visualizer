#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using FluentAssertions;
using NAudioVisualizer.Utilities;
using Xunit;

/// <summary>
/// Tests for the MathUtility class.
/// </summary>
public class MathUtilityTests
{
    /// <summary>
    /// Tests the FrequencyToMidiNote method with a frequency of A4 (440 Hz).
    /// </summary>
    [Fact]
    public void FrequencyToMidiNote_A4Frequency_Returns69()
    {
        var result = MathUtility.FrequencyToMidiNote(440f);

        result.Should().Be(69);
    }

    /// <summary>
    /// Tests the FrequencyToMidiNote method with non-positive frequencies.
    /// </summary>
    /// <param name="frequency">The frequency to test.</param>
    [Theory]
    [InlineData(0f)]
    [InlineData(-100f)]
    public void FrequencyToMidiNote_NonPositiveFrequency_ReturnsZero(float frequency)
    {
        var result = MathUtility.FrequencyToMidiNote(frequency);

        result.Should().Be(0);
    }

    /// <summary>
    /// Tests the AmplitudeToDb method with a zero amplitude.
    /// </summary>
    [Fact]
    public void AmplitudeToDb_ZeroAmplitude_ReturnsNegativeInfinity()
    {
        var result = MathUtility.AmplitudeToDb(0f);

        result.Should().Be(float.NegativeInfinity);
    }

    /// <summary>
    /// Tests the AmplitudeToDb method with a unit amplitude.
    /// </summary>
    [Fact]
    public void AmplitudeToDb_UnitAmplitude_ReturnsZeroDb()
    {
        var result = MathUtility.AmplitudeToDb(1f);

        result.Should().BeApproximately(0f, precision: 0.001f);
    }

    /// <summary>
    /// Tests the CalculateRms method with a uniform signal.
    /// </summary>
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

    /// <summary>
    /// Tests the CalculateRms method with an empty array.
    /// </summary>
    [Fact]
    public void CalculateRms_EmptyArray_ReturnsZero()
    {
        var result = MathUtility.CalculateRms([]);

        result.Should().Be(0f);
    }

    /// <summary>
    /// Tests the CalculatePeak method with a signal containing negative values.
    /// </summary>
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

    /// <summary>
    /// Tests the NextPowerOf2 method with various inputs.
    /// </summary>
    /// <param name="input">The input value to test.</param>
    /// <param name="expected">The expected result.</param>
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

    /// <summary>
    /// Tests the IsPowerOf2 method with various inputs.
    /// </summary>
    /// <param name="input">The input value to test.</param>
    /// <param name="expected">The expected result.</param>
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

    /// <summary>
    /// Tests the Lerp method with a value beyond the upper bound.
    /// </summary>
    [Fact]
    public void Lerp_TBeyondUpperBound_ClampsToEndValue()
    {
        var result = MathUtility.Lerp(0f, 10f, 2f);

        result.Should().Be(10f);
    }

    /// <summary>
    /// Tests the MapRange method with a midpoint value.
    /// </summary>
    [Fact]
    public void MapRange_MidpointValue_ReturnsMidpointOfTargetRange()
    {
        var result = MathUtility.MapRange(5f, 0f, 10f, 0f, 100f);

        result.Should().BeApproximately(50f, precision: 0.001f);
    }

    /// <summary>
    /// Tests the MapRange method with equal source bounds.
    /// </summary>
    [Fact]
    public void MapRange_EqualSourceBounds_ReturnsTargetMinimum()
    {
        var result = MathUtility.MapRange(5f, 5f, 5f, 0f, 100f);

        result.Should().Be(0f);
    }
}
