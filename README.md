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

## ServiceContainerJsonExtensions

`ServiceContainerJsonExtensions` provides helper methods for converting a `ServiceContainer` to and from JSON. It also exposes the lists of service and factory type names that have been registered, making it easy to inspect the container’s configuration.

### Usage Example

```csharp
using Configuration;

// Assume we have a ServiceContainer instance that has been populated
var container = new ServiceContainer();
container.Register<MyService>();
container.RegisterFactory<IMyFactory, MyFactory>();

// Serialize the container to a JSON string
string json = ServiceContainerJsonExtensions.ToJson(container);
Console.WriteLine($"Serialized container: {json}");

// Deserialize a container from JSON (may return null if the JSON is invalid)
ServiceContainer? deserialized = ServiceContainerJsonExtensions.FromJson(json);
if (deserialized != null)
{
    // Use the deserialized container
    Console.WriteLine("Container deserialized successfully.");
}

// Safely attempt to deserialize, receiving a bool result
if (ServiceContainerJsonExtensions.TryFromJson(json, out var safeContainer))
{
    Console.WriteLine("TryFromJson succeeded.");
}

// Inspect the registered service and factory type names
List<string>? serviceTypes = ServiceContainerJsonExtensions.RegisteredServiceTypes;
List<string>? factoryTypes = ServiceContainerJsonExtensions.RegisteredFactoryTypes;

Console.WriteLine($"Registered services: {string.Join(", ", serviceTypes ?? new List<string>())}");
Console.WriteLine($"Registered factories: {string.Join(", ", factoryTypes ?? new List<string>())}");
```

// ... existing content ...
