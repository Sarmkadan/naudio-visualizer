#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NAudioVisualizer.Utilities;

/// <summary>
/// Provides performance profiling and timing analysis capabilities.
/// Tracks execution time for operations and generates performance reports.
/// </summary>
public class PerformanceProfiler
{
    /// <summary>
    /// Stores aggregated timing data for a single operation.
    /// </summary>
    private class PerformanceData
    {
        /// <summary>
        /// The name of the operation being tracked.
        /// </summary>
        public string OperationName { get; set; } = string.Empty;

        /// <summary>
        /// The total elapsed time in milliseconds across all recorded calls.
        /// </summary>
        public long TotalMs { get; set; }

        /// <summary>
        /// The number of times the operation has been recorded.
        /// </summary>
        public int CallCount { get; set; }

        /// <summary>
        /// The minimum recorded elapsed time in milliseconds.
        /// </summary>
        public long MinMs { get; set; }

        /// <summary>
        /// The maximum recorded elapsed time in milliseconds.
        /// </summary>
        public long MaxMs { get; set; }

        /// <summary>
        /// The collection of recorded elapsed time samples in milliseconds.
        /// </summary>
        public readonly List<long> Samples = new();
    }

    private readonly Dictionary<string, PerformanceData> _metrics;
    private readonly Stopwatch _stopwatch;
    private readonly string _sessionName;

    /// <summary>
    /// Initializes a new instance of the performance profiler.
    /// </summary>
    /// <param name="sessionName">The name of the session.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="sessionName"/> is null.</exception>
    public PerformanceProfiler(string sessionName = "Default")
    {
        ArgumentNullException.ThrowIfNull(sessionName);
        _metrics = new Dictionary<string, PerformanceData>();
        _stopwatch = new Stopwatch();
        _sessionName = sessionName;
    }

    /// <summary>
    /// Starts timing an operation.
    /// Returns a disposable token that stops timing when disposed.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <returns>A timing token.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="operationName"/> is null, empty, or whitespace.</exception>
    public TimingToken StartTimer(string operationName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);
        return new TimingToken(this, operationName);
    }

    /// <summary>
    /// Records the execution time for an operation.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <param name="elapsedMs">The elapsed time in milliseconds.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="operationName"/> is null, empty, or whitespace.</exception>
    public void RecordTime(string operationName, long elapsedMs)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);

        lock (_metrics)
        {
            if (!_metrics.TryGetValue(operationName, out var data))
            {
                data = new PerformanceData
                {
                    OperationName = operationName,
                    MinMs = long.MaxValue,
                    MaxMs = long.MinValue
                };
                _metrics[operationName] = data;
            }

            data.TotalMs += elapsedMs;
            data.CallCount++;
            data.MinMs = Math.Min(data.MinMs, elapsedMs);
            data.MaxMs = Math.Max(data.MaxMs, elapsedMs);
            data.Samples.Add(elapsedMs);

            // Keep only last 1000 samples to avoid memory issues
            if (data.Samples.Count > 1000)
                data.Samples.RemoveAt(0);
        }
    }

    /// <summary>
    /// Gets the average execution time for an operation.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <returns>The average execution time in milliseconds, or 0 if the operation was not found.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="operationName"/> is null, empty, or whitespace.</exception>
    public double GetAverageTime(string operationName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);
        lock (_metrics)
        {
            if (!_metrics.TryGetValue(operationName, out var data))
                return 0;

            return data.CallCount > 0 ? (double)data.TotalMs / data.CallCount : 0;
        }
    }

    /// <summary>
    /// Gets the total execution time for an operation.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <returns>The total execution time in milliseconds, or 0 if the operation was not found.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="operationName"/> is null, empty, or whitespace.</exception>
    public long GetTotalTime(string operationName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);
        lock (_metrics)
        {
            return _metrics.TryGetValue(operationName, out var data) ? data.TotalMs : 0;
        }
    }

    /// <summary>
    /// Gets the number of times an operation was called.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <returns>The number of times the operation was called, or 0 if the operation was not found.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="operationName"/> is null, empty, or whitespace.</exception>
    public int GetCallCount(string operationName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);
        lock (_metrics)
        {
            return _metrics.TryGetValue(operationName, out var data) ? data.CallCount : 0;
        }
    }

    /// <summary>
    /// Gets the minimum execution time for an operation.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <returns>The minimum execution time in milliseconds, or 0 if the operation was not found or has no samples.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="operationName"/> is null, empty, or whitespace.</exception>
    public long GetMinTime(string operationName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);
        lock (_metrics)
        {
            if (!_metrics.TryGetValue(operationName, out var data) || data.CallCount == 0)
                return 0;

            return data.MinMs;
        }
    }

    /// <summary>
    /// Gets the maximum execution time for an operation.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <returns>The maximum execution time in milliseconds, or 0 if the operation was not found or has no samples.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="operationName"/> is null, empty, or whitespace.</exception>
    public long GetMaxTime(string operationName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);
        lock (_metrics)
        {
            if (!_metrics.TryGetValue(operationName, out var data) || data.CallCount == 0)
                return 0;

            return data.MaxMs;
        }
    }

    /// <summary>
    /// Gets the median execution time for an operation.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <returns>The median execution time in milliseconds, or 0 if the operation was not found or has no samples.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="operationName"/> is null, empty, or whitespace.</exception>
    public long GetMedianTime(string operationName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);
        lock (_metrics)
        {
            if (!_metrics.TryGetValue(operationName, out var data) || data.Samples.Count == 0)
                return 0;

            var sorted = data.Samples.OrderBy(x => x).ToList();
            int count = sorted.Count;
            return count % 2 == 0
                ? (sorted[count / 2 - 1] + sorted[count / 2]) / 2
                : sorted[count / 2];
        }
    }

    /// <summary>
    /// Gets all recorded metrics as a formatted string.
    /// </summary>
    /// <returns>A formatted string containing the performance report.</returns>
    public string GetReport()
    {
        var lines = new List<string>();
        lines.Add($"\n╔═════════════════════════════════════════════════════════════════════════════════╗");
        lines.Add($"║ Performance Report: {_sessionName,-64} ║");
        lines.Add($"╚═════════════════════════════════════════════════════════════════════════════════╝\n");

        lock (_metrics)
        {
            if (_metrics.Count == 0)
            {
                lines.Add("No performance data collected.\n");
                return string.Join("\n", lines);
            }

            lines.Add("Operation                    | Calls | Total(ms) | Avg(ms) | Min(ms) | Max(ms) | Median(ms)");
            lines.Add("".PadRight(100, '-'));

            foreach (var kvp in _metrics.OrderByDescending(x => x.Value.TotalMs))
            {
                var data = kvp.Value;
                double avg = data.CallCount > 0 ? (double)data.TotalMs / data.CallCount : 0;
                long median = GetMedianTime(data.OperationName);

                string line = $"{data.OperationName,-28} | {data.CallCount,5} | {data.TotalMs,9} | " +
                              $"{avg,7:F2} | {data.MinMs,7} | {data.MaxMs,7} | {median,9}";
                lines.Add(line);
            }

            lines.Add("");
        }

        return string.Join("\n", lines);
    }

    /// <summary>
    /// Clears all recorded metrics.
    /// </summary>
    public void Clear()
    {
        lock (_metrics)
        {
            _metrics.Clear();
        }
    }

    /// <summary>
    /// Disposable token for automatic timing with using statement.
    /// </summary>
    public class TimingToken : IDisposable
    {
        private readonly PerformanceProfiler _profiler;
        private readonly string _operationName;
        private readonly Stopwatch _stopwatch;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the TimingToken class.
        /// </summary>
        /// <param name="profiler">The performance profiler instance.</param>
        /// <param name="operationName">The name of the operation to time.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="profiler"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="operationName"/> is null, empty, or whitespace.</exception>
        public TimingToken(PerformanceProfiler profiler, string operationName)
        {
            ArgumentNullException.ThrowIfNull(profiler);
            ArgumentException.ThrowIfNullOrWhiteSpace(operationName);
            _profiler = profiler;
            _operationName = operationName;
            _stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Returns a string representation of the timing token, showing the operation name and elapsed milliseconds.
        /// </summary>
        /// <returns>A string in the format "OperationName: ElapsedMillisecondsms".</returns>
        public override string ToString()
        {
            return $"{_operationName}: {_stopwatch.ElapsedMilliseconds}ms";
        }

        /// <summary>
        /// Stops the timer and records the elapsed time.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _stopwatch.Stop();
            _profiler.RecordTime(_operationName, _stopwatch.ElapsedMilliseconds);
            _disposed = true;
        }
    }
}