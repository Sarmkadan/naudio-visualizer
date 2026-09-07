# Enums

This page provides a single reference for the public enumerations exposed by the `NAudioVisualizer` library. The visualization, device, and VST enumerations are in the `NAudioVisualizer.Domain.Models` namespace. `AudioStreamErrorCode` is in `NAudioVisualizer.Exceptions`, and `LogLevel` is in `NAudioVisualizer.Infrastructure`.

## WindowType

Specifies the window function applied to audio samples before FFT analysis.

*   **`Hann = 0`**
    Uses a Hann window.

*   **`Hamming = 1`**
    Uses a Hamming window.

*   **`Blackman = 2`**
    Uses a Blackman window.

*   **`Rectangular = 3`**
    Uses a rectangular window.

## ColormapType

Specifies the color map used to render spectrogram magnitude values.

*   **`Viridis = 0`**
    Uses the Viridis color map.

*   **`Plasma = 1`**
    Uses the Plasma color map.

*   **`Inferno = 2`**
    Uses the Inferno color map.

*   **`Magma = 3`**
    Uses the Magma color map.

*   **`Turbo = 4`**
    Uses the Turbo color map.

*   **`Hot = 5`**
    Uses the Hot color map.

## VisualizationType

Identifies the kind of audio visualization represented by a `VisualizationData` instance.

*   **`Waveform = 0`**
    Represents time-domain waveform data.

*   **`Spectrum = 1`**
    Represents frequency-spectrum data.

*   **`Spectrogram = 2`**
    Represents time-frequency spectrogram data.

## DeviceCapabilities

Describes audio-device capabilities. This enumeration has the `[Flags]` attribute, so its nonzero values can be combined with the bitwise OR operator.

*   **`None = 0`**
    Indicates that no capability flags are set.

*   **`Microphone = 1`**
    Indicates microphone input capability.

*   **`LineIn = 2`**
    Indicates line-input capability.

*   **`Stereo = 4`**
    Indicates stereo audio capability.

*   **`Mono = 8`**
    Indicates mono audio capability.

*   **`HighResolution = 16`**
    Indicates high-resolution audio capability.

*   **`RealTime = 32`**
    Indicates real-time audio capability.

## AudioStreamErrorCode

Categorizes failures reported by `AudioStreamException`.

*   **`Unknown = 0`**
    Indicates an unknown or uncategorized stream failure.

*   **`BufferOverflow = 1`**
    Indicates that an audio buffer overflowed.

*   **`BufferUnderrun = 2`**
    Indicates that an audio buffer did not contain enough data.

*   **`DeviceDisconnected = 3`**
    Indicates that the audio device was disconnected.

*   **`FormatUnsupported = 4`**
    Indicates that the requested audio format is unsupported.

*   **`PermissionDenied = 5`**
    Indicates that permission to access the audio resource was denied.

*   **`InitializationFailed = 6`**
    Indicates that audio-stream initialization failed.

*   **`HardwareError = 7`**
    Indicates an audio hardware failure.

## LogLevel

Specifies the severity of a log message. `Logger.MinimumLevel` uses the numeric ordering to discard messages whose level is lower than the configured minimum.

*   **`Debug = 0`**
    Diagnostic information used for debugging.

*   **`Info = 1`**
    General informational messages.

*   **`Warn = 2`**
    Warning messages for potentially problematic conditions.

*   **`Error = 3`**
    Error messages for failures.

*   **`Critical = 4`**
    Critical messages for the most severe failures.

## VstPluginState

Represents the lifecycle state of a VST plugin instance managed by the host.

*   **`Unloaded = 0`**
    The plugin library has not been loaded into memory.

*   **`Loaded = 1`**
    The library is loaded and waiting for initialization.

*   **`Initializing = 2`**
    One-time initialization is in progress.

*   **`Active = 3`**
    The plugin is fully initialized and accepting audio input and output.

*   **`Suspended = 4`**
    The plugin is temporarily suspended; its state is preserved while real-time resources are freed.

*   **`Error = 5`**
    The plugin encountered an unrecoverable error and should be unloaded and discarded.

## VstPluginCategory

Classifies VST plugins by function for filtering and display.

*   **`Undefined = 0`**
    The plugin has not declared a category.

*   **`Effect = 1`**
    A general audio effect, such as a compressor or saturator.

*   **`Synth = 2`**
    A software synthesizer or instrument.

*   **`Analyzer = 3`**
    A spectrum analyzer or metering tool.

*   **`Spatial = 4`**
    A spatial or positioning effect, such as a panner or binaural processor.

*   **`Mastering = 5`**
    Mastering-grade processing, such as a limiter or stereo enhancer.

*   **`Dynamics = 6`**
    A dynamic-range processor, such as a compressor, gate, or expander.

*   **`EQ = 7`**
    An equalizer or filter.

*   **`Reverb = 8`**
    Reverberation or room simulation.

*   **`Delay = 9`**
    Delay or echo processing.

*   **`Distortion = 10`**
    Distortion, overdrive, or saturation processing.

*   **`Modulation = 11`**
    A modulation effect, such as chorus, flanger, phaser, or tremolo.

## VstAutomationInterpolation

Specifies the interpolation curve used when evaluating values between adjacent VST parameter automation points.

*   **`Linear = 0`**
    Uses a straight line between adjacent points.

*   **`CubicSpline = 1`**
    Uses a smooth Catmull-Rom cubic spline through surrounding points.

*   **`Step = 2`**
    Holds the current value and jumps instantly to the next point without a ramp.

*   **`Cosine = 3`**
    Uses an S-curve cosine crossfade between adjacent points.

## Usage

The following example shows how to select a spectrum window and spectrogram color map, combine device capability flags, categorize a stream exception, and add a VST automation point.

```csharp
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Exceptions;

var spectrum = new SpectrumData(magnitudes, frequencies, 48000, 2048)
{
    WindowType = WindowType.Blackman
};

var spectrogram = new SpectrogramData(matrix, 48000, 2048, 512)
{
    ColormapType = ColormapType.Inferno
};

DeviceCapabilities capabilities =
    DeviceCapabilities.Microphone | DeviceCapabilities.Stereo;

bool supportsStereo = capabilities.HasFlag(DeviceCapabilities.Stereo);

var exception = new AudioStreamException(
    "The capture device was disconnected.",
    AudioStreamErrorCode.DeviceDisconnected);

var lane = new VstParameterAutomationLane
{
    PluginId = Guid.NewGuid(),
    ParameterId = 0
};
lane.AddPoint(1.5, 0.75f, VstAutomationInterpolation.Cosine);
```

## Notes

*   `DeviceCapabilities` values may be combined because the enumeration is marked with `[Flags]`. `None` represents the absence of all flags.
*   The integer values shown on this page are explicitly assigned in the source definitions.
