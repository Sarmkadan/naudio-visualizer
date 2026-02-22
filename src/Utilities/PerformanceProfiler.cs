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
    private class PerformanceData
    {
        public string OperationName { get; set; } = string.Empty;
        public long TotalMs { get; set; }
        public int CallCount { get; set; }
        public long MinMs { get; set; }
        public long MaxMs { get; set; }
        public readonly List<long> Samples = new();
    }

    private readonly Dictionary<string, PerformanceData> _metrics;
    private readonly Stopwatch _stopwatch;
    private readonly string _sessionName;

    /// <summary>
    /// Initializes a new instance of the performance profiler.
    /// </summary>
    public PerformanceProfiler(string sessionName = "Default")
    {
        _metrics = new Dictionary<string, PerformanceData>();
        _stopwatch = new Stopwatch();
        _sessionName = sessionName;
    }

    /// <summary>
    /// Starts timing an operation.
    /// Returns a disposable token that stops timing when disposed.
    /// </summary>
    public TimingToken StartTimer(string operationName)
    {
        return new TimingToken(this, operationName);
    }

    /// <summary>
    /// Records the execution time for an operation.
    /// </summary>
    public void RecordTime(string operationName, long elapsedMs)
    {
        if (string.IsNullOrWhiteSpace(operationName))
            throw new ArgumentException("Operation name cannot be null or empty.", nameof(operationName));

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
    public double GetAverageTime(string operationName)
    {
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
    public long GetTotalTime(string operationName)
    {
        lock (_metrics)
        {
            return _metrics.TryGetValue(operationName, out var data) ? data.TotalMs : 0;
        }
    }

    /// <summary>
    /// Gets the number of times an operation was called.
    /// </summary>
    public int GetCallCount(string operationName)
    {
        lock (_metrics)
        {
            return _metrics.TryGetValue(operationName, out var data) ? data.CallCount : 0;
        }
    }

    /// <summary>
    /// Gets the minimum execution time for an operation.
    /// </summary>
    public long GetMinTime(string operationName)
    {
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
    public long GetMaxTime(string operationName)
    {
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
    public long GetMedianTime(string operationName)
    {
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
    public string GetReport()
    {
        var lines = new List<string>();
        lines.Add($"\n╔════════════════════════════════════════════════════════════════════════════════╗");
        lines.Add($"║ Performance Report: {_sessionName,-64} ║");
        lines.Add($"╚════════════════════════════════════════════════════════════════════════════════╝\n");

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

        public TimingToken(PerformanceProfiler profiler, string operationName)
        {
            _profiler = profiler;
            _operationName = operationName;
            _stopwatch = Stopwatch.StartNew();
        }

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
