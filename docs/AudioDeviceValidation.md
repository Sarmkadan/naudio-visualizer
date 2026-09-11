# AudioDeviceValidation

## Overview
The `AudioDeviceValidation` class provides a set of static extension methods for validating `AudioDevice` instances. It checks for data integrity, range constraints, and consistency across related properties, returning detailed error messages when validation fails.

## Methods

### `Validate(AudioDevice value) → IReadOnlyList<string>`
Validates an `AudioDevice` instance and returns a list of human-readable problem descriptions. If the device is valid, an empty list is returned.

**Parameters:**
- `value`: The `AudioDevice` to validate.

**Returns:**
A read-only list of strings describing validation issues.

**Exceptions:**
- `ArgumentNullException`: Thrown if `value` is `null`.

---

### `IsValid(AudioDevice value) → bool`
Determines whether an `AudioDevice` is valid by checking if `Validate` returns an empty list.

**Parameters:**
- `value`: The `AudioDevice` to check.

**Returns:**
`true` if the device passes all validation rules; otherwise, `false`.

**Exceptions:**
- `ArgumentNullException`: Thrown if `value` is `null`.

---

### `EnsureValid(AudioDevice value) → void`
Ensures that an `AudioDevice` is valid. If validation fails, it throws an `ArgumentException` containing a summary of all validation problems.

**Parameters:**
- `value`: The `AudioDevice` to validate.

**Exceptions:**
- `ArgumentNullException`: Thrown if `value` is `null`.
- `ArgumentException`: Thrown if the device is invalid, containing details of the failures.

## Validation Rules
The following rules are enforced during validation:

| Property | Rule |
|----------|------|
| `Id` | Must not be `Guid.Empty`. |
| `Name` | Cannot be null or whitespace. |
| `DeviceIndex` | Cannot be negative. |
| `Manufacturer` | Cannot be null or whitespace. |
| `ChannelCount` | Must be greater than 0. |
| `SupportedSampleRates` | Cannot be null; must contain at least one sample rate; all sample rates must be positive. |
| `DefaultSampleRate` | Must be greater than 0. |
| `BitDepth` | Must be greater than 0. |
| `LastStatusCheck` | Must be set to a valid DateTime (not default). |
| `Capabilities` | Cannot be null. |