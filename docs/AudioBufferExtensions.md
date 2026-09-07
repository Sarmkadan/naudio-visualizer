# AudioBufferExtensions

Extension methods for `AudioBuffer` and raw audio buffers (`float[]`) that provide common audio processing functionality including channel operations, normalization, statistics, and JSON serialization.

## Float[] Extensions

These extension methods operate on raw audio sample arrays (`float[]`) and are located in the `NaudioVisualizer.Utilities` namespace.

### PeakDb

```csharp
public static float PeakDb(this float[] buffer)
```

Returns the peak amplitude of the buffer expressed in decibels (dBFS).

**Parameters:**
*   `buffer`: The audio buffer.

**Returns:** Peak level in dB. Returns `float.NegativeInfinity` for empty buffers.

**Example:**
```csharp
float[] audioBuffer = new float[] { 0.5f, -0.8f, 0.3f };
float peakDb = audioBuffer.PeakDb(); // Returns approximately -1.94 dB
```

### RmsDb

```csharp
public static float RmsDb(this float[] buffer)
```

Returns the RMS (root‑mean‑square) level of the buffer expressed in decibels (dBFS).

**Parameters:**
*   `buffer`: The audio buffer.

**Returns:** RMS level in dB. Returns `float.NegativeInfinity` for empty buffers.

**Example:**
```csharp
float[] audioBuffer = new float[] { 0.5f, -0.5f, 0.5f, -0.5f };
float rmsDb = audioBuffer.RmsDb(); // Returns approximately -6.02 dB
```

### NormalizeInPlace

```csharp
public static void NormalizeInPlace(this float[] buffer, float targetPeak)
```

Normalises the buffer in‑place so that its peak amplitude matches the specified target.

**Parameters:**
*   `buffer`: The audio buffer to normalise.
*   `targetPeak`: The desired peak amplitude (linear, not dB). Must be positive.

**Exceptions:**
*   `ArgumentNullException` if `buffer` is null.
*   `ArgumentOutOfRangeException` if `targetPeak` is not positive.

**Example:**
```csharp
float[] audioBuffer = new float[] { 0.2f, -0.4f, 0.1f };
audioBuffer.NormalizeInPlace(0.8f); // Buffer now contains [0.4f, -0.8f, 0.2f]
```

## AudioBuffer Extension Methods

These extension methods operate on `AudioBuffer` instances and are located in the `NAudioVisualizer.Domain.Models` namespace.

### CopyToChannel

```csharp
public static void CopyToChannel(
    this AudioBuffer buffer,
    float[] target,
    int targetChannel,
    int sourceChannel)
```

Copies samples from one channel to another in the audio buffer. The audio buffer stores samples in interleaved format: [ch0_sample0, ch1_sample0, ch0_sample1, ch1_sample1, ...].

**Parameters:**
*   `buffer`: The audio buffer containing interleaved audio data.
*   `target`: The target array to copy samples to. Must be large enough to hold the copied channel data.
*   `targetChannel`: The target channel index (0-based).
*   `sourceChannel`: The source channel index (0-based).

**Exceptions:**
*   `ArgumentNullException` if `buffer` or `target` is null.
*   `ArgumentOutOfRangeException` if channel indices are invalid or `target` is too small.

**Example:**
```csharp
// Copy left channel (0) to right channel (1) in a stereo buffer
float[] monoSamples = new float[buffer.Count];
buffer.CopyToChannel(monoSamples, 1, 0); // Copies channel 0 to channel 1
```

### ToNormalizedArray

```csharp
public static float[] ToNormalizedArray(this AudioBuffer buffer)
```

Converts the audio buffer to a normalized float array (values in range [-1, 1]).

**Parameters:**
*   `buffer`: The audio buffer.

**Returns:** An array containing normalized audio samples.

**Exceptions:**
*   `ArgumentNullException` if `buffer` is null.

**Example:**
```csharp
float[] normalized = buffer.ToNormalizedArray();
// All values in normalized array are now in range [-1, 1]
```

### CloneEmpty

```csharp
public static AudioBuffer CloneEmpty(this AudioBuffer buffer)
```

Creates a new `AudioBuffer` with the same configuration but empty content.

**Parameters:**
*   `buffer`: The audio buffer to clone.

**Returns:** A new empty buffer with identical capacity, sample rate, and channel count.

**Exceptions:**
*   `ArgumentNullException` if `buffer` is null.

**Example:**
```csharp
AudioBuffer emptyBuffer = buffer.CloneEmpty();
// emptyBuffer has same Capacity, SampleRate, and ChannelCount as buffer
// but Count = 0
```

### GetFillPercentageString

```csharp
public static string GetFillPercentageString(
    this AudioBuffer buffer,
    string format = "P2")
```

Gets the buffer fill percentage as a string formatted for display.

**Parameters:**
*   `buffer`: The audio buffer.
*   `format`: The format string for the percentage (default: "P2").

**Returns:** A formatted string representing the fill percentage.

**Exceptions:**
*   `ArgumentNullException` if `buffer` is null.

**Example:**
```csharp
string fillText = buffer.GetFillPercentageString(); // Returns something like "75.00%"
string fillTextP0 = buffer.GetFillPercentageString("P0"); // Returns something like "75%"
```

## AudioBuffer JSON Extensions

These extension methods provide JSON serialization capabilities for `AudioBuffer` and are located in the `NAudioVisualizer.Domain.Models` namespace.

### ToJson

```csharp
public static string ToJson(this AudioBuffer value, bool indented = false)
```

Converts an `AudioBuffer` to a JSON string.

**Parameters:**
*   `value`: The `AudioBuffer` to serialize.
*   `indented`: Whether to format the JSON with indentation.

**Returns:** A JSON string representation of the `AudioBuffer`.

**Exceptions:**
*   `ArgumentNullException` if `value` is null.

**Example:**
```csharp
string json = buffer.ToJson(); // Compact JSON
string prettyJson = buffer.ToJson(true); // Indented JSON
```

### FromJson

```csharp
public static AudioBuffer? FromJson(string json)
```

Deserializes an `AudioBuffer` from a JSON string.

**Parameters:**
*   `json`: The JSON string to deserialize.

**Returns:** The deserialized `AudioBuffer`, or null if deserialization fails.

**Exceptions:**
*   `ArgumentNullException` if `json` is null.

**Example:**
```csharp
string json = buffer.ToJson();
AudioBuffer? restored = AudioBufferJsonExtensions.FromJson(json);
// restored contains the same data as buffer
```

### TryFromJson

```csharp
public static bool TryFromJson(string json, out AudioBuffer? value)
```

Attempts to deserialize an `AudioBuffer` from a JSON string.

**Parameters:**
*   `json`: The JSON string to deserialize.
*   `value`: Output parameter containing the deserialized `AudioBuffer`, or null if deserialization fails.

**Returns:** True if deserialization succeeded; otherwise, false.

**Exceptions:**
*   `ArgumentNullException` if `json` is null.

**Example:**
```csharp
string json = buffer.ToJson();
if (AudioBufferJsonExtensions.TryFromJson(json, out AudioBuffer? result))
{
    // result contains the deserialized AudioBuffer
}
else
{
    // Handle deserialization failure
}
```