# EventBusExtensions

The `EventBusExtensions` class provides convenience extension methods for checking typed subscriptions, publishing only when a typed subscription exists, and removing all subscriptions for a type before publishing an event. All three methods require `T` to be a reference type (`class`).

## API

### Methods

*   **`public static bool HasSubscribers<T>(this EventBus bus) where T : class`**
    Returns `true` when `GetSubscriberCount<T>()` is greater than zero; otherwise, returns `false`.
    *   **Parameters**:
        *   `bus`: The event bus to inspect.
    *   **Throws**: `ArgumentNullException` if `bus` is `null`.

*   **`public static void PublishIfSubscribed<T>(this EventBus bus, T event) where T : class`**
    Publishes `event` only when `HasSubscribers<T>()` returns `true`. When there are no subscribers for `T`, the method returns without calling `Publish`.
    *   **Parameters**:
        *   `bus`: The event bus on which to publish.
        *   `event`: The event instance to publish.
    *   **Throws**: `ArgumentNullException` if `bus` or `event` is `null`. The event is validated even when there are no subscribers.

*   **`public static void UnsubscribeAllAndPublish<T>(this EventBus bus, T event) where T : class`**
    Calls `UnsubscribeAll<T>()` and then calls `Publish(event)`. Because existing subscriptions for `T` are removed before publication, those handlers do not receive the supplied event.
    *   **Parameters**:
        *   `bus`: The event bus to update and publish on.
        *   `event`: The event instance to publish after removing subscriptions.
    *   **Throws**: `ArgumentNullException` if `bus` or `event` is `null`. Both arguments are validated before subscriptions are removed.

## Usage

The following example demonstrates all three extension methods with a single event type.

```csharp
using System;
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

if (bus.HasSubscribers<PlaybackStopped>())
{
    bus.PublishIfSubscribed(new PlaybackStopped("End of stream"));
}

// Removes every PlaybackStopped subscription before publishing.
// The handler above does not receive this event.
bus.UnsubscribeAllAndPublish(new PlaybackStopped("Device disconnected"));

Console.WriteLine(bus.HasSubscribers<PlaybackStopped>()); // False
```

## Notes

*   Subscription checks are specific to the generic event type `T`; each method delegates to the corresponding generic `EventBus` operation for that type.
*   `PublishIfSubscribed<T>` performs the subscription check before calling `Publish`, so the check and publication are separate operations.
*   `UnsubscribeAllAndPublish<T>` performs unsubscription and publication as two separate operations in that order.
