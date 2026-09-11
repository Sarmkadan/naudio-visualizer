# VstPluginData

## Overview

This file contains the data models and enumerations for VST plugin integration in the NAudioVisualizer application. It defines the core types used to represent plugin state, parameters, automation, and presets.

## API

### Enumerations

#### VstPluginState

Lifecycle states for a VST plugin instance managed by the host.

| Value | Description |
|-------|-------------|
| Unloaded = 0 | Plugin library not yet loaded into memory. |
| Loaded = 1 | Library loaded; waiting for `Initialize()` to be called. |
| Initializing = 2 | One-time initialisation in progress. |
| Active = 3 | Fully initialised and accepting audio I/O. |
| Suspended = 4 | Temporarily suspended; state is preserved, real-time resources freed. |
| Error = 5 | Unrecoverable error; the plugin should be unloaded and discarded. |

#### VstPluginCategory

Functional categories used to classify and filter VST plugins in the browser.

| Value | Description |
|-------|-------------|
| Undefined = 0 | Category has not been declared by the plugin. |
| Effect = 1 | General audio effect (e.g. compressor, saturator). |
| Synth = 2 | Software synthesiser / instrument. |
| Analyzer = 3 | Spectrum or metering analyser. |
| Spatial = 4 | Spatial / positioning effect (panner, binaural). |
| Mastering = 5 | Mastering-grade processing (limiter, stereo enhancer). |
| Dynamics = 6 | Dynamic-range processor (compressor, gate, expander). |
| EQ = 7 | Equaliser or filter. |
| Reverb = 8 | Reverberation or room simulation. |
| Delay = 9 | Delay or echo. |
| Distortion = 10 | Distortion, overdrive, or saturation. |
| Modulation = 11 | Modulation effect (chorus, flanger, phaser, tremolo). |

#### VstAutomationInterpolation

Interpolation curves available when evaluating a `VstParameterAutomationLane`.

| Value | Description |
|-------|-------------|
| Linear = 0 | Straight line between adjacent automation points. |
| CubicSpline = 1 | Smooth Catmull-Rom cubic spline through surrounding points. |
| Step = 2 | Instant jump to the next point's value with no ramp. |
| Cosine = 3 | S-curve cosine crossfade between adjacent points. |

### Classes and Records

#### VstPluginInfo

Immutable snapshot of a loaded VST plugin's identity, capabilities, and provenance. Produced by the host at load time and never mutated thereafter.

**Properties**

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier for the plugin instance. |
| Name | string | Plugin name. |
| Vendor | string | Plugin vendor. |
| Version | string | Plugin version. |
| PluginPath | string | File system path to the plugin. |
| Category | VstPluginCategory | Functional category of the plugin. |
| ParameterCount | int | Number of parameters exposed by the plugin. |
| IsSynth | bool | Whether the plugin is a synthesizer. |
| LoadedAt | DateTime | UTC timestamp recorded when the plugin was first loaded into the host. |
| UniqueId | string | Four-character unique ID reported by the plugin (VST2 `uniqueID` field), or empty string if not exposed. |
| SdkVersion | string | Human-readable VST SDK version string (e.g. "VST 2.4", "VST 3.7"). |
| MaxChannels | int | Maximum number of audio I/O channels the plugin supports per side. |

**Methods**

| Method | Description |
|--------|-------------|
| IsValid() | Checks that all required fields are present and consistent. Returns `true` if `Id` is not empty, `Name` and `PluginPath` are not whitespace, `ParameterCount` is non-negative, and `MaxChannels` is positive. |
| ToString() | Returns a formatted string: `{Vendor} – {Name} v{Version} [{Category}]`. |

#### VstParameter

Live descriptor for a single automatable control exposed by a VST plugin. The host updates `CurrentValue` as parameters change during playback.

**Properties**

| Property | Type | Description |
|----------|------|-------------|
| Id | int | Zero-based parameter index used in all VST API calls. |
| Name | string | Short human-readable name displayed in the parameter list (e.g. "Cutoff", "Attack"). |
| Label | string | Display label appended to the formatted value in a UI (e.g. "Hz", "ms", "%"). |
| Units | string | Measurement unit hint for tooltip display (e.g. "dB", "semitones"). |
| MinValue | float | Minimum raw (domain) value on the plugin's internal scale. |
| MaxValue | float | Maximum raw (domain) value on the plugin's internal scale. |
| DefaultValue | float | Default raw value restored when the user resets the parameter. |
| CurrentValue | float | Current raw value; updated by the host in real time from automation or UI gestures. |
| IsAutomated | bool | `true` when an automation lane is actively writing to this parameter. |
| Category | string | Optional grouping name for organising parameters in a browser (e.g. "Envelope", "Filter"). |
| IsReadOnly | bool | When `true` the host must not write this parameter; it is updated exclusively by the plugin itself. |
| NormalizedValue | float | Current value normalised to [0.0, 1.0]. Returns 0 when the usable range is zero. |

**Methods**

| Method | Description |
|--------|-------------|
| IsValid() | Returns `true` when all field invariants hold and `CurrentValue` is within the declared range. |
| DenormalizeValue(float normalized) | Converts a normalised [0, 1] value to the plugin's raw domain range. |
| ToString() | Returns a formatted string: `{Name}: {CurrentValue:F3} {Units}` (trailing spaces trimmed). |

#### VstParameterAutomationPoint

A single timed value on an automation lane, identified by its position in seconds.

**Properties**

| Property | Type | Description |
|----------|------|-------------|
| PositionSeconds | double | Timeline position in seconds. |
| Value | float | Parameter value at this position. |
| Interpolation | VstAutomationInterpolation | Interpolation shape applied between this point and the next one on the lane. |

**Methods**

| Method | Description |
|--------|-------------|
| IsValid() | Returns `true` when the point's position is non-negative and `Value` is a normal finite number. |

#### VstParameterAutomationLane

An ordered, thread-safe collection of `VstParameterAutomationPoint` values that drives a single VST parameter over a timeline.

**Properties**

| Property | Type | Description |
|----------|------|-------------|
| PluginId | Guid | Plugin that owns the parameter driven by this lane. |
| ParameterId | int | Zero-based parameter index within the owning plugin. |
| ParameterName | string | Display name copied from the parameter descriptor at creation time. |
| IsEnabled | bool | When `false` the lane is ignored during playback evaluation. |
| Points | IReadOnlyList<VstParameterAutomationPoint> | Read-only snapshot of the automation points sorted ascending by position. |
| PointCount | int | Total number of automation points currently on the lane. |

**Methods**

| Method | Description |
|--------|-------------|
| AddPoint(double positionSeconds, float value, VstAutomationInterpolation interpolation = VstAutomationInterpolation.Linear) | Inserts a point at `positionSeconds`, replacing any existing point within 1 ms of that position. Throws `ArgumentOutOfRangeException` if `positionSeconds` is negative. |
| RemovePoint(double positionSeconds) | Removes the point closest to `positionSeconds` (within a 1 ms tolerance). |
| Evaluate(double positionSeconds) | Evaluates the lane at `positionSeconds` using the interpolation shape stored on each point. Returns the interpolated value, or `null` when the lane has no points. |
| Clear() | Removes all points from the lane, leaving it empty but still registered. |
| ToString() | Returns a formatted string: `Parameter {ParameterId}: {_points.Count} points`. |

#### VstPreset

A complete, named snapshot of a VST plugin's parameter state that can be saved, loaded, categorised, and applied back to the plugin at any time.

**Properties**

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier assigned when the preset is created. |
| PluginId | Guid | Identifier of the plugin this preset was captured from. |
| Name | string | Human-readable name displayed in the preset browser. |
| Description | string | Optional prose description of the sound or intended use. |
| Category | string | Top-level category for browser organisation (e.g. "Pads", "Leads", "Bus Comp"). |
| Tags | IReadOnlyList<string> | Freeform tags that enable multi-dimensional filtering (e.g. "warm", "dark", "analog"). |
| PluginChunk | byte[]? | Raw opaque chunk captured via `IVstPlugin.GetChunk(isPreset: true)`. Takes precedence over `ParameterValues` when non-empty. |
| ParameterValues | IReadOnlyDictionary<int, float> | Explicit parameter values keyed by zero-based parameter ID. Used when the plugin does not support chunk serialisation or for fine-grained overrides. Values are normalised to [0.0, 1.0]. |
| CreatedAt | DateTime | UTC timestamp when this preset was first created. |
| ModifiedAt | DateTime | UTC timestamp of the last modification. |
| IsFactory | bool | When `true` this is a factory preset shipped with the plugin and should be treated as read-only in the UI. |
| Author | string | Name of the author who created the preset, or empty for factory presets. |

**Methods**

| Method | Description |
|--------|-------------|
| IsValid() | Returns `true` when the preset contains the minimum data required for it to be applied: a non-empty name, a valid plugin reference, and at least one source of parameter state (chunk or individual values). |
| ToString() | Returns a formatted string: `[{Category}] {Name}` if `Category` is not empty, otherwise just `{Name}`. |

## Usage Example

```csharp
// Creating a VstPluginInfo
var pluginInfo = new VstPluginInfo(
    Guid.NewGuid(),
    "Example Plugin",
    "Example Vendor",
    "1.0.0",
    "/path/to/plugin.vst3",
    VstPluginCategory.Effect,
    4,
    false)
{
    LoadedAt = DateTime.UtcNow,
    UniqueId = "EXPL",
    SdkVersion = "VST 3.7",
    MaxChannels = 2
};

// Creating a VstParameter
var parameter = new VstParameter(
    0,
    "Cutoff",
    "Hz",
    "",
    20.0f,
    20000.0f,
    1000.0f)
{
    CurrentValue = 1000.0f,
    IsAutomated = false,
    Category = "Filter"
};

// Creating an automation lane and adding points
var automationLane = new VstParameterAutomationLane
{
    PluginId = pluginInfo.Id,
    ParameterId = parameter.Id,
    ParameterName = parameter.Name
};

automationLane.AddPoint(0.0, 0.0f, VstAutomationInterpolation.Linear);
automationLane.AddPoint(1.0, 1.0f, VstAutomationInterpolation.Linear);

// Evaluating the automation at 0.5 seconds
float? valueAtHalfSecond = automationLane.Evaluate(0.5); // Returns 0.5

// Creating a VstPreset
var preset = new VstPreset
{
    Id = Guid.NewGuid(),
    PluginId = pluginInfo.Id,
    Name = "Default Setting",
    Description = "Default parameter values",
    Category = "Presets",
    Tags = new List<string> { "default", "init" },
    ParameterValues = new Dictionary<int, float> { [0] = 0.05f },
    CreatedAt = DateTime.UtcNow,
    ModifiedAt = DateTime.UtcNow
};
```

## Notes

- All classes and records in this file are designed to be immutable where possible (`VstPluginInfo`, `VstParameterAutomationPoint`, `VstPreset` are records or have immutable properties). `VstParameter` and `VstParameterAutomationLane` are mutable to support runtime updates.
- `VstParameterAutomationLane` is thread-safe for concurrent access via internal locking.
- The `IsValid()` methods on each type provide validation logic to ensure objects are in a consistent state.
- When implementing VST host functionality, refer to the VST SDK documentation for details on parameter automation and chunk-based preset handling.