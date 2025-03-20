# GradientStop

A `GradientStop` represents a specific color point within a gradient definition used for rendering visualizer elements. It encapsulates positioning information and color data required to construct smooth color transitions for waveform and spectrogram components within the `naudio-visualizer` project.

## API

### Members

*   **`public float Position`**
    Gets or sets the normalized offset of the gradient stop. Values typically range from 0.0 (start of gradient) to 1.0 (end of gradient).

*   **`public uint Color`**
    Gets or sets the color value represented as a 32-bit unsigned integer, typically formatted in ARGB order.

*   **`public GradientStop()`**
    Initializes a new instance of the `GradientStop` class.

*   **`public string Name`**
    Gets or sets the display name associated with the gradient or theme configuration.

*   **`public uint BackgroundColor`**
    Gets or sets the base background color value for the visualization, represented as a 32-bit unsigned integer.

*   **`public IReadOnlyList<GradientStop> WaveformGradient`**
    Gets or sets the sequence of gradient stops that define the color spectrum applied to the rendered waveform.

*   **`public IReadOnlyList<GradientStop> SpectrogramPalette`**
    Gets or sets the sequence of gradient stops that define the color palette used for spectrogram frequency visualization.

*   **`public VisualizerTheme`**
    Represents the associated theme type context.

*   **`public static VisualizerTheme Classic`**
    Gets a pre-configured `VisualizerTheme` instance using the standard color palette.

*   **`public static VisualizerTheme Accessible`**
    Gets a pre-configured `VisualizerTheme` instance optimized for high-contrast visibility.

*   **`public static VisualizerTheme Monochrome`**
    Gets a pre-configured `VisualizerTheme` instance using a grayscale color palette.

## Usage

```csharp
// Define a single gradient stop at 50% position with a red color
var stop = new GradientStop {
    Position = 0.5f,
    Color = 0xFFFF0000 // ARGB format
};
```

```csharp
// Access a pre-defined theme and retrieve its waveform gradient
var theme = GradientStop.Classic;
IReadOnlyList<GradientStop> gradient = theme.WaveformGradient;

Console.WriteLine($"Using theme: {theme.Name}");
```

## Notes

*   **Position Range**: While the `Position` property accepts any `float` value, behavior for values outside the [0.0, 1.0] range is implementation-defined and should generally be clamped to valid bounds by the rendering consumer.
*   **Color Encoding**: The `Color` and `BackgroundColor` properties utilize `uint` as a raw representation. Ensure input values adhere to the specific ARGB byte order expected by the underlying rendering pipeline.
*   **Thread Safety**: These types act primarily as data-transfer objects. They are safe to read from multiple threads simultaneously; however, they are not inherently thread-safe for concurrent read-write operations. Instances should be treated as immutable once assigned to a rendering configuration.
