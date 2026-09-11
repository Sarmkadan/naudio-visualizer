# VisualizationSettingsJsonExtensions

Provides System.Text.Json serialization extensions for VisualizationSettings.

## API

### `public static string ToJson(this VisualizationSettings settings, bool indented = true)`
Serializes the supplied `VisualizationSettings` instance to a JSON string.

- **Parameters**
  - `settings`: The `VisualizationSettings` to serialize. Must not be `null`.
  - `indented`: Whether to format the JSON with indentation. Default is `true`.
- **Return value**: A JSON‑encoded string representing the visualization settings' current state.
- **Exceptions**
  - `ArgumentNullException` if `settings` is `null`.

## Usage

```csharp
using NAudioVisualizer.Domain.Models; // namespace containing the extensions

var settings = new VisualizationSettings();
// ... configure settings, e.g., set gradient stops, waveform data, etc.

// Serialize to JSON for storage or transmission
string json = settings.ToJson(); // or settings.ToJson(false) for compact JSON
File.WriteAllText("visualizationSettings.json", json);

// Later, restore the settings from JSON
string storedJson = File.ReadAllText("visualizationSettings.json");
// Note: There is no FromJson method for VisualizationSettings as it's designed for serialization only
// If deserialization is needed, use System.Text.Json.JsonSerializer directly with the same options
```

```csharp
// Inspect the settings' JSON representation
string json = settings.ToJson();
Console.WriteLine(json);
```

## Notes

- The JSON method operates on an immutable snapshot; it does not alter the source `VisualizationSettings` instance.
- The static extension method itself is thread‑safe provided it does not rely on mutable shared state; it only reads the supplied arguments and returns a new string.
- The `ToJson` method uses a `JsonSerializerOptions` instance with camelCase naming policy, which is efficient for repeated calls.
- The serialized object includes the visualization settings' properties such as `GradientStops`, `WaveformData`, `SpectrumData`, and other configuration values (as defined in the `VisualizationSettings` class).