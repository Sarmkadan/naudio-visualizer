# SpectrumAnalyzer
The `SpectrumAnalyzer` class is designed to analyze audio signals and extract relevant spectral features. It provides methods for analyzing the spectrum of an audio signal, smoothing and normalizing the spectrum, and extracting frequency bands. The class also tracks peak holds and provides energy levels for different frequency ranges.

## API
* `public float PeakHoldDecayDbPerSecond`: Gets the decay rate of peak holds in decibels per second.
* `public SpectrumData AnalyzeSpectrum`: Analyzes the spectrum of an audio signal and returns the result.
* `public void ConvertToLogScale`: Converts the spectrum to a logarithmic scale.
* `public void SmoothSpectrum`: Smooths the spectrum to reduce noise.
* `public float FindDominantFrequency`: Finds the dominant frequency in the spectrum.
* `public float CalculateSpectralCentroid`: Calculates the spectral centroid of the spectrum.
* `public FrequencyBands ExtractFrequencyBands`: Extracts frequency bands from the spectrum.
* `public void NormalizeSpectrum`: Normalizes the spectrum to a standard range.
* `public void UpdatePeakHolds`: Updates the peak holds based on the current spectrum.
* `public float[]? GetPeakHolds`: Gets the current peak holds.
* `public void ResetPeakHolds`: Resets the peak holds to their initial state.
* `public float BassEnergy`: Gets the energy level of the bass frequency range.
* `public float MidEnergy`: Gets the energy level of the mid frequency range.
* `public float TrebleEnergy`: Gets the energy level of the treble frequency range.

## Usage
```csharp
// Example 1: Analyzing an audio signal
SpectrumAnalyzer analyzer = new SpectrumAnalyzer();
SpectrumData spectrum = analyzer.AnalyzeSpectrum(audioSignal);
analyzer.SmoothSpectrum();
FrequencyBands frequencyBands = analyzer.ExtractFrequencyBands();

// Example 2: Tracking peak holds and energy levels
SpectrumAnalyzer analyzer = new SpectrumAnalyzer();
analyzer.UpdatePeakHolds();
float[] peakHolds = analyzer.GetPeakHolds();
float bassEnergy = analyzer.BassEnergy;
float midEnergy = analyzer.MidEnergy;
float trebleEnergy = analyzer.TrebleEnergy;
```

## Notes
The `SpectrumAnalyzer` class is not thread-safe, and its methods should not be called concurrently from multiple threads. The `AnalyzeSpectrum` method may throw an exception if the input audio signal is invalid or corrupted. The `ExtractFrequencyBands` method may return an empty frequency band if the spectrum does not contain any significant frequency components. The `GetPeakHolds` method may return null if the peak holds have not been updated recently. The energy levels returned by the `BassEnergy`, `MidEnergy`, and `TrebleEnergy` properties are relative to the overall energy of the audio signal and may not be absolute values.
