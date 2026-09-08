using System;
using System.Text.Json;

namespace NAudioVisualizer.Caching;

/// <summary>
/// Provides JSON serialization extensions for <see cref="CacheStatistics"/>.
/// </summary>
public static class CacheStatisticsExtensions
{
    /// <summary>
    /// Serializes cache statistics to JSON.
    /// </summary>
    /// <param name="stats">The cache statistics to serialize.</param>
    /// <param name="indented">
    /// <see langword="true"/> to format the JSON with indentation; otherwise, <see langword="false"/>.
    /// </param>
    /// <returns>A JSON representation of <paramref name="stats"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stats"/> is <see langword="null"/>.</exception>
    public static string ToJson(this CacheStatistics stats, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(stats);

        return JsonSerializer.Serialize(stats, new JsonSerializerOptions
        {
            WriteIndented = indented
        });
    }
}
