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

## AudioBuffer

`AudioBuffer` is a circular buffer that stores interleaved audio samples for real‑time processing. It supports writing, reading, peeking, and querying buffer state, making it suitable for audio streaming and analysis.

### Usage Example

```csharp
using Domain.Models;

// Create an AudioBuffer with capacity 1024 samples, sample rate 44100 Hz, 2 channels
var buffer = new AudioBuffer(1024, 44100, 2);

// Write some samples (e.g., 256 stereo samples)
float[] samplesToWrite = new float[256 * 2]; // interleaved left/right
buffer.Write(samplesToWrite);

// Peek at the next 128 samples without removing them
float[] peeked = buffer.Peek(128 * 2);

// Read 128 samples, removing them from the buffer
float[] readSamples = buffer.Read(128 * 2);

// Get all remaining samples
float[] allSamples = buffer.GetAll();

// Check buffer state
bool isFull = buffer.IsFull;
bool isEmpty = buffer.IsEmpty;
int available = buffer.AvailableSpace;

// Get statistics
AudioBufferStats stats = buffer.GetStats();

// Inspect properties
int count = buffer.CurrentCount;
int capacity = buffer.Capacity;
float fillPct = buffer.FillPercentage;
double durationSec = buffer.DurationSeconds;

// Clear the buffer
buffer.Clear();
```

// ... existing content ...
