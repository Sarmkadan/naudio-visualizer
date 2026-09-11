# AudioBufferJsonExtensions

Provides System.Text.Json serialization extensions for AudioBuffer.

## API

### `public static string ToJson(this AudioBuffer value, bool indented = false)`
Serializes the supplied `AudioBuffer` instance to a JSON string.

- **Parameters**
  - `value`: The `AudioBuffer` to serialize. Must not be `null`.
  - `indented`: Whether to format the JSON with indentation. Default is `false`.
- **Return value**: A JSON‑encoded string representing the audio buffer’s current state.
- **Exceptions**
  - `ArgumentNullException` if `value` is `null`.

### `public static AudioBuffer? FromJson(string json)`
Deserializes a JSON string into an `AudioBuffer` instance.

- **Parameters**
  - `json`: The JSON payload produced by `ToJson`. May be `null` or empty.
- **Return value**: The deserialized `AudioBuffer` object, or `null` when the input is `null`/`empty` or does not represent a valid audio buffer.
- **Exceptions**
  - `ArgumentNullException` if `json` is `null`.

### `public static bool TryFromJson(string json, out AudioBuffer? value)`
Attempts to deserialize a JSON string into an `AudioBuffer` instance without throwing exceptions.

- **Parameters**
  - `json`: The JSON payload to parse.
  - `value`: When the method returns `true`, contains the deserialized `AudioBuffer`; otherwise `null`.
- **Return value**: `true` if `json` was successfully parsed; `false` otherwise.
- **Exceptions**: None; all error conditions are reported via the return value.

## Usage

```csharp
using NAudioVisualizer.Domain.Models; // namespace containing the extensions

var buffer = new AudioBuffer();
// ... configure buffer, add samples, etc.

// Serialize to JSON for storage or transmission
string json = buffer.ToJson(); // or buffer.ToJson(true) for indented JSON
File.WriteAllText("bufferState.json", json);

// Later, restore the buffer from JSON
string storedJson = File.ReadAllText("bufferState.json");
if (AudioBufferJsonExtensions.TryFromJson(storedJson, out AudioBuffer? restored) &&
    restored != null)
{
    buffer = restored; // use the restored instance
}
else
{
    // Handle deserialization failure
    Console.WriteLine("Failed to restore buffer from JSON.");
}
```

```csharp
// Inspect the buffer's JSON representation
string json = AudioBufferJsonExtensions.ToJson(buffer);
Console.WriteLine(json);
```

## Notes

- The JSON methods operate on immutable snapshots; they do not alter the source `AudioBuffer` instance.
- The static extension methods themselves are thread‑safe provided they do not rely on mutable shared state; they only read the supplied arguments and return new objects or values.
- The `ToJson` method uses a cached `JsonSerializerOptions` instance with camelCase naming policy, which is efficient for repeated calls.
- If indented JSON is required for readability, pass `true` to the `indented` parameter; otherwise, the output is compact.