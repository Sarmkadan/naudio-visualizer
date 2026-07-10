# ColorScheme

`ColorScheme` encapsulates a named set of visual parameters used to style the audio visualization pipeline. It pairs a human-readable identifier with a `VisualizerTheme` that defines the actual colors, brushes, and pens applied to waveforms, spectrums, and related UI elements. Predefined static schemes provide ready-to-use light, dark, neon, and grayscale themes, while custom instances can be constructed for application-specific branding.

## API

### `public string Name`

Gets the display name of the color scheme. This value is intended for use in UI lists, configuration files, or debug output. It is set at construction time and remains immutable for the lifetime of the instance.

- **Type:** `string`
- **Access:** Read-only
- **Returns:** A non-null, possibly empty string identifying the scheme.

### `public VisualizerTheme Theme`

Gets the `VisualizerTheme` object that contains the concrete drawing resources (brushes, pens, color stops) for this scheme. The theme is applied by visualizer renderers when painting frames.

- **Type:** `VisualizerTheme`
- **Access:** Read-only
- **Returns:** A non-null `VisualizerTheme` instance.
- **Exceptions:** None from the property accessor itself; however, accessing members of the returned theme may throw if the theme is in an invalid state (implementation-dependent).

### `public static ColorScheme Dark`

A static, read-only `ColorScheme` instance providing a dark-themed visualizer style. Backgrounds are typically black or near-black, with waveform and spectrum elements rendered in high-contrast lighter tones.

- **Type:** `ColorScheme`
- **Access:** Static read-only field
- **Returns:** A fully initialized `ColorScheme` with `Name` set to a value such as `"Dark"` and a corresponding `VisualizerTheme`.
- **Thread safety:** Safe to read from any thread after static initialization completes.

### `public static ColorScheme Light`

A static, read-only `ColorScheme` instance providing a light-themed visualizer style. Backgrounds are white or light gray, with foreground elements in darker shades for clear visibility.

- **Type:** `ColorScheme`
- **Access:** Static read-only field
- **Returns:** A fully initialized `ColorScheme` with `Name` set to a value such as `"Light"` and a corresponding `VisualizerTheme`.
- **Thread safety:** Safe to read from any thread after static initialization completes.

### `public static ColorScheme Neon`

A static, read-only `ColorScheme` instance providing a high-saturation neon style. Typically features bright, fluorescent colors on a dark or black background, suitable for energetic or futuristic visualizations.

- **Type:** `ColorScheme`
- **Access:** Static read-only field
- **Returns:** A fully initialized `ColorScheme` with `Name` set to a value such as `"Neon"` and a corresponding `VisualizerTheme`.
- **Thread safety:** Safe to read from any thread after static initialization completes.

### `public static ColorScheme Grayscale`

A static, read-only `ColorScheme` instance providing a monochrome visualizer style. All elements are rendered in shades of gray, ranging from black to white, without any color hue.

- **Type:** `ColorScheme`
- **Access:** Static read-only field
- **Returns:** A fully initialized `ColorScheme` with `Name` set to a value such as `"Grayscale"` and a corresponding `VisualizerTheme`.
- **Thread safety:** Safe to read from any thread after static initialization completes.

## Usage

### Example 1: Applying a Predefined Scheme to a Visualizer

```csharp
using NAudioVisualizer;

public class VisualizerPanel
{
    private SpectrumRenderer _renderer;

    public void InitializeWithDarkTheme()
    {
        // Select the built-in dark color scheme
        ColorScheme scheme = ColorScheme.Dark;

        // Configure the renderer to use the scheme's theme
        _renderer = new SpectrumRenderer();
        _renderer.ApplyTheme(scheme.Theme);

        Console.WriteLine($"Applied scheme: {scheme.Name}");
    }
}
```

### Example 2: Building a Scheme Selection UI

```csharp
using NAudioVisualizer;
using System.Collections.Generic;

public class ThemeSelector
{
    public List<ColorScheme> AvailableSchemes { get; }

    public ThemeSelector()
    {
        // Populate a list with all predefined schemes
        AvailableSchemes = new List<ColorScheme>
        {
            ColorScheme.Dark,
            ColorScheme.Light,
            ColorScheme.Neon,
            ColorScheme.Grayscale
        };
    }

    public void ApplySchemeByName(string name, SpectrumRenderer renderer)
    {
        ColorScheme selected = AvailableSchemes.Find(s => s.Name == name);

        if (selected != null)
        {
            renderer.ApplyTheme(selected.Theme);
        }
        else
        {
            throw new ArgumentException($"No scheme found with name '{name}'.");
        }
    }
}
```

## Notes

- **Immutability:** `ColorScheme` instances are effectively immutable once constructed. The `Name` and `Theme` properties do not change, making instances safe to share across multiple visualizer components without synchronization.
- **Thread safety:** The static fields (`Dark`, `Light`, `Neon`, `Grayscale`) are initialized during the type’s static constructor and remain constant thereafter. Reading these fields from any thread is safe without locking. Custom `ColorScheme` instances should be fully initialized before being published to multiple threads.
- **Null handling:** The `Name` property is guaranteed to return a non-null string, but it may be empty if a custom scheme is created without a name. The `Theme` property never returns null; a valid `VisualizerTheme` must be supplied at construction.
- **Extensibility:** The predefined static schemes cover common use cases. Applications requiring custom branding can instantiate additional `ColorScheme` objects with bespoke `VisualizerTheme` definitions and manage them alongside the built-in ones.
- **Resource lifetime:** The `VisualizerTheme` returned by `Theme` may hold disposable drawing resources (brushes, pens). Ownership and disposal of these resources are managed by the theme itself or the renderer that consumes it; `ColorScheme` does not expose disposal logic directly.
