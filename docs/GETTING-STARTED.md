# Getting Started with NAudio Visualizer

This guide walks you through installing and running NAudio Visualizer for the first time.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Installation Steps](#installation-steps)
- [Running Your First Visualization](#running-your-first-visualization)
- [Troubleshooting](#troubleshooting)
- [Next Steps](#next-steps)

## Prerequisites

### System Requirements
- **Operating System**: Windows 7+ (Windows 10+ recommended), macOS 10.15+, or Linux with X11/Wayland
- **.NET Runtime**: .NET 10.0 or later ([Download](https://dotnet.microsoft.com/download))
- **Memory**: 512 MB minimum, 2 GB recommended
- **Audio Device**: Working microphone or line-in connection

### Verify .NET Installation
Open a terminal and check your .NET version:
```bash
dotnet --version
```

Should display version 10.0.0 or higher.

## Installation Steps

### Option 1: Clone from Source (Recommended)

1. **Clone the Repository**
   ```bash
   git clone https://github.com/Sarmkadan/naudio-visualizer.git
   cd naudio-visualizer
   ```

2. **Build the Project**
   ```bash
   dotnet build -c Release
   ```
   This creates the application in `bin/Release/net10.0-windows/`

3. **Run the Application**
   ```bash
   dotnet run -c Release
   ```

### Option 2: Docker Installation

1. **Build Docker Image**
   ```bash
   docker build -t naudio-visualizer .
   ```

2. **Run Container** (Linux/macOS with X11)
   ```bash
   docker run -it --rm \
     -e DISPLAY=$DISPLAY \
     -v /tmp/.X11-unix:/tmp/.X11-unix \
     naudio-visualizer:latest
   ```

### Option 3: Pre-built Binaries

Download pre-built executables from [Releases](https://github.com/Sarmkadan/naudio-visualizer/releases):
- `naudio-visualizer-win-x64.zip` (Windows)
- `naudio-visualizer-linux-x64.tar.gz` (Linux)
- `naudio-visualizer-macos-universal.dmg` (macOS)

Extract and run the executable.

## Running Your First Visualization

### Step 1: Check Audio Devices
```bash
# List available audio devices
dotnet run -- --list-devices
```

This shows:
- Device ID
- Device Name
- Number of channels
- Sample rates

### Step 2: Start Basic Capture
```bash
# Start capturing from default device
dotnet run
```

The application window opens with:
- Menu bar for control
- Status bar showing capture state
- Main visualization area (initially empty)

### Step 3: Interact with Visualizations

**Using the Menu:**
1. Click `Audio → Start Capture` to begin recording
2. Click `View → Waveform` to see the waveform
3. Click `View → Spectrum` to see frequency analysis
4. Click `View → Spectrogram` to see time-frequency plot
5. Click `Audio → Stop Capture` to end recording

**Keyboard Shortcuts:**
- `W` - Switch to waveform view
- `S` - Switch to spectrum view
- `G` - Switch to spectrogram view
- `Space` - Play/pause capture
- `Esc` - Exit application

### Step 4: Explore Settings
Edit the application settings file to customize:
- Sample rate (44100, 48000, 96000, 192000 Hz)
- FFT size (256, 512, 1024, 2048, 4096 Hz)
- Target FPS (30, 60, 120)
- Colors and rendering options

## Troubleshooting

### "No audio devices found"
- **Windows**: Check Sound settings (Control Panel > Sound)
- **macOS**: System Preferences > Sound > Input
- **Linux**: Run `pactl list short sources` to see PulseAudio devices
- **Solution**: Try connecting a USB audio device if internal device is disabled

### Application won't start
1. Check .NET is installed: `dotnet --version`
2. Clear cache: `rm -rf bin obj`
3. Rebuild: `dotnet clean && dotnet build`

### High CPU usage
- Reduce FFT size: 1024 or 512 instead of 4096
- Lower target FPS: 30 instead of 60
- Disable spectrogram if not needed

### Poor audio quality
- Check your microphone/input device
- Increase sample rate: 48000 or 96000 Hz
- Check input levels in system audio settings

### Can't see visualization
- Verify audio is being captured (check status bar)
- Try different visualization types (Waveform, Spectrum, Spectrogram)
- Check audio device has non-zero amplitude
- Try louder audio source (speaker/music near mic)

## Next Steps

### Learn the Code
- Read [Architecture Documentation](./ARCHITECTURE.md) to understand the design
- Study the [API Reference](./API-REFERENCE.md) for detailed interface documentation
- Check `examples/` directory for real-world usage patterns

### Run Examples
```bash
# Example 1: Basic waveform capture
dotnet run -- --example 01-BasicWaveformCapture

# Example 2: Frequency analysis
dotnet run -- --example 02-FrequencySpectrumAnalysis

# Example 3: Spectrogram generation
dotnet run -- --example 03-SpectrogramGeneration
```

### Explore Features
1. **Multi-device support**: Use different microphones/line-in
2. **Export functionality**: Save visualizations as JSON/CSV
3. **Custom processing**: Implement your own audio filters
4. **HTTP API**: Query visualizations via REST API
5. **Webhook integration**: Publish events to external services

### Configure for Your Use Case
- **Music production**: Use high sample rate (96/192 kHz) and large FFT (4096)
- **Real-time monitoring**: Use lower sample rate and small FFT for responsiveness
- **Speech analysis**: 16kHz sample rate with band-pass filters
- **Scientific research**: Export raw spectrograms for analysis

### Integration with Other Tools
- Export data to MATLAB/Python for analysis
- Stream audio analysis via HTTP API
- Publish events to webhook receivers
- Use as library in your own .NET applications

## Getting Help

### Documentation
- [Architecture Guide](./ARCHITECTURE.md) - System design and components
- [API Reference](./API-REFERENCE.md) - Complete API documentation
- [Deployment Guide](./DEPLOYMENT.md) - Production deployment
- [FAQ](./FAQ.md) - Common questions and solutions

### Community
- GitHub Issues: Report bugs and request features
- Discussions: Ask questions and share ideas
- Portfolio: [https://sarmkadan.com](https://sarmkadan.com)

### Debugging

Enable verbose logging:
```csharp
var logger = container.Resolve<Logger>();
logger.MinimumLevel = LogLevel.Debug;
```

This outputs detailed debug information to the console and `logs/app.log` file.

---

Happy visualizing! 🎵🎨
