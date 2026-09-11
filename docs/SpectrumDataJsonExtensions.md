# SpectrumDataJsonExtensions

Provides System.Text.Json serialization extensions for SpectrumData.

## API

### `public static string ToJson(this SpectrumData spectrum, bool indented = false)`
Serializes the supplied `SpectrumData` instance to a JSON string.

- **Parameters**
  - `spectrum`: The `SpectrumData` to serialize. Must not be `null`.
  - `indented`: Whether to format the JSON with indentation. Default is `false`.
- **Return value**: A JSON‑encoded string representing the spectrum data’s current state.
- **Exceptions**
  - `ArgumentNullException` if `spectrum` is `null`.

## Usage

```csharp
using NAudioVisualizer.Domain.Models; // namespace containing the extensions

var spectrum = new SpectrumData();
// ... configure spectrum, add frequency bins, etc.

// Serialize to JSON for storage or transmission
string json = spectrum.ToJson(); // or spectrum.ToJson(true) for indented JSON
File.WriteAllText("spectrumState.json", json);

// Later, restore the spectrum from JSON
string storedJson = File.ReadAllText("spectrumState.json");
// Note: There is no FromJson method for SpectrumData as it's designed for serialization only
// If deserialization is needed, use System.Text.Json.JsonSerializer directly with the same options
```

```csharp
// Inspect the spectrum's JSON representation
string json = spectrum.ToJson();
Console.WriteLine(json);
```

## Notes

- The JSON method operates on an immutable snapshot; it does not alter the source `SpectrumData` instance.
- The static extension method itself is thread‑safe provided it does not rely on mutable shared state; it only reads the supplied arguments and returns a new string.
- The `ToJson` method uses a cached `JsonSerializerOptions` instance with camelCase naming policy, which is efficient for repeated calls.
- If indented JSON is required for readability, pass `true` to the `indented` parameter; otherwise, the output is compact.