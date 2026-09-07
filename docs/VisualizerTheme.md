# VisualizerTheme

The `GradientStop` and `VisualizerTheme` classes define the colors used to render waveform and spectrogram views. Colors are represented as unsigned ARGB values in `0xAARRGGBB` format, and gradient positions are normalized from `0` to `1`.

## GradientStop API

### Properties

*   **`public float Position`**
    Gets the normalized position of the stop. A value of `0` represents the start of the gradient and `1` represents the end.

*   **`public uint Color`**
    Gets the ARGB color assigned to the stop.

### Constructor

*   **`public GradientStop(float position, uint color)`**
    Creates a gradient stop at the specified position and color.
    *   **Parameters**:
        *   `position`: The normalized gradient position in the inclusive range `[0, 1]`.
        *   `color`: The color in `0xAARRGGBB` format.
    *   **Throws**: `ArgumentOutOfRangeException` when `position` is less than `0` or greater than `1`.

## VisualizerTheme API

### Properties

*   **`public string Name`**
    Gets the human-readable theme name.

*   **`public uint BackgroundColor`**
    Gets the background fill color in `0xAARRGGBB` format.

*   **`public IReadOnlyList<GradientStop> WaveformGradient`**
    Gets the gradient used to color the waveform from its bottom at position `0` to its top at position `1`.

*   **`public IReadOnlyList<GradientStop> SpectrogramPalette`**
    Gets the gradient that maps normalized spectrogram intensity to color. Position `0` corresponds to silence or minimum dB, while position `1` corresponds to the loudest signal or maximum dB.

### Constructor

*   **`public VisualizerTheme(string name, uint backgroundColor, IReadOnlyList<GradientStop> waveformGradient, IReadOnlyList<GradientStop> spectrogramPalette)`**
    Creates a visualizer theme from a name, background color, waveform gradient, and spectrogram palette.
    *   **Parameters**:
        *   `name`: The human-readable theme name.
        *   `backgroundColor`: The background color in `0xAARRGGBB` format.
        *   `waveformGradient`: At least two stops used for waveform coloring.
        *   `spectrogramPalette`: At least two stops used for spectrogram intensity mapping.

## Validation Rules

*   A `GradientStop` position must be between `0` and `1`, inclusive. Positions outside that range cause `ArgumentOutOfRangeException`.
*   The position check uses less-than and greater-than comparisons, so `float.NaN` is not rejected.
*   A theme name cannot be null, empty, or consist only of whitespace. Any of these values causes `ArgumentNullException`.
*   `waveformGradient` must be non-null and contain at least two stops. Otherwise, the constructor throws `ArgumentException` with `waveformGradient` as the parameter name.
*   `spectrogramPalette` must be non-null and contain at least two stops. Otherwise, the constructor throws `ArgumentException` with `spectrogramPalette` as the parameter name.
*   `VisualizerTheme` does not validate stop ordering, require endpoint positions, or copy the supplied collections. It stores the supplied `IReadOnlyList<GradientStop>` references.

## Presets

The nested static `VisualizerTheme.Presets` class exposes three built-in themes as static get-only properties:

*   **`public static VisualizerTheme Classic`**
    A black-background theme with a dark-green-to-green waveform gradient and a five-stop heat-map spectrogram palette progressing through black, blue, red, yellow, and white.

*   **`public static VisualizerTheme Accessible`**
    A dark-blue-background theme with a blue-to-gold waveform gradient and a four-stop spectrogram palette progressing through dark blue, blue, orange, and pale yellow-white.

*   **`public static VisualizerTheme Monochrome`**
    A black-background theme with a dark-grey-to-white waveform gradient and a three-stop black, grey, and white spectrogram palette.

## Usage

The following example creates a custom theme and also retrieves a built-in preset:

```csharp
using NAudioVisualizer.Domain.Models;

var customTheme = new VisualizerTheme(
    name: "Blue Ocean",
    backgroundColor: 0xFF001122,
    waveformGradient: new[]
    {
        new GradientStop(0f, 0xFF003366),
        new GradientStop(1f, 0xFFCCEEFF)
    },
    spectrogramPalette: new[]
    {
        new GradientStop(0f, 0xFF000011),
        new GradientStop(0.5f, 0xFF0066AA),
        new GradientStop(1f, 0xFFFFFFFF)
    });

VisualizerTheme printTheme = VisualizerTheme.Presets.Monochrome;
```

## Notes

*   `GradientStop` and `VisualizerTheme` expose get-only properties after construction.
*   The `IReadOnlyList<GradientStop>` property types prevent mutation through those properties, but the constructor does not make defensive copies of the supplied collections.
