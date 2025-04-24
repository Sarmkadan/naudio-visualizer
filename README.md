// src/README.md

// ... existing content ...

## AudioCaptureService

`AudioCaptureService` is a service responsible for managing audio capture operations, including device selection, initialization, and recording. It provides a simple and asynchronous API for capturing audio data from available devices.

### Usage Example

```csharp
using Domain.Services;

// Create an instance of AudioCaptureService
var audioCaptureService = new AudioCaptureService();

// Get available audio devices
var devices = audioCaptureService.GetAvailableDevices;

// Initialize the service with the selected device
await audioCaptureService.Initialize(devices[0]);

// Start recording audio
await audioCaptureService.StartRecordingAsync();

// Get the current audio metadata
var metadata = audioCaptureService.GetCurrentMetadata;

// Get the buffered audio data
var audioData = audioCaptureService.GetBufferedAudio;

// Get the current audio frame
var frame = audioCaptureService.Frame;

// Check if the service is available
var isAvailable = audioCaptureService.IsAvailable;

// Stop recording audio
await audioCaptureService.StopRecordingAsync();

// Clear the audio buffer
audioCaptureService.ClearBuffer();

// Dispose of the service
audioCaptureService.Dispose();
```

## SpectrumAnalyzer

`SpectrumAnalyzer` is a service for performing FFT-based frequency spectrum analysis on audio data. It converts raw audio frames into frequency domain representations, enabling visualization of audio characteristics such as spectral content, dominant frequencies, and energy distribution across frequency bands. The analyzer supports peak-hold functionality for transient detection and provides various post-processing operations like logarithmic scaling, smoothing, and band energy extraction.


### Usage Example

```csharp
using NAudioVisualizer.Services;
using NAudioVisualizer.Domain.Models;

// Create analyzer with custom peak decay rate
var analyzer = new SpectrumAnalyzer { PeakHoldDecayDbPerSecond = 15f };

// Analyze audio frame (2048-sample FFT window)
var frame = new AudioFrame(sampleRate: 44100, samples: audioSamples);
SpectrumData spectrum = analyzer.AnalyzeSpectrum(frame, fftSize: 2048);

// Convert to logarithmic dB scale for visualization
analyzer.ConvertToLogScale(spectrum);

// Apply smoothing to reduce visual noise
analyzer.SmoothSpectrum(spectrum, windowSize: 5);

// Update peak holds for transient detection
analyzer.UpdatePeakHolds(spectrum, elapsedSeconds: 1.0 / 60);

// Get peak hold values
float[]? peaks = analyzer.GetPeakHolds();

// Extract frequency band energies
var bands = analyzer.ExtractFrequencyBands(spectrum);
Console.WriteLine($"Bass: {bands.BassEnergy:P0}, Mid: {bands.MidEnergy:P0}, Treble: {bands.TrebleEnergy:P0}");

// Find dominant frequency
float dominantFreq = analyzer.FindDominantFrequency(spectrum);
Console.WriteLine($"Dominant frequency: {dominantFreq:F1} Hz");

// Calculate spectral centroid (brightness measure)
float centroid = analyzer.CalculateSpectralCentroid(spectrum);
Console.WriteLine($"Spectral centroid: {centroid:F1} Hz");
```

// ... existing content ...
