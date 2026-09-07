# AudioFrame Extensions and Validation

The `AudioFrameExtensions` and `AudioFrameValidation` static classes provide analysis, channel extraction, formatting, and detailed validation helpers for `AudioFrame` instances.

## API

### AudioFrameExtensions

*   **`public static float CalculateAverageAmplitude(this AudioFrame frame)`**
    Calculates the arithmetic mean of the absolute values in `frame.Samples`. It returns `0f` when the sample array is empty.
    *   **Parameters:** `frame`, the frame whose samples are analyzed.
    *   **Returns:** The mean absolute sample amplitude.
    *   **Throws:** `ArgumentNullException` if `frame` is null.

*   **`public static IReadOnlyDictionary<int, float[]> GetChannelDataDictionary(this AudioFrame frame)`**
    Extracts every channel by calling `GetChannelData` for channel indexes from `0` through `ChannelCount - 1`. Each dictionary key is a zero-based channel index, and each value is a newly allocated array containing that channel's deinterleaved samples.
    *   **Parameters:** `frame`, the frame whose channel data is extracted.
    *   **Returns:** A read-only dictionary interface backed by a dictionary containing one entry per channel.
    *   **Throws:** `ArgumentNullException` if `frame` is null; `ArgumentOutOfRangeException` if `ChannelCount` is less than 1.

*   **`public static string Format(this AudioFrame frame)`**
    Formats the frame using the invariant culture. The result includes `Id`, `Timestamp`, `DurationSeconds`, `PeakAmplitude`, `RmsEnergy`, `ChannelCount`, and `SampleRate`. The timestamp uses the round-trip (`O`) format, while duration, peak amplitude, and RMS energy use three decimal places (`F3`). Sample values and `FrameIndex` are not included.
    *   **Parameters:** `frame`, the frame to format.
    *   **Returns:** A string in the form `AudioFrame(Id: ..., Timestamp: ..., DurationSeconds: ..., PeakAmplitude: ..., RmsEnergy: ..., ChannelCount: ..., SampleRate: ...)`.
    *   **Throws:** `ArgumentNullException` if `frame` is null.

*   **`public static string Format(this AudioFrame frame, CultureInfo cultureInfo)`**
    Produces the same fields and uses the same format specifiers as `Format()`, but applies the supplied culture to formatted values.
    *   **Parameters:** `frame`, the frame to format; `cultureInfo`, the culture used for formatting.
    *   **Returns:** The formatted frame description.
    *   **Throws:** `ArgumentNullException` if either `frame` or `cultureInfo` is null.

### AudioFrameValidation

*   **`public static IReadOnlyList<string> Validate(this AudioFrame value)`**
    Returns human-readable validation problems. A valid frame produces an empty read-only list. Validation checks:
    *   `Id` is not `Guid.Empty`.
    *   `Samples` is not null or empty and contains neither `NaN` nor infinite values. These sample checks form one conditional chain, so only the first applicable sample problem is added.
    *   `ChannelCount` is greater than zero and, when samples are present, the sample count is divisible by `ChannelCount`.
    *   `SampleRate` is greater than zero.
    *   `Timestamp` is neither the default value nor a non-UTC `DateTime`.
    *   `FrameIndex` is not negative.
    *   `DurationSeconds` is greater than zero.
    *   `PeakAmplitude` is finite and its absolute value does not exceed `1.0f`.
    *   `RmsEnergy` is finite and non-negative.
    *   **Throws:** `ArgumentNullException` if `value` is null.

*   **`public static bool IsValid(this AudioFrame value)`**
    Returns `true` when `Validate()` returns no problems; otherwise, returns `false`.
    *   **Throws:** `ArgumentNullException` if `value` is null.
    *   **Note:** `AudioFrame` also defines an instance method named `IsValid()` with a different, smaller set of checks. Normal `frame.IsValid()` syntax resolves to that instance method. Call `AudioFrameValidation.IsValid(frame)` when the validation helper documented here is required.

*   **`public static void EnsureValid(this AudioFrame value)`**
    Calls `Validate()` and returns normally when no problems are found. Otherwise, it throws an `ArgumentException` whose message starts with `AudioFrame validation failed:` and lists each problem on a new line.
    *   **Throws:** `ArgumentNullException` if `value` is null; `ArgumentException` if validation reports one or more problems.

## Usage

### Analyze, format, and validate a stereo frame

```csharp
using System;
using System.Globalization;
using NAudioVisualizer.Domain.Models;

var frame = new AudioFrame(
    samples: new[] { 0.25f, -0.50f, 0.75f, -1.00f },
    channelCount: 2,
    sampleRate: 48_000,
    frameIndex: 0);

float averageAmplitude = frame.CalculateAverageAmplitude(); // 0.625f
IReadOnlyDictionary<int, float[]> channels = frame.GetChannelDataDictionary();

Console.WriteLine(string.Join(", ", channels[0])); // 0.25, 0.75
Console.WriteLine(frame.Format(CultureInfo.InvariantCulture));

IReadOnlyList<string> problems = frame.Validate();
if (AudioFrameValidation.IsValid(frame))
{
    frame.EnsureValid();
}
```

## Notes

*   All helpers reject a null `AudioFrame` with `ArgumentNullException`.
*   `GetChannelDataDictionary` copies channel data; changing a returned channel array does not change `frame.Samples`.
*   `Validate` reports problems without modifying the frame, and `EnsureValid` does not attempt to repair invalid values.