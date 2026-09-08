#nullable enable

using System;
using System.Text.Json;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides JSON serialization extensions for <see cref="MidiNoteEvent"/>.
/// </summary>
public static class MidiNoteEventJsonExtensions
{
    /// <summary>
    /// Serializes a MIDI note event to JSON.
    /// </summary>
    /// <param name="midiEvent">The MIDI note event to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation.</param>
    /// <returns>A JSON representation of the MIDI note event.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="midiEvent"/> is <see langword="null"/>.
    /// </exception>
    public static string ToJson(this MidiNoteEvent midiEvent, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(midiEvent);

        return JsonSerializer.Serialize(midiEvent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = indented
        });
    }
}
