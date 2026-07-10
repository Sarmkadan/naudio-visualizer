# EventPublisher

The `EventPublisher` class serves as the central messaging hub for the `naudio-visualizer` application, implementing a publish-subscribe pattern to decouple audio processing, analysis, and visualization components. It provides a static interface for broadcasting system events across the application and allows components to subscribe to specific event types for reactive processing.

## API

*   `PublishAudioCaptureStarted`: Publishes notification that audio capture has commenced.
*   `PublishAudioCaptureStopped`: Publishes notification that audio capture has ended.
*   `PublishAudioFrameCaptured`: Publishes notification that a new audio frame has been captured.
*   `PublishWaveformGenerated`: Publishes notification that a new waveform data set has been generated.
*   `PublishSpectrumAnalyzed`: Publishes notification that a new spectrum analysis result is available.
*   `PublishSpectrogramGenerated`: Publishes notification that a new spectrogram data set has been generated.
*   `PublishVisualizationRenderStarted`: Publishes notification that a visualization render cycle has begun.
*   `PublishVisualizationRenderCompleted`: Publishes notification that a visualization render cycle has finished.
*   `PublishVisualizationError`: Publishes notification that an error occurred during visualization.
*   `PublishAudioDeviceConnected`: Publishes notification that an audio device has been connected.
*   `PublishAudioDeviceDisconnected`: Publishes notification that an audio device has been disconnected.
*   `PublishVisualizationSettingsChanged`: Publishes notification that visualization configuration settings have been modified.
*   `PublishPerformanceMetrics`: Publishes notification containing application performance metrics.
*   `PublishDataExportStarted`: Publishes notification that a data export operation has initiated.
*   `PublishDataExportCompleted`: Publishes notification that a data export operation has finished.
*   `PublishApplicationShuttingDown`: Publishes notification that the application is preparing to shut down.
*   `Subscribe<T>`: Registers a handler for event type `T`. Returns an `IDisposable` that, when disposed, unregisters the handler.
*   `Reset`: Clears all registered subscriptions and resets the internal event state.

## Usage

### Subscribing to Events

```csharp
// Register a subscriber for visualization error events
IDisposable subscription = EventPublisher.Subscribe<VisualizationErrorEventArgs>(e =>
{
    Console.WriteLine($"Visualization error encountered: {e.ErrorMessage}");
});

// Unsubscribe when the component is disposed to prevent memory leaks
// subscription.Dispose();
```

### Publishing Events

```csharp
// Notify the system that a rendering cycle has started
EventPublisher.PublishVisualizationRenderStarted();

try 
{
    // Perform render logic...
}
finally
{
    // Ensure the completion event is published
    EventPublisher.PublishVisualizationRenderCompleted();
}
```

## Notes

*   **Thread Safety:** The `EventPublisher` is designed to be thread-safe for both publishing events and managing subscriptions.
*   **Memory Management:** Failure to dispose of the `IDisposable` returned by `Subscribe<T>` will result in a memory leak, as the publisher maintains strong references to subscriber handlers.
*   **Event Handling:** Subscribers should handle exceptions internally. An unhandled exception within a subscriber handler may disrupt the delivery of events to subsequent subscribers.
*   **Reset:** The `Reset` method is destructive and should only be used when the application state requires a complete clearing of the event bus, such as during full component re-initialization.
