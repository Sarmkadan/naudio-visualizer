#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NAudioVisualizer.Domain.Models;

namespace NAudioVisualizer.VST;

/// <summary>
/// Defines the contract for a single VST plugin instance managed by <see cref="IVstPluginHost"/>.
/// </summary>
/// <remarks>
/// Implementations wrap a native VST bridge and expose a managed, lifecycle-aware API.
/// All audio processing methods must be called from the same thread that owns the audio engine.
/// </remarks>
public interface IVstPlugin : IDisposable
{
    /// <summary>
    /// Immutable metadata returned by the plugin on load (name, vendor, category, etc.).
    /// </summary>
    VstPluginInfo Info { get; }

    /// <summary>
    /// Live, ordered list of parameter descriptors exposed by this plugin.
    /// </summary>
    IReadOnlyList<VstParameter> Parameters { get; }

    /// <summary>
    /// Current lifecycle state of the plugin instance.
    /// </summary>
    VstPluginState State { get; }

    /// <summary>
    /// When <see langword="true"/> the plugin is skipped during <see cref="ProcessAudio"/> calls
    /// but remains initialised and ready to resume.
    /// </summary>
    bool IsBypassed { get; set; }

    /// <summary>
    /// Prepares the plugin for real-time audio processing at the given sample rate and block size.
    /// Must be called once before the first <see cref="ProcessAudio"/> invocation.
    /// </summary>
    /// <param name="sampleRate">Audio sample rate in Hz (e.g. 44100, 48000).</param>
    /// <param name="blockSize">Maximum number of samples per processing block.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="sampleRate"/> or <paramref name="blockSize"/> is not positive.
    /// </exception>
    void Initialize(int sampleRate, int blockSize);

    /// <summary>
    /// Suspends the plugin, releasing real-time resources while preserving all state.
    /// The plugin transitions to <see cref="VstPluginState.Suspended"/>.
    /// </summary>
    void Suspend();

    /// <summary>
    /// Resumes a previously suspended plugin, re-allocating all real-time resources.
    /// The plugin transitions back to <see cref="VstPluginState.Active"/>.
    /// </summary>
    void Resume();

    /// <summary>
    /// Processes a block of interleaved audio samples in-place, reading from
    /// <paramref name="inputSamples"/> and writing the processed result to
    /// <paramref name="outputSamples"/>.
    /// </summary>
    /// <param name="inputSamples">Read-only interleaved input buffer (normalised –1.0 to +1.0).</param>
    /// <param name="outputSamples">Destination buffer that receives the processed output.</param>
    /// <param name="sampleCount">
    /// Number of samples to process; must be ≤ the block size passed to <see cref="Initialize"/>.
    /// </param>
    void ProcessAudio(float[] inputSamples, float[] outputSamples, int sampleCount);

    /// <summary>
    /// Sets a parameter to <paramref name="normalizedValue"/> (in [0.0, 1.0]) by its zero-based index.
    /// </summary>
    /// <param name="parameterId">Zero-based parameter index.</param>
    /// <param name="normalizedValue">Normalised target value, clamped to [0.0, 1.0].</param>
    void SetParameter(int parameterId, float normalizedValue);

    /// <summary>
    /// Returns the current normalised value ([0.0, 1.0]) for the parameter at
    /// <paramref name="parameterId"/>.
    /// </summary>
    float GetParameter(int parameterId);

    /// <summary>
    /// Serialises the complete plugin state into an opaque byte chunk suitable for storage.
    /// </summary>
    /// <param name="isPreset">
    /// <see langword="true"/> to capture preset (program) state only;
    /// <see langword="false"/> for the full bank state.
    /// </param>
    /// <returns>A non-empty byte array, or an empty array if the plugin does not support chunks.</returns>
    byte[] GetChunk(bool isPreset);

    /// <summary>
    /// Restores plugin state from a chunk previously captured with <see cref="GetChunk"/>.
    /// </summary>
    /// <param name="data">Chunk data to restore.</param>
    /// <param name="isPreset">Must match the flag used when the chunk was captured.</param>
    void SetChunk(byte[] data, bool isPreset);

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="normalizedValue"/> is an acceptable
    /// input for parameter <paramref name="parameterId"/>.
    /// </summary>
    bool IsValidParameterValue(int parameterId, float normalizedValue);
}

/// <summary>
/// Manages an ordered chain of <see cref="IVstPlugin"/> instances and routes audio
/// sequentially through every active plugin.
/// </summary>
public interface IVstPluginHost
{
    /// <summary>
    /// Ordered, live list of all plugins currently in the processing chain.
    /// </summary>
    IReadOnlyList<IVstPlugin> LoadedPlugins { get; }

    /// <summary>
    /// Returns <see langword="true"/> while the host is actively routing audio
    /// through the plugin chain.
    /// </summary>
    bool IsProcessing { get; }

    /// <summary>
    /// Loads a VST plugin from the file system at <paramref name="pluginPath"/> and appends
    /// it to the end of the processing chain.
    /// </summary>
    /// <param name="pluginPath">Absolute path to the VST plugin DLL.</param>
    /// <returns>The fully initialised plugin instance.</returns>
    Task<IVstPlugin> LoadPluginAsync(string pluginPath);

    /// <summary>
    /// Removes the plugin identified by <paramref name="pluginId"/> from the chain, disposes
    /// it, and releases all associated native resources.
    /// </summary>
    Task UnloadPluginAsync(Guid pluginId);

    /// <summary>
    /// Routes <paramref name="buffer"/> sequentially through every non-bypassed, active plugin.
    /// The buffer is modified in-place; bypassed and non-active plugins are skipped.
    /// </summary>
    /// <param name="buffer">Interleaved sample buffer processed in-place.</param>
    /// <param name="sampleCount">Number of samples in <paramref name="buffer"/>.</param>
    void ProcessAudioChain(float[] buffer, int sampleCount);

    /// <summary>
    /// Enables or disables (bypasses) the plugin identified by <paramref name="pluginId"/>
    /// without removing it from the chain.
    /// </summary>
    void SetPluginEnabled(Guid pluginId, bool enabled);

    /// <summary>
    /// Reorders the plugin chain to match the sequence given by <paramref name="orderedIds"/>.
    /// Plugins whose IDs are omitted are appended at the end in their original order.
    /// </summary>
    void ReorderPlugins(IEnumerable<Guid> orderedIds);

    /// <summary>
    /// Returns the plugin with the given <paramref name="pluginId"/>,
    /// or <see langword="null"/> if no such plugin is loaded.
    /// </summary>
    IVstPlugin? FindPlugin(Guid pluginId);
}

/// <summary>
/// Provides storage, retrieval, and application operations for VST presets.
/// </summary>
public interface IVstPresetProvider
{
    /// <summary>
    /// Returns all presets belonging to the plugin identified by <paramref name="pluginId"/>,
    /// sorted alphabetically by name.
    /// </summary>
    IReadOnlyList<VstPreset> GetPresets(Guid pluginId);

    /// <summary>
    /// Persists <paramref name="preset"/> to the backing store, overwriting any existing
    /// entry that shares the same <see cref="VstPreset.Id"/>.
    /// </summary>
    Task SavePresetAsync(VstPreset preset);

    /// <summary>
    /// Loads and returns the preset identified by <paramref name="presetId"/>, or
    /// <see langword="null"/> when not found.
    /// </summary>
    Task<VstPreset?> LoadPresetAsync(Guid presetId);

    /// <summary>
    /// Permanently removes the preset identified by <paramref name="presetId"/>.
    /// </summary>
    Task DeletePresetAsync(Guid presetId);

    /// <summary>
    /// Applies <paramref name="preset"/> to <paramref name="plugin"/> by restoring the chunk
    /// or writing individual parameter values, whichever the preset contains.
    /// </summary>
    /// <exception cref="NAudioVisualizer.Exceptions.VisualizationException">
    /// Thrown when the preset's <see cref="VstPreset.PluginId"/> does not match
    /// <paramref name="plugin"/>'s identifier.
    /// </exception>
    void ApplyPreset(VstPreset preset, IVstPlugin plugin);
}
