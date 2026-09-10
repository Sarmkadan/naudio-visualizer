#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.IO;
using System.Text;

namespace NAudioVisualizer.Infrastructure
{
    /// <summary>
    /// Simple logging utility for application diagnostics and debugging.
    /// </summary>
    public sealed class Logger : IDisposable, ILogger
    {
        private readonly object _writeLock = new object();
        private readonly string _logFilePath;
        private StreamWriter? _writer;
        private readonly bool _writeToConsole;
        private bool _logFileInitializationFailed;
        private bool _isDisposed;

        /// <summary>
        /// Default directory name for log files.
        /// </summary>
        private const string DefaultLogDirectoryName = "logs";

        /// <summary>
        /// Default file name for log files.
        /// </summary>
        private const string DefaultLogFileName = "app.log";

        /// <summary>
        /// Format string for timestamps in log messages.
        /// </summary>
        private const string TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public LogLevel MinimumLevel { get; set; } = LogLevel.Info;

        /// <summary>
        /// Initializes a new logger instance.
        /// </summary>
        /// <param name="logFilePath">The path to the log file. If null, defaults to a file named "app.log" in a "logs" subdirectory of the application base directory.</param>
        /// <param name="writeToConsole">Whether to write log messages to the console.</param>
        public Logger(string? logFilePath = null, bool writeToConsole = true)
        {
            _logFilePath = logFilePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultLogDirectoryName, DefaultLogFileName);
            _writeToConsole = writeToConsole;

            InitializeLogFile();
        }

        private void InitializeLogFile()
        {
            try
            {
                string? directory = Path.GetDirectoryName(_logFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                _writer = new StreamWriter(_logFilePath, true, Encoding.UTF8)
                {
                    AutoFlush = true
                };
            }
            catch
            {
                _writer = null;
                _logFileInitializationFailed = true;
            }
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The debug message to log.</param>
        public void Debug(string message) => Log(LogLevel.Debug, message);

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="message">The information message to log.</param>
        public void Info(string message) => Log(LogLevel.Info, message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public void Warn(string message) => Log(LogLevel.Warn, message);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="exception">The exception associated with the error, if any.</param>
        public void Error(string message, Exception? exception = null)
        {
            var formattedMessage = FormatExceptionMessage(message, exception);
            Log(LogLevel.Error, formattedMessage);
        }

        /// <summary>
        /// Logs a critical message.
        /// </summary>
        /// <param name="message">The critical message to log.</param>
        /// <param name="exception">The exception associated with the critical message, if any.</param>
        public void Critical(string message, Exception? exception = null)
        {
            var formattedMessage = FormatExceptionMessage(message, exception);
            Log(LogLevel.Critical, formattedMessage);
        }

        private string FormatExceptionMessage(string message, Exception? exception)
        {
            if (exception is null)
                return message;

            var sb = new StringBuilder(message);
            sb.AppendLine();
            sb.Append("Exception: ").AppendLine(exception.GetType().Name);
            sb.Append("Message: ").AppendLine(exception.Message);
            sb.Append("StackTrace: ").AppendLine(exception.StackTrace);

            return sb.ToString();
        }

        /// <summary>
        /// Core logging method.
        /// </summary>
        private void Log(LogLevel level, string message)
        {
            lock (_writeLock)
            {
                if (_isDisposed)
                    return;

                if (level < MinimumLevel)
                    return;

                string logMessage = FormatLogMessage(level, message);

                WriteLogMessage(logMessage);
            }
        }

        private string FormatLogMessage(LogLevel level, string message)
        {
            string timestamp = DateTime.Now.ToString(TimestampFormat);
            return $"[{timestamp}] [{level}] {message}";
        }

        private void WriteLogMessage(string message)
        {
            if (_writeToConsole)
            {
                Console.WriteLine(message);
            }

            if (!_logFileInitializationFailed)
            {
                _writer?.WriteLine(message);
            }
        }

        public void Dispose()
        {
            lock (_writeLock)
            {
                if (_isDisposed)
                    return;

                _writer?.Dispose();
                _isDisposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Log level enumeration.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Debug level for detailed diagnostic information.
        /// </summary>
        Debug = 0,
        /// <summary>
        /// Info level for general informational messages.
        /// </summary>
        Info = 1,
        /// <summary>
        /// Warn level for warning messages.
        /// </summary>
        Warn = 2,
        /// <summary>
        /// Error level for error messages.
        /// </summary>
        Error = 3,
        /// <summary>
        /// Critical level for critical messages.
        /// </summary>
        Critical = 4
    }
}