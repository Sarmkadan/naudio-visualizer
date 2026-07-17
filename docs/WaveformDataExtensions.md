# WaveformDataExtensions

Provides a set of extension methods for analyzing, converting, and manipulating `WaveformData` instances. These helpers operate on the data contained in a `WaveformData` object without modifying the original instance unless explicitly noted (e.g., `NormalizedCopy` returns a new normalized copy).

## API

### ToStereoWaveform
```csharp
public static WaveformData ToStereoWaveform(this WaveformData waveform)
```
**Purpose:** Returns a stereo representation of the waveform. If the input is already stereo, the same instance is returned; if it is mono, a new stereo waveform is created by duplicating the mono channel into both left and right channels.  
**Parameters:** `waveform` – the `WaveformData` to convert.  
**Return Value:** A `WaveformData` containing stereo audio data.  
**Exceptions:** Throws `ArgumentNullException` if `waveform` is `null`. Throws `InvalidOperationException` if the underlying sample buffers are inaccessible or malformed.

### ToStereo
```csharp
public static WaveformData ToStereo(this WaveformData waveform)
```
**Purpose:** Alias for `ToStereoWaveform`; converts mono waveform data to stereo by channel duplication.  
**Parameters:** `waveform` – the `WaveformData` to convert.  
**Return Value:** A stereo `WaveformData`.  
**Exceptions:** Same as `ToStereoWaveform`.

### GetChannelPeaks
```csharp
public static float[]? GetChannelPeaks(this WaveformData waveform)
```
**Purpose:** Computes the peak (maximum absolute) amplitude for each channel present in the waveform.  
**Parameters:** `waveform` – the source waveform.  
**Return Value:** An array where each element corresponds to a channel’s peak amplitude; returns `null` if the waveform contains no sample data or the channel count cannot be determined.  
**Exceptions:** Throws `ArgumentNullException` if `waveform` is `null`.

### GetDurationSeconds
```csharp
public static double GetDurationSeconds(this WaveformData waveform)
```
**Purpose:** Calculates the playback duration of the waveform in seconds based on its sample count and sample rate.  
**Parameters:** `waveform` – the waveform to measure.  
**Return Value:** Duration as a `double`. Returns `0.0` if the waveform has no samples or an invalid sample rate.  
**Exceptions:** Throws `ArgumentNullException` if `waveform` is `null`.

### GetTotalSampleCount
```csharp
public static int GetTotalSampleCount(this WaveformData waveform)
```
**Purpose:** Retrieves the total number of samples stored per channel (all channels are assumed to have equal length).  
**Parameters:** `waveform` – the waveform to query.  
**Return Value:** An `int` representing the sample count per channel. Returns `0` for empty data.  
**Exceptions:** Throws `ArgumentNullException` if `waveform` is `null`.

### GetPointsPerChannel
```csharp
public static int GetPointsPerChannel(this WaveformData waveform)
```
**Purpose:** Gets the number of sample points (values) contained in each channel of the waveform data. Functionally equivalent to `GetTotalSampleCount` for typical usage.  
**Parameters:** `waveform` – the waveform to query.  
**Return Value:** An `int` indicating points per channel; `0` when no data is present.  
**Exceptions:** Throws `ArgumentNullException` if `waveform` is `null`.

### Downsample
```csharp
public static WaveformData Downsample(this WaveformData waveform)
```
**Purpose:** Produces a downsampled version of the waveform using an internal default factor (typically reducing the sample rate by half). The returned instance contains fewer samples while preserving the overall shape of the signal.  
**Parameters:** `waveform` – the waveform to downsample.  
**Return Value:** A new `WaveformData` with reduced sample density.  
**Exceptions:** Throws `ArgumentNullException` if `waveform` is `null`. Throws `InvalidOperationException` if downsampling would result in zero samples.

### GetPeakAmplitude
```csharp
public static float GetPeakAmplitude(this WaveformData waveform)
```
**Purpose:** Returns the highest absolute sample value found across all channels.  
**Parameters:** `waveform` – the waveform to inspect.  
**Return Value:** A `float` in the range `[0, max]` where `max` is the largest magnitude sample. Returns `0.0` for empty data.  
**Exceptions:** Throws `ArgumentNullException` if `waveform` is `null`.

### GetRmsAmplitude
```csharp
public static float GetRmsAmplitude(this WaveformData waveform)
```
**Purpose:** Computes the root‑mean‑square (RMS) amplitude of the waveform, providing a measure of its average power.  
**Parameters:** `waveform` – the waveform to analyze.  
**Return Value:** A `float` representing the RMS value; `0.0` if no samples are present.  
**Exceptions:** Throws `ArgumentNullException` if `waveform` is `null`.

### GetAverageAmplitude
```csharp
public static float GetAverageAmplitude(this WaveformData waveform)
```
**Purpose:** Calculates the arithmetic mean of the absolute sample values across all channels.  
**Parameters:** `waveform` – the waveform to process.  
**Return Value:** A `float` average amplitude; `0.0` for empty data.  
**Exceptions:** Throws `ArgumentNullException` if `waveform` is `null`.

### NormalizedCopy
```csharp
public static WaveformData NormalizedCopy(this WaveformData waveform)
```
**Purpose:** Returns a new `WaveformData` instance whose sample values have been scaled so that the largest absolute sample equals `1.0` (or `-1.0` for negative peaks). The original waveform is left unchanged.  
**Parameters:** `waveform` – the waveform to normalize.  
**Return Value:** A normalized copy of the waveform; returns an empty waveform if the source contains no data.  
**Exceptions:** Throws `ArgumentNullException` if `waveform` is `null`. Throws `InvalidOperationException` if the peak amplitude is zero (cannot normalize a silent signal).

### SplitStereoChannels
```csharp
public static (float[] Left, float[] Right)? SplitStereoChannels(this WaveformData waveform)
```
**Purpose:** Separates a stereo waveform into its left and right channel sample arrays.  
**Parameters:** `waveform` – the stereo waveform to split.  
**Return Value:** A tuple `(Left, Right)` containing two `float[]` arrays; returns `null` if the waveform is not stereo or lacks channel data.  
**Exceptions:** Throws `ArgumentNullException` if `waveform` is `null`.

### GetSample
```csharp
public static float GetSample(this WaveformData waveform)
```
**Purpose:** Retrieves a single sample value from the waveform. The implementation returns the first sample of the first channel (index 0, channel 0).  
**Parameters:** `waveform` – the source waveform.  
**Return Value:** A `float` sample; returns `0.0` when the waveform has no data.  
**Exceptions:** Throws `ArgumentNullException` if `waveform` is `null`.

### GetSampleRange
```csharp
public static float[] GetSampleRange(this WaveformData waveform)
```
**Purpose:** Provides the minimum and maximum sample values observed across all channels.  
**Parameters:** `waveform` – the waveform to scan.  
**Return Value:** A two‑element `float[]` where `[0]` is the minimum sample and `[1]` is the maximum sample. Returns `[0,0]` for empty data.  
**Exceptions:** Throws `ArgumentNullException` if `waveform` is `null`.

## Usage

### Example 1: Converting mono to stereo and measuring loudness
```csharp
using NAudio.Visualizer;

// Assume 'monoWave' is a WaveformData loaded from a mono audio file.
WaveformData monoWave = WaveformData.FromFile("mono.wav");

// Convert to stereo for further processing.
WaveformData stereoWave = monoWave.ToStereo();

// Get peak and RMS amplitudes to assess loudness.
float peak = stereoWave.GetPeakAmplitude();
float rms  = stereoWave.GetRmsAmplitude();

Console.WriteLine($"Peak: {peak:F3}, RMS: {rms:F3}");
```

### Example 2: Downsampling a waveform and extracting channel data
```csharp
using NAudio.Visualizer;

// Load a stereo audio file.
WaveformData wave = WaveformData.FromFile("stereo.wav");

// Downsample to reduce data size for visualization.
WaveformData small = wave.Downsample();

// Split into left and right channels for separate analysis.
var channels = small.SplitStereoChannels();
if (channels.HasValue)
{
    float[] left  = channels.Value.Left;
    float[] right = channels.Value.Right;

    // Example: compute average amplitude per channel.
    float avgLeft  = small.GetAverageAmplitude(); // uses whole waveform; for per‑channel you could slice.
    float avgRight = small.GetAverageAmplitude();

    Console.WriteLine($"Left avg: {avgLeft:F3}, Right avg: {avgRight:F3}");
}
```

## Notes

- **Null handling:** All extension methods throw `ArgumentNullException` when the `WaveformData` instance is `null`. Callers should validate or guard against null inputs where appropriate.  
- **Empty data:** If the waveform contains no sample buffers, most methods return neutral default values (`0`, `0.0`, or empty arrays) rather than throwing, except where a mathematical operation is undefined (e.g., `NormalizedCopy` throws when the peak amplitude is zero).  
- **Stereo assumptions:** Methods such as `ToStereoWaveform`, `ToStereo`, and `SplitStereoChannels` assume that a stereo waveform consists of exactly two channels of equal length. Providing data with a differing channel count will result in `null` or an exception as indicated.  
- **Thread safety:** The extension methods are stateless and do not modify the input `WaveformData` instance (except where explicitly noted, such as `NormalizedCopy` which creates a new copy). Consequently, they are safe to invoke concurrently from multiple threads on distinct `WaveformData` instances. Sharing the same instance across threads is safe as long as the instance itself is not mutated elsewhere.  
- **Performance:** Methods that iterate over samples (e.g., `GetPeakAmplitude`, `GetRmsAmplitude`, `GetAverageAmplitude`, `GetSampleRange`) operate in O(N) time where N is the total number of samples across all channels. Repeated calls on large waveforms may benefit from caching results if the underlying data does not change.  
- **Downsampling factor:** The `Downsample` method uses an internal default reduction factor (commonly 2×). For custom downsampling ratios, additional parameters would be required, but they are not part of the current public API.  
- **Sample retrieval:** `GetSample` returns the first sample of the first channel; for indexed access, consumers should inspect the waveform’s internal sample buffers directly or request a future API enhancement.  

This documentation covers only the members listed; any other members of `WaveformDataExtensions` are outside the scope of this page.
