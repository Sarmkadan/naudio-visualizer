# AudioDataConverterJsonExtensions

Provides System.Text.Json serialization extensions for audio data types.

## API

### `public static string ToJson<T>(this T value, bool indented = false) where T : notnull`
Serializes the supplied object to a JSON string using camelCase property naming.

- **Parameters**
  - `value`: The object instance to serialize. Must not be `null`.
  - `indented`: Whether to format the JSON with indentation for readability. Default is `false`.
- **Return value**: A JSON string representation of the object.
- **Exceptions**
  - `ArgumentNullException` if `value` is `null`.

### `public static T? FromJson<T>(string? json) where T : class`
Deserializes a JSON string to an object of the specified type.

- **Parameters**
  - `json`: The JSON string to deserialize.
- **Return value**: An instance of type T, or null if the JSON is null or empty.

### `public static bool TryFromJson<T>(string? json, out T? value) where T : class`
Attempts to deserialize a JSON string to an object of the specified type.

- **Parameters**
  - `json`: The JSON string to deserialize.
  - `value`: Receives the deserialized object instance, or null if deserialization fails.
- **Return value**: True if deserialization succeeded; otherwise, false.

## Usage

```csharp
using NAudioVisualizer.Infrastructure; // namespace containing the extensions

// Example: serializing and deserializing a simple class
public class AudioBuffer
{
    public float[] Samples { get; set; } = Array.Empty<float>();
    public int SampleRate { get; set; }
    public int Channels { get; set; }
}

var buffer = new AudioBuffer
{
    SampleRate = 48000,
    Channels = 2,
    Samples = new float[96000] // 2 seconds of stereo audio
};

// Serialize to JSON
string json = buffer.ToJson(); // or buffer.ToJson(true) for indented JSON
File.WriteAllText("audioBuffer.json", json);

// Deserialize from JSON
string storedJson = File.ReadAllText("audioBuffer.json");
AudioBuffer? restoredBuffer = storedJson.FromJson<AudioBuffer>();

// Using TryFromJson to avoid exceptions
if (storedJson.TryFromJson<AudioBuffer>(out var bufferFromTry))
{
    // Use bufferFromTry
}
```

## Notes

- The JSON methods operate on an immutable snapshot; they do not alter the source object.
- The static extension methods are thread-safe provided they do not rely on mutable shared state; they only read the supplied arguments and return a new string or boolean.
- The `ToJson` method uses a `JsonSerializerOptions` instance with camelCase naming policy, which is efficient for repeated calls.
- The `FromJson` and `TryFromJson` methods return null for classes when the JSON is null, empty, or invalid, and for `TryFromJson` the boolean indicates success.
- Note: The AudioDataConverter class itself is a static utility and cannot be meaningfully serialized; these extensions are intended for other audio-related types.