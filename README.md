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

```csharp
// src/README.md
```