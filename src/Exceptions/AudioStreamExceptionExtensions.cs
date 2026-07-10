#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =====================================================================

using System;
using System.Text;

namespace NAudioVisualizer.Exceptions;

/// <summary>
/// Extension methods for <see cref="AudioStreamException"/> that provide additional functionality
/// for error handling and diagnostics.
/// </summary>
public static class AudioStreamExceptionExtensions
{
    /// <summary>
    /// Creates a detailed error message that includes the error code and any inner exception information.
    /// </summary>
    /// <param name="exception">The audio stream exception to format.</param>
    /// <returns>A formatted error message string.</returns>
    public static string GetDetailedErrorMessage(this AudioStreamException exception)
    {
        if (exception is null)
            throw new ArgumentNullException(nameof(exception));

        var builder = new StringBuilder();
        builder.AppendLine(exception.Message);
        builder.AppendLine();
        builder.Append("Error Code: ");
        builder.AppendLine(exception.ErrorCode.ToString());

        if (exception.InnerException != null)
        {
            builder.AppendLine();
            builder.AppendLine("Inner Exception:");
            builder.Append("  Type: ");
            builder.AppendLine(exception.InnerException.GetType().FullName);
            builder.Append("  Message: ");
            builder.AppendLine(exception.InnerException.Message);
        }

        return builder.ToString();
    }

    /// <summary>
    /// Determines whether the exception represents a recoverable audio stream error.
    /// </summary>
    /// <param name="exception">The audio stream exception to check.</param>
    /// <returns>True if the error is potentially recoverable; otherwise, false.</returns>
    public static bool IsRecoverable(this AudioStreamException exception)
    {
        if (exception is null)
            throw new ArgumentNullException(nameof(exception));

        return exception.ErrorCode switch
        {
            AudioStreamErrorCode.BufferUnderrun => true,
            AudioStreamErrorCode.Unknown => true,
            AudioStreamErrorCode.InitializationFailed => true,
            _ => false
        };
    }

    /// <summary>
    /// Determines whether the exception represents a fatal audio stream error that cannot be recovered.
    /// </summary>
    /// <param name="exception">The audio stream exception to check.</param>
    /// <returns>True if the error is fatal; otherwise, false.</returns>
    public static bool IsFatal(this AudioStreamException exception)
    {
        if (exception is null)
            throw new ArgumentNullException(nameof(exception));

        return exception.ErrorCode switch
        {
            AudioStreamErrorCode.DeviceDisconnected => true,
            AudioStreamErrorCode.FormatUnsupported => true,
            AudioStreamErrorCode.PermissionDenied => true,
            AudioStreamErrorCode.HardwareError => true,
            AudioStreamErrorCode.BufferOverflow => true,
            _ => false
        };
    }

    /// <summary>
    /// Creates a new exception with the same error code but a modified message.
    /// </summary>
    /// <param name="exception">The original exception.</param>
    /// <param name="newMessage">The new error message.</param>
    /// <returns>A new AudioStreamException with the updated message.</returns>
    public static AudioStreamException WithMessage(this AudioStreamException exception, string newMessage)
    {
        if (exception is null)
            throw new ArgumentNullException(nameof(exception));
        if (string.IsNullOrWhiteSpace(newMessage))
            throw new ArgumentException("Message cannot be null or whitespace.", nameof(newMessage));

        return new AudioStreamException(newMessage, exception.ErrorCode, exception);
    }

    /// <summary>
    /// Gets a user-friendly description of the error code.
    /// </summary>
    /// <param name="exception">The audio stream exception.</param>
    /// <returns>A user-friendly description of the error.</returns>
    public static string GetUserFriendlyMessage(this AudioStreamException exception)
    {
        if (exception is null)
            throw new ArgumentNullException(nameof(exception));

        return exception.ErrorCode switch
        {
            AudioStreamErrorCode.Unknown => "An unknown error occurred during audio streaming.",
            AudioStreamErrorCode.BufferOverflow => "Audio buffer overflow detected. The system cannot process audio fast enough.",
            AudioStreamErrorCode.BufferUnderrun => "Audio buffer underrun detected. Audio data is not being captured fast enough.",
            AudioStreamErrorCode.DeviceDisconnected => "Audio device was disconnected during streaming.",
            AudioStreamErrorCode.FormatUnsupported => "The requested audio format is not supported by the device.",
            AudioStreamErrorCode.PermissionDenied => "Permission denied when accessing audio device. Check device permissions.",
            AudioStreamErrorCode.InitializationFailed => "Failed to initialize audio device or stream.",
            AudioStreamErrorCode.HardwareError => "Hardware error detected in audio device.",
            _ => "An audio streaming error occurred."
        };
    }
}