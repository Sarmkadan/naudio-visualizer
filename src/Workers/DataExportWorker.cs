#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NAudioVisualizer.Infrastructure;
using NAudioVisualizer.Integration;

namespace NAudioVisualizer.Workers;

/// <summary>
/// Background worker for handling data export operations asynchronously.
/// Prevents export operations from blocking the main UI thread.
/// </summary>
public class DataExportWorker : IDisposable
{
    private readonly Logger _logger;
    private readonly ExportService _exportService;
    private readonly Queue<ExportJob> _exportQueue;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _workerTask;
    private bool _isRunning;
    private readonly object _lockObject = new();

    /// <summary>
    /// Initializes a new instance of the data export worker.
    /// </summary>
    public DataExportWorker(Logger logger, ExportService exportService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
        _exportQueue = new Queue<ExportJob>();
    }

    /// <summary>
    /// Starts the export worker.
    /// </summary>
    public void Start()
    {
        lock (_lockObject)
        {
            if (_isRunning)
                return;

            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _workerTask = ProcessExportsAsync(_cancellationTokenSource.Token);

            _logger.Info("Data export worker started.");
        }
    }

    /// <summary>
    /// Stops the export worker gracefully.
    /// </summary>
    public async Task StopAsync()
    {
        lock (_lockObject)
        {
            if (!_isRunning)
                return;

            _isRunning = false;
            _cancellationTokenSource?.Cancel();
        }

        if (_workerTask is not null)
        {
            try
            {
                await _workerTask;
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
        }

        _logger.Info("Data export worker stopped.");
    }

    /// <summary>
    /// Queues a waveform export job.
    /// </summary>
    public async Task<string> QueueWaveformExportAsync(Domain.Models.WaveformData waveform, string filePath, string format)
    {
        var job = new ExportJob
        {
            Id = Guid.NewGuid().ToString(),
            Type = ExportType.Waveform,
            FilePath = filePath,
            Format = format,
            Data = waveform,
            CreatedAt = DateTime.UtcNow
        };

        lock (_exportQueue)
        {
            _exportQueue.Enqueue(job);
        }

        _logger.Debug($"Queued waveform export: {job.Id}");
        return job.Id;
    }

    /// <summary>
    /// Queues a spectrum export job.
    /// </summary>
    public async Task<string> QueueSpectrumExportAsync(Domain.Models.SpectrumData spectrum, string filePath, string format)
    {
        var job = new ExportJob
        {
            Id = Guid.NewGuid().ToString(),
            Type = ExportType.Spectrum,
            FilePath = filePath,
            Format = format,
            Data = spectrum,
            CreatedAt = DateTime.UtcNow
        };

        lock (_exportQueue)
        {
            _exportQueue.Enqueue(job);
        }

        _logger.Debug($"Queued spectrum export: {job.Id}");
        return job.Id;
    }

    /// <summary>
    /// Queues a spectrogram export job.
    /// </summary>
    public async Task<string> QueueSpectrogramExportAsync(Domain.Models.SpectrogramData spectrogram, string filePath, string format)
    {
        var job = new ExportJob
        {
            Id = Guid.NewGuid().ToString(),
            Type = ExportType.Spectrogram,
            FilePath = filePath,
            Format = format,
            Data = spectrogram,
            CreatedAt = DateTime.UtcNow
        };

        lock (_exportQueue)
        {
            _exportQueue.Enqueue(job);
        }

        _logger.Debug($"Queued spectrogram export: {job.Id}");
        return job.Id;
    }

    /// <summary>
    /// Gets the number of pending export jobs.
    /// </summary>
    public int GetPendingJobCount()
    {
        lock (_exportQueue)
        {
            return _exportQueue.Count;
        }
    }

    /// <summary>
    /// Main processing loop for export operations.
    /// </summary>
    private async Task ProcessExportsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            ExportJob? job = null;

            lock (_exportQueue)
            {
                if (_exportQueue.Count > 0)
                    job = _exportQueue.Dequeue();
            }

            if (job is not null)
            {
                try
                {
                    await ExecuteExportJobAsync(job, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Export job {job.Id} failed: {ex.Message}");
                }
            }
            else
            {
                // No jobs, sleep briefly
                await Task.Delay(100, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Executes a single export job.
    /// </summary>
    private async Task ExecuteExportJobAsync(ExportJob job, CancellationToken cancellationToken)
    {
        _logger.Info($"Processing export job: {job.Id}");

        try
        {
            switch (job.Type)
            {
                case ExportType.Waveform:
                    if (job.Data is Domain.Models.WaveformData waveform)
                        await _exportService.ExportWaveformAsync(waveform, job.FilePath, job.Format);
                    break;

                case ExportType.Spectrum:
                    if (job.Data is Domain.Models.SpectrumData spectrum)
                        await _exportService.ExportSpectrumAsync(spectrum, job.FilePath, job.Format);
                    break;

                case ExportType.Spectrogram:
                    if (job.Data is Domain.Models.SpectrogramData spectrogram)
                        await _exportService.ExportSpectrogramAsync(spectrogram, job.FilePath, job.Format);
                    break;
            }

            var duration = DateTime.UtcNow - job.CreatedAt;
            _logger.Info($"Export job {job.Id} completed in {duration.TotalMilliseconds:F0}ms");
        }
        catch (Exception ex)
        {
            _logger.Error($"Export job {job.Id} error: {ex.Message}");
            throw;
        }
    }

    public void Dispose()
    {
        if (_isRunning)
            StopAsync().Wait(TimeSpan.FromSeconds(10));

        _cancellationTokenSource?.Dispose();
        _workerTask?.Dispose();
    }
}

/// <summary>
/// Represents an export job to be processed.
/// </summary>
private class ExportJob
{
    public required string Id { get; init; }
    public required ExportType Type { get; init; }
    public required string FilePath { get; init; }
    public required string Format { get; init; }
    public required object Data { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Enum for export job types.
/// </summary>
private enum ExportType
{
    Waveform,
    Spectrum,
    Spectrogram
}
