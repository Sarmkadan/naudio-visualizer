#nullable enable
using FluentAssertions;
using NAudioVisualizer.Utilities;
using Xunit;

namespace NAudioVisualizer.Tests;

public sealed class StringUtilityEdgeCaseTests
{
    [Fact]
    public void Truncate_NullInput_ReturnsEmpty() =>
        StringUtility.Truncate(null, 10).Should().BeEmpty();

    [Fact]
    public void Truncate_EmptyInput_ReturnsEmpty() =>
        StringUtility.Truncate("", 10).Should().BeEmpty();

    [Fact]
    public void Truncate_ShortString_ReturnsUnchanged() =>
        StringUtility.Truncate("hello", 10).Should().Be("hello");

    [Fact]
    public void Truncate_LongString_WithEllipsis_TruncatesCorrectly()
    {
        var result = StringUtility.Truncate("Hello, World! This is a long string", 10);
        result.Should().EndWith("...");
        result.Length.Should().BeLessThanOrEqualTo(10);
    }

    [Fact]
    public void Truncate_LongString_WithoutEllipsis_TruncatesExactly()
    {
        var result = StringUtility.Truncate("Hello, World!", 5, addEllipsis: false);
        result.Should().Be("Hello");
    }

    [Fact]
    public void Truncate_MaxLengthZero_ReturnsEmptyOrEllipsis()
    {
        var result = StringUtility.Truncate("test", 0);
        result.Length.Should().BeLessThanOrEqualTo(3);
    }

    [Fact]
    public void Repeat_NullInput_ReturnsEmpty() =>
        StringUtility.Repeat(null!, 5).Should().BeEmpty();

    [Fact]
    public void Repeat_EmptyInput_ReturnsEmpty() =>
        StringUtility.Repeat("", 5).Should().BeEmpty();

    [Fact]
    public void Repeat_ZeroCount_ReturnsEmpty() =>
        StringUtility.Repeat("ab", 0).Should().BeEmpty();

    [Fact]
    public void Repeat_NegativeCount_ReturnsEmpty() =>
        StringUtility.Repeat("ab", -1).Should().BeEmpty();

    [Fact]
    public void Repeat_ValidInput_RepeatsCorrectly() =>
        StringUtility.Repeat("ab", 3).Should().Be("ababab");

    [Fact]
    public void PadCenter_ShortString_PadsEvenlyWithSpaces()
    {
        var result = StringUtility.PadCenter("hi", 6);
        result.Length.Should().Be(6);
        result.Should().Contain("hi");
    }

    [Fact]
    public void PadCenter_StringExceedsWidth_ReturnsUnchanged()
    {
        var result = StringUtility.PadCenter("long string", 5);
        result.Should().Be("long string");
    }
}
