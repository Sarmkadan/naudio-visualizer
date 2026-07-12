// src/README.md

// ... existing content ...

## WaveformService

`WaveformService` generates and processes waveform data from audio samples. It provides methods for downsampling, normalizing, and smoothing the waveform, as well as calculating peak values, frame energy, and zero crossings.

### Usage Example

```csharp
using Services;

// Create a waveform service instance
var waveformService = new WaveformService();

// Generate waveform data from samples
var waveform = waveformService.GenerateWaveform(samples);

// Downsample the waveform to reduce its resolution
var downsampledWaveform = waveformService.DownsampleSamples(waveform);

// Normalize the waveform to a specific range
waveformService.NormalizeWaveform(downsampledWaveform);

// Calculate peak values in the waveform
var peakValues = waveformService.CalculatePeakValues(downsampledWaveform);

// Apply a smoothing filter to the waveform
var smoothedWaveform = waveformService.ApplySmoothingFilter(downsampledWaveform);

// Calculate frame energy in the waveform
var frameEnergy = waveformService.CalculateFrameEnergy(smoothedWaveform);

// Count zero crossings in the waveform
var zeroCrossings = waveformService.CountZeroCrossings(smoothedWaveform);

Console.WriteLine($"Peak values: {string.Join(", ", peakValues)}");
Console.WriteLine($"Frame energy: {frameEnergy}");
Console.WriteLine($"Zero crossings: {zeroCrossings}");
```

// ... existing content ...
