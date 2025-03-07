#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Drawing;

namespace NAudioVisualizer.Utilities;

/// <summary>
/// Provides utility methods for color manipulation and gradient generation.
/// Supports color space conversions, interpolation, and visualization color schemes.
/// </summary>
public static class ColorUtility
{
    /// <summary>
    /// Converts RGB color to HSV (Hue, Saturation, Value).
    /// </summary>
    public static void RgbToHsv(float r, float g, float b, out float h, out float s, out float v)
    {
        float max = MathF.Max(r, MathF.Max(g, b));
        float min = MathF.Min(r, MathF.Min(g, b));
        float delta = max - min;

        // Value
        v = max;

        // Saturation
        s = max == 0 ? 0 : delta / max;

        // Hue
        if (delta == 0)
        {
            h = 0;
        }
        else if (max == r)
        {
            h = (60 * (((g - b) / delta) % 6) + 360) % 360;
        }
        else if (max == g)
        {
            h = (60 * (((b - r) / delta) + 2)) % 360;
        }
        else
        {
            h = (60 * (((r - g) / delta) + 4)) % 360;
        }
    }

    /// <summary>
    /// Converts HSV color to RGB.
    /// </summary>
    public static void HsvToRgb(float h, float s, float v, out float r, out float g, out float b)
    {
        float c = v * s;
        float x = c * (1 - MathF.Abs((h / 60f) % 2 - 1));
        float m = v - c;

        if (h < 60)
        {
            r = c; g = x; b = 0;
        }
        else if (h < 120)
        {
            r = x; g = c; b = 0;
        }
        else if (h < 180)
        {
            r = 0; g = c; b = x;
        }
        else if (h < 240)
        {
            r = 0; g = x; b = c;
        }
        else if (h < 300)
        {
            r = x; g = 0; b = c;
        }
        else
        {
            r = c; g = 0; b = x;
        }

        r = (r + m) * 255;
        g = (g + m) * 255;
        b = (b + m) * 255;
    }

    /// <summary>
    /// Linearly interpolates between two colors.
    /// </summary>
    public static Color LerpColor(Color a, Color b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        int r = (int)(a.R + (b.R - a.R) * t);
        int g = (int)(a.G + (b.G - a.G) * t);
        int blue = (int)(a.B + (b.B - a.B) * t);
        int alpha = (int)(a.A + (b.A - a.A) * t);

        return Color.FromArgb(alpha, r, g, blue);
    }

    /// <summary>
    /// Gets a color from the viridis colormap.
    /// Useful for spectrograms. Value should be in range [0, 1].
    /// </summary>
    public static Color GetViririsColor(float value)
    {
        value = Math.Clamp(value, 0f, 1f);

        // Viridis colormap approximation
        float r, g, b;

        if (value < 0.25f)
        {
            float t = value / 0.25f;
            r = MathUtility.Lerp(0.267f, 0.133f, t);
            g = MathUtility.Lerp(0.004f, 0.420f, t);
            b = MathUtility.Lerp(0.329f, 0.596f, t);
        }
        else if (value < 0.5f)
        {
            float t = (value - 0.25f) / 0.25f;
            r = MathUtility.Lerp(0.133f, 0.039f, t);
            g = MathUtility.Lerp(0.420f, 0.639f, t);
            b = MathUtility.Lerp(0.596f, 0.628f, t);
        }
        else if (value < 0.75f)
        {
            float t = (value - 0.5f) / 0.25f;
            r = MathUtility.Lerp(0.039f, 0.604f, t);
            g = MathUtility.Lerp(0.639f, 0.855f, t);
            b = MathUtility.Lerp(0.628f, 0.196f, t);
        }
        else
        {
            float t = (value - 0.75f) / 0.25f;
            r = MathUtility.Lerp(0.604f, 0.940f, t);
            g = MathUtility.Lerp(0.855f, 0.975f, t);
            b = MathUtility.Lerp(0.196f, 0.131f, t);
        }

        return Color.FromArgb(
            (int)(r * 255),
            (int)(g * 255),
            (int)(b * 255)
        );
    }

    /// <summary>
    /// Gets a color from the jet colormap.
    /// Classic blue-to-red colormap. Value should be in range [0, 1].
    /// </summary>
    public static Color GetJetColor(float value)
    {
        value = Math.Clamp(value, 0f, 1f);

        float r = 0, g = 0, b = 0;

        if (value < 0.125f)
        {
            b = 0.5f + 0.5f * (value / 0.125f);
        }
        else if (value < 0.375f)
        {
            b = 1.0f - (value - 0.125f) / 0.25f;
            g = (value - 0.125f) / 0.25f;
        }
        else if (value < 0.625f)
        {
            g = 1.0f - (value - 0.375f) / 0.25f;
            r = (value - 0.375f) / 0.25f;
        }
        else
        {
            r = 1.0f - (value - 0.625f) / 0.375f;
        }

        return Color.FromArgb(
            (int)(r * 255),
            (int)(g * 255),
            (int)(b * 255)
        );
    }

    /// <summary>
    /// Gets a grayscale color (0 = black, 1 = white).
    /// </summary>
    public static Color GetGrayscale(float value)
    {
        value = Math.Clamp(value, 0f, 1f);
        int gray = (int)(value * 255);
        return Color.FromArgb(gray, gray, gray);
    }

    /// <summary>
    /// Adjusts color brightness. Values above 1.0 increase brightness, below 1.0 decrease it.
    /// </summary>
    public static Color AdjustBrightness(Color color, float factor)
    {
        int r = (int)Math.Clamp(color.R * factor, 0, 255);
        int g = (int)Math.Clamp(color.G * factor, 0, 255);
        int b = (int)Math.Clamp(color.B * factor, 0, 255);
        return Color.FromArgb(color.A, r, g, b);
    }

    /// <summary>
    /// Adjusts color saturation. Values above 1.0 increase saturation, below 1.0 reduce it.
    /// </summary>
    public static Color AdjustSaturation(Color color, float factor)
    {
        RgbToHsv(color.R / 255f, color.G / 255f, color.B / 255f, out float h, out float s, out float v);
        s = Math.Clamp(s * factor, 0f, 1f);
        HsvToRgb(h, s, v, out float r, out float g, out float b);

        return Color.FromArgb(color.A, (int)r, (int)g, (int)b);
    }

    /// <summary>
    /// Gets the complementary (opposite) color on the color wheel.
    /// </summary>
    public static Color GetComplementaryColor(Color color)
    {
        RgbToHsv(color.R / 255f, color.G / 255f, color.B / 255f, out float h, out float s, out float v);
        h = (h + 180) % 360;
        HsvToRgb(h, s, v, out float r, out float g, out float b);

        return Color.FromArgb(color.A, (int)r, (int)g, (int)b);
    }

    /// <summary>
    /// Converts Color to hex string format (#RRGGBB).
    /// </summary>
    public static string ColorToHex(Color color)
    {
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    /// <summary>
    /// Converts hex string to Color.
    /// </summary>
    public static Color HexToColor(string hex)
    {
        hex = hex.TrimStart('#');
        if (hex.Length != 6)
            throw new ArgumentException("Hex color must be 6 characters long.");

        int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        return Color.FromArgb(r, g, b);
    }
}
