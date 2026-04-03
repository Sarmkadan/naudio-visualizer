# Changelog

All notable changes to NAudio Visualizer are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2026-01-14

### Added
- Add VST plugin host with parameter automation and preset management
- Docker support with multi-stage builds
- Health check endpoints (/health, /health/ready)
- Integration test suite with xUnit
- Migration guide from v1.x

### Changed
- Upgraded to .NET 10.0
- Modern C# features (records, primary constructors)
- Improved API consistency

### Fixed
- Various edge cases found through testing

## [1.0.0] - 2025-11-14

### Added
- HTTP REST API for remote visualization queries
- Webhook integration for event publishing
- Multi-format export (JSON, CSV, XML)
- Spectral flux detection for onset/beat detection
- Performance profiler utility for benchmarking
- Docker and docker-compose configuration
- GitHub Actions CI/CD pipeline with CodeQL security scanning
- Comprehensive documentation suite (API reference, architecture, deployment, FAQ)
- NuGet package configuration and publishing workflow

### Improved
- 40% faster waveform downsampling algorithm
- Reduced memory footprint for circular buffer (zero-allocation on hot path)
- Enhanced cache performance with LRU eviction policy
- Better error handling with typed custom exceptions
- Improved logging with multiple severity levels and file sink

### Fixed
- Memory leak in long-running sessions
- FFT window function application order
- Race condition in `VisualizationDataRepository` under concurrent writes
- Audio device initialization race on Windows WASAPI

## [0.5.0] - 2025-10-03

### Added
- Session recording and replay functionality
- `AudioSessionRepository` for durable frame persistence
- Spectrogram colormap selection (Viridis, Plasma, Inferno, Magma)
- Frequency band extraction (sub-bass / bass / mid / presence / treble)
- Peak hold feature for spectrum visualization
- Grid overlay option for waveform display
- Amplitude zoom control for waveform detail

### Improved
- Waveform rendering performance (25% faster via SIMD-friendly inner loop)
- FFT computation using optimized Cooley-Tukey variant
- Spectrum smoothing with adjustable exponential factor
- Background workers improve UI responsiveness under load

### Fixed
- Stereo channel separation in waveform display
- Spectrum magnitude scaling (correct dB conversion)
- Memory pruning age calculation for daylight-saving boundaries
- Device initialization ordering on macOS CoreAudio

## [0.3.0] - 2025-08-22

### Added
- `CacheManager` with configurable LRU eviction and TTL-based expiration
- `VisualizationCache` wrapper for typed visualization data
- Caching strategy examples
- `EventBus` and `EventPublisher` for decoupled in-process messaging
- `AudioVisualizationEvents` typed event definitions
- Middleware pipeline: exception handler, logging, rate-limit
- `RequestContext` for per-request correlation IDs
- `RateLimitMiddleware` with token-bucket algorithm

### Improved
- `ServiceContainer` now supports scoped lifetime registration
- `Logger` adds structured context fields alongside message
- `AudioDataConverter` upsample/downsample with anti-aliasing filter

### Fixed
- Null-reference in `SpectrogramAnalyzer` when frame list is empty
- Off-by-one in circular buffer read pointer after wrap-around

## [0.2.0] - 2025-07-11

### Added
- `SpectrogramAnalyzer` with time-frequency representation and log-scale normalization
- `SpectrogramData` domain model with 2D time-window array
- `MidiInputService` and `MidiNoteEvent` for MIDI trigger integration
- CLI layer: `CommandLineParser`, `CommandExecutor`, `HelpGenerator`, `VisualizationCommand`
- `ExportService` with JSON, CSV, and XML formatters
- `OutputFormatterFactory` for runtime format selection
- `AudioFileLoader` for loading WAV/MP3 files as frame sequences
- `WebhookPublisher` and `HttpClientFactory` for external integrations
- Six additional example applications (03 through 08)

### Improved
- `SpectrumAnalyzer` window functions extended to include Hamming and Blackman
- `WaveformService` smoothing uses double-pass IIR filter
- Domain models gain XML documentation on all public members

### Fixed
- `AudioBuffer` read/write index overflow on 32-bit builds
- Device enumeration skips disconnected devices instead of throwing

## [0.1.0] - 2025-06-02

### Added
- Core audio capture from system audio devices via NAudio WASAPI
- Real-time waveform visualization with stereo channel separation
- FFT-based spectrum analysis (256–8192 point, Hann window)
- Circular audio buffer with configurable capacity and thread-safe access
- `AudioCaptureService`, `WaveformService`, `SpectrumAnalyzer` core services
- Domain models: `AudioFrame`, `AudioBuffer`, `AudioDevice`, `WaveformData`, `SpectrumData`
- `VisualizationDataRepository` and `AudioSessionRepository` (in-memory)
- Lightweight `ServiceContainer` dependency injection
- `Logger` with console and file sinks and configurable minimum level
- `AudioDataConverter` float↔PCM int16 conversion utilities
- `AudioConstants` with standard sample rates, FFT sizes, frequency bands
- `VisualizationSettings` and `ApplicationSettings` configuration models
- Custom exceptions: `AudioDeviceException`, `AudioStreamException`, `VisualizationException`
- WinForms application entry point with `UseWindowsForms` project setup
- Two example applications: basic waveform capture and frequency spectrum analysis
- MIT License, README, and initial project documentation

---

## Version History

### Release Pattern
- **Major**: Significant API changes
- **Minor**: New backward-compatible features
- **Patch**: Bug fixes and minor improvements

### Support Policy
- Latest version: Full support
- Previous minor version: Bug fixes only
- Older versions: Community-driven support

---

## Unreleased

### Under Development
- [ ] Real-time spectrogram streaming via WebSocket
- [ ] GPU acceleration for FFT computations
- [ ] Advanced audio effects (EQ, compression, reverb)
- [ ] Machine learning-based audio classification
- [ ] Multi-channel audio (5.1, 7.1 surround)
- [ ] Web-based visualization dashboard

---

## Upgrading

### From 0.5.0 to 1.0.0
- No breaking changes to core service APIs
- HTTP API is additive; existing console/WinForms usage unchanged
- Docker images available from this release onwards

### From 0.3.0 to 0.5.0
- `AudioSessionRepository` replaces ad-hoc frame lists; update any direct frame storage
- Colormap enum values renamed for consistency (Viridis → ColormapType.Viridis)

### From 0.2.0 to 0.3.0
- `CacheManager` constructor now accepts `CacheOptions`; update instantiation sites

### Breaking Changes History
- **v0.2.0**: `SpectrogramAnalyzer` constructor requires explicit `fftSize` parameter
- **v0.1.0**: Initial release

---

## Contributors

- **Vladyslav Zaiets** - Creator and maintainer ([https://sarmkadan.com](https://sarmkadan.com))

## License

MIT License - Copyright (c) 2025 Vladyslav Zaiets
