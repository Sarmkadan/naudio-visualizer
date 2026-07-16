# Architecture

Overview of NAudio Visualizer's actual system design, grounded in the code under `src/`.

## Table of Contents
- [System Overview](#system-overview)
- [Project Layout](#project-layout)
- [Component Breakdown](#component-breakdown)
- [Data Flow](#data-flow)
- [Key Design Decisions](#key-design-decisions)
- [Threading Model](#threading-model)
- [Extension Points](#extension-points)
- [Known Limitations](#known-limitations)

## System Overview

NAudio Visualizer is a single Windows Forms executable (`net10.0-windows`, `OutputType=WinExe`) that captures audio via NAudio's `WaveInEvent` and is intended to render visualizations with SkiaSharp. It uses a layered layout:

```
+-----------------------------------------+
| Presentation: WinForms MainForm         |  src/Program.cs
+-----------------------------------------+
| Services: capture, waveform, FFT, MIDI  |  src/Services/
+-----------------------------------------+
| Domain: models, events, exceptions      |  src/Domain/, src/Events/, src/Exceptions/
+-----------------------------------------+
| Infrastructure: logging, config, cache, |  src/Infrastructure/, src/Configuration/,
| in-memory repositories, utilities       |  src/Caching/, src/Data/, src/Utilities/
+-----------------------------------------+
        External: NAudio 2.2.1, SkiaSharp 2.88.9, WinForms
```

There is **no** HTTP API, CLI, webhook publisher, or export service in this codebase. Earlier revisions of this document described such components; they do not exist.

## Project Layout

| Path | Contents |
|---|---|
| `src/Program.cs` | `Main` entry point plus `MainForm` (menu/status bar shell; visualization views are TODO stubs) |
| `src/Configuration/` | `ServiceContainer` (minimal DI), `ApplicationConfiguration`, `ApplicationSettings`, `ConfigurationManager` (key-value settings file) |
| `src/Services/` | `AudioCaptureService`, `WaveformService`, `SpectrumAnalyzer`, `SpectrogramAnalyzer`, `MidiInputService` |
| `src/Domain/Models/` | `AudioFrame`, `AudioBuffer`, `AudioMetadata`, `AudioDevice`, `WaveformData`, `SpectrumData`, `SpectrogramData`, `VisualizationData`, `VisualizationSettings`, themes/gradients |
| `src/Events/` | `EventBus` (weak-reference pub/sub), `EventPublisher`, event records |
| `src/Data/Repositories/` | `AudioSessionRepository`, `VisualizationDataRepository` (in-memory, lock-based) |
| `src/Caching/` | `CacheManager<TKey,TValue>` (TTL + least-recently-accessed eviction) |
| `src/Infrastructure/` | `Logger` (file/console), `AudioDataConverter` |
| `src/Workers/` | `AudioProcessingWorker` (queued background processing tasks) |
| `src/Utilities/`, `src/Constants/` | Math/color/path/string helpers, `AudioConstants`, `VisualizationConstants` |
| `tests/` | xUnit tests (`naudio-visualizer.Tests`) and BenchmarkDotNet project (`naudio-visualizer.Benchmarks`) |

## Component Breakdown

### Startup (`Program.Main`)
1. Builds an `ApplicationSettings` from `AudioConstants` / `VisualizationConstants` defaults and validates it with `IsValid()`.
2. Calls `ApplicationConfiguration.ConfigureServices(settings)` to populate a `ServiceContainer`.
3. Runs `MainForm`. The form's menu handlers (`OnStartCapture`, `OnShowWaveform`, etc.) are currently TODO stubs - the capture/render services are not yet wired to the UI.

### ServiceContainer (`src/Configuration/ServiceContainer.cs`)
A deliberately minimal DI container:
- `Register<T>(instance)` stores a singleton.
- `RegisterFactory<T>(Func<ServiceContainer,T>)` stores a factory; **the first `Resolve<T>()` caches the created instance**, so factory registrations are effectively lazy singletons, not transients.
- `Resolve<T>()` returns `null` for unknown types (no exception), so callers must null-check.
- `Dispose()` disposes every cached `IDisposable` service and clears registrations.

`ApplicationConfiguration.ConfigureServices()` registers: `AudioSessionRepository` and `VisualizationDataRepository` as instances; `AudioCaptureService`, `WaveformService`, `SpectrumAnalyzer`, `SpectrogramAnalyzer` as factories. The `ApplicationSettings` overload currently applies no extra configuration beyond the defaults.

### AudioCaptureService (`src/Services/AudioCaptureService.cs`)
- Wraps NAudio `WaveInEvent`, 16-bit PCM, 1-2 channels.
- `Initialize(deviceIndex, sampleRate, channelCount)` validates arguments, creates the wave input, an `AudioBuffer` sized to ~2 seconds of samples, and an `AudioMetadata` record.
- `OnDataAvailable` (NAudio callback thread) converts the byte buffer to normalized `float` samples (`short / 32768f`), writes them to the circular `AudioBuffer`, updates metadata and RMS level, then raises `FrameCaptured` **synchronously on the callback thread**.
- `DeviceStatusChanged` is raised on recording errors.
- Device enumeration (`GetAvailableDevices`) reads `WaveInEvent.DeviceCount`/`GetCapabilities`; supported sample rates are the hard-coded list `{44100, 48000, 96000, 192000}` (not queried from the driver).

### AudioBuffer (`src/Domain/Models/AudioBuffer.cs`)
Lock-protected circular `float[]` buffer. Writes overwrite the oldest data when full (read position advances). `Read(count, out actualRead)` zero-pads on underrun to avoid clicks; `Peek`/`GetAll` are non-destructive. Allocates a new array per read - it is not a zero-allocation/object-pool design.

### Analysis services
- **WaveformService** - pure functions over `float[]`: averaging downsample, peak extraction, moving-average smoothing, per-segment RMS energy, zero-crossing count. Produces `WaveformData`.
- **SpectrumAnalyzer** - Hann window + NAudio `FastFourierTransform`, returns `SpectrumData` with `fftSize/2` linear magnitude bins and matching frequency bins; optional in-place dB conversion, smoothing, and peak-hold with configurable dB/s decay. FFT sizes outside `[AudioConstants.FFT_MINIMUM, FFT_MAXIMUM]` are silently replaced with the 2048 default.
- **SpectrogramAnalyzer** - rolling time-frequency analysis building `SpectrogramData`, with log scaling, normalization, frequency/time slicing, spectral flux and transient detection.
- **MidiInputService** - NAudio MIDI input wrapper raising `NoteReceived` events.

### EventBus (`src/Events/EventBus.cs`)
Type-keyed pub/sub with **weak references** to handler targets, so a subscriber being garbage-collected does not leak; dead subscriptions are pruned on publish. `Subscribe` returns an `IDisposable` for explicit unsubscription. Publishing happens outside the lock; subscriber exceptions are swallowed (Debug-logged) so one bad handler cannot break the rest. Note: because handlers are held weakly, a lambda whose target is not referenced elsewhere can be collected and silently stop receiving events - keep the returned subscription (or the target) alive.

### Repositories (`src/Data/Repositories/`)
Both are **in-memory only** (Dictionary + lock); nothing is persisted to disk.
- `AudioSessionRepository`: session lifecycle (`CreateSession`/`EndSession`/`DeleteSession`), per-session frame storage with a max-frames-per-session cap (oldest frame dropped on overflow), time-range and recent-frame queries, aggregate stats.
- `VisualizationDataRepository`: stores `VisualizationData` by id with queries by session/type, `GetMostRecent`, `PruneOldest(keepCount)`, and `GetStats()`. Pruning is manual - there is no background auto-pruning timer.

### CacheManager (`src/Caching/CacheManager.cs`)
Generic lock-based cache with per-entry TTL (default 1 h) and capacity-based eviction of the least-recently-accessed entry when `Count > maxSize`. Expired entries are removed lazily on access or via `RemoveExpiredEntries()`. `GetStatistics()` reports `CurrentSize`, `MaxSize`, `FillPercentage` only (no hit-rate or byte-size tracking).

### AudioProcessingWorker (`src/Workers/AudioProcessingWorker.cs`)
Background worker that dequeues `ProcessingTask` items and executes them with error/completion callbacks.

## Data Flow

Intended pipeline (the services exist and are unit-tested; the WinForms UI wiring is still TODO):

```
[Audio device]
     |  NAudio WaveInEvent callback (16-bit PCM bytes)
     v
AudioCaptureService.OnDataAvailable
     |  bytes -> float[-1,1] samples -> AudioBuffer.Write
     |  RMS + metadata update
     v
FrameCaptured event (synchronous, callback thread)
     |
     +--> WaveformService.GenerateWaveform      -> WaveformData
     +--> SpectrumAnalyzer.AnalyzeSpectrum      -> SpectrumData
     +--> SpectrogramAnalyzer                   -> SpectrogramData
     +--> AudioSessionRepository.AddFrameToSession (optional recording)
                    |
                    v
     VisualizationDataRepository / CacheManager
                    |
                    v
     SkiaSharp rendering in MainForm (not yet implemented)
```

## Key Design Decisions

1. **Hand-rolled DI instead of `Microsoft.Extensions.DependencyInjection`** - the app needs only singleton services; `ServiceContainer` avoids the dependency and keeps resolution trivial. Trade-off: no transient/scoped lifetimes, no constructor injection, and `Resolve` returning `null` pushes error handling to call sites.
2. **Weak-reference EventBus** - prevents the classic "forgot to unsubscribe" leak in long-lived audio apps. Trade-off: subscriptions can silently die if the handler target is collected, so callers must hold the returned `IDisposable`.
3. **Circular AudioBuffer with overwrite-on-full** - real-time capture must never block the NAudio callback; dropping the oldest audio is preferable to stalling. Trade-off: readers can miss data under sustained backpressure.
4. **Synchronous `FrameCaptured` on the capture thread** - lowest latency and no queue to manage. Trade-off: a slow subscriber delays capture; heavy work (large FFTs) should be offloaded, e.g. via `AudioProcessingWorker`.
5. **In-memory repositories with hard caps** - visualization data is transient; capping frames per session bounds memory without a persistence layer. Trade-off: nothing survives process exit.
6. **Fixed 16-bit capture format** - `ConvertBytesToFloatSamples` assumes 2 bytes/sample; this simplifies conversion at the cost of not supporting 24/32-bit or IEEE-float devices.

## Threading Model

- **NAudio callback thread**: `OnDataAvailable` -> buffer write -> `FrameCaptured` handlers.
- **UI thread**: WinForms message loop; any UI updates from `FrameCaptured` must be marshalled with `Invoke`/`BeginInvoke` (subscriber responsibility).
- **Thread safety**: `AudioBuffer`, both repositories, `CacheManager`, and `EventBus` each guard state with a private lock. `EventBus.Publish` invokes handlers outside its lock to avoid deadlocks.
- `AudioCaptureService.StartRecordingAsync` also spins a keep-alive `Task` polling a `CancellationTokenSource` every 50 ms; `StopRecordingAsync` cancels and awaits it.

## Extension Points

- **New analyzers**: subscribe to `AudioCaptureService.FrameCaptured` (there is no `IAnalyzer` interface - analyzers are plain classes) and register the instance in `ServiceContainer` if it should be shared.
- **New event types**: define a class and use `EventBus.Subscribe<T>` / `Publish<T>`; no registration needed.
- **Rendering**: `VisualizationData` plus the `ColorScheme`/`GradientStop` models are renderer-agnostic; a SkiaSharp renderer would consume them inside `MainForm`'s `MainVisualizationPanel`.
- **Settings**: `ConfigurationManager` persists arbitrary key-value settings to a text file; add keys rather than new config classes for simple options.

## Known Limitations

- The WinForms UI is a shell: capture start/stop, device dialog, and all three visualization views are TODO stubs in `Program.cs`.
- Windows-only (`net10.0-windows`, WinForms, WaveInEvent); the Dockerfile/Makefile cannot produce a runnable GUI in a Linux container.
- Capture supports only 16-bit PCM, 1-2 channels; the device sample-rate support list is hard-coded, not queried from the driver.
- No persistence: sessions, visualization data, and cache contents are lost on exit.
- `ServiceContainer.Resolve` returns `null` instead of throwing for unregistered types.
- `MainForm.Dispose` and `Program.Main` both dispose the shared `ServiceContainer` (harmless today because `Dispose` clears its state, but the double ownership is fragile).
