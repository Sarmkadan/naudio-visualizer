// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using NAudioVisualizer.Events;
using NAudioVisualizer.Infrastructure;
using NAudioVisualizer.VST;

namespace NAudioVisualizer.Configuration;

/// <summary>
/// Extension methods for <see cref="ServiceContainer"/> that register the VST plugin host,
/// parameter automation engine, and preset management services as lazily-initialised singletons.
/// </summary>
public static class VstServiceExtensions
{
    /// <summary>
    /// Registers <see cref="VstPluginHostService"/>, <see cref="VstParameterAutomation"/>,
    /// and <see cref="VstPresetManager"/> in the container.
    /// </summary>
    /// <param name="container">The service container to configure.</param>
    /// <param name="options">
    /// Optional host configuration. Defaults are applied when <see langword="null"/>.
    /// </param>
    /// <returns>
    /// The same <paramref name="container"/> instance to support method chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="container"/> is <see langword="null"/>.
    /// </exception>
    public static ServiceContainer AddVstServices(
        this ServiceContainer container,
        VstHostOptions? options = null)
    {
        if (container is null)
            throw new ArgumentNullException(nameof(container));

        options ??= new VstHostOptions();

        container.RegisterFactory<VstPluginHostService>(c =>
        {
            var logger = c.Resolve<Logger>() ?? new Logger();
            var host   = new VstPluginHostService(logger);
            host.ConfigureAudioFormat(options.SampleRate, options.BlockSize);
            return host;
        });

        container.RegisterFactory<VstParameterAutomation>(c =>
        {
            var logger = c.Resolve<Logger>() ?? new Logger();
            return new VstParameterAutomation(logger);
        });

        container.RegisterFactory<VstPresetManager>(c =>
        {
            var logger = c.Resolve<Logger>() ?? new Logger();
            return new VstPresetManager(logger);
        });

        return container;
    }
}

// ── Host configuration options ─────────────────────────────────────────────────

/// <summary>
/// Configuration options forwarded to the VST services at construction time.
/// Passed to <see cref="VstServiceExtensions.AddVstServices"/>.
/// </summary>
public sealed class VstHostOptions
{
    /// <summary>
    /// Audio sample rate in Hz used for plugin initialisation (default: 44 100 Hz).
    /// </summary>
    public int SampleRate { get; set; } = 44100;

    /// <summary>
    /// Maximum audio block size in samples forwarded to each plugin (default: 512).
    /// </summary>
    public int BlockSize { get; set; } = 512;

    /// <summary>
    /// Directory scanned for <c>.vstpreset</c> files when the preset manager is first
    /// resolved. An empty string disables automatic file loading.
    /// </summary>
    public string DefaultPresetDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Maximum number of plugins permitted in the processing chain simultaneously.
    /// Zero means no enforced limit.
    /// </summary>
    public int MaxChainLength { get; set; } = 0;

    /// <summary>
    /// When <see langword="true"/> (default), plugins are activated immediately after
    /// loading without needing an explicit call to <see cref="VstPluginHostService.StartProcessing"/>.
    /// </summary>
    public bool AutoStartProcessing { get; set; } = true;

    /// <summary>
    /// Returns <see langword="true"/> when all fields hold consistent values.
    /// </summary>
    public bool IsValid() => SampleRate > 0 && BlockSize > 0;
}

// ── VST event publisher facade ─────────────────────────────────────────────────

/// <summary>
/// Convenience facade for publishing and subscribing to VST-related events on the
/// global <see cref="EventPublisher"/> bus without holding a direct reference to
/// the event bus instance.
/// </summary>
public static class VstEventPublisher
{
    /// <summary>
    /// Publishes a <see cref="VstPluginLoadedEvent"/> to all current subscribers.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="evt"/> is <see langword="null"/>.</exception>
    public static void PublishPluginLoaded(VstPluginLoadedEvent evt)
    {
        if (evt is null) throw new ArgumentNullException(nameof(evt));
        EventPublisher.Instance.Publish(evt);
    }

    /// <summary>
    /// Publishes a <see cref="VstPluginUnloadedEvent"/> to all current subscribers.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="evt"/> is <see langword="null"/>.</exception>
    public static void PublishPluginUnloaded(VstPluginUnloadedEvent evt)
    {
        if (evt is null) throw new ArgumentNullException(nameof(evt));
        EventPublisher.Instance.Publish(evt);
    }

    /// <summary>
    /// Publishes a <see cref="VstPresetAppliedEvent"/> to all current subscribers.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="evt"/> is <see langword="null"/>.</exception>
    public static void PublishPresetApplied(VstPresetAppliedEvent evt)
    {
        if (evt is null) throw new ArgumentNullException(nameof(evt));
        EventPublisher.Instance.Publish(evt);
    }

    /// <summary>
    /// Publishes a <see cref="VstPresetSavedEvent"/> to all current subscribers.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="evt"/> is <see langword="null"/>.</exception>
    public static void PublishPresetSaved(VstPresetSavedEvent evt)
    {
        if (evt is null) throw new ArgumentNullException(nameof(evt));
        EventPublisher.Instance.Publish(evt);
    }

    /// <summary>
    /// Publishes a <see cref="VstAutomationRecordingStartedEvent"/> to all current subscribers.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="evt"/> is <see langword="null"/>.</exception>
    public static void PublishAutomationRecordingStarted(VstAutomationRecordingStartedEvent evt)
    {
        if (evt is null) throw new ArgumentNullException(nameof(evt));
        EventPublisher.Instance.Publish(evt);
    }

    /// <summary>
    /// Publishes a <see cref="VstAutomationPlaybackStartedEvent"/> to all current subscribers.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="evt"/> is <see langword="null"/>.</exception>
    public static void PublishAutomationPlaybackStarted(VstAutomationPlaybackStartedEvent evt)
    {
        if (evt is null) throw new ArgumentNullException(nameof(evt));
        EventPublisher.Instance.Publish(evt);
    }

    // ── Subscription helpers ───────────────────────────────────────────────

    /// <summary>
    /// Subscribes to <see cref="VstPluginLoadedEvent"/> notifications.
    /// Dispose the returned token to cancel the subscription.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="handler"/> is <see langword="null"/>.</exception>
    public static IDisposable SubscribePluginLoaded(Action<VstPluginLoadedEvent> handler)
    {
        if (handler is null) throw new ArgumentNullException(nameof(handler));
        return EventPublisher.Subscribe(handler);
    }

    /// <summary>
    /// Subscribes to <see cref="VstPluginUnloadedEvent"/> notifications.
    /// Dispose the returned token to cancel the subscription.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="handler"/> is <see langword="null"/>.</exception>
    public static IDisposable SubscribePluginUnloaded(Action<VstPluginUnloadedEvent> handler)
    {
        if (handler is null) throw new ArgumentNullException(nameof(handler));
        return EventPublisher.Subscribe(handler);
    }

    /// <summary>
    /// Subscribes to <see cref="VstPresetAppliedEvent"/> notifications.
    /// Dispose the returned token to cancel the subscription.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="handler"/> is <see langword="null"/>.</exception>
    public static IDisposable SubscribePresetApplied(Action<VstPresetAppliedEvent> handler)
    {
        if (handler is null) throw new ArgumentNullException(nameof(handler));
        return EventPublisher.Subscribe(handler);
    }

    /// <summary>
    /// Subscribes to <see cref="VstAutomationPlaybackStartedEvent"/> notifications.
    /// Dispose the returned token to cancel the subscription.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="handler"/> is <see langword="null"/>.</exception>
    public static IDisposable SubscribeAutomationPlaybackStarted(
        Action<VstAutomationPlaybackStartedEvent> handler)
    {
        if (handler is null) throw new ArgumentNullException(nameof(handler));
        return EventPublisher.Subscribe(handler);
    }

    /// <summary>
    /// Subscribes to <see cref="VstAutomationRecordingStartedEvent"/> notifications.
    /// Dispose the returned token to cancel the subscription.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="handler"/> is <see langword="null"/>.</exception>
    public static IDisposable SubscribeAutomationRecordingStarted(
        Action<VstAutomationRecordingStartedEvent> handler)
    {
        if (handler is null) throw new ArgumentNullException(nameof(handler));
        return EventPublisher.Subscribe(handler);
    }
}
