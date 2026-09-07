# AudioMetadataExtensions

The `AudioMetadataExtensions` static class provides convenience methods for presenting and inspecting an `AudioMetadata` instance. It formats the current duration, exposes audio level values by name, identifies performance issues, and describes the assigned audio device.

## API

### Methods

*   **`public static string GetFormattedDuration(this AudioMetadata metadata)`**
    Formats `CurrentDurationSeconds` as `hh:mm:ss.fff` using the invariant culture. The value is first converted with `TimeSpan.FromSeconds`.
    *   **Parameters**:
        *   `metadata`: The `AudioMetadata` instance whose duration is formatted.
    *   **Returns**: The formatted duration string.
    *   **Throws**: `ArgumentNullException` if `metadata` is `null`.

*   **`public static IReadOnlyDictionary<string, float> GetLevelMetrics(this AudioMetadata metadata)`**
    Creates a dictionary containing the current audio level values.
    *   **Parameters**:
        *   `metadata`: The `AudioMetadata` instance whose levels are retrieved.
    *   **Returns**: A new read-only dictionary view with exactly three entries: `"Current"` mapped to `CurrentLevel`, `"Peak"` mapped to `PeakLevel`, and `"Average"` mapped to `AverageLevel`.
    *   **Throws**: `ArgumentNullException` if `metadata` is `null`.

*   **`public static bool HasPerformanceIssues(this AudioMetadata metadata)`**
    Checks the CPU usage and buffer underrun count reported by the metadata.
    *   **Parameters**:
        *   `metadata`: The `AudioMetadata` instance to inspect.
    *   **Returns**: `true` when `CpuUsagePercent` is greater than `80f` or `BufferUnderruns` is greater than zero; otherwise, `false`. A CPU usage value of exactly `80f` does not by itself indicate an issue.
    *   **Throws**: `ArgumentNullException` if `metadata` is `null`.

*   **`public static string GetDeviceDescription(this AudioMetadata metadata)`**
    Produces a description of the associated audio device.
    *   **Parameters**:
        *   `metadata`: The `AudioMetadata` instance whose device is described.
    *   **Returns**: The result of `AudioDevice.ToString()` when `AudioDevice` is not `null`; otherwise, `"No audio device assigned"`.
    *   **Throws**: `ArgumentNullException` if `metadata` is `null`.

## Usage

```csharp
using NAudioVisualizer.Domain.Models;

var metadata = new AudioMetadata
{
    SampleRate = 48_000,
    TotalSamplesCaptured = 180_000,
    CurrentLevel = 0.35f,
    PeakLevel = 0.82f,
    AverageLevel = 0.24f,
    CpuUsagePercent = 81f
};

metadata.UpdateDuration();

Console.WriteLine(metadata.GetFormattedDuration()); // 00:00:03.750

IReadOnlyDictionary<string, float> levels = metadata.GetLevelMetrics();
Console.WriteLine($"Current: {levels["Current"]}, Peak: {levels["Peak"]}, Average: {levels["Average"]}");

if (metadata.HasPerformanceIssues())
{
    Console.WriteLine("The capture session has a performance issue.");
}

Console.WriteLine(metadata.GetDeviceDescription()); // No audio device assigned
```

## Notes

*   Each method validates `metadata` before reading it and throws `ArgumentNullException` for a null receiver.
*   `GetLevelMetrics` copies the three level values into a new `Dictionary<string, float>` and returns it through the `IReadOnlyDictionary<string, float>` interface.
*   `HasPerformanceIssues` does not combine or score its inputs; either CPU usage above 80 percent or at least one buffer underrun is sufficient.
