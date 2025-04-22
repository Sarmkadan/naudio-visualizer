namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Extensions for <see cref="AudioDevice"/>.
/// </summary>
public static class AudioDeviceExtensions
{
    /// <summary>
    /// Determines if the <paramref name="device"/> supports a specific sample rate.
    /// </summary>
    /// <param name="device">The <see cref="AudioDevice"/> to check.</param>
    /// <param name="sampleRate">The sample rate to check for support.</param>
    /// <returns><see langword="true"/> if the device supports the sample rate; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="device"/> is <see langword="null"/>.</exception>
    public static bool IsSampleRateSupported(this AudioDevice device, int sampleRate)
    {
        ArgumentNullException.ThrowIfNull(device);
        return device.SupportsSampleRate(sampleRate);
    }

    /// <summary>
    /// Gets a list of supported sample rates for the <paramref name="device"/>.
    /// </summary>
    /// <param name="device">The <see cref="AudioDevice"/> to get sample rates for.</param>
    /// <returns>An <see cref="IReadOnlyList{T}"/> of supported sample rates.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="device"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<int> GetSupportedSampleRates(this AudioDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        return device.SupportedSampleRates.AsReadOnly();
    }

    /// <summary>
    /// Checks if the <paramref name="device"/> is a default device.
    /// </summary>
    /// <param name="device">The <see cref="AudioDevice"/> to check.</param>
    /// <returns><see langword="true"/> if the device is the default device; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="device"/> is <see langword="null"/>.</exception>
    public static bool IsDefaultDevice(this AudioDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        return device.IsDefault;
    }
}
