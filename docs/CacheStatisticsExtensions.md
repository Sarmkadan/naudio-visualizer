# CacheStatisticsExtensions

Provides System.Text.Json serialization extensions for cache statistics.

## API

### `public static string ToJson(this CacheStatistics stats, bool indented = false)`
Serializes cache statistics to JSON.

- **Parameters**
  - `stats`: The cache statistics to serialize. Must not be `null`.
  - `indented`: Whether to format the JSON with indentation for readability. Default is `false`.
- **Return value**: A JSON string representation of the cache statistics.
- **Exceptions**
  - `ArgumentNullException` if `stats` is `null`.

## Usage

```csharp
using NAudioVisualizer.Caching; // namespace containing the extensions

// Example: serializing cache statistics
var stats = new CacheStatistics
{
    Hits = 100,
    Misses = 20,
    Evictions = 5
};

// Serialize to JSON
string json = stats.ToJson(); // or stats.ToJson(true) for indented JSON
File.WriteAllText("cacheStats.json", json);

// Deserialize from JSON (using System.Text.Json directly)
string storedJson = File.ReadAllText("cacheStats.json");
CacheStatistics? restoredStats = JsonSerializer.Deserialize<CacheStatistics>(storedJson);
```

## Notes

- The JSON method operates on an immutable snapshot; it does not alter the source object.
- The static extension method is thread-safe provided it does not rely on mutable shared state; it only reads the supplied arguments and returns a new string.
- The `ToJson` method uses a `JsonSerializerOptions` instance with the default options (camelCase naming policy is not used here because CacheStatistics is a simple DTO with public fields/properties that match JSON property names).
- For deserialization, use `System.Text.Json.JsonSerializer.Deserialize<CacheStatistics>(json)` as shown in the usage example.