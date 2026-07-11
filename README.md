// src/README.md

// ... existing content ...

## AudioCaptureService

`AudioCaptureService` is a service responsible for managing audio capture operations, including device selection, initialization, and recording. It provides a simple and asynchronous API for capturing audio data from available devices.

### Usage Example

```csharp
using Domain.Services;

// Create an instance of AudioCaptureService
var audioCaptureService = new AudioCaptureService();

// Get available audio devices
var devices = audioCaptureService.GetAvailableDevices;

// Initialize the service with the selected device
await audioCaptureService.Initialize(devices[0]);

// Start recording audio
await audioCaptureService.StartRecordingAsync();

// Get the current audio metadata
var metadata = audioCaptureService.GetCurrentMetadata;

// Get the buffered audio data
var audioData = audioCaptureService.GetBufferedAudio;

// Get the current audio frame
var frame = audioCaptureService.Frame;

// Check if the service is available
var isAvailable = audioCaptureService.IsAvailable;

// Stop recording audio
await audioCaptureService.StopRecordingAsync();

// Clear the audio buffer
audioCaptureService.ClearBuffer();

// Dispose of the service
audioCaptureService.Dispose();
```

// ... existing content ...
