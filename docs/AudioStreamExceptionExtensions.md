# AudioStreamExceptionExtensions

Utility class providing extension-style helpers for `AudioStreamException`, including error classification and user-friendly message generation.

## API

### `public static string GetDetailedErrorMessage(AudioStreamException exception)`

Returns a detailed diagnostic message for the given `AudioStreamException`, including inner exception information and stack traces when available.

- **Parameters**
  - `exception`: The `AudioStreamException` instance to inspect. Must not be `null`.
- **Return value**
  - A string containing the detailed error message, including nested exception details if present.
- **Exceptions**
  - Throws `ArgumentNullException` if `exception` is `null`.

---

### `public static bool IsRecoverable(AudioStreamException exception)`

Determines whether the specified `AudioStreamException` represents a recoverable error condition that may be retried or handled gracefully.

- **Parameters**
  - `exception`: The `AudioStreamException` instance to evaluate. Must not be `null`.
- **Return value**
  - `true` if the error is potentially recoverable (e.g., transient I/O issues); `false` otherwise.
- **Exceptions**
  - Throws `ArgumentNullException` if `exception` is `null`.

---

### `public static bool IsFatal(AudioStreamException exception)`

Determines whether the specified `AudioStreamException` represents a fatal, unrecoverable condition that should halt processing.

- **Parameters**
  - `exception`: The `AudioStreamException` instance to evaluate. Must not be `null`.
- **Return value**
  - `true` if the error is unrecoverable (e.g., unsupported format, corrupt data); `false` otherwise.
- **Exceptions**
  - Throws `ArgumentNullException` if `exception` is `null`.

---
### `public static AudioStreamException WithMessage(AudioStreamException exception, string message)`

Creates a new `AudioStreamException` with the same properties as the original but with an updated error message.

- **Parameters**
  - `exception`: The source `AudioStreamException`. Must not be `null`.
  - `message`: The new error message to assign. Must not be `null`.
- **Return value**
  - A new `AudioStreamException` instance with the updated message.
- **Exceptions**
  - Throws `ArgumentNullException` if either `exception` or `message` is `null`.

---
### `public static string GetUserFriendlyMessage(AudioStreamException exception)`

Generates a concise, user-friendly error message suitable for display in UI or logs, omitting technical details.

- **Parameters**
  - `exception`: The `AudioStreamException` instance to convert. Must not be `null`.
- **Return value**
  - A localized, end-user oriented message describing the error.
- **Exceptions**
  - Throws `ArgumentNullException` if `exception` is `null`.

## Usage
