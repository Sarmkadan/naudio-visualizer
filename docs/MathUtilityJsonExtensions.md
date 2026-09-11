# MathUtilityJsonExtensions

Provides System.Text.Json serialization extensions for MathUtility.

## API

### `public static string ToJson(bool indented = false)`
Serializes the MathUtility type information to a JSON string.

- **Parameters**
  - `indented`: Whether to format the JSON with indentation. Default is `false`.
- **Return value**: A JSON string representation of the MathUtility type.

### `public static string? FromJson(string json)`
Deserializes a JSON string to validate MathUtility type information.
Since MathUtility is a static class, this validates JSON and returns a success indicator.

- **Parameters**
  - `json`: The JSON string to deserialize.
- **Return value**: A success indicator string if deserialization succeeds; otherwise, `null`.
- **Exceptions**
  - `ArgumentNullException` if `json` is `null`.

### `public static bool TryFromJson(string json, out string? value)`
Attempts to deserialize a JSON string.

- **Parameters**
  - `json`: The JSON string to deserialize.
  - `value`: The resulting value if deserialization succeeds.
- **Return value**: True if JSON is valid; otherwise, false.
- **Exceptions**
  - `ArgumentNullException` if `json` is `null`.

## Usage

```csharp
using NAudioVisualizer.Utilities;

// Serialize MathUtility type information to JSON
string json = MathUtilityJsonExtensions.ToJson();
// For indented JSON
string indentedJson = MathUtilityJsonExtensions.ToJson(true);

// Deserialize and validate
string? result = MathUtilityJsonExtensions.FromJson(json);
if (result != null)
{
    // Deserialization succeeded
}

// Using TryFromJson to avoid exceptions
if (MathUtilityJsonExtensions.TryFromJson(json, out var value))
{
    // Use value
}
```

## Notes

- The JSON methods operate on an immutable snapshot; they do not alter the source object (though MathUtility is static and has no instance state).
- The static methods are thread-safe as they do not rely on mutable shared state; they only read the supplied arguments and return a new string or boolean.
- The `ToJson` method uses a `JsonSerializerOptions` instance with camelCase naming policy, which is efficient for repeated calls.
- The `FromJson` and `TryFromJson` methods return null for classes when the JSON is null, empty, or invalid, and for `TryFromJson` the boolean indicates success.
- Note: The MathUtility class itself is a static utility and cannot be meaningfully serialized; these extensions are intended for serializing type information and validation.