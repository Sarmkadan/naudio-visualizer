# VisualizationData

`VisualizationData` is an abstract base class designed to store and manage processed audio visualization data within the `naudio-visualizer` pipeline. It encapsulates the resulting data points along with essential metadata—such as the generation timestamp, the source audio frame, and range statistics—providing a standardized interface for accessing, validating, and normalizing visualization outputs before they are consumed by rendering components or further analytical routines.

## API

### `public Guid Id`
A unique identifier for the specific instance of `VisualizationData`.

### `public VisualizationType VisualizationType`
Gets the `VisualizationType` enum value, specifying the algorithm or method used to generate this visualization data (e.g., FFT, waveform, spectrum).

### `public DateTime GeneratedAt`
The timestamp representing when this data instance was created.

### `public AudioFrame? SourceFrame`
The originating `AudioFrame` that was used to generate this visualization. Returns `null` if the data was not directly derived from a specific frame or if the reference is unavailable.

### `public int DataPointCount`
The total number of data points contained in the underlying data array.

### `public float MinValue`
The minimum value present in the current data set.

### `public float MaxValue`
The maximum value present in the current data set.

### `public bool IsNormalized`
Indicates whether the data set has already been scaled to a normalized range (e.g., 0.0 to 1.0).

### `public abstract float[] GetData()`
Returns the underlying array of floating-point data points representing the visualization.

### `public abstract void Normalize()`
Applies normalization to the internal data set, scaling the values based on the current `MinValue` and `MaxValue`. Updates the `IsNormalized` property to `true` upon completion.

### `public abstract bool IsValid`
Gets a value indicating whether the data set is considered valid. This typically checks that the array is not null, contains finite numerical values (no `NaN` or `Infinity`), and adheres to any structural constraints required by the specific `VisualizationType`.

## Usage

### Accessing and Normalizing Data
```csharp
public void ProcessVisualization(VisualizationData data)
{
    if (data.IsValid && !data.IsNormalized)
    {
        data.Normalize();
    }

    float[] points = data.GetData();
    // Render the normalized points to the screen
}
```

### Inspecting Metadata
```csharp
public void LogVisualizationInfo(VisualizationData data)
{
    Console.WriteLine($"ID: {data.Id}");
    Console.WriteLine($"Type: {data.VisualizationType}");
    Console.WriteLine($"Range: [{data.MinValue}, {data.MaxValue}]");
    Console.WriteLine($"Generated at: {data.GeneratedAt}");
}
```

## Notes

*   **Thread Safety:** Instances of classes inheriting from `VisualizationData` are not guaranteed to be thread-safe. Modifying data via `Normalize()` while another thread is reading data via `GetData()` may lead to inconsistent results or race conditions. Implementations should ensure appropriate synchronization if accessed from multiple threads.
*   **Validation:** The `IsValid` property should be checked before performing calculations or rendering, particularly after receiving data from an external source or before invoking `Normalize()`, as certain processing steps may be undefined for invalid numerical data (e.g., `NaN` or `Infinity`).
*   **Performance:** Depending on the implementation, `GetData()` might return a reference to the internal array or a copy. If the implementation returns a reference, callers must ensure they do not mutate the returned array directly, as this will bypass the class's internal state tracking (e.g., `MinValue`, `MaxValue`, `IsNormalized`).
