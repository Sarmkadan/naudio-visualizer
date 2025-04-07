# ColorUtility

The `ColorUtility` class provides a collection of static helper methods for common color space conversions, interpolation, palette mapping, and color manipulation. It is designed for use in audio visualization contexts, where colors are often derived from frequency or amplitude data. All methods operate on `System.Drawing.Color` values and return new `Color` instances; no instance state is maintained.

## API

### `RgbToHsv(Color color)`
Converts an RGB color to its HSV (Hue, Saturation, Value) representation.

- **Parameters**  
  `color` – The RGB color to convert.
- **Returns**  
  A tuple `(double hue, double saturation, double value)` where each component is in the range [0, 1] (hue is in degrees normalized to [0, 360]).
- **Throws**  
  None.

### `HsvToRgb(double hue, double saturation, double value)`
Converts HSV components back to an RGB color.

- **Parameters**  
  `hue` – Hue in degrees (0–360). Values outside this range are wrapped.  
  `saturation` – Saturation in the range [0, 1]. Values outside are clamped.  
  `value` – Value (brightness) in the range [0, 1]. Values outside are clamped.
- **Returns**  
  A `Color` representing the converted RGB value.
- **Throws**  
  None.

### `LerpColor(Color from, Color to, float amount)`
Linearly interpolates between two colors.

- **Parameters**  
  `from` – Starting color.  
  `to` – Ending color.  
  `amount` – Interpolation factor, typically in [0, 1]. Values outside this range are clamped.
- **Returns**  
  A `Color` that is the linear blend of `from` and `to` by `amount`.
- **Throws**  
  None.

### `GetViririsColor(float value)`
Maps a scalar value to a color from the Viridis-like palette (a perceptually uniform colormap).

- **Parameters**  
  `value` – A normalized value in the range [0, 1]. Values outside are clamped.
- **Returns**  
  A `Color` from the Viridis-inspired gradient.
- **Throws**  
  None.

### `GetJetColor(float value)`
Maps a scalar value to a color from the Jet colormap (blue-cyan-green-yellow-red).

- **Parameters**  
  `value` – A normalized value in the range [0, 1]. Values outside are clamped.
- **Returns**  
  A `Color` from the Jet gradient.
- **Throws**  
  None.

### `GetGrayscale(float value)`
Maps a scalar value to a grayscale color.

- **Parameters**  
  `value` – A normalized value in the range [0, 1] (0 = black, 1 = white). Values outside are clamped.
- **Returns**  
  A `Color` with equal RGB components.
- **Throws**  
  None.

### `AdjustBrightness(Color color, float factor)`
Adjusts the brightness (value) of a color by a multiplicative factor.

- **Parameters**  
  `color` – The source color.  
  `factor` – Brightness multiplier. Values ≤ 0 produce black; values > 1 increase brightness (clamped to maximum).
- **Returns**  
  A new `Color` with the same hue and saturation but modified value.
- **Throws**  
  None.

### `AdjustSaturation(Color color, float factor)`
Adjusts the saturation of a color by a multiplicative factor.

- **Parameters**  
  `color` – The source color.  
  `factor` – Saturation multiplier. Values ≤ 0 produce grayscale; values > 1 increase saturation (clamped to maximum).
- **Returns**  
  A new `Color` with the same hue and value but modified saturation.
- **Throws**  
  None.

### `GetComplementaryColor(Color color)`
Returns the complementary (opposite) color on the color wheel.

- **Parameters**  
  `color` – The source color.
- **Returns**  
  A `Color` whose hue is shifted by 180 degrees, with the same saturation and value.
- **Throws**  
  None.

### `ColorToHex(Color color)`
Converts a color to its hexadecimal string representation (e.g., `#FFAABB`).

- **Parameters**  
  `color` – The color to convert.
- **Returns**  
  A string in the format `#RRGGBB` (uppercase hexadecimal digits).
- **Throws**  
  None.

### `HexToColor(string hex)`
Parses a hexadecimal color string into a `Color`.

- **Parameters**  
  `hex` – A string in the format `#RRGGBB` or `RRGGBB` (with or without leading `#`). Case-insensitive.
- **Returns**  
  A `Color` corresponding to the parsed hex value.
- **Throws**  
  `ArgumentException` if `hex` is `null`, empty, or not a valid 6-digit hex color.

## Usage

### Example 1: Mapping audio amplitude to a Jet color and converting to hex

```csharp
using System.Drawing;
using NAudio.Visualizer;

float amplitude = 0.75f; // normalized amplitude from audio analysis
Color jetColor = ColorUtility.GetJetColor(amplitude);
string hex = ColorUtility.ColorToHex(jetColor);
Console.WriteLine($"Amplitude {amplitude} -> Jet color {hex}");
```

### Example 2: Creating a complementary color scheme for a visualization overlay

```csharp
using System.Drawing;
using NAudio.Visualizer;

Color baseColor = Color.FromArgb(50, 120, 200);
Color complement = ColorUtility.GetComplementaryColor(baseColor);
Color brightComplement = ColorUtility.AdjustBrightness(complement, 1.2f);
Color desaturatedBase = ColorUtility.AdjustSaturation(baseColor, 0.3f);

// Use these colors for different visual elements
```

## Notes

- All methods are static and thread-safe; they do not modify any shared state and can be called concurrently from multiple threads.
- HSV conversion methods use double precision; floating-point rounding may cause slight differences when round-tripping `RgbToHsv` followed by `HsvToRgb`.
- When saturation is zero, the hue is undefined; `RgbToHsv` returns a hue of 0 in that case.
- `HexToColor` expects exactly six hexadecimal digits; three-digit shorthand (e.g., `#ABC`) is not supported and will throw an `ArgumentException`.
- Clamping behavior for `value` and `factor` parameters ensures that returned colors always have valid ARGB components (0–255). Out-of-range inputs are silently clamped rather than throwing.
- The `GetViririsColor` and `GetJetColor` methods use precomputed lookup tables; performance is suitable for real-time visualization.
