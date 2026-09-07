# AudioConstants

The constants in `NAudioVisualizer.Constants` provide shared audio defaults, visualization settings, ARGB colors, and text defaults. All members documented below are compile-time constants declared in `AudioConstants.cs`.

## AudioConstants

`AudioConstants` defines standard audio formats, analysis thresholds, FFT bounds, time windows, frequency ranges, and decibel limits.

| Name | Value | Meaning |
| --- | ---: | --- |
| `SAMPLE_RATE_44100` | `44100` | Standard sample rate of 44,100 samples per second. |
| `SAMPLE_RATE_48000` | `48000` | Standard sample rate of 48,000 samples per second. |
| `SAMPLE_RATE_96000` | `96000` | Standard sample rate of 96,000 samples per second. |
| `SAMPLE_RATE_192000` | `192000` | Standard sample rate of 192,000 samples per second. |
| `DEFAULT_SAMPLE_RATE` | `SAMPLE_RATE_44100` (`44100`) | Default audio sample rate. |
| `DEFAULT_CHANNEL_COUNT` | `2` | Default number of audio channels. |
| `DEFAULT_BIT_DEPTH` | `16` | Default audio bit depth. |
| `DEFAULT_BUFFER_SIZE` | `4096` | Default audio buffer size. |
| `SILENCE_THRESHOLD` | `0.01f` | Audio-level threshold used to represent silence. |
| `PEAK_DETECTION_THRESHOLD` | `0.8f` | Audio-level threshold used for peak detection. |
| `CLIPPING_THRESHOLD` | `0.95f` | Audio-level threshold used to identify clipping. |
| `DEFAULT_FFT_SIZE` | `2048` | Default FFT size for spectrum analysis. |
| `DEFAULT_FFT_SIZE_LARGE` | `4096` | Larger default FFT size. |
| `FFT_MINIMUM` | `256` | Minimum FFT size. |
| `FFT_MAXIMUM` | `16384` | Maximum FFT size. |
| `FRAME_ANALYSIS_WINDOW_MS` | `100` | Frame-analysis window duration in milliseconds. |
| `HISTORY_BUFFER_DURATION_SECONDS` | `30` | History-buffer duration in seconds. |
| `MIN_FREQUENCY_HZ` | `20f` | Lower frequency bound in hertz. |
| `MAX_FREQUENCY_HZ` | `20000f` | Upper frequency bound in hertz. |
| `NYQUIST_FREQUENCY_HZ_44100` | `22050f` | Nyquist frequency in hertz for a 44,100 Hz sample rate. |
| `NYQUIST_FREQUENCY_HZ_48000` | `24000f` | Nyquist frequency in hertz for a 48,000 Hz sample rate. |
| `DB_REFERENCE_LEVEL` | `1f` | Reference amplitude level for decibel calculations. |
| `DB_MIN_LEVEL` | `-96f` | Minimum decibel level. |
| `DB_MAX_LEVEL` | `0f` | Maximum decibel level. |

## VisualizationConstants

`VisualizationConstants` defines rendering dimensions, performance defaults, and settings for waveform, spectrum, and spectrogram output.

| Name | Value | Meaning |
| --- | ---: | --- |
| `DEFAULT_RENDER_WIDTH` | `1920` | Default render width in pixels. |
| `DEFAULT_RENDER_HEIGHT` | `1080` | Default render height in pixels. |
| `MINIMUM_RENDER_WIDTH` | `320` | Minimum render width in pixels. |
| `MINIMUM_RENDER_HEIGHT` | `240` | Minimum render height in pixels. |
| `DEFAULT_TARGET_FPS` | `60` | Default target frame rate in frames per second. |
| `MAXIMUM_TARGET_FPS` | `144` | Maximum target frame rate in frames per second. |
| `DEFAULT_RENDERING_QUALITY` | `85` | Default rendering-quality value. |
| `DEFAULT_WAVEFORM_DOWNSAMPLING` | `4` | Default waveform downsampling value. |
| `DEFAULT_WAVEFORM_LINE_WIDTH` | `1.5f` | Default waveform line width. |
| `MINIMUM_WAVEFORM_LINE_WIDTH` | `0.5f` | Minimum waveform line width. |
| `MAXIMUM_WAVEFORM_LINE_WIDTH` | `5f` | Maximum waveform line width. |
| `DEFAULT_SPECTRUM_FFT_SIZE` | `2048` | Default FFT size for spectrum rendering. |
| `DEFAULT_SPECTRUM_SMOOTHING` | `3` | Default spectrum-smoothing value. |
| `MAXIMUM_SPECTRUM_SMOOTHING` | `10` | Maximum spectrum-smoothing value. |
| `DEFAULT_BAR_GAP` | `1` | Default gap between spectrum bars. |
| `DEFAULT_SPECTROGRAM_TIME_WINDOW` | `10f` | Default spectrogram time window. |
| `MINIMUM_SPECTROGRAM_TIME_WINDOW` | `1f` | Minimum spectrogram time window. |
| `MAXIMUM_SPECTROGRAM_TIME_WINDOW` | `60f` | Maximum spectrogram time window. |
| `DEFAULT_SPECTROGRAM_FFT_SIZE` | `2048` | Default FFT size for spectrogram rendering. |

## ColorConstants

`ColorConstants` defines visualization colors as 32-bit unsigned ARGB values in `0xAARRGGBB` format.

| Name | Value | Meaning |
| --- | ---: | --- |
| `COLOR_BLACK` | `0xFF000000` | Opaque black. |
| `COLOR_WHITE` | `0xFFFFFFFF` | Opaque white. |
| `COLOR_DARK_BACKGROUND` | `0xFF1a1a1a` | Opaque dark background color. |
| `COLOR_LIGHT_BACKGROUND` | `0xFFf5f5f5` | Opaque light background color. |
| `COLOR_WAVEFORM_DEFAULT` | `0xFF00D9FF` | Default opaque waveform color. |
| `COLOR_WAVEFORM_OUTLINE` | `0xFFFFFFFF` | Opaque white waveform-outline color. |
| `COLOR_SPECTRUM_DEFAULT` | `0xFF00FF00` | Default opaque green spectrum color. |
| `COLOR_SPECTRUM_PEAK` | `0xFFFF0000` | Opaque red spectrum-peak color. |
| `COLOR_SPECTRUM_AVERAGE` | `0xFFFFFF00` | Opaque yellow spectrum-average color. |
| `COLOR_GRID_LINE` | `0x33FFFFFF` | Translucent white grid-line color. |
| `COLOR_TEXT` | `0xFFCCCCCC` | Opaque light-gray text color. |
| `COLOR_TEXT_LABEL` | `0xFF999999` | Opaque gray label-text color. |
| `COLOR_BUTTON_NORMAL` | `0xFF404040` | Opaque button color for the normal state. |
| `COLOR_BUTTON_HOVER` | `0xFF606060` | Opaque button color for the hover state. |
| `COLOR_BUTTON_PRESSED` | `0xFF202020` | Opaque button color for the pressed state. |

## TextConstants

`TextConstants` defines default font sizes and font-family names.

| Name | Value | Meaning |
| --- | ---: | --- |
| `DEFAULT_FONT_SIZE` | `12f` | Default font size. |
| `LABEL_FONT_SIZE` | `10f` | Font size for labels. |
| `TITLE_FONT_SIZE` | `16f` | Font size for titles. |
| `DEFAULT_FONT_FAMILY` | `"Arial"` | Default font-family name. |
| `MONOSPACE_FONT_FAMILY` | `"Courier New"` | Monospace font-family name. |

## Usage

Constants can be referenced directly from their declaring static class:

```csharp
using NAudioVisualizer.Constants;

int sampleRate = AudioConstants.DEFAULT_SAMPLE_RATE;
int fftSize = AudioConstants.DEFAULT_FFT_SIZE;
int targetFps = VisualizationConstants.DEFAULT_TARGET_FPS;
uint waveformColor = ColorConstants.COLOR_WAVEFORM_DEFAULT;
string fontFamily = TextConstants.DEFAULT_FONT_FAMILY;
```

## Notes

*   `DEFAULT_SAMPLE_RATE` is an alias for `SAMPLE_RATE_44100`; both compile to the value `44100`.
*   Color values include the alpha byte as the most significant byte. For example, `COLOR_GRID_LINE` begins with alpha `0x33`, while colors beginning with `0xFF` are fully opaque.
*   The constants provide values only. `AudioConstants.cs` does not perform validation, rendering, analysis, unit conversion, or platform font resolution.
