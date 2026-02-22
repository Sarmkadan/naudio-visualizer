#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Represents an audio input device (microphone, line-in, etc.).
/// </summary>
public class AudioDevice
{
    /// <summary>
    /// Unique device identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Human-readable device name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Device index used by NAudio.
    /// </summary>
    public int DeviceIndex { get; set; }

    /// <summary>
    /// Manufacturer name.
    /// </summary>
    public string Manufacturer { get; set; } = string.Empty;

    /// <summary>
    /// Number of input channels.
    /// </summary>
    public int ChannelCount { get; set; }

    /// <summary>
    /// Supported sample rates (Hz).
    /// </summary>
    public List<int> SupportedSampleRates { get; set; } = [];

    /// <summary>
    /// Default sample rate for this device.
    /// </summary>
    public int DefaultSampleRate { get; set; } = 44100;

    /// <summary>
    /// Bit depth (16, 24, 32).
    /// </summary>
    public int BitDepth { get; set; } = 16;

    /// <summary>
    /// Whether this is the default device.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Whether the device is currently available/connected.
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// Timestamp of last status check.
    /// </summary>
    public DateTime LastStatusCheck { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Device capabilities flags.
    /// </summary>
    public DeviceCapabilities Capabilities { get; set; } = DeviceCapabilities.None;

    /// <summary>
    /// Initializes a new audio device.
    /// </summary>
    public AudioDevice(string name, int deviceIndex, int channelCount)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DeviceIndex = deviceIndex;
        ChannelCount = channelCount;
    }

    /// <summary>
    /// Validates that the device configuration is valid.
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name) &&
               DeviceIndex >= 0 &&
               ChannelCount > 0 &&
               DefaultSampleRate > 0 &&
               BitDepth > 0 &&
               IsAvailable;
    }

    /// <summary>
    /// Checks if a specific sample rate is supported.
    /// </summary>
    public bool SupportsSampleRate(int sampleRate)
    {
        return SupportedSampleRates.Contains(sampleRate);
    }

    /// <summary>
    /// Adds a supported sample rate.
    /// </summary>
    public void AddSupportedSampleRate(int sampleRate)
    {
        if (!SupportedSampleRates.Contains(sampleRate))
        {
            SupportedSampleRates.Add(sampleRate);
            SupportedSampleRates.Sort();
        }
    }

    /// <summary>
    /// Updates the device availability status.
    /// </summary>
    public void UpdateStatus(bool available)
    {
        IsAvailable = available;
        LastStatusCheck = DateTime.UtcNow;
    }

    public override string ToString() => $"{Name} (Device {DeviceIndex}, {ChannelCount} ch, {DefaultSampleRate}Hz)";
}

/// <summary>
/// Device capability flags.
/// </summary>
[Flags]
public enum DeviceCapabilities
{
    None = 0,
    Microphone = 1,
    LineIn = 2,
    Stereo = 4,
    Mono = 8,
    HighResolution = 16,
    RealTime = 32
}
