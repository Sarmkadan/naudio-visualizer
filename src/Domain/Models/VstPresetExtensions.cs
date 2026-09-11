#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Extension methods for <see cref="VstPreset"/>.
/// </summary>
public static class VstPresetExtensions
{
    /// <summary>
    /// Attempts to get the parameter value by name. Since <see cref="VstPreset"/> does not store parameter names,
    /// this method always returns false.
    /// </summary>
    /// <param name="preset">The preset.</param>
    /// <param name="name">The parameter name.</param>
    /// <param name="value">When this method returns true, contains the parameter value; otherwise, the default value.</param>
    /// <returns>false because parameter names are not stored in <see cref="VstPreset"/>.</returns>
    public static bool TryGetParameterValue(this VstPreset preset, string name, out float value)
    {
        value = 0f;
        return false;
    }

    /// <summary>
    /// Gets the parameter names. Since <see cref="VstPreset"/> does not store parameter names,
    /// this method returns an empty enumeration.
    /// </summary>
    /// <param name="preset">The preset.</param>
    /// <returns>An empty enumeration of parameter names.</returns>
    public static IEnumerable<string> GetParameterNames(this VstPreset preset)
    {
        return Enumerable.Empty<string>();
    }

    /// <summary>
    /// Returns a summary string representing the preset.
    /// </summary>
    /// <param name="preset">The preset.</param>
    /// <returns>A summary string.</returns>
    public static string ToSummaryString(this VstPreset preset)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(preset.Category))
            sb.Append($"[{preset.Category}] ");
        sb.Append(preset.Name);

        if (!string.IsNullOrWhiteSpace(preset.Description))
        {
            sb.AppendLine();
            sb.Append(preset.Description);
        }

        sb.AppendLine();
        sb.Append($"Author: {(preset.IsFactory ? "Factory" : preset.Author ?? "Unknown")}");
        sb.AppendLine();
        sb.Append($"Created: {preset.CreatedAt:yyyy-MM-dd}");
        sb.AppendLine();
        sb.Append($"Modified: {preset.ModifiedAt:yyyy-MM-dd}");
        sb.AppendLine();
        sb.Append($"Parameters: {preset.ParameterValues.Count}");

        if (preset.PluginChunk is { Length: > 0 })
        {
            sb.AppendLine();
            sb.Append($"Chunk: {preset.PluginChunk.Length} bytes");
        }

        return sb.ToString();
    }
}