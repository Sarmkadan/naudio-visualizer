# SpectrogramAnalyzer

A utility class for generating, analyzing, and processing spectrograms from audio data. It processes audio frames into frequency-domain representations, supports logarithmic scaling, normalization, and transient detection. Designed for real-time or batch analysis of audio streams.

## API

### `public SpectrogramAnalyzer`

Initializes a new instance of the `SpectrogramAnalyzer` with default buffer settings.

- **Parameters**: None
- **Remarks**: Uses default buffer size and FFT configuration. Call `SetBufferSize` to customize.

---

### `public SpectrogramData BuildSpectrogram(float[] audioSamples, int sampleRate)`

Generates a spectrogram from raw audio samples.

- **Parameters**:
  - `audioSamples`: Array of audio samples (PCM).
  - `sampleRate`: Sample rate of the audio in Hz.
- **Returns**: `SpectrogramData` containing frequency bins and time slices.
- **Throws**:
  - `ArgumentNullException`: If `audioSamples` is null.
  - `ArgumentException`: If `sampleRate` is not positive.
- **Remarks**: Processes samples in frames based on current buffer settings.

---

### `public void AddSpectrumFrame(float[] spectrum)`

Adds a precomputed frequency spectrum frame to the internal buffer.

- **Parameters**:
  - `spectrum`: Array of magnitude values representing frequency bins.
- **Throws**:
  - `ArgumentNullException`: If `spectrum` is null.
  - `ArgumentException`: If `spectrum` length does not match expected bin count.
- **Remarks**: Used for incremental spectrogram construction. Call `GetCurrentSpectrogram` to retrieve results.

---

### `public SpectrogramData? GetCurrentSpectrogram()`

Retrieves the most recent spectrogram data from the internal buffer.

- **Returns**: `SpectrogramData` if data is available; otherwise `null`.
- **Remarks**: Returns null if no frames have been added or buffer is empty.

---

### `public void ClearBuffer()`

Clears all buffered spectrum frames and resets internal state.

- **Parameters**: None
- **Remarks**: Subsequent calls to `GetCurrentSpectrogram` will return `null` until new frames are added.

---
### `public void SetBufferSize(int frameSize, int overlap)`

Configures the internal frame size and overlap for spectrogram computation.

- **Parameters**:
  - `frameSize`: Number of samples per FFT frame (must be power of two).
  - `overlap`: Number of overlapping samples between frames (must be less than `frameSize`).
- **Throws**:
  - `ArgumentOutOfRangeException`: If `frameSize` is not a power of two or less than 2.
  - `ArgumentException`: If `overlap` is negative or exceeds `frameSize`.
- **Remarks**: Affects all subsequent spectrogram operations.

---
### `public int GetBufferFrameCount()`

Returns the number of buffered spectrum frames.

- **Returns**: Number of frames currently stored.
- **Remarks**: Useful for monitoring buffer state during real-time processing.

---
### `public void ApplyLogScaling()`

Applies logarithmic scaling to the magnitude values in the current spectrogram.

- **Parameters**: None
- **Remarks**: Transforms magnitudes using `log10(1 + magnitude)` for improved perceptual display.

---
### `public void NormalizeSpectrogram()`

Normalizes the current spectrogram data to the range [0, 1].

- **Parameters**: None
- **Remarks**: Scales all magnitude values relative to the maximum in the current data.

---
### `public float[] GetFrequencySlice(int frameIndex)`

Extracts a single frequency slice (magnitudes across all bins) at the specified frame.

- **Parameters**:
  - `frameIndex`: Index of the time frame to retrieve.
- **Returns**: Array of magnitude values for the requested frame.
- **Throws**:
  - `ArgumentOutOfRangeException`: If `frameIndex` is negative or exceeds available frames.
- **Remarks**: Returns a copy of the data.

---
### `public float[] GetTimeSlice(int binIndex)`

Extracts a single time slice (magnitudes across all frames) at the specified frequency bin.

- **Parameters**:
  - `binIndex`: Index of the frequency bin to retrieve.
- **Returns**: Array of magnitude values for the requested bin.
- **Throws**:
  - `ArgumentOutOfRangeException`: If `binIndex` is negative or exceeds available bins.
- **Remarks**: Useful for analyzing specific frequency behavior over time.

---
### `public float[] CalculateSpectralFlux()`

Computes the spectral flux across all frames in the current spectrogram.

- **Returns**: Array of spectral flux values (frame-to-frame magnitude changes).
- **Throws**: None
- **Remarks**: Useful for detecting onsets or transients in audio.

---
### `public List<int> DetectTransients(float threshold)`

Identifies transient events in the current spectrogram based on spectral flux.

- **Parameters**:
  - `threshold`: Minimum spectral flux value to consider a transient.
- **Returns**: List of frame indices where transients were detected.
- **Throws**: None
- **Remarks**: Returns empty list if no transients exceed the threshold.

## Usage

### Real-time Audio Processing
