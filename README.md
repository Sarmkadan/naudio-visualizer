// src/README.md
// ... rest of the file content ...
## MidiNoteEvent
The `MidiNoteEvent` type represents a single MIDI note event, providing information about the note's channel, note number, velocity, and timestamp. It also includes methods to retrieve the note's name and frequency.

### Usage Example

```csharp
using Domain.Models;

// Create a new MidiNoteEvent
var event = new MidiNoteEvent
{
    Channel = 0,
    NoteNumber = 60,
    Velocity = 100,
    IsNoteOn = true,
    Frequency = MidiNoteEvent.GetFrequency(60),
    Timestamp = DateTime.Now,
    DeviceIndex = 0
};

// Inspect the event
Console.WriteLine($"Note: {event.NoteName}, Channel: {event.Channel}, Note Number: {event.NoteNumber}, Velocity: {event.Velocity}");
Console.WriteLine($"Frequency: {event.Frequency}, Timestamp: {event.Timestamp}, Device Index: {event.DeviceIndex}");

// Check if the event is valid
Console.WriteLine($"Is valid: {event.IsValid}");
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

## GradientStop
The `GradientStop` type represents a single stop in a color gradient used by visualizers. It exposes the stop's position, color, optional name, and background color, and is used by `VisualizerTheme` to build waveform and spectrogram palettes. A typical usage might look like this:

```csharp
using Domain.Models;

// Create a gradient stop
var stop = new GradientStop
{
    Position = 0.25f,
    Color = 0xFFFF0000,          // Red
    Name = "MidRed",
    BackgroundColor = 0xFF000000 // Black
};

// Inspect the stop
Console.WriteLine($"Stop {stop.Name} at {stop.Position} with color 0x{stop.Color:X8}");

// Use it with a theme
var theme = VisualizerTheme.Classic;
foreach (var gs in theme.WaveformGradient)
{
    Console.WriteLine($"{gs.Name} at {gs.Position}");
}
```

This example demonstrates creating a `GradientStop`, accessing its properties, and iterating over a theme's waveform gradient.
