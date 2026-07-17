# AudioDeviceExtensions

The `AudioDeviceExtensions` class provides a set of static extension methods designed to simplify the retrieval of hardware capabilities and configuration details for audio devices within the `naudio-visualizer` project. By extending the base device representation, this utility eliminates boilerplate code required to query sample rates, channel counts, bit depths, and manufacturer information, offering a unified interface for validating device compatibility and accessing default system settings.

## API

### IsSampleRateSupported
Determines whether a specific audio device supports a given sample rate.
*   **Parameters**: Takes the target device instance and an `int` representing the desired sample rate in Hertz.
*   **Returns**: `true` if the device supports the specified rate; otherwise, `false`.
*   **Throws**: No exceptions are thrown under normal operation; invalid device states typically result in `false`.

### GetSupportedSampleRates
Retrieves a comprehensive list of all sample rates supported by the audio device.
*   **Parameters**: Takes the target device instance.
*   **Returns**: An `IReadOnlyList<int>` containing supported sample rates in Hertz.
*   **Throws**: May throw an exception if the underlying driver fails to report capabilities or if the device handle is invalid.

### IsDefaultDevice
Checks if the current device is set as the system default for its category (capture or render).
*   **Parameters**: Takes the target device instance.
*   **Returns**: `true` if the device is the system default; otherwise, `false`.
*   **Throws**: No exceptions expected during standard execution.

### GetDefaultSampleRate
Fetches the default sample rate configured for the specific audio device.
*   **Parameters**: Takes the target device instance.
*   **Returns**: An `int` representing the default sample rate in Hertz.
*   **Throws**: Throws an exception if the device does not have a defined default rate or if the device is unavailable.

### GetChannelCount
Returns the number of audio channels (e.g., 2 for stereo, 1 for mono) supported or currently configured for the device.
*   **Parameters**: Takes the target device instance.
*   **Returns**: An `int` representing the channel count.
*   **Throws**: May throw if the device capabilities cannot be queried.

### GetBitDepth
Retrieves the bit depth (e.g., 16, 24, 32) used by the audio device.
*   **Parameters**: Takes the target device instance.
*   **Returns**: An `int` representing the bit depth in bits.
*   **Throws**: May throw if the device format information is inaccessible.

### GetManufacturer
Gets the name of the hardware manufacturer or driver vendor associated with the device.
*   **Parameters**: Takes the target device instance.
*   **Returns**: A `string` containing the manufacturer name. Returns an empty string if the information is unavailable.
*   **Throws**: No exceptions expected.

### GetName
Retrieves the friendly name of the audio device as displayed in the operating system.
*   **Parameters**: Takes the target device instance.
*   **Returns**: A `string` containing the device name.
*   **Throws**: No exceptions expected.

### GetCapabilities
Returns a structured object containing detailed technical specifications of the device.
*   **Parameters**: Takes the target device instance.
*   **Returns**: A `DeviceCapabilities` object encapsulating low-level device properties.
*   **Throws**: Throws an exception if the device handle is invalid or the underlying API call fails.

### IsAvailable
Verifies whether the audio device is currently present and accessible by the system.
*   **Parameters**: Takes the target device instance.
*   **Returns**: `true` if the device is ready for use; otherwise, `false`.
*   **Throws**: No exceptions are thrown; unavailability is indicated via the return value.

## Usage

### Example 1: Validating Device Compatibility
This example demonstrates how to check if a specific device is available and supports a required sample rate before initializing an audio stream.

```csharp
using NAudio.CoreAudioApi;
using NaudioVisualizer.Extensions;

public void InitializeStream(MMDevice device)
{
    if (!device.IsAvailable())
    {
        Console.WriteLine("Device is not currently available.");
        return;
    }

    const int RequiredSampleRate = 48000;
    
    if (!device.IsSampleRateSupported(RequiredSampleRate))
    {
        var supported = device.GetSupportedSampleRates();
        Console.WriteLine($"Rate {RequiredSampleRate} not supported. Available: {string.Join(", ", supported)}");
        return;
    }

    Console.WriteLine($"Initializing with {device.GetChannelCount()} channels at {RequiredSampleRate}Hz");
    // Proceed with initialization...
}
```

### Example 2: Reporting Device Configuration
This example retrieves and displays detailed metadata about the default rendering device for logging or UI display purposes.

```csharp
using NAudio.CoreAudioApi;
using NaudioVisualizer.Extensions;

public void LogDeviceDetails(MMDevice device)
{
    if (!device.IsDefaultDevice())
    {
        Console.WriteLine("Warning: This is not the system default device.");
    }

    var info = new
    {
        Name = device.GetName(),
        Manufacturer = device.GetManufacturer(),
        SampleRate = device.GetDefaultSampleRate(),
        BitDepth = device.GetBitDepth(),
        Capabilities = device.GetCapabilities()
    };

    Console.WriteLine($"Device: {info.Name} ({info.Manufacturer})");
    Console.WriteLine($"Format: {info.SampleRate}Hz, {info.BitDepth}-bit");
}
```

## Notes

*   **Thread Safety**: These extension methods are stateless and thread-safe regarding their internal logic. However, they rely on the underlying `MMDevice` or device wrapper instance passed as the `this` parameter. If the underlying device object is disposed or modified by another thread during execution, race conditions or `ObjectDisposedException` errors may occur. Callers must ensure the device instance remains valid for the duration of the call.
*   **Device Availability**: The `IsAvailable` method should be called before invoking methods like `GetDefaultSampleRate` or `GetCapabilities` in dynamic environments where devices can be unplugged (e.g., USB microphones). While some methods may return default values on failure, others will throw exceptions if the hardware handle is stale.
*   **Empty Results**: `GetSupportedSampleRates` returns an empty list rather than null if no rates are reported. `GetManufacturer` and `GetName` return empty strings if the metadata is missing from the driver, ensuring null-coalescing operators are not strictly required for string concatenation but should be considered for logic flow.
*   **Driver Dependencies**: The accuracy of `GetBitDepth` and `GetChannelCount` depends on the audio driver's ability to report current format settings. In some legacy driver scenarios, these methods may return the maximum supported capability rather than the currently active configuration.
