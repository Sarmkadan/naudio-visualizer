# Configuration Keys Reference

This document provides a complete reference of all configuration keys available in the NAudio Visualizer application, including their types, default values, and validation ranges.

## Configuration Keys

| Key | Type | Default Value | Validation Range/Allowed Values | Description |
|-----|------|---------------|---------------------------------|-------------|
| `audio.sampleRate` | `int` | 44100 | [8000, 96000] | Audio sample rate in Hz |
| `audio.channelCount` | `int` | 2 | [1, 8] | Number of audio channels |
| `audio.bitDepth` | `int` | 16 | [8, 32] | Audio bit depth in bits |
| `audio.fftSize` | `int` | 2048 | [64, 16384] | FFT size for audio analysis |
| `visualization.targetFps` | `int` | 60 | [1, 240] | Target frames per second for visualization |
| `visualization.brightness` | `float` | 1.0 | [0.0, 2.0] | Brightness multiplier for visualization |
| `visualization.contrast` | `float` | 1.0 | [0.0, 2.0] | Contrast multiplier for visualization |
| `visualization.peakHoldEnabled` | `bool` | false | true/false | Whether peak hold is enabled for visualization |
| `visualization.peakHoldFallRateDbPerSec` | `float` | 10.0 | [0.0, ∞) | Peak hold fall rate in dB per second |
| `display.width` | `int` | 1280 | [320, 7680] | Display width in pixels |
| `display.height` | `int` | 720 | [240, 4320] | Display height in pixels |
| `display.fullscreen` | `bool` | false | true/false | Whether display should be fullscreen |
| `export.defaultFormat` | `string` | "json" | "json", "xml", "yaml", "csv" | Default export format |
| `export.compress` | `bool` | false | true/false | Whether to compress exported data |
| `export.includeMetadata` | `bool` | true | true/false | Whether to include metadata in exported data |
| `logging.level` | `string` | "Info" | "Debug", "Info", "Warn", "Error" | Logging level |
| `logging.writeToConsole` | `bool` | true | true/false | Whether to write logs to console |
| `logging.writeToFile` | `bool` | true | true/false | Whether to write logs to file |

## Sample settings.json

```json
{
  "audio.sampleRate": 44100,
  "audio.channelCount": 2,
  "audio.bitDepth": 16,
  "audio.fftSize": 2048,
  "visualization.targetFps": 60,
  "visualization.brightness": 1.0,
  "visualization.contrast": 1.0,
  "visualization.peakHoldEnabled": false,
  "visualization.peakHoldFallRateDbPerSec": 10.0,
  "display.width": 1280,
  "display.height": 720,
  "display.fullscreen": false,
  "export.defaultFormat": "json",
  "export.compress": false,
  "export.includeMetadata": true,
  "logging.level": "Info",
  "logging.writeToConsole": true,
  "logging.writeToFile": true
}
```

## Notes

- All configuration keys are defined as constants in `ConfigurationManager.Keys`
- Default values are loaded in `ConfigurationManager.LoadDefaults()`
- Validation rules are implemented in `ConfigurationManagerValidation`
- String comparisons for enum-like settings are case-insensitive
- Numeric validations include both minimum and maximum bounds (inclusive)