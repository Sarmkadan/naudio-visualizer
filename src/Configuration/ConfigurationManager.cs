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

    /// <summary>
    /// Initializes a new instance of the configuration manager.
    /// </summary>
    public ConfigurationManager(Logger logger, string? configFilePath = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _configFilePath = configFilePath ?? Path.Combine(
            PathUtility.GetApplicationDataDirectory(),
            "settings.json"
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
    public void SetValue<T>(string key, T value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

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
        _settings["audio.sampleRate"] = 44100;
        _settings["audio.channelCount"] = 2;
        _settings["audio.bitDepth"] = 16;
        _settings["audio.fftSize"] = 2048;

        // Visualization settings
        _settings["visualization.targetFps"] = 60;
        _settings["visualization.brightness"] = 1.0f;
        _settings["visualization.contrast"] = 1.0f;

        // Display settings
        _settings["display.width"] = 1280;
        _settings["display.height"] = 720;
        _settings["display.fullscreen"] = false;

        // Export settings
        _settings["export.defaultFormat"] = "json";
        _settings["export.compress"] = false;
        _settings["export.includeMetadata"] = true;

        // Logging settings
        _settings["logging.level"] = "Info";
        _settings["logging.writeToConsole"] = true;
        _settings["logging.writeToFile"] = true;

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
    public void ExportSettings(string filePath)
    {
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
    public void ImportSettings(string filePath)
    {
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