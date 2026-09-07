# ColorSchemeExtensions

Extension methods for querying, formatting, and deriving `ColorScheme` instances. The methods are defined in the `NAudioVisualizer.Themes` namespace.

## Query and Formatting Methods

### IsDark

```csharp
public static bool IsDark(this ColorScheme scheme)
```

Returns `true` only when `scheme` is the same instance as `ColorScheme.Dark`. A derived scheme whose colors resemble the dark preset is not considered dark.

**Exceptions:**
*   `ArgumentNullException` if `scheme` is null.

### IsPredefined

```csharp
public static bool IsPredefined(this ColorScheme scheme)
```

Returns `true` when `scheme` is the same instance as one of the four predefined schemes: `ColorScheme.Dark`, `ColorScheme.Light`, `ColorScheme.Neon`, or `ColorScheme.Grayscale`. The check uses reference identity, so schemes returned by transformation methods are not predefined.

**Exceptions:**
*   `ArgumentNullException` if `scheme` is null.

### ToDisplayString

```csharp
public static string ToDisplayString(this ColorScheme scheme)
```

Returns the scheme name and its theme using the format `"{Name} ({Theme})"`. The text inside the parentheses is produced by the theme's normal string formatting.

**Exceptions:**
*   `ArgumentNullException` if `scheme` is null.

## Transformation Methods

Each transformation returns a new `ColorScheme`; it does not modify the source scheme.

### Reversed

```csharp
public static ColorScheme Reversed(this ColorScheme scheme)
```

Returns a scheme whose waveform and spectrogram stops have positions transformed to `1.0f - position` and are then sorted by their new positions. Stop colors and the background color are unchanged. `" Reversed"` is appended to both the scheme name and theme name.

**Exceptions:**
*   `ArgumentNullException` if `scheme` is null.

### WithBrightness

```csharp
public static ColorScheme WithBrightness(this ColorScheme scheme, float factor)
```

Returns a scheme with `factor` applied to the red, green, and blue channels of the background, waveform stops, and spectrogram stops. Adjusted channels are clamped to the byte range; alpha values and gradient positions are preserved.

If `factor` is greater than `1.0f`, `" Brighter"` is appended to the scheme and theme names. Otherwise, `" Darker"` is appended. The method does not validate the factor or restrict it to a particular range.

**Parameters:**
*   `factor`: The multiplier applied to each RGB channel.

**Exceptions:**
*   `ArgumentNullException` if `scheme` is null.

### Lerp

```csharp
public static ColorScheme Lerp(
    this ColorScheme scheme,
    ColorScheme other,
    float t)
```

Returns a scheme interpolated between corresponding stops and background colors from `scheme` and `other`. Waveform gradients and spectrogram palettes are paired by index, and each result contains only as many stops as the shorter corresponding input collection.

Color-channel interpolation clamps `t` to the range `0.0f` through `1.0f`. Stop positions are calculated with the supplied `t` directly and are not clamped. The returned scheme and theme are both named from the first theme's name with `" Lerp"` appended.

**Parameters:**
*   `other`: The second scheme used for interpolation.
*   `t`: The interpolation amount.

**Exceptions:**
*   `ArgumentNullException` if `scheme` or `other` is null.

## Predefined Schemes

### GetPredefinedSchemes

```csharp
public static IReadOnlyList<ColorScheme> GetPredefinedSchemes(
    this ColorScheme scheme)
```

Returns a new read-only list containing `ColorScheme.Dark`, `ColorScheme.Light`, `ColorScheme.Neon`, and `ColorScheme.Grayscale`, in that order. The receiver is used only to provide extension-method syntax.

**Exceptions:**
*   `ArgumentNullException` if the receiving scheme is null.

## Usage

```csharp
using NAudioVisualizer.Themes;

ColorScheme source = ColorScheme.Dark;

bool isDark = source.IsDark();                 // true
bool isPreset = source.IsPredefined();         // true
string label = source.ToDisplayString();

ColorScheme reversed = source.Reversed();
ColorScheme brighter = source.WithBrightness(1.25f);
ColorScheme blend = source.Lerp(ColorScheme.Neon, 0.5f);

IReadOnlyList<ColorScheme> presets = source.GetPredefinedSchemes();

Console.WriteLine(label);
Console.WriteLine(reversed.Name);               // Dark Reversed
Console.WriteLine(brighter.Name);               // Dark Brighter
Console.WriteLine(blend.Name);                  // Dark Lerp
Console.WriteLine(presets.Count);               // 4
Console.WriteLine(reversed.IsPredefined());      // false
```

## Notes

*   Every public extension method throws `ArgumentNullException` for a null receiver.
*   Colors are represented internally as packed ARGB values; transformations preserve alpha except when interpolation computes it between the two inputs.
*   `GetPredefinedSchemes` returns a read-only wrapper, but the contained entries are the shared predefined `ColorScheme` instances.
