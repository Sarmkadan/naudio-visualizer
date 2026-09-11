# VisualizationDataJsonExtensions

Provides System.Text.Json serialization and deserialization extensions for VisualizationData.

## API

### `public static string ToJson(this VisualizationData value, bool indented = false)`
Serializes the VisualizationData to a JSON string.

- **Parameters**
  - `value`: The visualization data to serialize. Must not be `null`.
  - `indented`: Whether to format the JSON with indentation for readability. Default is `false`.
- **Return value**: A JSON string representation of the visualization data.
- **Exceptions**
  - `ArgumentNullException` if `value` is `null`.

### `public static VisualizationData? FromJson(string json)`
Deserializes a JSON string to VisualizationData.

- **Parameters**
  - `json`: The JSON string to deserialize.
- **Return value**: The deserialized VisualizationData, or null if the JSON is empty or whitespace.
- **Exceptions**
  - `ArgumentNullException` if `json` is `null`.
  - `JsonException` if the JSON is invalid or cannot be deserialized.

### `public static bool TryFromJson(string json, out VisualizationData? value)`
Attempts to deserialize a JSON string to VisualizationData.

- **Parameters**
  - `json`: The JSON string to deserialize.
  - `value`: Receives the deserialized VisualizationData if successful.
- **Return value**: True if deserialization succeeded; otherwise, false.

## Usage

```csharp
using NAudioVisualizer.Domain.Models; // namespace containing the extensions

var data = new VisualizationData();
// ... configure data

// Serialize to JSON for storage or transmission
string json = data.ToJson(); // or data.ToJson(true) for indented JSON
File.WriteAllText("visualizationData.json", json);

// Later, restore the data from JSON
string storedJson = File.ReadAllText("visualizationData.json");
VisualizationData? restoredData = VisualizationDataJsonExtensions.FromJson(storedJson);
// Or using the TryFromJson method:
// if (VisualizationDataJsonExtensions.TryFromJson(storedJson, out var restoredData))
// {
//     // use restoredData
// }
```

```csharp
// Inspect the data's JSON representation
string json = data.ToJson();
Console.WriteLine(json);
```

## Notes

- The JSON methods operate on an immutable snapshot; they do not alter the source `VisualizationData` instance.
- The static extension methods are thread-safe provided they do not rely on mutable shared state; they only read the supplied arguments and return a new string (or boolean and output parameter).
- The `ToJson` method uses a `JsonSerializerOptions` instance with camelCase naming policy, which is efficient for repeated calls.
- When `indented` is set to `true`, the JSON is formatted with indentation for readability.