# VisualizationData Extensions

The `VisualizationDataExtensions` and `VisualizationDataJsonExtensions` static classes provide age checking, data-point scaling, and JSON conversion helpers for `VisualizationData`. Both classes are in the `NAudioVisualizer.Domain.Models` namespace.

## API

### VisualizationDataExtensions

#### IsWithinValidAgeRange

```csharp
public static bool IsWithinValidAgeRange(this VisualizationData data, float maxAge)
```

Calculates the age of `data` in seconds as `DateTime.UtcNow - data.GeneratedAt` and returns `true` when that value is less than or equal to `maxAge`. The method does not require `maxAge` to be non-negative. A future `GeneratedAt` value produces a negative age and can therefore satisfy the comparison.

**Parameters:**
*   `data`: The visualization data whose generation time is checked.
*   `maxAge`: The maximum permitted age in seconds.

**Returns:** `true` if the calculated age is less than or equal to `maxAge`; otherwise, `false`.

**Exceptions:**
*   `ArgumentNullException` if `data` is null.

#### ScaleDataPoints

```csharp
public static IReadOnlyList<float> ScaleDataPoints(
    this VisualizationData data,
    float minValue,
    float maxValue)
```

Returns a newly allocated array whose values are linearly mapped from the source range defined by `data.MinValue` and `data.MaxValue` to the requested range. Source values are obtained by calling `data.GetData()`. The source object and its data are not modified.

When `data.MaxValue` is greater than `data.MinValue`, each result is calculated as `(value - data.MinValue) / (data.MaxValue - data.MinValue) * (maxValue - minValue) + minValue`. The method does not clamp values, so a source value outside the declared source range can produce a result outside the requested range. When `data.MaxValue` is less than or equal to `data.MinValue`, every returned value is `minValue`.

**Parameters:**
*   `data`: The visualization data whose points are scaled.
*   `minValue`: The lower bound of the requested output range.
*   `maxValue`: The upper bound of the requested output range.

**Returns:** An `IReadOnlyList<float>` backed by a new `float[]`, with the same number of elements returned by `data.GetData()`.

**Exceptions:**
*   `ArgumentNullException` if `data` is null.
*   `ArgumentOutOfRangeException` if `minValue` is greater than `maxValue`.

### VisualizationDataJsonExtensions

The JSON helpers use `System.Text.Json` web defaults with camel-case property names, compact output by default, omission of null properties, and cycle references ignored.

#### ToJson

```csharp
public static string ToJson(this VisualizationData value, bool indented = false)
```

Serializes `value` to JSON. Passing `true` for `indented` uses a copy of the shared serializer options with indentation enabled.

**Parameters:**
*   `value`: The visualization data to serialize.
*   `indented`: Whether the JSON should be indented. The default is `false`.

**Returns:** The serialized JSON string.

**Exceptions:**
*   `ArgumentNullException` if `value` is null.

#### FromJson

```csharp
public static VisualizationData? FromJson(string json)
```

Returns `null` for an empty or whitespace-only string. Otherwise, it asks `System.Text.Json` to deserialize the input as `VisualizationData` and returns the resulting value.

**Parameters:**
*   `json`: The JSON text to deserialize.

**Returns:** The deserialized value, or `null` for empty or whitespace-only input or when the JSON value deserializes to null.

**Exceptions:**
*   `ArgumentNullException` if `json` is null.
*   `JsonException` if the JSON is invalid or cannot be converted.
*   Other exceptions produced by `JsonSerializer.Deserialize`, such as `NotSupportedException`, are not caught by this method.

#### TryFromJson

```csharp
public static bool TryFromJson(string json, out VisualizationData? value)
```

Sets `value` to `null` before processing. Empty, whitespace-only, and null input returns `false`. For other input, the method returns `false` only when `JsonSerializer.Deserialize` throws `JsonException`; in that case, `value` remains null. Otherwise, it assigns the deserializer result and returns `true`, including when that result is null (for example, for the JSON literal `null`). Exceptions other than `JsonException` are not caught.

**Parameters:**
*   `json`: The JSON text to deserialize.
*   `value`: Receives the deserializer result when no `JsonException` is thrown; otherwise, null.

**Returns:** `false` for null, empty, or whitespace-only input, or when deserialization throws `JsonException`; otherwise, `true`.

## Usage

```csharp
using System;
using System.Collections.Generic;
using NAudioVisualizer.Domain.Models;

var waveform = new WaveformData(
    samples: new[] { -1.0f, 0.0f, 1.0f },
    channelCount: 1,
    sampleRate: 48_000);

bool isRecent = waveform.IsWithinValidAgeRange(maxAge: 5.0f);
IReadOnlyList<float> scaled = waveform.ScaleDataPoints(0.0f, 100.0f);
// scaled contains 0.0f, 50.0f, and 100.0f.

string json = waveform.ToJson(indented: true);
Console.WriteLine(json);

VisualizationData? empty = VisualizationDataJsonExtensions.FromJson("   ");
bool parsedNull = VisualizationDataJsonExtensions.TryFromJson(
    "null",
    out VisualizationData? restored);
// empty and restored are null; parsedNull is true.
```

## Notes

*   `FromJson` and `TryFromJson` deserialize specifically to the abstract `VisualizationData` type. Whether a concrete subtype can be created depends on the `System.Text.Json` metadata and converters available at runtime; these helpers do not configure a polymorphic type discriminator or custom converter.
*   JSON serialization ignores reference cycles and omits properties whose values are null.
*   Scaling uses the stored `MinValue` and `MaxValue` properties rather than calculating bounds from the array returned by `GetData()`.
