# ILogger

The `ILogger` interface provides the minimal logging contract used by the `naudio-visualizer` project. It exposes a configurable minimum severity and methods for debug, informational, warning, error, and critical messages. The `Logger` class is the built-in implementation; it writes timestamped entries to a file and, optionally, the console.

## API

### Property

*   **`LogLevel MinimumLevel { get; set; }`**
    Gets or sets the minimum severity to emit. The interface defines the property but does not prescribe how an implementation applies it. `Logger` defaults this property to `LogLevel.Info` and ignores messages whose level is lower than the current value.

### Methods

*   **`void Debug(string message)`**
    Logs a debug message.

*   **`void Info(string message)`**
    Logs an informational message.

*   **`void Warn(string message)`**
    Logs a warning message.

*   **`void Error(string message, Exception? exception = null)`**
    Logs an error message with an optional exception.

*   **`void Critical(string message, Exception? exception = null)`**
    Logs a critical message with an optional exception.

## LogLevel

`LogLevel` is declared in `Logger.cs`. Its ordered values are:

*   **`Debug = 0`**
*   **`Info = 1`**
*   **`Warn = 2`**
*   **`Error = 3`**
*   **`Critical = 4`**

`Logger` uses this numeric ordering when applying `MinimumLevel`. For example, a minimum level of `Warn` suppresses `Debug` and `Info` entries while allowing `Warn`, `Error`, and `Critical` entries.

## Logger Implementation

`Logger` is a sealed class that implements both `ILogger` and `IDisposable`.

*   **Construction**: `Logger(string? logFilePath = null, bool writeToConsole = true)` opens the log file in append mode with UTF-8 encoding and automatic flushing. When no path is supplied, the default is `logs/app.log` beneath `AppDomain.CurrentDomain.BaseDirectory`. A missing containing directory is created.
*   **Output**: Console output is enabled by default and can be disabled through `writeToConsole`. File output is always attempted during construction.
*   **File initialization failure**: Exceptions raised while creating the directory or writer are caught. File logging is then disabled, while enabled console logging can continue.
*   **Formatting**: Each emitted entry uses the format `[yyyy-MM-dd HH:mm:ss.fff] [Level] message`, based on `DateTime.Now`.
*   **Exceptions**: When an exception is supplied to `Error` or `Critical`, `Logger` appends its type name, message, and stack trace to the provided message. Passing no exception logs only the message.
*   **Synchronization**: Filtering, formatting, and writing are protected by an internal lock, as is disposal.
*   **Disposal**: `Dispose()` closes the file writer and suppresses finalization. Repeated disposal is ignored, and log calls made after disposal produce no output. `IDisposable` is a feature of `Logger`, not a requirement of the `ILogger` interface.

## Usage

```csharp
using NAudioVisualizer.Infrastructure;

using var logger = new Logger("logs/visualizer.log", writeToConsole: true)
{
    MinimumLevel = LogLevel.Debug
};

logger.Debug("Visualizer initialization started.");
logger.Info("Audio input connected.");

try
{
    StartVisualization();
}
catch (Exception exception)
{
    logger.Error("Visualization could not be started.", exception);
}
```

## Custom Implementation

An `ILogger` implementation can route messages to another destination. The following implementation stores entries in memory and applies the same ordered minimum-level comparison used by `Logger`:

```csharp
using System;
using System.Collections.Generic;
using NAudioVisualizer.Infrastructure;

public sealed class ListLogger : ILogger
{
    public LogLevel MinimumLevel { get; set; } = LogLevel.Info;

    public List<string> Entries { get; } = new List<string>();

    public void Debug(string message) => Add(LogLevel.Debug, message);
    public void Info(string message) => Add(LogLevel.Info, message);
    public void Warn(string message) => Add(LogLevel.Warn, message);
    public void Error(string message, Exception? exception = null) =>
        Add(LogLevel.Error, Format(message, exception));
    public void Critical(string message, Exception? exception = null) =>
        Add(LogLevel.Critical, Format(message, exception));

    private void Add(LogLevel level, string message)
    {
        if (level >= MinimumLevel)
        {
            Entries.Add($"[{level}] {message}");
        }
    }

    private static string Format(string message, Exception? exception) =>
        exception is null ? message : $"{message}: {exception.Message}";
}
```

Custom implementations must provide all five logging methods and the read/write `MinimumLevel` property. Output destinations, formatting, filtering, exception representation, synchronization, and resource ownership remain implementation choices because the interface does not define them.
