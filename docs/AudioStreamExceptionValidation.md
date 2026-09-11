# AudioStreamExceptionValidation

`AudioStreamExceptionValidation` is a static validation harness for the `AudioStreamException` class. It provides extension methods to validate instances of `AudioStreamException` and report validation problems. The type is intended for diagnostic and self‑test scenarios, not for production exception handling.

## API

### `public static IReadOnlyList<string> Validate(this AudioStreamException value)`
Validates an `AudioStreamException` instance and returns a list of validation problems.
- **Parameters**
  - `value`: The exception to validate.
- **Returns**
  - A list of human-readable validation problems; empty if valid.
- **Exceptions**
  - `ArgumentNullException`: Thrown when `value` is null.

### `public static bool IsValid(this AudioStreamException value)`
Determines whether an `AudioStreamException` instance is valid.
- **Parameters**
  - `value`: The exception to check.
- **Returns**
  - True if the exception is valid; otherwise, false.

### `public static void EnsureValid(this AudioStreamException value)`
Ensures that an `AudioStreamException` instance is valid, throwing an `ArgumentException` if not.
- **Parameters**
  - `value`: The exception to validate.
- **Exceptions**
  - `ArgumentNullException`: Thrown when `value` is null.
  - `ArgumentException`: Thrown when the exception is invalid, containing a list of validation problems.

## Usage

```csharp
// Validate an exception and get problems
AudioStreamException ex = new AudioStreamException(...);
IReadOnlyList<string> problems = ex.Validate();
if (problems.Count > 0)
{
    foreach (string problem in problems)
    {
        Console.WriteLine(problem);
    }
}

// Check validity
bool isValid = ex.IsValid();

// Throw if invalid (guard clause)
ex.EnsureValid(); // throws ArgumentException if invalid
```

## Notes

- The `Validate` method returns the same list instance on every call for a given instance; however, the list is not cached across different instances.
- `IsValid` and `EnsureValid` are based on the result of `Validate`; they do not perform additional validation.
- The validation checks:
  - `ErrorCode` must be a defined `AudioStreamErrorCode` enum value.
  - `Message` (inherited from `Exception`) cannot be null or whitespace.
  - If `InnerException` is present, its `Message` cannot be null or whitespace.
- These validation helpers are useful for debugging and testing scenarios where you want to ensure exception objects are correctly populated before logging or re‑throwing.