// src/README.md

## VisualizationData

`VisualizationData` is the base class for all audio visualization types (waveform, spectrum, and spectrogram). It provides core properties and methods for storing, normalizing, and validating audio visualization data, including metadata like timestamps, source audio frames, and value ranges.

### Usage Example

```csharp
using Domain.Models;
using System;

// Create a concrete visualization data instance (e.g., WaveformData)
var waveformData = new WaveformData
{
    SampleRate = 44100,
    ChannelCount = 2,
    DownsamplingFactor = 4
};

// Generate some sample data
var leftChannelPeaks = new float[] { 0.5f, 0.2f, 0.8f };
var rightChannelPeaks = new float[] { 0.3f, 0.9f, 0.1f };

// Set the data through the SetSamples method
waveformData.SetSamples(leftChannelPeaks, rightChannelPeaks);

// Normalize the data
waveformData.Normalize();

// Downsample the data
waveformData.Downsample();

Console.WriteLine($"Sample Rate: {waveformData.SampleRate}");
Console.WriteLine($"Channel Count: {waveformData.ChannelCount}");
Console.WriteLine($"Downsampling Factor: {waveformData.DownsamplingFactor}");
Console.WriteLine($"Left Channel Peaks: [{string.Join(", ", leftChannelPeaks)}]");
Console.WriteLine($"Right Channel Peaks: [{string.Join(", ", rightChannelPeaks)}]");
Console.WriteLine($"Is Valid: {waveformData.IsValid()}");
```

## VisualizationSettings
`VisualizationSettings` encapsulates the configuration for visualizing audio data. It includes settings for waveform, spectrum, and spectrogram rendering, as well as display options such as grid, labels, and anti-aliasing.

### Usage Example

```csharp
using Domain.Models;

// Create a new VisualizationSettings instance
var settings = new VisualizationSettings
{
    Theme = VisualizerTheme.Classic,
    WaveformSettings = new WaveformRenderingSettings
    {
        LineColor = 0xFF0000FF, // Blue
        LineThickness = 2f
    },
    SpectrumSettings = new SpectrumRenderingSettings
    {
        MaxFrequencyDisplay = 20000f
    },
    SpectrogramSettings = new SpectrogramSettings
    {
        DownsamplingFactor = 4
    },
    RenderingQuality = 2,
    TargetFPS = 60,
    EnableAntiAliasing = true,
    BackgroundColor = 0xFF000000, // Black
    ShowGrid = true,
    ShowFrequencyLabels = true,
    ShowTimeLabels = true,
    TimeScale = 1f,
    MaxFrequencyDisplay = 20000f,
    IsValid = true,
    LineColor = 0xFF0000FF, // Blue
    LineThickness = 2f,
    ShowStereoSeparate = true,
    AmplitudeZoom = 1f
};

// Inspect the settings
Console.WriteLine($"Theme: {settings.Theme}");
Console.WriteLine($"Waveform Settings: {settings.WaveformSettings}");
Console.WriteLine($"Spectrum Settings: {settings.SpectrumSettings}");
Console.WriteLine($"Spectrogram Settings: {settings.SpectrogramSettings}");
Console.WriteLine($"Rendering Quality: {settings.RenderingQuality}");
Console.WriteLine($"Target FPS: {settings.TargetFPS}");
Console.WriteLine($"Enable Anti-Aliasing: {settings.EnableAntiAliasing}");
Console.WriteLine($"Background Color: 0x{settings.BackgroundColor:X8}");
Console.WriteLine($"Show Grid: {settings.ShowGrid}");
Console.WriteLine($"Show Frequency Labels: {settings.ShowFrequencyLabels}");
Console.WriteLine($"Show Time Labels: {settings.ShowTimeLabels}");
Console.WriteLine($"Time Scale: {settings.TimeScale}");
Console.WriteLine($"Max Frequency Display: {settings.MaxFrequencyDisplay}");
Console.WriteLine($"Is Valid: {settings.IsValid}");
Console.WriteLine($"Line Color: 0x{settings.LineColor:X8}");
Console.WriteLine($"Line Thickness: {settings.LineThickness}");
Console.WriteLine($"Show Stereo Separate: {settings.ShowStereoSeparate}");
Console.WriteLine($"Amplitude Zoom: {settings.AmplitudeZoom}");
Console.WriteLine($"Downsampling Factor: {settings.SpectrogramSettings.DownsamplingFactor}");
```

## AudioFrame

`AudioFrame` represents a single frame of audio data, including its timestamp, sample rate, and channel count. It also provides access to the raw audio samples and various calculated metrics such as peak amplitude and RMS energy.

### Usage Example

```csharp
using Domain.Models;

// Create a new AudioFrame instance
var frame = new AudioFrame
{
    SampleRate = 44100,
    Channels = 2,
    Timestamp = DateTime.Now,
    FrameIndex = 0,
    DurationSeconds = 0.1f,
    PeakAmplitude = 0.5f,
    RmsEnergy = 0.2f
};

// Access the raw audio samples
var samples = frame.Samples;

// Calculate the duration of the frame in seconds
var duration = frame.DurationSeconds;

// Check if the frame is valid
var isValid = frame.IsValid;

Console.WriteLine($"Sample Rate: {frame.SampleRate}");
Console.WriteLine($"Channels: {frame.Channels}");
Console.WriteLine($"Timestamp: {frame.Timestamp}");
Console.WriteLine($"Frame Index: {frame.FrameIndex}");
Console.WriteLine($"Duration (s): {frame.DurationSeconds}");
Console.WriteLine($"Peak Amplitude: {frame.PeakAmplitude}");
Console.WriteLine($"RMS Energy: {frame.RmsEnergy}");
Console.WriteLine($"Is Valid: {frame.IsValid}");
```

## VstPluginInfo

`VstPluginInfo` represents metadata about a loaded VST plugin, including its unique identifier, SDK version, channel capabilities, and parameter information. It provides runtime details about plugin validation, loading time, and parameter states for automation and parameter management.

### Usage Example

```csharp
using Domain.Models;
using System;

// Create a VST plugin info instance
var pluginInfo = new VstPluginInfo
{
    LoadedAt = DateTime.Now,
    UniqueId = "ABCD1234EFGH5678",
    SdkVersion = "VST3.7.8",
    MaxChannels = 2,
    IsValid = true
};

// Add parameters to the plugin
pluginInfo.Parameters.Add(new VstParameter
{
    Id = 0,
    Name = "Volume",
    Label = "dB",
    Units = "Decibels",
    MinValue = -60f,
    MaxValue = 12f,
    DefaultValue = 0f,
    CurrentValue = 0f,
    IsAutomated = false,
    Category = "Main",
    IsReadOnly = false,
    IsValid = true
});

pluginInfo.Parameters.Add(new VstParameter
{
    Id = 1,
    Name = "Reverb",
    Label = "%",
    Units = "Percentage",
    MinValue = 0f,
    MaxValue = 100f,
    DefaultValue = 30f,
    CurrentValue = 45.5f,
    IsAutomated = true,
    Category = "Effects",
    IsReadOnly = false,
    IsValid = true
});

// Display plugin information
Console.WriteLine($"Plugin Loaded At: {pluginInfo.LoadedAt}");
Console.WriteLine($"Plugin Unique ID: {pluginInfo.UniqueId}");
Console.WriteLine($"SDK Version: {pluginInfo.SdkVersion}");
Console.WriteLine($"Max Channels: {pluginInfo.MaxChannels}");
Console.WriteLine($"Is Valid: {pluginInfo.IsValid}");
Console.WriteLine($"ToString: {pluginInfo}");

// Display parameter information
foreach (var param in pluginInfo.Parameters)
{
    Console.WriteLine($"Parameter {param.Id}: {param.Name} = {param.CurrentValue} {param.Label}");
    Console.WriteLine($"  Range: {param.MinValue} to {param.MaxValue} {param.Units}");
    Console.WriteLine($"  Category: {param.Category}, Automated: {param.IsAutomated}");
}
```
```