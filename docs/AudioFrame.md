# AudioFrame

The `AudioFrame` class serves as a structured container for a specific segment of audio data, facilitating efficient processing, analysis, and visualization within the `naudio-visualizer` library. It encapsulates both raw PCM samples and pre-calculated metadata, such as peak amplitude and RMS energy, to minimize redundant computations during real-time audio analysis.

## API

### Properties

*   **`Guid Id`**
    A unique identifier for the specific audio frame instance, useful for tracking and debugging asynchronous processing pipelines.
*   **`float[] Samples`**
    The raw audio data represented as an array of floating-point values, typically normalized between -1.0 and 1.0.
*   **`int ChannelCount`**
    The number of audio channels present in the frame (e.g., 1 for mono, 2 for stereo).
*   **`int SampleRate`**
    The number of samples per second, defining the temporal resolution of the audio data.
*   **`DateTime Timestamp`**
    The precise point in time when the audio frame was captured or generated.
*   **`long FrameIndex`**
    A monotonically increasing sequential index representing the position of this frame within an audio stream.
*   **`double DurationSeconds`**
    The calculated duration of the audio frame in seconds, derived from the sample count and sample rate.
*   **`float PeakAmplitude`**
    The maximum absolute value found among the samples in this frame, often used for visualization scaling.
*   **`float RmsEnergy`**
    The Root Mean Square energy level of the frame, providing an indication of the perceived loudness.
*   **`bool IsValid`**
    A flag indicating whether the frame contains complete and consistent data. If false, downstream processes should treat the frame as corrupted or incomplete.

### Methods

*   **`AudioFrame()`**
    Initializes a new instance of the `AudioFrame` class.
*   **`float[] GetChannelData(int channelIndex)`**
    Extracts the samples for a specific audio channel.
    *   **Parameters:** `channelIndex` (the 0-based index of the channel to retrieve).
    *   **Returns:** An array of `float` representing the sample data for the requested channel.
    *   **Exceptions:** Throws `ArgumentOutOfRangeException` if `channelIndex` is less than zero or greater than or equal to `ChannelCount`.

## Usage

### Extracting Channel Data for Visualization
```csharp
public void UpdateVisualizer(AudioFrame frame)
{
    if (!frame.IsValid) return;

    // Extract left channel (index 0) for waveform visualization
    float[] leftChannelSamples = frame.GetChannelData(0);
    
    // Update UI elements based on frame metadata
    Console.WriteLine($"Processing Frame {frame.FrameIndex} at Peak: {frame.PeakAmplitude}");
}
```

### Filtering Frames by Duration
```csharp
public List<AudioFrame> FilterShortFrames(IEnumerable<AudioFrame> frames)
{
    // Return only frames that meet a minimum duration threshold
    return frames.Where(f => f.IsValid && f.DurationSeconds >= 0.01).ToList();
}
```

## Notes

*   **Thread Safety:** The `AudioFrame` instance is not inherently thread-safe. While property getters are generally safe to access concurrently, modifying the `Samples` array or invoking `GetChannelData` while the frame is being modified by another thread can result in race conditions.
*   **Data Integrity:** Always check the `IsValid` property before performing analysis. If a source stream produces a frame that is incomplete or misaligned due to buffer underruns, `IsValid` will be false, and accessing `Samples` may lead to unexpected results or exceptions depending on the implementation state.
*   **Memory Usage:** The `Samples` array is typically allocated on the heap. In high-performance, real-time scenarios, frequent creation of large `AudioFrame` objects may increase garbage collection pressure. Consider object pooling if the frame rate is high and latency requirements are strict.
