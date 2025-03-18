# EventBus

The `EventBus` provides a lightweight, decoupled communication mechanism for the `naudio-visualizer` project based on the publish-subscribe pattern. It enables components to exchange messages by broadcasting them to typed subscribers without requiring direct knowledge of the publisher or subscriber implementations.

## API

### EventBus
*   `public EventBus()`: Initializes a new instance of the `EventBus` class.

### Methods
*   `public IDisposable Subscribe<T>(Action<T> action)`: Registers an action to be invoked when a message of type `T` is published. Returns an `IDisposable` object; disposing this object unsubscribes the associated action.
*   `public void Publish<T>(T message)`: Dispatches the provided message of type `T` to all active subscribers registered for that type.
*   `public int GetSubscriberCount<T>()`: Returns the total number of active subscribers for the specified message type `T`.
*   `public void UnsubscribeAll<T>()`: Removes all registered subscribers for the message type `T`.
*   `public void Clear()`: Unsubscribes all handlers from all message types, effectively resetting the bus.
*   `public void Dispose()`: Performs cleanup of the event bus, unsubscribing all current handlers and releasing resources.

### WeakEventSubscription
*   `public WeakEventSubscription`: A nested class utilized internally to manage a weak reference to a subscription handler, helping prevent strong reference cycles that could lead to memory leaks.
*   `public void Handle(T message)`: Invokes the action associated with this subscription if the target is still alive.
*   `public void Dispose()`: Releases the weak reference and removes the subscription.

## Usage

### Basic Publish and Subscribe
```csharp
var bus = new EventBus();

// Subscriber
bus.Subscribe<string>(msg => Console.WriteLine($"Received: {msg}"));

// Publisher
bus.Publish("Audio stream started.");
```

### Unsubscribing
```csharp
var bus = new EventBus();

// Subscription with manual unsubscription
IDisposable subscription = bus.Subscribe<int>(value => Console.WriteLine($"Value: {value}"));

bus.Publish(42);

// Unsubscribe
subscription.Dispose();

bus.Publish(100); // Will not trigger the action
```

## Notes

*   **Memory Management**: The `EventBus` utilizes `WeakEventSubscription` to maintain references to subscribers. This is designed to prevent the event bus from keeping objects alive longer than necessary, provided the subscriber does not hold a strong reference to the `IDisposable` returned by `Subscribe`.
*   **Thread Safety**: The `EventBus` implementation is not guaranteed to be thread-safe for concurrent modifications to the subscriber collection (e.g., subscribing while publishing from another thread). If the bus is accessed from multiple threads, ensure external synchronization is applied.
*   **Exception Handling**: Actions registered with `Subscribe` should manage their own internal exceptions. An exception thrown within a subscriber's action may interrupt the notification process for subsequent subscribers to the same message type.
