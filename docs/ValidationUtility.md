# ValidationUtility
The `ValidationUtility` class provides a set of static methods for validating various parameters and data used in audio visualization. It helps ensure that the input values are within the expected ranges and formats, preventing potential errors and exceptions. This utility class is designed to be used throughout the `naudio-visualizer` project to maintain data integrity and consistency.

## API
The `ValidationUtility` class offers the following public members:
* `ValidateSampleRate`: Validates a sample rate value. Returns `true` if the sample rate is valid, `false` otherwise.
* `ValidateFftSize`: Validates an FFT size value. Returns `true` if the FFT size is valid, `false` otherwise.
* `ValidateChannelCount`: Validates a channel count value. Returns `true` if the channel count is valid, `false` otherwise.
* `ValidateFps`: Validates an FPS (frames per second) value. Returns `true` if the FPS is valid, `false` otherwise.
* `ValidateFrequency`: Validates a frequency value. Returns `true` if the frequency is valid, `false` otherwise.
* `ValidateAudioData`: Validates audio data. Returns `true` if the audio data is valid, `false` otherwise.
* `ValidateAmplitude`: Validates an amplitude value. Returns `true` if the amplitude is valid, `false` otherwise.
* `ValidateFilePath`: Validates a file path. Returns `true` if the file path is valid, `false` otherwise.
* `ValidateDuration`: Validates a duration value. Returns `true` if the duration is valid, `false` otherwise.
* `ValidateDeviceIndex`: Validates a device index value. Returns `true` if the device index is valid, `false` otherwise.
* `ValidateTimeInMs`: Validates a time in milliseconds value. Returns `true` if the time is valid, `false` otherwise.
* `ValidateNormalization`: Validates a normalization value. Returns `true` if the normalization is valid, `false` otherwise.
* `ValidateRequiredParameters`: Validates required parameters. Returns `true` if the parameters are valid, `false` otherwise.
* `ValidateCollection<T>`: Validates a collection of type `T`. Returns `true` if the collection is valid, `false` otherwise.
* `ThrowIfInvalid`: Throws an exception if the input is invalid.
* `ThrowIfNull`: Throws an exception if the input is null.
* `ThrowIfNullOrWhitespace`: Throws an exception if the input is null or whitespace.
* `ThrowIfOutOfRange`: Throws an exception if the input is out of range.

## Usage
Here are two examples of using the `ValidationUtility` class:
```csharp
// Example 1: Validating audio data
if (ValidationUtility.ValidateAudioData(audioData))
{
    // Process the audio data
}
else
{
    // Handle invalid audio data
}

// Example 2: Validating a file path
string filePath = "path/to/audio/file.wav";
if (ValidationUtility.ValidateFilePath(filePath))
{
    // Use the file path
}
else
{
    // Handle invalid file path
}
```

## Notes
When using the `ValidationUtility` class, keep in mind the following edge cases:
* The `Validate` methods return `false` for invalid input, but do not throw exceptions. Instead, use the `ThrowIf` methods to throw exceptions for invalid input.
* The `ThrowIf` methods throw exceptions immediately if the input is invalid.
* The `ValidationUtility` class is designed to be thread-safe, as it only contains static methods and does not maintain any state. However, the validity of the input data is not guaranteed to be consistent across threads. It is the responsibility of the caller to ensure that the input data is valid and consistent in a multi-threaded environment.
