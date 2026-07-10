# AudioCaptureStartedEventExtensions

Provides utility extension methods for inspecting and comparing `AudioCaptureStartedEvent` instances. These helpers allow consumers to extract human-readable session descriptions, estimate the memory footprint of an event, verify whether an event matches a given configuration, and obtain the label of the associated capture device.

## API

### GetSessionDescription

```csharp
public static string GetSessionDescription(this AudioCaptureStartedEvent event)
```

Returns a formatted string that describes the capture session represented by the event. The description typically includes the device label, sample rate, channel count, and bit depth, making it suitable for logging or display purposes.

- **Parameters**:  
  `event` — the `AudioCaptureStartedEvent` to describe. Must not be null.
- **Return value**: a non-null, non-empty string summarizing the session.
- **Exceptions**: throws `ArgumentNullException` when `event` is null.

### EstimateMemoryUsage

```csharp
public static long EstimateMemoryUsage(this AudioCaptureStartedEvent event)
```

Calculates an approximate memory footprint in bytes for the event object and its internal data structures. This is intended for diagnostics and resource monitoring, not as an exact allocation measurement.

- **Parameters**:  
  `event` — the `AudioCaptureStartedEvent` to measure. Must not be null.
- **Return value**: a non-negative `long` representing the estimated number of bytes.
- **Exceptions**: throws `ArgumentNullException` when `event` is null.

### MatchesConfiguration

```csharp
public static bool MatchesConfiguration(this AudioCaptureStartedEvent event, CaptureConfiguration configuration)
```

Determines whether the given event was produced by a capture session whose settings match the supplied `CaptureConfiguration`. The comparison typically covers sample rate, channel layout, and format parameters.

- **Parameters**:  
  `event` — the `AudioCaptureStartedEvent` to inspect. Must not be null.  
  `configuration` — the `CaptureConfiguration` to compare against. Must not be null.
- **Return value**: `true` if the event’s embedded configuration matches the provided configuration; otherwise `false`.
- **Exceptions**: throws `ArgumentNullException` if either argument is null.

### GetDeviceLabel

```csharp
public static string GetDeviceLabel(this AudioCaptureStartedEvent event)
```

Extracts the human-readable label of the audio capture device that originated the event. This is the same label that would appear in system audio device lists.

- **Parameters**:  
  `event` — the `AudioCaptureStartedEvent` to query. Must not be null.
- **Return value**: a non-null string containing the device label. May be empty if the event carries no label.
- **Exceptions**: throws `ArgumentNullException` when `event` is null.

## Usage

### Example 1: Logging capture details and memory impact

```csharp
void LogCaptureStart(AudioCaptureStartedEvent evt)
{
    string sessionDesc = evt.GetSessionDescription();
    long memEstimate = evt.EstimateMemoryUsage();
    string device = evt.GetDeviceLabel();

    Console.WriteLine($"Capture started on [{device}]: {sessionDesc}");
    Console.WriteLine($"Estimated memory footprint: {memEstimate} bytes");
}
```

### Example 2: Validating an event against a desired configuration

```csharp
bool IsExpectedConfiguration(AudioCaptureStartedEvent evt, CaptureConfiguration desiredConfig)
{
    if (!evt.MatchesConfiguration(desiredConfig))
    {
        Debug.WriteLine(
            $"Unexpected capture config. Device: {evt.GetDeviceLabel()}, " +
            $"Session: {evt.GetSessionDescription()}");
        return false;
    }

    return true;
}
```

## Notes

- All methods perform null-guard checks on their primary `event` argument and will throw `ArgumentNullException` immediately if it is null. `MatchesConfiguration` additionally guards against a null `configuration`.
- `EstimateMemoryUsage` returns an approximation; the actual managed and unmanaged memory consumed may vary depending on runtime internals, string interning, and platform-specific representations. Do not rely on this value for precise accounting.
- `MatchesConfiguration` performs a field-by-field comparison of the relevant audio parameters. Two configurations that are semantically equivalent but represented by different object instances will still yield `true` as long as their parameter values are equal.
- `GetDeviceLabel` may return an empty string when the underlying event was created without a device label (for example, in synthetic or test scenarios). Callers should handle empty labels gracefully.
- These methods are static extension methods and are not designed to be called on a null reference. There is no internal synchronization; they operate on immutable snapshots of event data and are safe to call from multiple threads concurrently without external locking.
