# MidiNoteEventJsonExtensions

Provides System.Text.Json serialization extensions for MidiNoteEvent.

## API

### `public static string ToJson(this MidiNoteEvent midiEvent, bool indented = false)`
Serializes the supplied `MidiNoteEvent` instance to a JSON string.

- **Parameters**
  - `midiEvent`: The `MidiNoteEvent` to serialize. Must not be `null`.
  - `indented`: Whether to format the JSON with indentation. Default is `false`.
- **Return value**: A JSON‑encoded string representing the MIDI note event’s current state.
- **Exceptions**
  - `ArgumentNullException` if `midiEvent` is `null`.

## Usage

```csharp
using NAudioVisualizer.Domain.Models; // namespace containing the extensions

var midiEvent = new MidiNoteEvent();
// ... configure midi event, set note, velocity, etc.

// Serialize to JSON for storage or transmission
string json = midiEvent.ToJson(); // or midiEvent.ToJson(true) for indented JSON
File.WriteAllText("midiEventState.json", json);

// Later, restore the MIDI event from JSON
string storedJson = File.ReadAllText("midiEventState.json");
// Note: There is no FromJson method for MidiNoteEvent as it's designed for serialization only
// If deserialization is needed, use System.Text.Json.JsonSerializer directly with the same options
```

```csharp
// Inspect the MIDI event's JSON representation
string json = midiEvent.ToJson();
Console.WriteLine(json);
```

## Notes

- The JSON method operates on an immutable snapshot; it does not alter the source `MidiNoteEvent` instance.
- The static extension method itself is thread‑safe provided it does not rely on mutable shared state; it only reads the supplied arguments and returns a new string.
- The `ToJson` method uses a `JsonSerializerOptions` instance with camelCase naming policy, which is efficient for repeated calls.
- The serialized object includes the MIDI note event's properties such as `NoteNumber`, `Velocity`, `Offset`, `Duration`, and `Channel` (as defined in the `MidiNoteEvent` class).