# ConfigurationManagerValidation

## Overview
The `ConfigurationManagerValidation` class provides a set of static extension methods for validating `ConfigurationManager` instances. It checks configuration keys and values against defined bounds and constraints, returning detailed error messages when validation fails.

## Methods

### `Validate(ConfigurationManager value) → IReadOnlyList<string>`
Validates a `ConfigurationManager` instance and returns a list of human-readable problem descriptions. If the configuration is valid, an empty list is returned.

**Parameters:**
- `value`: The `ConfigurationManager` to validate.

**Returns:**
A read-only list of strings describing validation issues.

**Exceptions:**
- `ArgumentNullException`: Thrown if `value` is `null`.

---

### `IsValid(ConfigurationManager value) → bool`
Determines whether a `ConfigurationManager` is valid by checking if `Validate` returns an empty list.

**Parameters:**
- `value`: The `ConfigurationManager` to check.

**Returns:**
`true` if the configuration passes all validation rules; otherwise, `false`.

**Exceptions:**
- `ArgumentNullException`: Thrown if `value` is `null`.

---

### `EnsureValid(ConfigurationManager value) → void`
Ensures that a `ConfigurationManager` is valid. If validation fails, it throws an `ArgumentException` containing a summary of all validation problems.

**Parameters:**
- `value`: The `ConfigurationManager` to validate.

**Exceptions:**
- `ArgumentNullException`: Thrown if `value` is `null`.
- `ArgumentException`: Thrown if the configuration is invalid, containing details of the failures.

## Validation Rules
The following rules are enforced during validation:

| Setting Key | Rule |
|-------------|------|
| `audio.sampleRate` | Must be in the range [8000, 96000] Hz. |
| `audio.channelCount` | Must be in the range [1, 8]. |
| `audio.bitDepth` | Must be in the range [8, 32] bits. |
| `audio.fftSize` | Must be in the range [64, 16384]. |
| `visualization.targetFps` | Must be in the range [1, 240] FPS. |
| `visualization.brightness` | Must be in the range [0.0, 2.0]. |
| `visualization.contrast` | Must be in the range [0.0, 2.0]. |
| `display.width` | Must be in the range [320, 7680] pixels. |
| `display.height` | Must be in the range [240, 4320] pixels. |
| `display.fullscreen` | Must be a valid boolean value. |
| `export.compress` | Must be a valid boolean value. |
| `export.includeMetadata` | Must be a valid boolean value. |
| `logging.writeToConsole` | Must be a valid boolean value. |
| `logging.writeToFile` | Must be a valid boolean value. |
| `export.defaultFormat` | Must be one of: "json", "xml", "yaml", "csv". |
| `logging.level` | Must be one of: "Debug", "Info", "Warn", "Error". |

Additionally, all configuration keys must be non-null, non-empty, and non-whitespace strings.