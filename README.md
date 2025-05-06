// src/README.md

// ... existing content ...

## GradientStopExtensions

`GradientStopExtensions` provides a set of extension methods for working with `GradientStop` instances. These methods allow you to modify gradient stops, navigate through a collection of gradient stops, interpolate between stops, and analyze their properties.

### Usage Example

```csharp
using Domain.Models;

// Assume 'stop' is a GradientStop instance
var stop = new GradientStop { Color = Colors.Red, Position = 0.5 };

// Change the color of the gradient stop
var newStop = GradientStopExtensions.WithColor(stop, Colors.Blue);
Console.WriteLine($"New color: {newStop.Color}");

// Adjust the brightness of the gradient stop
var brighterStop = GradientStopExtensions.AdjustBrightness(stop, 0.2);
Console.WriteLine($"Brighter color: {brighterStop.Color}");

// Get the ARGB components of the gradient stop
GradientStopExtensions.GetArgbComponents(stop, out byte a, out byte r, out byte g, out byte b);
Console.WriteLine($"ARGB: {a}, {r}, {g}, {b}");

// Check if the gradient stop is dark or light
bool isDark = GradientStopExtensions.IsDark(stop);
bool isLight = GradientStopExtensions.IsLight(stop);
Console.WriteLine($"Is dark: {isDark}, Is light: {isLight}");

// Interpolate between two gradient stops
var nextStop = GradientStopExtensions.Next(stop);
var interpolatedStop = GradientStopExtensions.Interpolate(stop, nextStop, 0.7);
Console.WriteLine($"Interpolated color: {interpolatedStop.Color}");
```

## VisualizationSettingsExtensions

`VisualizationSettingsExtensions` provides convenient static methods for creating, cloning, and configuring `VisualizationSettings` objects, as well as validating them and retrieving key colors used in the visualizer. It allows you to quickly switch between high‑performance and high‑quality presets and inspect the current color scheme.

### Usage Example

```csharp
using Domain.Models;
using System.Drawing;

// Create a default settings instance
var settings = VisualizationSettingsExtensions.CreateDefault();

// Clone and tweak for high‑performance mode
var perfSettings = VisualizationSettingsExtensions.Clone(settings);
perfSettings = VisualizationSettingsExtensions.WithHighPerformance(perfSettings);

// Validate the settings before use
VisualizationSettingsExtensions.Validate(perfSettings);

// Retrieve colors used by the visualizer
var bg = VisualizationSettingsExtensions.GetBackgroundColor(perfSettings);
var waveform = VisualizationSettingsExtensions.GetWaveformLineColor(perfSettings);
var spectrum = VisualizationSettingsExtensions.GetSpectrumBarColor(perfSettings);

Console.WriteLine($"Background: {bg}");
Console.WriteLine($"Waveform line: {waveform}");
Console.WriteLine($"Spectrum bar: {spectrum}");
```

## MidiInputService

`MidiInputService` captures real-time MIDI input from a connected MIDI device and raises structured `MidiNoteEvent` notifications for downstream visualization. It wraps the NAudio `MidiIn` API and bridges incoming MIDI messages into the application event bus so that visualizers can subscribe without coupling to NAudio directly.

### Usage Example


```csharp
using NAudioVisualizer.Services;
using NAudioVisualizer.Domain.Models;

// Create the MIDI input service
var midiService = new MidiInputService();

// List available MIDI devices
var devices = await midiService.GetAvailableDevicesAsync();
Console.WriteLine($"Found {devices.Count} MIDI devices:");
foreach (var device in devices)
{
    Console.WriteLine($"  {device.Index}: {device.ProductName}");
}

// Start listening to a specific device (e.g., device index 0)
try
{
    await midiService.StartAsync(0);
    Console.WriteLine("MIDI input started. Listening for note events...");
    
    // Subscribe to note events
    midiService.NoteReceived += (sender, args) =>
    {
        var note = args.Note;
        Console.WriteLine($"MIDI Note: {note.NoteName} (Channel: {note.Channel}, " +
                        $"Velocity: {note.Velocity}, Frequency: {note.Frequency:F2}Hz)");
    };
    
    // Keep listening for a while...
    await Task.Delay(30000); // Listen for 30 seconds
}
finally
{
    // Stop listening and clean up
    await midiService.StopAsync();
    midiService.Dispose();
}
```

// ... existing content ...
