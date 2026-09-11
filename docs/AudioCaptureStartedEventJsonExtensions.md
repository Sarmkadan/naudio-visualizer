# AudioCaptureStartedEventJsonExtensions

Provides System.Text.Json serialization extensions for AudioCaptureStartedEvent.

## API

### `public static string ToJson(this AudioCaptureStartedEvent value, bool indented = false)`
Serializes the supplied `AudioCaptureStartedEvent` instance to a JSON string.

- **Parameters**
  - `value`: The `AudioCaptureStartedEvent` to serialize. Must not be `null`.
  - `indented`: Whether to format the JSON with indentation. Default is `false`.
- **Return value**: A JSON‑encoded string representing the event's current state.
- **Exceptions**
  - `ArgumentNullException` if `value` is `null`.

### `public static AudioCaptureStartedEvent? FromJson(string json)`
Deserializes a JSON string to an `AudioCaptureStartedEvent` instance.

- **Parameters**
  - `json`: The JSON string to deserialize. Must not be `null`.
- **Return value**: The deserialized event, or `null` if the JSON is invalid or empty.
- **Exceptions**
  - `ArgumentNullException` if `json` is `null`.

### `public static bool TryFromJson(string json, out AudioCaptureStartedEvent? value)`
Attempts to deserialize a JSON string to an `AudioCaptureStartedEvent` instance.

- **Parameters**
  - `json`: The JSON string to deserialize. Must not be `null`.
  - `value`: Receives the deserialized event if successful.
- **Return value**: `true` if deserialization succeeded; otherwise, `false`.
- **Exceptions**
  - `ArgumentNullException` if `json` is `null`.

## Usage

```csharp
using NAudioVisualizer.Events; // namespace containing the extensions

var @event = new AudioCaptureStartedEvent();
// ... configure event properties, set timestamp, etc.

// Serialize to JSON for storage or transmission
string json = @event.ToJson(); // or @event.ToJson(true) for indented JSON
File.WriteAllText("audioCaptureStartedEvent.json", json);

// Later, restore the event from JSON
string storedJson = File.ReadAllText("audioCaptureStartedEvent.json");
AudioCaptureStartedEvent? restoredEvent = AudioCaptureStartedEventJsonExtensions.FromJson(storedJson);
// Alternatively, use TryFromJson
if (AudioCaptureStartedEventJsonExtensions.TryFromJson(storedJson, out var parsedEvent))
{
    // parsedEvent contains the deserialized event
}
```

```csharp
// Inspect the event's JSON representation
string json = @event.ToJson();
Console.WriteLine(json);
```

## Notes

- The JSON method operates on an immutable snapshot; it does not alter the source `AudioCaptureStartedEvent` instance.
- The static extension methods are thread‑safe provided they do not rely on mutable shared state; they only read the supplied arguments and return a new string or boolean.
- The `ToJson` method uses a `JsonSerializerOptions` instance with camelCase naming policy, which is efficient for repeated calls.
- The serialized object includes the event's properties such as `Timestamp` and any other fields defined in the `AudioCaptureStartedEvent` class.