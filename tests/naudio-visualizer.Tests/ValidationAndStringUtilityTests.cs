// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using FluentAssertions;
using NAudioVisualizer.Utilities;
using Xunit;

namespace NAudioVisualizer.Tests;

public class ValidationAndStringUtilityTests
{
    // -------------------------------------------------------------------------
    // ValidationUtility
    // -------------------------------------------------------------------------

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

    [Fact]
    public void ValidateAudioData_NullArray_ReturnsFalse()
    {
        var result = ValidationUtility.ValidateAudioData(null);

        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateAudioData_NonEmptyArray_ReturnsTrue()
    {
        var result = ValidationUtility.ValidateAudioData(new float[] { 0.5f });

        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateAmplitude_ValueWithinRange_ReturnsTrue()
    {
        ValidationUtility.ValidateAmplitude(0.5f).Should().BeTrue();
        ValidationUtility.ValidateAmplitude(-1f).Should().BeTrue();
        ValidationUtility.ValidateAmplitude(1f).Should().BeTrue();
    }

    [Fact]
    public void ValidateAmplitude_ValueOutsideRange_ReturnsFalse()
    {
        ValidationUtility.ValidateAmplitude(1.1f).Should().BeFalse();
        ValidationUtility.ValidateAmplitude(-1.1f).Should().BeFalse();
    }

    [Fact]
    public void ThrowIfNull_NullValue_ThrowsArgumentNullException()
    {
        var act = () => ValidationUtility.ThrowIfNull(null, "testParam");

        act.Should().Throw<ArgumentNullException>().WithParameterName("testParam");
    }

    [Fact]
    public void ThrowIfNull_NonNullValue_DoesNotThrow()
    {
        var act = () => ValidationUtility.ThrowIfNull(new object(), "testParam");

        act.Should().NotThrow();
    }

    [Fact]
    public void ThrowIfOutOfRange_ValueBelowMinimum_ThrowsArgumentOutOfRangeException()
    {
        var act = () => ValidationUtility.ThrowIfOutOfRange(5, 10, 100, "value");

        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("value");
    }

    [Fact]
    public void ThrowIfNullOrWhitespace_WhitespaceString_ThrowsArgumentException()
    {
        var act = () => ValidationUtility.ThrowIfNullOrWhitespace("   ", "param");

        act.Should().Throw<ArgumentException>().WithParameterName("param");
    }

    // -------------------------------------------------------------------------
    // StringUtility
    // -------------------------------------------------------------------------

    [Fact]
    public void Truncate_LongerThanMaxLength_TruncatesWithEllipsis()
    {
        // "Hello..." = 8 chars, input truncated to maxLength - 3 = 5
        var result = StringUtility.Truncate("Hello World!", 8);

        result.Should().Be("Hello...");
        result.Length.Should().Be(8);
    }

    [Fact]
    public void Truncate_ShorterThanMaxLength_ReturnsOriginalString()
    {
        var result = StringUtility.Truncate("Hi", 10);

        result.Should().Be("Hi");
    }

    [Fact]
    public void Truncate_WithEllipsisDisabled_TruncatesWithoutEllipsis()
    {
        var result = StringUtility.Truncate("Hello World!", 5, addEllipsis: false);

        result.Should().Be("Hello");
    }

    [Theory]
    [InlineData(0L, "B")]
    [InlineData(1024L, "KB")]
    [InlineData(1048576L, "MB")]
    public void FormatBytes_VariousSizes_OutputContainsCorrectUnit(long bytes, string expectedUnit)
    {
        var result = StringUtility.FormatBytes(bytes);

        result.Should().Contain(expectedUnit);
    }

    [Fact]
    public void FormatMilliseconds_NegativeValue_ReturnsInvalidString()
    {
        var result = StringUtility.FormatMilliseconds(-1);

        result.Should().Be("Invalid");
    }

    [Fact]
    public void FormatMilliseconds_LessThanOneSecond_ReturnsMsFormat()
    {
        var result = StringUtility.FormatMilliseconds(500);

        result.Should().Be("500ms");
    }

    [Fact]
    public void ToSnakeCase_PascalCaseInput_ReturnsSnakeCase()
    {
        var result = StringUtility.ToSnakeCase("SampleRate");

        result.Should().Be("sample_rate");
    }

    [Fact]
    public void CountOccurrences_MultipleNonOverlappingMatches_ReturnsCorrectCount()
    {
        var result = StringUtility.CountOccurrences("abcabcabc", "abc");

        result.Should().Be(3);
    }

    [Fact]
    public void CountOccurrences_NoMatch_ReturnsZero()
    {
        var result = StringUtility.CountOccurrences("hello", "xyz");

        result.Should().Be(0);
    }

    [Fact]
    public void IsAlphanumeric_StringWithSpecialCharacters_ReturnsFalse()
    {
        StringUtility.IsAlphanumeric("hello!").Should().BeFalse();
    }

    [Fact]
    public void IsAlphanumeric_PureAlphanumericString_ReturnsTrue()
    {
        StringUtility.IsAlphanumeric("Hello123").Should().BeTrue();
    }

    [Fact]
    public void Repeat_ValidInputs_ReturnsRepeatedString()
    {
        var result = StringUtility.Repeat("ab", 3);

        result.Should().Be("ababab");
    }
}
