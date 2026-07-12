// src/README.md

// ... existing content ...

## LoggerExtensions

`LoggerExtensions` provides a set of extension methods for logging and timing operations. It offers a simple way to log messages at different levels, measure execution time, and create a disposable scope for logging method execution.

### Usage Example

```csharp
using Infrastructure;

// Create a logger instance
var logger = new Logger();

// Log a debug message
LoggerExtensions.Debug(logger, "This is a debug message");

// Log an info message
LoggerExtensions.Info(logger, "This is an info message");

// Log a warning message
LoggerExtensions.Warn(logger, "This is a warning message");

// Log an error message
LoggerExtensions.Error(logger, "This is an error message");

// Log a critical message
LoggerExtensions.Critical(logger, "This is a critical message");

// Measure execution time
var startTime = LoggerExtensions.Time();
// ... some code ...
var endTime = LoggerExtensions.Time();
logger.Info($"Execution time: {endTime - startTime} ms");

// Create a disposable scope for logging method execution
using var scope = LoggerExtensions.MethodScope(logger);
// ... some code ...
logger.Info("Method executed successfully");

// Use the If method to log a message only if a condition is true
if (LoggerExtensions.If(logger, true))
{
    logger.Info("Condition is true");
}
else
{
    logger.Warn("Condition is false");
}
```

## AudioStreamExceptionExtensions

`AudioStreamExceptionExtensions` provides a set of extension methods for working with `AudioStreamException` instances. These methods allow you to get detailed error messages, determine if an exception is recoverable or fatal, and create new exceptions with custom messages.

### Usage Example

```csharp
try
{
    // ... code that might throw an AudioStreamException ...
}
catch (AudioStreamException ex)
{
    string detailedErrorMessage = AudioStreamExceptionExtensions.GetDetailedErrorMessage(ex);
    bool isRecoverable = AudioStreamExceptionExtensions.IsRecoverable(ex);
    bool isFatal = AudioStreamExceptionExtensions.IsFatal(ex);

    if (isRecoverable)
    {
        // Attempt to recover from the exception
    }
    else
    {
        // Handle the fatal exception
        var newException = AudioStreamExceptionExtensions.WithMessage(ex, "Custom error message");
        // ... handle or rethrow the new exception ...
    }

    string userFriendlyMessage = AudioStreamExceptionExtensions.GetUserFriendlyMessage(ex);
    // ... log or display the user-friendly message ...
}
```

## EventPublisherJsonExtensions

`EventPublisherJsonExtensions` adds JSON serialization helpers and subscriber statistics to an event publisher. It lets you convert a publisher's state to JSON, recreate a publisher from JSON, safely attempt deserialization, and inspect how many subscribers each event has as well as the total subscriber count.

### Usage Example

```csharp
using Events;

// Assume `publisher` is an instance of a class that implements the
// EventPublisherJsonExtensions members (e.g., EventPublisher).
var publisher = new EventPublisher();

// Serialize the publisher to JSON.
string json = EventPublisherJsonExtensions.ToJson(publisher);
Console.WriteLine($"Serialized JSON: {json}");

// Deserialize back to an object.
object? deserialized = EventPublisherJsonExtensions.FromJson(json);
Console.WriteLine($"Deserialized type: {deserialized?.GetType().Name ?? "null"}");

// Try to deserialize safely.
if (EventPublisherJsonExtensions.TryFromJson(json, out var safeResult))
{
    Console.WriteLine("TryFromJson succeeded.");
}
else
{
    Console.WriteLine("TryFromJson failed.");
}

// Access subscriber statistics.
Dictionary<string, int> counts = publisher.SubscriberCounts;
foreach (var kvp in counts)
{
    Console.WriteLine($"Event \"{kvp.Key}\" has {kvp.Value} subscriber(s).");
}

// Total number of subscribers across all events.
int total = publisher.TotalSubscribers;
Console.WriteLine($"Total subscribers: {total}");
```

// ... existing content ...
```​```