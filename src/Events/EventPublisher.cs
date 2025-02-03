#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;

namespace NAudioVisualizer.Events;

/// <summary>
/// Provides a convenience wrapper around EventBus for publishing events throughout the application.
/// This static facade simplifies event publishing by making it accessible from anywhere.
/// </summary>
public static class EventPublisher
{
    private static EventBus? _instance;
    private static readonly object LockObject = new();

    /// <summary>
    /// Gets or creates the global event bus instance.
    /// </summary>
    public static EventBus Instance
    {
        get
        {
            if (_instance is null)
            {
                lock (LockObject)
                {
                    _instance ??= new EventBus();
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// Publishes an audio capture started event.
    /// </summary>
    public static void PublishAudioCaptureStarted(int deviceId, int sampleRate, int channelCount)
    {
        Instance.Publish(new AudioCaptureStartedEvent
        {
            DeviceId = deviceId,
            SampleRate = sampleRate,
            ChannelCount = channelCount
        });
    }

    /// <summary>
    /// Publishes an audio capture stopped event.
    /// </summary>
    public static void PublishAudioCaptureStopped(int deviceId, long totalSamples, TimeSpan duration)
    {
        Instance.Publish(new AudioCaptureStoppedEvent
        {
            DeviceId = deviceId,
            TotalSamplesCaptured = totalSamples,
            Duration = duration
        });
    }

    /// <summary>
    /// Publishes an audio frame captured event.
    /// </summary>
    public static void PublishAudioFrameCaptured(Domain.Models.AudioFrame frame, long sequenceNumber, TimeSpan elapsed)
    {
        Instance.Publish(new AudioFrameCapturedEvent
        {
            Frame = frame,
            FrameSequenceNumber = sequenceNumber,
            ElapsedTime = elapsed
        });
    }

    /// <summary>
    /// Publishes a waveform generated event.
    /// </summary>
    public static void PublishWaveformGenerated(Domain.Models.WaveformData waveform, long generationTimeMs, int frameCount)
    {
        Instance.Publish(new WaveformGeneratedEvent
        {
            Waveform = waveform,
            GenerationTimeMs = generationTimeMs,
            FrameCount = frameCount
        });
    }

    /// <summary>
    /// Publishes a spectrum analyzed event.
    /// </summary>
    public static void PublishSpectrumAnalyzed(Domain.Models.SpectrumData spectrum, long analysisTimeMs, float peakMagnitude)
    {
        Instance.Publish(new SpectrumAnalyzedEvent
        {
            Spectrum = spectrum,
            AnalysisTimeMs = analysisTimeMs,
            PeakMagnitude = peakMagnitude
        });
    }

    /// <summary>
    /// Publishes a spectrogram generated event.
    /// </summary>
    public static void PublishSpectrogramGenerated(Domain.Models.SpectrogramData spectrogram, long generationTimeMs, int timeFrames)
    {
        Instance.Publish(new SpectrogramGeneratedEvent
        {
            Spectrogram = spectrogram,
            GenerationTimeMs = generationTimeMs,
            TimeFramesProcessed = timeFrames
        });
    }

    /// <summary>
    /// Publishes a visualization render started event.
    /// </summary>
    public static void PublishVisualizationRenderStarted(string visualizationType, int width, int height)
    {
        Instance.Publish(new VisualizationRenderStartedEvent
        {
            VisualizationType = visualizationType,
            Width = width,
            Height = height
        });
    }

    /// <summary>
    /// Publishes a visualization render completed event.
    /// </summary>
    public static void PublishVisualizationRenderCompleted(string visualizationType, long renderTimeMs, int frameRate)
    {
        Instance.Publish(new VisualizationRenderCompletedEvent
        {
            VisualizationType = visualizationType,
            RenderTimeMs = renderTimeMs,
            FrameRate = frameRate
        });
    }

    /// <summary>
    /// Publishes a visualization error event.
    /// </summary>
    public static void PublishVisualizationError(string errorMessage, Exception exception, string componentName, int errorCode = 0)
    {
        Instance.Publish(new VisualizationErrorEvent
        {
            ErrorMessage = errorMessage,
            Exception = exception,
            ComponentName = componentName,
            ErrorCode = errorCode
        });
    }

    /// <summary>
    /// Publishes an audio device connected event.
    /// </summary>
    public static void PublishAudioDeviceConnected(int deviceId, string deviceName, int maxChannels)
    {
        Instance.Publish(new AudioDeviceConnectedEvent
        {
            DeviceId = deviceId,
            DeviceName = deviceName,
            MaxChannels = maxChannels
        });
    }

    /// <summary>
    /// Publishes an audio device disconnected event.
    /// </summary>
    public static void PublishAudioDeviceDisconnected(int deviceId, string deviceName)
    {
        Instance.Publish(new AudioDeviceDisconnectedEvent
        {
            DeviceId = deviceId,
            DeviceName = deviceName
        });
    }

    /// <summary>
    /// Publishes a visualization settings changed event.
    /// </summary>
    public static void PublishVisualizationSettingsChanged(string settingName, object? oldValue, object? newValue)
    {
        Instance.Publish(new VisualizationSettingsChangedEvent
        {
            SettingName = settingName,
            OldValue = oldValue,
            NewValue = newValue
        });
    }

    /// <summary>
    /// Publishes performance metrics event.
    /// </summary>
    public static void PublishPerformanceMetrics(double cpuUsage, long memoryBytes, int framesProcessed, double avgFrameTimeMs)
    {
        Instance.Publish(new PerformanceMetricsEvent
        {
            CpuUsagePercent = cpuUsage,
            MemoryUsageBytes = memoryBytes,
            FramesProcessed = framesProcessed,
            AverageFrameTimeMs = avgFrameTimeMs
        });
    }

    /// <summary>
    /// Publishes a data export started event.
    /// </summary>
    public static void PublishDataExportStarted(string exportPath, string format, int dataPointCount)
    {
        Instance.Publish(new DataExportStartedEvent
        {
            ExportPath = exportPath,
            Format = format,
            DataPointCount = dataPointCount
        });
    }

    /// <summary>
    /// Publishes a data export completed event.
    /// </summary>
    public static void PublishDataExportCompleted(string exportPath, string format, long fileSize, long exportTimeMs, bool success)
    {
        Instance.Publish(new DataExportCompletedEvent
        {
            ExportPath = exportPath,
            Format = format,
            FileSize = fileSize,
            ExportTimeMs = exportTimeMs,
            Success = success
        });
    }

    /// <summary>
    /// Publishes an application shutting down event.
    /// </summary>
    public static void PublishApplicationShuttingDown(string reason, long uptimeMs)
    {
        Instance.Publish(new ApplicationShuttingDownEvent
        {
            Reason = reason,
            UptimeMs = uptimeMs
        });
    }

    /// <summary>
    /// Subscribes to events of a specific type.
    /// </summary>
    public static IDisposable Subscribe<T>(Action<T> handler) where T : class
    {
        return Instance.Subscribe(handler);
    }

    /// <summary>
    /// Resets the event bus (clears all subscriptions).
    /// </summary>
    public static void Reset()
    {
        _instance?.Clear();
    }
}
