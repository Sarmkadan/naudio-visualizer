// src/README.md
// ... rest of the file content ...
## AudioStreamException
The `AudioStreamException` is thrown when an error occurs while working with audio streams. It provides information about the error through its `ErrorCode` property. This exception can be used to handle different types of audio stream errors.

### Usage Example

```csharp
try
{
    // Attempt to read from an audio stream
    using (var stream = new AudioStream())
    {
        // ... read from stream ...
    }
}
catch (AudioStreamException ex)
{
    Console.WriteLine($"Audio stream error: {ex.ErrorCode}");
}
```

## AudioDeviceException
The `AudioDeviceException` is thrown when an audio device is not found or inaccessible. It includes an optional `DeviceIndex` property that identifies which specific audio device caused the error, enabling targeted error handling and recovery attempts.

```csharp
try
{
// Attempt to use audio device with index 2
using (var audioClient = new WasapiAudioClient(2))
{
// ... audio processing ...
}
}
catch (AudioDeviceException ex) when (ex.DeviceIndex.HasValue)
{
Console.WriteLine($"Audio device {ex.DeviceIndex} is not available: {ex.Message}");
// Attempt fallback to default device
}
catch (AudioDeviceException ex)
{
Console.WriteLine($"Audio device error: {ex.Message}");
}
```

## VisualizationException
The `VisualizationException` is thrown when an error occurs during the rendering or processing of a visualization. It carries an optional `VisualizationType` property that indicates which visualization component caused the failure, allowing callers to handle different visualizers separately.

```csharp
try
{
    // Simulate a failure in a specific visualization
    throw new VisualizationException("Failed to render waveform");
}
catch (VisualizationException ex)
{
    Console.WriteLine($"Error in {ex.VisualizationType ?? "unknown"}: {ex.Message}");
}
```
