![CI](https://github.com/sarmkadan/naudio-visualizer/actions/workflows/ci.yml/badge.svg)
![License](https://img.shields.io/github/license/sarmkadan/naudio-visualizer)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)

# naudio-visualizer

Windows Forms application for real-time audio visualization using NAudio and SkiaSharp.
Captures audio from any input device and renders waveform, frequency spectrum, and spectrogram views.

## Requirements

- Windows 10 or later
- .NET 10 SDK
- Audio input device (microphone, line-in, etc.)

## Build

```bash
git clone https://github.com/Sarmkadan/naudio-visualizer.git
cd naudio-visualizer
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
