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
    /// <param name="deviceId">The identifier of the audio device that started capturing.</param>
    /// <param name="sampleRate">The sample rate in Hz used for the capture session.</param>
    /// <param name="channelCount">The number of audio channels being captured.</param>
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
    /// <param name="deviceId">The identifier of the audio device that stopped capturing.</param>
    /// <param name="totalSamples">The total number of samples captured during the session.</param>
    /// <param name="duration">The total duration of the capture session.</param>
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
    /// <param name="frame">The audio frame that was captured.</param>
    /// <param name="sequenceNumber">The monotonically increasing sequence number of the frame.</param>
    /// <param name="elapsed">The elapsed time since the capture session started.</param>
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
    /// <param name="waveform">The waveform data that was generated.</param>
    /// <param name="generationTimeMs">The time taken to generate the waveform, in milliseconds.</param>
    /// <param name="frameCount">The number of frames included in the waveform.</param>
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
    /// <param name="spectrum">The spectrum data produced by the analysis.</param>
    /// <param name="analysisTimeMs">The time taken to analyze the spectrum, in milliseconds.</param>
    /// <param name="peakMagnitude">The peak magnitude detected in the spectrum.</param>
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
    /// <param name="spectrogram">The spectrogram data that was generated.</param>
    /// <param name="generationTimeMs">The time taken to generate the spectrogram, in milliseconds.</param>
    /// <param name="timeFrames">The number of time frames processed.</param>
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
    /// <param name="visualizationType">The name of the visualization type being rendered.</param>
    /// <param name="width">The width of the render surface, in pixels.</param>
    /// <param name="height">The height of the render surface, in pixels.</param>
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
    /// <param name="visualizationType">The name of the visualization type that was rendered.</param>
    /// <param name="renderTimeMs">The time taken to render the frame, in milliseconds.</param>
    /// <param name="frameRate">The achieved frame rate, in frames per second.</param>
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
    /// <param name="errorMessage">A human-readable description of the error.</param>
    /// <param name="exception">The exception that caused the error, if any.</param>
    /// <param name="componentName">The name of the component that raised the error.</param>
    /// <param name="errorCode">An optional numeric error code identifying the failure.</param>
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
    /// <param name="deviceId">The identifier of the connected audio device.</param>
    /// <param name="deviceName">The display name of the connected audio device.</param>
    /// <param name="maxChannels">The maximum number of channels supported by the device.</param>
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
    /// <param name="deviceId">The identifier of the disconnected audio device.</param>
    /// <param name="deviceName">The display name of the disconnected audio device.</param>
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
    /// <param name="settingName">The name of the setting that changed.</param>
    /// <param name="oldValue">The previous value of the setting.</param>
    /// <param name="newValue">The new value of the setting.</param>
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
    /// <param name="cpuUsage">The current CPU usage, as a percentage.</param>
    /// <param name="memoryBytes">The current memory usage, in bytes.</param>
    /// <param name="framesProcessed">The total number of frames processed.</param>
    /// <param name="avgFrameTimeMs">The average frame processing time, in milliseconds.</param>
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
    /// <param name="exportPath">The destination path of the export.</param>
    /// <param name="format">The identifier of the export format.</param>
    /// <param name="dataPointCount">The number of data points being exported.</param>
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
    /// <param name="exportPath">The destination path of the export.</param>
    /// <param name="format">The identifier of the export format.</param>
    /// <param name="fileSize">The size of the exported file, in bytes.</param>
    /// <param name="exportTimeMs">The time taken to perform the export, in milliseconds.</param>
    /// <param name="success">A value indicating whether the export completed successfully.</param>
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
    /// <param name="reason">The reason the application is shutting down.</param>
    /// <param name="uptimeMs">The application uptime at shutdown, in milliseconds.</param>
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
    /// <typeparam name="T">The type of event to subscribe to.</typeparam>
    /// <param name="handler">The action invoked whenever an event of type <typeparamref name="T"/> is published.</param>
    /// <returns>A disposable subscription handle; disposing it removes the subscription from the bus.</returns>
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
