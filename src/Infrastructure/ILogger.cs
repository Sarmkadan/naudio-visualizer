#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;

namespace NAudioVisualizer.Infrastructure
{
    /// <summary>
    /// Minimal logger abstraction used throughout the application.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets or sets the minimum log level that will be emitted.
        /// </summary>
        LogLevel MinimumLevel { get; set; }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The debug message to log.</param>
        void Debug(string message);

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="message">The information message to log.</param>
        void Info(string message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        void Warn(string message);

        /// <summary>
        /// Logs an error message, optionally with an exception.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="exception">The exception associated with the error, if any.</param>
        void Error(string message, Exception? exception = null);

        /// <summary>
        /// Logs a critical message, optionally with an exception.
        /// </summary>
        /// <param name="message">The critical message to log.</param>
        /// <param name="exception">The exception associated with the critical message, if any.</param>
        void Critical(string message, Exception? exception = null);
    }
}
