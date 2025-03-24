# AudioBuffer

The `AudioBuffer` class serves as a fixed-size, circular buffer designed for efficient streaming and visualization of real-time audio data within the `naudio-visualizer` project. It manages a pre-allocated memory space for floating-point audio samples, supporting multi-channel configurations and providing mechanisms to write incoming data, peek at current values without consuming them, or read data sequentially. The type exposes comprehensive statistics regarding buffer fill levels, duration, and sample counts, enabling precise synchronization and resource monitoring for audio processing pipelines.

## API

### Properties

*   **`public int Count`**
    Gets the current number of valid samples stored in the buffer. This value fluctuates as data is written and read.

*   **`public int Capacity`**
    Gets the maximum number of samples the buffer can hold. This value is set at initialization and remains constant.

*   **`public int SampleRate`**
    Gets the sample rate (in Hz) configured for this buffer instance, used to calculate time-based metrics.

*   **`public int ChannelCount`**
    Gets the number of audio channels (e.g., 1 for mono, 2 for stereo) configured for this buffer.

*   **`public bool IsFull`**
    Gets a value indicating whether the buffer has reached its maximum capacity and cannot accept more data without overwriting or blocking (depending on implementation strategy).

*   **`public bool IsEmpty`**
    Gets a value indicating whether the buffer currently contains no valid samples.

*   **`public int AvailableSpace`**
    Gets the number of additional samples that can be written to the buffer before it becomes full.

*   **`public long SamplesWritten`**
    Gets the cumulative total number of samples written to this buffer instance since its creation. This counter does not reset when the buffer is cleared or overwritten.

*   **`public int CurrentCount`**
    An alias for `Count`, representing the immediate number of samples available for reading.

*   **`public float FillPercentage`**
    Gets the buffer utilization as a floating-point value between 0.0f and 1.0f, calculated based on `Count` and `Capacity`.

*   **`public double DurationSeconds`**
    Gets the duration of the audio currently held in the buffer in seconds, derived from `Count`, `SampleRate`, and `ChannelCount`.

### Methods

*   **`public AudioBuffer(int capacity, int sampleRate, int channelCount)`**
    Initializes a new instance of the `AudioBuffer` class.
    *   **Parameters**:
        *   `capacity`: The maximum number of samples to store.
        *   `sampleRate`: The audio sample rate in Hz.
        *   `channelCount`: The number of audio channels.
    *   **Throws**: `ArgumentOutOfRangeException` if capacity, sampleRate, or channelCount is less than or equal to zero.

*   **`public void Write(float[] data, int offset, int count)`**
    Writes a block of audio samples into the buffer.
    *   **Parameters**:
        *   `data`: The source array containing audio samples.
        *   `offset`: The zero-based index in `data` at which to begin copying.
        *   `count`: The number of samples to write.
    *   **Behavior**: If the write operation exceeds available space, behavior depends on the specific circular buffer implementation (typically overwrites oldest data or throws).
    *   **Throws**: `ArgumentNullException` if `data` is null; `ArgumentOutOfRangeException` if `offset` or `count` is invalid relative to the `data` array length.

*   **`public float[] Peek(int count)`**
    Retrieves a copy of the next `count` samples without advancing the read pointer or removing data from the buffer.
    *   **Parameters**:
        *   `count`: The number of samples to retrieve.
    *   **Returns**: A new `float[]` array containing the requested samples. If fewer samples are available than requested, the returned array may be smaller or padded depending on implementation specifics.
    *   **Throws**: `ArgumentOutOfRangeException` if `count` is negative.

*   **`public float[] Read(int count)`**
    Reads and removes the next `count` samples from the buffer, advancing the internal read pointer.
    *   **Parameters**:
        *   `count`: The number of samples to read and remove.
    *   **Returns**: A new `float[]` array containing the read samples.
    *   **Throws**: `ArgumentOutOfRangeException` if `count` is negative; `InvalidOperationException` if the buffer does not contain enough data to satisfy the request (if strict reading is enforced).

*   **`public float[] GetAll()`**
    Reads and removes all currently available samples from the buffer.
    *   **Returns**: A `float[]` array containing all valid samples currently in the buffer. Returns an empty array if the buffer is empty.

*   **`public void Clear()`**
    Resets the buffer state, discarding all stored samples. The `Count` becomes 0, and `AvailableSpace` equals `Capacity`. The `SamplesWritten` counter is typically preserved.

*   **`public double GetDurationSeconds()`**
    Calculates and returns the duration of the currently buffered audio in seconds.
    *   **Returns**: A `double` representing the time span of the current `Count` of samples.
    *   **Note**: Functionally equivalent to the `DurationSeconds` property.

*   **`public AudioBufferStats GetStats()`**
    Retrieves a snapshot of the current buffer statistics.
    *   **Returns**: An `AudioBufferStats` object containing aggregated metrics such as peak levels, average amplitude, or detailed fill history (structure defined in `AudioBufferStats`).

## Usage

### Example 1: Real-time Data Ingestion and Visualization Peek
This example demonstrates initializing a stereo buffer and periodically inspecting the latest data for visualization without consuming the stream.

```csharp
// Initialize a 1-second buffer for 44.1kHz stereo audio
int sampleRate = 44100;
int channels = 2;
int capacity = sampleRate * channels; // 1 second of data

var buffer = new AudioBuffer(capacity, sampleRate, channels);

// Simulate incoming audio chunk
float[] incomingChunk = new float[1024]; 
// ... populate incomingChunk with audio data ...

// Write data to the buffer
buffer.Write(incomingChunk, 0, incomingChunk.Length);

// For visualization: Peek at the last 256 samples without removing them
if (buffer.Count >= 256)
{
    float[] visualData = buffer.Peek(256);
    RenderWaveform(visualData);
}

// Check fill level for UI indicators
Console.WriteLine($"Buffer Fill: {buffer.FillPercentage:P2}");
```

### Example 2: Processing Stream with Read and Stats
This example illustrates a consumer pattern where data is read for processing, and cumulative statistics are monitored.

```csharp
var buffer = new AudioBuffer(8192, 48000, 1);

// Write multiple chunks
buffer.Write(GetNextAudioBlock(), 0, 4096);
buffer.Write(GetNextAudioBlock(), 0, 4096);

// Process available data
if (!buffer.IsEmpty)
{
    // Read all available data for batch processing
    float[] processedData = buffer.GetAll();
    PerformAudioAnalysis(processedData);
}

// Retrieve detailed statistics after operation
var stats = buffer.GetStats();
Console.WriteLine($"Total Samples Written: {buffer.SamplesWritten}");
Console.WriteLine($"Current Duration: {buffer.DurationSeconds:F4}s");
Console.WriteLine($"Peak Level: {stats.PeakLevel}");
```

## Notes

*   **Thread Safety**: The provided signatures do not explicitly indicate internal synchronization. In a multi-threaded environment where one thread writes (`Write`) while another reads (`Read`, `Peek`, `GetAll`) or accesses properties (`Count`, `FillPercentage`), external locking mechanisms (e.g., `lock` statements) are required to prevent race conditions and data corruption.
*   **Circular Behavior**: Given the presence of `IsFull`, `AvailableSpace`, and `SamplesWritten` (which accumulates beyond `Capacity`), this buffer likely implements a circular (ring) logic. When `IsFull` is true, subsequent `Write` operations may overwrite the oldest unread data. Consumers must ensure read speeds match write speeds if data loss is unacceptable.
*   **Sample vs. Frame Calculation**: Properties like `Count` and `Capacity` refer to individual samples, not frames. For multi-channel audio, the number of time-domain frames is `Count / ChannelCount`. Calculations for duration automatically account for this, but manual indexing into arrays returned by `Read` or `Peek` must respect the interleaved channel format (e.g., L, R, L, R).
*   **Edge Cases**:
    *   Calling `Read` or `Peek` with a `count` larger than `CurrentCount` may result in partial data returns or exceptions depending on the specific internal enforcement; checking `Count` before calling is recommended.
    *   `GetDurationSeconds` and `DurationSeconds` will return 0.0 if the buffer is empty (`IsEmpty` is true).
    *   The `Clear` method resets the logical count but does not resize the underlying array, making it efficient for repeated reuse without garbage collection pressure.
