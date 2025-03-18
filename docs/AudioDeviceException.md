# AudioDeviceException

`AudioDeviceException` is a custom exception class in the `naudio-visualizer` project designed to signal errors that occur during operations involving specific audio hardware devices. It provides necessary context, including the index of the device involved, to facilitate robust error handling and diagnostic tracking within audio processing workflows.

## API

### Properties

#### `public int? DeviceIndex`
Gets the zero-based index of the audio device that triggered the exception. This property returns `null` if the exception is not associated with a specific device index.

### Constructors

#### `public AudioDeviceException()`
Initializes a new instance of the `AudioDeviceException` class with default values.

#### `public AudioDeviceException(string message)`
Initializes a new instance of the `AudioDeviceException` class with a specified error message.

*   **Parameters:**
    *   `message`: A string that describes the error.

#### `public AudioDeviceException(string message, int deviceIndex)`
Initializes a new instance of the `AudioDeviceException` class with a specified error message and the index of the audio device associated with the failure.

*   **Parameters:**
    *   `message`: A string that describes the error.
    *   `deviceIndex`: The index of the audio device.

#### `public AudioDeviceException` (Constructor Overload)
Initializes a new instance of the `AudioDeviceException` class, typically used for wrapping inner exceptions.

## Usage

### Throwing an exception with a device index
```csharp
public void InitializeDevice(int index)
{
    if (index < 0 || index >= DeviceCapabilities.Count)
    {
        throw new AudioDeviceException("The specified audio device index is out of range.", index);
    }
    // Proceed with device initialization
}
```

### Catching and processing the exception
```csharp
try
{
    audioSource.Start(deviceIndex);
}
catch (AudioDeviceException ex)
{
    int index = ex.DeviceIndex ?? -1;
    Logger.LogError($"Error occurred on device {index}: {ex.Message}");
}
```

## Notes

*   **Thread Safety:** As an exception type, `AudioDeviceException` is intended to be immutable once instantiated and thrown. It is safe for use across threads, provided standard C# exception handling and propagation patterns are followed.
*   **Edge Cases:** If `DeviceIndex` is `null`, consuming code should be prepared to handle the `null` value gracefully, often by treating it as an unknown or global device failure. Avoid assuming that a `DeviceIndex` is always present in all instances of this exception.
