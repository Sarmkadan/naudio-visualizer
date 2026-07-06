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
    private readonly Logger _logger;
    private readonly Queue<ProcessingTask> _taskQueue;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _workerTask;
    private bool _isRunning;
    private readonly object _lockObject = new();

    /// <summary>
    /// Initializes a new instance of the audio processing worker.
    /// </summary>
    public AudioProcessingWorker(Logger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            _logger.Info("Audio processing worker started.");
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

        _logger.Info("Audio processing worker stopped.");
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
                    _logger.Error($"Error processing task: {ex.Message}");
                }
            }
            else
            {
                // No tasks, sleep briefly to avoid busy waiting
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

            _logger.Debug($"Task '{task.Name}' completed in {stopwatch.ElapsedMilliseconds}ms");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.Error($"Task '{task.Name}' failed after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}");

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
