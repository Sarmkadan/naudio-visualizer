# Deployment Guide

Production deployment strategies for NAudio Visualizer.

## Table of Contents
- [Windows Deployment](#windows-deployment)
- [Linux Deployment](#linux-deployment)
- [Docker Deployment](#docker-deployment)
- [Performance Tuning](#performance-tuning)
- [Monitoring](#monitoring)
- [Troubleshooting](#troubleshooting)

## Windows Deployment

### Self-Contained Executable

Create a single executable without requiring .NET installation:

```bash
# Publish as self-contained
dotnet publish -c Release \
  --self-contained \
  --runtime win-x64 \
  --output publish/windows

# Create ZIP for distribution
cd publish/windows
zip -r naudio-visualizer-win-x64.zip .
```

### MSIX Package (Microsoft Store)

```bash
# Create MSIX package
dotnet publish -c Release \
  --self-contained \
  --runtime win-x64 \
  --output publish/msix

# Use MsixPackagingTool to create .msix file
# Then publish to Microsoft Store
```

### Installer with WiX

```bash
# Install WiX Toolset
choco install wixtoolset

# Create WiX configuration
# See: https://wixtoolset.org/docs/

# Build MSI
candle.exe setup.wxs
light.exe setup.wixobj -out setup.msi
```

## Linux Deployment

### AppImage

Create portable Linux package:

```bash
# Install appimagetool
wget https://github.com/AppImage/AppImageKit/releases/download/...

# Create AppImage structure
mkdir AppDir
dotnet publish -c Release \
  --self-contained \
  --runtime linux-x64 \
  --output AppDir/usr/bin

# Create desktop entry
cat > AppDir/naudio-visualizer.desktop << EOF
[Desktop Entry]
Name=NAudio Visualizer
Exec=naudio-visualizer
Icon=naudio-visualizer
Type=Application
Categories=Multimedia;Audio;
EOF

# Create AppImage
./appimagetool AppDir naudio-visualizer.AppImage
```

### Snap Package

```bash
# Create snapcraft.yaml
cat > snapcraft.yaml << EOF
name: naudio-visualizer
version: 1.0.0
summary: Real-time audio visualization
description: |
  NAudio Visualizer provides real-time
  audio visualization with waveform,
  spectrum, and spectrogram displays.

base: core22
confinement: strict

parts:
  naudio-visualizer:
    plugin: dotnet
    source: .
    dotnet-build-configuration: Release
    dotnet-self-contained-runtime: linux-x64

apps:
  naudio-visualizer:
    command: bin/Release/net10.0-linux-x64/naudio-visualizer
    plugs:
      - audio
      - display
      - x11
EOF

# Build snap
snapcraft

# Install locally
snap install naudio-visualizer_1.0.0_amd64.snap --dangerous
```

### DEB Package

```bash
# Create DEB structure
mkdir -p deb_package/DEBIAN
mkdir -p deb_package/usr/local/bin
mkdir -p deb_package/usr/share/applications

# Build application
dotnet publish -c Release \
  --self-contained \
  --runtime linux-x64 \
  --output deb_package/usr/local/bin/naudio-visualizer

# Create control file
cat > deb_package/DEBIAN/control << EOF
Package: naudio-visualizer
Version: 1.0.0
Architecture: amd64
Maintainer: Vladyslav Zaiets <rutova2@gmail.com>
Description: Real-time audio visualization

Depends: libudev0, libx11-6
EOF

# Create desktop entry
cat > deb_package/usr/share/applications/naudio-visualizer.desktop << EOF
[Desktop Entry]
Name=NAudio Visualizer
Exec=/usr/local/bin/naudio-visualizer/naudio-visualizer
Icon=naudio-visualizer
Type=Application
Categories=Multimedia;Audio;
EOF

# Build DEB
dpkg-deb --build deb_package naudio-visualizer_1.0.0_amd64.deb

# Install
sudo dpkg -i naudio-visualizer_1.0.0_amd64.deb
```

## Docker Deployment

### Single-Stage Build

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 as build
WORKDIR /src
COPY . .
RUN dotnet build -c Release

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /src/bin/Release/net10.0-linux-x64/publish .
ENTRYPOINT ["dotnet", "naudio-visualizer.dll"]
```

### Multi-Stage Build (Optimized)

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 as builder
WORKDIR /build
COPY . .
RUN dotnet publish -c Release \
    --self-contained \
    --runtime linux-x64 \
    --output /app

FROM ubuntu:22.04
RUN apt-get update && apt-get install -y \
    libx11-6 libasound2 alsa-utils \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=builder /app .
ENV DISPLAY=:0
ENTRYPOINT ["./naudio-visualizer"]
```

### Docker Compose

```yaml
version: '3.8'
services:
  naudio-visualizer:
    build: .
    environment:
      - DISPLAY=$DISPLAY
    volumes:
      - /tmp/.X11-unix:/tmp/.X11-unix
      - ./data:/app/data
      - ./logs:/app/logs
    devices:
      - /dev/snd:/dev/snd
    restart: unless-stopped
```

### Running in Container

```bash
# Build image
docker build -t naudio-visualizer:1.0 .

# Run with audio access
docker run -it --rm \
  --device /dev/snd \
  --volume /tmp/.X11-unix:/tmp/.X11-unix \
  -e DISPLAY=$DISPLAY \
  naudio-visualizer:1.0

# Run in background
docker run -d \
  --name visualizer \
  --device /dev/snd \
  --volume /tmp/.X11-unix:/tmp/.X11-unix \
  -e DISPLAY=$DISPLAY \
  naudio-visualizer:1.0
```

## Performance Tuning

### FFT Size Optimization

```csharp
// For real-time visualization
settings.DefaultFftSize = 2048;  // Good balance

// For maximum responsiveness
settings.DefaultFftSize = 512;   // Fast, less detail

// For maximum frequency detail
settings.DefaultFftSize = 4096;  // Slow, high resolution
```

### Sample Rate Selection

```csharp
// For speech/monitoring
settings.DefaultSampleRate = 16000;  // Lower bandwidth, faster

// For music (CD quality)
settings.DefaultSampleRate = 44100;  // Standard

// For high-fidelity audio
settings.DefaultSampleRate = 96000;  // Professional
```

### Buffer Configuration

```csharp
// Small buffer = low latency, high CPU
settings.BufferSize = 2048;

// Medium buffer = balanced
settings.BufferSize = 4096;  // Default

// Large buffer = high latency, low CPU
settings.BufferSize = 16384;
```

### Frame Rate

```csharp
// 24 FPS = minimal CPU, smooth for video
settings.TargetFps = 24;

// 30 FPS = standard
settings.TargetFps = 30;

// 60 FPS = high-quality, high CPU
settings.TargetFps = 60;

// 120 FPS = maximum quality, very high CPU
settings.TargetFps = 120;
```

## Monitoring

### Health Check Endpoint

```csharp
// Implement health check API
[HttpGet("health")]
public IActionResult Health()
{
    var audioService = _container.Resolve<AudioCaptureService>();
    return Ok(new {
        status = "healthy",
        recording = audioService.IsRecording,
        timestamp = DateTime.UtcNow
    });
}
```

### Performance Metrics

```csharp
// Track metrics
var profiler = new PerformanceProfiler();
profiler.StartMeasure("fft_analysis");
// ... perform FFT ...
var duration = profiler.StopMeasure("fft_analysis");

Console.WriteLine($"FFT: {duration.TotalMilliseconds}ms");
```

### Logging

Configure logging for production:

```csharp
var logger = container.Resolve<Logger>();
logger.MinimumLevel = LogLevel.Warning;  // Only warnings and errors
```

## Troubleshooting

### Audio Device Not Found in Container

```bash
# Verify ALSA is configured
docker run -it --rm \
  --device /dev/snd \
  ubuntu:22.04 \
  bash -c "apt-get update && apt-get install -y alsa-utils && aplay -l"

# Check PulseAudio
docker run -it --rm \
  -e PULSE_SERVER=unix:/run/user/1000/pulse/native \
  -v /run/user/1000/pulse:/run/user/1000/pulse \
  naudio-visualizer:latest
```

### High CPU Usage in Container

1. Reduce FFT size and frame rate
2. Use smaller display resolution
3. Disable spectrogram if not needed
4. Check for CPU limits: `docker inspect <container>`

### Memory Leak in Long-Running Deployment

1. Enable GC collection: `GC.Collect()`
2. Monitor cache size: limit with `CacheMaxSize`
3. Enable frame pruning: `EnableAutoPruning()`
4. Check for event subscription leaks

### Display Issues in Docker

Ensure X11 forwarding:
```bash
docker run -it \
  -e DISPLAY=$DISPLAY \
  -v /tmp/.X11-unix:/tmp/.X11-unix:rw \
  -v $HOME/.Xauthority:/root/.Xauthority:rw \
  naudio-visualizer:latest
```

## Systemd Service (Linux)

Create `/etc/systemd/system/naudio-visualizer.service`:

```ini
[Unit]
Description=NAudio Visualizer
After=network.target

[Service]
Type=simple
User=visualizer
WorkingDirectory=/opt/naudio-visualizer
ExecStart=/opt/naudio-visualizer/naudio-visualizer
Restart=on-failure
RestartSec=5

[Install]
WantedBy=multi-user.target
```

Enable and start:
```bash
sudo systemctl enable naudio-visualizer
sudo systemctl start naudio-visualizer
```

## Release Checklist

- [ ] Update version in `.csproj`
- [ ] Update `CHANGELOG.md`
- [ ] Run full test suite
- [ ] Performance benchmark (FFT, memory)
- [ ] Create signed binaries
- [ ] Test deployment on target platforms
- [ ] Create GitHub release
- [ ] Document any breaking changes
- [ ] Update documentation
- [ ] Announce release

---

For installation instructions, see [Getting Started](./GETTING-STARTED.md).
