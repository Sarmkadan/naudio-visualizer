#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.Events;

/// <summary>
/// Central event bus for the application using pub-sub pattern.
/// Decouples event publishers from subscribers for better architecture.
/// Thread-safe implementation with weak event support to prevent memory leaks.
/// </summary>
public sealed class EventBus : IDisposable
{
    private readonly Dictionary<Type, List<WeakEventSubscription>> _subscriptions;
    private readonly object _lockObject = new();
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the event bus.
    /// </summary>
    public EventBus()
    {
        _subscriptions = new Dictionary<Type, List<WeakEventSubscription>>();
    }

    /// <summary>
    /// Subscribes to events of a specific type.
    /// </summary>
    public IDisposable Subscribe<T>(Action<T> handler) where T : class
    {
        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        lock (_lockObject)
        {
            var eventType = typeof(T);

            if (!_subscriptions.TryGetValue(eventType, out var handlers))
            {
                handlers = new List<WeakEventSubscription>();
                _subscriptions[eventType] = handlers;
            }

            var subscription = new WeakEventSubscription(handler, this, eventType);
            handlers.Add(subscription);

            return subscription;
        }
    }

    /// <summary>
    /// Publishes an event to all subscribers.
    /// </summary>
    public void Publish<T>(T eventData) where T : class
    {
        if (eventData is null)
            throw new ArgumentNullException(nameof(eventData));

        List<WeakEventSubscription>? handlers;

        lock (_lockObject)
        {
            if (!_subscriptions.TryGetValue(typeof(T), out var allHandlers))
                return;

            handlers = new List<WeakEventSubscription>(allHandlers);
        }

        // Publish outside the lock to prevent deadlocks
        var deadSubscriptions = new List<WeakEventSubscription>();

        foreach (var subscription in handlers)
        {
            if (subscription.IsAlive)
            {
                try
                {
                    subscription.Handle(eventData);
                }
                catch (Exception ex)
                {
                    // Log but don't throw - allow other subscribers to process
                    System.Diagnostics.Debug.WriteLine($"Error in event subscriber: {ex.Message}");
                }
            }
            else
            {
                deadSubscriptions.Add(subscription);
            }
        }

        // Clean up dead subscriptions
        if (deadSubscriptions.Count > 0)
        {
            lock (_lockObject)
            {
                if (_subscriptions.TryGetValue(typeof(T), out var allHandlers))
                {
                    foreach (var dead in deadSubscriptions)
                    {
                        allHandlers.Remove(dead);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the number of subscribers for a specific event type.
    /// </summary>
    public int GetSubscriberCount<T>() where T : class
    {
        lock (_lockObject)
        {
            return _subscriptions.TryGetValue(typeof(T), out var handlers)
                ? handlers.Count
                : 0;
        }
    }

    /// <summary>
    /// Unsubscribes all handlers for a specific event type.
    /// </summary>
    public void UnsubscribeAll<T>() where T : class
    {
        lock (_lockObject)
        {
            _subscriptions.Remove(typeof(T));
        }
    }

    /// <summary>
    /// Clears all subscriptions.
    /// </summary>
    public void Clear()
    {
        lock (_lockObject)
        {
            _subscriptions.Clear();
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        Clear();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Internal class for managing weak event subscriptions.
    /// </summary>
    private sealed class WeakEventSubscription : IDisposable
    {
        private readonly WeakReference _handlerReference;
        private readonly EventBus _eventBus;
        private readonly Type _eventType;

        public bool IsAlive => _handlerReference.IsAlive;

        public WeakEventSubscription(Delegate handler, EventBus eventBus, Type eventType)
        {
            _handlerReference = new WeakReference(handler);
            _eventBus = eventBus;
            _eventType = eventType;
        }

        public void Handle(object eventData)
        {
            if (_handlerReference.Target is Delegate handler)
            {
                handler.DynamicInvoke(eventData);
            }
        }

        public void Dispose() => _eventBus.Unsubscribe(this, _eventType);
    }

    /// <summary>
    /// Unsubscribe a specific weak subscription.
    /// </summary>
    private void Unsubscribe(WeakEventSubscription subscription, Type eventType)
    {
        lock (_lockObject)
        {
            if (_subscriptions.TryGetValue(eventType, out var handlers))
            {
                handlers.Remove(subscription);
            }
        }
    }
}