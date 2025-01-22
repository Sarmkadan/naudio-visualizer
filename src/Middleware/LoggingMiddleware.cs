#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using NAudioVisualizer.Infrastructure;

namespace NAudioVisualizer.Middleware;

/// <summary>
/// Middleware for logging operations across the application pipeline.
/// Captures timing information, method names, and operation status.
/// </summary>
public class LoggingMiddleware
{
    private readonly Logger _logger;
    private readonly bool _enableTimingInfo;
    private const string LogFormat = "[{0:yyyy-MM-dd HH:mm:ss.fff}] {1} - {2}";

    /// <summary>
    /// Initializes a new instance of the logging middleware.
    /// </summary>
    public LoggingMiddleware(Logger logger, bool enableTimingInfo = true)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _enableTimingInfo = enableTimingInfo;
    }

    /// <summary>
    /// Executes an operation with logging. Captures execution time and any exceptions.
    /// </summary>
    public T Execute<T>(string operationName, Func<T> operation)
    {
        if (operation is null)
            throw new ArgumentNullException(nameof(operation));

        var startTime = DateTime.UtcNow;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            _logger.Debug($"[START] Operation: {operationName}");
            var result = operation();
            stopwatch.Stop();

            string timing = _enableTimingInfo ? $" (completed in {stopwatch.ElapsedMilliseconds}ms)" : "";
            _logger.Debug($"[SUCCESS] Operation: {operationName}{timing}");

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            string timing = _enableTimingInfo ? $" (failed after {stopwatch.ElapsedMilliseconds}ms)" : "";
            _logger.Error($"[ERROR] Operation: {operationName}{timing} - {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Executes an asynchronous operation with logging.
    /// </summary>
    public async System.Threading.Tasks.Task<T> ExecuteAsync<T>(string operationName, Func<System.Threading.Tasks.Task<T>> operation)
    {
        if (operation is null)
            throw new ArgumentNullException(nameof(operation));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            _logger.Debug($"[START] Async Operation: {operationName}");
            var result = await operation();
            stopwatch.Stop();

            string timing = _enableTimingInfo ? $" (completed in {stopwatch.ElapsedMilliseconds}ms)" : "";
            _logger.Debug($"[SUCCESS] Async Operation: {operationName}{timing}");

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            string timing = _enableTimingInfo ? $" (failed after {stopwatch.ElapsedMilliseconds}ms)" : "";
            _logger.Error($"[ERROR] Async Operation: {operationName}{timing} - {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Executes an operation without a return value with logging.
    /// </summary>
    public void Execute(string operationName, Action operation)
    {
        if (operation is null)
            throw new ArgumentNullException(nameof(operation));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            _logger.Debug($"[START] Operation: {operationName}");
            operation();
            stopwatch.Stop();

            string timing = _enableTimingInfo ? $" (completed in {stopwatch.ElapsedMilliseconds}ms)" : "";
            _logger.Debug($"[SUCCESS] Operation: {operationName}{timing}");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            string timing = _enableTimingInfo ? $" (failed after {stopwatch.ElapsedMilliseconds}ms)" : "";
            _logger.Error($"[ERROR] Operation: {operationName}{timing} - {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Executes an asynchronous operation without a return value with logging.
    /// </summary>
    public async System.Threading.Tasks.Task ExecuteAsync(string operationName, Func<System.Threading.Tasks.Task> operation)
    {
        if (operation is null)
            throw new ArgumentNullException(nameof(operation));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            _logger.Debug($"[START] Async Operation: {operationName}");
            await operation();
            stopwatch.Stop();

            string timing = _enableTimingInfo ? $" (completed in {stopwatch.ElapsedMilliseconds}ms)" : "";
            _logger.Debug($"[SUCCESS] Async Operation: {operationName}{timing}");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            string timing = _enableTimingInfo ? $" (failed after {stopwatch.ElapsedMilliseconds}ms)" : "";
            _logger.Error($"[ERROR] Async Operation: {operationName}{timing} - {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Logs custom performance metrics for analysis.
    /// </summary>
    public void LogMetric(string metricName, double value, string? unit = null)
    {
        string unitStr = string.IsNullOrEmpty(unit) ? "" : $" {unit}";
        _logger.Info($"[METRIC] {metricName}: {value:F2}{unitStr}");
    }

    /// <summary>
    /// Logs a debug message with timestamp.
    /// </summary>
    public void LogDebug(string message)
    {
        _logger.Debug(message);
    }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    public void LogInfo(string message)
    {
        _logger.Info(message);
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    public void LogWarning(string message)
    {
        _logger.Warn(message);
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    public void LogError(string message)
    {
        _logger.Error(message);
    }
}
