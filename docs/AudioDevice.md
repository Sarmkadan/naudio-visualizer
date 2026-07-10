# AudioDevice

Represents an audio endpoint device with metadata and capabilities, used for enumeration, selection, and status monitoring of audio devices in the `naudio-visualizer` project.

## API

### `public Guid Id`
A unique identifier for the audio device. Used internally for device lookup and comparison.

### `public string Name`
The human-readable name of the audio device (e.g., "Speakers (Realtek Audio)"). May be empty if the device name is unavailable.

### `public int DeviceIndex`
The zero-based index of the device in the system's audio device enumeration. Corresponds to the order returned by the underlying audio API.

### `public string Manufacturer`
The name of the device manufacturer (e.g., "Realtek"). May be empty if the manufacturer string is not provided by the system.

### `public int ChannelCount`
The number of audio channels supported by the device (e.g., 2 for stereo, 6 for 5.1 surround). Value is undefined if the device is unavailable.

### `public List<int> SupportedSampleRates`
A list of sample rates (in Hz) supported by the device (e.g., 44100, 48000). May be empty if the device does not expose sample rate support or is unavailable.

### `public int DefaultSampleRate`
The default sample rate (in Hz) for the device. Typically the most commonly used rate (e.g., 44100 or 48000). Undefined if the device is unavailable.

### `public int BitDepth`
The bit depth (e.g., 16, 24, 32) of the audio format supported by the device. Undefined if the device is unavailable.

### `public bool IsDefault`
Indicates whether the device is the system's default audio output device. Read-only; reflects the system state at the time of the last `UpdateStatus` call.

### `public bool IsAvailable`
Indicates whether the device is currently available and accessible. Updated via `UpdateStatus`. A device may become unavailable if unplugged or disabled.

### `public DateTime LastStatusCheck`
The timestamp of the last successful call to `UpdateStatus`. Used to detect stale device status.

### `public DeviceCapabilities Capabilities`
A container object describing advanced device capabilities (e.g., formats, controls). May be `null` if capabilities could not be retrieved.

### `public AudioDevice public bool IsValid`
Indicates whether the `AudioDevice` instance represents a valid, initialized device. `false` if the device was constructed with invalid or missing data.

### `public bool SupportsSampleRate(int sampleRate)`
Determines whether the device supports a specific sample rate.

- **Parameters**: `sampleRate` — The sample rate (in Hz) to check.
- **Return value**: `true` if the rate is in `SupportedSampleRates`; otherwise, `false`.
- **Throws**: `ArgumentOutOfRangeException` if `sampleRate` is less than or equal to zero.

### `public void AddSupportedSampleRate(int sampleRate)`
Adds a sample rate to the `SupportedSampleRates` list if it is not already present.

- **Parameters**: `sampleRate` — The sample rate (in Hz) to add.
- **Throws**: `ArgumentOutOfRangeException` if `sampleRate` is less than or equal to zero.

### `public void UpdateStatus()`
Refreshes the device's availability, default status, and capabilities by querying the system. Updates `IsAvailable`, `IsDefault`, `ChannelCount`, `SupportedSampleRates`, `DefaultSampleRate`, `BitDepth`, and `Capabilities`.

- **Throws**: May throw platform-specific exceptions if the underlying audio subsystem is unreachable or returns an error.

### `public override string ToString()`
Returns a formatted string representation of the device, including `Name`, `Manufacturer`, `DeviceIndex`, and `IsDefault`.

- **Return value**: A string in the format: `"[DeviceIndex] Name (Manufacturer) - Default: IsDefault"`.

## Usage
