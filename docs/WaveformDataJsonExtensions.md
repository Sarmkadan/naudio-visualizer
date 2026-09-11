# WaveformDataJsonExtensions

Provides System.Text.Json serialization extensions for WaveformData.

## API

### `public static string ToJson(this WaveformData waveform, bool indented = false)`
Serializes the supplied `WaveformData` instance to a JSON string.

- **Parameters**
  - `waveform`: The `WaveformData` to serialize. Must not be `null`.
  - `indented`: Whether to format the JSON with indentation. Default is `false`.
- **Return value**: A JSON‑encoded string representing the waveform data’s current state.
- **Exceptions**
  - `ArgumentNullException` if `waveform` is `null`.

## Usage

```csharp
using NAudioVisualizer.Domain.Models; // namespace containing the extensions

var waveform = new WaveformData();
// ... configure waveform, set data, etc.

// Serialize to JSON for storage or transmission
string json = waveform.ToJson(); // or waveform.ToJson(true) for indented JSON
File.WriteAllText("waveformState.json", json);

// Later, restore the waveform from JSON
string storedJson = File.ReadAllText("waveformState.json");
// Note: There is no FromJson method for WaveformData as it's designed for serialization only
// If deserialization is needed, use System.Text.Json.JsonSerializer directly with the same options
```

```csharp
// Inspect the waveform's JSON representation
string json = waveform.ToJson();
Console.WriteLine(json);
```

## Notes

- The JSON method operates on an immutable snapshot; it does not alter the source `WaveformData` instance.
- The static extension method itself is thread‑safe provided it does not rely on mutable shared state; it only reads the supplied arguments and returns a new string.
- The `ToJson` method uses a `JsonSerializerOptions` instance with camelCase naming policy, which is efficient for repeated calls.
- The serialized object includes the waveform's `SampleRate`, `ChannelCount`, `DownsamplingFactor`, `DataPointCount`, `IsNormalized`, and the data points obtained via `GetData()`.