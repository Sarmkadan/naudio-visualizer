// src/README.md

// ... existing content ...

## ServiceContainer

`ServiceContainer` is a utility class for managing dependencies and services in the application. It provides methods for registering services, resolving instances, and checking registration status.

### Usage Example

```csharp
using Configuration;

var container = new ServiceContainer();
container.Register<AudioBufferStats>();
container.RegisterFactory<AudioFrame>();
var stats = container.Resolve<AudioBufferStats>();
var frame = container.Resolve<AudioFrame>();
Console.WriteLine($"Stats: {stats}");
Console.WriteLine($"Frame: {frame}");
```

## PathUtilityValidation

`PathUtilityValidation` is a utility class for validating and normalizing file paths. It provides static methods for common path operations with built-in validation, returning results as `IReadOnlyList<string>` for batch operations and exposing `IsValid`/`EnsureValid` for validation state checks.

### Usage Example

```csharp
using Utilities;

// Normalize a path and validate
var normalizedPaths = PathUtilityValidation.ValidateNormalizePath(new[] { "C:/some/path", "relative/path" });
PathUtilityValidation.EnsureValid(); // Throws if any validation failed

// Combine paths and validate
var combinedPaths = PathUtilityValidation.ValidateCombine(new[] { "C:/base", "subdir", "file.txt" });
if (PathUtilityValidation.IsValid)
{
    Console.WriteLine($"Combined paths: {string.Join(", ", combinedPaths)}");
}
else
{
    Console.WriteLine("Path validation failed.");
}
```

## WaveformService

`WaveformService` generates and processes waveform data from audio samples... (existing content remains unchanged)

## AudioSessionRepository

`AudioSessionRepository` is a repository class responsible for managing audio sessions. It provides methods for creating, retrieving, and manipulating sessions, as well as tracking session statistics.

### Usage Example

```csharp
using Data.Repositories;

var repository = new AudioSessionRepository();
var session = repository.CreateSession();
repository.AddFrameToSession(session.Id, new AudioFrame());
var frames = repository.GetSessionFrames(session.Id);
repository.EndSession(session.Id);
```

## SpectrogramAnalyzer

`SpectrogramAnalyzer` provides a thread‑safe, rolling buffer for spectrum frames and utilities to build, query, and process spectrogram data such as logarithmic scaling, normalization, frequency‑slice extraction, and transient detection.

### Usage Example

```csharp
using NAudioVisualizer.Services;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Constants;

// Create the analyzer and configure the buffer
var analyzer = new SpectrogramAnalyzer();
analyzer.SetBufferSize(200);

// Build a spectrogram from an array of audio frames
AudioFrame[] frames = GetAudioFrames(); // assume this returns populated frames
SpectrogramData spectrogram = analyzer.BuildSpectrogram(frames);

// Add a single spectrum frame (e.g., from a real‑time callback)
SpectrumData spectrum = null!; // replace with an actual SpectrumData instance
analyzer.AddSpectrumFrame(spectrum);

// Retrieve the current rolling spectrogram
SpectrogramData? current = analyzer.GetCurrentSpectrogram();
if (current != null)
{
    // Apply perceptual scaling and normalization
    analyzer.ApplyLogScaling(current);
    analyzer.NormalizeSpectrogram(current);

    // Query a frequency slice at 440 Hz
    float[] freqSlice = analyzer.GetFrequencySlice(current, 440f);

    // Query a time slice at 1.5 seconds
    float[] timeSlice = analyzer.GetTimeSlice(current, 1.5);

    // Compute spectral flux and detect transients
    float[] flux = analyzer.CalculateSpectralFlux(current);
    List<int> transients = analyzer.DetectTransients(current, 0.6f);
}
```

## ConfigurationManager

`ConfigurationManager` provides centralized configuration management for the application, allowing you to store, retrieve, and manage application settings with type safety. It supports loading from and saving to persistent storage, resetting to defaults, and exporting/importing configurations for backup or sharing purposes.

### Usage Example

```csharp
using NAudioVisualizer.Configuration;

// Create a configuration manager instance
var configManager = new ConfigurationManager();

// Load settings from the default configuration file
configManager.LoadSettings();

// Check if a setting exists
if (configManager.Contains("AudioBufferSize"))
{
    // Get a typed configuration value
    int bufferSize = configManager.GetValue<int>("AudioBufferSize");
    Console.WriteLine($"Current buffer size: {bufferSize}");
}

// Set a configuration value
configManager.SetValue("AudioBufferSize", 8192);

// Set a nullable configuration value
configManager.SetValue<int?>("CustomThreshold", null);

// Get all configuration keys
foreach (string key in configManager.GetAllKeys())
{
    Console.WriteLine($"Config key: {key}");
}

// Check if a setting exists
bool hasSetting = configManager.Contains("SampleRate");

// Remove a configuration value
configManager.Remove("TempSetting");

// Reset all settings to their default values
configManager.ResetToDefaults();

// Get a summary of the current configuration
string summary = configManager.GetConfigurationSummary();
Console.WriteLine(summary);

// Save the current configuration to file
configManager.SaveSettings();

// Export settings to a custom file
configManager.ExportSettings("backup-config.json");

// Import settings from a file
configManager.ImportSettings("restore-config.json");
```

## CacheManager

`CacheManager<TKey, TValue>` is a generic LRU (Least Recently Used) cache implementation that provides automatic expiration, thread-safe operations, and memory management for cached items. It supports customizable cache size limits, default expiration times, and comprehensive statistics tracking.

### Usage Example

```csharp
using NAudioVisualizer.Caching;
using System;

// Create a cache manager with a maximum size of 500 items and 2-hour default expiration
var cache = new CacheManager<string, byte[]>(
    maxSize: 500,
    defaultExpiration: TimeSpan.FromHours(2)
);

// Add items to the cache
cache.Set("audio-buffer-1", audioData1);
cache.Set("audio-buffer-2", audioData2);
cache.Set("audio-buffer-3", audioData3, TimeSpan.FromMinutes(30)); // Custom expiration

// Check if a key exists
bool hasKey = cache.Contains("audio-buffer-1");
Console.WriteLine($"Contains audio-buffer-1: {hasKey}");

// Try to get a value
if (cache.TryGetValue("audio-buffer-2", out var cachedData))
{
    Console.WriteLine($"Retrieved {cachedData.Length} bytes from cache");
}

// Get a value with a default fallback
byte[] defaultData = new byte[1024];
byte[] data = cache.GetOrDefault("missing-key", defaultData);
Console.WriteLine($"Retrieved {data.Length} bytes (default fallback)");

// Remove an item
bool removed = cache.Remove("audio-buffer-1");
Console.WriteLine($"Item removed: {removed}");

// Get cache statistics
var stats = cache.GetStatistics();
Console.WriteLine($"Cache size: {stats.CurrentSize}/{stats.MaxSize} ({stats.FillPercentage:F1}% full)");

// Remove all expired entries
int expiredCount = cache.RemoveExpiredEntries();
Console.WriteLine($"Removed {expiredCount} expired entries");

// Clear the entire cache
cache.Clear();
Console.WriteLine("Cache cleared");
```

## AudioProcessingWorker

`AudioProcessingWorker` is a background worker that processes audio frames asynchronously on a dedicated thread to avoid blocking the UI. It maintains an internal queue of processing tasks and executes them sequentially, providing thread-safe operations for task management and graceful start/stop functionality.

### Usage Example

```csharp
using NAudioVisualizer.Workers;
using NAudioVisualizer.Infrastructure;

// Create a logger and processing worker
var logger = new Logger();
var worker = new AudioProcessingWorker(logger);

// Start the worker
worker.Start();

// Create and enqueue a processing task
var task = new AudioProcessingWorker.ProcessingTask
{
    Name = "Real-time FFT Analysis",
    ExecuteAsync = async ct =>
    {
        // Your audio processing logic here
        await Task.Delay(100, ct);
        Console.WriteLine("Processing task executed");
    },
    OnComplete = () => Console.WriteLine("Task completed successfully"),
    OnError = ex => Console.WriteLine($"Task failed: {ex.Message}")
};

worker.EnqueueTask(task);

// Check queue depth
int queueDepth = worker.GetQueueDepth();
Console.WriteLine($"Queue depth: {queueDepth}");

// Clear the queue when needed
int clearedCount = worker.ClearQueue();
Console.WriteLine($"Cleared {clearedCount} tasks from queue");

// Stop the worker gracefully
await worker.StopAsync();

// Dispose when done
worker.Dispose();
```

## MathUtility

`MathUtility` is a static utility class providing mathematical functions for audio processing and signal analysis. It includes frequency conversions (MIDI note ↔ Hz), decibel calculations, window functions (Hann, Hamming), and various mathematical utilities for signal processing tasks.

### Usage Example

```csharp
using NAudioVisualizer.Utilities;

// Convert between MIDI notes and frequencies
int midiNote = MathUtility.FrequencyToMidiNote(440f); // Returns 69 (A4)
float frequency = MathUtility.MidiNoteToFrequency(69); // Returns 440 Hz

// Convert between amplitude and decibels
float amplitude = 0.5f;
float db = MathUtility.AmplitudeToDb(amplitude); // ~-6 dB
float linearAmplitude = MathUtility.DbToAmplitude(db); // Returns ~0.5f

// Calculate signal metrics
float[] signal = new float[1024]; // Sample signal
float rms = MathUtility.CalculateRms(signal);
float peak = MathUtility.CalculatePeak(signal);

// Apply window functions
MathUtility.ApplyHannWindow(signal);
MathUtility.ApplyHammingWindow(signal);

// Power and logarithmic scaling
float logValue = MathUtility.LogScale(1000f); // Log10(1000) = 3
float powerValue = MathUtility.PowerScale(0.5f, 2.0f); // sqrt(0.5) ≈ 0.707

// Range mapping and interpolation
float mapped = MathUtility.MapRange(50f, 0f, 100f, 0f, 1f); // 0.5
float interpolated = MathUtility.Lerp(0f, 100f, 0.5f); // 50

// Power-of-2 utilities
int nextPower = MathUtility.NextPowerOf2(100); // 128
bool isPower = MathUtility.IsPowerOf2(128); // true

// Distance calculation
float dist = MathUtility.Distance(0f, 0f, 3f, 4f); // 5 (3-4-5 triangle)
```

## Architecture

The application is a Windows Forms executable layered into services (audio capture, waveform/FFT/spectrogram analysis, MIDI input), domain models, a weak-reference event bus, in-memory repositories, and a small hand-rolled DI container. See [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) for the component breakdown, data flow, design decisions, and known limitations.
