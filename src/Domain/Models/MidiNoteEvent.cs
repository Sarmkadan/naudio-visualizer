#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Represents a single MIDI note event captured from a MIDI input device,
/// including pitch identity, velocity, and derived visualization properties.
/// </summary>
public sealed class MidiNoteEvent
{
    private static readonly string[] NoteNames = ["C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"];

    /// <summary>Gets the MIDI channel (1–16) on which the note was received.</summary>
    public int Channel { get; init; }

    /// <summary>Gets the MIDI note number (0–127) identifying the pitch.</summary>
    public int NoteNumber { get; init; }

    /// <summary>Gets the human-readable note name including octave, e.g. "A4" or "C#3".</summary>
    public string NoteName { get; init; } = string.Empty;

    /// <summary>Gets the note velocity (0–127). A zero velocity on a NoteOn command is treated as NoteOff.</summary>
    public int Velocity { get; init; }

    /// <summary>Gets a value indicating whether this is a note-on (key press) event.</summary>
    public bool IsNoteOn { get; init; }

    /// <summary>Gets the fundamental frequency in Hz derived from equal-temperament tuning (A4 = 440 Hz).</summary>
    public float Frequency { get; init; }

    /// <summary>Gets the UTC timestamp at which the event was received from the device.</summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>Gets the zero-based index of the MIDI input device that produced this event.</summary>
    public int DeviceIndex { get; init; }

    /// <summary>
    /// Returns the standard note name for the given MIDI note number.
    /// </summary>
    /// <param name="noteNumber">MIDI note number in the range 0–127.</param>
    /// <returns>A string such as "C4" or "A#3".</returns>
    public static string GetNoteName(int noteNumber)
    {
        int octave = (noteNumber / 12) - 1;
        return $"{NoteNames[noteNumber % 12]}{octave}";
    }

    /// <summary>
    /// Calculates the fundamental frequency in Hz for a given MIDI note number using
    /// equal-temperament tuning with A4 (note 69) anchored at 440 Hz.
    /// </summary>
    /// <param name="noteNumber">MIDI note number in the range 0–127.</param>
    /// <returns>The frequency in Hz.</returns>
    public static float GetFrequency(int noteNumber) =>
        440f * MathF.Pow(2f, (noteNumber - 69) / 12f);

    /// <summary>
    /// Determines whether this event contains valid MIDI data.
    /// </summary>
    /// <returns><see langword="true"/> when all fields are within legal MIDI ranges.</returns>
    public bool IsValid() =>
        NoteNumber is >= 0 and <= 127 &&
        Velocity is >= 0 and <= 127 &&
        Channel is >= 1 and <= 16;
}

/// <summary>
/// Describes a MIDI input device available on the current system.
/// </summary>
public sealed class MidiDeviceInfo
{
    /// <summary>Gets the zero-based device index used when opening the device.</summary>
    public int Index { get; init; }

    /// <summary>Gets the product name reported by the device driver.</summary>
    public string ProductName { get; init; } = string.Empty;

    /// <summary>Gets a value indicating whether the device is currently available for use.</summary>
    public bool IsAvailable { get; init; } = true;

    /// <summary>
    /// Determines whether this device info contains a usable product name and a valid index.
    /// </summary>
    /// <returns><see langword="true"/> when the device can be opened.</returns>
    public bool IsValid() => !string.IsNullOrWhiteSpace(ProductName) && Index >= 0;
}
