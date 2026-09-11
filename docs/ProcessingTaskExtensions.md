# ProcessingTaskExtensions

Provides System.Text.Json serialization extensions for <see cref="ProcessingTask"/>.

## API

### `public static string ToJson(this ProcessingTask task)`
Serializes the public scalar properties of a <see cref="ProcessingTask"/> to JSON.

- **Parameters**
  - `task`: The processing task to serialize. Must not be `null`.
- **Return value**: A JSON string containing the task name and creation timestamp.
- **Exceptions**
  - `ArgumentNullException` if <paramref name="task"/> is `null`.

## Usage

```csharp
using NAudioVisualizer.Workers; // namespace containing the extensions

// Example: serializing a processing task
var task = new ProcessingTask
{
    Name = "AudioAnalysis",
    CreatedAt = DateTime.UtcNow
};

// Serialize to JSON
string json = task.ToJson();
File.WriteAllText("task.json", json);
```

## Notes

- The JSON method operates on an immutable snapshot; it does not alter the source object.
- The static extension method is thread-safe provided it does not rely on mutable shared state; it only reads the supplied arguments and returns a new string.
- The `ToJson` method uses the default `JsonSerializerOptions` with camelCase naming policy.