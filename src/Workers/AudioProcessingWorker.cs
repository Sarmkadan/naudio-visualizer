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

namespace NAudioVisualizer.Workers;

/// <summary>
/// Background worker for processing audio frames and generating visualization data.
/// Runs asynchronously on a dedicated thread to avoid blocking the UI.
/// </summary>
public sealed class AudioProcessingWorker : IDisposable
{
    private readonly ILogger? _logger;
    private readonly Queue<ProcessingTask> _taskQueue;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _workerTask;
    private bool _isRunning;
    private readonly object _lockObject = new();

    /// <summary>
    /// Initializes a new instance of the audio processing worker.
    /// </summary>
    public AudioProcessingWorker(ILogger? logger = null)
    {
        _logger = logger;
        _taskQueue = new Queue<ProcessingTask>();
    }

    /// <summary>
    /// Starts the background worker.
    /// </summary>
    public void Start()
    {
        lock (_lockObject)
        {
            if (_isRunning)
                return;

            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _workerTask = ProcessQueueAsync(_cancellationTokenSource.Token);

            if (_logger is not null)
                _logger.Info("AudioProcessingWorker started (state=running, pollingInterval=10ms).");
        }
    }

    /// <summary>
    /// Stops the background worker gracefully.
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

        if (_logger is not null)
            _logger.Info("AudioProcessingWorker stopped (state=stopped).");
    }

    /// <summary>
    /// Enqueues a processing task.
    /// </summary>
    public void EnqueueTask(ProcessingTask task)
    {
        if (task is null)
            throw new ArgumentNullException(nameof(task));

        lock (_taskQueue)
        {
            _taskQueue.Enqueue(task);
        }
    }

    /// <summary>
    /// Gets the current queue depth.
    /// </summary>
    public int GetQueueDepth()
    {
        lock (_taskQueue)
        {
            return _taskQueue.Count;
        }
    }

    /// <summary>
    /// Clears all pending tasks from the queue.
    /// </summary>
    public int ClearQueue()
    {
        lock (_taskQueue)
        {
            int count = _taskQueue.Count;
            _taskQueue.Clear();

            if (count > 0 && _logger is not null)
                _logger.Warn($"AudioProcessingWorker cleared pending queue (state={(_isRunning ? "running" : "stopped")}, droppedTasks={count}).");

            return count;
        }
    }

    /// <summary>
    /// Main processing loop for the background worker.
    /// </summary>
    private async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            ProcessingTask? task = null;

            lock (_taskQueue)
            {
                if (_taskQueue.Count > 0)
                    task = _taskQueue.Dequeue();
            }

            if (task is not null)
            {
                try
                {
                    await ExecuteTaskAsync(task, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    if (_logger is not null)
                        _logger.Error($"AudioProcessingWorker processing cycle failed (state={(_isRunning ? "running" : "stopped")}, task='{task.Name}').", ex);
                }
            }
            else
            {
                // No tasks, wait briefly to avoid busy waiting
                await Task.Delay(10, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Executes a single processing task.
    /// </summary>
    private async Task ExecuteTaskAsync(ProcessingTask task, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            await task.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            stopwatch.Stop();

            if (_logger is not null)
                _logger.Debug($"AudioProcessingWorker task completed (state={(_isRunning ? "running" : "stopped")}, task='{task.Name}', elapsed={stopwatch.ElapsedMilliseconds}ms).");

            task.OnComplete?.Invoke();
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            if (_logger is not null)
                _logger.Error($"AudioProcessingWorker processing cycle failed (state={(_isRunning ? "running" : "stopped")}, task='{task.Name}', elapsed={stopwatch.ElapsedMilliseconds}ms).", ex);

            // Call error handler if provided
            task.OnError?.Invoke(ex);
        }
    }

    public void Dispose()
    {
        if (_isRunning)
            StopAsync().Wait(TimeSpan.FromSeconds(5));

        _cancellationTokenSource?.Dispose();
        _workerTask?.Dispose();

        if (_logger is not null)
            _logger.Info("AudioProcessingWorker disposed (state=disposed).");
    }
}

/// <summary>
/// Represents a processing task to be executed by the worker.
/// </summary>
public sealed class ProcessingTask
{
    public required string Name { get; init; }
    public required Func<CancellationToken, Task> ExecuteAsync { get; init; }
    public Action<Exception>? OnError { get; init; }
    public Action? OnComplete { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
