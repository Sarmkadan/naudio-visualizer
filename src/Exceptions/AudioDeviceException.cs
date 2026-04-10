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

    public AudioDeviceException(string message) : base(message) { }

    public AudioDeviceException(string message, int deviceIndex) : base(message)
    {
        DeviceIndex = deviceIndex;
    }

    public AudioDeviceException(string message, Exception innerException)
        : base(message, innerException) { }

    public AudioDeviceException(string message, int deviceIndex, Exception innerException)
        : base(message, innerException)
    {
        DeviceIndex = deviceIndex;
    }
}
