# SpectrumDataExtensions

Extension methods for `SpectrumData` that validate its calculated peak, locate the peak in the frequency bins, and scale its magnitude data. These methods are located in the `NAudioVisualizer.Domain.Models` namespace.

## API

### HasValidPeak

```csharp
public static bool HasValidPeak(this SpectrumData spectrumData)
```

Returns `true` when both `PeakFrequency` and `PeakMagnitude` are greater than zero; otherwise, returns `false`.

**Parameters:**
*   `spectrumData`: The spectrum data whose peak values are checked.

**Returns:** `true` if the peak frequency and peak magnitude are both positive; otherwise, `false`.

**Exceptions:**
*   `ArgumentNullException` if `spectrumData` is null.

### GetPeakFrequencyIndex

```csharp
public static int GetPeakFrequencyIndex(this SpectrumData spectrumData)
```

Searches the array returned by `GetFrequencies()` from beginning to end and returns the first index whose frequency exactly equals `PeakFrequency`.

**Parameters:**
*   `spectrumData`: The spectrum data whose peak frequency is located.

**Returns:** The zero-based index of the first exact match for `PeakFrequency`.

**Exceptions:**
*   `ArgumentNullException` if `spectrumData` is null, or if `GetFrequencies()` returns null.
*   `InvalidOperationException` if `PeakFrequency` is zero or negative.
*   `InvalidOperationException` if the peak frequency is not found in the frequency array.

### NormalizeToOne

```csharp
public static void NormalizeToOne(this SpectrumData spectrumData)
```

Gets the magnitude array from `GetData()` and modifies it in place. If the array is non-empty and its maximum value is positive, every magnitude is divided by that maximum, making the maximum magnitude `1`. An empty array or an array whose maximum is not positive is left unchanged.

This method does not recalculate `PeakMagnitude`, `MinValue`, `MaxValue`, or `IsNormalized` after changing the magnitude array.

**Parameters:**
*   `spectrumData`: The spectrum data to normalize.

**Exceptions:**
*   `ArgumentNullException` if `spectrumData` is null.

## Usage

```csharp
using NAudioVisualizer.Domain.Models;

float[] magnitudes = [0.25f, 0.5f, 0.1f];
float[] frequencies = [0f, 1000f, 2000f];
var spectrum = new SpectrumData(magnitudes, frequencies, 48000, 2048);

if (spectrum.HasValidPeak())
{
    int peakIndex = spectrum.GetPeakFrequencyIndex(); // 1
    Console.WriteLine($"Peak bin: {peakIndex}");
}

spectrum.NormalizeToOne();
// spectrum.GetData() now contains [0.5f, 1.0f, 0.2f].
```

## Notes

*   `HasValidPeak` checks only whether the two calculated peak values are positive; it does not call `SpectrumData.IsValid()` or inspect the arrays directly.
*   `GetPeakFrequencyIndex` uses exact `float` equality and returns the first match when the frequency array contains duplicates.
*   `NormalizeToOne` scales by the largest signed magnitude, not by the largest absolute magnitude. Negative values remain negative when a positive maximum exists.
