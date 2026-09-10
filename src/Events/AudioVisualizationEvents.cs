#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using NAudioVisualizer.Domain.Models;

namespace NAudioVisualizer.Events

// Event classes for audio capture and visualization pipeline.
// These events allow decoupled communication between different components.

{
    /// <summary>
    /// Raised when audio capture starts.
    /// </summary>
    public class AudioCaptureStartedEvent
    {
        /// <summary>
        /// Gets the audio device ID.
        /// </summary>
        public int DeviceId { get; init; }
        /// <summary>
        /// Gets the sample rate in Hz.
        /// </summary>
        public int SampleRate { get; init; }
        /// <summary>
        /// Gets the number of audio channels.
        /// </summary>
        public int ChannelCount { get; init; }
        /// <summary>
        /// Gets the start time of the capture.
        /// </summary>
        public DateTime StartTime { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Raised when audio capture stops.
    /// </summary>
    public class AudioCaptureStoppedEvent
    {
        /// <summary>
        /// Gets the audio device ID.
        /// </summary>
        public int DeviceId { get; init; }
        /// <summary>
        /// Gets the total number of samples captured.
        /// </summary>
        public long TotalSamplesCaptured { get; init; }
        /// <summary>
        /// Gets the duration of the capture.
        /// </summary>
        public TimeSpan Duration { get; init; }
        /// <summary>
        /// Gets the stop time of the capture.
        /// </summary>
        public DateTime StopTime { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Raised when a new audio frame is captured.
    /// </summary>
    public class AudioFrameCapturedEvent
    {
        /// <summary>
        /// Gets the captured audio frame.
        /// </summary>
        public required AudioFrame Frame { get; init; }
        /// <summary>
        /// Gets the sequence number of the frame.
        /// </summary>
        public long FrameSequenceNumber { get; init; }
        /// <summary>
        /// Gets the elapsed time since capture started.
        /// </summary>
        public TimeSpan ElapsedTime { get; init; }
    }

    /// <summary>
    /// Raised when waveform data is generated.
    /// </summary>
    public class WaveformGeneratedEvent
    {
        /// <summary>
        /// Gets the generated waveform data.
        /// </summary>
        public required WaveformData Waveform { get; init; }
        /// <summary>
        /// Gets the generation time in milliseconds.
        /// </summary>
        public long GenerationTimeMs { get; init; }
        /// <summary>
        /// Gets the number of frames used to generate the waveform.
        /// </summary>
        public int FrameCount { get; init; }
    }

    /// <summary>
    /// Raised when spectrum analysis is complete.
    /// </summary>
    public class SpectrumAnalyzedEvent
    {
        /// <summary>
        /// Gets the analyzed spectrum data.
        /// </summary>
        public required SpectrumData Spectrum { get; init; }
        /// <summary>
        /// Gets the analysis time in milliseconds.
        /// </summary>
        public long AnalysisTimeMs { get; init; }
        /// <summary>
        /// Gets the peak magnitude value.
        /// </summary>
        public float PeakMagnitude { get; init; }
    }

    /// <summary>
    /// Raised when spectrogram data is generated.
    /// </summary>
    public class SpectrogramGeneratedEvent
    {
        /// <summary>
        /// Gets the generated spectrogram data.
        /// </summary>
        public required SpectrogramData Spectrogram { get; init; }
        /// <summary>
        /// Gets the generation time in milliseconds.
        /// </summary>
        public long GenerationTimeMs { get; init; }
        /// <summary>
        /// Gets the number of time frames processed.
        /// </summary>
        public int TimeFramesProcessed { get; init; }
    }

    /// <summary>
    /// Raised when visualization rendering begins.
    /// </summary>
    public class VisualizationRenderStartedEvent
    {
        /// <summary>
        /// Gets the type of visualization being rendered.
        /// </summary>
        public string VisualizationType { get; init; } = string.Empty;
        /// <summary>
        /// Gets the width of the visualization in pixels.
        /// </summary>
        public int Width { get; init; }
        /// <summary>
        /// Gets the height of the visualization in pixels.
        /// </summary>
        public int Height { get; init; }
        /// <summary>
        /// Gets the start time of the rendering.
        /// </summary>
        public DateTime StartTime { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Raised when visualization rendering completes.
    /// </summary>
    public class VisualizationRenderCompletedEvent
    {
        /// <summary>
        /// Gets the type of visualization that was rendered.
        /// </summary>
        public string VisualizationType { get; init; } = string.Empty;
        /// <summary>
        /// Gets the render time in milliseconds.
        /// </summary>
        public long RenderTimeMs { get; init; }
        /// <summary>
        /// Gets the frame rate of the rendering.
        /// </summary>
        public int FrameRate { get; init; }
        /// <summary>
        /// Gets the completion time of the rendering.
        /// </summary>
        public DateTime CompletionTime { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Raised when an error occurs in the visualization pipeline.
    /// </summary>
    public class VisualizationErrorEvent
    {
        /// <summary>
        /// Gets the error message.
        /// </summary>
        public required string ErrorMessage { get; init; }
        /// <summary>
        /// Gets the exception associated with the error.
        /// </summary>
        public required Exception Exception { get; init; }
        /// <summary>
        /// Gets the name of the component where the error occurred.
        /// </summary>
        public string ComponentName { get; init; } = string.Empty;
        /// <summary>
        /// Gets the error code.
        /// </summary>
        public int ErrorCode { get; init; }
        /// <summary>
        /// Gets the time when the error occurred.
        /// </summary>
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Raised when audio device is connected.
    /// </summary>
    public class AudioDeviceConnectedEvent
    {
        /// <summary>
        /// Gets the audio device ID.
        /// </summary>
        public int DeviceId { get; init; }
        /// <summary>
        /// Gets the name of the audio device.
        /// </summary>
        public required string DeviceName { get; init; }
        /// <summary>
        /// Gets the maximum number of channels supported by the device.
        /// </summary>
        public int MaxChannels { get; init; }
        /// <summary>
        /// Gets the time when the device was connected.
        /// </summary>
        public DateTime ConnectedAt { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Raised when audio device is disconnected.
    /// </summary>
    public class AudioDeviceDisconnectedEvent
    {
        /// <summary>
        /// Gets the audio device ID.
        /// </summary>
        public int DeviceId { get; init; }
        /// <summary>
        /// Gets the name of the audio device.
        /// </summary>
        public required string DeviceName { get; init; }
        /// <summary>
        /// Gets the time when the device was disconnected.
        /// </summary>
        public DateTime DisconnectedAt { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Raised when visualization settings are changed.
    /// </summary>
    public class VisualizationSettingsChangedEvent
    {
        /// <summary>
        /// Gets the name of the setting that was changed.
        /// </summary>
        public required string SettingName { get; init; }
        /// <summary>
        /// Gets the old value of the setting.
        /// </summary>
        public object? OldValue { get; init; }
        /// <summary>
        /// Gets the new value of the setting.
        /// </summary>
        public object? NewValue { get; init; }
        /// <summary>
        /// Gets the time when the setting was changed.
        /// </summary>
        public DateTime ChangedAt { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Raised when performance metrics are available.
    /// </summary>
    public class PerformanceMetricsEvent
    {
        /// <summary>
        /// Gets the CPU usage percentage.
        /// </summary>
        public double CpuUsagePercent { get; init; }
        /// <summary>
        /// Gets the memory usage in bytes.
        /// </summary>
        public long MemoryUsageBytes { get; init; }
        /// <summary>
        /// Gets the number of frames processed.
        /// </summary>
        public int FramesProcessed { get; init; }
        /// <summary>
        /// Gets the average frame time in milliseconds.
        /// </summary>
        public double AverageFrameTimeMs { get; init; }
        /// <summary>
        /// Gets the time when the metrics were recorded.
        /// </summary>
        public DateTime RecordedAt { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Raised when data export starts.
    /// </summary>
    public class DataExportStartedEvent
    {
        /// <summary>
        /// Gets the path where the data will be exported.
        /// </summary>
        public required string ExportPath { get; init; }
        /// <summary>
        /// Gets the export format (e.g., WAV, MP4).
        /// </summary>
        public required string Format { get; init; }
        /// <summary>
        /// Gets the number of data points to be exported.
        /// </summary>
        public int DataPointCount { get; init; }
        /// <summary>
        /// Gets the start time of the export.
        /// </summary>
        public DateTime StartTime { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Raised when data export completes.
    /// </summary>
    public class DataExportCompletedEvent
    {
        /// <summary>
        /// Gets the path where the data was exported.
        /// </summary>
        public required string ExportPath { get; init; }
        /// <summary>
        /// Gets the export format (e.g., WAV, MP4).
        /// </summary>
        public required string Format { get; init; }
        /// <summary>
        /// Gets the size of the exported file in bytes.
        /// </summary>
        public long FileSize { get; init; }
        /// <summary>
        /// Gets the export time in milliseconds.
        /// </summary>
        public long ExportTimeMs { get; init; }
        /// <summary>
        /// Gets whether the export was successful.
        /// </summary>
        public bool Success { get; init; }
        /// <summary>
        /// Gets the completion time of the export.
        /// </summary>
        public DateTime CompletionTime { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Raised when application is shutting down.
    /// </summary>
    public class ApplicationShuttingDownEvent
    {
        /// <summary>
        /// Gets the reason for the shutdown.
        /// </summary>
        public string Reason { get; init; } = string.Empty;
        /// <summary>
        /// Gets the uptime in milliseconds.
        /// </summary>
        public long UptimeMs { get; init; }
        /// <summary>
        /// Gets the shutdown time.
        /// </summary>
        public DateTime ShutdownTime { get; init; } = DateTime.UtcNow;
    }
}