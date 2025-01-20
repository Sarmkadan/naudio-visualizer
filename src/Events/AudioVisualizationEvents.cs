// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using NAudioVisualizer.Domain.Models;

namespace NAudioVisualizer.Events;

/// <summary>
/// Event classes for audio capture and visualization pipeline.
/// These events allow decoupled communication between different components.
/// </summary>

/// <summary>
/// Raised when audio capture starts.
/// </summary>
public class AudioCaptureStartedEvent
{
    public int DeviceId { get; init; }
    public int SampleRate { get; init; }
    public int ChannelCount { get; init; }
    public DateTime StartTime { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Raised when audio capture stops.
/// </summary>
public class AudioCaptureStoppedEvent
{
    public int DeviceId { get; init; }
    public long TotalSamplesCaptured { get; init; }
    public TimeSpan Duration { get; init; }
    public DateTime StopTime { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Raised when a new audio frame is captured.
/// </summary>
public class AudioFrameCapturedEvent
{
    public required AudioFrame Frame { get; init; }
    public long FrameSequenceNumber { get; init; }
    public TimeSpan ElapsedTime { get; init; }
}

/// <summary>
/// Raised when waveform data is generated.
/// </summary>
public class WaveformGeneratedEvent
{
    public required WaveformData Waveform { get; init; }
    public long GenerationTimeMs { get; init; }
    public int FrameCount { get; init; }
}

/// <summary>
/// Raised when spectrum analysis is complete.
/// </summary>
public class SpectrumAnalyzedEvent
{
    public required SpectrumData Spectrum { get; init; }
    public long AnalysisTimeMs { get; init; }
    public float PeakMagnitude { get; init; }
}

/// <summary>
/// Raised when spectrogram data is generated.
/// </summary>
public class SpectrogramGeneratedEvent
{
    public required SpectrogramData Spectrogram { get; init; }
    public long GenerationTimeMs { get; init; }
    public int TimeFramesProcessed { get; init; }
}

/// <summary>
/// Raised when visualization rendering begins.
/// </summary>
public class VisualizationRenderStartedEvent
{
    public string VisualizationType { get; init; } = string.Empty;
    public int Width { get; init; }
    public int Height { get; init; }
    public DateTime StartTime { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Raised when visualization rendering completes.
/// </summary>
public class VisualizationRenderCompletedEvent
{
    public string VisualizationType { get; init; } = string.Empty;
    public long RenderTimeMs { get; init; }
    public int FrameRate { get; init; }
    public DateTime CompletionTime { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Raised when an error occurs in the visualization pipeline.
/// </summary>
public class VisualizationErrorEvent
{
    public required string ErrorMessage { get; init; }
    public required Exception Exception { get; init; }
    public string ComponentName { get; init; } = string.Empty;
    public int ErrorCode { get; init; }
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Raised when audio device is connected.
/// </summary>
public class AudioDeviceConnectedEvent
{
    public int DeviceId { get; init; }
    public required string DeviceName { get; init; }
    public int MaxChannels { get; init; }
    public DateTime ConnectedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Raised when audio device is disconnected.
/// </summary>
public class AudioDeviceDisconnectedEvent
{
    public int DeviceId { get; init; }
    public required string DeviceName { get; init; }
    public DateTime DisconnectedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Raised when visualization settings are changed.
/// </summary>
public class VisualizationSettingsChangedEvent
{
    public required string SettingName { get; init; }
    public object? OldValue { get; init; }
    public object? NewValue { get; init; }
    public DateTime ChangedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Raised when performance metrics are available.
/// </summary>
public class PerformanceMetricsEvent
{
    public double CpuUsagePercent { get; init; }
    public long MemoryUsageBytes { get; init; }
    public int FramesProcessed { get; init; }
    public double AverageFrameTimeMs { get; init; }
    public DateTime RecordedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Raised when data export starts.
/// </summary>
public class DataExportStartedEvent
{
    public required string ExportPath { get; init; }
    public required string Format { get; init; }
    public int DataPointCount { get; init; }
    public DateTime StartTime { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Raised when data export completes.
/// </summary>
public class DataExportCompletedEvent
{
    public required string ExportPath { get; init; }
    public required string Format { get; init; }
    public long FileSize { get; init; }
    public long ExportTimeMs { get; init; }
    public bool Success { get; init; }
    public DateTime CompletionTime { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Raised when application is shutting down.
/// </summary>
public class ApplicationShuttingDownEvent
{
    public string Reason { get; init; } = string.Empty;
    public long UptimeMs { get; init; }
    public DateTime ShutdownTime { get; init; } = DateTime.UtcNow;
}
