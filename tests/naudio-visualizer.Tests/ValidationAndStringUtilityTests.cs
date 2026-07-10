#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using FluentAssertions;
using NAudioVisualizer.Utilities;
using Xunit;

namespace NAudioVisualizer.Tests;

/// <summary>
/// Contains unit tests for the ValidationUtility and StringUtility classes.
/// </summary>
public class ValidationAndStringUtilityTests
{
    // -------------------------------------------------------------------------
    // ValidationUtility
    // -------------------------------------------------------------------------

    /// <summary>
    /// Tests that ValidateSampleRate returns the expected boolean result for various sample rates.
    /// </summary>
    /// <param name="rate">The sample rate to test.</param>
    /// <param name="expected">The expected result of the validation.</param>
    [Theory]
    [InlineData(44100, true)]
    [InlineData(48000, true)]
    [InlineData(192000, true)]
    [InlineData(8000, true)]
    [InlineData(7999, false)]   // below minimum
    [InlineData(192100, false)] // above maximum
    [InlineData(44101, false)]  // not divisible by 100
    public void ValidateSampleRate_VariousRates_ReturnsExpectedResult(int rate, bool expected)
    {
        var result = ValidationUtility.ValidateSampleRate(rate);

        result.Should().Be(expected);
    }

    /// <summary>
    /// Tests that ValidateFftSize returns the expected boolean result for various FFT sizes.
    /// </summary>
    /// <param name="fftSize">The FFT size to test.</param>
    /// <param name="expected">The expected result of the validation.</param>
    [Theory]
    [InlineData(256, true)]
    [InlineData(1024, true)]
    [InlineData(16384, true)]
    [InlineData(255, false)]    // below minimum
    [InlineData(32768, false)]  // above maximum
    [InlineData(500, false)]    // not a power of 2
    public void ValidateFftSize_VariousSizes_ReturnsExpectedResult(int fftSize, bool expected)
    {
        var result = ValidationUtility.ValidateFftSize(fftSize);

        result.Should().Be(expected);
    }

    /// <summary>
    /// Tests that ValidateChannelCount returns the expected boolean result for various channel counts.
    /// </summary>
    /// <param name="channels">The channel count to test.</param>
    /// <param name="expected">The expected result of the validation.</param>
    [Theory]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(0, false)]
    [InlineData(3, false)]
    public void ValidateChannelCount_VariousCounts_ReturnsExpectedResult(int channels, bool expected)
    {
        var result = ValidationUtility.ValidateChannelCount(channels);

        result.Should().Be(expected);
    }

    /// <summary>
    /// Tests that ValidateFrequency returns the expected boolean result for boundary and specific frequency values.
    /// </summary>
    /// <param name="frequency">The frequency to test.</param>
    /// <param name="expected">The expected result of the validation.</param>
    [Theory]
    [InlineData(20f, true)]     // lower boundary
    [InlineData(20000f, true)]  // upper boundary
    [InlineData(1000f, true)]
    [InlineData(19f, false)]
    [InlineData(20001f, false)]
    public void ValidateFrequency_BoundaryValues_ReturnsExpectedResult(float frequency, bool expected)
    {
        var result = ValidationUtility.ValidateFrequency(frequency);

        result.Should().Be(expected);
    }

    /// <summary>
    /// Tests that ValidateAudioData returns false when the input array is null.
    /// </summary>
    [Fact]
    public void ValidateAudioData_NullArray_ReturnsFalse()
    {
        var result = ValidationUtility.ValidateAudioData(null);

        result.Should().BeFalse();
    }

    /// <summary>
    /// Tests that ValidateAudioData returns true when the input array is non-empty.
    /// </summary>
    [Fact]
    public void ValidateAudioData_NonEmptyArray_ReturnsTrue()
    {
        var result = ValidationUtility.ValidateAudioData(new float[] { 0.5f });

        result.Should().BeTrue();
    }

    /// <summary>
    /// Tests that ValidateAmplitude returns true for values within the valid range [-1, 1].
    /// </summary>
    [Fact]
    public void ValidateAmplitude_ValueWithinRange_ReturnsTrue()
    {
        ValidationUtility.ValidateAmplitude(0.5f).Should().BeTrue();
        ValidationUtility.ValidateAmplitude(-1f).Should().BeTrue();
        ValidationUtility.ValidateAmplitude(1f).Should().BeTrue();
    }

    /// <summary>
    /// Tests that ValidateAmplitude returns false for values outside the valid range [-1, 1].
    /// </summary>
    [Fact]
    public void ValidateAmplitude_ValueOutsideRange_ReturnsFalse()
    {
        ValidationUtility.ValidateAmplitude(1.1f).Should().BeFalse();
        ValidationUtility.ValidateAmplitude(-1.1f).Should().BeFalse();
    }

    /// <summary>
    /// Tests that ThrowIfNull throws an ArgumentNullException when the value is null.
    /// </summary>
    [Fact]
    public void ThrowIfNull_NullValue_ThrowsArgumentNullException()
    {
        var act = () => ValidationUtility.ThrowIfNull(null, "testParam");

        act.Should().Throw<ArgumentNullException>().WithParameterName("testParam");
    }

    /// <summary>
    /// Tests that ThrowIfNull does not throw an exception when the value is not null.
    /// </summary>
    [Fact]
    public void ThrowIfNull_NonNullValue_DoesNotThrow()
    {
        var act = () => ValidationUtility.ThrowIfNull(new object(), "testParam");

        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that ThrowIfOutOfRange throws an ArgumentOutOfRangeException when the value is below the minimum.
    /// </summary>
    [Fact]
    public void ThrowIfOutOfRange_ValueBelowMinimum_ThrowsArgumentOutOfRangeException()
    {
        var act = () => ValidationUtility.ThrowIfOutOfRange(5, 10, 100, "value");

        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("value");
    }

    /// <summary>
    /// Tests that ThrowIfNullOrWhitespace throws an ArgumentException when the string consists only of whitespace.
    /// </summary>
    [Fact]
    public void ThrowIfNullOrWhitespace_WhitespaceString_ThrowsArgumentException()
    {
        var act = () => ValidationUtility.ThrowIfNullOrWhitespace("   ", "param");

        act.Should().Throw<ArgumentException>().WithParameterName("param");
    }

    // -------------------------------------------------------------------------
    // StringUtility
    // -------------------------------------------------------------------------

    /// <summary>
    /// Tests that Truncate shortens a string longer than the max length and appends an ellipsis.
    /// </summary>
    [Fact]
    public void Truncate_LongerThanMaxLength_TruncatesWithEllipsis()
    {
        // "Hello..." = 8 chars, input truncated to maxLength - 3 = 5
        var result = StringUtility.Truncate("Hello World!", 8);

        result.Should().Be("Hello...");
        result.Length.Should().Be(8);
    }

    /// <summary>
    /// Tests that Truncate returns the original string if it is shorter than the max length.
    /// </summary>
    [Fact]
    public void Truncate_ShorterThanMaxLength_ReturnsOriginalString()
    {
        var result = StringUtility.Truncate("Hi", 10);

        result.Should().Be("Hi");
    }

    /// <summary>
    /// Tests that Truncate shortens a string without appending an ellipsis when the flag is disabled.
    /// </summary>
    [Fact]
    public void Truncate_WithEllipsisDisabled_TruncatesWithoutEllipsis()
    {
        var result = StringUtility.Truncate("Hello World!", 5, addEllipsis: false);

        result.Should().Be("Hello");
    }

    /// <summary>
    /// Tests that FormatBytes outputs a string containing the correct unit (B, KB, MB) for various byte sizes.
    /// </summary>
    /// <param name="bytes">The number of bytes to format.</param>
    /// <param name="expectedUnit">The expected unit string in the output.</param>
    [Theory]
    [InlineData(0L, "B")]
    [InlineData(1024L, "KB")]
    [InlineData(1048576L, "MB")]
    public void FormatBytes_VariousSizes_OutputContainsCorrectUnit(long bytes, string expectedUnit)
    {
        var result = StringUtility.FormatBytes(bytes);

        result.Should().Contain(expectedUnit);
    }

    /// <summary>
    /// Tests that FormatMilliseconds returns "Invalid" for negative input values.
    /// </summary>
    [Fact]
    public void FormatMilliseconds_NegativeValue_ReturnsInvalidString()
    {
        var result = StringUtility.FormatMilliseconds(-1);

        result.Should().Be("Invalid");
    }

    /// <summary>
    /// Tests that FormatMilliseconds returns the value in milliseconds format for values less than 1000ms.
    /// </summary>
    [Fact]
    public void FormatMilliseconds_LessThanOneSecond_ReturnsMsFormat()
    {
        var result = StringUtility.FormatMilliseconds(500);

        result.Should().Be("500ms");
    }

    /// <summary>
    /// Tests that ToSnakeCase converts a PascalCase string to snake_case.
    /// </summary>
    [Fact]
    public void ToSnakeCase_PascalCaseInput_ReturnsSnakeCase()
    {
        var result = StringUtility.ToSnakeCase("SampleRate");

        result.Should().Be("sample_rate");
    }

    /// <summary>
    /// Tests that CountOccurrences returns the correct count for multiple non-overlapping substring matches.
    /// </summary>
    [Fact]
    public void CountOccurrences_MultipleNonOverlappingMatches_ReturnsCorrectCount()
    {
        var result = StringUtility.CountOccurrences("abcabcabc", "abc");

        result.Should().Be(3);
    }

    /// <summary>
    /// Tests that CountOccurrences returns zero when the substring is not found.
    /// </summary>
    [Fact]
    public void CountOccurrences_NoMatch_ReturnsZero()
    {
        var result = StringUtility.CountOccurrences("hello", "xyz");

        result.Should().Be(0);
    }

    /// <summary>
    /// Tests that IsAlphanumeric returns false for strings containing special characters.
    /// </summary>
    [Fact]
    public void IsAlphanumeric_StringWithSpecialCharacters_ReturnsFalse()
    {
        StringUtility.IsAlphanumeric("hello!").Should().BeFalse();
    }

    /// <summary>
    /// Tests that IsAlphanumeric returns true for strings containing only letters and numbers.
    /// </summary>
    [Fact]
    public void IsAlphanumeric_PureAlphanumericString_ReturnsTrue()
    {
        StringUtility.IsAlphanumeric("Hello123").Should().BeTrue();
    }

    /// <summary>
    /// Tests that Repeat returns the input string repeated the specified number of times.
    /// </summary>
    [Fact]
    public void Repeat_ValidInputs_ReturnsRepeatedString()
    {
        var result = StringUtility.Repeat("ab", 3);

        result.Should().Be("ababab");
    }
}
