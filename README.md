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
    VisualizationType = VisualizationType.Waveform,
    DataPointCount = 1024,
    MinValue = -1.0f,
    MaxValue = 1.0f,
    IsNormalized = false,
    SourceFrame = new AudioFrame { SampleRate = 44100, Channels = 2 }
};

// Generate some sample data
var rawData = new float[1024];
var random = new Random();
for (int i = 0; i < rawData.Length; i++)
{
    rawData[i] = (float)(random.NextDouble() * 2 - 1); // -1 to 1 range
}

// Set the data through the abstract GetData method
var dataArray = waveformData.GetData();
Array.Copy(rawData, dataArray, rawData.Length);

// Normalize the data
waveformData.Normalize();

Console.WriteLine($"Visualization ID: {waveformData.Id}");
Console.WriteLine($"Type: {waveformData.VisualizationType}");
Console.WriteLine($"Generated At: {waveformData.GeneratedAt}");
Console.WriteLine($"Data Points: {waveformData.DataPointCount}");
Console.WriteLine($"Min Value: {waveformData.MinValue}");
Console.WriteLine($"Max Value: {waveformData.MaxValue}");
Console.WriteLine($"Is Normalized: {waveformData.IsNormalized}");
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
```