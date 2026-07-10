# AudioDataConverter

The `AudioDataConverter` class serves as a centralized utility for performing common audio data transformations, mathematical conversions, and formatting operations within the `naudio-visualizer` project. It provides static methods for converting between decibel and linear scales, transforming sample formats (float to PCM and vice versa), manipulating multi-channel audio data, calculating signal levels (RMS and Peak), and generating human-readable strings for frequency, duration, and audio levels. This class is designed to be stateless and thread-safe, acting as a pure function library for audio processing pipelines.

## API

### `DbToLinear`
Converts a decibel value to its corresponding linear amplitude ratio.
*   **Parameters**: `float db` – The value in decibels.
*   **Returns**: `float` – The linear amplitude value.
*   **Throws**: None. Returns `0f` for negative infinity equivalents if the input is sufficiently low.

### `LinearToDb`
Converts a linear amplitude ratio to its corresponding decibel value.
*   **Parameters**: `float linear` – The linear amplitude value.
*   **Returns**: `float` – The value in decibels.
*   **Throws**: None. Handles zero or negative inputs by returning a representative minimum decibel floor.

### `FormatFrequency`
Formats a frequency value in Hertz into a human-readable string with appropriate units (Hz, kHz, MHz).
*   **Parameters**: `double frequency` – The frequency value in Hertz.
*   **Returns**: `string` – The formatted frequency string (e.g., "44.1 kHz").
*   **Throws**: None.

### `FormatDuration`
Formats a time span represented in seconds into a human-readable duration string.
*   **Parameters**: `double seconds` – The duration in seconds.
*   **Returns**: `string` – The formatted duration string (e.g., "03:45" or "1h 20m").
*   **Throws**: None.

### `FormatAudioLevel`
Formats a linear audio level into a percentage-based string representation.
*   **Parameters**: `float level` – The linear audio level (typically 0.0 to 1.0).
*   **Returns**: `string` – The formatted percentage string (e.g., "75%").
*   **Throws**: None.

### `FormatAudioLevelDb`
Formats a linear audio level into a decibel string representation.
*   **Parameters**: `float level` – The linear audio level.
*   **Returns**: `string` – The formatted decibel string (e.g., "-12.5 dB").
*   **Throws**: None.

### `FloatToInt16Pcm`
Converts an array of floating-point samples (normalized -1.0 to 1.0) into a byte array representing 16-bit PCM data.
*   **Parameters**: `float[] samples` – The array of float samples.
*   **Returns**: `byte[]` – The resulting byte array containing interleaved 16-bit integer data.
*   **Throws**: `ArgumentNullException` if `samples` is null.

### `Int16PcmToFloat`
Converts a byte array representing 16-bit PCM data into an array of normalized floating-point samples.
*   **Parameters**: `byte[] pcmData` – The byte array containing 16-bit PCM data.
*   **Returns**: `float[]` – The array of float samples normalized between -1.0 and 1.0.
*   **Throws**: `ArgumentNullException` if `pcmData` is null. Throws `ArgumentException` if the byte array length is not even.

### `ExtractChannel`
Extracts a single channel from an interleaved multi-channel float array.
*   **Parameters**: `float[] interleavedSamples` – The source array containing interleaved channel data. `int channelIndex` – The zero-based index of the channel to extract. `int totalChannels` – The total number of channels in the source data.
*   **Returns**: `float[]` – A new array containing only the samples for the specified channel.
*   **Throws**: `ArgumentNullException` if the sample array is null. `ArgumentOutOfRangeException` if `channelIndex` is invalid or `totalChannels` is less than 1.

### `InterleaveChannels`
Combines multiple single-channel float arrays into a single interleaved multi-channel array.
*   **Parameters**: `float[][] channels` – A jagged array where each element is a single channel's sample data.
*   **Returns**: `float[]` – The interleaved array containing data from all channels.
*   **Throws**: `ArgumentNullException` if `channels` is null or contains null elements. `ArgumentException` if channel arrays have mismatched lengths.

### `CalculateRmsLevel`
Calculates the Root Mean Square (RMS) level of a given sample array, representing the average power of the signal.
*   **Parameters**: `float[] samples` – The array of audio samples.
*   **Returns**: `float` – The calculated RMS level.
*   **Throws**: `ArgumentNullException` if `samples` is null. Returns `0f` if the array is empty.

### `CalculatePeakLevel`
Calculates the peak amplitude level (maximum absolute value) of a given sample array.
*   **Parameters**: `float[] samples` – The array of audio samples.
*   **Returns**: `float` – The peak level found in the array.
*   **Throws**: `ArgumentNullException` if `samples` is null. Returns `0f` if the array is empty.

### `NormalizeSamples`
Scales an array of samples so that the peak amplitude reaches a target level (defaulting to 1.0).
*   **Parameters**: `float[] samples` – The array of audio samples. `float targetPeak` – (Optional) The desired peak level after normalization.
*   **Returns**: `float[]` – A new array containing the normalized samples.
*   **Throws**: `ArgumentNullException` if `samples` is null.

### `ApplyGain`
Applies a gain factor to an array of samples.
*   **Parameters**: `float[] samples` – The array of audio samples. `float gain` – The multiplication factor for the gain.
*   **Returns**: `float[]` – A new array containing the gain-adjusted samples.
*   **Throws**: `ArgumentNullException` if `samples` is null.

## Usage

### Example 1: Converting Raw PCM Data and Calculating Levels
This example demonstrates reading raw 16-bit PCM data, converting it to float for processing, calculating the RMS level, and converting that level to a decibel string for display.

```csharp
using System;
using naudio_visualizer.Utilities; // Hypothetical namespace

public class AudioProcessor
{
    public void ProcessBuffer(byte[] rawPcmData)
    {
        if (rawPcmData == null || rawPcmData.Length == 0) return;

        // Convert 16-bit PCM bytes to normalized floats
        float[] samples = AudioDataConverter.Int16PcmToFloat(rawPcmData);

        // Calculate the RMS level
        float rms = AudioDataConverter.CalculateRmsLevel(samples);

        // Format the level for UI display
        string dbDisplay = AudioDataConverter.FormatAudioLevelDb(rms);
        
        Console.WriteLine($"Current Audio Level: {dbDisplay}");
    }
}
```

### Example 2: Channel Extraction and Normalization
This example shows how to extract the left channel from a stereo stream, normalize it to prevent clipping, and apply additional gain before output.

```csharp
using System;
using naudio_visualizer.Utilities;

public class ChannelMixer
{
    public float[] PrepareLeftChannel(float[] stereoInterleavedData)
    {
        if (stereoInterleavedData == null) throw new ArgumentNullException(nameof(stereoInterleavedData));
        
        // Extract the left channel (index 0) from stereo data
        float[] leftChannel = AudioDataConverter.ExtractChannel(
            stereoInterleavedData, 
            channelIndex: 0, 
            totalChannels: 2
        );

        // Normalize the channel to maximize dynamic range without clipping
        float[] normalized = AudioDataConverter.NormalizeSamples(leftChannel, 0.95f);

        // Apply a slight gain boost
        float[] finalOutput = AudioDataConverter.ApplyGain(normalized, 1.2f);

        return finalOutput;
    }
}
```

## Notes

*   **Thread Safety**: All methods in `AudioDataConverter` are static and operate solely on their input parameters without maintaining internal state. Consequently, the class is fully thread-safe and can be called concurrently from multiple threads without locking.
*   **Memory Allocation**: Methods that transform data (e.g., `Int16PcmToFloat`, `ExtractChannel`, `NormalizeSamples`, `ApplyGain`) allocate and return new arrays. They do not modify the input arrays in place. Callers should be mindful of memory pressure when processing large audio buffers in tight loops.
*   **Edge Cases**:
    *   **Empty Arrays**: Level calculation methods (`CalculateRmsLevel`, `CalculatePeakLevel`) return `0f` when provided with an empty array rather than throwing an exception.
    *   **Zero/Negative Values**: Conversion methods `LinearToDb` and `DbToLinear` handle mathematical singularities (such as log(0)) by clamping results to a safe minimum floor value to prevent `NaN` or `Infinity` propagation.
    *   **Data Alignment**: `Int16PcmToFloat` expects the input byte array length to be even (multiples of 2 bytes per sample). An odd-length array will result in an `ArgumentException`.
*   **Precision**: Floating-point operations utilize standard `float` (single-precision) arithmetic. While sufficient for visualization and general playback, cumulative operations (like repeated `ApplyGain` calls on the same data) may introduce minor precision errors over time.
