#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using NAudioVisualizer.Exceptions;
using NAudioVisualizer.Infrastructure;

namespace NAudioVisualizer.Middleware;

/// <summary>
/// Central exception handler for the application.
/// Provides consistent error handling, recovery, and logging across all operations.
/// Implements a handler registry pattern to support custom handlers per exception type.
/// </summary>
public class ExceptionHandler
{
    private readonly Logger _logger;
    private readonly Dictionary<Type, Func<Exception, ExceptionContext>> _handlers;
    private bool _rethrowUnhandled = true;

    /// <summary>
    /// Initializes a new instance of the exception handler.
    /// </summary>
    public ExceptionHandler(Logger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _handlers = new Dictionary<Type, Func<Exception, ExceptionContext>>();
        RegisterDefaultHandlers();
    }

    /// <summary>
    /// Registers the default exception handlers for domain-specific exceptions.
    /// </summary>
    private void RegisterDefaultHandlers()
    {
        // Handle audio device exceptions
        RegisterHandler<AudioDeviceException>(ex => new ExceptionContext
        {
            Message = $"Audio device error: {ex.Message}",
            Severity = ExceptionSeverity.High,
            IsRecoverable = true,
            SuggestedAction = "Please check your audio device connection and try again."
        });

        // Handle audio stream exceptions
        RegisterHandler<AudioStreamException>(ex => new ExceptionContext
        {
            Message = $"Audio stream error: {ex.Message}",
            Severity = ExceptionSeverity.High,
            IsRecoverable = true,
            SuggestedAction = "The audio stream was interrupted. Attempting to reconnect..."
        });

        // Handle visualization exceptions
        RegisterHandler<VisualizationException>(ex => new ExceptionContext
        {
            Message = $"Visualization error: {ex.Message}",
            Severity = ExceptionSeverity.Medium,
            IsRecoverable = true,
            SuggestedAction = "Visualization update failed. Retrying with different parameters."
        });

        // Handle argument exceptions
        RegisterHandler<ArgumentException>(ex => new ExceptionContext
        {
            Message = $"Invalid argument: {ex.Message}",
            Severity = ExceptionSeverity.Low,
            IsRecoverable = true,
            SuggestedAction = "Please verify the provided parameters and try again."
        });

        // Handle null reference exceptions
        RegisterHandler<NullReferenceException>(ex => new ExceptionContext
        {
            Message = $"Null reference error: {ex.Message}",
            Severity = ExceptionSeverity.Critical,
            IsRecoverable = false,
            SuggestedAction = "This is an internal application error. Please restart the application."
        });

        // Handle out of memory exceptions
        RegisterHandler<OutOfMemoryException>(ex => new ExceptionContext
        {
            Message = "Out of memory error",
            Severity = ExceptionSeverity.Critical,
            IsRecoverable = false,
            SuggestedAction = "The application ran out of memory. Please close other applications and restart."
        });
    }

    /// <summary>
    /// Registers a custom handler for a specific exception type.
    /// </summary>
    public void RegisterHandler<T>(Func<T, ExceptionContext> handler) where T : Exception
    {
        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        _handlers[typeof(T)] = ex => handler((T)ex);
    }

    /// <summary>
    /// Handles an exception with logging and recovery logic.
    /// </summary>
    public ExceptionContext Handle(Exception exception)
    {
        if (exception is null)
            throw new ArgumentNullException(nameof(exception));

        // Look up registered handler
        var exceptionType = exception.GetType();
        if (_handlers.TryGetValue(exceptionType, out var handler))
        {
            var context = handler(exception);
            LogException(context, exception);
            return context;
        }

        // Handle as generic exception
        var genericContext = new ExceptionContext
        {
            Message = exception.Message,
            Severity = ExceptionSeverity.High,
            IsRecoverable = false,
            SuggestedAction = "An unexpected error occurred. Please check the logs for details."
        };

        LogException(genericContext, exception);

        if (_rethrowUnhandled)
            throw exception;

        return genericContext;
    }

    /// <summary>
    /// Handles an exception and executes recovery logic if the exception is recoverable.
    /// </summary>
    public void HandleWithRecovery(Exception exception, Action recoveryAction)
    {
        var context = Handle(exception);

        if (context.IsRecoverable && recoveryAction is not null)
        {
            try
            {
                _logger.Info("Attempting recovery...");
                recoveryAction();
                _logger.Info("Recovery successful.");
            }
            catch (Exception recoveryEx)
            {
                _logger.Error($"Recovery failed: {recoveryEx.Message}");
                if (_rethrowUnhandled)
                    throw new Exception("Recovery from exception failed.", recoveryEx);
            }
        }
    }

    /// <summary>
    /// Attempts to recover from an exception with custom logic.
    /// </summary>
    public T HandleWithRecovery<T>(Exception exception, Func<T> recoveryFunc)
    {
        var context = Handle(exception);

        if (context.IsRecoverable && recoveryFunc is not null)
        {
            try
            {
                _logger.Info("Attempting recovery...");
                var result = recoveryFunc();
                _logger.Info("Recovery successful.");
                return result;
            }
            catch (Exception recoveryEx)
            {
                _logger.Error($"Recovery failed: {recoveryEx.Message}");
                if (_rethrowUnhandled)
                    throw new Exception("Recovery from exception failed.", recoveryEx);
                throw;
            }
        }

        throw exception;
    }

    /// <summary>
    /// Logs the exception context with appropriate severity level.
    /// </summary>
    private void LogException(ExceptionContext context, Exception originalException)
    {
        string logMessage = $"{context.Message} | Recoverable: {context.IsRecoverable}";

        switch (context.Severity)
        {
            case ExceptionSeverity.Debug:
                _logger.Debug(logMessage);
                break;
            case ExceptionSeverity.Low:
                _logger.Info(logMessage);
                break;
            case ExceptionSeverity.Medium:
                _logger.Warn(logMessage);
                break;
            case ExceptionSeverity.High:
                _logger.Error(logMessage);
                break;
            case ExceptionSeverity.Critical:
                _logger.Error($"[CRITICAL] {logMessage}\n{originalException.StackTrace}");
                break;
        }
    }

    /// <summary>
    /// Sets whether unhandled exceptions should be rethrown.
    /// </summary>
    public void SetRethrowUnhandled(bool rethrow) => _rethrowUnhandled = rethrow;
}

/// <summary>
/// Represents the context and severity of an exception.
/// </summary>
public class ExceptionContext
{
    public required string Message { get; init; }
    public ExceptionSeverity Severity { get; init; }
    public bool IsRecoverable { get; init; }
    public string? SuggestedAction { get; init; }
}

/// <summary>
/// Enum representing exception severity levels.
/// </summary>
public enum ExceptionSeverity
{
    Debug = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}
