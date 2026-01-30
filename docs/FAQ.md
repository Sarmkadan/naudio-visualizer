# FAQ - Frequently Asked Questions

## Installation & Setup

### Q: What are the system requirements?

**A:** NAudio Visualizer requires:
- **.NET 10.0** or later ([Download](https://dotnet.microsoft.com/download))
- **Windows 7+**, **macOS 10.15+**, or **Linux with X11/Wayland**
- **512 MB** RAM (1-2 GB recommended)
- Working audio input device (microphone, line-in)

Verify with: `dotnet --version`

### Q: Can I run this on macOS?

**A:** Yes! NAudio Visualizer supports macOS 10.15 (Catalina) and later. Follow the standard build process:

```bash
git clone https://github.com/Sarmkadan/naudio-visualizer.git
cd naudio-visualizer
dotnet build -c Release
dotnet run -c Release
```

### Q: Can I run this on Linux?

**A:** Yes. Linux requires X11 or Wayland with audio support (PulseAudio/ALSA). Install prerequisites:

```bash
# Ubuntu/Debian
sudo apt-get install dotnet-sdk-10.0 libasound2

# Fedora
sudo dnf install dotnet-sdk-10.0 alsa-lib

# Arch
pacman -S dotnet-runtime-10.0 alsa-lib
```

### Q: What if I'm behind a proxy?

**A:** Configure NuGet to use your proxy:

```bash
dotnet nuget add source \
  -n ProxyFeed \
  -u username \
  -p password \
  "https://proxy.company.com/nuget"
```

### Q: How do I uninstall?

**A:** Simply delete the project directory:

```bash
rm -rf naudio-visualizer
```

Windows: Delete folder via File Explorer.

---

## Audio Capture

### Q: Why isn't my microphone being detected?

**A:** Check your audio device:

1. **Windows**: Control Panel > Sound > Recording tab
2. **macOS**: System Preferences > Sound > Input
3. **Linux**: Run `pactl list short sources`

Enable disabled devices, then restart the application.

### Q: Can I use line-in instead of microphone?

**A:** Yes! Select your line-in device from the device list:

```csharp
var devices = audioService.GetAvailableDevices();
var lineInDevice = devices.FirstOrDefault(d => d.Name.Contains("Line"));
audioService.Initialize(lineInDevice.DeviceId);
```

### Q: What sample rates are supported?

**A:** 44,100 Hz, 48,000 Hz, 96,000 Hz, and 192,000 Hz. Select based on your needs:

- **16kHz**: Low bandwidth (speech)
- **44.1kHz**: CD quality (music)
- **48kHz**: Professional audio
- **96kHz, 192kHz**: Studio quality (consumes more CPU)

### Q: Can I capture from multiple devices simultaneously?

**A:** Currently, audio capture is single-device. For multiple sources:

1. Create separate application instances
2. Use virtual audio routing (VB-Audio Cable on Windows)
3. Mix sources in your audio interface

### Q: What's the latency between input and visualization?

**A:** Typical latency:
- Audio buffer: 10-50ms
- FFT computation: 2-10ms
- Rendering: 5-20ms
- **Total: 20-80ms** (acceptable for real-time)

Reduce with smaller FFT size and buffer size.

---

## Visualization & Performance

### Q: Why is the visualization choppy?

**A:** Check CPU usage:

1. Reduce FFT size: `2048 → 1024 → 512`
2. Lower target FPS: `60 → 30 → 24`
3. Disable spectrogram if not needed
4. Check system background tasks

### Q: How do I improve frequency resolution?

**A:** Increase FFT size:

```csharp
settings.DefaultFftSize = 4096;  // Higher resolution, slower
```

Trade-off: Better frequency detail but more CPU usage.

### Q: The waveform is too small. How do I zoom?

**A:** Adjust amplitude zoom:

```csharp
settings.WaveformSettings.AmplitudeZoom = 2.0f;  // 2x zoom
```

### Q: Can I export visualizations?

**A:** Yes! Multiple formats are supported:

```csharp
var exporter = container.Resolve<ExportService>();
await exporter.ExportToJsonAsync(spectrum, "spectrum.json");
await exporter.ExportToCsvAsync(waveform, "waveform.csv");
```

### Q: What resolution should I use for display?

**A:** Recommendations:
- **1920x1080**: Good for most applications
- **1280x720**: Lower CPU, still good quality
- **3840x2160**: High detail, very high CPU

Set via UI window size.

---

## Development & Customization

### Q: Can I use NAudio Visualizer as a library in my project?

**A:** Yes! Reference the NuGet package (when published) or build locally:

```csharp
// In your project
using NAudioVisualizer.Services;
using NAudioVisualizer.Configuration;

var settings = new ApplicationSettings { /* ... */ };
var container = ApplicationConfiguration.ConfigureServices(settings);
var audioService = container.Resolve<AudioCaptureService>();
```

### Q: How do I add custom audio processing?

**A:** Subscribe to frame events and implement your DSP:

```csharp
audioService.FrameCaptured += (sender, args) =>
{
    var samples = args.Frame.Samples[0];  // Left channel
    
    // Your processing here
    ApplyNoiseReduction(samples);
    ApplyEQ(samples);
    
    // Continue visualization
    var waveform = waveformService.GenerateWaveform(args.Frame);
};
```

### Q: Can I add new visualization types?

**A:** Yes! Create a new analyzer:

```csharp
public class CustomAnalyzer
{
    public CustomData Analyze(AudioFrame frame)
    {
        // Your analysis
        return new CustomData { /* ... */ };
    }
}

// Register and use
container.Register<CustomAnalyzer>(() => new CustomAnalyzer());
var analyzer = container.Resolve<CustomAnalyzer>();
```

### Q: How do I integrate with my existing application?

**A:** Use the HTTP API:

```bash
# Query latest spectrum
curl http://localhost:5000/api/spectrum

# Get waveform data
curl http://localhost:5000/api/waveform

# Export session
curl http://localhost:5000/api/export/session-123 -o data.json
```

### Q: Can I contribute improvements?

**A:** Absolutely! Follow these steps:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Implement and test
4. Submit a Pull Request
5. See [Contributing Guidelines](../README.md#contributing) for details

---

## Troubleshooting & Errors

### Q: "AudioDeviceException: No matching audio devices found"

**A:** Solution:
1. Run `--list-devices` to see available devices
2. Check device index in code
3. Verify audio drivers installed
4. Try different sample rate

### Q: "VisualizationException: FFT computation failed"

**A:** Causes:
1. Invalid FFT size (must be power of 2)
2. Audio frame too small for FFT
3. Out of memory
4. Numeric overflow

**Fix:**
```csharp
// Use standard FFT sizes
int[] validSizes = { 256, 512, 1024, 2048, 4096, 8192 };
```

### Q: Memory usage grows continuously

**A:** Solutions:
1. Enable auto-pruning:
   ```csharp
   repository.EnableAutoPruning(retentionTimeSeconds: 300);
   ```

2. Clear cache periodically:
   ```csharp
   cacheManager.Clear();
   ```

3. Monitor with:
   ```csharp
   var stats = cacheManager.GetStatistics();
   Console.WriteLine($"Memory: {stats.TotalSizeBytes / 1024 / 1024} MB");
   ```

### Q: Application crashes on startup

**A:** Check:
1. .NET installation: `dotnet --version`
2. Audio device available: `--list-devices`
3. File permissions: Can write to `logs/` directory
4. Disk space: At least 500MB free

### Q: Which version of .NET should I use?

**A:** Use **.NET 10.0** (latest). Older versions (6, 7, 8) are not supported.

```bash
# Download from:
https://dotnet.microsoft.com/download
```

---

## Performance & Optimization

### Q: How can I measure performance?

**A:** Use the PerformanceProfiler:

```csharp
var profiler = new PerformanceProfiler();
profiler.StartMeasure("operation");
// ... do work ...
var elapsed = profiler.StopMeasure("operation");
Console.WriteLine($"Time: {elapsed.TotalMilliseconds}ms");
```

### Q: What's the fastest configuration?

**A:** For maximum performance:

```csharp
var settings = new ApplicationSettings
{
    DefaultSampleRate = 16000,      // Lower bandwidth
    DefaultFftSize = 512,            // Smaller FFT
    TargetFps = 24,                 // Lower FPS
    BufferSize = 2048               // Smaller buffer
};
```

### Q: What's the best configuration for high quality?

**A:**

```csharp
var settings = new ApplicationSettings
{
    DefaultSampleRate = 96000,      // Professional
    DefaultFftSize = 4096,           // High resolution
    TargetFps = 60,                 // Smooth
    BufferSize = 8192               // Larger buffer
};
```

---

## Licensing & Legal

### Q: What license is this project under?

**A:** MIT License - see [LICENSE](../LICENSE) file for full text.

### Q: Can I use this commercially?

**A:** Yes! MIT License allows commercial use.

### Q: Do I need to attribute the author?

**A:** Attribution is appreciated but not required by the MIT License.

### Q: Can I modify and redistribute?

**A:** Yes, under the MIT License terms.

---

## Additional Resources

- **Getting Started**: [GETTING-STARTED.md](./GETTING-STARTED.md)
- **Architecture**: [ARCHITECTURE.md](./ARCHITECTURE.md)
- **API Reference**: [API-REFERENCE.md](./API-REFERENCE.md)
- **Deployment**: [DEPLOYMENT.md](./DEPLOYMENT.md)
- **Main README**: [../README.md](../README.md)

---

Still have questions? Open an issue on [GitHub](https://github.com/Sarmkadan/naudio-visualizer/issues) or contact the author at [https://sarmkadan.com](https://sarmkadan.com).
