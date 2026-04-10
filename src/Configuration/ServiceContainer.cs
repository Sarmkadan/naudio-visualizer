#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using NAudioVisualizer.Data.Repositories;
using NAudioVisualizer.Services;

namespace NAudioVisualizer.Configuration;

/// <summary>
/// Simple dependency injection container for managing service instances.
/// </summary>
public class ServiceContainer
{
    private readonly Dictionary<Type, object> _services = [];
    private readonly Dictionary<Type, Func<ServiceContainer, object>> _factories = [];

    /// <summary>
    /// Registers a singleton service instance.
    /// </summary>
    public void Register<T>(T instance) where T : class
    {
        if (instance is null)
            throw new ArgumentNullException(nameof(instance));

        _services[typeof(T)] = instance;
    }

    /// <summary>
    /// Registers a factory for lazy creation of services.
    /// </summary>
    public void RegisterFactory<T>(Func<ServiceContainer, T> factory) where T : class
    {
        if (factory is null)
            throw new ArgumentNullException(nameof(factory));

        _factories[typeof(T)] = container => factory(container)!;
    }

    /// <summary>
    /// Resolves a service instance.
    /// </summary>
    public T? Resolve<T>() where T : class
    {
        var type = typeof(T);

        // Check for registered singleton
        if (_services.ContainsKey(type))
        {
            return _services[type] as T;
        }

        // Check for registered factory
        if (_factories.ContainsKey(type))
        {
            var instance = _factories[type](this);
            _services[type] = instance; // Cache the created instance
            return instance as T;
        }

        return null;
    }

    /// <summary>
    /// Checks if a service is registered.
    /// </summary>
    public bool IsRegistered<T>() where T : class
    {
        var type = typeof(T);
        return _services.ContainsKey(type) || _factories.ContainsKey(type);
    }

    /// <summary>
    /// Removes a registered service.
    /// </summary>
    public bool Unregister<T>() where T : class
    {
        var type = typeof(T);
        bool removed = _services.Remove(type);
        return _factories.Remove(type) || removed;
    }

    /// <summary>
    /// Clears all registered services.
    /// </summary>
    public void Clear()
    {
        _services.Clear();
        _factories.Clear();
    }

    /// <summary>
    /// Disposes all services that implement IDisposable.
    /// </summary>
    public void Dispose()
    {
        foreach (var service in _services.Values)
        {
            (service as IDisposable)?.Dispose();
        }
        Clear();
    }
}

/// <summary>
/// Configuration and setup for the application services.
/// </summary>
public class ApplicationConfiguration
{
    /// <summary>
    /// Configures all application services with default setup.
    /// </summary>
    public static ServiceContainer ConfigureServices()
    {
        var container = new ServiceContainer();

        // Register repositories
        container.Register(new AudioSessionRepository());
        container.Register(new VisualizationDataRepository());

        // Register services with factory pattern (lazy initialization)
        container.RegisterFactory<AudioCaptureService>(c => new AudioCaptureService());
        container.RegisterFactory<WaveformService>(c => new WaveformService());
        container.RegisterFactory<SpectrumAnalyzer>(c => new SpectrumAnalyzer());
        container.RegisterFactory<SpectrogramAnalyzer>(c => new SpectrogramAnalyzer());

        return container;
    }

    /// <summary>
    /// Configures services with custom settings.
    /// </summary>
    public static ServiceContainer ConfigureServices(ApplicationSettings settings)
    {
        var container = ConfigureServices();

        // Additional configuration based on settings can be added here
        if (settings.MaxAudioBufferSize > 0)
        {
            // Configure buffer settings
        }

        return container;
    }
}

/// <summary>
/// Application configuration settings.
/// </summary>
public class ApplicationSettings
{
    /// <summary>
    /// Maximum audio buffer size in samples.
    /// </summary>
    public int MaxAudioBufferSize { get; set; } = 192000;

    /// <summary>
    /// Default sample rate for audio capture.
    /// </summary>
    public int DefaultSampleRate { get; set; } = 44100;

    /// <summary>
    /// Default FFT size for spectrum analysis.
    /// </summary>
    public int DefaultFftSize { get; set; } = 2048;

    /// <summary>
    /// Target frames per second for visualization.
    /// </summary>
    public int TargetFps { get; set; } = 60;

    /// <summary>
    /// Enable audio logging.
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    /// Maximum frames to keep in memory per session.
    /// </summary>
    public int MaxFramesPerSession { get; set; } = 5000;

    /// <summary>
    /// Validates settings for consistency.
    /// </summary>
    public bool IsValid()
    {
        return MaxAudioBufferSize > 0 &&
               DefaultSampleRate > 0 &&
               DefaultFftSize > 0 &&
               TargetFps > 0 &&
               MaxFramesPerSession > 0;
    }
}
