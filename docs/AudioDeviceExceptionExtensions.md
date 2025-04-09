# AudioDeviceExceptionExtensions
The `AudioDeviceExceptionExtensions` class provides a set of extension methods for the `AudioDeviceException` type, allowing for more convenient and expressive handling of audio device-related exceptions. These methods enable developers to easily extract relevant information from exceptions, log errors, and perform other common tasks.

## API
* `public static int GetDeviceIndexOrDefault(this AudioDeviceException exception)`: Retrieves the device index associated with the exception, or a default value if no index is available. This method does not throw any exceptions.
* `public static AudioDeviceException WithMessage(this AudioDeviceException exception, string message)`: Creates a new `AudioDeviceException` instance with the specified message. This method does not throw any exceptions.
* `public static string ToLogString(this AudioDeviceException exception)`: Converts the exception into a string suitable for logging purposes. This method does not throw any exceptions.
* `public static bool HasDeviceIndex(this AudioDeviceException exception)`: Determines whether the exception has a valid device index associated with it. This method does not throw any exceptions.

## Usage
The following examples demonstrate how to use the `AudioDeviceExceptionExtensions` methods:
```csharp
try
{
    // Attempt to access an audio device
    var device = new AudioDevice();
    device.Initialize();
}
catch (AudioDeviceException ex)
{
    // Log the exception with a custom message
    var loggedException = ex.WithMessage("Failed to initialize audio device");
    Console.WriteLine(loggedException.ToLogString());
    
    // Check if the exception has a device index
    if (ex.HasDeviceIndex())
    {
        Console.WriteLine("Device index: " + ex.GetDeviceIndexOrDefault());
    }
}

// Create a new exception with a custom message
var customException = new AudioDeviceException().WithMessage("Custom audio device error");
Console.WriteLine(customException.ToLogString());
```

## Notes
When using the `AudioDeviceExceptionExtensions` methods, consider the following edge cases:
* If an `AudioDeviceException` instance does not have a device index associated with it, `GetDeviceIndexOrDefault` will return a default value.
* The `WithMessage` method creates a new exception instance, which can be useful for logging purposes but may also lead to increased memory allocation if used excessively.
* The `ToLogString` method is designed for logging purposes and may not provide a complete or detailed representation of the exception.
* The `HasDeviceIndex` method is a simple boolean check and does not throw any exceptions.
* These extension methods are thread-safe, as they only operate on the input `AudioDeviceException` instance and do not access any shared state. However, the underlying `AudioDeviceException` instance may still be subject to threading constraints depending on its implementation.
