using System;
using System.Threading.Tasks;

namespace NAudioVisualizer.Events;

/// <summary>
/// Provides asynchronous extension methods for <see cref="EventBus"/>.
/// </summary>
public static class EventBusAsyncExtensions
{
    /// <summary>
    /// Publishes an event asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of event to publish.</typeparam>
    /// <param name="bus">The event bus. Cannot be <see langword="null"/>.</param>
    /// <param name="eventData">The event data to publish. Cannot be <see langword="null"/>.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="bus"/> or <paramref name="eventData"/> is <see langword="null"/>.
    /// </exception>
    public static Task PublishAsync<T>(this EventBus bus, T eventData) where T : class
    {
        ArgumentNullException.ThrowIfNull(bus);
        ArgumentNullException.ThrowIfNull(eventData);

        return Task.Run(() => bus.Publish(eventData));
    }
}
