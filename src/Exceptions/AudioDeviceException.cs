#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;

namespace NAudioVisualizer.Exceptions;

/// <summary>
/// Thrown when an audio device is not found or inaccessible.
/// </summary>
public class AudioDeviceException : Exception
{
    /// <summary>
    /// Device index that caused the error.
    /// </summary>
    public int? DeviceIndex { get; set; }

    /// <summary>
    /// Initializes a new instance of the AudioDeviceException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AudioDeviceException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the AudioDeviceException class with a specified error message and the device index that caused the error.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="deviceIndex">The index of the audio device that caused the error.</param>
    public AudioDeviceException(string message, int deviceIndex) : base(message)
    {
        DeviceIndex = deviceIndex;
    }

    /// <summary>
    /// Initializes a new instance of the AudioDeviceException class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public AudioDeviceException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the AudioDeviceException class with a specified error message, the device index that caused the error, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="deviceIndex">The index of the audio device that caused the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public AudioDeviceException(string message, int deviceIndex, Exception innerException)
        : base(message, innerException)
    {
        DeviceIndex = deviceIndex;
    }
}
