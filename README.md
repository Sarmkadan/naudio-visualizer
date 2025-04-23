// src/README.md

// ... existing content ...

## SpectrogramData

`SpectrogramData` represents a single frame of audio spectrogram data, including its frequency bins, time frames, sample rate, FFT size, and hop size. It provides access to the raw frequency and time data, as well as various calculated metrics such as time per frame and frequency resolution.

### Usage Example

```csharp
using Domain.Models;

// Create a new SpectrogramData instance
var spectrogramData = new SpectrogramData
{
    FrequencyBins = 1024,
    TimeFrames = 512,
    SampleRate = 44100,
    FftSize = 1024,
    HopSize = 256,
    TimePerFrame = 0.01f,
    FrequencyResolution = 10f,
    ColormapType = ColormapType.Jet
};

// Access the raw frequency and time data
var frequencyData = spectrogramData.GetData();
var timeData = spectrogramData.GetTimeFrame();

// Calculate the time per frame and frequency resolution
var timePerFrame = spectrogramData.TimePerFrame;
var frequencyResolution = spectrogramData.FrequencyResolution;

// Apply log scale to the spectrogram data
spectrogramData.ApplyLogScale();

// Check if the spectrogram data is valid
var isValid = spectrogramData.IsValid();

Console.WriteLine($"Frequency Bins: {spectrogramData.FrequencyBins}");
Console.WriteLine($"Time Frames: {spectrogramData.TimeFrames}");
Console.WriteLine($"Sample Rate: {spectrogramData.SampleRate}");
Console.WriteLine($"FFT Size: {spectrogramData.FftSize}");
Console.WriteLine($"Hop Size: {spectrogramData.HopSize}");
Console.WriteLine($"Time Per Frame: {spectrogramData.TimePerFrame}");
Console.WriteLine($"Frequency Resolution: {spectrogramData.FrequencyResolution}");
Console.WriteLine($"Colormap Type: {spectrogramData.ColormapType}");
Console.WriteLine($"Is Valid: {spectrogramData.IsValid()}");
```

// ... existing content ...
```