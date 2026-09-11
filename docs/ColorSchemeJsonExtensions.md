# ColorSchemeJsonExtensions

Provides System.Text.Json serialization extensions for ColorScheme.

## API

### `public static string ToJson(this ColorScheme scheme, bool indented = false)`
Serializes the supplied `ColorScheme` instance to a JSON string.

- **Parameters**
  - `scheme`: The `ColorScheme` to serialize. Must not be `null`.
  - `indented`: Whether to format the JSON with indentation. Default is `false`.
- **Return value**: A JSON‑encoded string representing the color scheme's current state.
- **Exceptions**
  - `ArgumentNullException` if `scheme` is `null`.

## Usage

```csharp
using NAudioVisualizer.Themes; // namespace containing the extensions

var scheme = new ColorScheme();
// ... configure scheme, set theme, name, etc.

// Serialize to JSON for storage or transmission
string json = scheme.ToJson(); // or scheme.ToJson(true) for indented JSON
File.WriteAllText("colorSchemeState.json", json);

// Later, restore the scheme from JSON
string storedJson = File.ReadAllText("colorSchemeState.json");
// Note: There is no FromJson method for ColorScheme as it's designed for serialization only
// If deserialization is needed, use System.Text.Json.JsonSerializer directly with the same options
```

```csharp
// Inspect the scheme's JSON representation
string json = scheme.ToJson();
Console.WriteLine(json);
```

## Notes

- The JSON method operates on an immutable snapshot; it does not alter the source `ColorScheme` instance.
- The static extension method itself is thread‑safe provided it does not rely on mutable shared state; it only reads the supplied arguments and returns a new string.
- The `ToJson` method uses a `JsonSerializerOptions` instance with camelCase naming policy, which is efficient for repeated calls.
- The serialized object includes the color scheme's `Name` and `Theme` properties, with the theme's `BackgroundColor`, `WaveformGradient`, and `SpectrogramPalette` (as defined in the `ColorScheme` and `Theme` classes).