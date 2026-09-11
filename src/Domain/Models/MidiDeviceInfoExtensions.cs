#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides extension methods for <see cref="MidiDeviceInfo"/> to simplify common MIDI device operations.
/// </summary>
public static class MidiDeviceInfoExtensions
{
    /// <summary>
    /// Returns a string representation of the MIDI device info suitable for display in UI.
    /// </summary>
    /// <param name="device">The MIDI device info to format.</param>
    /// <returns>A formatted string containing device index, name, and availability status.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="device"/> is null.</exception>
    public static string ToDisplayString(this MidiDeviceInfo device)
    {
        ArgumentNullException.ThrowIfNull(device);

        string availability = device.IsAvailable ? "available" : "unavailable";
        string validity = device.IsValid() ? "valid" : "invalid";
        return $"[{device.Index}] {device.ProductName} ({availability}, {validity})";
    }

    /// <summary>
    /// Determines whether the MIDI device is usable (both available and valid).
    /// </summary>
    /// <param name="device">The MIDI device info to check.</param>
    /// <returns><see langword="true"/> if the device is both available and valid; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="device"/> is null.</exception>
    public static bool IsUsable(this MidiDeviceInfo device)
    {
        ArgumentNullException.ThrowIfNull(device);
        return device.IsAvailable && device.IsValid();
    }

    /// <summary>
    /// Filters a sequence of MIDI device info objects by product name (case-insensitive partial match).
    /// </summary>
    /// <param name="devices">The sequence of MIDI device info objects to filter.</param>
    /// <param name="namePart">The substring to search for in device product names.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="MidiDeviceInfo"/> containing devices whose product name contains the specified substring (case-insensitive).</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="devices"/> is null.</exception>
    public static IEnumerable<MidiDeviceInfo> FindByName(this IEnumerable<MidiDeviceInfo> devices, string namePart)
    {
        ArgumentNullException.ThrowIfNull(devices);
        if (string.IsNullOrWhiteSpace(namePart))
            return Enumerable.Empty<MidiDeviceInfo>();

        string searchTerm = namePart.Trim().ToLowerInvariant();
        return devices.Where(d => !string.IsNullOrWhiteSpace(d.ProductName) &&
                                 d.ProductName.ToLowerInvariant().Contains(searchTerm));
    }
}