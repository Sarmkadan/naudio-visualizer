# WaveformServiceTests

Unit test class for verifying the behavior of `WaveformService` methods. Contains tests for sample downsampling, peak calculation, smoothing filters, energy computation, and zero-crossing detection.

## API

### `public WaveformServiceTests()`
Constructor for the test class. Initializes test dependencies and configurations required for waveform analysis tests.

### `public void DownsampleSamples_ReturnsCorrectLengthAndAverages()`
Verifies that the `WaveformService.DownsampleSamples` method produces output with the expected length and correct average values. Ensures that the downsampling operation maintains signal integrity while reducing resolution.

- **Parameters**: None
- **Return value**: None
- **Throws**: No exceptions expected under normal test conditions

### `public void CalculatePeakValues_ReturnsCorrectPeaks()`
Tests that `WaveformService.CalculatePeakValues` correctly identifies the minimum and maximum peak values from a sample buffer. Validates both positive and negative peak detection.

- **Parameters**: None
- **Return value**: None
- **Throws**: No exceptions expected under normal test conditions

### `public void CalculatePeakValues_InvalidPeakCount_ThrowsArgumentException()`
Ensures that `WaveformService.CalculatePeakValues` throws an `ArgumentException` when an invalid peak count (e.g., zero or negative) is provided.

- **Parameters**: None
- **Return value**: None
- **Throws**: `ArgumentException` if peak count is invalid

### `public void ApplySmoothingFilter_SmoothesSignal()`
Confirms that `WaveformService.ApplySmoothingFilter` reduces high-frequency noise in a signal while preserving overall shape. Validates that the smoothing operation produces a lower variance output compared to the input.

- **Parameters**: None
- **Return value**: None
- **Throws**: No exceptions expected under normal test conditions

### `public void CalculateFrameEnergy_ReturnsCorrectRms()`
Checks that `WaveformService.CalculateFrameEnergy` computes the root mean square (RMS) energy of a sample frame accurately. Compares computed energy against a known reference value.

- **Parameters**: None
- **Return value**: None
- **Throws**: No exceptions expected under normal test conditions

### `public void CountZeroCrossings_ReturnsCorrectCount()`
Validates that `WaveformService.CountZeroCrossings` accurately counts the number of times a signal crosses zero within a given frame. Ensures correct handling of positive-to-negative and negative-to-positive transitions.

- **Parameters**: None
- **Return value**: None
- **Throws**: No exceptions expected under normal test conditions

## Usage
