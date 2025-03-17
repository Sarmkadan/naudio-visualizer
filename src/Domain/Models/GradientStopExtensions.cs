#nullable enable

// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides useful extension methods for <see cref="GradientStop"/> operations.
/// </summary>
public static class GradientStopExtensions
{
    /// <summary>
    /// Creates a new <see cref="GradientStop"/> with the same position but a modified color.
    /// </summary>
    /// <param name="stop">The gradient stop to modify.</param>
    /// <param name="newColor">The new ARGB color value (0xAARRGGBB).</param>
    /// <returns>A new <see cref="GradientStop"/> instance with the updated color.</returns>
    public static GradientStop WithColor(this GradientStop stop, uint newColor)
    {
        ArgumentNullException.ThrowIfNull(stop);
        return new GradientStop(stop.Position, newColor);
    }

    /// <summary>
    /// Creates a new <see cref="GradientStop"/> with the same color but a modified position.
    /// </summary>
    /// <param name="stop">The gradient stop to modify.</param>
    /// <param name="newPosition">The new normalized position in [0, 1].</param>
    /// <returns>A new <see cref="GradientStop"/> instance with the updated position.</returns>
    public static GradientStop WithPosition(this GradientStop stop, float newPosition)
    {
        ArgumentNullException.ThrowIfNull(stop);
        return new GradientStop(newPosition, stop.Color);
    }

    /// <summary>
    /// Gets the ARGB color components from the gradient stop.
    /// </summary>
    /// <param name="stop">The gradient stop.</param>
    /// <param name="alpha">The alpha/transparency component (0-255).</param>
    /// <param name="red">The red component (0-255).</param>
    /// <param name="green">The green component (0-255).</param>
    /// <param name="blue">The blue component (0-255).</param>
    public static void GetArgbComponents(this GradientStop stop,
        out byte alpha, out byte red, out byte green, out byte blue)
    {
        ArgumentNullException.ThrowIfNull(stop);
        alpha = (byte)((stop.Color >> 24) & 0xFF);
        red = (byte)((stop.Color >> 16) & 0xFF);
        green = (byte)((stop.Color >> 8) & 0xFF);
        blue = (byte)(stop.Color & 0xFF);
    }

    /// <summary>
    /// Creates a new gradient stop with adjusted alpha/transparency.
    /// </summary>
    /// <param name="stop">The gradient stop to modify.</param>
    /// <param name="alpha">The new alpha value (0-255).</param>
    /// <returns>A new <see cref="GradientStop"/> with the updated alpha.</returns>
    public static GradientStop WithAlpha(this GradientStop stop, byte alpha)
    {
        ArgumentNullException.ThrowIfNull(stop);
        var color = stop.Color;
        var newColor = (uint)((uint)alpha << 24 | (color & 0x00FFFFFFu));
        return new GradientStop(stop.Position, newColor);
    }

    /// <summary>
    /// Creates a new gradient stop with adjusted brightness.
    /// </summary>
    /// <param name="stop">The gradient stop to modify.</param>
    /// <param name="brightnessFactor">Brightness multiplier (0.0 to 2.0). Values &gt; 1.0 increase brightness, &lt; 1.0 decrease.</param>
    /// <returns>A new <see cref="GradientStop"/> with adjusted brightness.</returns>
    public static GradientStop AdjustBrightness(this GradientStop stop, float brightnessFactor)
    {
        ArgumentNullException.ThrowIfNull(stop);
        ArgumentOutOfRangeException.ThrowIfLessThan(brightnessFactor, 0.0f);

        if (Math.Abs(brightnessFactor - 1.0f) < float.Epsilon)
        {
            return stop;
        }

        GetArgbComponents(stop, out var alpha, out var red, out var green, out var blue);

        // Apply brightness to RGB components
        red = (byte)Math.Clamp((int)(red * brightnessFactor), 0, 255);
        green = (byte)Math.Clamp((int)(green * brightnessFactor), 0, 255);
        blue = (byte)Math.Clamp((int)(blue * brightnessFactor), 0, 255);

        var newColor = (uint)((alpha << 24) | (red << 16) | (green << 8) | blue);
        return new GradientStop(stop.Position, newColor);
    }

    /// <summary>
    /// Creates a new gradient stop with adjusted contrast.
    /// </summary>
    /// <param name="stop">The gradient stop to modify.</param>
    /// <param name="contrastFactor">Contrast multiplier (0.0 to 2.0). Values &gt; 1.0 increase contrast, &lt; 1.0 decrease.</param>
    /// <returns>A new <see cref="GradientStop"/> with adjusted contrast.</returns>
    public static GradientStop AdjustContrast(this GradientStop stop, float contrastFactor)
    {
        ArgumentNullException.ThrowIfNull(stop);
        ArgumentOutOfRangeException.ThrowIfLessThan(contrastFactor, 0.0f);

        if (Math.Abs(contrastFactor - 1.0f) < float.Epsilon)
        {
            return stop;
        }

        GetArgbComponents(stop, out var alpha, out var red, out var green, out var blue);

        // Apply contrast formula: result = (value - 128) * contrastFactor + 128
        red = (byte)Math.Clamp((int)((red - 128) * contrastFactor + 128), 0, 255);
        green = (byte)Math.Clamp((int)((green - 128) * contrastFactor + 128), 0, 255);
        blue = (byte)Math.Clamp((int)((blue - 128) * contrastFactor + 128), 0, 255);

        var newColor = (uint)((alpha << 24) | (red << 16) | (green << 8) | blue);
        return new GradientStop(stop.Position, newColor);
    }

    /// <summary>
    /// Gets the relative position of this gradient stop within a collection.
    /// </summary>
    /// <param name="stop">The gradient stop.</param>
    /// <param name="stops">The collection of gradient stops.</param>
    /// <returns>The zero-based index of the stop, or -1 if not found.</returns>
    public static int IndexIn(this GradientStop stop, IReadOnlyList<GradientStop> stops)
    {
        ArgumentNullException.ThrowIfNull(stop);
        ArgumentNullException.ThrowIfNull(stops);
        for (int i = 0; i < stops.Count; i++)
        {
            if (ReferenceEquals(stop, stops[i]))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Determines whether this gradient stop is the first stop in the collection.
    /// </summary>
    /// <param name="stop">The gradient stop.</param>
    /// <param name="stops">The collection of gradient stops.</param>
    /// <returns>True if this is the first stop; otherwise, false.</returns>
    public static bool IsFirst(this GradientStop stop, IReadOnlyList<GradientStop> stops)
    {
        ArgumentNullException.ThrowIfNull(stop);
        ArgumentNullException.ThrowIfNull(stops);
        return stops.Count > 0 && stops[0] == stop;
    }

    /// <summary>
    /// Determines whether this gradient stop is the last stop in the collection.
    /// </summary>
    /// <param name="stop">The gradient stop.</param>
    /// <param name="stops">The collection of gradient stops.</param>
    /// <returns>True if this is the last stop; otherwise, false.</returns>
    public static bool IsLast(this GradientStop stop, IReadOnlyList<GradientStop> stops)
    {
        ArgumentNullException.ThrowIfNull(stop);
        ArgumentNullException.ThrowIfNull(stops);
        return stops.Count > 0 && stops[^1] == stop;
    }

    /// <summary>
    /// Gets the next gradient stop in the collection, or null if this is the last stop.
    /// </summary>
    /// <param name="stop">The gradient stop.</param>
    /// <param name="stops">The collection of gradient stops.</param>
    /// <returns>The next stop, or null if this is the last.</returns>
    public static GradientStop? Next(this GradientStop stop, IReadOnlyList<GradientStop> stops)
    {
        ArgumentNullException.ThrowIfNull(stop);
        ArgumentNullException.ThrowIfNull(stops);

        var index = IndexIn(stop, stops);
        return index >= 0 && index < stops.Count - 1 ? stops[index + 1] : null;
    }

    /// <summary>
    /// Gets the previous gradient stop in the collection, or null if this is the first stop.
    /// </summary>
    /// <param name="stop">The gradient stop.</param>
    /// <param name="stops">The collection of gradient stops.</param>
    /// <returns>The previous stop, or null if this is the first.</returns>
    public static GradientStop? Previous(this GradientStop stop, IReadOnlyList<GradientStop> stops)
    {
        ArgumentNullException.ThrowIfNull(stop);
        ArgumentNullException.ThrowIfNull(stops);

        var index = IndexIn(stop, stops);
        return index > 0 ? stops[index - 1] : null;
    }

    /// <summary>
    /// Creates a new gradient stop that is the color-interpolated version between this stop and another.
    /// </summary>
    /// <param name="stop">The first gradient stop (at position 0 of interpolation).</param>
    /// <param name="other">The second gradient stop (at position 1 of interpolation).</param>
    /// <param name="t">Interpolation factor (0.0 to 1.0).</param>
    /// <returns>A new gradient stop with interpolated color.</returns>
    public static GradientStop Interpolate(this GradientStop stop, GradientStop other, float t)
    {
        ArgumentNullException.ThrowIfNull(stop);
        ArgumentNullException.ThrowIfNull(other);
        ArgumentOutOfRangeException.ThrowIfLessThan(t, 0.0f);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(t, 1.0f);

        GetArgbComponents(stop, out var a1, out var r1, out var g1, out var b1);
        GetArgbComponents(other, out var a2, out var r2, out var g2, out var b2);

        // Linear interpolation of each component
        var alpha = (byte)(a1 + (a2 - a1) * t);
        var red = (byte)(r1 + (r2 - r1) * t);
        var green = (byte)(g1 + (g2 - g1) * t);
        var blue = (byte)(b1 + (b2 - b1) * t);

        var newColor = (uint)((alpha << 24) | (red << 16) | (green << 8) | blue);
        return new GradientStop(stop.Position, newColor);
    }

    /// <summary>
    /// Determines whether two gradient stops have the same color.
    /// </summary>
    /// <param name="stop">The first gradient stop.</param>
    /// <param name="other">The second gradient stop.</param>
    /// <returns>True if colors are equal; otherwise, false.</returns>
    public static bool HasSameColor(this GradientStop stop, GradientStop other)
    {
        ArgumentNullException.ThrowIfNull(stop);
        ArgumentNullException.ThrowIfNull(other);
        return stop.Color == other.Color;
    }

    /// <summary>
    /// Determines whether two gradient stops have the same position.
    /// </summary>
    /// <param name="stop">The first gradient stop.</param>
    /// <param name="other">The second gradient stop.</param>
    /// <returns>True if positions are equal; otherwise, false.</returns>
    public static bool HasSamePosition(this GradientStop stop, GradientStop other)
    {
        ArgumentNullException.ThrowIfNull(stop);
        ArgumentNullException.ThrowIfNull(other);
        return Math.Abs(stop.Position - other.Position) < float.Epsilon;
    }

    /// <summary>
    /// Gets the perceived brightness of the gradient stop's color (0-255).
    /// Uses the standard luminance formula: 0.299*R + 0.587*G + 0.114*B
    /// </summary>
    /// <param name="stop">The gradient stop.</param>
    /// <returns>The perceived brightness value.</returns>
    public static byte GetBrightness(this GradientStop stop)
    {
        ArgumentNullException.ThrowIfNull(stop);
        GetArgbComponents(stop, out _, out var red, out var green, out var blue);
        return (byte)(0.299 * red + 0.587 * green + 0.114 * blue);
    }

    /// <summary>
    /// Determines whether the gradient stop's color is considered dark (brightness &lt; 128).
    /// </summary>
    /// <param name="stop">The gradient stop.</param>
    /// <returns>True if the color is dark; otherwise, false.</returns>
    public static bool IsDark(this GradientStop stop)
    {
        return GetBrightness(stop) < 128;
    }

    /// <summary>
    /// Determines whether the gradient stop's color is considered light (brightness &gt;= 128).
    /// </summary>
    /// <param name="stop">The gradient stop.</param>
    /// <returns>True if the color is light; otherwise, false.</returns>
    public static bool IsLight(this GradientStop stop)
    {
        return GetBrightness(stop) >= 128;
    }
}