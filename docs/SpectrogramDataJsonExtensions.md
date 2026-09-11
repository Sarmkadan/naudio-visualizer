# SpectrogramDataJsonExtensions

Provides System.Text.Json serialization extensions for SpectrogramData.

## API

### `public static string ToJson(this SpectrogramData spectrogram, bool includeMatrix = false)`
Serializes the supplied `SpectrogramData` instance to a JSON string.

- **Parameters**
  - `spectrogram`: The `SpectrogramData` to serialize. Must not be `null`.
  - `includeMatrix`: Whether to include the flattened data returned by <see cref="VisualizationData.GetData"/>. Default is `false`.
- **Return value**: A JSON‑encoded string representing the spectrogram’s current state.
- **Exceptions**
  - `ArgumentNullException` if `spectrogram` is `null`.

## Usage

```csharp
using NAudioVisualizer.Domain.Models; // namespace containing the extensions

var spectrogram = new SpectrogramData();
// ... configure spectrogram, set data, etc.

// Serialize to JSON for storage or transmission
string json = spectrogram.ToJson(); // or spectrogram.ToJson(true) to include matrix
File.WriteAllText("spectrogramState.json", json);

// Later, restore the spectrogram from JSON
string storedJson = File.ReadAllText("spectrogramState.json");
// Note: There is no FromJson method for SpectrogramData as it's designed for serialization only
// If deserialization is needed, use System.Text.Json.JsonSerializer directly with the same options
```

```csharp
// Inspect the spectrogram's JSON representation
string json = spectrogram.ToJson();
Console.WriteLine(json);
```

## Notes

- The JSON method operates on an immutable snapshot; it does not alter the source `SpectrogramData` instance.
- The static extension method itself is thread‑safe provided it does not rely on mutable shared state; it only reads the supplied arguments and returns a new string.
- The `ToJson` method uses a `JsonSerializerOptions` instance with camelCase naming policy, which is efficient for repeated calls.
- If including the matrix is required, pass `true` to the `includeMatrix` parameter; otherwise, the matrix is omitted (null) and not included in the JSON due to `DefaultIgnoreCondition.WhenWritingNull`.