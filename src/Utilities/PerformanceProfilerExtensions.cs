#nullable enable

using System;

namespace NAudioVisualizer.Utilities;

/// <summary>
/// Provides convenience methods for measuring operations with a <see cref="PerformanceProfiler"/>.
/// </summary>
public static class PerformanceProfilerExtensions
{
    /// <summary>
    /// Measures an operation and returns its result.
    /// </summary>
    /// <typeparam name="T">The type of result returned by the operation.</typeparam>
    /// <param name="profiler">The profiler used to measure the operation.</param>
    /// <param name="operationName">The name of the operation to measure.</param>
    /// <param name="action">The operation to execute.</param>
    /// <returns>The result returned by <paramref name="action"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="profiler"/> or <paramref name="action"/> is <see langword="null"/>.
    /// </exception>
    public static T Measure<T>(this PerformanceProfiler profiler, string operationName, Func<T> action)
    {
        ArgumentNullException.ThrowIfNull(profiler);
        ArgumentNullException.ThrowIfNull(action);

        using (profiler.StartTimer(operationName))
        {
            return action();
        }
    }
}
