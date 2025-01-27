// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using NAudioVisualizer.Events;
using NAudioVisualizer.Infrastructure;
using NAudioVisualizer.Integration;
using NAudioVisualizer.Workers;

namespace NAudioVisualizer.API;

/// <summary>
/// Configures API services and middleware for the visualization system.
/// Sets up event subscriptions, workers, and export services.
/// </summary>
public class VisualizationApiConfiguration
{
    private readonly Logger _logger;
    private readonly WebhookPublisher? _webhookPublisher;
    private readonly AudioProcessingWorker? _audioWorker;
    private readonly DataExportWorker? _exportWorker;
    private readonly ScheduledTaskRunner? _taskRunner;

    /// <summary>
    /// Initializes a new instance of the API configuration.
    /// </summary>
    public VisualizationApiConfiguration(Logger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Initializes with all optional services.
    /// </summary>
    public VisualizationApiConfiguration(
        Logger logger,
        WebhookPublisher? webhookPublisher,
        AudioProcessingWorker? audioWorker,
        DataExportWorker? exportWorker,
        ScheduledTaskRunner? taskRunner)
    {
        _logger = logger;
        _webhookPublisher = webhookPublisher;
        _audioWorker = audioWorker;
        _exportWorker = exportWorker;
        _taskRunner = taskRunner;
    }

    /// <summary>
    /// Configures event listeners for audio events.
    /// </summary>
    public void ConfigureEventListeners()
    {
        // Subscribe to audio frame captured events
        EventPublisher.Subscribe<AudioFrameCapturedEvent>(e =>
        {
            _logger.Debug($"Frame captured: sequence={e.FrameSequenceNumber}, elapsed={e.ElapsedTime.TotalMilliseconds:F0}ms");
        });

        // Subscribe to waveform generated events
        EventPublisher.Subscribe<WaveformGeneratedEvent>(e =>
        {
            _logger.Info($"Waveform generated in {e.GenerationTimeMs}ms with {e.FrameCount} frames");
        });

        // Subscribe to spectrum analyzed events
        EventPublisher.Subscribe<SpectrumAnalyzedEvent>(e =>
        {
            _logger.Info($"Spectrum analyzed in {e.AnalysisTimeMs}ms, peak magnitude: {e.PeakMagnitude:F2}");
        });

        // Subscribe to error events
        EventPublisher.Subscribe<VisualizationErrorEvent>(e =>
        {
            _logger.Error($"Visualization error in {e.ComponentName}: {e.ErrorMessage} (code: {e.ErrorCode})");
        });

        // Subscribe to export events
        EventPublisher.Subscribe<DataExportStartedEvent>(e =>
        {
            _logger.Info($"Data export started: {e.Format} format to {e.ExportPath}");
        });

        EventPublisher.Subscribe<DataExportCompletedEvent>(e =>
        {
            string status = e.Success ? "success" : "failed";
            _logger.Info($"Data export completed ({status}): {e.ExportPath} ({e.FileSize} bytes, {e.ExportTimeMs}ms)");
        });
    }

    /// <summary>
    /// Starts all configured background workers.
    /// </summary>
    public void StartWorkers()
    {
        _audioWorker?.Start();
        _exportWorker?.Start();
        _taskRunner?.Start();

        _logger.Info("API workers started.");
    }

    /// <summary>
    /// Stops all background workers gracefully.
    /// </summary>
    public async System.Threading.Tasks.Task StopWorkersAsync()
    {
        try
        {
            if (_audioWorker != null)
                await _audioWorker.StopAsync();

            if (_exportWorker != null)
                await _exportWorker.StopAsync();

            if (_taskRunner != null)
                await _taskRunner.StopAsync();

            _logger.Info("API workers stopped.");
        }
        catch (Exception ex)
        {
            _logger.Error($"Error stopping workers: {ex.Message}");
        }
    }

    /// <summary>
    /// Configures webhook endpoints for event delivery.
    /// </summary>
    public void ConfigureWebhooks(string? audioFrameWebhook = null, string? errorWebhook = null)
    {
        if (_webhookPublisher == null)
        {
            _logger.Warn("Webhook publisher not configured.");
            return;
        }

        if (!string.IsNullOrEmpty(audioFrameWebhook))
        {
            _webhookPublisher.Subscribe<AudioFrameCapturedEvent>(audioFrameWebhook);
            _logger.Info($"Configured webhook for audio frames: {audioFrameWebhook}");
        }

        if (!string.IsNullOrEmpty(errorWebhook))
        {
            _webhookPublisher.Subscribe<VisualizationErrorEvent>(errorWebhook);
            _logger.Info($"Configured webhook for errors: {errorWebhook}");
        }
    }

    /// <summary>
    /// Registers periodic maintenance tasks.
    /// </summary>
    public void RegisterMaintenanceTasks()
    {
        if (_taskRunner == null)
        {
            _logger.Warn("Task runner not configured.");
            return;
        }

        // Schedule cache cleanup task
        _taskRunner.ScheduleTask("cache-cleanup", TimeSpan.FromMinutes(5), async (ct) =>
        {
            _logger.Debug("Running cache cleanup task...");
            await System.Threading.Tasks.Task.CompletedTask;
        });

        // Schedule performance metrics collection task
        _taskRunner.ScheduleTask("performance-metrics", TimeSpan.FromSeconds(30), async (ct) =>
        {
            var cpuUsage = GetSystemCpuUsage();
            var memoryUsage = GetSystemMemoryUsage();
            EventPublisher.PublishPerformanceMetrics(cpuUsage, memoryUsage, 0, 0);
            await System.Threading.Tasks.Task.CompletedTask;
        });

        _logger.Info("Maintenance tasks registered.");
    }

    /// <summary>
    /// Gets current system CPU usage percentage.
    /// Placeholder implementation.
    /// </summary>
    private double GetSystemCpuUsage()
    {
        // In a real implementation, use PerformanceCounter or similar
        return 0.0;
    }

    /// <summary>
    /// Gets current system memory usage in bytes.
    /// Placeholder implementation.
    /// </summary>
    private long GetSystemMemoryUsage()
    {
        // In a real implementation, use GC.GetMemory or similar
        return GC.GetMemory(false);
    }

    /// <summary>
    /// Gets API health status.
    /// </summary>
    public ApiHealthStatus GetHealthStatus()
    {
        return new ApiHealthStatus
        {
            IsHealthy = true,
            AudioWorkerQueueDepth = _audioWorker?.GetQueueDepth() ?? 0,
            ExportJobsPending = _exportWorker?.GetPendingJobCount() ?? 0,
            ScheduledTaskCount = _taskRunner?.GetTaskCount() ?? 0,
            WebhookSubscriptions = _webhookPublisher?.GetSubscriptionCount() ?? 0
        };
    }
}

/// <summary>
/// API health status information.
/// </summary>
public class ApiHealthStatus
{
    public bool IsHealthy { get; init; }
    public int AudioWorkerQueueDepth { get; init; }
    public int ExportJobsPending { get; init; }
    public int ScheduledTaskCount { get; init; }
    public int WebhookSubscriptions { get; init; }
}
