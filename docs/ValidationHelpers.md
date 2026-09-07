# Validation Helpers

The validation helper classes provide a common set of extension methods for checking the state of `AudioDevice`, `AudioFrame`, `GradientStop`, `ConfigurationManager`, `SpectrogramAnalyzer`, and `AudioStreamException` instances. Each helper exposes `Validate`, `IsValid`, and `EnsureValid`: use `Validate` when error details are needed, `IsValid` for a Boolean check, and `EnsureValid` as a guard that throws when validation fails.

The helpers are defined in the namespaces of the types they validate:

*   `NAudioVisualizer.Domain.Models`: `AudioDeviceValidation`, `AudioFrameValidation`, and `GradientStopValidation`
*   `NAudioVisualizer.Configuration`: `ConfigurationManagerValidation`
*   `NAudioVisualizer.Services`: `SpectrogramAnalyzerValidation`
*   `NAudioVisualizer.Exceptions`: `AudioStreamExceptionValidation`

## Shared API Pattern

### `Validate`

Each `Validate` extension returns an `IReadOnlyList<string>` containing every detected problem, or an empty list when the instance passes all implemented checks. Passing `null` throws `ArgumentNullException` for all six helpers.

### `IsValid`

Each `IsValid` extension returns `true` when `Validate` finds no problems. Null handling differs slightly between implementations:

*   `AudioDeviceValidation.IsValid` and `AudioFrameValidation.IsValid` throw `ArgumentNullException` for `null`.
*   `GradientStopValidation.IsValid`, `ConfigurationManagerValidation.IsValid`, `SpectrogramAnalyzerValidation.IsValid`, and `AudioStreamExceptionValidation.IsValid` return `false` for `null`.

### `EnsureValid`

Each `EnsureValid` extension returns normally when validation succeeds. It throws `ArgumentNullException` for `null` and `ArgumentException` when one or more validation problems are found. The exception message includes the collected problems, although the exact heading, separators, and use of the `value` parameter name vary by helper.

## Validation Rules

### `AudioDeviceValidation`

`Validate(this AudioDevice value)` reports problems when:

*   `Id` is `Guid.Empty`.
*   `Name` or `Manufacturer` is null, empty, or whitespace.
*   `DeviceIndex` is negative.
*   `ChannelCount`, `DefaultSampleRate`, or `BitDepth` is not greater than zero.
*   `SupportedSampleRates` is null, empty, or contains a value that is not positive. A separate problem is added for every invalid rate.
*   `LastStatusCheck` is the default `DateTime` value.
*   `Capabilities` is null.

`EnsureValid` joins the problems on separate lines beneath `AudioDevice validation failed:`.

### `AudioFrameValidation`

`Validate(this AudioFrame value)` checks that:

*   `Id` is not `Guid.Empty`.
*   `Samples` is non-null and non-empty, contains no `NaN` values, and contains no infinite values. Because these sample checks form an `else if` chain, only the first applicable sample-content problem is reported.
*   `ChannelCount` is greater than zero and, when it is positive and `Samples` is non-null, the sample count is divisible by it.
*   `SampleRate` and `DurationSeconds` are greater than zero.
*   `Timestamp` is neither the default value nor a non-UTC `DateTime`.
*   `FrameIndex` is not negative.
*   `PeakAmplitude` is finite and its absolute value does not exceed `1.0f`.
*   `RmsEnergy` is finite and non-negative.

`EnsureValid` joins the problems on separate lines beneath `AudioFrame validation failed:`.

### `GradientStopValidation`

`Validate(this GradientStop? value)` reports a problem when `Position` is `NaN` or infinite, or when it is outside the inclusive range `[0, 1]`. It also reports a problem when `Color` is `0u` and `Position` is not `0f`.

`EnsureValid` combines the problems into one space-separated message beginning with `GradientStop is invalid.` and supplies `value` as the exception parameter name.

### `ConfigurationManagerValidation`

`Validate(this ConfigurationManager value)` first reads all keys through `GetAllKeys`. It reports one problem if it encounters any null, empty, or whitespace key, then stops scanning keys. It also requires the following known settings:

| Setting | Required value |
| --- | --- |
| `audio.sampleRate` | Numeric value from 8000 through 96000 |
| `audio.channelCount` | Numeric value from 1 through 8 |
| `audio.bitDepth` | Numeric value from 8 through 32 |
| `audio.fftSize` | Numeric value from 64 through 16384 |
| `visualization.targetFps` | Numeric value from 1 through 240 |
| `visualization.brightness` | Numeric value from 0.0 through 2.0 |
| `visualization.contrast` | Numeric value from 0.0 through 2.0 |
| `display.width` | Numeric value from 320 through 7680 |
| `display.height` | Numeric value from 240 through 4320 |
| `display.fullscreen` | Key must exist |
| `export.compress` | Key must exist |
| `export.includeMetadata` | Key must exist |
| `logging.writeToConsole` | Key must exist |
| `logging.writeToFile` | Key must exist |
| `export.defaultFormat` | `json`, `xml`, `yaml`, or `csv`, ignoring case |
| `logging.level` | `Debug`, `Info`, `Warn`, or `Error`, ignoring case |

Numeric settings are read with `GetValue<double>`; a failed read is reported as an invalid numeric value. String settings are read with `GetValue<string>` and are rejected when null, empty, whitespace, or outside their allowed set. The Boolean-setting checks only test key presence; an exception during that check is reported as an invalid Boolean value.

`EnsureValid` numbers each problem on a separate line beneath `Configuration validation failed:` and supplies `value` as the exception parameter name.

### `SpectrogramAnalyzerValidation`

`Validate(this SpectrogramAnalyzer? value)` calls `GetBufferFrameCount()`. A negative result is reported as a problem. If the call throws any `Exception`, validation captures it and adds a problem containing the exception message instead of propagating that exception.

`EnsureValid` prefixes each problem with `- ` beneath `SpectrogramAnalyzer validation failed:` and supplies `value` as the exception parameter name.

### `AudioStreamExceptionValidation`

`Validate(this AudioStreamException value)` reports problems when:

*   `ErrorCode` is not a defined `AudioStreamErrorCode` value.
*   The inherited `Message` is null, empty, or whitespace.
*   `InnerException` is present and its `Message` is null, empty, or whitespace.

Unlike the other five `Validate` implementations, this method returns its mutable `List<string>` through the `IReadOnlyList<string>` interface rather than calling `AsReadOnly`.

`EnsureValid` places each problem on a separate line beneath `AudioStreamException is invalid. Problems:` and supplies `value` as the exception parameter name.

## Usage

```csharp
using NAudioVisualizer.Domain.Models;

var device = new AudioDevice("USB microphone", deviceIndex: 0, channelCount: 2)
{
    Manufacturer = "Example Audio"
};
device.AddSupportedSampleRate(48000);

IReadOnlyList<string> problems = device.Validate();

if (device.IsValid())
{
    device.EnsureValid(); // Returns normally.
}
else
{
    foreach (string problem in problems)
    {
        Console.WriteLine(problem);
    }
}
```

## Notes

*   These are extension methods, so the namespace containing the relevant validation class must be imported before calling them with instance syntax.
*   `Validate` gathers problems without changing the validated instance.
*   `IsValid` performs validation each time it is called; it does not cache a prior result.
*   `EnsureValid` performs validation itself, so calling `IsValid` immediately before it validates the same instance twice.
