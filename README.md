// src/README.md
// ... rest of the file content ...
## AudioDataConverter
The `AudioDataConverter` class provides a set of static methods for converting and manipulating audio data. It offers functionality for converting between decibel and linear scales, formatting audio data, and performing operations such as RMS and peak level calculation, normalization, and gain application.

### Usage Example

```csharp
// Initialize audio data
float[] samples = new float[1024];
// ... populate samples array ...

// Calculate RMS level
float rmsLevel = AudioDataConverter.CalculateRmsLevel(samples);

// Normalize samples
float[] normalizedSamples = AudioDataConverter.NormalizeSamples(samples);

// Apply gain
float[] gainedSamples = AudioDataConverter.ApplyGain(normalizedSamples, 2.0f);
```

// ... rest of the file content ...
