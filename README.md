# NAudio Visualizer

Real-time audio visualization with NAudio and SkiaSharp - waveform, spectrum, spectrogram visualization system for .NET.

## Features

- **Real-time Audio Capture**: Capture audio from any input device with minimal latency
- **Waveform Visualization**: Display audio waveforms with stereo channel separation
- **Spectrum Analysis**: FFT-based frequency spectrum visualization with logarithmic scaling
- **Spectrogram Display**: Time-frequency representation with multiple color mappings
- **Device Management**: Support for multiple audio devices with automatic detection
- **Audio Buffering**: Efficient circular buffer for audio data management
- **Performance Optimized**: Configurable quality levels and downsampling for smooth rendering
- **Extensible Architecture**: Clean separation of concerns with services, repositories, and domain models

## Architecture

### Domain Models
- **AudioFrame**: Raw audio data with timing and sample information
- **VisualizationData**: Base class for all visualization types
- **WaveformData**: Waveform visualization with stereo separation
- **SpectrumData**: Frequency spectrum with magnitude and frequency bins
- **SpectrogramData**: 2D time-frequency representation
- **AudioDevice**: Audio input device representation with capabilities
- **AudioBuffer**: Thread-safe circular buffer for audio streaming
- **AudioMetadata**: Metadata about audio capture session

### Services
- **AudioCaptureService**: Handles audio device capture and frame delivery
- **WaveformService**: Generates waveform data with optional downsampling and smoothing
- **SpectrumAnalyzer**: FFT-based frequency analysis with window functions
- **SpectrogramAnalyzer**: Time-frequency analysis with spectral flux detection

### Data Access
- **VisualizationDataRepository**: In-memory storage and querying of visualization data
- **AudioSessionRepository**: Session and frame storage with efficient memory management

### Infrastructure
- **ServiceContainer**: Lightweight dependency injection container
- **Logger**: File and console logging utility
- **AudioDataConverter**: Audio format conversion and utilities

## System Requirements

- .NET 10.0 or later
- Windows 7 or later
- Audio input device (microphone, line-in, etc.)

## Dependencies

- **NAudio** 2.2.1 - Audio I/O and processing
- **SkiaSharp** 2.88.8 - 2D graphics rendering
- **SkiaSharp.Views.Desktop.WinForms** - WinForms integration

## Getting Started

### Build

```bash
cd naudio-visualizer
dotnet build
```

### Run

```bash
dotnet run
```

## Configuration

### Application Settings

Configuration is managed through the `ApplicationSettings` class:

```csharp
var settings = new ApplicationSettings
{
    DefaultSampleRate = 44100,
    DefaultFftSize = 2048,
    TargetFps = 60,
    MaxFramesPerSession = 5000
};
```

### Visualization Settings

Customize rendering with `VisualizationSettings`:

- **WaveformRenderingSettings**: Line color, thickness, stereo mode, amplitude zoom
- **SpectrumRenderingSettings**: Frequency scale, magnitude scale, smoothing, bar properties
- **SpectrogramRenderingSettings**: Colormap, time window, brightness, contrast

## Usage Examples

### Capture Audio and Generate Waveform

```csharp
var audioCaptureService = new AudioCaptureService();
audioCaptureService.Initialize(deviceIndex: 0, sampleRate: 44100, channelCount: 2);

audioCaptureService.FrameCaptured += (sender, args) =>
{
    var waveformService = new WaveformService();
    var waveform = waveformService.GenerateWaveform(args.Frame);
    // Render waveform...
};

await audioCaptureService.StartRecordingAsync();
```

### Analyze Frequency Spectrum

```csharp
var spectrumAnalyzer = new SpectrumAnalyzer();
var audioFrame = /* ... */;

var spectrum = spectrumAnalyzer.AnalyzeSpectrum(audioFrame, fftSize: 2048);
spectrumAnalyzer.ConvertToLogScale(spectrum);

var dominantFrequency = spectrum.PeakFrequency;
var bands = spectrumAnalyzer.ExtractFrequencyBands(spectrum);
```

### Build Spectrogram

```csharp
var spectrogramAnalyzer = new SpectrogramAnalyzer();

var frames = new AudioFrame[] { /* ... */ };
var spectrogram = spectrogramAnalyzer.BuildSpectrogram(frames, fftSize: 2048, hopSize: 512);

spectrogram.ApplyLogScale();
spectrogram.Normalize();
```

## Audio Constants

Common audio constants are defined in `AudioConstants`:

- Sample Rates: 44100, 48000, 96000, 192000 Hz
- FFT Sizes: 256 - 16384
- Frequency Range: 20 Hz - 20000 Hz
- Buffer Duration: 30 seconds

## Thread Safety

All critical components are thread-safe:

- `AudioBuffer`: Uses locks for concurrent read/write
- `AudioSessionRepository`: Thread-safe frame storage
- `VisualizationDataRepository`: Thread-safe visualization storage
- `AudioCaptureService`: Handles multi-threaded audio delivery

## Performance Considerations

- **Audio Buffering**: Circular buffer prevents memory allocation on each frame
- **Downsampling**: Waveform downsampling reduces rendering complexity
- **Spectral Smoothing**: Temporal and frequency smoothing for visual clarity
- **Memory Pruning**: Automatic pruning of old visualization data
- **Lazy Initialization**: Service container creates services on-demand

## Error Handling

Custom exceptions for domain-specific errors:

- `AudioDeviceException`: Device initialization or access failures
- `AudioStreamException`: Audio capture or stream errors
- `VisualizationException`: Visualization generation failures

## Logging

Enable logging for diagnostics:

```csharp
var logger = new Logger("logs/app.log", writeToConsole: true);
logger.MinimumLevel = LogLevel.Debug;
logger.Info("Application started");
```

## Project Structure

```
naudio-visualizer/
├── src/
│   ├── Domain/
│   │   └── Models/           # Domain entities
│   ├── Services/             # Business logic
│   ├── Data/
│   │   └── Repositories/     # Data access
│   ├── Exceptions/           # Custom exceptions
│   ├── Configuration/        # DI and settings
│   ├── Constants/            # Constants and enums
│   ├── Infrastructure/       # Utilities and helpers
│   └── Program.cs            # Entry point
├── LICENSE                   # MIT License
├── .gitignore
├── naudio-visualizer.csproj  # Project file
└── README.md                 # This file
```

## License

MIT License - Copyright (c) 2026 Vladyslav Zaiets

See LICENSE file for full text.

## Author

Vladyslav Zaiets  
CTO & Software Architect  
https://sarmkadan.com

---

**Real-time audio visualization for .NET** - Built with precision and performance in mind.
