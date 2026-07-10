# VstPluginInfo

`VstPluginInfo` encapsulates metadata and capability details for a Virtual Studio Technology (VST) plugin instance loaded within the `naudio-visualizer` framework. This record provides essential identification, versioning, and channel configuration data, acting as a read-only snapshot of the plugin's state. `VstParameter` represents individual, configurable settings exposed by the associated VST plugin.

## API

### VstPluginInfo

Represents the core metadata for a VST plugin instance.

*   **LoadedAt** (`DateTime`): Gets the timestamp indicating when the plugin was loaded.
*   **UniqueId** (`string`): Gets the unique identifier for the plugin, typically based on the VST plugin ID.
*   **SdkVersion** (`string`): Gets the version of the VST SDK used by the plugin.
*   **MaxChannels** (`int`): Gets the maximum number of audio channels supported by the plugin.
*   **IsValid** (`bool`): Gets a value indicating whether the plugin information is currently in a valid state.
*   **ToString()** (`override string`): Returns a string representation of the plugin instance. Does not throw exceptions.

### VstParameter

Represents an individual parameter exposed by a VST plugin.

*   **Id** (`int`): Gets the unique numeric identifier for the parameter.
*   **Name** (`string`): Gets the display name of the parameter.
*   **Label** (`string`): Gets the short label or identifier for the parameter.
*   **Units** (`string`): Gets the unit of measurement for the parameter value (e.g., "dB", "Hz").
*   **MinValue** (`float`): Gets the minimum allowed value for the parameter.
*   **MaxValue** (`float`): Gets the maximum allowed value for the parameter.
*   **DefaultValue** (`float`): Gets the default value for the parameter.
*   **CurrentValue** (`float`): Gets the current value of the parameter.
*   **IsAutomated** (`bool`): Gets a value indicating whether the parameter supports automation.
*   **Category** (`string`): Gets the category classification of the parameter.
*   **IsReadOnly** (`bool`): Gets a value indicating whether the parameter value can be modified.
*   **IsValid** (`bool`): Gets a value indicating whether the parameter definition is valid.

## Usage

### Accessing Plugin Metadata
```csharp
void DisplayPluginDetails(VstPluginInfo pluginInfo)
{
    if (pluginInfo.IsValid)
    {
        Console.WriteLine($"Plugin ID: {pluginInfo.UniqueId}");
        Console.WriteLine($"Loaded at: {pluginInfo.LoadedAt}");
        Console.WriteLine($"Max Channels: {pluginInfo.MaxChannels}");
    }
}
```

### Inspecting Parameter Characteristics
```csharp
void LogParameterConfiguration(VstParameter parameter)
{
    Console.WriteLine($"Parameter: {parameter.Name}");
    Console.WriteLine($"Range: {parameter.MinValue} to {parameter.MaxValue}");
    Console.WriteLine($"Current: {parameter.CurrentValue} {parameter.Units}");
    
    if (parameter.IsReadOnly)
    {
        Console.WriteLine("This parameter cannot be automated.");
    }
}
```

## Notes

*   **Thread-Safety**: `VstPluginInfo` is a `record` and is intended to be immutable after initialization. However, `VstParameter` instances reflect the live state of the VST plugin, which may change asynchronously depending on the host's interaction or automation. Accessing `CurrentValue` should be performed with caution if the plugin is actively processing audio on a different thread.
*   **Validity**: The `IsValid` property should always be checked before accessing other properties, as it indicates whether the plugin or parameter initialization was successful.
*   **Floating-Point Precision**: Parameter values (`MinValue`, `MaxValue`, `DefaultValue`, `CurrentValue`) use `float` representation, consistent with standard VST SDK parameter specifications.
