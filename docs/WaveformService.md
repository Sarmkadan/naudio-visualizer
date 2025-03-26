# WaveformService

`WaveformService` provides a set of audio analysis utilities for generating waveform representations from raw sample data. It is designed to process PCM audio buffers into visualizable forms by offering downsampling, normalization, peak calculation, smoothing, frame energy computation, and zero-crossing detection. Each method operates on `float[]` arrays representing single-channel sample data.

## API

### `WaveformData GenerateWaveform`

Generates a complete waveform data structure from the provided audio samples. This method orchestrates multiple processing steps internally to produce a ready-to-render waveform representation.

- **Parameters:** Accepts raw audio sample data and configuration parameters (specific signature details depend on internal implementation).
- **Returns:** A `WaveformData` object containing processed waveform information suitable for visualization.
- **Throws:** `ArgumentNullException` when the sample array is null. `ArgumentException` when the sample array is empty or configuration parameters are invalid.

### `float[] DownsampleSamples`

Reduces the resolution of an audio sample array to a target number of points, preserving the overall amplitude envelope. This is typically used to match the sample count to a display width.

- **Parameters:**
  - `float[] samples`: The source audio samples.
  - `int targetLength`: The desired number of output samples.
- **Returns:** A new `float[]` of length `targetLength` containing the downsampled data.
- **Throws:** `ArgumentNullException` when `samples` is null. `ArgumentOutOfRangeException` when `targetLength` is less than 1 or greater than `samples.Length`.

### `void NormalizeWaveform`

Scales waveform data in-place so that its absolute peak value reaches a specified target amplitude, typically 1.0 for full-scale normalization.

- **Parameters:**
  - `float[] waveform`: The waveform data to normalize. Modified in place.
  - `float targetPeak`: The desired maximum absolute value after normalization (default typically 1.0f).
- **Returns:** Void. The input array is mutated directly.
- **Throws:** `ArgumentNullException` when `waveform` is null. `InvalidOperationException` when all values in `waveform` are zero, as normalization would require division by zero.

### `float[] CalculatePeakValues`

Computes the maximum absolute amplitude within each segment of a partitioned sample array. Useful for generating coarse amplitude envelopes.

- **Parameters:**
  - `float[] samples`: The source audio samples.
  - `int segments`: The number of equal-sized segments to divide the samples into.
- **Returns:** A `float[]` of length `segments`, where each element is the peak absolute value found in the corresponding segment.
- **Throws:** `ArgumentNullException` when `samples` is null. `ArgumentOutOfRangeException` when `segments` is less than 1 or exceeds `samples.Length`.

### `float[] ApplySmoothingFilter`

Applies a moving-average or similar low-pass smoothing filter to an array of waveform values, reducing jaggedness for cleaner visualization.

- **Parameters:**
  - `float[] data`: The waveform data to smooth.
  - `int windowSize`: The number of neighboring samples to include in the averaging window.
- **Returns:** A new `float[]` containing the smoothed data, with the same length as the input.
- **Throws:** `ArgumentNullException` when `data` is null. `ArgumentOutOfRangeException` when `windowSize` is less than 1 or greater than `data.Length`. Edge samples are handled by reducing the effective window size near boundaries.

### `float[] CalculateFrameEnergy`

Computes the root-mean-square (RMS) energy for consecutive frames of audio samples, providing a measure of perceived loudness over time.

- **Parameters:**
  - `float[] samples`: The source audio samples.
  - `int frameSize`: The number of samples per energy calculation frame.
- **Returns:** A `float[]` where each element represents the RMS energy of one frame. Length is `samples.Length / frameSize` (truncated division).
- **Throws:** `ArgumentNullException` when `samples` is null. `ArgumentOutOfRangeException` when `frameSize` is less than 1 or exceeds `samples.Length`.

### `int CountZeroCrossings`

Counts the number of times the audio signal crosses the zero amplitude axis within a given sample array. This is often used as a rough proxy for frequency content or pitch detection.

- **Parameters:**
  - `float[] samples`: The source audio samples.
- **Returns:** An `int` representing the total number of zero-crossing events detected.
- **Throws:** `ArgumentNullException` when `samples` is null. A zero-crossing is counted when two consecutive samples have opposite signs; exact zero values are treated as positive for crossing determination.

## Usage

### Example 1: Generating a Downsampled and Normalized Waveform for Display

```csharp
var service = new WaveformService();
float[] rawSamples = GetAudioSamples(); // Assume this returns PCM data

// Downsample to match a 1024-pixel-wide display
float[] downsampled = service.DownsampleSamples(rawSamples, 1024);

// Normalize to full scale
service.NormalizeWaveform(downsampled, 1.0f);

// Smooth for a cleaner visual
float[] smoothed = service.ApplySmoothingFilter(downsampled, 3);

RenderWaveform(smoothed);
```

### Example 2: Computing Energy Profile and Zero-Crossing Rate

```csharp
var service = new WaveformService();
float[] samples = GetAudioSamples();

// Calculate frame-by-frame energy with 256-sample frames
float[] energyProfile = service.CalculateFrameEnergy(samples, 256);

// Count zero-crossings as a basic frequency indicator
int crossings = service.CountZeroCrossings(samples);

Console.WriteLine($"Total zero-crossings: {crossings}");
Console.WriteLine($"Energy frames computed: {energyProfile.Length}");

// Use energy profile to color-code waveform segments
for (int i = 0; i < energyProfile.Length; i++)
{
    float intensity = energyProfile[i];
    // Map intensity to color or opacity
}
```

## Notes

- **Edge Cases:** `NormalizeWaveform` throws `InvalidOperationException` when all samples are zero, as no meaningful scaling factor can be derived. `DownsampleSamples` returns an array of the requested length even when `targetLength` equals the input length, effectively performing a copy. `CalculateFrameEnergy` discards trailing samples that do not fill a complete frame; the return length is the integer quotient of `samples.Length / frameSize`. `CountZeroCrossings` treats exactly-zero samples as non-negative, so a transition from a negative value to exactly zero counts as a crossing, but zero-to-negative does not.
- **Thread Safety:** All public methods are static or operate on their input arrays without shared mutable state. They are safe to call concurrently from multiple threads provided each call uses distinct array instances. Methods that mutate arrays in place (`NormalizeWaveform`) are not safe when the same array reference is passed from multiple threads simultaneously.
- **Memory Allocation:** Methods returning `float[]` (`DownsampleSamples`, `CalculatePeakValues`, `ApplySmoothingFilter`, `CalculateFrameEnergy`) allocate new arrays on each invocation. Callers should reuse or pool arrays where allocation pressure is a concern. `NormalizeWaveform` performs no allocation.
