# SpectrogramDataExtensions

Provides a set of pure‑functional extension methods for manipulating and analyzing `SpectrogramData` instances. The methods operate on immutable copies, leaving the original data unchanged, and are intended for use in audio visualization pipelines where frequency‑domain data needs to be normalized, sliced, masked, or summarized.

## API

### NormalizeFrequencyBins
```csharp
public static SpectrogramData NormalizeFrequencyBins(this SpectrogramData data)
```
*Purpose* – Scales the magnitude values in each frequency bin so that the maximum amplitude across the entire spectrogram equals 1.0, preserving relative dynamics while bringing the data into a comparable range.  
*Parameters* – `data`: The spectrogram to normalize.  
*Return value* – A new `SpectrogramData` instance containing the normalized magnitudes; the original `data` is unaltered.  
*Exceptions* –  
- `ArgumentNullException` if `data` is `null`.  
- `InvalidOperationException` if the spectrogram contains no frequency bins (zero‑length `Frequencies` array).

### ExtractRegion
```csharp
public static SpectrogramData ExtractRegion(
    this SpectrogramData data,
    double startTime,
    double endTime,
    int startFreqIndex,
    int endFreqIndex)
```
*Purpose* – Returns a sub‑spectrogram covering the requested time interval and frequency index range.  
*Parameters* –  
- `data`: Source spectrogram.  
- `startTime`: Inclusive start time in seconds (must be ≥ 0 and < `data.Duration`).  
- `endTime`: Exclusive end time in seconds (must be > `startTime` and ≤ `data.Duration`).  
- `startFreqIndex`: Inclusive index of the first frequency bin to include (0 ≤ `startFreqIndex` < `data.FrequencyCount`).  
- `endFreqIndex`: Exclusive index after the last frequency bin to include (`startFreqIndex` < `endFreqIndex` ≤ `data.FrequencyCount`).  
*Return value* – A new `SpectrogramData` containing only the magnitude values within the specified bounds; frequency and time axes are adjusted accordingly.  
*Exceptions* –  
- `ArgumentNullException` if `data` is `null`.  
- `ArgumentOutOfRangeException` if any of the time or frequency arguments fall outside the valid ranges or if `startTime ≥ endTime` or `startFreqIndex ≥ endFreqIndex`.  
- `ArgumentException` if the resulting region would have zero width in either dimension.

### ApplyFrequencyMask
```csharp
public static SpectrogramData ApplyFrequencyMask(
    this SpectrogramData data,
    bool[] mask)
```
*Purpose* – Zeroes out magnitude values for frequency bins where the corresponding mask entry is `false`, effectively suppressing or preserving selected frequency ranges.  
*Parameters* –  
- `data`: Source spectrogram.  
- `mask`: Boolean array whose length must equal `data.FrequencyCount`; `true` keeps the bin, `false` silences it.  
*Return value* – A new `SpectrogramData` with masked frequency bins set to zero; time axis unchanged.  
*Exceptions* –  
- `ArgumentNullException` if `data` or `mask` is `null`.  
- `ArgumentException` if `mask.Length` does not match `data.FrequencyCount`.

### CalculateSpectralCentroids
```csharp
public static float[] CalculateSpectralCentroids(this SpectrogramData data)
```
*Purpose* – Computes the spectral centroid for each time frame, providing a measure of the “brightness” or center of mass of the magnitude spectrum.  
*Parameters* – `data`: Spectrogram to analyze.  
*Return value* – A float array of length `data.TimeCount` where each element is the centroid frequency (in Hz) for the corresponding time slice.  
*Exceptions* –  
- `ArgumentNullException` if `data` is `null`.  
- `InvalidOperationException` if any time frame contains zero total energy (sum of magnitudes equals zero), which would cause a division by zero when computing the weighted average.

## Usage

### Normalizing and visualizing a spectrogram
```csharp
using NAudio.Visualizer;

// Assume 'raw' is a SpectrogramData obtained from an STFT.
SpectrogramData normalized = raw.NormalizeFrequencyBins();

// Pass 'normalized' to a rendering component that expects values in [0,1].
spectrogramView.SetData(normalized);
```

### Isolating a frequency band and computing its centroid over time
```csharp
using NAudio.Visualizer;

// Extract the region covering 200 Hz–2000 Hz for the first 5 seconds.
SpectrogramData band = raw.ExtractRegion(
    startTime: 0.0,
    endTime: 5.0,
    startFreqIndex: raw.FrequencyIndexForHz(200),
    endFreqIndex:   raw.FrequencyIndexForHz(2000));

// Apply a mask to further attenuate harmonics above 1500 Hz.
bool[] mask = new bool[band.FrequencyCount];
for (int i = 0; i < band.FrequencyCount; i++)
{
    double hz = band.HzForBin(i);
    mask[i] = hz <= 1500.0;
}
SpectrogramData masked = band.ApplyFrequencyMask(mask);

// Compute the spectral centroid of the masked band.
float[] centroids = masked.CalculateSpectralCentroids();

// 'centroids' can now be plotted or used for feature extraction.
```

## Notes

- All methods are **pure**: they never modify the input `SpectrogramData` instance and are safe to call concurrently from multiple threads as long as the supplied arguments are not mutated during the call.  
- The returned instances allocate new internal arrays for magnitude data; callers are responsible for disposing of any unmanaged resources if the `SpectrogramData` type implements `IDisposable` (the current implementation does not).  
- When extracting a region, the resulting spectrogram’s time and frequency axes are rescaled to start at zero; callers needing absolute timestamps should retain the original `startTime` and `startFreqIndex` values.  
- `CalculateSpectralCentroids` returns `0.0` for frames where the total magnitude is zero only if the input data already contains such frames; otherwise an exception is thrown to prevent silently incorrect results.  
- Frequency‑to‑bin conversion helpers (e.g., `FrequencyIndexForHz`, `HzForBin`) are assumed to exist on `SpectrogramData` and are used in the examples for clarity; they are not part of this extension type.
