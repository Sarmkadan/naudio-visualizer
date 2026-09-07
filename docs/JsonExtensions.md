# JSON Extensions

The `*JsonExtensions` classes provide `System.Text.Json` helpers for domain models, events, services, configuration, and utility metadata. Most classes expose the same three-operation shape: `ToJson` serializes a value or state snapshot, `FromJson` reads JSON, and `TryFromJson` converts expected parse failures into a Boolean result. The exact contract differs by class, so callers should not assume that every `TryFromJson` method has identical null, whitespace, or failure behavior.

All serializers use camel-case property names. Passing `true` to a `ToJson` method's optional `indented` parameter creates formatted JSON; the default is compact JSON.

## API

### AudioBufferJsonExtensions

Namespace: `NAudioVisualizer.Domain.Models`

*   **`public static string ToJson(this AudioBuffer value, bool indented = false)`**
    Serializes an `AudioBuffer`. Null values throw `ArgumentNullException`. Its options include fields, ignore null properties, and accept property names case-insensitively during deserialization.

*   **`public static AudioBuffer? FromJson(string json)`**
    Rejects null, empty, or whitespace input with `ArgumentException`. Malformed JSON is caught and returned as `null`.

*   **`public static bool TryFromJson(string json, out AudioBuffer? value)`**
    Rejects null, empty, or whitespace input with `ArgumentException`. It returns `false` for a caught `JsonException`; otherwise it returns `true`, including when the serializer produces `null`.

### VisualizationDataJsonExtensions

Namespace: `NAudioVisualizer.Domain.Models`

*   **`public static string ToJson(this VisualizationData value, bool indented = false)`**
    Serializes visualization data while ignoring null properties and reference cycles. A null value throws `ArgumentNullException`.

*   **`public static VisualizationData? FromJson(string json)`**
    Null input throws `ArgumentNullException`, while empty or whitespace input returns `null`. Deserialization errors are not caught and therefore propagate as `JsonException`.

*   **`public static bool TryFromJson(string json, out VisualizationData? value)`**
    Returns `false` for null, empty, whitespace, or malformed JSON. Otherwise it returns `true` after deserialization, even if the resulting value is `null`.

### AudioCaptureStartedEventJsonExtensions

Namespace: `NAudioVisualizer.Events`

*   **`public static string ToJson(this AudioCaptureStartedEvent value, bool indented = false)`**
    Serializes an audio-capture start event. A null value throws `ArgumentNullException`.

*   **`public static AudioCaptureStartedEvent? FromJson(string json)`**
    Null input throws `ArgumentNullException`; empty, whitespace, and malformed JSON return `null`.

*   **`public static bool TryFromJson(string json, out AudioCaptureStartedEvent? value)`**
    Null input throws `ArgumentNullException`. Empty, whitespace, and malformed JSON return `false`; a completed deserialization returns `true`, including when its value is `null`.

### EventPublisherJsonExtensions

Namespace: `NAudioVisualizer.Events`

This class serializes a snapshot of the singleton publisher rather than delegates. The snapshot contains subscriber counts for the 16 event types enumerated by the implementation and a `totalSubscribers` sum.

*   **`public static string ToJson(bool indented = false)`**
    Serializes subscriber-count state from `EventPublisher.Instance`. This is a regular static method, not an extension method.

*   **`public static object? FromJson(string json)`**
    Null input throws `ArgumentNullException`. When JSON deserializes to a non-null state, the method calls `EventPublisher.Reset()`; subscriber delegates are not restored. It returns `EventPublisher.Instance`, or `null` after catching malformed JSON.

*   **`public static bool TryFromJson(string json, out object? value)`**
    Null input throws `ArgumentNullException`. The method returns `true` after `FromJson` returns, including when `FromJson` caught malformed JSON and returned `null`.

### ServiceContainerJsonExtensions

Namespace: `NAudioVisualizer.Configuration`

The serialized state contains the full names (or simple names when no full name exists) of types registered in the container's service and factory dictionaries. It does not serialize service instances or factory delegates.

*   **`public static string ToJson(this ServiceContainer value, bool indented = false)`**
    Serializes the registered service and factory type-name lists. A null container throws `ArgumentNullException`. Null properties and reference cycles are ignored, and enums are written as camel-case strings.

*   **`public static ServiceContainer? FromJson(string json)`**
    Null, empty, or whitespace input throws `ArgumentNullException`. Valid non-null state creates a new, empty `ServiceContainer`; registrations and factories are not restored. A JSON `null` returns `null`, while malformed JSON propagates `JsonException`.

*   **`public static bool TryFromJson(string json, out ServiceContainer? value)`**
    Catches every exception and returns `false` with a null output on failure. Otherwise it returns `true`, including when a JSON `null` produces a null output.

### MidiInputServiceJsonExtensions

Namespace: `NAudioVisualizer.Services`

The JSON snapshot contains only `isDisposed` and `activeDeviceIndex`. Disposable resources and event handlers are not represented.

*   **`public static string ToJson(this MidiInputService value, bool indented = false)`**
    Serializes the two state values. A null service throws `ArgumentNullException`.

*   **`public static MidiInputService? FromJson(string json)`**
    Null input throws `ArgumentNullException`, and empty or whitespace input throws `ArgumentException`. For any other string—including malformed JSON—the method does not parse the content; it creates a new `MidiInputService` and returns `null` only if construction throws.

*   **`public static bool TryFromJson(string json, out MidiInputService? value)`**
    Null input throws `ArgumentNullException`. It delegates to `FromJson`, returning `true` only for a non-null service and returning `false` for caught failures such as empty or whitespace input.

### MathUtilityJsonExtensions

Namespace: `NAudioVisualizer.Utilities`

Because `MathUtility` is static, its JSON is descriptive metadata rather than object state. The generated object has `type` set to `"MathUtility"` and a `methods` array listing the 15 math methods and their signatures.

*   **`public static string ToJson(bool indented = false)`**
    Serializes the fixed metadata object. This is a regular static method, not an extension method.

*   **`public static string? FromJson(string json)`**
    Null input throws `ArgumentNullException`. Empty, whitespace, the exact text `null`, and malformed JSON return `null`. Any other syntactically valid JSON returns the marker string `"MathUtility"`; the JSON shape is not checked against the generated metadata.

*   **`public static bool TryFromJson(string json, out string? value)`**
    Null input throws `ArgumentNullException`. Empty, whitespace, the exact text `null`, and malformed JSON return `false`; any other valid JSON returns `true` and outputs `"MathUtility"`.

### AudioDataConverterJsonExtensions

Namespace: `NAudioVisualizer.Infrastructure`

`AudioDataConverter` is static, so these methods are generic helpers for reference-type audio data rather than serialized converter state.

*   **`public static string ToJson<T>(this T value, bool indented = false) where T : notnull`**
    Serializes any non-null value. A runtime null value throws `ArgumentNullException`.

*   **`public static T? FromJson<T>(string? json) where T : class`**
    Returns `null` for null, empty, or whitespace input. Malformed JSON is not caught and propagates `JsonException`.

*   **`public static bool TryFromJson<T>(string? json, out T? value) where T : class`**
    Returns `false` for null, empty, whitespace, malformed JSON, or a deserialized null value. It returns `true` only when deserialization produces a non-null `T`.

## Usage

The following example serializes an event, safely reads it back, and requests readable output. The declaring class is used explicitly to make the selected JSON contract clear when several extension namespaces are in scope.

```csharp
using NAudioVisualizer.Events;

var started = new AudioCaptureStartedEvent
{
    DeviceId = 0,
    SampleRate = 48_000,
    ChannelCount = 2,
    StartTime = DateTime.UtcNow
};

string json = AudioCaptureStartedEventJsonExtensions.ToJson(started, indented: true);

if (AudioCaptureStartedEventJsonExtensions.TryFromJson(json, out var restored))
{
    Console.WriteLine($"Restored device: {restored?.DeviceId}");
}
```

## Notes

*   **Failure contracts differ**: Some `FromJson` methods return `null` for malformed JSON, while others propagate `JsonException` or do not parse the input. Review the class-specific contract before substituting one helper for another.
*   **A true result may still contain null**: `AudioBuffer`, `VisualizationData`, `AudioCaptureStartedEvent`, `EventPublisher`, and `ServiceContainer` can return `true` from `TryFromJson` with a null output in specific cases described above. `AudioDataConverterJsonExtensions.TryFromJson<T>` explicitly requires a non-null result.
*   **Runtime resources are snapshots only**: Event subscriptions, service registrations, factory delegates, MIDI resources, and event handlers are not reconstructed from JSON.
*   **Extension visibility**: `EventPublisherJsonExtensions.ToJson` and `MathUtilityJsonExtensions.ToJson` are static methods because their targets are static or singleton-oriented. The remaining `ToJson` methods use extension-method syntax.
