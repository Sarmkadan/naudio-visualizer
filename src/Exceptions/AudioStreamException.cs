#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;

namespace NAudioVisualizer.Exceptions;

/// <summary>
/// Thrown when an error occurs during audio streaming or capture.
/// </summary>
public class AudioStreamException : Exception
{
    /// <summary>
    /// Error code for categorization.
    /// </summary>
    public AudioStreamErrorCode ErrorCode { get; set; }

    /// <summary>
    /// Initializes a new instance of the AudioStreamException class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public AudioStreamException(string message) : base(message)
    {
        ErrorCode = AudioStreamErrorCode.Unknown;
    }

    /// <summary>
    /// Initializes a new instance of the AudioStreamException class with a specified error message and error code.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errorCode">The error code associated with this exception.</param>
    public AudioStreamException(string message, AudioStreamErrorCode errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the AudioStreamException class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public AudioStreamException(string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = AudioStreamErrorCode.Unknown;
    }

    /// <summary>
    /// Initializes a new instance of the AudioStreamException class with a specified error message, error code, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errorCode">The error code associated with this exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public AudioStreamException(string message, AudioStreamErrorCode errorCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Error codes for audio stream failures.
/// </summary>
public enum AudioStreamErrorCode
{
    /// <summary>
    /// An unknown error occurred.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The audio buffer overflowed.
    /// </summary>
    BufferOverflow = 1,

    /// <summary>
    /// The audio buffer underran.
    /// </summary>
    BufferUnderrun = 2,

    /// <summary>
    /// The audio device was disconnected.
    /// </summary>
    DeviceDisconnected = 3,

    /// <summary>
    /// The audio format is not supported.
    /// </summary>
    FormatUnsupported = 4,

    /// <summary>
    /// Permission to access the audio device was denied.
    /// </summary>
    PermissionDenied = 5,

    /// <summary>
    /// Failed to initialize the audio device or stream.
    /// </summary>
    InitializationFailed = 6,

    /// <summary>
    /// A hardware error occurred.
    /// </summary>
    HardwareError = 7
}