// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =====================================================================

using System.Text.Json;
using System.Text.Json.Serialization;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides System.Text.Json serialization extensions for SpectrumData.
/// </summary>
public static class SpectrumDataJsonExtensions
{
    /// <summary>
    /// Cached JsonSerializerOptions with camelCase naming policy.
    /// </summary>
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        IncludeFields = true
    };

    /// <summary>
    /// Converts a SpectrumData to a JSON string.
    /// </summary>
    /// <param name="spectrum">The SpectrumData to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation.</param>
    /// <returns>A JSON string representation of the SpectrumData.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="spectrum"/> is null.</exception>
    public static string ToJson(this SpectrumData spectrum, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(spectrum);

        var options = indented
            ? new JsonSerializerOptions(_jsonOptions)
            {
                WriteIndented = true
            }
            : _jsonOptions;

        return JsonSerializer.Serialize(spectrum, options);
    }
}