#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.IO;
using System.Text;

namespace NAudioVisualizer.Infrastructure;

/// <summary>
/// Simple logging utility for application diagnostics and debugging.
/// </summary>
public sealed class Logger : IDisposable
{
    private readonly string _logFilePath;
    private StreamWriter? _writer;
    private readonly bool _writeToConsole;
    private bool _isDisposed;

    public LogLevel MinimumLevel { get; set; } = LogLevel.Info;

    /// <summary>
    /// Initializes a new logger instance.
    /// </summary>
    public Logger(string? logFilePath = null, bool writeToConsole = true)
    {
        _logFilePath = logFilePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "app.log");
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
        }
    }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    public void Debug(string message) => Log(LogLevel.Debug, message);

    /// <summary>
    /// Logs an information message.
    /// </summary>
    public void Info(string message) => Log(LogLevel.Info, message);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    public void Warn(string message) => Log(LogLevel.Warn, message);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    public void Error(string message, Exception? exception = null)
    {
        var formattedMessage = FormatExceptionMessage(message, exception);
        Log(LogLevel.Error, formattedMessage);
    }

    /// <summary>
    /// Logs a critical message.
    /// </summary>
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
        ThrowIfDisposed();

        if (level < MinimumLevel)
            return;

        string logMessage = FormatLogMessage(level, message);

        WriteLogMessage(logMessage);
    }

    private string FormatLogMessage(LogLevel level, string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        return $"[{timestamp}] [{level}] {message}";
    }

    private void WriteLogMessage(string message)
    {
        if (_writeToConsole)
        {
            Console.WriteLine(message);
        }

        _writer?.WriteLine(message);
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(GetType().Name);
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _writer?.Dispose();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Log level enumeration.
/// </summary>
public enum LogLevel
{
    Debug = 0,
    Info = 1,
    Warn = 2,
    Error = 3,
    Critical = 4
}