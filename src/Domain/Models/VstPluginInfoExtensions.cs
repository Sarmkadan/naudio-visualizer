#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace NAudioVisualizer.Domain.Models
{
    /// <summary>
    /// Extension methods for <see cref="VstPluginInfo"/>.
    /// </summary>
    public static class VstPluginInfoExtensions
    {
        /// <summary>
        /// Determines whether the plugin is an instrument (synthesizer).
        /// </summary>
        /// <param name="plugin">The plugin to check.</param>
        /// <returns><see langword="true"/> if the plugin is an instrument; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is <see langword="null"/>.</exception>
        public static bool IsInstrument(this VstPluginInfo plugin)
        {
            if (plugin is null)
                throw new ArgumentNullException(nameof(plugin));

            return plugin.Category == VstPluginCategory.Synth;
        }

        /// <summary>
        /// Determines whether the plugin is an effect.
        /// </summary>
        /// <param name="plugin">The plugin to check.</param>
        /// <returns><see langword="true"/> if the plugin is an effect; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is <see langword="null"/>.</exception>
        public static bool IsEffect(this VstPluginInfo plugin)
        {
            if (plugin is null)
                throw new ArgumentNullException(nameof(plugin));

            return plugin.Category == VstPluginCategory.Effect;
        }

        /// <summary>
        /// Gets the display name of the plugin in the format "Vendor – Name vVersion".
        /// </summary>
        /// <param name="plugin">The plugin to get the display name for.</param>
        /// <returns>The display name string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is <see langword="null"/>.</exception>
        public static string GetDisplayName(this VstPluginInfo plugin)
        {
            if (plugin is null)
                throw new ArgumentNullException(nameof(plugin));

            return $"{plugin.Vendor} – {plugin.Name} v{plugin.Version}";
        }

        /// <summary>
        /// Filters a sequence of plugins by the specified category.
        /// </summary>
        /// <param name="plugins">The sequence of plugins to filter.</param>
        /// <param name="category">The category to filter by.</param>
        /// <returns>An enumerable of plugins that match the specified category.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugins"/> is <see langword="null"/>.</exception>
        public static IEnumerable<VstPluginInfo> FilterByCategory(this IEnumerable<VstPluginInfo> plugins, VstPluginCategory category)
        {
            if (plugins is null)
                throw new ArgumentNullException(nameof(plugins));

            return plugins.Where(p => p.Category == category);
        }
    }
}