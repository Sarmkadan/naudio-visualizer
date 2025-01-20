# API Reference

Complete API documentation for NAudio Visualizer.

## Table of Contents
- [Core Services](#core-services)
- [Domain Models](#domain-models)
- [Data Repositories](#data-repositories)
- [Utility Classes](#utility-classes)
- [Events](#events)
- [Exceptions](#exceptions)
- [Configuration](#configuration)

## Core Services

### AudioCaptureService

Manages real-time audio capture from devices.

#### Constructor
```csharp
public AudioCaptureService()
```

#### Properties
```csharp
public bool IsRecording { get; }
public int CurrentDeviceIndex { get; }
public int SampleRate { get; private set; }
public int ChannelCount { get; private set; }
```

#### Methods

##### Initialize
```csharp
public void Initialize(
    int deviceIndex = 0,
    int sampleRate = 44100,
    int channelCount = 2
)
```
Initializes audio capture with specified device and parameters.

**Parameters:**
- `deviceIndex`: Audio device index (0 = default)
- `sampleRate`: Sample rate in Hz (44100, 48000, 96000, 192000)
- `channelCount`: Number of channels (1 = mono, 2 = stereo)

**Throws:** `AudioDeviceException` if device not available

##### StartRecordingAsync
```csharp
public Task StartRecordingAsync()
```
Begins capturing audio frames asynchronously.

**Returns:** Task that completes when recording started

##### StopRecordingAsync
```csharp
public Task StopRecordingAsync()
```
Stops audio capture gracefully.

**Returns:** Task that completes when recording stopped

##### GetAvailableDevices
```csharp
public AudioDevice[] GetAvailableDevices()
```
Returns list of available audio input devices.

**Returns:** Array of `AudioDevice` objects

**Example:**
```csharp
var devices = audioService.GetAvailableDevices();
foreach (var device in devices)
{
    Console.WriteLine($"{device.Name} - Channels: {device.ChannelCount}");
}
```

#### Events

##### FrameCaptured
```csharp
public event EventHandler<AudioFrameEventArgs> FrameCaptured
```
Fired when new audio frame is captured.

**Event Args:**
- `Frame`: AudioFrame containing samples
- `Timestamp`: Capture timestamp
- `FrameNumber`: Sequential frame index

---

### WaveformService

Generates waveform visualization data from audio frames.

#### Methods

##### GenerateWaveform
```csharp
public WaveformData GenerateWaveform(AudioFrame frame)
```
Creates waveform data from audio frame.

**Parameters:**
- `frame`: AudioFrame to process

**Returns:** WaveformData with per-channel amplitude

**Example:**
```csharp
var waveform = waveformService.GenerateWaveform(audioFrame);
Console.WriteLine($"Peak: {waveform.PeakAmplitude}");
```

##### DownsampleWaveform
```csharp
public WaveformData DownsampleWaveform(
    WaveformData waveform,
    int targetSamples
)
```
Reduces waveform data points while preserving peaks.

**Parameters:**
- `waveform`: Waveform to downsample
- `targetSamples`: Desired number of samples

**Returns:** Downsampled WaveformData

**Use Case:** Rendering on display with limited pixels

##### SmoothWaveform
```csharp
public WaveformData SmoothWaveform(
    WaveformData waveform,
    float smoothingFactor = 0.85f
)
```
Applies exponential moving average smoothing.

**Parameters:**
- `waveform`: Waveform to smooth
- `smoothingFactor`: Alpha value (0-1, higher = more smoothing)

**Returns:** Smoothed WaveformData

##### CalculatePeaks
```csharp
public float[] CalculatePeaks(
    WaveformData waveform,
    int peakCount
)
```
Extracts peak amplitudes from waveform.

**Parameters:**
- `waveform`: Waveform data
- `peakCount`: Number of peaks to extract

**Returns:** Array of peak amplitudes

---

### SpectrumAnalyzer

Performs FFT-based frequency analysis.

#### Methods

##### AnalyzeSpectrum
```csharp
public SpectrumData AnalyzeSpectrum(
    AudioFrame frame,
    int fftSize = 2048
)
```
Computes frequency spectrum from audio frame.

**Parameters:**
- `frame`: AudioFrame to analyze
- `fftSize`: FFT size (256-16384, must be power of 2)

**Returns:** SpectrumData with magnitude and frequency information

**Performance:**
- 2048 FFT: ~2-5ms on modern CPU
- 4096 FFT: ~4-10ms
- Higher resolution requires more computation

**Example:**
```csharp
var spectrum = analyzer.AnalyzeSpectrum(frame, 2048);
Console.WriteLine($"Peak Frequency: {spectrum.PeakFrequency} Hz");
Console.WriteLine($"Bins: {spectrum.FrequencyBins.Length}");
```

##### ConvertToLogScale
```csharp
public void ConvertToLogScale(SpectrumData spectrum)
```
Applies logarithmic frequency scaling.

**Modifies:** Magnitude values in-place

**Use Case:** Human hearing follows log frequency scale (musical notes)

##### ApplyWindow
```csharp
public void ApplyWindow(
    SpectrumData spectrum,
    WindowFunction window = WindowFunction.Hann
)
```
Applies window function to reduce spectral leakage.

**Parameters:**
- `spectrum`: Spectrum data
- `window`: Window type (Hann, Hamming, Blackman, Rectangle)

**Window Properties:**
- **Hann**: Best general-purpose (default)
- **Hamming**: Similar to Hann, slightly different sidelobe
- **Blackman**: Lower sidelobe, wider main lobe
- **Rectangle**: No windowing, spectral leakage

##### ExtractFrequencyBands
```csharp
public float[] ExtractFrequencyBands(
    SpectrumData spectrum,
    int bandCount = 3
)
```
Extracts standard frequency bands (bass, mid, treble).

**Parameters:**
- `spectrum`: Spectrum data
- `bandCount`: Number of bands to extract

**Returns:** Array of band magnitudes (dB)

**Band Distribution (3 bands):**
- [0]: Bass (20-250 Hz)
- [1]: Midrange (250-2000 Hz)
- [2]: Treble (2000-20000 Hz)

**Example:**
```csharp
var bands = analyzer.ExtractFrequencyBands(spectrum, 3);
Console.WriteLine($"Bass: {bands[0]} dB");
```

---

### SpectrogramAnalyzer

Builds time-frequency representation.

#### Methods

##### BuildSpectrogram
```csharp
public SpectrogramData BuildSpectrogram(
    AudioFrame[] frames,
    int fftSize = 2048,
    int hopSize = 512
)
```
Constructs spectrogram from frame sequence.

**Parameters:**
- `frames`: Array of AudioFrames
- `fftSize`: FFT size for each frame
- `hopSize`: Number of samples to advance between FFT windows

**Returns:** SpectrogramData with 2D magnitude array

**Typical Settings:**
- `fftSize=2048, hopSize=512`: Good time/frequency resolution
- `fftSize=4096, hopSize=1024`: Better frequency resolution
- `fftSize=1024, hopSize=256`: Better time resolution

**Example:**
```csharp
var spectrogram = analyzer.BuildSpectrogram(
    frames,
    fftSize: 2048,
    hopSize: 512
);
```

##### ApplyLogScale
```csharp
public void ApplyLogScale(SpectrogramData spectrogram)
```
Applies logarithmic magnitude scaling to spectrogram.

**Modifies:** Magnitude values in-place

##### Normalize
```csharp
public void Normalize(SpectrogramData spectrogram)
```
Normalizes magnitude values to [0, 1] range.

**Modifies:** Values scaled to 0-1

##### DetectSpectralFlux
```csharp
public float[] DetectSpectralFlux(
    SpectrogramData spectrogram
)
```
Detects transient events using spectral flux.

**Returns:** Array of flux values per time frame

**Use Case:** Onset detection for beat tracking

---

## Domain Models

### AudioFrame

Represents captured audio data.

```csharp
public class AudioFrame
{
    public Guid SessionId { get; }
    public long FrameNumber { get; }
    public DateTime Timestamp { get; }
    public float[][] Samples { get; }  // [channel][sample]
    public int SampleCount { get; }
    public int SampleRate { get; }
    public int ChannelCount { get; }
}
```

### VisualizationData (Base Class)

Base class for all visualization types.

```csharp
public abstract class VisualizationData
{
    public Guid SessionId { get; protected set; }
    public long FrameIndex { get; protected set; }
    public DateTime TimeStamp { get; protected set; }
    public bool IsProcessed { get; protected set; }
}
```

### WaveformData

```csharp
public class WaveformData : VisualizationData
{
    public float[] LeftChannel { get; }
    public float[] RightChannel { get; }
    public float PeakAmplitude { get; }
    public float RmsAmplitude { get; }
    public int DownsampleRatio { get; }
}
```

### SpectrumData

```csharp
public class SpectrumData : VisualizationData
{
    public float[] MagnitudeSpectrum { get; }
    public float[] PhaseSpectrum { get; }
    public float[] FrequencyBins { get; }
    public float PeakFrequency { get; }
    public float PeakMagnitude { get; }
}
```

### SpectrogramData

```csharp
public class SpectrogramData : VisualizationData
{
    public float[,] TimeWindows { get; }  // [time, freq]
    public float FrequencyRangeMin { get; }
    public float FrequencyRangeMax { get; }
    public ColormapType ColorMapping { get; }
}
```

---

## Data Repositories

### VisualizationDataRepository

In-memory storage for visualization data.

```csharp
public class VisualizationDataRepository
{
    // Add visualization data
    public void Add(VisualizationData data);
    
    // Query by time range
    public List<T> Query<T>(
        DateTime startTime,
        DateTime endTime
    ) where T : VisualizationData;
    
    // Get most recent
    public T GetLatest<T>() where T : VisualizationData;
    
    // Clear all data
    public void Clear();
    
    // Enable automatic pruning
    public void EnableAutoPruning(
        int retentionTimeSeconds = 300,
        int checkIntervalSeconds = 10
    );
}
```

### AudioSessionRepository

Session and frame storage.

```csharp
public class AudioSessionRepository
{
    // Create new session
    public AudioSession StartSession(string name);
    
    // Add frame to session
    public Task AddFrameAsync(
        string sessionId,
        AudioFrame frame
    );
    
    // Get session info
    public AudioSession GetSession(string sessionId);
    
    // Get frames in session
    public List<AudioFrame> GetSessionFrames(string sessionId);
    
    // Get frames by time range
    public List<AudioFrame> GetFramesByTimeRange(
        string sessionId,
        DateTime startTime,
        DateTime endTime
    );
}
```

---

## Utility Classes

### CacheManager

Memory caching with TTL support.

```csharp
public class CacheManager : IDisposable
{
    // Add to cache
    public void Set<T>(
        string key,
        T value,
        TimeSpan? expiration = null
    );
    
    // Try to retrieve
    public bool TryGetValue<T>(
        string key,
        out T value
    );
    
    // Remove specific item
    public void Remove(string key);
    
    // Clear all cache
    public void Clear();
    
    // Get statistics
    public CacheStatistics GetStatistics();
}
```

### PerformanceProfiler

Performance measurement utility.

```csharp
public class PerformanceProfiler
{
    // Start measurement
    public void StartMeasure(string operationName);
    
    // End measurement
    public TimeSpan StopMeasure(string operationName);
    
    // Get statistics for operation
    public PerformanceStats GetStats(string operationName);
}
```

### Logger

Logging utility with multiple sinks.

```csharp
public class Logger
{
    public Logger(string filePath, bool writeToConsole = true);
    
    public LogLevel MinimumLevel { get; set; }
    
    public void Debug(string message);
    public void Info(string message);
    public void Warning(string message);
    public void Error(string message);
}
```

---

## Events

### AudioVisualizationEvents

```csharp
public class FrameCapturedEventArgs : EventArgs
{
    public AudioFrame Frame { get; }
    public DateTime Timestamp { get; }
    public int FrameNumber { get; }
}

public class VisualizationUpdatedEventArgs : EventArgs
{
    public VisualizationData Data { get; }
    public VisualizationType Type { get; }
}
```

---

## Exceptions

### AudioDeviceException
```csharp
public class AudioDeviceException : Exception
{
    // Thrown when device initialization fails
}
```

### AudioStreamException
```csharp
public class AudioStreamException : Exception
{
    // Thrown for audio capture/stream errors
}
```

### VisualizationException
```csharp
public class VisualizationException : Exception
{
    // Thrown when visualization generation fails
}
```

---

## Configuration

### ApplicationSettings

```csharp
public class ApplicationSettings
{
    // Audio
    public int DefaultSampleRate { get; set; }
    public int DefaultFftSize { get; set; }
    public int MaxFramesPerSession { get; set; }
    
    // Rendering
    public int TargetFps { get; set; }
    public int MaxVisualizationFrames { get; set; }
    
    // Performance
    public int CacheMaxSize { get; set; }
    public int BufferSize { get; set; }
    
    // Visualization
    public WaveformRenderingSettings WaveformSettings { get; set; }
    public SpectrumRenderingSettings SpectrumSettings { get; set; }
    public SpectrogramRenderingSettings SpectrogramSettings { get; set; }
    
    // Validation
    public bool IsValid();
}
```

---

For architectural overview, see [Architecture Guide](./ARCHITECTURE.md).
