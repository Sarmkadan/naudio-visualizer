#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Events;
using NAudioVisualizer.Exceptions;
using NAudioVisualizer.Infrastructure;

namespace NAudioVisualizer.VST;

/// <summary>
/// Manages the VST plugin processing chain: loading, unloading, reordering, bypassing,
/// and routing audio sequentially through every active plugin.
/// All public members are thread-safe with respect to chain mutation; audio processing
/// must remain on a single dedicated thread.
/// </summary>
public sealed class VstPluginHostService : IVstPluginHost, IDisposable
{
    private readonly List<IVstPlugin> _plugins = [];
    private readonly Logger _logger;
    private readonly object _chainLock = new();
    private bool _isDisposed;

    /// <inheritdoc/>
    public IReadOnlyList<IVstPlugin> LoadedPlugins
    {
        get { lock (_chainLock) return _plugins.AsReadOnly(); }
    }

    /// <inheritdoc/>
    public bool IsProcessing { get; private set; }

    /// <summary>
    /// Sample rate (Hz) forwarded to each plugin during <see cref="LoadPluginAsync"/>.
    /// </summary>
    public int SampleRate { get; private set; } = 44100;

    /// <summary>
    /// Maximum block size (samples) forwarded to each plugin during <see cref="LoadPluginAsync"/>.
    /// </summary>
    public int BlockSize { get; private set; } = 512;

    /// <summary>
    /// Initialises a new <see cref="VstPluginHostService"/> with the provided logger.
    /// </summary>
    /// <param name="logger">Application logger used for diagnostics.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <see langword="null"/>.</exception>
    public VstPluginHostService(Logger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Sets the audio format parameters used when initialising subsequently loaded plugins.
    /// Already-loaded plugins are not affected; suspend and resume them individually if needed.
    /// </summary>
    /// <param name="sampleRate">Target sample rate in Hz.</param>
    /// <param name="blockSize">Maximum audio block size in samples.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when either value is not positive.</exception>
    public void ConfigureAudioFormat(int sampleRate, int blockSize)
    {
        if (sampleRate <= 0) throw new ArgumentOutOfRangeException(nameof(sampleRate));
        if (blockSize <= 0)  throw new ArgumentOutOfRangeException(nameof(blockSize));

        SampleRate = sampleRate;
        BlockSize  = blockSize;
    }

    /// <summary>
    /// Begins routing audio through the plugin chain.
    /// Publishes a <see cref="VstChainProcessingStartedEvent"/> on the global event bus.
    /// </summary>
    public void StartProcessing()
    {
        ThrowIfDisposed();
        IsProcessing = true;
        _logger.Info("VST plugin chain processing started.");
        EventPublisher.Instance.Publish(new VstChainProcessingStartedEvent
        {
            PluginCount = LoadedPlugins.Count
        });
    }

    /// <summary>
    /// Stops audio routing while keeping all plugins loaded and initialised.
    /// Publishes a <see cref="VstChainProcessingStoppedEvent"/> on the global event bus.
    /// </summary>
    public void StopProcessing()
    {
        IsProcessing = false;
        _logger.Info("VST plugin chain processing stopped.");
        EventPublisher.Instance.Publish(new VstChainProcessingStoppedEvent());
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Validates that the file exists, creates a <see cref="ManagedVstPlugin"/> wrapper,
    /// calls <see cref="IVstPlugin.Initialize"/>, and appends the plugin to the chain.
    /// In a production integration the wrapper would delegate to a native VST bridge.
    /// </remarks>
    public async Task<IVstPlugin> LoadPluginAsync(string pluginPath)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(pluginPath))
            throw new ArgumentException("Plugin path must not be empty.", nameof(pluginPath));

        if (!File.Exists(pluginPath))
            throw new VisualizationException(
                $"VST plugin not found at '{pluginPath}'.",
                nameof(VstPluginHostService));

        try
        {
            _logger.Info($"Loading VST plugin: {Path.GetFileName(pluginPath)}");

            var plugin = await Task.Run(() => CreatePluginInstance(pluginPath)).ConfigureAwait(false);
            plugin.Initialize(SampleRate, BlockSize);

            lock (_chainLock)
                _plugins.Add(plugin);

            _logger.Info($"Plugin loaded: {plugin.Info.Name} ({plugin.Info.Id})");

            EventPublisher.Instance.Publish(new VstPluginLoadedEvent
            {
                PluginId   = plugin.Info.Id,
                PluginName = plugin.Info.Name,
                PluginPath = pluginPath,
                Category   = plugin.Info.Category.ToString()
            });

            return plugin;
        }
        catch (VisualizationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to load VST plugin '{pluginPath}': {ex.Message}", ex);
            throw new VisualizationException(
                $"Unexpected error loading VST plugin '{Path.GetFileName(pluginPath)}': {ex.Message}",
                nameof(VstPluginHostService));
        }
    }

    /// <inheritdoc/>
    public async Task UnloadPluginAsync(Guid pluginId)
    {
        ThrowIfDisposed();

        IVstPlugin? plugin;
        lock (_chainLock)
        {
            plugin = _plugins.FirstOrDefault(p => p.Info.Id == pluginId);
            if (plugin is null) return;
            _plugins.Remove(plugin);
        }

        string name = plugin.Info.Name;
        await Task.Run(plugin.Dispose).ConfigureAwait(false);

        _logger.Info($"Plugin unloaded: {name} ({pluginId})");
        EventPublisher.Instance.Publish(new VstPluginUnloadedEvent
        {
            PluginId   = pluginId,
            PluginName = name
        });
    }

    /// <inheritdoc/>
    public void ProcessAudioChain(float[] buffer, int sampleCount)
    {
        if (!IsProcessing || buffer is null || sampleCount <= 0)
            return;

        IReadOnlyList<IVstPlugin> snapshot;
        lock (_chainLock) snapshot = _plugins.AsReadOnly();

        if (snapshot.Count == 0) return;

        var workBuffer = new float[sampleCount];

        foreach (var plugin in snapshot)
        {
            if (plugin.IsBypassed || plugin.State != VstPluginState.Active)
                continue;

            try
            {
                plugin.ProcessAudio(buffer, workBuffer, sampleCount);
                Array.Copy(workBuffer, buffer, sampleCount);
            }
            catch (Exception ex)
            {
                _logger.Warn($"Plugin '{plugin.Info.Name}' threw during ProcessAudio: {ex.Message}");
            }
        }
    }

    /// <inheritdoc/>
    public void SetPluginEnabled(Guid pluginId, bool enabled)
    {
        lock (_chainLock)
        {
            var plugin = _plugins.FirstOrDefault(p => p.Info.Id == pluginId);
            if (plugin is null) return;
            plugin.IsBypassed = !enabled;
        }

        EventPublisher.Instance.Publish(new VstPluginBypassChangedEvent
        {
            PluginId   = pluginId,
            IsBypassed = !enabled
        });
    }

    /// <inheritdoc/>
    public void ReorderPlugins(IEnumerable<Guid> orderedIds)
    {
        if (orderedIds is null) throw new ArgumentNullException(nameof(orderedIds));

        var ids = orderedIds.ToList();

        lock (_chainLock)
        {
            var reordered = ids
                .Select(id => _plugins.FirstOrDefault(p => p.Info.Id == id))
                .OfType<IVstPlugin>()
                .ToList();

            // Append any plugins whose IDs were not present in orderedIds
            var tail = _plugins.Where(p => !ids.Contains(p.Info.Id)).ToList();
            _plugins.Clear();
            _plugins.AddRange(reordered);
            _plugins.AddRange(tail);
        }
    }

    /// <inheritdoc/>
    public IVstPlugin? FindPlugin(Guid pluginId)
    {
        lock (_chainLock)
            return _plugins.FirstOrDefault(p => p.Info.Id == pluginId);
    }

    // ── Private helpers ────────────────────────────────────────────────────

    private static ManagedVstPlugin CreatePluginInstance(string pluginPath)
    {
        var info = new VstPluginInfo(
            Id:             Guid.NewGuid(),
            Name:           Path.GetFileNameWithoutExtension(pluginPath),
            Vendor:         "Unknown",
            Version:        "1.0.0",
            PluginPath:     pluginPath,
            Category:       VstPluginCategory.Effect,
            ParameterCount: 0,
            IsSynth:        false)
        {
            SdkVersion = "VST 2.4"
        };

        return new ManagedVstPlugin(info);
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed) throw new ObjectDisposedException(GetType().Name);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_isDisposed) return;

        StopProcessing();

        lock (_chainLock)
        {
            foreach (var plugin in _plugins)
                plugin.Dispose();
            _plugins.Clear();
        }

        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}

// ── Concrete plugin wrapper ────────────────────────────────────────────────────

/// <summary>
/// Host-managed wrapper around a VST plugin instance.
/// Maintains lifecycle state transitions and delegates audio I/O to the plugin's
/// native processing routine via the VST bridge.
/// </summary>
internal sealed class ManagedVstPlugin : IVstPlugin
{
    private readonly List<VstParameter> _parameters = [];
    private bool _isDisposed;

    /// <inheritdoc/>
    public VstPluginInfo Info { get; }

    /// <inheritdoc/>
    public IReadOnlyList<VstParameter> Parameters => _parameters.AsReadOnly();

    /// <inheritdoc/>
    public VstPluginState State { get; private set; } = VstPluginState.Loaded;

    /// <inheritdoc/>
    public bool IsBypassed { get; set; }

    internal ManagedVstPlugin(VstPluginInfo info)
    {
        Info = info ?? throw new ArgumentNullException(nameof(info));
    }

    /// <inheritdoc/>
    public void Initialize(int sampleRate, int blockSize)
    {
        if (sampleRate <= 0) throw new ArgumentOutOfRangeException(nameof(sampleRate));
        if (blockSize  <= 0) throw new ArgumentOutOfRangeException(nameof(blockSize));

        State = VstPluginState.Initializing;
        // Native bridge initialisation would occur here.
        State = VstPluginState.Active;
    }

    /// <inheritdoc/>
    public void Suspend()
    {
        if (State == VstPluginState.Active)
            State = VstPluginState.Suspended;
    }

    /// <inheritdoc/>
    public void Resume()
    {
        if (State == VstPluginState.Suspended)
            State = VstPluginState.Active;
    }

    /// <inheritdoc/>
    public void ProcessAudio(float[] inputSamples, float[] outputSamples, int sampleCount)
    {
        // Pass-through: native DSP dispatch would replace this in a real bridge.
        Array.Copy(inputSamples, outputSamples, sampleCount);
    }

    /// <inheritdoc/>
    public void SetParameter(int parameterId, float normalizedValue)
    {
        var param = _parameters.FirstOrDefault(p => p.Id == parameterId);
        if (param is null) return;

        param.CurrentValue = param.DenormalizeValue(normalizedValue);
    }

    /// <inheritdoc/>
    public float GetParameter(int parameterId)
    {
        var param = _parameters.FirstOrDefault(p => p.Id == parameterId);
        return param?.NormalizedValue ?? 0f;
    }

    /// <inheritdoc/>
    public byte[] GetChunk(bool isPreset) => [];

    /// <inheritdoc/>
    public void SetChunk(byte[] data, bool isPreset) { /* Reserved for native bridge. */ }

    /// <inheritdoc/>
    public bool IsValidParameterValue(int parameterId, float normalizedValue) =>
        normalizedValue >= 0f && normalizedValue <= 1f &&
        _parameters.Any(p => p.Id == parameterId);

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_isDisposed) return;
        Suspend();
        State    = VstPluginState.Unloaded;
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}

// ── VST host event classes ─────────────────────────────────────────────────────

/// <summary>Raised when a VST plugin is successfully loaded and appended to the chain.</summary>
public class VstPluginLoadedEvent
{
    /// <summary>Unique identifier assigned to the new plugin instance.</summary>
    public Guid PluginId { get; init; }

    /// <summary>Human-readable plugin name.</summary>
    public required string PluginName { get; init; }

    /// <summary>Absolute file-system path of the loaded plugin DLL.</summary>
    public required string PluginPath { get; init; }

    /// <summary>Category string reported by the plugin (e.g. "Effect", "EQ").</summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>UTC timestamp of when the plugin finished loading.</summary>
    public DateTime LoadedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>Raised when a VST plugin is removed from the chain and disposed.</summary>
public class VstPluginUnloadedEvent
{
    /// <summary>Identifier of the plugin that was removed.</summary>
    public Guid PluginId { get; init; }

    /// <summary>Human-readable name of the removed plugin.</summary>
    public required string PluginName { get; init; }

    /// <summary>UTC timestamp of the unload.</summary>
    public DateTime UnloadedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>Raised when a plugin's bypass state is toggled.</summary>
public class VstPluginBypassChangedEvent
{
    /// <summary>Identifier of the affected plugin.</summary>
    public Guid PluginId { get; init; }

    /// <summary><see langword="true"/> when the plugin was bypassed; <see langword="false"/> when re-enabled.</summary>
    public bool IsBypassed { get; init; }

    /// <summary>UTC timestamp of the change.</summary>
    public DateTime ChangedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>Raised when the host begins routing audio through the plugin chain.</summary>
public class VstChainProcessingStartedEvent
{
    /// <summary>Number of plugins in the chain at the moment processing started.</summary>
    public int PluginCount { get; init; }

    /// <summary>UTC timestamp of the start.</summary>
    public DateTime StartedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>Raised when the host stops routing audio through the plugin chain.</summary>
public class VstChainProcessingStoppedEvent
{
    /// <summary>UTC timestamp of the stop.</summary>
    public DateTime StoppedAt { get; init; } = DateTime.UtcNow;
}
