# AudioFrameJsonExtensions

Provides System.Text.Json serialization extensions for AudioFrame.

## API

### `public static string ToJson(this AudioFrame frame, bool includeSamples = false)`
Serializes the supplied `AudioFrame` instance to a JSON string.

- **Parameters**
  - `frame`: The `AudioFrame` to serialize. Must not be `null`.
  - `includeSamples`: Whether to include raw audio samples in the JSON. Default is `false`.
- **Return value**: A JSON‑encoded string representing the audio frame’s current state.
- **Exceptions**
  - `ArgumentNullException` if `frame` is `null`.

## Usage

```csharp
using NAudioVisualizer.Domain.Models; // namespace containing the extensions

var frame = new AudioFrame();
// ... configure frame, set samples, etc.

// Serialize to JSON for storage or transmission
string json = frame.ToJson(); // or frame.ToJson(true) to include samples
File.WriteAllText("frameState.json", json);

// Later, restore the frame from JSON
string storedJson = File.ReadAllText("frameState.json");
// Note: There is no FromJson method for AudioFrame as it's designed for serialization only
// If deserialization is needed, use System.Text.Json.JsonSerializer directly with the same options
```

```csharp
// Inspect the frame's JSON representation
string json = frame.ToJson();
Console.WriteLine(json);
```

## Notes

- The JSON method operates on an immutable snapshot; it does not alter the source `AudioFrame` instance.
- The static extension method itself is thread‑safe provided it does not rely on mutable shared state; it only reads the supplied arguments and returns a new string.
- The `ToJson` method uses a `JsonSerializerOptions` instance with camelCase naming policy, which is efficient for repeated calls.
- If including samples is required, pass `true` to the `includeSamples` parameter; otherwise, the samples are omitted (null) and not included in the JSON due to `DefaultIgnoreCondition.WhenWritingNull`.