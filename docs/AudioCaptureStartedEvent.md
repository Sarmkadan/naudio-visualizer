# AudioCaptureStartedEvent

The `AudioCaptureStartedEvent` class serves as a payload object triggered when an audio capture session is initiated within the `naudio-visualizer` framework. It provides essential metadata regarding the capture configuration, timing information, and the initial set of processed audio analysis data, allowing downstream consumers to initialize visualizers, recording buffers, or real-time analysis tools efficiently.

## API

### Configuration and Identification
*   **`DeviceId`** (`int`): The unique identifier of the audio input device being used for the capture session.
*   **`SampleRate`** (`int`): The sampling rate of the captured audio in Hertz (Hz).
*   **`ChannelCount`** (`int`): The number of audio channels active in the capture stream (e.g., 1 for mono, 2 for stereo).

### Timing and Telemetry
*   **`StartTime`** (`DateTime`): The system timestamp indicating when the capture process began.
*   **`StopTime`** (`DateTime`): The system timestamp indicating when the capture process stopped.
*   **`Duration`** (`TimeSpan`): The total elapsed time of the capture session.
*   **`ElapsedTime`** (`TimeSpan`): The duration elapsed since the session start at the time this event was generated.
*   **`GenerationTimeMs`** (`long`): The timestamp (in milliseconds) of when this event object was generated.
*   **`AnalysisTimeMs`** (`long`): The duration (in milliseconds) required to perform the initial analysis on the captured frame.

### Data Payloads
*   **`Frame`** (`AudioFrame`, `required`): The raw audio frame data associated with the start of the capture.
*   **`Waveform`** (`WaveformData`, `required`): The initial waveform analysis data derived from the captured audio.
*   **`Spectrum`** (`SpectrumData`, `required`): The initial frequency spectrum analysis data derived from the captured audio.
*   **`Spectrogram`** (`SpectrogramData`, `required`): The initial spectrogram analysis data derived from the captured audio.

### Processing Metrics
*   **`TotalSamplesCaptured`** (`long`): The running total count of audio samples captured up to this point.
*   **`FrameSequenceNumber`** (`long`): The monotonic sequence number of the current audio frame.
*   **`FrameCount`** (`int`): The total number of audio frames processed in this session.
*   **`TimeFramesProcessed`** (`int`): The count of time-domain frames processed by the analyzer.
*   **`PeakMagnitude`** (`float`): The maximum absolute amplitude value detected in the current analysis window.

## Usage

### Example 1: Subscribing to the Capture Started Event
This example demonstrates a basic event handler that logs capture initiation and displays key configuration parameters.

```csharp
public void OnAudioCaptureStarted(AudioCaptureStartedEvent e)
{
    Console.WriteLine($"Capture started on Device ID: {e.DeviceId}");
    Console.WriteLine($"Configured for {e.SampleRate}Hz, {e.ChannelCount} channels.");
    Console.WriteLine($"Initial peak magnitude: {e.PeakMagnitude}");
}
```

### Example 2: Initializing a Visualizer Component
This example shows how a UI component might use the initial `WaveformData` and `SpectrumData` to set up its display state upon the start of a capture session.

```csharp
public void InitializeVisualizer(AudioCaptureStartedEvent e)
{
    // Ensure required data payloads are present
    var initialWaveform = e.Waveform;
    var initialSpectrum = e.Spectrum;

    // Use initial data to set visualizer state
    this.visualizerUI.ResetBuffers();
    this.visualizerUI.UpdateWaveform(initialWaveform);
    this.visualizerUI.UpdateFrequencyBand(initialSpectrum);
    
    this.isVisualizing = true;
}
```

## Notes

*   **Required Fields:** The `Frame`, `Waveform`, `Spectrum`, and `Spectrogram` properties are marked as `required`. Consumers must ensure these properties are initialized before the event is processed, as they are essential for standard rendering operations.
*   **Thread Safety:** This event class is designed as a data-transfer object (DTO). Instances are intended to be immutable once generated to ensure safe consumption across multiple threads (e.g., audio capture thread vs. UI render thread).
*   **Data Latency:** While `GenerationTimeMs` provides the time of object creation, consumers should not rely on this for high-precision synchronization across different subsystems if significant processing latency occurs between capture and event dispatch.
*   **Edge Cases:** If `TotalSamplesCaptured` or `FrameCount` return unexpected values (e.g., zero at start), handle these gracefully, as they typically represent the very first event emitted immediately following device initialization.
