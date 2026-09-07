# VstParameter

`VstParameter` describes one live, automatable control exposed by a VST plugin. The same model file also defines timed automation points and lanes, the available interpolation modes, and `VstPreset` snapshots. For plugin identity and capability metadata, see [VstPluginInfo](VstPluginInfo.md).

## API

### VstParameter

`VstParameter` is created with an integer ID, name, label, units, minimum value, maximum value, and default value. A `null` name throws `ArgumentNullException`; `null` labels and units are converted to empty strings. `CurrentValue` initially equals `DefaultValue`.

*   **`int Id`**
    Gets the zero-based parameter index used in VST API calls.
*   **`string Name`**
    Gets the parameter's display name.
*   **`string Label`**
    Gets the display label appended to a formatted value.
*   **`string Units`**
    Gets the measurement-unit hint.
*   **`float MinValue`**, **`float MaxValue`**, and **`float DefaultValue`**
    Get the parameter's raw domain range and default value.
*   **`float CurrentValue`**
    Gets or sets the current raw value.
*   **`bool IsAutomated`**
    Gets or sets whether an automation lane is actively writing the parameter.
*   **`string Category`**
    Gets the optional grouping name. It is init-only and defaults to an empty string.
*   **`bool IsReadOnly`**
    Gets whether the host must avoid writing the parameter. It is init-only.
*   **`float NormalizedValue`**
    Converts `CurrentValue` to the range relative to `MinValue` and `MaxValue`. It returns `0` when the absolute range is less than `0.0001`; otherwise the result is not clamped.
*   **`bool IsValid()`**
    Returns `true` when the minimum is not greater than the maximum and `CurrentValue` is finite and within that inclusive range.
*   **`float DenormalizeValue(float normalized)`**
    Clamps `normalized` to `[0, 1]` and maps it into the raw parameter range.
*   **`string ToString()`**
    Formats the name, current value to three decimal places, and units, trimming trailing whitespace.

### VstParameterAutomationPoint

`VstParameterAutomationPoint` is a sealed record with positional properties **`double PositionSeconds`** and **`float Value`**. Its init-only **`Interpolation`** property defaults to `VstAutomationInterpolation.Linear` and controls the segment from this point toward the next point.

**`IsValid()`** returns `true` when the position is non-negative and the value is neither `NaN` nor infinity. It does not restrict the value to `[0, 1]`.

### VstParameterAutomationLane

An automation lane is an ordered collection of points for one plugin parameter. Access to its internal point list is protected by a lock.

*   **`required Guid PluginId`** and **`required int ParameterId`**
    Identify the owning plugin and its zero-based parameter index.
*   **`string ParameterName`**
    Gets the init-only display name, which defaults to an empty string.
*   **`bool IsEnabled`**
    Gets or sets whether playback should use the lane. It defaults to `true`; `Evaluate` itself does not inspect this flag.
*   **`IReadOnlyList<VstParameterAutomationPoint> Points`**
    Gets a read-only view of the internally sorted points.
*   **`int PointCount`**
    Gets the current number of points.

#### AddPoint

**`void AddPoint(double positionSeconds, float value, VstAutomationInterpolation interpolation = VstAutomationInterpolation.Linear)`** adds a point, removes every existing point less than 1 ms from the supplied position, and sorts the lane by ascending position. A negative position throws `ArgumentOutOfRangeException`. The method does not validate or clamp `value`.

#### RemovePoint

**`void RemovePoint(double positionSeconds)`** removes every point whose position differs from the supplied position by less than 1 ms. It does not throw when no point matches.

#### Evaluate

**`float? Evaluate(double positionSeconds)`** returns:

*   `null` for an empty lane;
*   the only point's value for a one-point lane;
*   the first or last value when the requested position lies outside the point range; or
*   the interpolated value for the enclosing segment.

The interpolation mode is read from the segment's earlier point. `Evaluate` does not check `IsEnabled`.

#### Clear

**`void Clear()`** removes all points while leaving the lane object available for reuse.

### Interpolation modes

`VstAutomationInterpolation` supplies four modes:

*   **`Linear`**: linearly blends from the earlier point to the next point.
*   **`CubicSpline`**: uses a Catmull-Rom cubic spline, taking neighboring point values into account and repeating the nearest endpoint value when a neighbor is unavailable.
*   **`Step`**: holds the earlier point's value throughout the segment; the next value is returned when its position is reached.
*   **`Cosine`**: applies a cosine S-curve between the two point values.

### VstPreset

`VstPreset` is a named snapshot of one plugin's state.

*   **`Guid Id`**: Init-only identifier, initialized with `Guid.NewGuid()`.
*   **`required Guid PluginId`**: Identifies the plugin from which the preset was captured.
*   **`required string Name`**: Gets the preset's display name.
*   **`string Description`**, **`string Category`**, and **`string Author`**: Init-only metadata that defaults to empty strings.
*   **`IReadOnlyList<string> Tags`**: Init-only freeform tags; defaults to an empty list.
*   **`byte[]? PluginChunk`**: Optional opaque plugin state. A non-empty chunk takes precedence over `ParameterValues` when applying the preset.
*   **`IReadOnlyDictionary<int, float> ParameterValues`**: Normalized values keyed by zero-based parameter ID; defaults to an empty dictionary.
*   **`DateTime CreatedAt`** and **`DateTime ModifiedAt`**: Init-only UTC timestamps, each initialized to `DateTime.UtcNow`.
*   **`bool IsFactory`**: Indicates a factory preset that should be treated as read-only by the UI.
*   **`bool IsValid()`**: Requires a non-empty plugin ID, a non-whitespace name, and either a non-empty plugin chunk or at least one parameter value.
*   **`string ToString()`**: Returns the name alone when `Category` is empty, or `"[Category] Name"` otherwise.

## Usage

```csharp
using NAudioVisualizer.Domain.Models;

var cutoff = new VstParameter(
    id: 0,
    name: "Cutoff",
    label: "Hz",
    units: "Hz",
    minValue: 20f,
    maxValue: 20_000f,
    defaultValue: 1_000f)
{
    Category = "Filter"
};

cutoff.CurrentValue = cutoff.DenormalizeValue(0.5f);

var lane = new VstParameterAutomationLane
{
    PluginId = Guid.NewGuid(),
    ParameterId = cutoff.Id,
    ParameterName = cutoff.Name
};

lane.AddPoint(0.0, 0.2f, VstAutomationInterpolation.Linear);
lane.AddPoint(2.0, 0.8f, VstAutomationInterpolation.Cosine);

if (lane.IsEnabled && lane.Evaluate(1.0) is float normalizedValue)
{
    cutoff.CurrentValue = cutoff.DenormalizeValue(normalizedValue);
    cutoff.IsAutomated = true;
}
```

## Notes

*   Automation lane collection operations are synchronized, but `VstParameter` and `VstPreset` do not add synchronization around their data.
*   Automation values are described as normalized values, but `AddPoint` and `Evaluate` do not clamp them. Validate or clamp values at the integration boundary when required.
*   The lane retains points in ascending position order. The 1 ms replacement and removal tolerance uses a strict comparison (`< 0.001`), so points exactly 1 ms apart are distinct.
