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
/// Manages scheduled tasks that run at regular intervals.
/// Provides a lightweight scheduling mechanism for maintenance tasks and periodic operations.
/// </summary>
public class ScheduledTaskRunner : IDisposable
{
    private readonly Logger _logger;
    private readonly Dictionary<string, ScheduledTask> _tasks;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _runnerTask;
    private bool _isRunning;
    private readonly object _lockObject = new();

    /// <summary>
    /// Initializes a new instance of the scheduled task runner.
    /// </summary>
    public ScheduledTaskRunner(Logger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tasks = new Dictionary<string, ScheduledTask>();
    }

    /// <summary>
    /// Starts the scheduled task runner.
    /// </summary>
    public void Start()
    {
        lock (_lockObject)
        {
            if (_isRunning)
                return;

            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _runnerTask = RunSchedulerAsync(_cancellationTokenSource.Token);

            _logger.Info("Scheduled task runner started.");
        }
    }

    /// <summary>
    /// Stops the scheduled task runner gracefully.
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

        if (_runnerTask is not null)
        {
            try
            {
                await _runnerTask;
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
        }

        _logger.Info("Scheduled task runner stopped.");
    }

    /// <summary>
    /// Schedules a periodic task.
    /// </summary>
    public void ScheduleTask(string taskName, TimeSpan interval, Func<CancellationToken, Task> taskDelegate)
    {
        if (string.IsNullOrWhiteSpace(taskName))
            throw new ArgumentException("Task name cannot be null or empty.", nameof(taskName));

        if (interval <= TimeSpan.Zero)
            throw new ArgumentException("Interval must be greater than zero.", nameof(interval));

        if (taskDelegate is null)
            throw new ArgumentNullException(nameof(taskDelegate));

        lock (_lockObject)
        {
            var task = new ScheduledTask
            {
                Name = taskName,
                Interval = interval,
                TaskDelegate = taskDelegate,
                LastRunTime = null,
                NextRunTime = DateTime.UtcNow.Add(interval)
            };

            _tasks[taskName] = task;
            _logger.Info($"Scheduled task '{taskName}' with interval {interval.TotalSeconds:F0} seconds.");
        }
    }

    /// <summary>
    /// Removes a scheduled task.
    /// </summary>
    public bool UnscheduleTask(string taskName)
    {
        lock (_lockObject)
        {
            return _tasks.Remove(taskName);
        }
    }

    /// <summary>
    /// Gets the number of scheduled tasks.
    /// </summary>
    public int GetTaskCount()
    {
        lock (_lockObject)
        {
            return _tasks.Count;
        }
    }

    /// <summary>
    /// Gets information about all scheduled tasks.
    /// </summary>
    public List<TaskInfo> GetTaskInfo()
    {
        var taskList = new List<TaskInfo>();

        lock (_lockObject)
        {
            foreach (var kvp in _tasks)
            {
                taskList.Add(new TaskInfo
                {
                    Name = kvp.Value.Name,
                    Interval = kvp.Value.Interval,
                    LastRunTime = kvp.Value.LastRunTime,
                    NextRunTime = kvp.Value.NextRunTime,
                    ExecutionCount = kvp.Value.ExecutionCount,
                    LastExecutionTime = kvp.Value.LastExecutionTime
                });
            }
        }

        return taskList;
    }

    /// <summary>
    /// Main scheduler loop.
    /// </summary>
    private async Task RunSchedulerAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            List<string> tasksToRun = new();

            lock (_lockObject)
            {
                foreach (var kvp in _tasks)
                {
                    if (kvp.Value.NextRunTime <= now)
                        tasksToRun.Add(kvp.Key);
                }
            }

            // Execute tasks outside the lock
            foreach (var taskName in tasksToRun)
            {
                try
                {
                    ScheduledTask task;
                    lock (_lockObject)
                    {
                        if (!_tasks.TryGetValue(taskName, out task))
                            continue;
                    }

                    await ExecuteTaskAsync(task, cancellationToken).ConfigureAwait(false);

                    lock (_lockObject)
                    {
                        task.NextRunTime = DateTime.UtcNow.Add(task.Interval);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Scheduled task '{taskName}' failed: {ex.Message}");
                }
            }

            // Sleep briefly to avoid busy waiting
            await Task.Delay(100, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Executes a scheduled task with timing.
    /// </summary>
    private async Task ExecuteTaskAsync(ScheduledTask task, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            await task.TaskDelegate(cancellationToken).ConfigureAwait(false);
            stopwatch.Stop();

            task.LastRunTime = DateTime.UtcNow;
            task.ExecutionCount++;
            task.LastExecutionTime = stopwatch.ElapsedMilliseconds;

            _logger.Debug($"Task '{task.Name}' executed in {stopwatch.ElapsedMilliseconds}ms");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.Error($"Task '{task.Name}' failed after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (_isRunning)
            StopAsync().Wait(TimeSpan.FromSeconds(5));

        _cancellationTokenSource?.Dispose();
        _runnerTask?.Dispose();
    }
}

/// <summary>
/// Internal representation of a scheduled task.
/// </summary>
private class ScheduledTask
{
    public required string Name { get; init; }
    public required TimeSpan Interval { get; init; }
    public required Func<CancellationToken, Task> TaskDelegate { get; init; }
    public DateTime? LastRunTime { get; set; }
    public DateTime NextRunTime { get; set; }
    public long ExecutionCount { get; set; }
    public long LastExecutionTime { get; set; }
}

/// <summary>
/// Information about a scheduled task.
/// </summary>
public class TaskInfo
{
    public required string Name { get; init; }
    public required TimeSpan Interval { get; init; }
    public DateTime? LastRunTime { get; init; }
    public DateTime NextRunTime { get; init; }
    public long ExecutionCount { get; init; }
    public long LastExecutionTime { get; init; }
}
