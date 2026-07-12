// src/README.md

// ... existing content ...

## WaveformService

`WaveformService` generates and processes waveform data from audio samples. It provides methods for downsampling, normalizing, and smoothing the waveform, as well as calculating peak values, frame energy, and zero crossings.

### Usage Example

```csharp
using Services;

// Create a waveform service instance
var waveformService = new WaveformService();

// Generate waveform data from samples
var waveform = waveformService.GenerateWaveform(samples);

// Downsample the waveform to reduce its resolution
var downsampledWaveform = waveformService.DownsampleSamples(waveform);

// Normalize the waveform to a specific range
waveformService.NormalizeWaveform(downsampledWaveform);

// Calculate peak values in the waveform
var peakValues = waveformService.CalculatePeakValues(downsampledWaveform);

// Apply a smoothing filter to the waveform
var smoothedWaveform = waveformService.ApplySmoothingFilter(downsampledWaveform);

// Calculate frame energy in the waveform
var frameEnergy = waveformService.CalculateFrameEnergy(smoothedWaveform);

// Count zero crossings in the waveform
var zeroCrossings = waveformService.CountZeroCrossings(smoothedWaveform);

Console.WriteLine($"Peak values: {string.Join(", ", peakValues)}");
Console.WriteLine($"Frame energy: {frameEnergy}");
Console.WriteLine($"Zero crossings: {zeroCrossings}");
```

## VisualizationDataRepository

`VisualizationDataRepository` provides a data storage and retrieval mechanism for visualization data. It allows you to store, retrieve, and manage visualization data by session, type, and other criteria.

### Usage Example

```csharp
using Data.Repositories;

// Create a visualization data repository instance
var visualizationDataRepository = new VisualizationDataRepository();

// Store a new visualization data entry
var visualizationData = new VisualizationData();
visualizationDataRepository.Store(visualizationData);

// Retrieve a visualization data entry by ID
var visualizationDataById = visualizationDataRepository.GetById(visualizationData.Id);

// Retrieve all visualization data entries for a specific session
var visualizationDataBySession = visualizationDataRepository.GetBySession(sessionId);

// Retrieve all visualization data entries of a specific type
var visualizationDataByType = visualizationDataRepository.GetByType(visualizationDataType);

// Get the most recent visualization data entry
var mostRecentVisualizationData = visualizationDataRepository.GetMostRecent();

// Get all visualization data entries
var allVisualizationData = visualizationDataRepository.GetAll();

// Delete a visualization data entry
visualizationDataRepository.Delete(visualizationData.Id);

// Delete all visualization data entries for a specific session
var deletedCount = visualizationDataRepository.DeleteBySession(sessionId);

// Clear all visualization data entries
visualizationDataRepository.Clear();

// Get the total count of visualization data entries
var totalCount = visualizationDataRepository.Count;

// Get repository statistics
var repositoryStats = visualizationDataRepository.GetStats();

// Prune the oldest visualization data entries
var prunedCount = visualizationDataRepository.PruneOldest();

// Get the total count of waveform, spectrum, and spectrogram data entries
var waveformCount = visualizationDataRepository.WaveformCount;
var spectrumCount = visualizationDataRepository.SpectrumCount;
var spectrogramCount = visualizationDataRepository.SpectrogramCount;

// Get the total count of sessions
var sessionCount = visualizationDataRepository.SessionCount;

// Get the oldest and newest entry dates
var oldestEntryDate = visualizationDataRepository.OldestEntry;
var newestEntryDate = visualizationDataRepository.NewestEntry;
```

// ... existing content ...
