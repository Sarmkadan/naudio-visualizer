# Audio Visualization Events

The event classes in `NAudioVisualizer.Events` describe lifecycle, processing, rendering, device, settings, performance, export, and shutdown notifications. Each class is a mutable reference type whose properties can be set only during initialization. `EventPublisher` constructs these events and sends them through its shared `EventBus` instance.

## API

### `AudioCaptureStartedEvent`

Raised by `EventPublisher.PublishAudioCaptureStarted(int deviceId, int sampleRate, int channelCount)`.

*   **`int DeviceId`**: The identifier supplied as `deviceId`.
*   **`int SampleRate`**: The capture sample rate in Hz supplied as `sampleRate`.
*   **`int ChannelCount`**: The number of captured channels supplied as `channelCount`.
*   **`DateTime StartTime`**: Initialized to `DateTime.UtcNow` when the event is created.

### `AudioCaptureStoppedEvent`

Raised by `EventPublisher.PublishAudioCaptureStopped(int deviceId, long totalSamples, TimeSpan duration)`.

*   **`int DeviceId`**: The identifier supplied as `deviceId`.
*   **`long TotalSamplesCaptured`**: Set from `totalSamples`.
*   **`TimeSpan Duration`**: The capture duration supplied as `duration`.
*   **`DateTime StopTime`**: Initialized to `DateTime.UtcNow` when the event is created.

### `AudioFrameCapturedEvent`

Raised by `EventPublisher.PublishAudioFrameCaptured(AudioFrame frame, long sequenceNumber, TimeSpan elapsed)`.

*   **`required AudioFrame Frame`**: The captured frame supplied as `frame`.
*   **`long FrameSequenceNumber`**: Set from `sequenceNumber`.
*   **`TimeSpan ElapsedTime`**: The elapsed capture time supplied as `elapsed`.

### `WaveformGeneratedEvent`

Raised by `EventPublisher.PublishWaveformGenerated(WaveformData waveform, long generationTimeMs, int frameCount)`.

*   **`required WaveformData Waveform`**: The generated waveform supplied as `waveform`.
*   **`long GenerationTimeMs`**: The generation time in milliseconds supplied as `generationTimeMs`.
*   **`int FrameCount`**: The number of waveform frames supplied as `frameCount`.

### `SpectrumAnalyzedEvent`

Raised by `EventPublisher.PublishSpectrumAnalyzed(SpectrumData spectrum, long analysisTimeMs, float peakMagnitude)`.

*   **`required SpectrumData Spectrum`**: The analyzed spectrum supplied as `spectrum`.
*   **`long AnalysisTimeMs`**: The analysis time in milliseconds supplied as `analysisTimeMs`.
*   **`float PeakMagnitude`**: The detected peak magnitude supplied as `peakMagnitude`.

### `SpectrogramGeneratedEvent`

Raised by `EventPublisher.PublishSpectrogramGenerated(SpectrogramData spectrogram, long generationTimeMs, int timeFrames)`.

*   **`required SpectrogramData Spectrogram`**: The generated spectrogram supplied as `spectrogram`.
*   **`long GenerationTimeMs`**: The generation time in milliseconds supplied as `generationTimeMs`.
*   **`int TimeFramesProcessed`**: Set from `timeFrames`.

### `VisualizationRenderStartedEvent`

Raised by `EventPublisher.PublishVisualizationRenderStarted(string visualizationType, int width, int height)`.

*   **`string VisualizationType`**: The supplied visualization name; its declaration defaults to `string.Empty`.
*   **`int Width`**: The supplied render-surface width in pixels.
*   **`int Height`**: The supplied render-surface height in pixels.
*   **`DateTime StartTime`**: Initialized to `DateTime.UtcNow` when the event is created.

### `VisualizationRenderCompletedEvent`

Raised by `EventPublisher.PublishVisualizationRenderCompleted(string visualizationType, long renderTimeMs, int frameRate)`.

*   **`string VisualizationType`**: The supplied visualization name; its declaration defaults to `string.Empty`.
*   **`long RenderTimeMs`**: The supplied render time in milliseconds.
*   **`int FrameRate`**: The supplied frame rate in frames per second.
*   **`DateTime CompletionTime`**: Initialized to `DateTime.UtcNow` when the event is created.

### `VisualizationErrorEvent`

Raised by `EventPublisher.PublishVisualizationError(string errorMessage, Exception exception, string componentName, int errorCode = 0)`.

*   **`required string ErrorMessage`**: The human-readable error description supplied as `errorMessage`.
*   **`required Exception Exception`**: The exception supplied as `exception`.
*   **`string ComponentName`**: The supplied component name; its declaration defaults to `string.Empty`.
*   **`int ErrorCode`**: The supplied numeric error code. The publisher helper defaults this argument to `0`.
*   **`DateTime OccurredAt`**: Initialized to `DateTime.UtcNow` when the event is created.

### `AudioDeviceConnectedEvent`

Raised by `EventPublisher.PublishAudioDeviceConnected(int deviceId, string deviceName, int maxChannels)`.

*   **`int DeviceId`**: The supplied device identifier.
*   **`required string DeviceName`**: The supplied device display name.
*   **`int MaxChannels`**: The supplied maximum supported channel count.
*   **`DateTime ConnectedAt`**: Initialized to `DateTime.UtcNow` when the event is created.

### `AudioDeviceDisconnectedEvent`

Raised by `EventPublisher.PublishAudioDeviceDisconnected(int deviceId, string deviceName)`.

*   **`int DeviceId`**: The supplied device identifier.
*   **`required string DeviceName`**: The supplied device display name.
*   **`DateTime DisconnectedAt`**: Initialized to `DateTime.UtcNow` when the event is created.

### `VisualizationSettingsChangedEvent`

Raised by `EventPublisher.PublishVisualizationSettingsChanged(string settingName, object? oldValue, object? newValue)`.

*   **`required string SettingName`**: The supplied name of the changed setting.
*   **`object? OldValue`**: The supplied previous value; it can be `null`.
*   **`object? NewValue`**: The supplied replacement value; it can be `null`.
*   **`DateTime ChangedAt`**: Initialized to `DateTime.UtcNow` when the event is created.

### `PerformanceMetricsEvent`

Raised by `EventPublisher.PublishPerformanceMetrics(double cpuUsage, long memoryBytes, int framesProcessed, double avgFrameTimeMs)`.

*   **`double CpuUsagePercent`**: The supplied CPU usage percentage.
*   **`long MemoryUsageBytes`**: The supplied memory usage in bytes.
*   **`int FramesProcessed`**: The supplied number of processed frames.
*   **`double AverageFrameTimeMs`**: The average frame processing time in milliseconds, set from `avgFrameTimeMs`.
*   **`DateTime RecordedAt`**: Initialized to `DateTime.UtcNow` when the event is created.

### `DataExportStartedEvent`

Raised by `EventPublisher.PublishDataExportStarted(string exportPath, string format, int dataPointCount)`.

*   **`required string ExportPath`**: The supplied export destination path.
*   **`required string Format`**: The supplied export format identifier.
*   **`int DataPointCount`**: The supplied number of data points being exported.
*   **`DateTime StartTime`**: Initialized to `DateTime.UtcNow` when the event is created.

### `DataExportCompletedEvent`

Raised by `EventPublisher.PublishDataExportCompleted(string exportPath, string format, long fileSize, long exportTimeMs, bool success)`.

*   **`required string ExportPath`**: The supplied export destination path.
*   **`required string Format`**: The supplied export format identifier.
*   **`long FileSize`**: The supplied exported file size in bytes.
*   **`long ExportTimeMs`**: The supplied export duration in milliseconds.
*   **`bool Success`**: The supplied completion status.
*   **`DateTime CompletionTime`**: Initialized to `DateTime.UtcNow` when the event is created.

### `ApplicationShuttingDownEvent`

Raised by `EventPublisher.PublishApplicationShuttingDown(string reason, long uptimeMs)`.

*   **`string Reason`**: The supplied shutdown reason; its declaration defaults to `string.Empty`.
*   **`long UptimeMs`**: The supplied application uptime in milliseconds.
*   **`DateTime ShutdownTime`**: Initialized to `DateTime.UtcNow` when the event is created.

## Usage

Subscribe to a specific event class with `EventPublisher.Subscribe<T>`, retain the returned subscription for as long as notifications are needed, and call the matching publisher helper to raise the event.

```csharp
using NAudioVisualizer.Events;

using IDisposable subscription =
    EventPublisher.Subscribe<AudioCaptureStartedEvent>(captured =>
    {
        Console.WriteLine(
            $"Device {captured.DeviceId}: " +
            $"{captured.SampleRate} Hz, {captured.ChannelCount} channels");
    });

EventPublisher.PublishAudioCaptureStarted(
    deviceId: 0,
    sampleRate: 48_000,
    channelCount: 2);
```

## Notes

*   All event properties use `init` accessors. Properties marked `required` must be initialized when callers construct those event classes directly.
*   The publisher helpers populate their event properties from the supplied arguments. Timestamp properties are not helper parameters; their declaration initializers capture `DateTime.UtcNow` during event construction.
*   `EventPublisher.PublishVisualizationError` is the only catalog helper with an optional argument: `errorCode` defaults to `0`.
*   Publishing is synchronous through `EventBus.Publish`. Subscriber exceptions are caught by the bus and written to debug output so remaining subscribers can still be invoked.
