# AudioCaptureService

The `AudioCaptureService` is a core component of the `naudio-visualizer` project responsible for managing the lifecycle of audio input from system devices. It handles device enumeration, initialization, asynchronous recording control, and provides access to real-time audio data buffers and metadata. This service acts as the bridge between the underlying audio hardware and the visualization logic, ensuring thread-safe access to audio frames while exposing error states and availability status.

## API

### `GetAvailableDevices`
```csharp
public IReadOnlyList<AudioDevice> GetAvailableDevices { get; }
```
Retrieves a read-only list of all currently detected audio input devices. This property allows the consumer to enumerate available hardware before initialization. It returns an `IReadOnlyList<AudioDevice>` and does not throw exceptions under normal operation, though it may return an empty list if no input devices are present.

### `Initialize`
```csharp
public void Initialize()
```
Prepares the service for recording by configuring the selected audio device and allocating necessary resources. This method must be called before invoking `StartRecordingAsync`. It takes no parameters and returns void. It may throw an exception if the default device is unavailable, if the audio engine is already initialized, or if the underlying audio driver fails to respond.

### `StartRecordingAsync`
```csharp
public async Task StartRecordingAsync()
```
Asynchronously begins capturing audio data from the initialized device. This method initiates the background capture loop and populates the internal buffer. It takes no parameters and returns a `Task` that completes when recording has successfully started. It may throw if the service has not been initialized, if recording is already in progress, or if the device is lost during startup.

### `StopRecordingAsync`
```csharp
public async Task StopRecordingAsync()
```
Asynchronously halts the audio capture process and releases the active stream handle. This method ensures that any pending I/O operations are completed before returning. It takes no parameters and returns a `Task`. It may throw if the service is not currently recording or if an error occurs while shutting down the audio stream.

### `GetCurrentMetadata`
```csharp
public AudioMetadata? GetCurrentMetadata()
```
Returns the current configuration details of the active audio stream, such as sample rate, channel count, and bit depth. If no stream is active or metadata has not yet been populated, this method returns `null`. It takes no parameters and does not throw exceptions.

### `GetBufferedAudio`
```csharp
public float[]? GetBufferedAudio()
```
Retrieves a copy of the most recently captured audio samples as a normalized float array (typically ranging from -1.0f to 1.0f). If the buffer is empty or cleared, this method returns `null`. It takes no parameters. This operation involves memory allocation for the array copy.

### `GetAudioBuffer`
```csharp
public AudioBuffer? GetAudioBuffer()
```
Returns the raw `AudioBuffer` object containing the current frame data, potentially including additional context like timestamp or buffer length. If no data is available, it returns `null`. This method provides direct access to the buffer structure without copying the underlying array immediately.

### `ClearBuffer`
```csharp
public void ClearBuffer()
```
Empties the internal audio buffer, discarding any unread samples. This is useful for resetting the visualization state or recovering from lag spikes. It takes no parameters and returns void. It does not stop the recording process.

### `Dispose`
```csharp
public void Dispose()
```
Releases all unmanaged resources associated with the service, including stopping any active recording and disposing of the audio device handle. This method should be called when the service is no longer needed. It implements the standard disposal pattern and may throw if called multiple times concurrently without proper synchronization by the caller, though it is generally safe to call repeatedly.

### `Frame`
```csharp
public AudioFrame? Frame { get; }
```
Gets the most recently processed `AudioFrame` object. This property provides snapshot access to the latest individual frame of audio data available for visualization. It returns `null` if no frames have been captured yet. Accessing this property is thread-safe relative to the capture loop.

### `IsAvailable`
```csharp
public bool IsAvailable { get; }
```
Indicates whether the service is currently in a valid state to capture or provide audio data. This returns `true` only if the service is initialized, not disposed, and not in an error state. It is a read-only boolean property.

### `Exception`
```csharp
public Exception? Exception { get; }
```
Exposes the last critical exception encountered during the recording lifecycle (e.g., device disconnect, driver failure). If the service is operating normally, this returns `null`. Consumers should check this property if `IsAvailable` returns `false` unexpectedly.

## Usage

### Example 1: Basic Initialization and Recording Lifecycle
This example demonstrates enumerating devices, initializing the service, starting recording, and handling the shutdown sequence.

```csharp
using var captureService = new AudioCaptureService();

// Check for available devices
var devices = captureService.GetAvailableDevices;
if (devices.Count == 0)
{
    Console.WriteLine("No audio input devices found.");
    return;
}

try
{
    // Initialize and start capturing
    captureService.Initialize();
    await captureService.StartRecordingAsync();

    Console.WriteLine("Recording started...");
    
    // Simulate recording duration
    await Task.Delay(5000);

    // Stop and cleanup
    await captureService.StopRecordingAsync();
    Console.WriteLine("Recording stopped.");
}
catch (Exception ex)
{
    Console.WriteLine($"Audio error: {ex.Message}");
    // Check detailed exception state
    if (captureService.Exception != null)
    {
        Console.WriteLine($"Internal State Error: {captureService.Exception.Message}");
    }
}
```

### Example 2: Real-Time Buffer Access for Visualization
This example shows how to poll the audio buffer within a loop to process data for a visualizer, ensuring proper null checks and buffer management.

```csharp
// Assume captureService is already initialized and running
while (captureService.IsAvailable)
{
    // Retrieve the latest audio frame
    var frame = captureService.Frame;
    if (frame != null)
    {
        // Option A: Get raw float array for processing
        var samples = captureService.GetBufferedAudio();
        if (samples != null)
        {
            ProcessVisualizationData(samples);
        }
        
        // Option B: Access metadata if needed dynamically
        var metadata = captureService.GetCurrentMetadata();
        if (metadata != null)
        {
            UpdateSampleRateDisplay(metadata.SampleRate);
        }
    }

    // Prevent tight looping; sync with render rate
    await Task.Delay(16); 
    
    // Optional: Clear buffer if visualization falls behind significantly
    if (detectLag()) 
    {
        captureService.ClearBuffer();
    }
}

if (captureService.Exception != null)
{
    throw captureService.Exception;
}
```

## Notes

*   **Thread Safety**: The properties `Frame`, `GetBufferedAudio`, and `GetAudioBuffer` are designed to be accessed from threads other than the internal capture thread (e.g., a UI or rendering thread). However, the returned arrays or objects should be treated as snapshots; modifications to the returned `float[]` from `GetBufferedAudio` will not affect the internal state, but references to `AudioBuffer` or `AudioFrame` should not be stored long-term as their internal data may be reused or invalidated by the next capture cycle.
*   **Initialization Order**: Calling `StartRecordingAsync` before `Initialize` will result in an exception. Similarly, calling `StopRecordingAsync` without an active recording session may throw or result in a no-op depending on the internal state machine implementation.
*   **Resource Management**: The class implements `IDisposable`. It is critical to call `Dispose` (or use a `using` statement) when the service is no longer needed to ensure the audio device handle is released back to the OS. Failure to do so may prevent other applications from accessing the microphone.
*   **Error Handling**: The service does not automatically restart upon device failure. If the `Exception` property becomes non-null or `IsAvailable` flips to `false` during recording, the consumer must explicitly stop, dispose, and re-initialize the service to attempt recovery.
*   **Buffer Latency**: `ClearBuffer` discards data immediately. In high-latency scenarios, frequent clearing may result in gaps in the visualization, while never clearing may cause the visualizer to process stale data if the consumption rate is slower than the capture rate.
