#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace NAudioVisualizer.Configuration;

/// <summary>
/// Provides validation helpers for ConfigurationManager instances.
/// </summary>
public static class ConfigurationManagerValidation
{
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
        ValidateNumericSetting(value, "audio.sampleRate", problems, min: 8000, max: 96000);
        ValidateNumericSetting(value, "audio.channelCount", problems, min: 1, max: 8);
        ValidateNumericSetting(value, "audio.bitDepth", problems, min: 8, max: 32);
        ValidateNumericSetting(value, "audio.fftSize", problems, min: 64, max: 16384);
        ValidateNumericSetting(value, "visualization.targetFps", problems, min: 1, max: 240);
        ValidateNumericSetting(value, "visualization.brightness", problems, min: 0.0f, max: 2.0f);
        ValidateNumericSetting(value, "visualization.contrast", problems, min: 0.0f, max: 2.0f);
        ValidateNumericSetting(value, "display.width", problems, min: 320, max: 7680);
        ValidateNumericSetting(value, "display.height", problems, min: 240, max: 4320);

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