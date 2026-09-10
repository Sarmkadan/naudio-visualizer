#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using NAudioVisualizer.Infrastructure;
using NAudioVisualizer.Utilities;

namespace NAudioVisualizer.Configuration;

/// <summary>
/// Manages application configuration settings with file persistence.
/// Loads and saves settings to JSON configuration files.
/// </summary>
public sealed class ConfigurationManager
{
    private readonly Logger _logger;
    private readonly string _configFilePath;
    private readonly Dictionary<string, object> _settings;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string DefaultConfigFileName = "settings.json";

    /// <summary>
    /// String constants for configuration keys.
    /// </summary>
    public static class Keys
    {
        /// <summary>
        /// Audio sample rate in Hz.
        /// </summary>
        public const string AudioSampleRate = "audio.sampleRate";

        /// <summary>
        /// Number of audio channels.
        /// </summary>
        public const string AudioChannelCount = "audio.channelCount";

        /// <summary>
        /// Audio bit depth in bits.
        /// </summary>
        public const string AudioBitDepth = "audio.bitDepth";

        /// <summary>
        /// FFT size for audio analysis.
        /// </summary>
        public const string AudioFftSize = "audio.fftSize";

        /// <summary>
        /// Target frames per second for visualization.
        /// </summary>
        public const string VisualizationTargetFps = "visualization.targetFps";

        /// <summary>
        /// Brightness multiplier for visualization (0.0 to 2.0).
        /// </summary>
        public const string VisualizationBrightness = "visualization.brightness";

        /// <summary>
        /// Contrast multiplier for visualization (0.0 to 2.0).
        /// </summary>
        public const string VisualizationContrast = "visualization.contrast";

        /// <summary>
        /// Whether peak hold is enabled for visualization.
        /// </summary>
        public const string VisualizationPeakHoldEnabled = "visualization.peakHoldEnabled";

        /// <summary>
        /// Peak hold fall rate in dB per second.
        /// </summary>
        public const string VisualizationPeakHoldFallRateDbPerSec = "visualization.peakHoldFallRateDbPerSec";

        /// <summary>
        /// Display width in pixels.
        /// </summary>
        public const string DisplayWidth = "display.width";

        /// <summary>
        /// Display height in pixels.
        /// </summary>
        public const string DisplayHeight = "display.height";

        /// <summary>
        /// Whether display should be fullscreen.
        /// </summary>
        public const string DisplayFullscreen = "display.fullscreen";

        /// <summary>
        /// Default export format (json, csv, etc.).
        /// </summary>
        public const string ExportDefaultFormat = "export.defaultFormat";

        /// <summary>
        /// Whether to compress exported data.
        /// </summary>
        public const string ExportCompress = "export.compress";

        /// <summary>
        /// Whether to include metadata in exported data.
        /// </summary>
        public const string ExportIncludeMetadata = "export.includeMetadata";

        /// <summary>
        /// Logging level (Trace, Debug, Info, Warn, Error, Fatal).
        /// </summary>
        public const string LoggingLevel = "logging.level";

        /// <summary>
        /// Whether to write logs to console.
        /// </summary>
        public const string LoggingWriteToConsole = "logging.writeToConsole";

        /// <summary>
        /// Whether to write logs to file.
        /// </summary>
        public const string LoggingWriteToFile = "logging.writeToFile";
    }

    /// <summary>
    /// Initializes a new instance of the configuration manager.
    /// </summary>
    public ConfigurationManager(Logger logger, string? configFilePath = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _configFilePath = configFilePath ?? Path.Combine(
            PathUtility.GetApplicationDataDirectory(),
            DefaultConfigFileName
        );

        _settings = new Dictionary<string, object>();
        _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        LoadSettings();
    }

    /// <summary>
    /// Gets a configuration value.
    /// </summary>
    public T? GetValue<T>(string key, T? defaultValue = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            return defaultValue;

        if (_settings.TryGetValue(key, out var value))
        {
            if (value is T typedValue)
                return typedValue;

            // Try to convert the value
            try
            {
                if (value is JsonElement element)
                    return element.Deserialize<T>(_jsonOptions);

                return (T?)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        return defaultValue;
    }

    /// <summary>
    /// Sets a configuration value.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="key"/> is null, empty, or consists only of white-space characters.</exception>
    public void SetValue<T>(string key, T value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        _settings[key] = value ?? throw new ArgumentNullException(nameof(value));
        _logger.Debug($"Configuration value set: {key}");
    }

    /// <summary>
    /// Checks if a configuration key exists.
    /// </summary>
    public bool Contains(string key) => !string.IsNullOrEmpty(key) && _settings.ContainsKey(key);

    /// <summary>
    /// Removes a configuration value.
    /// </summary>
    public bool Remove(string key)
    {
        if (string.IsNullOrEmpty(key))
            return false;

        return _settings.Remove(key);
    }

    /// <summary>
    /// Gets all configuration keys.
    /// </summary>
    public IEnumerable<string> GetAllKeys() => _settings.Keys;

    /// <summary>
    /// Loads settings from the configuration file.
    /// </summary>
    public void LoadSettings()
    {
        try
        {
            _settings.Clear();

            if (!File.Exists(_configFilePath))
            {
                _logger.Debug("Configuration file not found. Using defaults.");
                LoadDefaults();
                return;
            }

            string json = File.ReadAllText(_configFilePath);
            var loadedSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(json, _jsonOptions);

            if (loadedSettings is not null)
            {
                foreach (var kvp in loadedSettings)
                {
                    _settings[kvp.Key] = kvp.Value;
                }

                _logger.Info($"Configuration loaded from '{_configFilePath}'");
            }
        }
        catch (IOException ex)
        {
            _logger.Warn($"Failed to load configuration: {ex.Message}. Using defaults.");
            LoadDefaults();
        }
        catch (JsonException ex)
        {
            _logger.Warn($"Failed to load configuration: {ex.Message}. Using defaults.");
            LoadDefaults();
        }
    }

    /// <summary>
    /// Saves the current settings to the configuration file.
    /// </summary>
    public void SaveSettings()
    {
        try
        {
            string directory = Path.GetDirectoryName(_configFilePath) ?? ".";
            FileSystemUtility.EnsureDirectoryExists(directory);

            string json = JsonSerializer.Serialize(_settings, _jsonOptions);
            File.WriteAllText(_configFilePath, json);

            _logger.Info($"Configuration saved to '{_configFilePath}'");
        }
        catch (IOException ex)
        {
            _logger.Error($"Failed to save configuration: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Resets all settings to defaults.
    /// </summary>
    public void ResetToDefaults()
    {
        _settings.Clear();
        LoadDefaults();
        _logger.Info("Configuration reset to defaults.");
    }

    /// <summary>
    /// Loads default configuration values.
    /// </summary>
    private void LoadDefaults()
    {
        // Audio settings
        _settings[Keys.AudioSampleRate] = 44100;
        _settings[Keys.AudioChannelCount] = 2;
        _settings[Keys.AudioBitDepth] = 16;
        _settings[Keys.AudioFftSize] = 2048;

        // Visualization settings
        _settings[Keys.VisualizationTargetFps] = 60;
        _settings[Keys.VisualizationBrightness] = 1.0f;
        _settings[Keys.VisualizationContrast] = 1.0f;

        // Peak‑hold settings (new)
        _settings[Keys.VisualizationPeakHoldEnabled] = false;
        _settings[Keys.VisualizationPeakHoldFallRateDbPerSec] = 10f;

        // Display settings
        _settings[Keys.DisplayWidth] = 1280;
        _settings[Keys.DisplayHeight] = 720;
        _settings[Keys.DisplayFullscreen] = false;

        // Export settings
        _settings[Keys.ExportDefaultFormat] = "json";
        _settings[Keys.ExportCompress] = false;
        _settings[Keys.ExportIncludeMetadata] = true;

        // Logging settings
        _settings[Keys.LoggingLevel] = "Info";
        _settings[Keys.LoggingWriteToConsole] = true;
        _settings[Keys.LoggingWriteToFile] = true;

        _logger.Debug("Default configuration values loaded.");
    }

    /// <summary>
    /// Gets a summary of current configuration.
    /// </summary>
    public string GetConfigurationSummary()
    {
        var lines = new List<string>
        {
            "\nConfiguration Summary:",
            "=" + new string('=', 50)
        };

        foreach (var kvp in _settings)
        {
            lines.Add($"{kvp.Key}: {kvp.Value}");
        }

        lines.Add("=" + new string('=', 50) + "\n");

        return string.Join("\n", lines);
    }

    /// <summary>
    /// Exports settings to a JSON file.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
    public void ExportSettings(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        try
        {
            string json = JsonSerializer.Serialize(_settings, _jsonOptions);
            FileSystemUtility.EnsureDirectoryExists(Path.GetDirectoryName(filePath) ?? ".");
            File.WriteAllText(filePath, json);

            _logger.Info($"Settings exported to '{filePath}'");
        }
        catch (IOException ex)
        {
            _logger.Error($"Failed to export settings: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Imports settings from a JSON file.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
    /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
    /// <exception cref="JsonException">Thrown when the JSON is invalid.</exception>
    public void ImportSettings(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Settings file not found: {filePath}");

        try
        {
            string json = File.ReadAllText(filePath);
            var importedSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(json, _jsonOptions);

            if (importedSettings is not null)
            {
                _settings.Clear();
                foreach (var kvp in importedSettings)
                {
                    _settings[kvp.Key] = kvp.Value;
                }

                _logger.Info($"Settings imported from '{filePath}'");
            }
        }
        catch (IOException ex)
        {
            _logger.Error($"Failed to import settings: {ex.Message}");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.Error($"Failed to import settings: {ex.Message}");
            throw;
        }
    }
}
