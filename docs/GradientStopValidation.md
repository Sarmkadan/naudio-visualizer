# GradientStopValidation

## Overview
The `GradientStopValidation` class provides a set of static extension methods for validating `GradientStop` instances. It checks for data integrity, range constraints, and consistency across related properties, returning detailed error messages when validation fails.

## Methods

### `Validate(GradientStop value) → IReadOnlyList<string>`
Validates a `GradientStop` instance and returns a list of human-readable problem descriptions. If the gradient stop is valid, an empty list is returned.

**Parameters:**
- `value`: The `GradientStop` to validate.

**Returns:**
A read-only list of strings describing validation issues.

**Exceptions:**
- `ArgumentNullException`: Thrown if `value` is `null`.

---

### `IsValid(GradientStop value) → bool`
Determines whether a `GradientStop` is valid by checking if `Validate` returns an empty list.

**Parameters:**
- `value`: The `GradientStop` to check.

**Returns:**
`true` if the gradient stop passes all validation rules; otherwise, `false`.

**Exceptions:**
- `ArgumentNullException`: Thrown if `value` is `null`.

---

### `EnsureValid(GradientStop value) → void`
Ensures that a `GradientStop` is valid. If validation fails, it throws an `ArgumentException` containing a summary of all validation problems.

**Parameters:**
- `value`: The `GradientStop` to validate.

**Exceptions:**
- `ArgumentNullException`: Thrown if `value` is `null`.
- `ArgumentException`: Thrown if the gradient stop is invalid, containing details of the failures.

## Validation Rules
The following rules are enforced during validation:

| Property | Rule |
|----------|------|
| `Position` | Must be a valid number (not NaN or infinity). Must be in the range [0, 1]. |
| `Color` | If the color value is 0 (transparent black), the position should be 0 (for gradient start). This is a heuristic, not a strict rule. |