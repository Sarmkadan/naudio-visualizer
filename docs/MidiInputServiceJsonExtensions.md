# MidiInputServiceJsonExtensions

Provides System.Text.Json serialization extensions for MidiInputService.

## API

### `public static string ToJson(this MidiInputService value, bool indented = false)`
Serializes the supplied `MidiInputService` instance to a JSON string.

- **Parameters**
  - `value`: The `MidiInputService` to serialize. Must not be `null`.
  - `indented`: Whether to format the JSON with indentation. Default is `false`.
- **Return value**: A JSON‑encoded string representing the service state (specifically `IsDisposed` and `ActiveDeviceIndex`).
- **Exceptions**
  - `ArgumentNullException` if `value` is `null`.

### `public static MidiInputService? FromJson(string json)`
Deserializes a JSON string to a `MidiInputService` instance.

- **Remarks**
  - Note: `MidiInputService` contains disposable resources and event handlers that cannot be deserialized.
  - This method creates a new instance regardless of the JSON content.
- **Parameters**
  - `json`: The JSON string to deserialize.
- **Return value**: The deserialized service instance, or `null` if deserialization fails.
- **Exceptions**
  - `ArgumentNullException` if `json` is `null`.
  - `ArgumentException` if `json` is empty or whitespace.

### `public static bool TryFromJson(string json, out MidiInputService? value)`
Attempts to deserialize a JSON string to a `MidiInputService` instance.

- **Remarks**
  - Note: `MidiInputService` contains disposable resources and event handlers that cannot be serialized.
  - This method creates a new instance regardless of the JSON content.
- **Parameters**
  - `json`: The JSON string to deserialize.
  - `value`: The deserialized service instance, or `null` if deserialization fails.
- **Return value**: `True` if deserialization succeeds; otherwise, `false`.
- **Exceptions**
  - `ArgumentNullException` if `json` is `null`.

## Usage

```csharp
using NAudioVisualizer.Services; // namespace containing the extensions

var service = new MidiInputService();
// ... configure service, set active device, etc.

// Serialize to JSON for storage or transmission
string json = service.ToJson(); // or service.ToJson(true) for indented JSON
File.WriteAllText("midiInputServiceState.json", json);

// Later, restore the service from JSON
string storedJson = File.ReadAllText("midiInputServiceState.json");
MidiInputService? restoredService = MidiInputServiceJsonExtensions.FromJson(storedJson);
// Note: The restored service is a new instance; event handlers and disposable resources must be reinitialized.
// If deserialization is needed, consider using TryFromJson to handle potential errors gracefully.

// Inspect the service's JSON representation
string json = service.ToJson();
Console.WriteLine(json);
```

```csharp
// Using TryFromJson for safe deserialization
string json = File.ReadAllText("midiInputServiceState.json");
if (MidiInputServiceJsonExtensions.TryFromJson(json, out var service))
{
    // Use service
}
else
{
    // Handle deserialization failure
}
```

## Notes

- The JSON method operates on an immutable snapshot; it does not alter the source `MidiInputService` instance.
- The static extension method itself is thread‑safe provided it does not rely on mutable shared state; it only reads the supplied arguments and returns a new string.
- The `ToJson` method uses a `JsonSerializerOptions` instance with camelCase naming policy, which is efficient for repeated calls.
- The serialized object includes only the `IsDisposed` and `ActiveDeviceIndex` properties of the `MidiInputService` state, as other properties (such as disposable resources and event handlers) cannot be meaningfully serialized.