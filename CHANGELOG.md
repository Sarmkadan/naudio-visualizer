# Changelog

All notable changes to NAudio Visualizer are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.0] - 2026-03-15

### Added
- HTTP REST API for remote visualization queries
- Webhook integration for event publishing
- Multi-format export (JSON, CSV, XML)
- Spectral flux detection for onset/beat detection
- Performance profiler utility for benchmarking
- Docker and docker-compose configuration
- GitHub Actions CI/CD pipeline
- Comprehensive documentation suite (5 docs)

### Improved
- 40% faster waveform downsampling algorithm
- Reduced memory footprint for circular buffer (zero-allocation)
- Enhanced cache performance with LRU eviction
- Better error handling with custom exceptions
- Improved logging with multiple severity levels

### Fixed
- Memory leak in long-running sessions
- FFT window function application bug
- Race condition in VisualizationDataRepository
- Audio device initialization race on Windows

## [1.1.0] - 2026-02-10

### Added
- Session recording and replay functionality
- Audio session repository for frame persistence
- Spectrogram colormap selection (Viridis, Plasma, Inferno, Magma)
- Frequency band extraction (bass/mid/treble)
- Peak hold feature for spectrum visualization
- Grid overlay option for waveforms
- Amplitude zoom control for better visibility

### Improved
- Waveform rendering performance (25% faster)
- FFT computation using optimized algorithm
- Spectrum smoothing with adjustable factor
- UI responsiveness with background workers
- Configuration validation

### Fixed
- Stereo channel separation in waveform display
- Spectrum magnitude scaling (dB conversion)
- Memory pruning age calculation
- Device initialization on macOS

## [1.0.0] - 2026-01-20

### Added
- Core audio capture from multiple devices
- Real-time waveform visualization
- FFT-based spectrum analysis
- Spectrogram generation and display
- Circular audio buffer with configurable capacity
- Service-based architecture with dependency injection
- Comprehensive logging system
- Thread-safe data repositories
- Custom exception types for error handling
- Audio constants and standard definitions
- Command-line interface
- WinForms-based GUI application
- Example applications demonstrating features

### Features
- **Waveform Visualization**: Stereo channel separation, downsampling, smoothing
- **Spectrum Analysis**: 256-16384 FFT sizes, logarithmic scaling, peak detection
- **Spectrogram Display**: Time-frequency representation with multiple colormaps
- **Device Management**: Automatic device enumeration, multi-device support
- **Performance Optimized**: Configurable quality levels, adaptive downsampling
- **Memory Efficient**: Circular buffering, automatic pruning, object pooling
- **Extensible Architecture**: Clean separation of concerns, service-based design

### Documentation
- Comprehensive README with usage examples
- Architecture documentation
- API reference guide
- Getting started tutorial
- Deployment guide
- FAQ section
- 8 example applications

---

## Version History

### Release Pattern
- **Major**: Significant API changes or new features
- **Minor**: New features (backward compatible)
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
- [ ] Android/iOS mobile visualization apps
- [ ] Web-based visualization dashboard
- [ ] MIDI input support
- [ ] Multi-channel audio (5.1, 7.1 surround)

### Planned Improvements
- Performance optimization for 4K displays
- Advanced caching strategies
- Custom colormap support
- Audio format conversion utilities
- Real-time statistics export
- A/B comparison visualization

---

## Upgrading

### From 1.1.0 to 1.2.0
- No breaking changes
- HTTP API is new, no migration needed
- Existing applications continue to work unchanged

### From 1.0.0 to 1.1.0
- Session recording API is additive
- Existing code compatible
- New features optional

### Breaking Changes History
- **v1.0.0**: Initial release (no breaking changes)

---

## Contributors

- **Vladyslav Zaiets** - Creator and maintainer ([https://sarmkadan.com](https://sarmkadan.com))

## License

MIT License - Copyright (c) 2026 Vladyslav Zaiets

---

## Notes

### Performance Benchmarks

**FFT Analysis (2048-point):**
- Time: 2-5ms per frame
- Memory: ~1MB per frame
- CPU: 5-15% (single core)

**Waveform Rendering (1920x1080):**
- Time: 3-8ms per frame
- Memory: ~2MB per frame
- CPU: 10-20% (single core)

**Spectrogram Generation (5-second window):**
- Time: 50-100ms
- Memory: ~5MB
- CPU: 20-30% (single core)

### Known Limitations

1. Single audio device capture (multiple devices via separate instances)
2. Maximum 10,000 frames per session (configurable)
3. Cache size limited to 100MB (configurable)
4. Windows audio subsystem (WASAPI) only on Windows
5. Display server required on Linux (X11/Wayland)

### Future Roadmap

**Q2 2026**: WebSocket streaming, advanced filters
**Q3 2026**: GPU acceleration, machine learning integration
**Q4 2026**: Mobile applications, web dashboard

---

For more details, see the [README](./README.md) and [GitHub Releases](https://github.com/Sarmkadan/naudio-visualizer/releases).
