# MathUtility

Utility class providing static helper methods for audio signal processing, mathematical conversions, and common DSP operations used throughout the naudio‑visualizer project.

## API

### FrequencyToMidiNote
```csharp
public static int FrequencyToMidiNote(float frequency)
```
Converts a frequency in hertz to the nearest MIDI note number.  
- **Parameters**  
  - `frequency`: Frequency value in Hz. Must be greater than 0.  
- **Return value**  
  - MIDI note number as an integer (0‑127). Values outside the audible range are clamped to the nearest valid MIDI note.  
- **Exceptions**  
  - `ArgumentOutOfRangeException` if `frequency` is less than or equal to 0.

### MidiNoteToFrequency
```csharp
public static float MidiNoteToFrequency(int midiNote)
```
Converts a MIDI note number to its corresponding frequency in hertz.  
- **Parameters**  
  - `midiNote`: MIDI note number. Values outside 0‑127 are allowed; the function extrapolates using the standard 12‑tone equal temperament formula.  
- **Return value**  
  - Frequency in Hz as a `float`.  
- **Exceptions**  
  - None.

### AmplitudeToDb
```csharp
public static float AmplitudeToDb(float amplitude)
```
Converts a linear amplitude ratio to decibels (dB).  
- **Parameters**  
  - `amplitude`: Linear amplitude, typically in the range `[0, ∞)`. Values of 0 or negative produce `-∞` dB; the method returns `float.NegativeInfinity` for 0.  
- **Return value**  
  - Amplitude expressed in dB.  
- **Exceptions**  
  - None.

### DbToAmplitude
```csharp
public static float DbToAmplitude(float decibels)
```
Converts a decibel value to a linear amplitude ratio.  
- **Parameters**  
  - `decibels`: Amplitude in dB.  
- **Return value**  
  - Linear amplitude as a `float`.  
- **Exceptions**  
  - None.

### CalculateRms
```csharp
public static float CalculateRms(float[] samples)
```
Computes the root‑mean‑square (RMS) level of a sample buffer.  
- **Parameters**  
  - `samples`: Array of mono audio samples. Must not be `null` and must contain at least one element.  
- **Return value**  
  - RMS value as a `float`.  
- **Exceptions**  
  - `ArgumentNullException` if `samples` is `null`.  
  - `ArgumentException` if `samples` length is 0.

### CalculatePeak
```csharp
public static float CalculatePeak(float[] samples)
```
Returns the peak absolute sample value in a buffer.  
- **Parameters**  
  - `samples`: Array of mono audio samples. Must not be `null` and must contain at least one element.  
- **Return value**  
  - Peak amplitude as a non‑negative `float`.  
- **Exceptions**  
  - `ArgumentNullException` if `samples` is `null`.  
  - `ArgumentException` if `samples` length is 0.

### LogScale
```csharp
public static float LogScale(float value, float baseValue = 10f)
```
Applies a logarithmic scaling to an input value.  
- **Parameters**  
  - `value`: Input value to scale. Must be greater than 0.  
  - `baseValue`: Logarithm base (default 10). Must be greater than 1.  
- **Return value**  
  - `log_base(value)` as a `float`.  
- **Exceptions**  
  - `ArgumentOutOfRangeException` if `value` ≤ 0 or `baseValue` ≤ 1.

### PowerScale
```csharp
public static float PowerScale(float value, float exponent)
```
Raises an input value to a specified exponent (power scaling).  
- **Parameters**  
  - `value`: Input value. Must be non‑negative when `exponent` is non‑integer to avoid NaN.  
  - `exponent`: Exponent to apply.  
- **Return value**  
  - `value` raised to `exponent` as a `float`.  
- **Exceptions**  
  - `ArgumentOutOfRangeException` if `value` < 0 and `exponent` is not an integer.

### ApplyHannWindow
```csharp
public static void ApplyHannWindow(float[] buffer)
```
Applies a Hann (Hanning) window to the supplied buffer in place.  
- **Parameters**  
  - `buffer`: Array of samples to window. Must not be `null` and must contain at least one element.  
- **Return value**  
  - None (the buffer is modified).  
- **Exceptions**  
  - `ArgumentNullException` if `buffer` is `null`.  
  - `ArgumentException` if `buffer` length is 0.

### ApplyHammingWindow
```csharp
public static void ApplyHammingWindow(float[] buffer)
```
Applies a Hamming window to the supplied buffer in place.  
- **Parameters**  
  - `buffer`: Array of samples to window. Must not be `null` and must contain at least one element.  
- **Return value**  
  - None (the buffer is modified).  
- **Exceptions**  
  - `ArgumentNullException` if `buffer` is `null`.  
  - `ArgumentException` if `buffer` length is 0.

### NextPowerOf2
```csharp
public static int NextPowerOf2(int value)
```
Returns the smallest power of two that is greater than or equal to `value`.  
- **Parameters**  
  - `value`: Positive integer.  
- **Return value**  
  - Next power of two as an `int`.  
- **Exceptions**  
  - `ArgumentOutOfRangeException` if `value` ≤ 0.

### IsPowerOf2
```csharp
public static bool IsPowerOf2(int value)
```
Determines whether an integer is an exact power of two.  
- **Parameters**  
  - `value`: Integer to test.  
- **Return value**  
  - `true` if `value` is a power of two; otherwise `false`.  
- **Exceptions**  
  - None.

### Lerp
```csharp
public static float Lerp(float start, float end, float t)
```
Performs linear interpolation between `start` and `end`.  
- **Parameters**  
  - `start`: Starting value.  
  - `end`: Ending value.  
  - `t`: Interpolation factor, typically in the range `[0, 1]`. Values outside this range are allowed and will extrapolate.  
- **Return value**  
  - Interpolated value as a `float`.  
- **Exceptions**  
  - None.

### MapRange
```csharp
public static float MapRange(float value, float from1, float to1, float from2, float to2)
```
Maps a value from one numeric range to another.  
- **Parameters**  
  - `value`: Input value to map.  
  - `from1`: Lower bound of the source range.  
  - `to1`: Upper bound of the source range.  
  - `from2`: Lower bound of the target range.  
  - `to2`: Upper bound of the target range.  
- **Return value**  
  - Mapped value as a `float`.  
- **Exceptions**  
  - `DivideByZeroException` if `from1` equals `to1` (zero‑length source range).

### Distance
```csharp
public static float Distance(float x1, float y1, float x2, float y2)
```
Calculates the Euclidean distance between two points in 2‑D space.  
- **Parameters**  
  - `x1`, `y1`: Coordinates of the first point.  
  - `x2`, `y2`: Coordinates of the second point.  
- **Return value**  
  - Distance as a `float`.  
- **Exceptions**  
  - None.

## Usage

### Example 1: Converting frequency to MIDI note and visualizing amplitude
```csharp
float frequency = 440f; // A4
int midiNote = MathUtility.FrequencyToMidiNote(frequency);
// midiNote == 69

float[] buffer = GetAudioSamples(); // assume filled with PCM data
float rms = MathUtility.CalculateRms(buffer);
float db = MathUtility.AmplitudeToDb(rms);
// db can now be used for a dB meter UI
```

### Example 2: Applying a window and computing peak before FFT
```csharp
float[] frame = new float[1024];
AudioCapture.Read(frame, 0, frame.Length);

MathUtility.ApplyHannWindow(frame); // window in place
float peak = MathUtility.CalculatePeak(frame);
// Use peak for normalization or thresholding before spectral analysis
```

## Notes

- All methods are **thread‑safe** because they operate only on their parameters and do not modify any shared state. The windowing methods modify the supplied buffer, so callers must ensure exclusive access to that buffer if it may be used concurrently elsewhere.  
- Conversion methods (`FrequencyToMidiNote`, `MidiNoteToFrequency`, `AmplitudeToDb`, `DbToAmplitudeToDb`DbToAmplitude` accept any`; they do not clamped or extrapolated as described; they never throw for numeric overflow because the result fits within the range of `float`/`int`.  
- The RMS and peak calculators return `0` for an empty buffer only if the caller bypasses the argument checks; the public contract throws for empty or `null` input to avoid silent bugs.  
- `NextPowerOf2` returns `1` for any input `≤ 1` after validation; the method throws for non‑positive inputs to make misuse explicit.  
- `LogScale` and `PowerScale` are intended for positive inputs; passing invalid values results in an `ArgumentOutOfRangeException` to prevent NaN or infinity from propagating unintentionally.  
- `MapRange` will throw if the source range has zero length; this guards against division by zero. If such a scenario is possible in your code, validate the range beforehand.  
- The `Distance` method assumes Euclidean space; for other metrics (Manhattan, Chebyshev) a different helper would be required.
