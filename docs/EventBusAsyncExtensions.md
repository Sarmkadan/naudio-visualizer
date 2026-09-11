# EventBusAsyncExtensions

The `EventBusAsyncExtensions` class provides asynchronous extension methods for publishing events via the event bus.

## API

### Methods

*   **`public static Task PublishAsync<T>(this EventBus bus, T eventData) where T : class`**
    Publishes an event asynchronously using `Task.Run`.
    *   **Parameters**:
        *   `bus`: The event bus on which to publish.
        *   `eventData`: The event instance to publish.
    *   **Returns**: A task that represents the asynchronous publish operation.
    *   **Throws**: `ArgumentNullException` if `bus` or `eventData` is `null`.

## Usage

The following example demonstrates the asynchronous extension method with a single event type.

```csharp
using System;
using System.Threading.Tasks;
using NAudioVisualizer.Events;

public sealed class PlaybackStopped
{
    public PlaybackStopped(string reason)
    {
        Reason = reason;
    }

    public string Reason { get; }
}

var bus = new EventBus();
Action<PlaybackStopped> handler = message =>
    Console.WriteLine($"Playback stopped: {message.Reason}");

using IDisposable subscription = bus.Subscribe(handler);

// Publish asynchronously
await bus.PublishAsync(new PlaybackStopped("End of stream"));

// You can also fire-and-forget if you don't need to await
bus.PublishAsync(new PlaybackStopped("Device disconnected"));

Console.WriteLine(bus.HasSubscribers<PlaybackStopped>()); // True (if handler still subscribed)
```

## Notes

*   The method offloads the synchronous `Publish` call to a thread pool thread via `Task.Run`. Use it when you need to avoid blocking the calling thread.
*   Exceptions thrown by the synchronous `Publish` method are captured and placed on the returned task.
*   The generic type constraint `where T : class` ensures that only reference types can be used as event data.