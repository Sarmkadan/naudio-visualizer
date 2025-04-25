using System;

namespace NAudioVisualizer.Events
{
    /// <summary>
    /// Provides extension methods for <see cref="EventBus"/> to simplify common event bus operations.
    /// </summary>
    public static class EventBusExtensions
    {
        /// <summary>
        /// Determines whether there are any subscribers for the specified event type.
        /// </summary>
        /// <typeparam name="T">The type of event.</typeparam>
        /// <param name="bus">The event bus. Cannot be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if there are subscribers; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bus"/> is <see langword="null"/>.</exception>
        public static bool HasSubscribers<T>(this EventBus bus) where T : class
        {
            ArgumentNullException.ThrowIfNull(bus);
            return bus.GetSubscriberCount<T>() > 0;
        }

        /// <summary>
        /// Publishes the event only if there are subscribers for the specified event type.
        /// </summary>
        /// <typeparam name="T">The type of event.</typeparam>
        /// <param name="bus">The event bus. Cannot be <see langword="null"/>.</param>
        /// <param name="event">The event to publish. Cannot be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="bus"/> or <paramref name="event"/> is <see langword="null"/>.</exception>
        public static void PublishIfSubscribed<T>(this EventBus bus, T @event) where T : class
        {
            ArgumentNullException.ThrowIfNull(bus);
            ArgumentNullException.ThrowIfNull(@event);

            if (bus.HasSubscribers<T>())
            {
                bus.Publish(@event);
            }
        }

        /// <summary>
        /// Unsubscribes all handlers for the specified event type and then publishes the event.
        /// </summary>
        /// <typeparam name="T">The type of event.</typeparam>
        /// <param name="bus">The event bus. Cannot be <see langword="null"/>.</param>
        /// <param name="event">The event to publish after unsubscribing. Cannot be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="bus"/> or <paramref name="event"/> is <see langword="null"/>.</exception>
        public static void UnsubscribeAllAndPublish<T>(this EventBus bus, T @event) where T : class
        {
            ArgumentNullException.ThrowIfNull(bus);
            ArgumentNullException.ThrowIfNull(@event);

            bus.UnsubscribeAll<T>();
            bus.Publish(@event);
        }
    }
}