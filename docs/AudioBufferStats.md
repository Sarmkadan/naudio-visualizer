# AudioBufferStats

The `AudioBufferStats` class contains a snapshot of an `AudioBuffer` instance's usage statistics. Call `AudioBuffer.GetStats()` to create a new statistics object populated from the buffer's state at that moment.

## API

### Properties

*   **`public long SamplesWritten { get; set; }`**
    Gets or sets the cumulative number of individual samples written to the buffer since it was created. The value continues increasing when a full buffer overwrites older samples and is not reset by `AudioBuffer.Clear()`. Its unit is samples.

*   **`public int CurrentCount { get; set; }`**
    Gets or sets the number of individual samples currently stored in the buffer. Its unit is samples.

*   **`public int Capacity { get; set; }`**
    Gets or sets the maximum number of individual samples the buffer can store. Its unit is samples.

*   **`public float FillPercentage { get; set; }`**
    Gets or sets the percentage of the buffer currently occupied. `AudioBuffer.GetStats()` computes it as `(float)CurrentCount / Capacity * 100f`, so an empty buffer reports `0`, a half-full buffer reports `50`, and a full buffer reports `100`. Its unit is percent, not a fraction between 0 and 1.

*   **`public double DurationSeconds { get; set; }`**
    Gets or sets the duration represented by the samples currently in the buffer. `AudioBuffer.GetStats()` obtains this value from `AudioBuffer.GetDurationSeconds()`, which computes `(double)CurrentCount / SampleRate`. Its unit is seconds.

## Usage

The returned object is a snapshot. Later writes, reads, or clears on the buffer do not update an already returned `AudioBufferStats` instance; call `GetStats()` again to obtain current values.

```csharp
using NAudioVisualizer.Domain.Models;

var buffer = new AudioBuffer(capacity: 48_000, sampleRate: 48_000, channelCount: 1);
buffer.Write(new float[12_000]);

AudioBufferStats stats = buffer.GetStats();

Console.WriteLine($"Samples written: {stats.SamplesWritten}"); // 12000 samples
Console.WriteLine($"Samples buffered: {stats.CurrentCount}");  // 12000 samples
Console.WriteLine($"Capacity: {stats.Capacity}");              // 48000 samples
Console.WriteLine($"Fill: {stats.FillPercentage:F1}%");        // 25.0%
Console.WriteLine($"Duration: {stats.DurationSeconds:F2}s");   // 0.25s
```

## Notes

*   All properties have public getters and setters. Changing a property on an `AudioBufferStats` object does not change the source `AudioBuffer`.
*   `CurrentCount`, `Capacity`, and `SamplesWritten` count individual samples.
*   `DurationSeconds` follows the implementation's direct `CurrentCount / SampleRate` calculation; `ChannelCount` is not part of that calculation.
