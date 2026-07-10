# VisualizationSettings

The `VisualizationSettings` class provides a comprehensive configuration structure for customizing audio visualizations within the `naudio-visualizer` library. It encapsulates settings related to rendering quality, visual aesthetics, and specific parameters for different visualization types, such as waveforms, frequency spectrums, and spectrograms.

## API

*   **`VisualizerTheme Theme`**: Gets or sets the visual theme used for rendering, defining the overall color palette and stylistic elements.
*   **`Guid Id`**: A unique identifier for this specific instance of `VisualizationSettings`, useful for tracking and serialization purposes.
*   **`WaveformRenderingSettings WaveformSettings`**: Configuration object containing settings specifically tailored for waveform visualization modes.
*   **`SpectrumRenderingSettings SpectrumSettings`**: Configuration object containing settings specifically tailored for frequency spectrum visualization modes.
*   **`SpectrogramRenderingSettings SpectrogramSettings`**: Configuration object containing settings specifically tailored for spectrogram visualization modes.
*   **`int RenderingQuality`**: Specifies the quality level for rendering operations. Higher values generally result in improved visual fidelity but may increase CPU or GPU overhead.
*   **`int TargetFPS`**: Sets the desired frames per second for the visualization rendering loop.
*   **`bool EnableAntiAliasing`**: Toggles anti-aliasing to smooth out lines and edges during rendering.
*   **`uint BackgroundColor`**: The background color of the visualization area, represented as a 32-bit unsigned integer (typically an ARGB hex value).
*   **`bool ShowGrid`**: Toggles the visibility of the background grid lines.
*   **`bool ShowFrequencyLabels`**: Toggles the visibility of labels indicating frequency values on the axes.
*   **`bool ShowTimeLabels`**: Toggles the visibility of labels indicating time markers on the axes.
*   **`float TimeScale`**: A scaling factor applied to the time axis, effectively acting as a zoom level for time-domain visualizations.
*   **`float MaxFrequencyDisplay`**: The maximum frequency limit to be displayed in spectrum or spectrogram views.
*   **`bool IsValid`**: A read-only indicator that returns true if the current configuration is valid and ready for use.
*   **`uint LineColor`**: The primary color used for rendering lines, represented as a 32-bit unsigned integer (typically an ARGB hex value).
*   **`float LineThickness`**: The width of the lines drawn in the visualization.
*   **`bool ShowStereoSeparate`**: If enabled, forces the visualizer to render left and right audio channels separately.
*   **`float AmplitudeZoom`**: A multiplier applied to the audio amplitude, allowing for scaling of the visualization height.
*   **`int DownsamplingFactor`**: A factor used to reduce the amount of data processed per frame, primarily used for performance optimization when dealing with high-density audio streams.

## Usage

### Creating Default Settings

```csharp
var settings = new VisualizationSettings
{
    Id = Guid.NewGuid(),
    Theme = VisualizerTheme.Dark,
    TargetFPS = 60,
    BackgroundColor = 0xFF000000, // Black
    LineColor = 0xFF00FF00        // Green
};
```

### Applying Custom Configuration

```csharp
VisualizationSettings mySettings = LoadSettingsFromFile("config.json");

if (mySettings.IsValid)
{
    mySettings.AmplitudeZoom = 1.5f;
    mySettings.EnableAntiAliasing = true;
    mySettings.ShowGrid = false;
    
    // Apply settings to the active visualizer instance
    visualizer.UpdateSettings(mySettings);
}
```

## Notes

*   **Thread Safety**: The `VisualizationSettings` class is not inherently thread-safe. When modifying settings on a background thread while the rendering loop is running on another, it is recommended to implement external locking or replace the settings object entirely using a thread-safe reference swap.
*   **Performance**: Extremely high values for `RenderingQuality` combined with low `DownsamplingFactor` can significantly impact the performance of the rendering engine.
*   **Validation**: Always verify the `IsValid` property before attempting to apply a settings object to a visualizer component, especially after deserializing settings from external sources like files or network streams.
*   **Color Representation**: The `uint` properties (`BackgroundColor`, `LineColor`) expect color data in ARGB format (Alpha, Red, Green, Blue). Ensure input values are correctly formatted (e.g., `0xAARRGGBB`) to avoid unexpected color rendering.
