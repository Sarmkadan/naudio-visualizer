# AudioStreamException

The `AudioStreamException` class serves as the primary exception type for errors encountered during audio stream processing within the `naudio-visualizer` application. It is designed to capture and report specific failure conditions, facilitating robust error handling and diagnostic reporting by providing an associated `AudioStreamErrorCode` that allows calling code to programmatically differentiate between various types of stream-related failures.

## API

### Properties

#### `public AudioStreamErrorCode ErrorCode`
Gets the `AudioStreamErrorCode` associated with this exception instance. This property allows consumers to identify the specific nature of the audio stream failure and determine the appropriate recovery strategy or logging level.

### Constructors

#### `public AudioStreamException()`
Initializes a new instance of the `AudioStreamException` class with default values.

#### `public AudioStreamException(string message)`
Initializes a new instance of the `AudioStreamException` class with a specified error message.

#### `public AudioStreamException(string message, Exception innerException)`
Initializes a new instance of the `AudioStreamException` class with a specified error message and a reference to the inner exception that is the cause of this exception.

## Usage

### Handling Audio Stream Errors
```csharp
try
{
    // Attempting to interact with an audio stream
    audioProcessor.Process();
}
catch (AudioStreamException ex)
{
    if (ex.ErrorCode == AudioStreamErrorCode.DeviceDisconnected)
    {
        // Handle audio device disconnection gracefully
    }
    else
    {
        // Log other audio stream errors
        Logger.LogError($"Audio stream error: {ex.Message}");
    }
}
```

### Throwing Custom Audio Stream Exceptions
```csharp
public void InitializeStream(Stream stream)
{
    if (stream == null)
    {
        throw new AudioStreamException("The required audio stream is null.")
        {
            // Ensure the appropriate error code is set
            // ErrorCode = AudioStreamErrorCode.StreamNotFound
        };
    }
}
```

## Notes

- **Thread-Safety:** `AudioStreamException` instances are thread-safe for read operations after creation. As with standard .NET exceptions, the properties of the exception object should not be modified after it has been thrown.
- **Edge Cases:** If an `AudioStreamException` is caught, ensure that the `ErrorCode` property is validated. Depending on the constructor used, it may contain default values if not explicitly set during object construction or via object initializer syntax.
