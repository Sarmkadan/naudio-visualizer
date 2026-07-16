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

## Architecture

The application is a Windows Forms executable layered into services (audio capture, waveform/FFT/spectrogram analysis, MIDI input), domain models, a weak-reference event bus, in-memory repositories, and a small hand-rolled DI container. See [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) for the component breakdown, data flow, design decisions, and known limitations.
