# SpectrumData

The `SpectrumData` class encapsulates and manipulates results derived from Fast Fourier Transform (FFT) analysis within audio visualization workflows. It provides properties to describe the characteristics of the spectral analysis, such as sample rate and frequency resolution, and offers methods to normalize, smooth, and adjust the scaling of the spectrum data for rendering or further analysis.

## API

### Properties
*   **`SampleRate`** (`int`): The audio sampling rate in Hz used for the FFT analysis.
*   **`FftSize`** (`int`): The number of samples used in the FFT window. This must typically be a power of two.
*   **`WindowType`** (`WindowType`): The type of windowing function applied to the time-domain signal prior to the FFT.
*   **`IsLogScale`** (`bool`): Indicates if the frequency spectrum data is currently represented on a logarithmic scale.
*   **`FrequencyResolution`** (`float`): The width in Hz of each individual frequency bin.
*   **`PeakFrequency`** (`float`): The frequency corresponding to the bin with the highest magnitude in the current spectrum.
*   **`PeakMagnitude`** (`float`): The magnitude value of the highest frequency bin.
*   **`IsValid`** (`bool`): Indicates whether the current spectrum data is valid and suitable for further processing or rendering. Overrides the base class implementation.

### Methods
*   **`SpectrumData()`**: Initializes a new instance of the `SpectrumData` class.
*   **`GetData()`** (`float[]`): Returns an array containing the magnitude values of the current spectrum. Overrides the base class implementation.
*   **`GetFrequencies()`** (`float[]`): Returns an array of frequency values corresponding to each magnitude bin in the spectrum.
*   **`ConvertToLogScale()`** (`void`): Transforms the linear frequency representation of the spectrum into a logarithmic scale.
*   **`Normalize()`** (`void`): Normalizes the spectrum magnitude values. Overrides the base class implementation.
*   **`SmoothSpectrum()`** (`void`): Applies a smoothing algorithm to the spectrum data to reduce visual artifacts in real-time applications.

## Usage

### Example 1: Basic Initialization and Data Retrieval
```csharp
// Initialize spectrum analysis with a 2048 FFT size
var spectrum = new SpectrumData(sampleRate: 44100, fftSize: 2048, windowType: WindowType.Hann);

// Retrieve magnitude data and peak information
float[] magnitudes = spectrum.GetData();
float peakFreq = spectrum.PeakFrequency;

Console.WriteLine($"Peak frequency: {peakFreq} Hz");
```

### Example 2: Normalizing and Smoothing Data
```csharp
// Prepare data for rendering
if (spectrum.IsValid)
{
    // Apply smoothing to reduce jitter
    spectrum.SmoothSpectrum();
    
    // Normalize values to a [0, 1] range for visual scaling
    spectrum.Normalize();
    
    float[] processedData = spectrum.GetData();
    // Render processedData to UI...
}
```

## Notes

*   **Thread Safety**: The `SpectrumData` class is not inherently thread-safe. If an instance is being updated by an audio processing thread and read by a rendering thread simultaneously, external synchronization mechanisms (such as `lock` statements or concurrent buffers) are required to prevent data races.
*   **Validity**: Always check the `IsValid` property before accessing `GetData()` or performing transformations. Operations on an invalid instance may throw exceptions or result in undefined behavior.
*   **FFT Constraints**: The `FftSize` property is expected to be a power of two (e.g., 512, 1024, 2048) by most underlying FFT implementations. Providing an invalid size may result in an initialization failure or improper spectrum generation.
*   **Normalization**: The `Normalize()` method behavior depends on the implementation of the base class. It typically rescales the magnitude values based on the maximum observed peak or a fixed maximum threshold.
