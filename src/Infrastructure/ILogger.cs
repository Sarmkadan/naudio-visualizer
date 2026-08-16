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
        void Debug(string message);

        /// <summary>
        /// Logs an information message.
        /// </summary>
        void Info(string message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        void Warn(string message);

        /// <summary>
        /// Logs an error message, optionally with an exception.
        /// </summary>
        void Error(string message, Exception? exception = null);

        /// <summary>
        /// Logs a critical message, optionally with an exception.
        /// </summary>
        void Critical(string message, Exception? exception = null);
    }
}
