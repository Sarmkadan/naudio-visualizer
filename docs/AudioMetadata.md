# AudioMetadata

`AudioMetadata` serves as a container for telemetry, performance metrics, and configuration data related to an active audio capture or analysis session within the `naudio-visualizer` library. It maintains the current state, including sample statistics, volume levels, and diagnostic information such as buffer underrun counts and CPU usage, facilitating real-time monitoring and logging of audio processing pipelines.

## API

### Properties

*   **`Guid SessionId`**: A unique identifier for the specific audio session.
*   **`DateTime StartTime`**: The timestamp when the audio session was initialized.
*   **`DateTime LastUpdateTime`**: The timestamp of the most recent data refresh.
*   **`int SampleRate`**: The frequency of audio samples in Hz (e.g., 44100).
*   **`int ChannelCount`**: The number of audio channels active in the session (e.g., 1 for mono, 2 for stereo).
*   **`int BitDepth`**: The bit depth of the audio samples (e.g., 16, 24).
*   **`long TotalSamplesCaptured`**: The cumulative total of audio samples captured since the session started.
*   **`long TotalFramesProcessed`**: The cumulative total of audio frames processed.
*   **`float CurrentLevel`**: The current instantaneous amplitude or volume level.
*   **`float PeakLevel`**: The highest amplitude level observed during the session.
*   **`float AverageLevel`**: The average amplitude level calculated over the session duration.
*   **`double CurrentDurationSeconds`**: The elapsed time of the session in seconds.
*   **`AudioDevice? AudioDevice`**: The audio capture device associated with the session. May be `null` if no device is selected.
*   **`bool IsCapturing`**: A flag indicating whether the audio capture process is currently active.
*   **`float CpuUsagePercent`**: The reported percentage of CPU usage attributable to the current audio processing task.
*   **`int BufferUnderruns`**: The count of buffer underrun events encountered during capture.
*   **`float DominantFrequency`**: The currently detected dominant frequency in Hz.

### Methods

*   **`void UpdateDuration()`**: Recalculates `CurrentDurationSeconds` based on the elapsed time since `StartTime`.
*   **`void UpdateLevelMetrics()`**: Refreshes amplitude-related metrics (`CurrentLevel`, `PeakLevel`, `AverageLevel`) based on the latest internal buffer state.
*   **`void RecordBufferUnderrun()`**: Increments the `BufferUnderruns` counter to track performance degradation.

## Usage

### Accessing Session Telemetry
```csharp
public void LogSessionState(AudioMetadata metadata)
{
    Console.WriteLine($"Session {metadata.SessionId} is active: {metadata.IsCapturing}");
    Console.WriteLine($"Current Level: {metadata.CurrentLevel:F2}, Peak: {metadata.PeakLevel:F2}");
    Console.WriteLine($"Frequency: {metadata.DominantFrequency} Hz");
}
```

### Updating Metrics After Processing
```csharp
public void ProcessAudioBuffer(AudioMetadata metadata)
{
    // ... processing logic ...
    
    // Update metrics after data processing
    metadata.UpdateLevelMetrics();
    metadata.UpdateDuration();
    
    if (bufferUnderflowOccurred)
    {
        metadata.RecordBufferUnderrun();
    }
}
```

## Notes

*   **Thread Safety**: This class is intended to be accessed from multiple threads (typically a high-priority capture thread updating metrics and a UI thread reading them). Consumers must implement appropriate locking or synchronization mechanisms when accessing or modifying `AudioMetadata` properties to avoid race conditions.
*   **Nullable References**: The `AudioDevice` property is nullable (`AudioDevice?`). Always verify its state before accessing properties of the device instance to prevent `NullReferenceException`.
*   **Performance**: `UpdateLevelMetrics` and `UpdateDuration` are computationally inexpensive; however, excessive calls in extremely high-frequency processing loops should be profiled to ensure minimal impact on overall capture latency.
