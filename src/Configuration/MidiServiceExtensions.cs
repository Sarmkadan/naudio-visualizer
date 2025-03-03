#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Events;
using NAudioVisualizer.Services;

namespace NAudioVisualizer.Configuration;

/// <summary>
/// Extension methods for <see cref="ServiceContainer"/> that register MIDI input services.
/// </summary>
public static class MidiServiceExtensions
{
    /// <summary>
    /// Registers <see cref="MidiInputService"/> as a lazily-initialised singleton in the container.
    /// </summary>
    /// <param name="container">The service container to configure.</param>
    /// <returns>The same <paramref name="container"/> instance to support method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/> is <see langword="null"/>.</exception>
    public static ServiceContainer AddMidiServices(this ServiceContainer container)
    {
        if (container is null)
            throw new ArgumentNullException(nameof(container));

        container.RegisterFactory<MidiInputService>(static _ => new MidiInputService());

        return container;
    }
}

/// <summary>
/// Convenience facade for publishing and subscribing to MIDI note events on the global event bus.
/// </summary>
public static class MidiEventPublisher
{
    /// <summary>
    /// Publishes a <see cref="MidiNoteEvent"/> to all current subscribers on the global event bus.
    /// </summary>
    /// <param name="note">The note event to broadcast.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="note"/> is <see langword="null"/>.</exception>
    public static void PublishNoteReceived(MidiNoteEvent note)
    {
        if (note is null)
            throw new ArgumentNullException(nameof(note));

        EventPublisher.Instance.Publish(note);
    }

    /// <summary>
    /// Subscribes to all <see cref="MidiNoteEvent"/> instances published on the global event bus.
    /// </summary>
    /// <param name="handler">The callback to invoke for each incoming note event.</param>
    /// <returns>
    /// An <see cref="IDisposable"/> that, when disposed, cancels the subscription and prevents
    /// further invocations of <paramref name="handler"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="handler"/> is <see langword="null"/>.</exception>
    public static IDisposable SubscribeNoteReceived(Action<MidiNoteEvent> handler)
    {
        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        return EventPublisher.Subscribe(handler);
    }
}
