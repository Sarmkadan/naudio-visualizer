#nullable enable

using System;
using System.Text.Json;

namespace NAudioVisualizer.Workers;

/// <summary>
/// Provides JSON serialization extensions for <see cref="ProcessingTask"/>.
/// </summary>
public static class ProcessingTaskExtensions
{
    /// <summary>
    /// Serializes the public scalar properties of a <see cref="ProcessingTask"/> to JSON.
    /// </summary>
    /// <param name="task">The processing task to serialize.</param>
    /// <returns>A JSON string containing the task name and creation timestamp.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>.</exception>
    public static string ToJson(this ProcessingTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        return JsonSerializer.Serialize(new
        {
            task.Name,
            task.CreatedAt
        });
    }
}
