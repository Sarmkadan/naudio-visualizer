# AudioMetadataJsonExtensions

Provides System.Text.Json serialization extensions for AudioMetadata.

## API

### `public static string ToJson(this AudioMetadata metadata, bool indented = false)`
Serializes the supplied `AudioMetadata` instance to a JSON string.

- **Parameters**
  - `metadata`: The `AudioMetadata` to serialize. Must not be `null`.
  - `indented`: Whether to format the JSON with indentation. Default is `false`.
- **Return value**: A JSON‑encoded string representing the audio metadata's current state.
- **Exceptions**
  - `ArgumentNullException` if `metadata` is `null`.

## Usage

```csharp
using NAudioVisualizer.Domain.Models; // namespace containing the extensions

var metadata = new AudioMetadata();
// ... configure metadata, set sample rate, channel count, etc.

// Serialize to JSON for storage or transmission
string json = metadata.ToJson(); // or metadata.ToJson(true) for indented JSON
File.WriteAllText("audioMetadataState.json", json);

// Later, restore the metadata from JSON
string storedJson = File.ReadAllText("audioMetadataState.json");
// Note: There is no FromJson method for AudioMetadata as it's designed for serialization only
// If deserialization is needed, use System.Text.Json.JsonSerializer directly with the same options
```

```csharp
// Inspect the metadata's JSON representation
string json = metadata.ToJson();
Console.WriteLine(json);
```

## Notes

- The JSON method operates on an immutable snapshot; it does not alter the source `AudioMetadata` instance.
- The static extension method itself is thread‑safe provided it does not rely on mutable shared state; it only reads the supplied arguments and returns a new string.
- The `ToJson` method uses a `JsonSerializerOptions` instance with camelCase naming policy, which is efficient for repeated calls.
- The serialized object includes the audio metadata's properties such as `SessionId`, `StartTime`, `LastUpdateTime`, `SampleRate`, `ChannelCount`, `BitDepth`, `TotalSamplesCaptured`, `TotalFramesProcessed`, `CurrentLevel`, `PeakLevel`, `AverageLevel`, `CurrentDurationSeconds`, `AudioDevice`, `IsCapturing`, `CpuUsagePercent`, `BufferUnderruns`, and `DominantFrequency` (as defined in the `AudioMetadata` class).