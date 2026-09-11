# AudioDeviceJsonExtensions

Provides System.Text.Json serialization extensions for AudioDevice.

## API

### `public static string ToJson(this AudioDevice device, bool indented = false)`
Serializes the supplied `AudioDevice` instance to a JSON string.

- **Parameters**
  - `device`: The `AudioDevice` to serialize. Must not be `null`.
  - `indented`: Whether to format the JSON with indentation. Default is `false`.
- **Return value**: A JSON‑encoded string representing the audio device’s current state.
- **Exceptions**
  - `ArgumentNullException` if `device` is `null`.

## Usage

```csharp
using NAudioVisualizer.Domain.Models; // namespace containing the extensions

var device = new AudioDevice();
// ... configure device, set name, index, channels, etc.

// Serialize to JSON for storage or transmission
string json = device.ToJson(); // or device.ToJson(true) for indented JSON
File.WriteAllText("deviceState.json", json);

// Later, restore the device from JSON
string storedJson = File.ReadAllText("deviceState.json");
// Note: There is no FromJson method for AudioDevice as it's designed for serialization only
// If deserialization is needed, use System.Text.Json.JsonSerializer directly with the same options
```

```csharp
// Inspect the device's JSON representation
string json = device.ToJson();
Console.WriteLine(json);
```

## Notes

- The JSON method operates on an immutable snapshot; it does not alter the source `AudioDevice` instance.
- The static extension method itself is thread‑safe provided it does not rely on mutable shared state; it only reads the supplied arguments and returns a new string.
- The `ToJson` method uses a `JsonSerializerOptions` instance with camelCase naming policy, which is efficient for repeated calls.
- The serialized object includes the audio device's properties such as `Name`, `DeviceIndex`, `ChannelCount`, `DefaultSampleRate`, `SupportedSampleRates`, and `IsAvailable` (as defined in the `AudioDevice` class).