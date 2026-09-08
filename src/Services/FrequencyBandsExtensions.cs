#nullable enable

using System;

namespace NAudioVisualizer.Services;

/// <summary>
/// Provides extension methods for <see cref="FrequencyBands"/> instances.
/// </summary>
public static class FrequencyBandsExtensions
{
    /// <summary>
    /// Returns the frequency band energy values in their declared order.
    /// </summary>
    /// <param name="bands">The frequency band values to convert.</param>
    /// <returns>
    /// An array containing bass, mid, and treble energy values, in that order.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="bands"/> is null.
    /// </exception>
    public static float[] ToArray(this FrequencyBands bands)
    {
        if (bands is null)
            throw new ArgumentNullException(nameof(bands));

        return new[]
        {
            bands.BassEnergy,
            bands.MidEnergy,
            bands.TrebleEnergy
        };
    }
}
