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

    public AudioStreamException(string message) : base(message)
    {
        ErrorCode = AudioStreamErrorCode.Unknown;
    }

    public AudioStreamException(string message, AudioStreamErrorCode errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public AudioStreamException(string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = AudioStreamErrorCode.Unknown;
    }

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
    Unknown = 0,
    BufferOverflow = 1,
    BufferUnderrun = 2,
    DeviceDisconnected = 3,
    FormatUnsupported = 4,
    PermissionDenied = 5,
    InitializationFailed = 6,
    HardwareError = 7
}
