#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;

namespace NAudioVisualizer.Configuration;

/// <summary>
/// Extension methods for <see cref="ConfigurationManager"/> to provide convenient and strongly-typed configuration access.
/// </summary>
public static class ConfigurationManagerExtensions
{
    /// <summary>
    /// Gets a required configuration value. Throws <see cref="KeyNotFoundException"/> if the key is missing.
    /// </summary>
    /// <typeparam name="T">The type of the configuration value.</typeparam>
    /// <param name="configuration">The configuration manager instance.</param>
    /// <param name="key">The configuration key.</param>
    /// <returns>The configuration value.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the configuration key is not found.</exception>
    public static T GetRequired<T>(this ConfigurationManager configuration, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Configuration key cannot be null, empty, or whitespace.", nameof(key));

        if (!configuration.Contains(key))
            throw new KeyNotFoundException($"Configuration key '{key}' not found.");

        // GetValue returns null/default if conversion fails, but we've already verified the key exists.
        // We rely on GetValue's conversion logic; if conversion fails, it returns default(T).
        // For reference types, default(T) is null; for value types, it's 0/false/etc.
        // This matches the behavior of GetValue when the key exists but value is invalid.
        var value = configuration.GetValue<T>(key);
        return value;
    }

    /// <summary>
    /// Attempts to get a configuration value, returning whether the key exists and the value could be retrieved.
    /// </summary>
    /// <typeparam name="T">The type of the configuration value.</typeparam>
    /// <param name="configuration">The configuration manager instance.</param>
    /// <param name="key">The configuration key.</param>
    /// <param name="value">When this method returns true, contains the configuration value; otherwise, the default value for <typeparamref name="T"/>.</param>
    /// <returns>true if the key was found; otherwise, false.</returns>
    public static bool TryGetValue<T>(this ConfigurationManager configuration, string key, out T value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            value = default;
            return false;
        }

        if (configuration.Contains(key))
        {
            value = configuration.GetValue<T>(key);
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Gets an integer configuration value. Returns 0 if the key is missing or the value cannot be converted.
    /// </summary>
    /// <param name="configuration">The configuration manager instance.</param>
    /// <param name="key">The configuration key.</param>
    /// <returns>The integer configuration value.</returns>
    public static int GetInt(this ConfigurationManager configuration, string key) =>
        configuration.GetValue<int>(key, 0);

    /// <summary>
    /// Gets a floating-point configuration value. Returns 0.0 if the key is missing or the value cannot be converted.
    /// </summary>
    /// <param name="configuration">The configuration manager instance.</param>
    /// <param name="key">The configuration key.</param>
    /// <returns>The floating-point configuration value.</returns>
    public static float GetFloat(this ConfigurationManager configuration, string key) =>
        configuration.GetValue<float>(key, 0f);

    /// <summary>
    /// Gets a boolean configuration value. Returns false if the key is missing or the value cannot be converted.
    /// </summary>
    /// <param name="configuration">The configuration manager instance.</param>
    /// <param name="key">The configuration key.</param>
    /// <returns>The boolean configuration value.</returns>
    public static bool GetBool(this ConfigurationManager configuration, string key) =>
        configuration.GetValue<bool>(key, false);
}