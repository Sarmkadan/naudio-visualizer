// <copyright file="ConfigurationManagerExtensions.cs" company="NAudioVisualizer">
// Copyright (c) NAudioVisualizer. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace NAudioVisualizer.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides extension methods for <see cref="ConfigurationManager"/>.
    /// </summary>
    public static class ConfigurationManagerExtensions
    {
        /// <summary>
        /// Gets the required configuration value for the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the configuration value.</typeparam>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the key is not found.</exception>
        public static T GetRequired<T>(this ConfigurationManager configurationManager, string key)
        {
            if (configurationManager == null)
            {
                throw new ArgumentNullException(nameof(configurationManager));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (configurationManager.TryGetValue(key, out T? value))
            {
                return value!;
            }

            throw new KeyNotFoundException($"Configuration key '{key}' not found.");
        }

        /// <summary>
        /// Tries to get the configuration value for the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the configuration value.</typeparam>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="key">The configuration key.</param>
        /// <param name="value">When this method returns, contains the configuration value if the key is found; otherwise, the default value for the type of the value parameter.</param>
        /// <returns>true if the key is found; otherwise, false.</returns>
        public static bool TryGetValue<T>(this ConfigurationManager configurationManager, string key, out T value)
        {
            if (configurationManager == null)
            {
                throw new ArgumentNullException(nameof(configurationManager));
            }

            if (string.IsNullOrEmpty(key))
            {
                value = default!;
                return false;
            }

            try
            {
                value = configurationManager.GetValue<T>(key);
                return true;
            }
            catch
            {
                value = default!;
                return false;
            }
        }

        /// <summary>
        /// Gets the configuration value as an integer.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value as an integer.</returns>
        public static int GetInt(this ConfigurationManager configurationManager, string key)
        {
            return configurationManager.GetValue<int>(key);
        }

        /// <summary>
        /// Gets the configuration value as a float.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value as a float.</returns>
        public static float GetFloat(this ConfigurationManager configurationManager, string key)
        {
            return configurationManager.GetValue<float>(key);
        }

        /// <summary>
        /// Gets the configuration value as a boolean.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value as a boolean.</returns>
        public static bool GetBool(this ConfigurationManager configurationManager, string key)
        {
            return configurationManager.GetValue<bool>(key);
        }
    }
}
