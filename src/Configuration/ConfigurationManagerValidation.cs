#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace NAudioVisualizer.Configuration;
using NAudioVisualizer.Constants;

/// <summary>
/// Provides validation helpers for ConfigurationManager instances.
/// </summary>
public static class ConfigurationManagerValidation
{
    // Audio sample rate bounds
    /// <summary>
    /// Minimum allowed sample rate in Hz (8000).
    /// </summary>
    private const int MinSampleRate = 8000;
    /// <summary>
    /// Maximum allowed sample rate in Hz (96000).
    /// </summary>
    private const int MaxSampleRate = AudioConstants.SAMPLE_RATE_96000;

    // Audio channel count bounds
    /// <summary>
    /// Minimum allowed audio channel count (1).
    /// </summary>
    private const int MinChannelCount = 1;
    /// <summary>
    /// Maximum allowed audio channel count (8).
    /// </summary>
    private const int MaxChannelCount = 8;

    // Audio bit depth bounds
    /// <summary>
    /// Minimum allowed bit depth in bits (8).
    /// </summary>
    private const int MinBitDepth = 8;
    /// <summary>
    /// Maximum allowed bit depth in bits (32).
    /// </summary>
    private const int MaxBitDepth = 32;

    // FFT size bounds
    /// <summary>
    /// Minimum allowed FFT size (64).
    /// </summary>
    private const int MinFftSize = 64;
    /// <summary>
    /// Maximum allowed FFT size (16384).
    /// </summary>
    private const int MaxFftSize = AudioConstants.FFT_MAXIMUM;

    // Visualization target FPS bounds
    /// <summary>
    /// Minimum allowed target frames per second (1).
    /// </summary>
    private const int MinTargetFps = 1;
    /// <summary>
    /// Maximum allowed target frames per second (240).
    /// </summary>
    private const int MaxTargetFps = 240;

    // Visualization brightness bounds
    /// <summary>
    /// Minimum allowed brightness value (0.0).
    /// </summary>
    private const float MinBrightness = 0.0f;
    /// <summary>
    /// Maximum allowed brightness value (2.0).
    /// </summary>
    private const float MaxBrightness = 2.0f;

    // Visualization contrast bounds
    /// <summary>
    /// Minimum allowed contrast value (0.0).
    /// </summary>
    private const float MinContrast = 0.0f;
    /// <summary>
    /// Maximum allowed contrast value (2.0).
    /// </summary>
    private const float MaxContrast = 2.0f;

    // Display width bounds
    /// <summary>
    /// Minimum allowed display width in pixels (320).
    /// </summary>
    private const int MinDisplayWidth = VisualizationConstants.MINIMUM_RENDER_WIDTH;
    /// <summary>
    /// Maximum allowed display width in pixels (7680).
    /// </summary>
    private const int MaxDisplayWidth = 7680;

    // Display height bounds
    /// <summary>
    /// Minimum allowed display height in pixels (240).
    /// </summary>
    private const int MinDisplayHeight = VisualizationConstants.MINIMUM_RENDER_HEIGHT;
    /// <summary>
    /// Maximum allowed display height in pixels (4320).
    /// </summary>
    private const int MaxDisplayHeight = 4320;

    /// <summary>
    /// Validates the configuration manager and returns a list of human-readable problems.
    /// </summary>
    /// <param name="value">The configuration manager to validate.</param>
    /// <returns>List of validation problems; empty if valid.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    public static IReadOnlyList<string> Validate(this ConfigurationManager value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = new List<string>();

        // Validate configuration keys and values based on known default settings
        var allKeys = value.GetAllKeys().ToList();

        // Check for empty or whitespace keys
        foreach (var key in allKeys)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                problems.Add("Configuration contains a null, empty, or whitespace key.");
                break;
            }
        }

        // Validate known numeric settings ranges
        ValidateNumericSetting(value, "audio.sampleRate", problems, min: MinSampleRate, max: MaxSampleRate);
        ValidateNumericSetting(value, "audio.channelCount", problems, min: MinChannelCount, max: MaxChannelCount);
        ValidateNumericSetting(value, "audio.bitDepth", problems, min: MinBitDepth, max: MaxBitDepth);
        ValidateNumericSetting(value, "audio.fftSize", problems, min: MinFftSize, max: MaxFftSize);
        ValidateNumericSetting(value, "visualization.targetFps", problems, min: MinTargetFps, max: MaxTargetFps);
        ValidateNumericSetting(value, "visualization.brightness", problems, min: MinBrightness, max: MaxBrightness);
        ValidateNumericSetting(value, "visualization.contrast", problems, min: MinContrast, max: MaxContrast);
        ValidateNumericSetting(value, "display.width", problems, min: MinDisplayWidth, max: MaxDisplayWidth);
        ValidateNumericSetting(value, "display.height", problems, min: MinDisplayHeight, max: MaxDisplayHeight);

        // Validate boolean settings
        ValidateBooleanSetting(value, "display.fullscreen", problems);
        ValidateBooleanSetting(value, "export.compress", problems);
        ValidateBooleanSetting(value, "export.includeMetadata", problems);
        ValidateBooleanSetting(value, "logging.writeToConsole", problems);
        ValidateBooleanSetting(value, "logging.writeToFile", problems);

        // Validate string settings
        ValidateStringSetting(value, "export.defaultFormat", problems, allowedValues: new[] { "json", "xml", "yaml", "csv" });
        ValidateStringSetting(value, "logging.level", problems, allowedValues: new[] { "Debug", "Info", "Warn", "Error" });

        return problems.AsReadOnly();
    }

    /// <summary>
    /// Checks if the configuration manager is valid.
    /// </summary>
    /// <param name="value">The configuration manager to check.</param>
    /// <returns>True if valid; false otherwise.</returns>
    public static bool IsValid(this ConfigurationManager value) => value?.Validate().Count == 0;

    /// <summary>
    /// Ensures the configuration manager is valid, throwing an exception if not.
    /// </summary>
    /// <param name="value">The configuration manager to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the configuration is invalid.</exception>
    public static void EnsureValid(this ConfigurationManager value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = value.Validate();

        if (problems.Count > 0)
        {
            var errorMessage = "Configuration validation failed:\n" + string.Join("\n", problems.Select((p, i) => $" {i + 1}. {p}"));
            throw new ArgumentException(errorMessage, nameof(value));
        }
    }

    private static void ValidateNumericSetting(ConfigurationManager config, string key, List<string> problems, double min, double max)
    {
        try
        {
            if (!config.Contains(key))
            {
                problems.Add($"Configuration key '{key}' is missing.");
                return;
            }

            var value = config.GetValue<double>(key);

            if (value < min || value > max)
            {
                problems.Add($"Configuration key '{key}' has value {value} which is out of range [{min}, {max}] for this setting.");
            }
        }
        catch
        {
            // If we can't get the value as double, it might be the wrong type
            problems.Add($"Configuration key '{key}' has an invalid numeric value.");
        }
    }

    private static void ValidateBooleanSetting(ConfigurationManager config, string key, List<string> problems)
    {
        try
        {
            if (!config.Contains(key))
            {
                problems.Add($"Configuration key '{key}' is missing.");
            }
        }
        catch
        {
            problems.Add($"Configuration key '{key}' has an invalid boolean value.");
        }
    }

    private static void ValidateStringSetting(ConfigurationManager config, string key, List<string> problems, string[] allowedValues)
    {
        try
        {
            if (!config.Contains(key))
            {
                problems.Add($"Configuration key '{key}' is missing.");
                return;
            }

            var value = config.GetValue<string>(key);

            if (value == null)
            {
                problems.Add($"Configuration key '{key}' has a null value.");
            }
            else if (string.IsNullOrWhiteSpace(value))
            {
                problems.Add($"Configuration key '{key}' has an empty or whitespace value.");
            }
            else if (allowedValues != null && !allowedValues.Contains(value, StringComparer.OrdinalIgnoreCase))
            {
                problems.Add($"Configuration key '{key}' has value '{value}' which is not one of the allowed values [{string.Join(", ", allowedValues)}].");
            }
        }
        catch
        {
            problems.Add($"Configuration key '{key}' has an invalid string value.");
        }
    }
}
