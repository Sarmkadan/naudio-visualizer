// src/README.md
// ... rest of the file content ...
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