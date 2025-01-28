[![Build](https://github.com/sarmkadan/naudio-visualizer/actions/workflows/build.yml/badge.svg)](https://github.com/sarmkadan/naudio-visualizer/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)

# NAudio Visualizer

Real-time audio visualization for .NET applications with NAudio and SkiaSharp. Capture audio from any device and render stunning waveform, spectrum, and spectrogram visualizations with high performance and minimal latency.

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [System Requirements](#system-requirements)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Usage Examples](#usage-examples)
- [API Reference](#api-reference)
- [Configuration Reference](#configuration-reference)
- [Troubleshooting](#troubleshooting)
- [Performance Optimization](#performance-optimization)
- [Contributing](#contributing)

## Features

### Core Visualization Capabilities

- **Real-time Waveform Visualization** - Display audio waveforms with stereo channel separation and configurable rendering options
- **Frequency Spectrum Analysis** - FFT-based frequency spectrum with logarithmic scaling and peak detection
- **Spectrogram Display** - Time-frequency representation showing audio evolution over time with multiple color mappings
- **Multi-Device Support** - Seamlessly work with multiple audio input devices with automatic device detection and management

### Performance & Optimization

- **Zero-Allocation Audio Buffering** - Thread-safe circular buffer using object pooling for minimal GC pressure
- **Configurable Quality Levels** - Adaptive downsampling and rendering quality for smooth performance on any hardware
- **Intelligent Caching** - Visualization data caching with LRU eviction policy for faster re-renders
- **Spectral Smoothing** - Temporal and frequency-domain smoothing for visually coherent spectrograms

### Advanced Features

- **Device Management** - Automatic device enumeration with sample rate and channel detection
- **Extensible Architecture** - Clean separation of concerns with services, repositories, and domain models
- **Comprehensive Logging** - File and console logging with configurable verbosity levels
- **Memory Efficient** - Automatic pruning of old visualization data with configurable retention policies
- **Thread-Safe Design** - All components engineered for concurrent access without locks on hot paths

### Export & Integration

- **Multiple Export Formats** - Save visualizations as JSON, CSV, or XML
- **HTTP API** - RESTful API for remote visualization queries
- **Webhook Integration** - Publish visualization events to external services
- **Session Recording** - Capture and replay audio sessions with full metadata

## Architecture

### System Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                         NAudio Visualizer                        │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌──────────────────┐      ┌─────────────────────────────────┐ │
│  │  Audio Input     │      │   Visualization Rendering       │ │
│  ├──────────────────┤      ├─────────────────────────────────┤ │
│  │ - Device Enum   │      │ - Waveform Renderer              │ │
│  │ - Audio Capture │      │ - Spectrum Renderer              │ │
│  │ - Frame Delivery│      │ - Spectrogram Renderer           │ │
│  └────────┬─────────┘      └────────────┬────────────────────┘ │
│           │                              │                       │
│           │        ┌────────────────────┐│                       │
│           └───────▶│  Processing Layer  │◀┘                       │
│                    ├────────────────────┤                         │
│                    │ - FFT Analysis     │                         │
│                    │ - Waveform Gen     │                         │
│                    │ - Spectrogram      │                         │
│                    │ - Frequency Bands  │                         │
│                    └────────┬───────────┘                         │
│                             │                                     │
│           ┌─────────────────┴──────────────────┐                 │
│           │                                    │                 │
│      ┌────▼────┐                         ┌────▼────┐             │
│      │ Caching │                         │Repository│             │
│      │ Service │                         │ Service   │             │
│      └─────────┘                         └──────────┘             │
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              Service Container (DI)                       │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
```

### Domain Models

The project uses a clean domain-driven design with well-defined models:

#### AudioFrame
Core data structure for audio samples:
- **Timestamp**: Precise capture time with microsecond accuracy
- **Samples**: Raw PCM audio data (float32 normalized [-1.0, 1.0])
- **Channels**: Stereo or mono audio data
- **SampleRate**: Capture device sample rate (44.1kHz, 48kHz, 96kHz, 192kHz)

#### VisualizationData (Base)
Abstract base class for all visualization types with:
- **Session ID**: Links data to audio capture session
- **FrameIndex**: Sequential frame counter
- **TimeStamp**: Temporal position in session
- **IsProcessed**: Processing completion flag

#### WaveformData
Waveform visualization representation:
- **ChannelData**: Per-channel amplitude arrays
- **DownsampleRatio**: Data reduction for rendering
- **PeakAmplitudes**: Maximum amplitude per segment
- **RmsValues**: RMS amplitude for volume measurement

#### SpectrumData
FFT-based frequency analysis:
- **MagnitudeSpectrum**: Amplitude for each frequency bin
- **PhaseSpectrum**: Phase information (optional)
- **FrequencyBins**: Bin center frequencies
- **PeakFrequency**: Dominant frequency component

#### SpectrogramData
Time-frequency representation:
- **TimeWindows**: 2D array [time, frequency]
- **FrequencyRange**: Min/max frequency bounds
- **ColorMapping**: Perceptual colormaps (Viridis, Plasma, etc.)
- **SpectralFlux**: Change detection between frames

#### AudioDevice
Audio device representation:
- **DeviceID**: NAudio device identifier
- **DeviceName**: Human-readable name
- **Channels**: Number of audio channels
- **SampleRate**: Supported sample rates
- **Formats**: Supported audio formats

### Service Layer

#### AudioCaptureService
Handles real-time audio capture:
- **Initialize**: Configure device, sample rate, channels
- **StartRecordingAsync**: Begin capturing audio
- **StopRecordingAsync**: Stop capture gracefully
- **FrameCaptured Event**: Fires on each audio frame arrival

#### WaveformService
Generates waveform visualization data:
- **GenerateWaveform**: Create waveform from audio frame
- **DownsampleWaveform**: Reduce data points for rendering
- **SmoothWaveform**: Apply temporal smoothing filter
- **CalculatePeaks**: Find local maxima for visualization

#### SpectrumAnalyzer
Performs FFT-based frequency analysis:
- **AnalyzeSpectrum**: Compute magnitude and phase spectra
- **ConvertToLogScale**: Apply logarithmic frequency scaling
- **ApplyWindow**: Apply Hann/Hamming window function
- **ExtractFrequencyBands**: Extract standard frequency bands (bass, mid, treble, etc.)

#### SpectrogramAnalyzer
Builds time-frequency representation:
- **BuildSpectrogram**: Compute spectrogram from frame sequence
- **ApplyLogScale**: Logarithmic magnitude scaling
- **Normalize**: Normalize magnitude values to [0, 1]
- **DetectSpectralFlux**: Identify transient events

### Data Access Layer

#### VisualizationDataRepository
In-memory storage with query capability:
- **Add**: Store visualization data
- **Query**: Retrieve by session/time range
- **GetLatest**: Get most recent visualization
- **Prune**: Remove old data based on retention policy

#### AudioSessionRepository
Session and frame storage:
- **StartSession**: Create new recording session
- **AddFrame**: Store audio frame
- **GetSession**: Retrieve session by ID
- **GetFrames**: Query frames by time range

### Infrastructure Components

#### ServiceContainer
Lightweight dependency injection:
- **Register**: Register service implementations
- **Resolve**: Get service instance
- **CreateScope**: Create service scope
- **Dispose**: Cleanup resources

#### Logger
Comprehensive logging system:
- **Debug/Info/Warning/Error**: Log at different levels
- **MinimumLevel**: Filter logs by severity
- **WriteToConsole**: Output to console
- **WriteToFile**: Persist to log file

#### AudioDataConverter
Audio format utilities:
- **FloatToShort**: Convert normalized float to PCM int16
- **ShortToFloat**: Convert PCM int16 to normalized float
- **Upsample**: Increase sample rate
- **Downsample**: Decrease sample rate with filtering

## System Requirements

### Windows
- **OS**: Windows 7 SP1 or later
- **.NET Runtime**: .NET 10.0 or later
- **Memory**: Minimum 256MB, recommended 1GB+ for real-time visualization
- **CPU**: Dual-core processor, 1.5GHz or higher

### Linux (via Mono/Wine)
- **OS**: Ubuntu 20.04 or later, Fedora 35+
- **.NET Runtime**: .NET 10.0 or later
- **Audio**: ALSA/PulseAudio support required
- **Display Server**: X11 or Wayland

### macOS (Intel/Apple Silicon)
- **OS**: macOS 10.15 (Catalina) or later
- **.NET Runtime**: .NET 10.0 or later
- **Audio**: CoreAudio support
- **Architecture**: Intel x64 or Apple Silicon native

## Installation

### Method 1: From Source (Recommended)

1. **Prerequisites**
   ```bash
   # Install .NET 10 SDK
   # Windows: Download from https://dotnet.microsoft.com/download
   # macOS: brew install dotnet
   # Linux: Follow https://learn.microsoft.com/en-us/dotnet/core/install/linux
   ```

2. **Clone and Build**
   ```bash
   git clone https://github.com/Sarmkadan/naudio-visualizer.git
   cd naudio-visualizer
   dotnet build -c Release
   ```

3. **Run**
   ```bash
   dotnet run -c Release
   ```

### Method 2: Docker

1. **Build Image**
   ```bash
   docker build -t naudio-visualizer:latest .
   ```

2. **Run Container**
   ```bash
   docker run -it --rm \
     -e DISPLAY=$DISPLAY \
     -v /tmp/.X11-unix:/tmp/.X11-unix \
     naudio-visualizer:latest
   ```

### Method 3: Using Docker Compose

```bash
docker-compose up --build
```

### Method 4: Pre-built Releases

Download pre-built binaries from [Releases](https://github.com/Sarmkadan/naudio-visualizer/releases):

```bash
# Windows
naudio-visualizer-win-x64.zip

# Linux
naudio-visualizer-linux-x64.tar.gz

# macOS
naudio-visualizer-macos-universal.dmg
```

## Quick Start

### Minimal Example

```csharp
using NAudioVisualizer.Configuration;
using NAudioVisualizer.Constants;
using NAudioVisualizer.Services;

// Initialize settings
var settings = new ApplicationSettings
{
    DefaultSampleRate = AudioConstants.DEFAULT_SAMPLE_RATE,
    DefaultFftSize = AudioConstants.DEFAULT_FFT_SIZE,
    TargetFps = 60
};

// Setup DI container
var container = ApplicationConfiguration.ConfigureServices(settings);

// Get audio service
var audioService = container.Resolve<AudioCaptureService>();
audioService.Initialize(deviceIndex: 0);

// Subscribe to frames
audioService.FrameCaptured += (sender, frame) =>
{
    var waveform = container.Resolve<WaveformService>()
        .GenerateWaveform(frame.AudioData);
    Console.WriteLine($"Waveform peak: {waveform.PeakAmplitude}");
};

// Start capturing
await audioService.StartRecordingAsync();
await Task.Delay(TimeSpan.FromSeconds(30));
await audioService.StopRecordingAsync();
```

## Usage Examples

### Example 1: Real-time Waveform Capture

```csharp
var audioService = new AudioCaptureService();
audioService.Initialize(
    deviceIndex: 0,
    sampleRate: 44100,
    channelCount: 2
);

audioService.FrameCaptured += (sender, args) =>
{
    var waveformService = new WaveformService();
    var waveform = waveformService.GenerateWaveform(args.Frame);
    
    Console.WriteLine($"Left Channel RMS: {waveform.LeftChannelRms:F4}");
    Console.WriteLine($"Right Channel RMS: {waveform.RightChannelRms:F4}");
};

await audioService.StartRecordingAsync();
```

### Example 2: Frequency Spectrum Analysis

```csharp
var analyzer = new SpectrumAnalyzer();
var frames = GetAudioFrames(); // Your audio frames

foreach (var frame in frames)
{
    var spectrum = analyzer.AnalyzeSpectrum(frame, fftSize: 2048);
    analyzer.ConvertToLogScale(spectrum);
    
    Console.WriteLine($"Peak Frequency: {spectrum.PeakFrequency} Hz");
    Console.WriteLine($"Peak Magnitude: {spectrum.PeakMagnitude:F4} dB");
    
    var bands = analyzer.ExtractFrequencyBands(spectrum);
    Console.WriteLine($"Bass: {bands[0]:F2} dB");
    Console.WriteLine($"Midrange: {bands[1]:F2} dB");
    Console.WriteLine($"Treble: {bands[2]:F2} dB");
}
```

### Example 3: Building a Spectrogram

```csharp
var spectrogramAnalyzer = new SpectrogramAnalyzer();
var frames = GetAudioFrames();

var spectrogram = spectrogramAnalyzer.BuildSpectrogram(
    frames,
    fftSize: 2048,
    hopSize: 512
);

spectrogram.ApplyLogScale();
spectrogram.Normalize();

// Export spectrogram data
var exporter = new ExportService();
exporter.ExportToJson(spectrogram, "spectrogram.json");
```

### Example 4: Multi-Device Audio Capture

```csharp
var audioService = new AudioCaptureService();
var devices = audioService.GetAvailableDevices();

Console.WriteLine("Available Audio Devices:");
foreach (var device in devices)
{
    Console.WriteLine($"ID: {device.DeviceId}, Name: {device.Name}");
    Console.WriteLine($"  Channels: {device.Channels}");
    Console.WriteLine($"  Sample Rates: {string.Join(", ", device.SampleRates)}");
}

// Initialize with specific device
var selectedDevice = devices.First(d => d.Name.Contains("Microphone"));
audioService.Initialize(selectedDevice.DeviceId);
```

### Example 5: Real-time Visualization with Caching

```csharp
var cacheManager = new CacheManager();
var audioService = new AudioCaptureService();
var waveformService = new WaveformService();

audioService.FrameCaptured += async (sender, args) =>
{
    var cacheKey = $"waveform_{args.Frame.Timestamp:O}";
    
    // Check cache first
    if (!cacheManager.TryGetValue(cacheKey, out var cachedWaveform))
    {
        cachedWaveform = waveformService.GenerateWaveform(args.Frame);
        cacheManager.Set(cacheKey, cachedWaveform, 
            TimeSpan.FromSeconds(30));
    }
    
    RenderWaveform(cachedWaveform);
};
```

### Example 6: HTTP API Integration

```csharp
// Create API service
var apiService = container.Resolve<VisualizationApiService>();

// Query latest spectrum data
var spectrum = await apiService.GetLatestSpectrum();

// Get historical waveform data
var waveforms = await apiService.GetWaveformRange(
    startTime: DateTime.UtcNow.AddMinutes(-5),
    endTime: DateTime.UtcNow
);

// Export to multiple formats
await apiService.ExportVisualization(
    sessionId: "session-123",
    format: ExportFormat.Json
);
```

### Example 7: Custom Audio Frame Processing

```csharp
var audioService = new AudioCaptureService();
var buffer = new AudioBuffer(capacity: 44100 * 2);

audioService.FrameCaptured += (sender, args) =>
{
    // Add to circular buffer
    buffer.Write(args.Frame.Samples, args.Frame.SampleCount);
    
    // Process when buffer has enough data
    if (buffer.Count >= 2048)
    {
        var samples = new float[2048];
        buffer.Read(samples, 0, 2048);
        
        // Apply custom DSP processing
        ApplyNoiseGate(samples, threshold: 0.01f);
        ApplyCompression(samples);
        
        Console.WriteLine($"Processed {samples.Length} samples");
    }
};
```

### Example 8: Session Recording and Replay

```csharp
var sessionService = new AudioSessionService();
var sessionId = await sessionService.StartSession("My Recording");

var audioService = new AudioCaptureService();
audioService.FrameCaptured += async (sender, args) =>
{
    await sessionService.AddFrame(sessionId, args.Frame);
};

// Start capturing
await audioService.StartRecordingAsync();
await Task.Delay(TimeSpan.FromSeconds(60));
await audioService.StopRecordingAsync();

// Replay session
var frames = await sessionService.GetSessionFrames(sessionId);
foreach (var frame in frames)
{
    Console.WriteLine($"Frame at {frame.Timestamp}: {frame.SampleCount} samples");
}
```

### Example 9: Performance Profiling

```csharp
var profiler = new PerformanceProfiler();
var analyzer = new SpectrumAnalyzer();

profiler.StartMeasure("FFT Analysis");

for (int i = 0; i < 1000; i++)
{
    var frame = GetAudioFrame();
    var spectrum = analyzer.AnalyzeSpectrum(frame, fftSize: 2048);
}

var stats = profiler.StopMeasure("FFT Analysis");
Console.WriteLine($"Total: {stats.TotalMilliseconds:F2}ms");
Console.WriteLine($"Average: {stats.AverageMilliseconds:F4}ms");
Console.WriteLine($"Min: {stats.MinMilliseconds:F4}ms");
Console.WriteLine($"Max: {stats.MaxMilliseconds:F4}ms");
```

### Example 10: Advanced Configuration

```csharp
var settings = new ApplicationSettings
{
    DefaultSampleRate = 96000,
    DefaultFftSize = 4096,
    TargetFps = 60,
    MaxFramesPerSession = 10000,
    
    WaveformSettings = new WaveformRenderingSettings
    {
        LineColor = new SkiaSharp.SKColor(0, 255, 0),
        LineThickness = 2.0f,
        StereoMode = true,
        AmplitudeZoom = 1.5f,
        DownsampleRatio = 4
    },
    
    SpectrumSettings = new SpectrumRenderingSettings
    {
        FrequencyScale = FrequencyScale.Logarithmic,
        MagnitudeScale = MagnitudeScale.Decibel,
        BarWidth = 2,
        SmoothingFactor = 0.85f
    },
    
    SpectrogramSettings = new SpectrogramRenderingSettings
    {
        Colormap = ColormapType.Viridis,
        TimeWindowSeconds = 5,
        BrightnessMultiplier = 1.2f,
        ContrastEnhancement = true
    }
};

var container = ApplicationConfiguration.ConfigureServices(settings);
```

## API Reference

### AudioCaptureService

```csharp
public class AudioCaptureService
{
    // Initialize with specific device and settings
    public void Initialize(int deviceIndex, int sampleRate = 44100, 
                          int channelCount = 2);
    
    // Start capturing audio frames
    public Task StartRecordingAsync();
    
    // Stop capturing audio
    public Task StopRecordingAsync();
    
    // Get list of available audio devices
    public AudioDevice[] GetAvailableDevices();
    
    // Event fired when new frame is captured
    public event EventHandler<AudioFrameEventArgs> FrameCaptured;
    
    // Current capture state
    public bool IsRecording { get; }
}
```

### WaveformService

```csharp
public class WaveformService
{
    // Generate waveform from audio frame
    public WaveformData GenerateWaveform(AudioFrame frame);
    
    // Downsample waveform data for rendering
    public WaveformData DownsampleWaveform(WaveformData waveform, 
                                           int targetSamples);
    
    // Apply temporal smoothing
    public WaveformData SmoothWaveform(WaveformData waveform, 
                                       float smoothingFactor = 0.85f);
    
    // Calculate peak amplitudes
    public float[] CalculatePeaks(WaveformData waveform, int peakCount);
}
```

### SpectrumAnalyzer

```csharp
public class SpectrumAnalyzer
{
    // Perform FFT analysis
    public SpectrumData AnalyzeSpectrum(AudioFrame frame, int fftSize = 2048);
    
    // Convert to logarithmic frequency scale
    public void ConvertToLogScale(SpectrumData spectrum);
    
    // Apply window function
    public void ApplyWindow(SpectrumData spectrum, 
                           WindowFunction window = WindowFunction.Hann);
    
    // Extract standard frequency bands
    public float[] ExtractFrequencyBands(SpectrumData spectrum, 
                                         int bandCount = 3);
}
```

### SpectrogramAnalyzer

```csharp
public class SpectrogramAnalyzer
{
    // Build spectrogram from frame sequence
    public SpectrogramData BuildSpectrogram(AudioFrame[] frames, 
                                            int fftSize = 2048, 
                                            int hopSize = 512);
    
    // Apply logarithmic magnitude scaling
    public void ApplyLogScale(SpectrogramData spectrogram);
    
    // Normalize magnitude values
    public void Normalize(SpectrogramData spectrogram);
    
    // Detect spectral flux
    public float[] DetectSpectralFlux(SpectrogramData spectrogram);
}
```

### CacheManager

```csharp
public class CacheManager : IDisposable
{
    // Add item to cache
    public void Set<T>(string key, T value, TimeSpan? expiration = null);
    
    // Try to retrieve cached item
    public bool TryGetValue<T>(string key, out T value);
    
    // Remove specific item
    public void Remove(string key);
    
    // Clear all cache
    public void Clear();
    
    // Get cache statistics
    public CacheStatistics GetStatistics();
}
```

## Configuration Reference

### ApplicationSettings

```csharp
public class ApplicationSettings
{
    // Audio capture settings
    public int DefaultSampleRate { get; set; } = 44100;
    public int DefaultFftSize { get; set; } = 2048;
    public int MaxFramesPerSession { get; set; } = 5000;
    
    // Rendering settings
    public int TargetFps { get; set; } = 60;
    public int MaxVisualizationFrames { get; set; } = 1000;
    
    // Performance settings
    public int CacheMaxSize { get; set; } = 100 * 1024 * 1024; // 100MB
    public int BufferSize { get; set; } = 4096;
    
    // Visualization settings
    public WaveformRenderingSettings WaveformSettings { get; set; }
    public SpectrumRenderingSettings SpectrumSettings { get; set; }
    public SpectrogramRenderingSettings SpectrogramSettings { get; set; }
}
```

### WaveformRenderingSettings

```csharp
public class WaveformRenderingSettings
{
    public SKColor LineColor { get; set; }
    public float LineThickness { get; set; } = 1.0f;
    public bool StereoMode { get; set; } = true;
    public float AmplitudeZoom { get; set; } = 1.0f;
    public int DownsampleRatio { get; set; } = 1;
    public bool ShowGrid { get; set; } = true;
    public float GridOpacity { get; set; } = 0.3f;
}
```

### SpectrumRenderingSettings

```csharp
public class SpectrumRenderingSettings
{
    public FrequencyScale FrequencyScale { get; set; } = FrequencyScale.Logarithmic;
    public MagnitudeScale MagnitudeScale { get; set; } = MagnitudeScale.Decibel;
    public float BarWidth { get; set; } = 2.0f;
    public float SmoothingFactor { get; set; } = 0.85f;
    public float FrequencyMin { get; set; } = 20;
    public float FrequencyMax { get; set; } = 20000;
    public bool PeakHold { get; set; } = true;
}
```

### SpectrogramRenderingSettings

```csharp
public class SpectrogramRenderingSettings
{
    public ColormapType Colormap { get; set; } = ColormapType.Viridis;
    public float TimeWindowSeconds { get; set; } = 5;
    public float BrightnessMultiplier { get; set; } = 1.0f;
    public bool ContrastEnhancement { get; set; } = false;
    public float MagnitudeMin { get; set; } = -80;
    public float MagnitudeMax { get; set; } = 0;
}
```

## Troubleshooting

### No Audio Devices Found

**Problem**: Application reports no audio devices available
- Check audio drivers are installed and enabled
- Run `mmdevapi.dll` diagnostics on Windows
- Verify audio services running (`services.msc`)
- Try different USB audio devices

**Solution**:
```csharp
var devices = audioService.GetAvailableDevices();
if (devices.Length == 0)
{
    // Check system audio
    // Windows: Control Panel > Sound
    // macOS: System Preferences > Sound
    // Linux: pavucontrol or alsamixer
}
```

### Audio Capture Latency

**Problem**: High latency between audio input and visualization
- Increase buffer size in settings
- Reduce FFT size for faster computation
- Lower target FPS if CPU-bound
- Disable spectral smoothing

**Solution**:
```csharp
var settings = new ApplicationSettings
{
    DefaultFftSize = 1024,  // Smaller = faster but less resolution
    TargetFps = 30,          // Lower FPS reduces rendering overhead
    BufferSize = 2048        // Larger buffer = less frequent updates
};
```

### Memory Usage Growing

**Problem**: Memory usage increases over time
- Enable automatic frame pruning
- Reduce `MaxFramesPerSession`
- Clear cache regularly
- Check for memory leaks in custom code

**Solution**:
```csharp
var repository = container.Resolve<VisualizationDataRepository>();
repository.EnableAutoPruning(retentionTimeSeconds: 300);

var cache = container.Resolve<CacheManager>();
cache.Clear(); // Manual cache clearing
```

### Poor Visualization Quality

**Problem**: Visualization appears pixelated or smooth
- Increase `AmplitudeZoom` for waveforms
- Use logarithmic frequency scale for spectrum
- Enable spectral smoothing
- Increase rendering resolution

**Solution**:
```csharp
settings.WaveformSettings.AmplitudeZoom = 2.0f;
settings.SpectrumSettings.SmoothingFactor = 0.9f;
settings.TargetFps = 60;
```

### CPU Usage Too High

**Problem**: CPU usage exceeds 50% at idle
- Reduce FFT size
- Decrease target FPS
- Lower spectrogram update frequency
- Disable real-time processing

**Solution**:
```csharp
settings.DefaultFftSize = 512;
settings.TargetFps = 24;
settings.SpectrogramSettings.TimeWindowSeconds = 10;
```

### Windows Audio Not Starting

**Problem**: Windows Forms application doesn't capture audio
- Ensure audio device initialized before capture starts
- Check for device in use by other application
- Verify COM initialization for NAudio
- Check Windows Firewall/antivirus blocking

**Solution**:
```csharp
try
{
    audioService.Initialize(deviceIndex: 0);
    await audioService.StartRecordingAsync();
}
catch (AudioDeviceException ex)
{
    Console.WriteLine($"Device error: {ex.Message}");
    // Try next device
}
```

## Performance Optimization

### Audio Buffering Strategy

Use circular buffering to avoid allocations:

```csharp
var buffer = new AudioBuffer(capacity: 44100 * 5);
audioService.FrameCaptured += (sender, args) =>
{
    buffer.Write(args.Frame.Samples, args.Frame.SampleCount);
    
    if (buffer.Count >= 4096)
    {
        var samples = new float[4096];
        buffer.Read(samples, 0, 4096);
        // Process samples
    }
};
```

### Selective Downsampling

Reduce data points for rendering:

```csharp
var waveform = waveformService.GenerateWaveform(frame);
var downsampled = waveformService.DownsampleWaveform(waveform, 
    targetSamples: width);
```

### Caching Strategy

Leverage memory caching for repeated queries:

```csharp
var cacheManager = new CacheManager();
cacheManager.Set($"spectrum_{timestamp}", spectrum, 
    TimeSpan.FromSeconds(30));
```

## Contributing

Contributions are welcome! Please follow these guidelines:

1. **Fork** the repository
2. **Create** feature branch: `git checkout -b feature/my-feature`
3. **Implement** your changes with proper error handling
4. **Add** unit tests for new functionality
5. **Update** documentation as needed
6. **Commit** with descriptive messages: `git commit -am 'Add feature X'`
7. **Push** to branch: `git push origin feature/my-feature`
8. **Submit** Pull Request with detailed description

### Code Style

- Use PascalCase for public members, camelCase for private
- Add XML documentation for public APIs
- Include proper error handling and logging
- Write unit tests for business logic
- Follow SOLID principles

### Build & Test

```bash
# Build project
dotnet build

# Run tests
dotnet test

# Create release build
dotnet publish -c Release
```

## Project Structure

```
naudio-visualizer/
├── src/
│   ├── API/                 # HTTP API implementation
│   ├── Caching/            # Caching services
│   ├── CLI/                # Command-line interface
│   ├── Configuration/      # Dependency injection and settings
│   ├── Constants/          # Audio constants and enums
│   ├── Data/               # Data access layer
│   │   └── Repositories/
│   ├── Domain/             # Domain models
│   │   └── Models/
│   ├── Events/             # Event bus and publishers
│   ├── Exceptions/         # Custom exceptions
│   ├── Infrastructure/     # Utilities and helpers
│   ├── Integration/        # External service integration
│   ├── Middleware/         # HTTP middleware
│   ├── Serialization/      # Format serialization
│   ├── Services/           # Business logic services
│   ├── Utilities/          # General utilities
│   ├── Workers/            # Background workers
│   └── Program.cs          # Entry point
├── examples/               # Example applications
├── docs/                   # Detailed documentation
├── docker-compose.yml      # Docker composition
├── Dockerfile              # Container configuration
├── Makefile               # Build automation
├── CHANGELOG.md           # Version history
├── LICENSE                # MIT License
├── README.md              # This file
└── naudio-visualizer.csproj  # Project file
```

## License

MIT License - Copyright (c) 2026 Vladyslav Zaiets

See [LICENSE](LICENSE) file for full text.

---

**Built by [Vladyslav Zaiets](https://sarmkadan.com) - CTO & Software Architect**

[Portfolio](https://sarmkadan.com) | [GitHub](https://github.com/Sarmkadan) | [Telegram](https://t.me/sarmkadan)
