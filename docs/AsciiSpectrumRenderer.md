# AsciiSpectrumRenderer

The `AsciiSpectrumRenderer` class converts an array of spectrum magnitudes into a multi-line ASCII bar chart. It maps the input values to the requested number of columns, scales each column relative to the largest mapped value, and renders the result with Unicode block characters. Optional peak-hold markers can be enabled through a `ConfigurationManager`.

## API

### Constructor

*   **`public AsciiSpectrumRenderer(int width = 80, int height = 20, bool logScale = false, ConfigurationManager? config = null)`**
    Creates a new spectrum renderer.
    *   **Parameters**:
        *   `width`: The requested number of output columns. The default is `80`. During rendering, the effective width is limited to `Console.WindowWidth`.
        *   `height`: The requested number of output rows. The default is `20`. During rendering, the effective height is limited to `Console.WindowHeight`.
        *   `logScale`: When `false`, magnitudes are used unchanged. When `true`, each magnitude is transformed with `Math.Log10(value + 1e-6f)` before maximum detection and bar-height calculation.
        *   `config`: An optional `ConfigurationManager` used to read peak-hold settings. `visualization.peakHoldEnabled` defaults to `false`, and `visualization.peakHoldFallRateDbPerSec` defaults to `10f`.
    *   **Throws**: `ArgumentOutOfRangeException` if `width` or `height` is less than or equal to zero.

### Methods

*   **`public string Render(float[] spectrumFrame)`**
    Renders a spectrum frame as a bar chart.
    *   **Parameters**:
        *   `spectrumFrame`: An array of spectrum magnitudes.
    *   **Returns**: A string containing one line per effective output row, with each line terminated by the environment newline. Returns `string.Empty` when `spectrumFrame` is empty.
    *   **Throws**: `ArgumentNullException` if `spectrumFrame` is `null`.
    *   **Behavior**:
        *   If the input has fewer values than the effective width, it is padded with zero-valued columns. If it has more values, consecutive blocks are averaged to produce the effective width; any remainder is distributed one extra value at a time from the first column.
        *   The largest scaled mapped value becomes the chart maximum. A non-positive maximum is replaced with `1f` to avoid division by zero.
        *   Each bar height is the rounded ratio of its scaled value to the maximum, multiplied by the effective height.
        *   Filled cells use one of `▁`, `▂`, `▃`, `▄`, `▅`, `▆`, `▇`, or `█`, selected from the column's rounded bar height. Heights beyond the glyph range use `█`; empty cells are spaces.
        *   When peak hold is enabled, stored peaks decay according to the configured dB-per-second fall rate. A `^` is drawn for peak levels above the current bar.

## Usage

```csharp
using NaudioVisualizer.Services;

var renderer = new AsciiSpectrumRenderer(
    width: 4,
    height: 4,
    logScale: false);

string chart = renderer.Render(new[] { 0.25f, 0.5f, 0.75f, 1.0f });
Console.Write(chart);
```

With a console window at least four columns wide and four rows high, the output is:

```text
   █
  ▃█
 ▂▃█
▁▂▃█
```

Each displayed row is followed by `Environment.NewLine`, including the final row. Actual dimensions can be smaller than the requested dimensions when the console window is smaller.

## Notes

*   `AsciiSpectrumRenderer` does not expose its configured dimensions or peak state as public properties.
*   Peak-hold timing uses elapsed UTC time between render calls. Peak state belongs to the renderer instance and is retained across calls to `Render`.
*   The renderer reuses an internal `StringBuilder`, so concurrent calls on the same instance are not synchronized by the class.
