#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace NAudioVisualizer.Constants;

/// <summary>
/// Audio processing constants including sample rates, FFT sizes, frequency ranges,
/// and level thresholds used throughout the visualization pipeline.
/// </summary>
public static class AudioConstants
{
    // Sample rate standards (Hz)
    /// <summary>CD-quality sample rate (44.1 kHz).</summary>
    public const int SAMPLE_RATE_44100 = 44100;
    /// <summary>Professional audio / video standard sample rate (48 kHz).</summary>
    public const int SAMPLE_RATE_48000 = 48000;
    /// <summary>High-resolution audio sample rate (96 kHz).</summary>
    public const int SAMPLE_RATE_96000 = 96000;
    /// <summary>Ultra high-resolution audio sample rate (192 kHz).</summary>
    public const int SAMPLE_RATE_192000 = 192000;

    // Default values
    /// <summary>Default sample rate for audio capture (44100 Hz).</summary>
    public const int DEFAULT_SAMPLE_RATE = SAMPLE_RATE_44100;
    /// <summary>Default number of audio channels (stereo).</summary>
    public const int DEFAULT_CHANNEL_COUNT = 2;
    /// <summary>Default bit depth per sample (16-bit PCM).</summary>
    public const int DEFAULT_BIT_DEPTH = 16;
    /// <summary>Default audio buffer size in samples.</summary>
    public const int DEFAULT_BUFFER_SIZE = 4096;

    // Audio level thresholds (normalized 0.0 - 1.0)
    /// <summary>Amplitude below which audio is considered silence.</summary>
    public const float SILENCE_THRESHOLD = 0.01f;
    /// <summary>Amplitude threshold for peak detection algorithms.</summary>
    public const float PEAK_DETECTION_THRESHOLD = 0.8f;
    /// <summary>Amplitude at which audio is considered clipping.</summary>
    public const float CLIPPING_THRESHOLD = 0.95f;

    // FFT and spectrum analysis
    /// <summary>Default FFT window size (2048 bins). Must be a power of 2.</summary>
    public const int DEFAULT_FFT_SIZE = 2048;
    /// <summary>Large FFT window for higher frequency resolution (4096 bins).</summary>
    public const int DEFAULT_FFT_SIZE_LARGE = 4096;
    /// <summary>Minimum allowed FFT size.</summary>
    public const int FFT_MINIMUM = 256;
    /// <summary>Maximum allowed FFT size.</summary>
    public const int FFT_MAXIMUM = 16384;

    // Time constants
    /// <summary>Duration of each analysis frame in milliseconds.</summary>
    public const int FRAME_ANALYSIS_WINDOW_MS = 100;
    /// <summary>Duration of the rolling history buffer in seconds.</summary>
    public const int HISTORY_BUFFER_DURATION_SECONDS = 30;

    // Frequency ranges (Hz)
    /// <summary>Lower bound of human hearing range (20 Hz).</summary>
    public const float MIN_FREQUENCY_HZ = 20f;
    /// <summary>Upper bound of human hearing range (20 kHz).</summary>
    public const float MAX_FREQUENCY_HZ = 20000f;
    /// <summary>Nyquist frequency for 44.1 kHz sample rate.</summary>
    public const float NYQUIST_FREQUENCY_HZ_44100 = 22050f;
    /// <summary>Nyquist frequency for 48 kHz sample rate.</summary>
    public const float NYQUIST_FREQUENCY_HZ_48000 = 24000f;

    // Loudness/dB constants
    /// <summary>Reference level for dB calculations (0 dBFS = 1.0 amplitude).</summary>
    public const float DB_REFERENCE_LEVEL = 1f;
    /// <summary>Minimum representable dB level (-96 dB for 16-bit audio).</summary>
    public const float DB_MIN_LEVEL = -96f;
    /// <summary>Maximum dB level (0 dBFS = full scale).</summary>
    public const float DB_MAX_LEVEL = 0f;
}

/// <summary>
/// Visualization rendering constants.
/// </summary>
public static class VisualizationConstants
{
    // Default dimensions
    public const int DEFAULT_RENDER_WIDTH = 1920;
    public const int DEFAULT_RENDER_HEIGHT = 1080;
    public const int MINIMUM_RENDER_WIDTH = 320;
    public const int MINIMUM_RENDER_HEIGHT = 240;

    // Performance constants
    public const int DEFAULT_TARGET_FPS = 60;
    public const int MAXIMUM_TARGET_FPS = 144;
    public const int DEFAULT_RENDERING_QUALITY = 85;

    // Waveform constants
    public const int DEFAULT_WAVEFORM_DOWNSAMPLING = 4;
    public const float DEFAULT_WAVEFORM_LINE_WIDTH = 1.5f;
    public const float MINIMUM_WAVEFORM_LINE_WIDTH = 0.5f;
    public const float MAXIMUM_WAVEFORM_LINE_WIDTH = 5f;

    // Spectrum constants
    public const int DEFAULT_SPECTRUM_FFT_SIZE = 2048;
    public const int DEFAULT_SPECTRUM_SMOOTHING = 3;
    public const int MAXIMUM_SPECTRUM_SMOOTHING = 10;
    public const int DEFAULT_BAR_GAP = 1;

    // Spectrogram constants
    public const float DEFAULT_SPECTROGRAM_TIME_WINDOW = 10f;
    public const float MINIMUM_SPECTROGRAM_TIME_WINDOW = 1f;
    public const float MAXIMUM_SPECTROGRAM_TIME_WINDOW = 60f;
    public const int DEFAULT_SPECTROGRAM_FFT_SIZE = 2048;
}

/// <summary>
/// Color constants for visualization.
/// </summary>
public static class ColorConstants
{
    // Common colors (ARGB format)
    public const uint COLOR_BLACK = 0xFF000000;
    public const uint COLOR_WHITE = 0xFFFFFFFF;
    public const uint COLOR_DARK_BACKGROUND = 0xFF1a1a1a;
    public const uint COLOR_LIGHT_BACKGROUND = 0xFFf5f5f5;

    // Waveform colors
    public const uint COLOR_WAVEFORM_DEFAULT = 0xFF00D9FF;
    public const uint COLOR_WAVEFORM_OUTLINE = 0xFFFFFFFF;

    // Spectrum colors
    public const uint COLOR_SPECTRUM_DEFAULT = 0xFF00FF00;
    public const uint COLOR_SPECTRUM_PEAK = 0xFFFF0000;
    public const uint COLOR_SPECTRUM_AVERAGE = 0xFFFFFF00;

    // Grid and text
    public const uint COLOR_GRID_LINE = 0x33FFFFFF;
    public const uint COLOR_TEXT = 0xFFCCCCCC;
    public const uint COLOR_TEXT_LABEL = 0xFF999999;

    // UI elements
    public const uint COLOR_BUTTON_NORMAL = 0xFF404040;
    public const uint COLOR_BUTTON_HOVER = 0xFF606060;
    public const uint COLOR_BUTTON_PRESSED = 0xFF202020;
}

/// <summary>
/// Font and text constants.
/// </summary>
public static class TextConstants
{
    public const float DEFAULT_FONT_SIZE = 12f;
    public const float LABEL_FONT_SIZE = 10f;
    public const float TITLE_FONT_SIZE = 16f;

    public const string DEFAULT_FONT_FAMILY = "Arial";
    public const string MONOSPACE_FONT_FAMILY = "Courier New";
}
