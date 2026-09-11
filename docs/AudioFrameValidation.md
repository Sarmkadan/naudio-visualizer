# AudioFrameValidation

## Overview
The `AudioFrameValidation` class provides a set of static extension methods for validating `AudioFrame` instances. It checks for data integrity, range constraints, and consistency across related properties, returning detailed error messages when validation fails.

## Methods

### `Validate(AudioFrame value) → IReadOnlyList<string>`
Validates an `AudioFrame` instance and returns a list of human-readable problem descriptions. If the frame is valid, an empty list is returned.

**Parameters:**
- `value`: The `AudioFrame` to validate.

**Returns:**
A read-only list of strings describing validation issues.

**Exceptions:**
- `ArgumentNullException`: Thrown if `value` is `null`.

---

### `IsValid(AudioFrame value) → bool`
Determines whether an `AudioFrame` is valid by checking if `Validate` returns an empty list.

**Parameters:**
- `value`: The `AudioFrame` to check.

**Returns:**
`true` if the frame passes all validation rules; otherwise, `false`.

**Exceptions:**
- `ArgumentNullException`: Thrown if `value` is `null`.

---

### `EnsureValid(AudioFrame value) → void`
Ensures that an `AudioFrame` is valid. If validation fails, it throws an `ArgumentException` containing a summary of all validation problems.

**Parameters:**
- `value`: The `AudioFrame` to validate.

**Exceptions:**
- `ArgumentNullException`: Thrown if `value` is `null`.
- `ArgumentException`: Thrown if the frame is invalid, containing details of the failures.

## Validation Rules
The following rules are enforced during validation:

| Property | Rule |
|----------|------|
| `Id` | Must not be `Guid.Empty`. |
| `Samples` | Cannot be `null` or empty. Must not contain `NaN` or infinite values. |
| `ChannelCount` | Must be greater than `0`. `Samples.Length` must be evenly divisible by `ChannelCount`. |
| `SampleRate` | Must be greater than `0`. |
| `Timestamp` | Must not be `default`. Must be in UTC format (`DateTimeKind.Utc`). |
| `FrameIndex` | Must not be negative (`>= 0`). |
| `DurationSeconds` | Must be greater than `0`. |
| `PeakAmplitude` | Must not be `NaN` or infinite. Must be within the range `[-1.0, 1.0]`. |
| `RmsEnergy` | Must not be `NaN` or infinite. Must be non-negative (`>= 0`). |
