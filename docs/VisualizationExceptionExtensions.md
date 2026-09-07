# VisualizationExceptionExtensions

The `VisualizationExceptionExtensions` static class provides helpers for formatting a `VisualizationException`, comparing its visualization type, and creating a new exception associated with a different visualization type.

## API

### `public static string GetFormattedMessage(this VisualizationException exception)`

Returns the exception message followed by ` (Visualization Type: {value})` when `VisualizationType` is not `null`. If `VisualizationType` is `null`, it returns the exception message unchanged.

- **Parameters**
  - `exception`: The `VisualizationException` to format.
- **Return value**
  - The formatted message, including the visualization type when one is available.
- **Exceptions**
  - Throws `ArgumentNullException` if `exception` is `null`.

---

### `public static bool IsVisualizationType(this VisualizationException exception, string visualizationType)`

Compares the exception's `VisualizationType` with `visualizationType` by using the string equality operator. The comparison is case-sensitive.

- **Parameters**
  - `exception`: The `VisualizationException` to inspect.
  - `visualizationType`: The visualization type to compare with the exception's value. It must not be `null` or empty.
- **Return value**
  - `true` when the two visualization type strings are equal; otherwise, `false`.
- **Exceptions**
  - Throws `ArgumentNullException` if `exception` is `null`.
  - Throws `ArgumentNullException` if `visualizationType` is `null`.
  - Throws `ArgumentException` if `visualizationType` is empty.

---

### `public static VisualizationException WithVisualizationType(this VisualizationException exception, string visualizationType)`

Creates a new `VisualizationException` whose message is copied from `exception` and whose `VisualizationType` is set to `visualizationType`. The original exception becomes the new exception's `InnerException`; the original instance is not modified.

- **Parameters**
  - `exception`: The original `VisualizationException` to wrap.
  - `visualizationType`: The visualization type for the new exception. It must not be `null` or empty.
- **Return value**
  - A new `VisualizationException` containing the copied message, supplied visualization type, and original exception as its inner exception.
- **Exceptions**
  - Throws `ArgumentNullException` if `exception` is `null`.
  - Throws `ArgumentNullException` if `visualizationType` is `null`.
  - Throws `ArgumentException` if `visualizationType` is empty.

## Usage

```csharp
using System;
using NAudioVisualizer.Exceptions;

var original = new VisualizationException("Unable to render the frame");
var waveformError = original.WithVisualizationType("Waveform");

Console.WriteLine(waveformError.GetFormattedMessage());
// Unable to render the frame (Visualization Type: Waveform)

if (waveformError.IsVisualizationType("Waveform"))
{
    Console.WriteLine("Handle the waveform rendering failure.");
}

Console.WriteLine(ReferenceEquals(waveformError.InnerException, original));
// True
```

## Notes

- `GetFormattedMessage` includes an empty `VisualizationType` in the formatted suffix because it checks only for `null`.
- `IsVisualizationType` does not perform case normalization; for example, `"Waveform"` and `"waveform"` do not match.
- `WithVisualizationType` always allocates a new exception and preserves the original exception as the immediate inner exception.
