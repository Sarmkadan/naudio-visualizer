# LoggerExtensions

The `LoggerExtensions` class provides a static utility surface for structured logging and performance measurement within the `naudio-visualizer` project. It encapsulates standard logging levels (Debug through Critical) and offers a mechanism for scoped timing via `MethodScope`, allowing developers to track execution duration and conditionally execute logic based on logging configuration without boilerplate code.

## API

### Debug
Writes a debug-level message to the configured logging backend.
*   **Parameters**: Accepts a message string and optional formatting arguments.
*   **Return Value**: `void`.
*   **Throws**: Throws an exception if the underlying logging provider fails to write or if format arguments do not match the message template.

### Info
Writes an informational-level message indicating normal application operation.
*   **Parameters**: Accepts a message string and optional formatting arguments.
*   **Return Value**: `void`.
*   **Throws**: Throws an exception if the underlying logging provider fails to write or if format arguments do not match the message template.

### Warn
Writes a warning-level message indicating a potential issue that does not halt execution.
*   **Parameters**: Accepts a message string and optional formatting arguments.
*   **Return Value**: `void`.
*   **Throws**: Throws an exception if the underlying logging provider fails to write or if format arguments do not match the message template.

### Error
Writes an error-level message indicating a functional failure that requires attention.
*   **Parameters**: Accepts a message string, an optional `Exception` object, and optional formatting arguments.
*   **Return Value**: `void`.
*   **Throws**: Throws an exception if the underlying logging provider fails to write.

### Critical
Writes a critical-level message indicating a severe failure requiring immediate intervention.
*   **Parameters**: Accepts a message string, an optional `Exception` object, and optional formatting arguments.
*   **Return Value**: `void`.
*   **Throws**: Throws an exception if the underlying logging provider fails to write.

### MethodScope
Initiates a timed scope for the current method execution.
*   **Parameters**: Accepts an optional method name string; if omitted, the caller member name is used automatically.
*   **Return Value**: Returns an `IDisposable` object (`MethodScopeDisposable`). Disposing this object finalizes the timing measurement and logs the elapsed duration.
*   **Throws**: Throws if the internal timer cannot be initialized.

### Time
Retrieves the current high-resolution timestamp used for performance calculations.
*   **Parameters**: None.
*   **Return Value**: Returns a `long` representing the current tick count or timestamp.
*   **Throws**: None.

### If
Evaluates whether a specific log level is enabled before executing expensive operations.
*   **Parameters**: Accepts a `LogLevel` enumeration value.
*   **Return Value**: Returns a `bool` indicating if the specified level is active.
*   **Throws**: None.

### MethodScopeDisposable
A private or internal struct/class implementing `IDisposable` used exclusively by `MethodScope` to manage the lifecycle of a timing operation. It is not intended for direct instantiation by consumers.
*   **Parameters**: N/A (Internal construction).
*   **Return Value**: N/A.
*   **Throws**: N/A.

### Dispose
Releases resources associated with the `MethodScopeDisposable` instance.
*   **Parameters**: None (Implicit via `using` statement).
*   **Return Value**: `void`.
*   **Throws**: May throw if the logging backend is unavailable during the disposal phase when attempting to write the elapsed time.

## Usage

### Conditional Logging with Performance Check
Use the `If` method to prevent unnecessary string allocation or computation when the target log level is disabled, and `MethodScope` to automatically log execution time.

```csharp
public void ProcessAudioBuffer(float[] buffer)
{
    using (LoggerExtensions.MethodScope())
    {
        if (LoggerExtensions.If(LogLevel.Debug))
        {
            // Only execute this expensive serialization if Debug is enabled
            var bufferSummary = SerializeBufferStats(buffer);
            LoggerExtensions.Debug("Processing buffer: {Summary}", bufferSummary);
        }

        // Core processing logic
        ApplyVisualizationTransform(buffer);
    }
}
```

### Error Handling with Contextual Timing
Combine `Error` logging with scoped timing to diagnose performance degradation during failure scenarios.

```csharp
public void LoadVisualizationSettings(string path)
{
    using (LoggerExtensions.MethodScope("LoadVisualizationSettings"))
    {
        try
        {
            var config = File.ReadAllText(path);
            ParseSettings(config);
            LoggerExtensions.Info("Settings loaded successfully from {Path}", path);
        }
        catch (Exception ex)
        {
            LoggerExtensions.Error(ex, "Failed to load settings from {Path}. Duration logged above.", path);
            throw;
        }
    }
}
```

## Notes

*   **Thread Safety**: The static logging methods (`Debug`, `Info`, `Warn`, `Error`, `Critical`) are designed to be thread-safe, allowing concurrent calls from multiple audio processing threads without external locking. However, the `MethodScope` pattern relies on the returned `IDisposable` instance being disposed on the same thread that created it to ensure accurate elapsed time calculation.
*   **Disposal Requirement**: The `MethodScope` method must be used within a `using` statement. Failure to dispose the returned `MethodScopeDisposable` will result in the elapsed time never being logged and may lead to resource leaks if the internal timer holds unmanaged handles.
*   **Exception Swallowing**: While the logging methods themselves may throw if the backend fails, the `Dispose` implementation of `MethodScopeDisposable` typically suppresses secondary exceptions during teardown to prevent masking the original exception in a `catch` block. Verify backend health if timing logs disappear silently.
*   **Level Evaluation**: The `If` method provides a fast path for checking log levels. It should be preferred over passing complex objects directly into logging methods when the construction of those objects is computationally expensive.
