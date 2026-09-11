# Application Settings

The `ApplicationSettings` and `ApplicationConfiguration` classes manage application-wide configuration and service setup for the `naudio-visualizer` project.

## ApplicationSettings

The `ApplicationSettings` class holds configurable application parameters with sensible defaults.

### Properties

#### `public int MaxAudioBufferSize { get; set; } = AudioConstants.SAMPLE_RATE_192000;`

Maximum audio buffer size in samples. Defaults to 192000 (corresponding to the 192kHz sample rate).

#### `public int DefaultSampleRate { get; set; } = AudioConstants.DEFAULT_SAMPLE_RATE;`

Default sample rate for audio capture in Hz. Defaults to 44100 (44.1kHz).

#### `public int DefaultFftSize { get; set; } = AudioConstants.DEFAULT_FFT_SIZE;`

Default FFT size for spectrum analysis. Defaults to 2048.

#### `public int TargetFps { get; set; } = VisualizationConstants.DEFAULT_TARGET_FPS;`

Target frames per second for visualization rendering. Defaults to 60.

#### `public bool EnableLogging { get; set; } = true;`

Enable or disable audio logging. Defaults to `true`.

#### `public int MaxFramesPerSession { get; set; } = 5000;`

Maximum frames to keep in memory per session. Defaults to 5000.

### Methods

#### `public override string ToString()`

Returns a formatted string summarizing the current settings:
```
MaxAudioBufferSize={value}, DefaultSampleRate={value}, DefaultFftSize={value}, TargetFps={value}, EnableLogging={value}, MaxFramesPerSession={value}
```

#### `public bool IsValid()`

Validates that all settings have appropriate values:
- Returns `true` if all numeric properties are greater than zero
- Returns `false` otherwise

## ApplicationConfiguration

The `ApplicationConfiguration` class provides static methods for configuring application services.

### Methods

#### `public static ServiceContainer ConfigureServices()`

Configures all application services with default setup:
- Registers `ILogger` with a `Logger` instance
- Registers `AudioSessionRepository` and `VisualizationDataRepository` as singletons
- Registers factory delegates for lazy initialization of:
  - `AudioCaptureService`
  - `WaveformService`
  - `SpectrumAnalyzer`
  - `SpectrogramAnalyzer`

**Returns:** A configured `ServiceContainer` instance.

#### `public static ServiceContainer ConfigureServices(ApplicationSettings settings)`

Configures services with custom settings:
- Calls the parameterless `ConfigureServices()` to get a base container
- Applies additional configuration based on the provided settings (currently checks if `MaxAudioBufferSize > 0`)
- Returns the configured container

**Parameters:**
- `settings` (`ApplicationSettings`): The settings to apply to the service configuration

**Returns:** A configured `ServiceContainer` instance.

#### `public override string ToString()`

Returns the full name of the type.

## Usage Example

```csharp
// Create default settings
var settings = new ApplicationSettings();
// Or customize settings
var customSettings = new ApplicationSettings
{
    MaxAudioBufferSize = 96000,
    DefaultSampleRate = 48000,
    TargetFps = 30,
    EnableLogging = false
};

// Configure services with settings
var container = ApplicationConfiguration.ConfigureServices(customSettings);

// Resolve services from the container
var audioCapture = container.Resolve<AudioCaptureService>();
var waveformService = container.Resolve<WaveformService>();
```