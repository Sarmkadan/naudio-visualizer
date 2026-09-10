#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace NAudioVisualizer.Constants;

/// <summary>
/// Audio-related constants and default values.
/// </summary>
public static class AudioConstants
{
    // Sample rate standards
    /// <summary>
    /// Sample rate of 44,100 Hz.
    /// </summary>
    public const int SAMPLE_RATE_44100 = 44100;
    /// <summary>
    /// Sample rate of 48,000 Hz.
    /// </summary>
    public const int SAMPLE_RATE_48000 = 48000;
    /// <summary>
    /// Sample rate of 96,000 Hz.
    /// </summary>
    public const int SAMPLE_RATE_96000 = 96000;
    /// <summary>
    /// Sample rate of 192,000 Hz.
    /// </summary>
    public const int SAMPLE_RATE_192000 = 192000;

    // Default values
    /// <summary>
    /// Default sample rate (44,100 Hz).
    /// </summary>
    public const int DEFAULT_SAMPLE_RATE = SAMPLE_RATE_44100;
    /// <summary>
    /// Default number of audio channels (2).
    /// </summary>
    public const int DEFAULT_CHANNEL_COUNT = 2;
    /// <summary>
    /// Default bit depth (16 bits).
    /// </summary>
    public const int DEFAULT_BIT_DEPTH = 16;
    /// <summary>
    /// Default buffer size (4096 samples).
    /// </summary>
    public const int DEFAULT_BUFFER_SIZE = 4096;

    // Audio level thresholds
    /// <summary>
    /// Silence threshold (amplitude 0.01, unitless).
    /// </summary>
    public const float SILENCE_THRESHOLD = 0.01f;
    /// <summary>
    /// Peak detection threshold (amplitude 0.8, unitless).
    /// </summary>
    public const float PEAK_DETECTION_THRESHOLD = 0.8f;
    /// <summary>
    /// Clipping threshold (amplitude 0.95, unitless).
    /// </summary>
    public const float CLIPPING_THRESHOLD = 0.95f;

    // FFT and spectrum analysis
    /// <summary>
    /// Default FFT size (2048 points).
    /// </summary>
    public const int DEFAULT_FFT_SIZE = 2048;
    /// <summary>
    /// Default large FFT size (4096 points).
    /// </summary>
    public const int DEFAULT_FFT_SIZE_LARGE = 4096;
    /// <summary>
    /// Minimum FFT size (256 points).
    /// </summary>
    public const int FFT_MINIMUM = 256;
    /// <summary>
    /// Maximum FFT size (16,384 points).
    /// </summary>
    public const int FFT_MAXIMUM = 16384;

    // Time constants
    /// <summary>
    /// Frame analysis window duration (100 ms).
    /// </summary>
    public const int FRAME_ANALYSIS_WINDOW_MS = 100;
    /// <summary>
    /// History buffer duration (30 seconds).
    /// </summary>
    public const int HISTORY_BUFFER_DURATION_SECONDS = 30;

    // Frequency ranges
    /// <summary>
    /// Minimum audible frequency (20 Hz).
    /// </summary>
    public const float MIN_FREQUENCY_HZ = 20f;
    /// <summary>
    /// Maximum audible frequency (20 000 Hz).
    /// </summary>
    public const float MAX_FREQUENCY_HZ = 20000f;
    /// <summary>
    /// Nyquist frequency for 44,100 Hz sample rate (22,050 Hz).
    /// </summary>
    public const float NYQUIST_FREQUENCY_HZ_44100 = 22050f;
    /// <summary>
    /// Nyquist frequency for 48,000 Hz sample rate (24,000 Hz).
    /// </summary>
    public const float NYQUIST_FREQUENCY_HZ_48000 = 24000f;

    // Loudness/dB constants
    /// <summary>
    /// Reference level for dB calculations (1.0, unitless).
    /// </summary>
    public const float DB_REFERENCE_LEVEL = 1f;
    /// <summary>
    /// Minimum dB level (-96 dB).
    /// </summary>
    public const float DB_MIN_LEVEL = -96f;
    /// <summary>
    /// Maximum dB level (0 dB).
    /// </summary>
    public const float DB_MAX_LEVEL = 0f;
}

/// <summary>
/// Visualization rendering constants.
/// </summary>
public static class VisualizationConstants
{
    // Default dimensions
    /// <summary>
    /// Default render width (1920 pixels).
    /// </summary>
    public const int DEFAULT_RENDER_WIDTH = 1920;
    /// <summary>
    /// Default render height (1080 pixels).
    /// </summary>
    public const int DEFAULT_RENDER_HEIGHT = 1080;
    /// <summary>
    /// Minimum render width (320 pixels).
    /// </summary>
    public const int MINIMUM_RENDER_WIDTH = 320;
    /// <summary>
    /// Minimum render height (240 pixels).
    /// </summary>
    public const int MINIMUM_RENDER_HEIGHT = 240;

    // Performance constants
    /// <summary>
    /// Default target frames per second (60 fps).
    /// </summary>
    public const int DEFAULT_TARGET_FPS = 60;
    /// <summary>
    /// Maximum target frames per second (144 fps).
    /// </summary>
    public const int MAXIMUM_TARGET_FPS = 144;
    /// <summary>
    /// Default rendering quality (85 % JPEG quality or equivalent).
    /// </summary>
    public const int DEFAULT_RENDERING_QUALITY = 85;

    // Waveform constants
    /// <summary>
    /// Default waveform down‑sampling factor (4).
    /// </summary>
    public const int DEFAULT_WAVEFORM_DOWNSAMPLING = 4;
    /// <summary>
    /// Default waveform line width (1.5 units).
    /// </summary>
    public const float DEFAULT_WAVEFORM_LINE_WIDTH = 1.5f;
    /// <summary>
    /// Minimum waveform line width (0.5 units).
    /// </summary>
    public const float MINIMUM_WAVEFORM_LINE_WIDTH = 0.5f;
    /// <summary>
    /// Maximum waveform line width (5 units).
    /// </summary>
    public const float MAXIMUM_WAVEFORM_LINE_WIDTH = 5f;

    // Spectrum constants
    /// <summary>
    /// Default spectrum FFT size (2048 points).
    /// </summary>
    public const int DEFAULT_SPECTRUM_FFT_SIZE = 2048;
    /// <summary>
    /// Default spectrum smoothing factor (3).
    /// </summary>
    public const int DEFAULT_SPECTRUM_SMOOTHING = 3;
    /// <summary>
    /// Maximum spectrum smoothing factor (10).
    /// </summary>
    public const int MAXIMUM_SPECTRUM_SMOOTHING = 10;
    /// <summary>
    /// Default bar gap between spectrum bars (1 pixel).
    /// </summary>
    public const int DEFAULT_BAR_GAP = 1;

    // Spectrogram constants
    /// <summary>
    /// Default spectrogram time window (10 seconds).
    /// </summary>
    public const float DEFAULT_SPECTROGRAM_TIME_WINDOW = 10f;
    /// <summary>
    /// Minimum spectrogram time window (1 second).
    /// </summary>
    public const float MINIMUM_SPECTROGRAM_TIME_WINDOW = 1f;
    /// <summary>
    /// Maximum spectrogram time window (60 seconds).
    /// </summary>
    public const float MAXIMUM_SPECTROGRAM_TIME_WINDOW = 60f;
    /// <summary>
    /// Default spectrogram FFT size (2048 points).
    /// </summary>
    public const int DEFAULT_SPECTROGRAM_FFT_SIZE = 2048;
}

/// <summary>
/// Color constants for visualization.
/// </summary>
public static class ColorConstants
{
    // Common colors (ARGB format)
    /// <summary>
    /// Black color (ARGB 0xFF000000).
    /// </summary>
    public const uint COLOR_BLACK = 0xFF000000;
    /// <summary>
    /// White color (ARGB 0xFFFFFFFF).
    /// </summary>
    public const uint COLOR_WHITE = 0xFFFFFFFF;
    /// <summary>
    /// Dark background color (ARGB 0xFF1A1A1A).
    /// </summary>
    public const uint COLOR_DARK_BACKGROUND = 0xFF1a1a1a;
    /// <summary>
    /// Light background color (ARGB 0xFFF5F5F5).
    /// </summary>
    public const uint COLOR_LIGHT_BACKGROUND = 0xFFf5f5f5;

    // Waveform colors
    /// <summary>
    /// Default waveform color (ARGB 0xFF00D9FF).
    /// </summary>
    public const uint COLOR_WAVEFORM_DEFAULT = 0xFF00D9FF;
    /// <summary>
    /// Waveform outline color (ARGB 0xFFFFFFFF).
    /// </summary>
    public const uint COLOR_WAVEFORM_OUTLINE = 0xFFFFFFFF;

    // Spectrum colors
    /// <summary>
    /// Default spectrum color (ARGB 0xFF00FF00).
    /// </summary>
    public const uint COLOR_SPECTRUM_DEFAULT = 0xFF00FF00;
    /// <summary>
    /// Spectrum peak color (ARGB 0xFFFF0000).
    /// </summary>
    public const uint COLOR_SPECTRUM_PEAK = 0xFFFF0000;
    /// <summary>
    /// Spectrum average color (ARGB 0xFFFFFF00).
    /// </summary>
    public const uint COLOR_SPECTRUM_AVERAGE = 0xFFFFFF00;

    // Grid and text
    /// <summary>
    /// Grid line color (ARGB 0x33FFFFFF, semi‑transparent white).
    /// </summary>
    public const uint COLOR_GRID_LINE = 0x33FFFFFF;
    /// <summary>
    /// Text color (ARGB 0xFFCCCCCC).
    /// </summary>
    public const uint COLOR_TEXT = 0xFFCCCCCC;
    /// <summary>
    /// Text label color (ARGB 0xFF999999).
    /// </summary>
    public const uint COLOR_TEXT_LABEL = 0xFF999999;

    // UI elements
    /// <summary>
    /// Normal button color (ARGB 0xFF404040).
    /// </summary>
    public const uint COLOR_BUTTON_NORMAL = 0xFF404040;
    /// <summary>
    /// Hovered button color (ARGB 0xFF606060).
    /// </summary>
    public const uint COLOR_BUTTON_HOVER = 0xFF606060;
    /// <summary>
    /// Pressed button color (ARGB 0xFF202020).
    /// </summary>
    public const uint COLOR_BUTTON_PRESSED = 0xFF202020;
}

/// <summary>
/// Font and text constants.
/// </summary>
public static class TextConstants
{
    /// <summary>
    /// Default font size (12 points).
    /// </summary>
    public const float DEFAULT_FONT_SIZE = 12f;
    /// <summary>
    /// Label font size (10 points).
    /// </summary>
    public const float LABEL_FONT_SIZE = 10f;
    /// <summary>
    /// Title font size (16 points).
    /// </summary>
    public const float TITLE_FONT_SIZE = 16f;

    /// <summary>
    /// Default font family name ("Arial").
    /// </summary>
    public const string DEFAULT_FONT_FAMILY = "Arial";
    /// <summary>
    /// Monospace font family name ("Courier New").
    /// </summary>
    public const string MONOSPACE_FONT_FAMILY = "Courier New";
}
