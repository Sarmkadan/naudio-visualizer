// src/README.md
// ... rest of the file content ...
## AudioCaptureStartedEvent
The `AudioCaptureStartedEvent` is published when audio capture begins. It provides detailed information about the capture process, including the device ID, sample rate, channel count, and start time.

### Usage Example

```csharp
using Events; // adjust namespace as needed

var eventPublisher = EventPublisher.Subscribe<AudioCaptureStartedEvent>(event =>
{
    Console.WriteLine($"Audio capture started on device {event.DeviceId} at {event.StartTime}");
    Console.WriteLine($"Sample rate: {event.SampleRate} Hz, Channel count: {event.ChannelCount}");
    Console.WriteLine($"Total samples captured: {event.TotalSamplesCaptured}, Duration: {event.Duration}");
    Console.WriteLine($"Frame sequence number: {event.FrameSequenceNumber}, Elapsed time: {event.ElapsedTime}");
    Console.WriteLine($"Waveform: {event.Waveform}");
    Console.WriteLine($"Spectrum: {event.Spectrum}");
    Console.WriteLine($"Spectrogram: {event.Spectrogram}");
});

EventPublisher.PublishAudioCaptureStarted();
```

## EventBus
`EventBus` is a lightweight publish/subscribe system that allows components to communicate without tight coupling. It supports generic event types, keeps track of subscriber counts, and can clean up all subscriptions when needed.

### Usage Example

```csharp
using Events; // adjust namespace as needed

var bus = new EventBus();

// Subscribe to a string event
var subscription = bus.Subscribe<string>(msg => Console.WriteLine($"Received: {msg}"));

// Publish an event
bus.Publish("Hello, world!");

// Check how many subscribers are listening to string events
Console.WriteLine($"Subscribers: {bus.GetSubscriberCount<string>()}");

// Unsubscribe all string listeners
bus.UnsubscribeAll<string>();

// Clear all remaining subscriptions
bus.Clear();

// Dispose the bus when done
bus.Dispose();
```

## EventPublisher
`EventPublisher` is a static helper that publishes a wide range of audio and visualization events to the global `EventBus`. It exposes strongly‑typed publish methods such as `PublishAudioCaptureStarted`, `PublishWaveformGenerated`, and `PublishVisualizationError`, along with a generic `Subscribe<T>` method for consuming those events. The `Reset` method clears all internal state and subscriptions, allowing a clean slate for new event streams.

### Usage Example

```csharp
using Events;

var audioStarted = EventPublisher.Subscribe<string>(msg => Console.WriteLine($"Audio started: {msg}"));
var waveformGenerated = EventPublisher.Subscribe<string>(msg => Console.WriteLine($"Waveform: {msg}"));

EventPublisher.PublishAudioCaptureStarted();
EventPublisher.PublishWaveformGenerated();

audioStarted.Dispose();
waveformGenerated.Dispose();

EventPublisher.Reset();
```
```