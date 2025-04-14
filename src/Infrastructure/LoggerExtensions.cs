#nullable enable

// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =====================================================================

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NAudioVisualizer.Infrastructure;

/// <summary>
/// Extension methods for <see cref="Logger"/> class providing convenient logging functionality.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Logs a debug message with optional formatted string and arguments.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="message">Format string.</param>
    /// <param name="args">Format arguments.</param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static void Debug(this Logger logger, string message, params object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(message);

        if (logger.MinimumLevel <= LogLevel.Debug)
        {
            string formattedMessage = args is null || args.Length == 0
                ? message
                : string.Format(message, args);
            logger.Debug(formattedMessage);
        }
    }

    /// <summary>
    /// Logs an information message with optional formatted string and arguments.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="message">Format string.</param>
    /// <param name="args">Format arguments.</param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static void Info(this Logger logger, string message, params object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(message);

        if (logger.MinimumLevel <= LogLevel.Info)
        {
            string formattedMessage = args is null || args.Length == 0
                ? message
                : string.Format(message, args);
            logger.Info(formattedMessage);
        }
    }

    /// <summary>
    /// Logs a warning message with optional formatted string and arguments.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="message">Format string.</param>
    /// <param name="args">Format arguments.</param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static void Warn(this Logger logger, string message, params object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(message);

        if (logger.MinimumLevel <= LogLevel.Warn)
        {
            string formattedMessage = args is null || args.Length == 0
                ? message
                : string.Format(message, args);
            logger.Warn(formattedMessage);
        }
    }

    /// <summary>
    /// Logs an error message with optional formatted string and arguments.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="message">Format string.</param>
    /// <param name="exception">Optional exception to include.</param>
    /// <param name="args">Format arguments.</param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static void Error(this Logger logger, string message, Exception? exception = null, params object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(message);

        if (logger.MinimumLevel <= LogLevel.Error)
        {
            string formattedMessage = args is null || args.Length == 0
                ? message
                : string.Format(message, args);
            logger.Error(formattedMessage, exception);
        }
    }

    /// <summary>
    /// Logs a critical message with optional formatted string and arguments.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="message">Format string.</param>
    /// <param name="exception">Optional exception to include.</param>
    /// <param name="args">Format arguments.</param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static void Critical(this Logger logger, string message, Exception? exception = null, params object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(message);

        if (logger.MinimumLevel <= LogLevel.Critical)
        {
            string formattedMessage = args is null || args.Length == 0
                ? message
                : string.Format(message, args);
            logger.Critical(formattedMessage, exception);
        }
    }

    /// <summary>
    /// Logs a method entry and exit with timing information for performance monitoring.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="methodName">Name of the calling method (automatically populated).</param>
    /// <returns>An <see cref="IDisposable"/> that logs method exit when disposed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
    public static IDisposable MethodScope(this Logger logger, [CallerMemberName] string methodName = "")
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Info("Entering method: {MethodName}", methodName);
        var stopwatch = Stopwatch.StartNew();

        return new MethodScopeDisposable(logger, methodName, stopwatch);
    }

    /// <summary>
    /// Logs a message with the current execution time and optional formatted content.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="operationName">Name of the operation being timed.</param>
    /// <param name="action">Action to execute and time.</param>
    /// <param name="args">Format arguments for operation name.</param>
    /// <returns>Time taken for the operation in milliseconds.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="operationName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public static long Time(this Logger logger, string operationName, Action action, params object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(operationName);
        ArgumentNullException.ThrowIfNull(action);

        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();

        string formattedOperation = args is null || args.Length == 0
            ? operationName
            : string.Format(operationName, args);

        logger.Info("Operation '{Operation}' completed in {Elapsed}ms", formattedOperation, stopwatch.ElapsedMilliseconds);

        return stopwatch.ElapsedMilliseconds;
    }

    /// <summary>
    /// Conditionally logs a message based on a condition.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="condition">Condition to check.</param>
    /// <param name="message">Message to log if condition is true.</param>
    /// <param name="args">Format arguments.</param>
    /// <returns>True if message was logged, false otherwise.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static bool If(this Logger logger, bool condition, string message, params object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(message);

        if (condition && logger.MinimumLevel <= LogLevel.Info)
        {
            string formattedMessage = args is null || args.Length == 0
                ? message
                : string.Format(message, args);
            logger.Info(formattedMessage);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Helper class for method scope tracking.
    /// </summary>
    private sealed class MethodScopeDisposable : IDisposable
    {
        private readonly Logger _logger;
        private readonly string _methodName;
        private readonly Stopwatch _stopwatch;
        private bool _isDisposed;

        public MethodScopeDisposable(Logger logger, string methodName, Stopwatch stopwatch)
        {
            _logger = logger;
            _methodName = methodName;
            _stopwatch = stopwatch;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _stopwatch.Stop();
            _logger.Info("Exiting method: {MethodName} - took {Elapsed}ms", _methodName, _stopwatch.ElapsedMilliseconds);
            _isDisposed = true;
        }
    }
}