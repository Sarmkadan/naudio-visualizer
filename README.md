![Build](https://github.com/sarmkadan/naudio-visualizer/actions/workflows/build.yml/badge.svg)
![License](https://img.shields.io/github/license/sarmkadan/naudio-visualizer)

# naudio-visualizer

Windows Forms application for real-time audio visualization using NAudio and SkiaSharp.

## Installation

```bash
git clone https://github.com/sarmkadan/naudio-visualizer.git
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

## License

MIT - Copyright (c) 2026 Vladyslav Zaiets
