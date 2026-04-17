#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Events;
using NAudioVisualizer.Exceptions;
using NAudioVisualizer.Infrastructure;
using NAudioVisualizer.Utilities;

namespace NAudioVisualizer.VST;

/// <summary>
/// Manages the full lifecycle of VST presets: creation from live plugin state, in-memory
/// caching, JSON persistence, categorisation, search, import, and export.
/// All async operations are safe for concurrent callers; the in-memory cache is lock-free.
/// </summary>
public sealed class VstPresetManager : IVstPresetProvider, IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented           = true,
        PropertyNameCaseInsensitive = true,
        Converters              = { new JsonStringEnumConverter() }
    };

    private readonly ConcurrentDictionary<Guid, VstPreset> _cache = new();
    private readonly Logger _logger;
    private string _presetDirectory = string.Empty;
    private bool _isDisposed;

    // ── Properties ─────────────────────────────────────────────────────────

    /// <summary>Directory from which presets are loaded and to which they are saved.</summary>
    public string PresetDirectory => _presetDirectory;

    /// <summary>Total number of presets currently held in memory.</summary>
    public int Count => _cache.Count;

    /// <summary>
    /// Creates a new <see cref="VstPresetManager"/> backed by the provided logger.
    /// Call <see cref="InitializeAsync"/> before performing file-backed operations.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <see langword="null"/>.</exception>
    public VstPresetManager(Logger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ── Initialisation ─────────────────────────────────────────────────────

    /// <summary>
    /// Sets the preset storage directory and eagerly loads every <c>*.vstpreset</c> file
    /// found there into the in-memory cache.
    /// </summary>
    /// <param name="presetDirectory">
    /// Absolute path to the directory. The directory is created if it does not exist.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="presetDirectory"/> is null or whitespace.
    /// </exception>
    public async Task InitializeAsync(string presetDirectory)
    {
        if (string.IsNullOrWhiteSpace(presetDirectory))
            throw new ArgumentException("Preset directory path must not be empty.", nameof(presetDirectory));

        _presetDirectory = presetDirectory;
        FileSystemUtility.EnsureDirectoryExists(presetDirectory);

        await ScanAndLoadDirectoryAsync(presetDirectory).ConfigureAwait(false);

        _logger.Info(
            $"VST preset manager initialised: {_cache.Count} preset(s) loaded from '{presetDirectory}'.");
    }

    // ── IVstPresetProvider ─────────────────────────────────────────────────

    /// <inheritdoc/>
    public IReadOnlyList<VstPreset> GetPresets(Guid pluginId) =>
        _cache.Values
              .Where(p => p.PluginId == pluginId)
              .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
              .ToList();

    /// <inheritdoc/>
    public async Task SavePresetAsync(VstPreset preset)
    {
        if (preset is null)   throw new ArgumentNullException(nameof(preset));
        if (!preset.IsValid())
            throw new VisualizationException(
                $"Cannot save preset '{preset.Name}': validation failed.",
                nameof(VstPresetManager));

        _cache[preset.Id] = preset;

        if (!string.IsNullOrEmpty(_presetDirectory))
        {
            string path = BuildFilePath(preset);
            string json = JsonSerializer.Serialize(preset, JsonOptions);
            await FileSystemUtility.WriteFileAsync(path, json).ConfigureAwait(false);
        }

        _logger.Info($"Preset saved: '{preset.Name}' ({preset.Id})");
        EventPublisher.Instance.Publish(new VstPresetSavedEvent
        {
            PresetId   = preset.Id,
            PresetName = preset.Name,
            PluginId   = preset.PluginId
        });
    }

    /// <inheritdoc/>
    public async Task<VstPreset?> LoadPresetAsync(Guid presetId)
    {
        if (_cache.TryGetValue(presetId, out var cached))
            return cached;

        if (string.IsNullOrEmpty(_presetDirectory))
            return null;

        // Scan disk for a file whose name includes the preset ID
        var matches = Directory.GetFiles(_presetDirectory, $"*{presetId}*.vstpreset");
        if (matches.Length == 0) return null;

        var loaded = await DeserializePresetFileAsync(matches[0]).ConfigureAwait(false);
        if (loaded is not null) _cache[loaded.Id] = loaded;
        return loaded;
    }

    /// <inheritdoc/>
    public async Task DeletePresetAsync(Guid presetId)
    {
        _cache.TryRemove(presetId, out var removed);

        if (!string.IsNullOrEmpty(_presetDirectory))
        {
            foreach (var file in Directory.GetFiles(_presetDirectory, $"*{presetId}*.vstpreset"))
                File.Delete(file);
        }

        if (removed is not null)
        {
            _logger.Info($"Preset deleted: '{removed.Name}' ({presetId})");
            EventPublisher.Instance.Publish(new VstPresetDeletedEvent
            {
                PresetId = presetId,
                PluginId = removed.PluginId
            });
        }

        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    /// <exception cref="VisualizationException">
    /// Thrown when the preset targets a different plugin than <paramref name="plugin"/>.
    /// </exception>
    public void ApplyPreset(VstPreset preset, IVstPlugin plugin)
    {
        if (preset is null) throw new ArgumentNullException(nameof(preset));
        if (plugin is null) throw new ArgumentNullException(nameof(plugin));

        if (preset.PluginId != plugin.Info.Id)
            throw new VisualizationException(
                $"Preset '{preset.Name}' targets plugin {preset.PluginId} " +
                $"but was applied to plugin {plugin.Info.Id}.",
                nameof(VstPresetManager));

        if (preset.PluginChunk is { Length: > 0 })
        {
            plugin.SetChunk(preset.PluginChunk, isPreset: true);
        }
        else
        {
            foreach (var (parameterId, normalizedValue) in preset.ParameterValues)
            {
                if (plugin.IsValidParameterValue(parameterId, normalizedValue))
                    plugin.SetParameter(parameterId, normalizedValue);
            }
        }

        _logger.Info($"Preset '{preset.Name}' applied to plugin '{plugin.Info.Name}'.");
        EventPublisher.Instance.Publish(new VstPresetAppliedEvent
        {
            PresetId   = preset.Id,
            PresetName = preset.Name,
            PluginId   = plugin.Info.Id
        });
    }

    // ── Extended query operations ──────────────────────────────────────────

    /// <summary>
    /// Returns all presets whose <see cref="VstPreset.Category"/> matches
    /// <paramref name="category"/> (case-insensitive), sorted alphabetically.
    /// </summary>
    public IReadOnlyList<VstPreset> GetPresetsByCategory(string category) =>
        _cache.Values
              .Where(p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase))
              .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
              .ToList();

    /// <summary>
    /// Returns all presets whose <see cref="VstPreset.Tags"/> collection contains
    /// <paramref name="tag"/> (case-insensitive).
    /// </summary>
    public IReadOnlyList<VstPreset> SearchByTag(string tag) =>
        _cache.Values
              .Where(p => p.Tags.Any(t => string.Equals(t, tag, StringComparison.OrdinalIgnoreCase)))
              .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
              .ToList();

    /// <summary>
    /// Performs a case-insensitive partial-name search across all loaded presets.
    /// </summary>
    public IReadOnlyList<VstPreset> SearchByName(string fragment) =>
        _cache.Values
              .Where(p => p.Name.Contains(fragment, StringComparison.OrdinalIgnoreCase))
              .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
              .ToList();

    /// <summary>
    /// Returns every distinct category string present in the current preset cache,
    /// sorted alphabetically (case-insensitive).
    /// </summary>
    public IReadOnlyList<string> GetAllCategories() =>
        _cache.Values
              .Select(p => p.Category)
              .Where(c => !string.IsNullOrEmpty(c))
              .Distinct(StringComparer.OrdinalIgnoreCase)
              .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
              .ToList();

    // ── Creation, import, and export ───────────────────────────────────────

    /// <summary>
    /// Captures the current parameter state of <paramref name="plugin"/> and returns a new
    /// <see cref="VstPreset"/> without persisting it. Call <see cref="SavePresetAsync"/>
    /// afterwards to persist.
    /// </summary>
    /// <param name="plugin">Plugin whose state to snapshot.</param>
    /// <param name="name">Human-readable name for the new preset.</param>
    /// <param name="category">Optional browser category.</param>
    /// <param name="description">Optional prose description.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is empty.</exception>
    public VstPreset CreatePresetFromPlugin(
        IVstPlugin plugin,
        string name,
        string category    = "",
        string description = "")
    {
        if (plugin is null)
            throw new ArgumentNullException(nameof(plugin));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Preset name must not be empty.", nameof(name));

        var paramValues = plugin.Parameters
            .Where(p => !p.IsReadOnly)
            .ToDictionary(p => p.Id, p => p.NormalizedValue);

        byte[]? chunk = null;
        try { chunk = plugin.GetChunk(isPreset: true); }
        catch { /* Plugin does not support chunk serialisation — fall back to individual values. */ }

        return new VstPreset
        {
            PluginId        = plugin.Info.Id,
            Name            = name,
            Category        = category,
            Description     = description,
            ParameterValues = paramValues,
            PluginChunk     = chunk is { Length: > 0 } ? chunk : null
        };
    }

    /// <summary>
    /// Imports a preset from an external <c>.vstpreset</c> file, adds it to the cache,
    /// and returns the deserialised instance.
    /// </summary>
    /// <param name="filePath">Absolute path to the source <c>.vstpreset</c> file.</param>
    /// <exception cref="VisualizationException">
    /// Thrown when the file does not exist or cannot be deserialised.
    /// </exception>
    public async Task<VstPreset> ImportPresetAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new VisualizationException(
                $"Preset file not found: '{filePath}'.",
                nameof(VstPresetManager));

        var preset = await DeserializePresetFileAsync(filePath)
            ?? throw new VisualizationException(
                $"Failed to deserialise preset from '{filePath}'.",
                nameof(VstPresetManager));

        _cache[preset.Id] = preset;
        _logger.Info($"Preset imported: '{preset.Name}' from '{filePath}'.");
        return preset;
    }

    /// <summary>
    /// Exports the preset identified by <paramref name="presetId"/> to
    /// <paramref name="outputPath"/> in JSON format.
    /// </summary>
    /// <exception cref="VisualizationException">
    /// Thrown when the preset is not found in the cache.
    /// </exception>
    public async Task ExportPresetAsync(Guid presetId, string outputPath)
    {
        if (!_cache.TryGetValue(presetId, out var preset))
            throw new VisualizationException(
                $"Preset {presetId} not found.",
                nameof(VstPresetManager));

        string json = JsonSerializer.Serialize(preset, JsonOptions);
        await FileSystemUtility.WriteFileAsync(outputPath, json).ConfigureAwait(false);
        _logger.Info($"Preset '{preset.Name}' exported to '{outputPath}'.");
    }

    // ── Private helpers ────────────────────────────────────────────────────

    private async Task ScanAndLoadDirectoryAsync(string directory)
    {
        var files = Directory.GetFiles(directory, "*.vstpreset");
        foreach (var file in files)
        {
            try
            {
                var preset = await DeserializePresetFileAsync(file).ConfigureAwait(false);
                if (preset is not null)
                    _cache[preset.Id] = preset;
            }
            catch (Exception ex)
            {
                _logger.Warn($"Skipping malformed preset file '{Path.GetFileName(file)}': {ex.Message}");
            }
        }
    }

    private static async Task<VstPreset?> DeserializePresetFileAsync(string filePath)
    {
        string json = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);
        return JsonSerializer.Deserialize<VstPreset>(json, JsonOptions);
    }

    private string BuildFilePath(VstPreset preset)
    {
        // Sanitise the name to produce a safe filename segment
        string safeName = string.Concat(preset.Name.Split(Path.GetInvalidFileNameChars()))
                                .Replace(' ', '_')
                                .TrimEnd('.');
        string fileName = $"{safeName}_{preset.Id}.vstpreset";
        return Path.Combine(_presetDirectory, fileName);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_isDisposed) return;
        _cache.Clear();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}

// ── Preset event classes ───────────────────────────────────────────────────────

/// <summary>Raised when a preset is persisted to storage.</summary>
public class VstPresetSavedEvent
{
    /// <summary>Unique identifier of the saved preset.</summary>
    public Guid PresetId { get; init; }

    /// <summary>Human-readable name of the saved preset.</summary>
    public required string PresetName { get; init; }

    /// <summary>Identifier of the plugin this preset belongs to.</summary>
    public Guid PluginId { get; init; }

    /// <summary>UTC timestamp of the save operation.</summary>
    public DateTime SavedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>Raised when a preset's parameter values are applied to a plugin instance.</summary>
public class VstPresetAppliedEvent
{
    /// <summary>Unique identifier of the applied preset.</summary>
    public Guid PresetId { get; init; }

    /// <summary>Human-readable name of the applied preset.</summary>
    public required string PresetName { get; init; }

    /// <summary>Identifier of the plugin the preset was applied to.</summary>
    public Guid PluginId { get; init; }

    /// <summary>UTC timestamp of the apply operation.</summary>
    public DateTime AppliedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>Raised when a preset is permanently removed from storage.</summary>
public class VstPresetDeletedEvent
{
    /// <summary>Unique identifier of the deleted preset.</summary>
    public Guid PresetId { get; init; }

    /// <summary>Identifier of the plugin the deleted preset belonged to.</summary>
    public Guid PluginId { get; init; }

    /// <summary>UTC timestamp of the deletion.</summary>
    public DateTime DeletedAt { get; init; } = DateTime.UtcNow;
}
