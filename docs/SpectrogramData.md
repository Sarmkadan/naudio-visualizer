# SpectrogramData

`SpectrogramData` provides a structured representation for storing, accessing, and manipulating audio spectrogram information. It serves as a container for frequency domain data, allowing for efficient processing, transformation, and retrieval of spectral components—derived from original signal data—for visualization or further analytical tasks within the `naudio-visualizer` framework.

## API

### Properties

*   `public int FrequencyBins`: The total number of frequency bins per time frame.
*   `public int TimeFrames`: The total number of time frames currently stored.
*   `public int SampleRate`: The sample rate (in Hz) of the original audio signal.
*   `public int FftSize`: The size of the Fast Fourier Transform window used for frequency analysis.
*   `public int HopSize`: The number of samples skipped between consecutive FFT frames.
*   `public float TimePerFrame`: The temporal duration of a single spectrum frame in seconds.
*   `public float FrequencyResolution`: The frequency spacing between bins, measured in Hz.
*   `public ColormapType ColormapType`: Defines the color mapping strategy used to represent amplitude values.

### Constructors

*   `public SpectrogramData()`: Initializes a new, empty instance of the `SpectrogramData` class.

### Methods

*   `public override float[] GetData()`: Returns the raw, underlying flat array representation of the spectrogram data.
*   `public float[] GetTimeFrame(int frameIndex)`: Retrieves the spectral data for a specific time frame index.
*   `public float[] GetFrequencySlice(int binIndex)`: Retrieves the amplitude values for a specific frequency bin across all time frames.
*   `public void AddSpectrumFrame(SpectrumData frame)`: Appends a new `SpectrumData` frame to the collection.
*   `public IReadOnlyList<SpectrumData> GetSpectrumFrames()`: Returns a read-only list of all currently stored `SpectrumData` frames.
*   `public override void Normalize()`: Scales the amplitude values of the spectrogram data to a normalized range (e.g., 0.0 to 1.0).
*   `public void ApplyLogScale()`: Applies a logarithmic scale transformation to the amplitude values to better match human auditory perception.
*   `public override bool IsValid()`: Validates the integrity of the data, returning `true` if the spectrogram data is correctly structured and contains valid values.

## Usage

### Populating SpectrogramData
```csharp
var spectrogram = new SpectrogramData();
// ... assume spectrumFrame is populated from audio processing ...
spectrogram.AddSpectrumFrame(spectrumFrame);

if (spectrogram.IsValid())
{
    spectrogram.ApplyLogScale();
    spectrogram.Normalize();
}
```

### Accessing Spectrogram Data
```csharp
// Retrieve a specific time frame for visualization
float[] currentFrameData = spectrogram.GetTimeFrame(0);

// Retrieve temporal data for the first frequency bin
float[] lowFrequencyTrend = spectrogram.GetFrequencySlice(0);
```

## Notes

*   **Thread Safety:** Instances of `SpectrogramData` are not thread-safe. Concurrent access, particularly calls to `AddSpectrumFrame` while reading data (e.g., `GetTimeFrame` or `GetData`), requires external synchronization to prevent data corruption or inconsistent states.
*   **Validation:** Always verify the integrity of the data using `IsValid()` before performing operations like `Normalize()` or `ApplyLogScale()`, especially if the object has been populated via asynchronous or stream-based processes.
*   **Edge Cases:** Accessing `GetTimeFrame` or `GetFrequencySlice` with an out-of-bounds index will throw an `ArgumentOutOfRangeException`. Ensure indices are verified against `TimeFrames` and `FrequencyBins` properties respectively prior to access.
