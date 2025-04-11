# EventPublisherJsonExtensions

Provides JSON serialization and deserialization helpers for the `EventPublisher` type, along with read‑only counters that track how many subscribers are registered per event name and the overall subscriber total.

## API

### `public static string ToJson(EventPublisher publisher)`
Serializes the supplied `EventPublisher` instance to a JSON string.

- **Parameters**
  - `publisher`: The `EventPublisher` to serialize. Must not be `null‑ Return value**: A JSON‑encoded string representing the publisher’s current state.
- **Exceptions**
  - `ArgumentNullException` if `publisher` is `null`.
  - `JsonSerializationException` (or a derived type) if the serialization process fails.

### `public static object? FromJson(string json)`
Deserializes a JSON string into an `EventPublisher` instance.

- **Parameters**
  - `publisherJson`: The JSON payload produced by `ToJson`. May be `null` or empty.
- **‑ Return value**: The deserialized `EventPublisher` object, or `null` when the input is `null`/`empty` or does not represent a valid publisher.
- **Exceptions**
  - `JsonSerializationException` if the JSON is malformed or cannot be mapped to the `EventPublisher` type.

### `public static bool TryFromJson(string json, out object? result)`
Attempts to deserialize a JSON string into an `EventPublisher` instance without throwing exceptions.

- **Parameters**
  - `json`: The JSON payload to parse.
  - `result`: When the method returns `true`, contains the deserialized `EventPublisher`; otherwise `null`.
- **‑ Return value**: `true` if `json` was successfully parsed; `false` otherwise.
- **Exceptions**: None; all error conditions are reported via the return value.

### `public Dictionary<string, int> SubscriberCounts`
Gets a read‑only dictionary mapping event names to the number of subscribers currently registered for each event.

- **‑ Return value**: A dictionary where the key is the event name (`string`) and the value is the subscriber count (`int`). The dictionary reflects the state at the moment of access; subsequent modifications to the publisher are not automatically reflected in existing dictionary instances.

### `public int TotalSubscribers`
Gets the total number of subscribers across all events managed by the publisher.

- **‑ Return value**: An integer representing the summed subscriber count.

## Usage

```csharp
using NaudioVisualizer.Json; // namespace containing the extensions

var publisher = new EventPublisher();
// ... configure publisher, add subscribers, etc.

// Serialize to JSON for storage or transmission
string json = EventPublisherJsonExtensions.ToJson(publisher);
File.WriteAllText("publisherState.json", json);

// Later, restore the publisher from JSON
string storedJson = File.ReadAllText("publisherState.json");
if (EventPublisherJsonExtensions.TryFromJson(storedJson, out object? obj) &&
    obj is EventPublisher restored)
{
    publisher = restored; // use the restored instance
}
else
{
    // Handle deserialization failure
    Console.WriteLine("Failed to restore publisher from JSON.");
}
```

```csharp
// Inspect subscriber statistics without modifying the publisher
var counts = EventPublisherJsonExtensions.SubscriberCounts;
foreach (var kvp in counts)
{
    Console.WriteLine($"Event '{kvp.Key}' has {kvp.Value} subscriber(s).");
}
Console.WriteLine($"Total subscribers: {EventPublisherJsonExtensions.TotalSubscribers}");
```

## Notes

- The JSON methods operate on immutable snapshots; they do not alter the source `EventPublisher` instance.
- `SubscriberCounts` and `TotalSubscribers` reflect the publisher’s internal state at the time the property is accessed. Concurrent modifications to the publisher (e.g., adding or removing subscribers from multiple threads) may result in stale or inconsistent values if the properties are read without external synchronization.
- The static extension methods themselves are thread‑safe provided they do not rely on mutable shared state; they only read the supplied arguments and return new objects or values.
- If high‑frequency updates to subscriber lists are expected, consider copying the dictionary returned by `SubscriberCounts` before iterating to avoid enumeration exceptions caused by concurrent modifications.
