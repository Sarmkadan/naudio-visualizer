![Build](https://github.com/sarmkadan/naudio-visualizer/actions/workflows/build.yml/badge.svg)
![License](https://img.shields.io/github/license/sarmkadan/naudio-visualizer)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)

# naudio-visualizer

Windows Forms application for real-time audio visualization using NAudio and SkiaSharp.

## Installation

```bash
git clone https://github.com/Sarmkadan/naudio-visualizer.git
cd naudio-visualizer
dotnet build -c Release
```

## Quick Start

```csharp
// Initialize audio capture
var capture = new AudioCaptureService();
capture.Initialize(deviceIndex: 0, sampleRate: 44100, channelCount: 2);
await capture.StartRecordingAsync();

// Analyze spectrum
var analyzer = new SpectrumAnalyzer();
var spectrum = analyzer.AnalyzeSpectrum(frame, fftSize: 2048);
analyzer.ConvertToLogScale(spectrum);
```

## Configuration

The application uses `ApplicationSettings` for configuration. You can modify audio capture parameters, visualization themes, and FFT settings in the application configuration file or directly via the UI.

## Build

```bash
dotnet build -c Release
dotnet run -c Release
```

## Usage

On startup the app lists available audio input devices. Select a device from the Audio menu,
then use Audio > Start Capture to begin visualization.

Switch between views from the View menu:
- **Waveform** - time-domain amplitude display
- **Spectrum** - FFT frequency analysis (Hann window, configurable FFT size)
- **Spectrogram** - scrolling time-frequency heatmap

## Usage Examples

The repository includes practical usage examples demonstrating different ways to use the NAudio Visualizer library. These examples serve as both documentation and executable samples you can run to see the library in action.

### Available Examples

| Example | Description | Key Features |
|---------|-------------|--------------|
| **[BasicUsage.cs](examples/BasicUsage.cs)** | Minimal setup and first call to get started quickly | Simple initialization, basic audio capture, event handling |
| **[AdvancedUsage.cs](examples/AdvancedUsage.cs)** | Configuration, custom options, error handling, and spectrum analysis | Custom device settings, error handling, advanced processing |
| **[IntegrationExample.cs](examples/IntegrationExample.cs)** | Integration with dependency injection and using multiple services together | ASP.NET Core DI, service registration, multi-service workflow |

### Running the Examples

Each example is a complete, standalone application with its own entry point. To run any example:

```bash
# Run Basic Usage example (simplest way to get started)
dotnet run --project examples/BasicUsage.csproj

# Run Advanced Usage example (custom settings and error handling)
dotnet run --project examples/AdvancedUsage.csproj

# Run Integration example (ASP.NET Core DI demonstration)
dotnet run --project examples/IntegrationExample.csproj
```

Alternatively, navigate to the examples directory and run:

```bash
cd examples

# Each example has its own Program.cs entry point
# Run any example using its specific project file
dotnet run --project BasicUsage.csproj
```

### Example Highlights

**Basic Usage** demonstrates the minimal setup required to start capturing and visualizing audio:
- Creating an `AudioCaptureService`
- Initializing with default settings (device 0, 44.1kHz, stereo)
- Subscribing to audio frame events
- Starting and stopping audio recording

**Advanced Usage** shows how to customize the library for your specific needs:
- Using custom audio device settings (different sample rate, mono/stereo)
- Implementing error handling around audio processing
- Extending with additional audio analysis features

**Integration Example** illustrates how to integrate NAudio Visualizer into larger applications:
- Configuring dependency injection with `ServiceCollection`
- Registering NAudio Visualizer services (`AudioCaptureService`, `SpectrumAnalyzer`, etc.)
- Using multiple analyzer services together (spectrum, waveform, spectrogram)
- Leveraging logging for diagnostics

### When to Use Each Example

- **Use BasicUsage** when you're just getting started or need a simple audio capture setup
- **Use AdvancedUsage** when you need to customize audio parameters or add error handling
- **Use IntegrationExample** when building a larger application or need to integrate with ASP.NET Core, WPF, or other frameworks

For production applications, you'll typically use a combination of these patterns, especially the dependency injection approach from IntegrationExample combined with custom settings from AdvancedUsage.

## Code structure

```
src/
  Services/           - AudioCaptureService, SpectrumAnalyzer, WaveformService, SpectrogramAnalyzer
  Domain/Models/      - AudioFrame, AudioBuffer, SpectrumData, WaveformData, SpectrogramData
  Events/             - EventBus (weak-reference pub/sub)
  Configuration/      - ServiceContainer (lightweight DI), ApplicationSettings
  Caching/            - CacheManager<TKey,TValue> with LRU eviction
  Infrastructure/     - Logger, AudioDataConverter
  Utilities/          - MathUtility, ValidationUtility, StringUtility, ColorUtility
  Workers/            - AudioProcessingWorker
  Constants/          - AudioConstants, VisualizationConstants
  Program.cs          - entry point + MainForm
tests/
  naudio-visualizer.Tests/ - xUnit tests for core utilities and audio buffer
```

## Key APIs

**Capture audio:**
```csharp
var capture = new AudioCaptureService();
capture.Initialize(deviceIndex: 0, sampleRate: 44100, channelCount: 2);
capture.FrameCaptured += (_, args) => ProcessFrame(args.Frame);
await capture.StartRecordingAsync();
```

**Analyze spectrum:**
```csharp
var analyzer = new SpectrumAnalyzer();
var spectrum = analyzer.AnalyzeSpectrum(frame, fftSize: 2048);
analyzer.ConvertToLogScale(spectrum);
var bands = analyzer.ExtractFrequencyBands(spectrum); // bass / mid / treble
```

**Generate waveform:**
```csharp
var service = new WaveformService();
var waveform = service.GenerateWaveform(frame, downsamplingFactor: 4);
float[] peaks = service.CalculatePeakValues(waveform.GetData(), peakCount: 512);
```

## Tests

```bash
dotnet test tests/naudio-visualizer.Tests/
```

Covers: AudioBuffer read/write, CacheManager expiry/eviction, EventBus pub-sub,
MathUtility (FFT helpers, RMS, dB conversion), ValidationUtility, StringUtility.

## License

MIT - Copyright (c) 2026 Vladyslav Zaiets
