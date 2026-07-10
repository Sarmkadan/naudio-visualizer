# SpectrumAnalyzerExtensions

Provides extension methods for analyzing audio spectrum data, particularly for extracting features relevant to music visualization such as bass-to-treble balance and spectral flux.

## API

### `CalculateBassToTrebleRatio`

Calculates the ratio of low-frequency energy (bass) to high-frequency energy (treble) in the provided spectrum data. Useful for determining the tonal balance of the audio signal.

- **Parameters**
  - `spectrum` (float[]): The frequency spectrum data, typically obtained from an FFT analysis of an audio signal. Must not be null.
  - `bassCutoff` (int): The index in the spectrum array below which frequencies are considered bass. Must be between 0 and `spectrum.Length - 1`.
  - `trebleCutoff` (int): The index in the spectrum array above which frequencies are considered treble. Must be between `bassCutoff + 1` and `spectrum.Length - 1`.

- **Return Value**
  - Returns a float representing the ratio of the sum of bass frequencies to the sum of treble frequencies. Returns `0f` if the treble sum is zero to avoid division by zero.

- **Exceptions**
  - Throws `ArgumentNullException` if `spectrum` is null.
  - Throws `ArgumentOutOfRangeException` if `bassCutoff` or `trebleCutoff` are outside the valid range or if `trebleCutoff` is not greater than `bassCutoff`.

---

### `HasSignificantBass`

Determines whether the provided spectrum data contains significant bass energy, based on a threshold relative to the total energy in the spectrum.

- **Parameters**
  - `spectrum` (float[]): The frequency spectrum data. Must not be null.
  - `bassCutoff` (int): The index in the spectrum array below which frequencies are considered bass. Must be between 0 and `spectrum.Length - 1`.
  - `bassThreshold` (float): The minimum ratio of bass energy to total energy required to return true. Must be between 0 and 1.

- **Return Value**
  - Returns `true` if the sum of bass frequencies exceeds `bassThreshold` times the total energy in the spectrum; otherwise, returns `false`.

- **Exceptions**
  - Throws `ArgumentNullException` if `spectrum` is null.
  - Throws `ArgumentOutOfRangeException` if `bassCutoff` is outside the valid range or if `bassThreshold` is not between 0 and 1.

---
### `CalculateSpectralFlux`

Computes the spectral flux between the current and previous spectrum frames. Spectral flux is a measure of how much the power spectrum changes between frames, often used to detect transients or beats.

- **Parameters**
  - `currentSpectrum` (float[]): The current frame's frequency spectrum data. Must not be null.
  - `previousSpectrum` (float[]): The previous frame's frequency spectrum data. Must not be null and must have the same length as `currentSpectrum`.

- **Return Value**
  - Returns a float representing the spectral flux, calculated as the Euclidean distance between the two spectra after normalization.

- **Exceptions**
  - Throws `ArgumentNullException` if either `currentSpectrum` or `previousSpectrum` is null.
  - Throws `ArgumentException` if `currentSpectrum` and `previousSpectrum` have different lengths.

---
### `NormalizeAndSmooth`

Normalizes the spectrum data to a maximum value of 1.0 and applies a simple smoothing filter to reduce noise and emphasize sustained energy.

- **Parameters**
  - `spectrum` (float[]): The frequency spectrum data to normalize and smooth. Must not be null.
  - `smoothingFactor` (float): A value between 0 and 1 indicating the strength of the smoothing. Higher values result in more smoothing. Must be between 0 and 1.

- **Return Value**
  - Returns the normalized and smoothed spectrum as a new array. The original array is not modified.

- **Exceptions**
  - Throws `ArgumentNullException` if `spectrum` is null.
  - Throws `ArgumentOutOfRangeException` if `smoothingFactor` is not between 0 and 1.

## Usage
