# Architecture Guide

Comprehensive overview of NAudio Visualizer's system design, components, and data flow.

## Table of Contents
- [System Overview](#system-overview)
- [Architecture Patterns](#architecture-patterns)
- [Component Details](#component-details)
- [Data Flow](#data-flow)
- [Threading Model](#threading-model)
- [Memory Management](#memory-management)
- [Extensibility](#extensibility)

## System Overview

NAudio Visualizer uses a **layered architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│  (WinForms UI, CLI, HTTP API)           │
└─────────────┬───────────────────────────┘
              │
┌─────────────▼───────────────────────────┐
│      Application Layer                  │
│  (Services: Capture, Analyze, Cache)   │
└─────────────┬───────────────────────────┘
              │
┌─────────────▼───────────────────────────┐
│        Domain Layer                     │
│  (Models, Business Logic, Events)      │
└─────────────┬───────────────────────────┘
              │
┌─────────────▼───────────────────────────┐
│     Infrastructure Layer                │
│  (Data Access, Logging, Utilities)     │
└─────────────┴───────────────────────────┘
              │
              ▼
        External Systems
    (NAudio, SkiaSharp, OS)
```

## Architecture Patterns

### 1. Dependency Injection
The `ServiceContainer` provides lightweight IoC functionality:
- Lazy service registration
- Transient and singleton scopes
- Factory pattern support
- Minimal reflection overhead

```csharp
var container = new ServiceContainer();
container.Register<ILogger>(() => new Logger("app.log"));
container.Register<IAudioService>(() => new AudioCaptureService());
var logger = container.Resolve<ILogger>();
```

### 2. Repository Pattern
Data access abstraction with in-memory repositories:

**VisualizationDataRepository**
- Stores and queries visualization data
- Provides time-range queries
- Implements memory pruning
- Thread-safe operations

**AudioSessionRepository**
- Session lifecycle management
- Frame storage and retrieval
- Efficient memory management
- Session statistics

### 3. Service Layer Pattern
Business logic isolated in service classes:

- **AudioCaptureService**: Hardware abstraction, device enumeration, frame delivery
- **WaveformService**: Waveform generation, downsampling, smoothing
- **SpectrumAnalyzer**: FFT analysis, windowing, frequency band extraction
- **SpectrogramAnalyzer**: Time-frequency representation, spectral flux detection

### 4. Event-Driven Architecture
Asynchronous communication between components:
- `FrameCaptured` - New audio frame available
- `VisualizationUpdated` - Visualization data ready
- `SessionStarted/Ended` - Session lifecycle events
- `CacheHit/Miss` - Caching events for monitoring

### 5. Observer Pattern
Components subscribe to events without tight coupling:

```csharp
audioService.FrameCaptured += (sender, frame) =>
{
    var waveform = waveformService.GenerateWaveform(frame);
    // Handle visualization
};
```

## Component Details

### Audio Capture Pipeline

```
┌────────────┐    ┌─────────────┐    ┌──────────────┐
│ NAudio    │───▶│ AudioBuffer │───▶│ Frame        │
│ WaveIn    │    │ (circular)  │    │ EventPublish │
└────────────┘    └─────────────┘    └──────────────┘
     ▲                                     │
     │                                     ▼
     └─────────────────────────────────────┐
                                           │
                          ┌────────────────┴──────────────┐
                          │ Service Subscribers            │
                          ├───────────────────────────────┤
                          │ - WaveformService             │
                          │ - SpectrumAnalyzer            │
                          │ - SpectrogramAnalyzer         │
                          │ - VisualizationDataRepository │
                          └───────────────────────────────┘
```

**Key Components:**

1. **NAudio WaveInEvent**
   - Captures audio from device
   - Delivers frames asynchronously
   - 44.1kHz-192kHz sample rate support
   - Mono/stereo channel support

2. **AudioBuffer**
   - Thread-safe circular buffer
   - Zero-allocation design (object pool)
   - Configurable capacity (default 44100 * 5 samples)
   - Supports variable read/write sizes

3. **AudioFrame**
   - Contains raw PCM samples
   - Timestamp information
   - Channel metadata
   - Sample rate information

### Analysis Pipeline

```
┌────────────────┐
│ AudioFrame     │
└────────┬───────┘
         │
    ┌────┴───────────────┬──────────────┬──────────────┐
    │                    │              │              │
    ▼                    ▼              ▼              ▼
┌──────────┐    ┌────────────────┐  ┌────────────┐  ┌──────────────┐
│ Waveform │    │ Spectrum       │  │Spectrogram │  │AudioMetadata │
│ Service  │    │ Analyzer (FFT) │  │ Analyzer   │  │ Repository   │
└──────┬───┘    └────────┬───────┘  └────┬───────┘  └──────────────┘
       │                 │               │
       │      ┌──────────┴───────────────┘
       │      │
       ▼      ▼
    ┌──────────────────┐
    │ Cache Manager    │
    ├──────────────────┤
    │ - Expiration TTL │
    │ - LRU eviction   │
    │ - Statistics     │
    └──────────────────┘
```

### Visualization Pipeline

```
┌──────────────────────┐
│ VisualizationData    │
│ - Waveform          │
│ - Spectrum          │
│ - Spectrogram       │
└──────────┬───────────┘
           │
    ┌──────┴────────────────────────┐
    │                               │
    ▼                               ▼
┌──────────────┐           ┌────────────────┐
│ SkiaSharp    │           │ Export Service │
│ Renderer     │           │ (JSON/CSV/XML) │
└──────────────┘           └────────────────┘
    │
    ▼
┌──────────────┐
│ Display/File │
└──────────────┘
```

## Data Flow

### Capture → Analysis → Visualization

```
[Audio Device]
      │
      │ PCM samples @ 44.1kHz stereo
      ▼
[AudioCaptureService.FrameCaptured]
      │ 1 frame / 10ms (4410 samples)
      ▼
┌─────────────────────────────────┐
│ Multiple Parallel Subscribers   │
├─────────────────────────────────┤
│ 1. WaveformService              │ ──→ WaveformData
│ 2. SpectrumAnalyzer (2048 FFT)  │ ──→ SpectrumData
│ 3. SpectrogramAnalyzer          │ ──→ SpectrogramData
│ 4. SessionRepository (optional) │ ──→ Frame storage
└─────────────────────────────────┘
      │
      ▼
[CacheManager]
    Hit? ────────┐
    Miss?        │
      │          │
      ▼          ▼
[Render]───────→[Display]
      │
      ▼
[Event Bus]
      │
      ├─→ [HTTP API clients]
      ├─→ [Webhook publishers]
      └─→ [Export workers]
```

### Session Recording Flow

```
[Audio Device]
    │
    ▼
[AudioSessionRepository.AddFrameAsync]
    │
    ├─→ [Memory storage]
    │   (up to MaxFramesPerSession)
    │
    └─→ [SessionMetadata]
        - StartTime
        - EndTime
        - FrameCount
        - Duration
        - Statistics
```

## Threading Model

### Thread Safety Design

1. **AudioCaptureService**
   - Receives frames on NAudio callback thread
   - Events fired asynchronously (subscriber responsibility)
   - Lock-free when possible

2. **AudioBuffer**
   - Thread-safe circular buffer
   - Read/write position tracking
   - Single reader/writer optimal
   - Multiple readers: synchronized

3. **VisualizationDataRepository**
   - Thread-safe concurrent collections
   - Lock on data modification
   - Lock-free reads when possible

4. **CacheManager**
   - Thread-safe dictionary
   - Atomic operations
   - No blocking on hit path

### Synchronization Strategy

```csharp
// Lock-free on hot path (cache hit)
if (cache.TryGetValue(key, out var value))
{
    return value;  // No lock
}

// Lock only on miss path
lock (cache)
{
    value = Compute();
    cache[key] = value;
}
```

### Background Processing

**ScheduledTaskRunner**
- Runs long-duration tasks on thread pool
- Doesn't block UI thread
- Supports cancellation tokens
- Monitors progress

**DataExportWorker**
- Exports visualization data asynchronously
- Batches operations
- Monitors for completion

## Memory Management

### Audio Buffering

```csharp
// Circular buffer design
var buffer = new AudioBuffer(capacity: 44100 * 5);  // 5 seconds @ 44.1kHz

// Write (from capture thread)
buffer.Write(samples, count);  // O(1) operation

// Read (from UI thread)
buffer.Read(output, offset, length);  // O(1) operation
```

### Visualization Data Retention

```csharp
// Configure retention policy
repository.EnableAutoPruning(
    retentionTimeSeconds: 300,  // Keep 5 minutes
    checkIntervalSeconds: 10    // Check every 10s
);
```

### Cache Management

```csharp
// LRU eviction with TTL
cacheManager.Set(key, value, 
    expiration: TimeSpan.FromSeconds(30));

// Statistics tracking
var stats = cacheManager.GetStatistics();
// TotalSizeBytes, ItemCount, HitRate, etc.
```

## Extensibility

### Adding Custom Analyzers

1. **Implement IAnalyzer**
```csharp
public interface IAnalyzer
{
    AnalysisResult Analyze(AudioFrame frame);
}
```

2. **Register in ServiceContainer**
```csharp
container.Register<IAnalyzer>(() => new CustomAnalyzer());
```

3. **Subscribe to FrameCaptured**
```csharp
audioService.FrameCaptured += (s, args) =>
{
    var result = analyzer.Analyze(args.Frame);
};
```

### Adding Custom Renderers

1. **Extend SkiaSharp canvas**
```csharp
public class CustomRenderer
{
    public void Render(SKCanvas canvas, VisualizationData data)
    {
        // Custom rendering logic
    }
}
```

2. **Hook into event system**
```csharp
eventBus.Subscribe<VisualizationUpdatedEvent>(
    (e) => renderer.Render(e.Data)
);
```

### Adding Export Formats

1. **Implement IFormatter**
```csharp
public interface IFormatter
{
    string Format(VisualizationData data);
}
```

2. **Register factory**
```csharp
formatterFactory.Register("custom", () => new CustomFormatter());
```

## Performance Considerations

### Optimization Techniques

1. **Downsampling**
   - Reduce waveform points for rendering
   - Example: 44100 samples → 1024 points
   - Improves render performance by 40x

2. **Spectral Smoothing**
   - Temporal: Smooth across time (exponential moving average)
   - Frequency: Smooth across bins (Gaussian filter)
   - Reduces flickering, visual coherence

3. **FFT Optimization**
   - Use power-of-2 FFT sizes (2048 optimal)
   - Smaller sizes (512) = faster but less frequency resolution
   - Larger sizes (4096) = slower but more resolution

4. **Cache Coherency**
   - Align visualization update frequency with display refresh (60Hz)
   - Avoid redundant FFT calculations
   - Reuse computed results where possible

### Memory Profiling

Monitor heap allocations:
```csharp
var sw = Stopwatch.StartNew();
var before = GC.GetTotalMemory(true);

// Run operation

var after = GC.GetTotalMemory(false);
var elapsed = sw.ElapsedMilliseconds;

Console.WriteLine($"Allocs: {(after - before) / 1024} KB");
Console.WriteLine($"Time: {elapsed} ms");
```

---

For detailed API documentation, see [API Reference](./API-REFERENCE.md).
