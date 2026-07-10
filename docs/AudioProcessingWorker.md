# AudioProcessingWorker

`AudioProcessingWorker` is a dedicated asynchronous task processor designed for sequential execution of audio processing operations. It accepts work items via `EnqueueTask`, runs them one at a time on a background thread, and provides lifecycle management through `Start`, `StopAsync`, and `Dispose`. The worker supports cancellation, error reporting, and completion notification, making it suitable for pipelines where ordered, non-overlapping processing of audio data is required.

## API

### `public AudioProcessingWorker`

Creates a new instance of the worker. The worker is not started upon construction; call `Start` to begin processing queued tasks.

### `public required string Name`

A human-readable identifier for the worker instance. Must be set during initialization. Used for diagnostic and logging purposes.

### `public required Func<CancellationToken, Task> ExecuteAsync`

The delegate invoked for each queued work item. Receives a `CancellationToken` that signals when the worker is stopping or disposing. The delegate must return a `Task` representing the asynchronous work. Set during initialization.

### `public Action<Exception>? OnError`

Optional callback invoked when `ExecuteAsync` throws an unhandled exception. Receives the thrown `Exception`. If null, exceptions are silently swallowed (though the worker continues processing subsequent tasks).

### `public Action? OnComplete`

Optional callback invoked when the worker has finished processing all queued tasks and the queue becomes empty. Not called during shutdown via `StopAsync` or `Dispose` unless the queue drains naturally before stopping.

### `public DateTime CreatedAt`

The UTC timestamp when the instance was constructed. Read-only after creation.

### `public void Start()`

Begins processing tasks from the queue on a background execution loop. If the worker is already running, calling `Start` again has no effect. Does not return a value.

### `public async Task StopAsync()`

Signals the worker to cease processing after the current executing task completes. Returns a `Task` that completes when the background loop has exited and no further tasks will be dequeued. Any tasks remaining in the queue at the time of the call will not be executed. Safe to call multiple times; subsequent calls return immediately if already stopped.

### `public void EnqueueTask()`

Adds a work item to the internal queue for execution. The queued item is the `ExecuteAsync` delegate configured on the instance. If the worker has been stopped or disposed, the task may be silently discarded or rejected depending on internal state. Does not return a value.

### `public int GetQueueDepth()`

Returns the number of pending tasks currently in the queue. Does not include the task actively being executed, if any. Returns `0` if the worker is stopped or disposed.

### `public int ClearQueue()`

Removes all pending tasks from the queue without executing them. Returns the number of tasks that were removed. The currently executing task, if any, is unaffected and will run to completion.

### `public void Dispose()`

Releases all resources held by the worker. Internally calls `StopAsync` and blocks until the worker has fully stopped. After disposal, no further tasks can be enqueued, and all other methods except `GetQueueDepth` (which returns `0`) should be considered invalid to call. Must be called to avoid resource leaks.

## Usage

### Example 1: Basic sequential audio file processing

```csharp
var worker = new AudioProcessingWorker
{
    Name = "FileProcessor",
    ExecuteAsync = async (token) =>
    {
        // Simulate processing an audio file chunk
        await Task.Delay(500, token);
        Console.WriteLine("Processed chunk");
    },
    OnError = (ex) => Console.WriteLine($"Error: {ex.Message}"),
    OnComplete = () => Console.WriteLine("All tasks completed")
};

worker.Start();

// Enqueue multiple work items
for (int i = 0; i < 10; i++)
{
    worker.EnqueueTask();
}

// Wait for natural completion
while (worker.GetQueueDepth() > 0)
{
    await Task.Delay(100);
}

await worker.StopAsync();
worker.Dispose();
```

### Example 2: Cancellation and queue management during live audio capture

```csharp
var worker = new AudioProcessingWorker
{
    Name = "CaptureProcessor",
    ExecuteAsync = async (token) =>
    {
        // Process captured audio buffer; respects cancellation
        for (int i = 0; i < 100; i++)
        {
            token.ThrowIfCancellationRequested();
            await Task.Delay(10, token);
        }
    },
    OnError = (ex) =>
    {
        if (ex is OperationCanceledException)
            Console.WriteLine("Task cancelled during shutdown");
        else
            Console.WriteLine($"Unexpected error: {ex.Message}");
    }
};

worker.Start();

// Enqueue tasks as audio chunks arrive
for (int i = 0; i < 5; i++)
{
    worker.EnqueueTask();
}

// Mid-processing, decide to clear pending work
int removed = worker.ClearQueue();
Console.WriteLine($"Cleared {removed} pending tasks");

// Graceful shutdown
await worker.StopAsync();
worker.Dispose();
```

## Notes

- **Execution order**: Tasks are processed strictly in FIFO order. A task begins only after the previous task's `ExecuteAsync` delegate has completed.
- **Thread safety**: `EnqueueTask`, `GetQueueDepth`, and `ClearQueue` are safe to call from any thread. `Start`, `StopAsync`, and `Dispose` should be called from a single controlling thread to avoid race conditions in lifecycle transitions.
- **Exception handling**: If `ExecuteAsync` throws and `OnError` is null, the exception is silently consumed and the worker proceeds to the next task. If `OnError` itself throws, that secondary exception may escape and destabilize the worker; clients should guard against this.
- **StopAsync behavior**: Calling `StopAsync` does not cancel the currently executing task; it only prevents new tasks from being dequeued. The executing task will run to completion unless it observes the `CancellationToken` and cooperatively cancels.
- **Dispose semantics**: `Dispose` synchronously blocks until the worker stops. To avoid deadlocks, do not call `Dispose` from within `ExecuteAsync` or from `OnError`/`OnComplete` callbacks that may be invoked on the worker's processing thread.
- **Queue depth after stop**: Once `StopAsync` completes, `GetQueueDepth` returns `0` regardless of whether tasks were cleared or simply left unprocessed. `ClearQueue` after stopping returns `0`.
- **Multiple starts**: Calling `Start` on an already-running worker is a no-op. Calling `Start` after `StopAsync` or `Dispose` is not supported and may result in undefined behavior.
- **Resource cleanup**: Always pair a started worker with a corresponding `Dispose` call to release internal synchronization primitives and background task resources.
