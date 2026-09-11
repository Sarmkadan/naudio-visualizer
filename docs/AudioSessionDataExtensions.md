# AudioSessionDataExtensions

Extension methods for `AudioSessionData` that provide utility functionality for checking session state.

## AudioSessionData Extension Methods

These extension methods operate on `AudioSessionData` instances and are located in the `NAudioVisualizer.Data.Repositories` namespace.

### IsActive

```csharp
public static bool IsActive(this AudioSessionData session)
```

Determines whether the audio session is active.

**Parameters:**
*   `session`: The audio session.

**Returns:** `<see langword="true"/>` when the session has no end time; otherwise, `<see langword="false"/>`.

**Exceptions:**
*   `ArgumentNullException` if `session` is null.

**Example:**
```csharp
if (session.IsActive())
{
    // Session is currently recording
}
```