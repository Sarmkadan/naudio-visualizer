#nullable enable

// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Text.Json;
using System.Text.Json.Serialization;

namespace NAudioVisualizer.Utilities;

/// <summary>
/// Provides System.Text.Json serialization extensions for MathUtility.
/// </summary>
public static class MathUtilityJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    /// <summary>
    /// Serializes the MathUtility type information to a JSON string.
    /// </summary>
    /// <param name="indented">Whether to format the JSON with indentation.</param>
    /// <returns>A JSON string representation of the MathUtility type.</returns>
    public static string ToJson(bool indented = false)
    {
        var options = indented
            ? new JsonSerializerOptions(_jsonOptions)
            {
                WriteIndented = true
            }
            : _jsonOptions;

        var typeInfo = new
        {
            Type = "MathUtility",
            Methods = new[]
            {
                new { Name = "FrequencyToMidiNote", Signature = "(float frequency) -> int" },
                new { Name = "MidiNoteToFrequency", Signature = "(int midiNote) -> float" },
                new { Name = "AmplitudeToDb", Signature = "(float amplitude) -> float" },
                new { Name = "DbToAmplitude", Signature = "(float db) -> float" },
                new { Name = "CalculateRms", Signature = "(float[] signal) -> float" },
                new { Name = "CalculatePeak", Signature = "(float[] signal) -> float" },
                new { Name = "LogScale", Signature = "(float value, float minValue = 1) -> float" },
                new { Name = "PowerScale", Signature = "(float value, float gamma = 2) -> float" },
                new { Name = "ApplyHannWindow", Signature = "(float[] signal) -> void" },
                new { Name = "ApplyHammingWindow", Signature = "(float[] signal) -> void" },
                new { Name = "NextPowerOf2", Signature = "(int n) -> int" },
                new { Name = "IsPowerOf2", Signature = "(int n) -> bool" },
                new { Name = "Lerp", Signature = "(float a, float b, float t) -> float" },
                new { Name = "MapRange", Signature = "(float value, float fromMin, float fromMax, float toMin, float toMax) -> float" },
                new { Name = "Distance", Signature = "(float x1, float y1, float x2, float y2) -> float" }
            }
        };

        return JsonSerializer.Serialize(typeInfo, options);
    }

    /// <summary>
    /// Deserializes a JSON string to validate MathUtility type information.
    /// Since MathUtility is a static class, this validates JSON and returns a success indicator.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>A success indicator string if deserialization succeeds; otherwise, null.</returns>
    public static string? FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || json == "null")
            return null;

        try
        {
            using (JsonDocument.Parse(json))
            {
                return "MathUtility";
            }
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to deserialize a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">The resulting value if deserialization succeeds.</param>
    /// <returns>True if JSON is valid; otherwise, false.</returns>
    public static bool TryFromJson(string json, out string? value)
    {
        value = null;

        if (string.IsNullOrWhiteSpace(json) || json == "null")
            return false;

        try
        {
            using (JsonDocument.Parse(json))
            {
                value = "MathUtility";
                return true;
            }
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
