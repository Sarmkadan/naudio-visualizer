using Xunit;
using NAudioVisualizer.Themes;
using NAudioVisualizer.Domain.Models;

namespace NAudioVisualizer.Tests;

public class ColorSchemeExtensionsTests
{
    [Fact]
    public void Reversed_ShouldReverseGradientStopsAndPositions()
    {
        var scheme = ColorScheme.Dark;
        var reversed = scheme.Reversed();

        Assert.Equal(scheme.Theme.WaveformGradient.Count, reversed.Theme.WaveformGradient.Count);
        
        // Check first stop is now at 1.0 (approximately)
        // Original dark: [0.00, 0xFF00004D], [0.50, 0xFF0080FF], [1.00, 0xFFFFFFFF]
        // Reversed: [0.00, 0xFFFFFFFF], [0.50, 0xFF0080FF], [1.00, 0xFF00004D]
        
        Assert.Equal(0.0f, reversed.Theme.WaveformGradient[0].Position, 2);
        Assert.Equal(0xFFFFFFFF, reversed.Theme.WaveformGradient[0].Color);
        
        Assert.Equal(0.5f, reversed.Theme.WaveformGradient[1].Position, 2);
        Assert.Equal(0xFF0080FF, reversed.Theme.WaveformGradient[1].Color);
        
        Assert.Equal(1.0f, reversed.Theme.WaveformGradient[2].Position, 2);
        Assert.Equal(0xFF00004D, reversed.Theme.WaveformGradient[2].Color);
    }

    [Fact]
    public void WithBrightness_ShouldAdjustColors()
    {
        var scheme = ColorScheme.Dark;
        var brighter = scheme.WithBrightness(2.0f);

        Assert.NotEqual(scheme.Theme.BackgroundColor, brighter.Theme.BackgroundColor);
        Assert.NotEqual(scheme.Theme.WaveformGradient[0].Color, brighter.Theme.WaveformGradient[0].Color);
    }

    [Fact]
    public void Lerp_ShouldInterpolateBetweenSchemes()
    {
        var scheme1 = ColorScheme.Dark;
        var scheme2 = ColorScheme.Light;
        var lerped = scheme1.Lerp(scheme2, 0.5f);

        Assert.NotNull(lerped);
        Assert.NotEqual(scheme1.Theme.BackgroundColor, lerped.Theme.BackgroundColor);
        Assert.NotEqual(scheme2.Theme.BackgroundColor, lerped.Theme.BackgroundColor);
    }
}
