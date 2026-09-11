# SpectrogramAnalyzerValidation

## Overview
The `SpectrogramAnalyzerValidation` class provides a set of static extension methods for validating `SpectrogramAnalyzer` instances. It validates the internal state of the analyzer via its public API, returning detailed error messages when validation fails.

## Methods

### `Validate(SpectrogramAnalyzer value) → IReadOnlyList<string>`
Validates a `SpectrogramAnalyzer` instance and returns a list of human-readable problem descriptions. If the analyzer is valid, an empty list is returned.

**Parameters:**
- `value`: The `SpectrogramAnalyzer` to validate.

**Returns:**
A read-only list of strings describing validation issues.

**Exceptions:**
- `ArgumentNullException`: Thrown if `value` is `null`.

---

### `IsValid(SpectrogramAnalyzer value) → bool`
Determines whether a `SpectrogramAnalyzer` is valid by checking if `Validate` returns an empty list.

**Parameters:**
- `value`: The `SpectrogramAnalyzer` to check.

**Returns:**
`true` if the analyzer passes all validation rules; otherwise, `false`.

**Exceptions:**
- `ArgumentNullException`: Thrown if `value` is `null`.

---

### `EnsureValid(SpectrogramAnalyzer value) → void`
Ensures that a `SpectrogramAnalyzer` is valid. If validation fails, it throws an `ArgumentException` containing a summary of all validation problems.

**Parameters:**
- `value`: The `SpectrogramAnalyzer` to validate.

**Exceptions:**
- `ArgumentNullException`: Thrown if `value` is `null`.
- `ArgumentException`: Thrown if the analyzer is invalid, containing details of the failures.

## Validation Rules
The following rules are enforced during validation:

| Rule | Description |
|------|-------------|
| Buffer frame count | Must be non-negative (>= 0). The buffer frame count is obtained via the `GetBufferFrameCount()` method. |