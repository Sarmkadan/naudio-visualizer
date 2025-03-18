# Logger

The `Logger` class provides a simple, level-based logging mechanism for the `naudio-visualizer` project. It allows code to emit messages at different severity levels and to control which messages are actually output by setting a minimum threshold. The class implements `IDisposable` to release any underlying resources (e.g., file handles, network connections) when the logger is no longer needed.

## API

### `public LogLevel MinimumLevel { get; set; }`

Gets or sets the minimum log level that will be processed. Messages with a level lower than this value are silently discarded. The `LogLevel` enumeration typically defines values such as `Debug`, `Info`, `Warn`, `Error`, and `Critical`, ordered by increasing severity.

- **Type:** `LogLevel`
- **Getter:** Returns the current minimum level.
- **Setter:** Sets the minimum level. Throws no documented exceptions.

### `public Logger()`

Initializes a new instance of the `Logger` class with default settings. The default `MinimumLevel` is implementation-defined (commonly `Info`).

- **Parameters:** None.
- **Return value:** None (constructor).
- **Throws:** No documented exceptions.

### `public void Debug()`

Logs a message at the `Debug` level. The message content is determined by the implementation (e.g., a preconfigured message or the current state of the application). This method is a no‑op if `MinimumLevel` is higher than `Debug`.

- **Parameters:** None.
- **Return value:** `void`.
- **Throws:** No documented exceptions.

### `public void Info()`

Logs a message at the `Info` level. The message content is implementation‑defined. This method is a no‑op if `MinimumLevel` is higher than `Info`.

- **Parameters:** None.
- **Return value:** `void`.
- **Throws:** No documented exceptions.

### `public void Warn()`

Logs a message at the `Warn` level. The message content is implementation‑defined. This method is a no‑op if `MinimumLevel` is higher than `Warn`.

- **Parameters:** None.
- **Return value:** `void`.
- **Throws:** No documented exceptions.

### `public void Error()`

Logs a message at the `Error` level. The message content is implementation‑defined. This method is a no‑op if `MinimumLevel` is higher than `Error`.

- **Parameters:** None.
- **Return value:** `void`.
- **Throws:** No documented exceptions.

### `public void Critical()`

Logs a message at the `Critical` level. The message content is implementation‑defined. This method is a no‑op if `MinimumLevel` is higher than `Critical`.

- **Parameters:** None.
- **Return value:** `void`.
- **Throws:** No documented exceptions.

### `public void Dispose()`

Releases all resources held by the `Logger` instance (e.g., open file streams, network connections). After calling `Dispose`, the logger should not be used for further logging; behaviour is undefined if any logging method is called after disposal.

- **Parameters:** None.
- **Return value:** `void`.
- **Throws:** No documented exceptions.

## Usage

### Example 1: Basic logging with minimum level control

```csharp
using var logger = new Logger();
logger.MinimumLevel = LogLevel.Warn;

// These calls are silently ignored because their level is below Warn.
logger.Debug();
logger.Info();

// These calls produce output (implementation‑specific).
logger.Warn();
logger.Error();
logger.Critical();
```

### Example 2: Using the logger in a short‑lived scope

```csharp
void ProcessAudioData()
{
    using var logger = new Logger();
    logger.MinimumLevel = LogLevel.Info;

    logger.Info(); // e.g., "Processing started"
    // ... perform work ...
    logger.Warn(); // e.g., "Buffer underrun detected"
    // ... more work ...
    logger.Info(); // e.g., "Processing finished"
}
```

## Notes

- **Message content:** The `Debug`, `Info`, `Warn`, `Error`, and `Critical` methods accept no parameters. The actual message that gets logged is determined by the concrete implementation of the `Logger` class (e.g., it may log a fixed string, the current timestamp, or state captured at construction time). Consumers should not rely on a specific message format unless documented elsewhere.
- **MinimumLevel edge case:** If `MinimumLevel` is set to a value that does not exist in the `LogLevel` enumeration (e.g., via reflection or invalid cast), behaviour is undefined. The property should only be set to a valid `LogLevel` member.
- **Disposal:** After `Dispose()` is called, the `Logger` instance is in an invalid state. Calling any logging method (`Debug`, `Info`, etc.) after disposal may result in undefined behaviour (e.g., exceptions, silent failure, or resource leaks). Always use the `using` statement or explicit `try`/`finally` blocks to ensure disposal.
- **Thread safety:** This class is **not thread‑safe**. If multiple threads access the same `Logger` instance concurrently (including reading or writing `MinimumLevel`), external synchronization (e.g., a `lock` statement) must be used to prevent race conditions and inconsistent state.
