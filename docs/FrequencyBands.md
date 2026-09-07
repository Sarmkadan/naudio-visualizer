# FrequencyBands

The `FrequencyBands` class contains the relative magnitude contributed by the bass, mid, and treble regions of a `SpectrumData` instance. `SpectrumAnalyzer.ExtractFrequencyBands` creates and populates this class by summing spectrum-bin magnitudes in each frequency range and, when their combined sum is positive, normalizing the three values by that sum.

## API

### Properties

*   **`public float BassEnergy { get; set; }`**
    Gets or sets the bass contribution. `ExtractFrequencyBands` includes bins whose frequency is less than 250 Hz (`frequency < 250`).

*   **`public float MidEnergy { get; set; }`**
    Gets or sets the midrange contribution. `ExtractFrequencyBands` includes bins from 250 Hz inclusive to 4000 Hz exclusive (`frequency >= 250 && frequency < 4000`).

*   **`public float TrebleEnergy { get; set; }`**
    Gets or sets the treble contribution. `ExtractFrequencyBands` includes bins at or above 4000 Hz (`frequency >= 4000`).

## Creation

`FrequencyBands` has no explicitly declared constructor. `SpectrumAnalyzer.ExtractFrequencyBands(SpectrumData spectrum)` returns a new instance populated from the spectrum's frequency and magnitude arrays.

The method processes matching frequency and magnitude entries up to the length of the shorter array. It adds each magnitude directly to its band; it does not square the magnitude. If the total of all three bands is greater than zero, each property is divided by that total, so the returned values sum to 1. If the total is zero, the properties remain zero. Passing `null` as `spectrum` throws `ArgumentNullException`.

## Usage

```csharp
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Services;

var analyzer = new SpectrumAnalyzer();
SpectrumData spectrum = analyzer.AnalyzeSpectrum(audioFrame, fftSize: 2048);

FrequencyBands bands = analyzer.ExtractFrequencyBands(spectrum);

Console.WriteLine($"Bass (< 250 Hz): {bands.BassEnergy:P1}");
Console.WriteLine($"Mid (250 to < 4000 Hz): {bands.MidEnergy:P1}");
Console.WriteLine($"Treble (>= 4000 Hz): {bands.TrebleEnergy:P1}");
```

## Notes

*   The values are relative contributions, not raw accumulated magnitudes, when the combined magnitude is positive.
*   A bin at exactly 250 Hz belongs to `MidEnergy`, and a bin at exactly 4000 Hz belongs to `TrebleEnergy`.
*   The properties are publicly settable and can be changed after extraction.
