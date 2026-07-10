using System;

namespace NAudioVisualizer.Events
{
    public static class EventBusExtensions
    {
        /// <summary>
        /// Determines whether there are any subscribers for the specified event type.
        /// </summary>
        /// <typeparam name="T">The type of event.</typeparam>
        /// <param name="bus">The event bus.</param>
        /// <returns><c>true</c> if there are subscribers; otherwise, <c>false</c>.</returns>
        public static bool HasSubscribers<T>(this EventBus bus) where T : class
        {
            return bus.GetSubscriberCount<T>() > 0;
        }

        /// <summary>
        /// Publishes the event only if there are subscribers for the specified event type.
        /// </summary>
        /// <typeparam name="T">The type of event.</typeparam>
        /// <param name="bus">The event bus.</param>
        /// <param name="event">The event to publish.</param>
        public static void PublishIfSubscribed<T>(this EventBus bus, T @event) where T : class
        {
            if (bus.HasSubscribers<T>())
            {
                bus.Publish(@event);
            }
        }

        /// <summary>
        /// Unsubscribes all handlers for the specified event type and then publishes the event.
        /// </summary>
        /// <typeparam name="T">The type of event.</typeparam>
        /// <param name="bus">The event bus.</param>
        /// <param name="event">The event to publish after unsubscribing.</param>
        public static void UnsubscribeAllAndPublish<T>(this EventBus bus, T @event) where T : class
        {
            bus.UnsubscribeAll<T>();
            bus.Publish(@event);
        }
    }
}
