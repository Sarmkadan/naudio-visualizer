# WaveformData

The `WaveformData` class is designed to encapsulate and manage audio peak data, providing essential functionality for processing, normalizing, and optimizing raw audio samples for visualization purposes. It supports stereo audio splitting, amplitude normalization, and data reduction through downsampling, ensuring that audio data is in a suitable format for efficient rendering.

## API

### SampleRate
The sample rate of the audio data, measured in Hertz (Hz).

### ChannelCount
The number of channels present in the audio data (e.g., 1 for mono, 2 for stereo).

### DownsamplingFactor
The factor currently applied to reduce the audio data resolution. A value of 1 indicates no downsampling.

### LeftChannelPeaks
An array of float values representing the peak levels for the left audio channel. This may be `null` if the data is mono or has not been processed.

### RightChannelPeaks
An array of float values representing the peak levels for the right audio channel. This may be `null` if the data is mono or has not been processed.

### WaveformData()
Initializes a new instance of the `WaveformData` class.

### GetData
Retrieves the processed audio data as a float array.

### SetSamples(float[] samples)
Populates the internal buffer with raw audio samples for processing.

### Normalize
Normalizes the audio peak levels to a standard amplitude range (typically 0.0 to 1.0) to ensure consistent visualization scaling.

### CalculateStereoChannels
Processes the loaded samples to split the audio stream and calculate distinct peak levels for the left and right channels.

### Downsample(int factor)
Reduces the number of samples in the data by the specified factor, effectively lowering the resolution for improved visualization performance.

### IsValid
A boolean indicator that returns `true` if the instance contains valid, processed audio data ready for visualization.

## Usage

### Basic Initialization and Processing
```csharp
var waveform = new WaveformData();
float[] rawSamples = LoadSamplesFromSource(); // Assume this retrieves float[]

waveform.SetSamples(rawSamples);
waveform.CalculateStereoChannels();
waveform.Normalize();

if (waveform.IsValid)
{
    // Ready to render using waveform.LeftChannelPeaks or waveform.RightChannelPeaks
}
```

### Downsampling for Performance
```csharp
var waveform = new WaveformData();
waveform.SetSamples(rawSamples);
waveform.CalculateStereoChannels();

// Downsample by a factor of 4 to reduce data points for high-performance UI rendering
waveform.Downsample(4);
waveform.Normalize();

float[] renderData = waveform.GetData;
```

## Notes

*   **Null Checks**: The `LeftChannelPeaks` and `RightChannelPeaks` properties can be `null` if the data has not been processed or if the input audio is mono. Always verify these properties before accessing them to avoid `NullReferenceException`.
*   **Thread Safety**: This class is not inherently thread-safe. If an instance of `WaveformData` is accessed or modified by multiple threads, appropriate external synchronization mechanisms must be implemented.
*   **Downsampling**: The `Downsample` method should be called after `CalculateStereoChannels` if stereo processing is required. Calling `Downsample` frequently on very large datasets may impact performance.
*   **Normalization**: The `Normalize` method scales the peak values based on the maximum detected amplitude within the current dataset. If the dataset changes, `Normalize` may need to be invoked again to maintain consistent scaling.
