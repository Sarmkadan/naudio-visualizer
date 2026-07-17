namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides extension methods for <see cref="AudioDevice"/> to enhance device manipulation and querying.
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
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="sampleRate"/> is less than or equal to zero.</exception>
    public static bool IsSampleRateSupported(this AudioDevice device, int sampleRate)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sampleRate);
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

    /// <summary>
    /// Gets the default sample rate for the <paramref name="device"/>.
    /// </summary>
    /// <param name="device">The <see cref="AudioDevice"/> to get the default sample rate for.</param>
    /// <returns>The default sample rate in Hz.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="device"/> is <see langword="null"/>.</exception>
    public static int GetDefaultSampleRate(this AudioDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        return device.DefaultSampleRate;
    }

    /// <summary>
    /// Gets the number of input channels for the <paramref name="device"/>.
    /// </summary>
    /// <param name="device">The <see cref="AudioDevice"/> to get the channel count for.</param>
    /// <returns>The number of input channels.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="device"/> is <see langword="null"/>.</exception>
    public static int GetChannelCount(this AudioDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        return device.ChannelCount;
    }

    /// <summary>
    /// Gets the bit depth for the <paramref name="device"/>.
    /// </summary>
    /// <param name="device">The <see cref="AudioDevice"/> to get the bit depth for.</param>
    /// <returns>The bit depth in bits (16, 24, or 32).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="device"/> is <see langword="null"/>.</exception>
    public static int GetBitDepth(this AudioDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        return device.BitDepth;
    }

    /// <summary>
    /// Gets the manufacturer name for the <paramref name="device"/>.
    /// </summary>
    /// <param name="device">The <see cref="AudioDevice"/> to get the manufacturer for.</param>
    /// <returns>The manufacturer name.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="device"/> is <see langword="null"/>.</exception>
    public static string GetManufacturer(this AudioDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        return device.Manufacturer;
    }

    /// <summary>
    /// Gets the device name.
    /// </summary>
    /// <param name="device">The <see cref="AudioDevice"/> to get the name for.</param>
    /// <returns>The device name.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="device"/> is <see langword="null"/>.</exception>
    public static string GetName(this AudioDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        return device.Name;
    }

    /// <summary>
    /// Gets the device capabilities.
    /// </summary>
    /// <param name="device">The <see cref="AudioDevice"/> to get capabilities for.</param>
    /// <returns>The <see cref="DeviceCapabilities"/> flags.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="device"/> is <see langword="null"/>.</exception>
    public static DeviceCapabilities GetCapabilities(this AudioDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        return device.Capabilities;
    }

    /// <summary>
    /// Determines whether the device is currently available.
    /// </summary>
    /// <param name="device">The <see cref="AudioDevice"/> to check availability for.</param>
    /// <returns><see langword="true"/> if the device is available; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="device"/> is <see langword="null"/>.</exception>
    public static bool IsAvailable(this AudioDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        return device.IsAvailable;
    }
}
