// src/README.md

// ... existing content ...

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
