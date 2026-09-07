# ProcessingTask

`ProcessingTask` represents one asynchronous work item consumed by [`AudioProcessingWorker`](AudioProcessingWorker.md). Each instance supplies a name, an asynchronous delegate, and optional callbacks for successful completion or failure. The class is sealed, and all of its properties are init-only.

## API

### Properties

*   **`public required string Name { get; init; }`**
    Gets the task name. Callers must set this property in an object initializer or constructor marked for required members. `AudioProcessingWorker` includes the name in task completion and error log messages; it does not use the name to schedule or identify tasks internally.

*   **`public required Func<CancellationToken, Task> ExecuteAsync { get; init; }`**
    Gets the asynchronous operation that performs the work. Callers must set this property. The worker invokes the delegate with its current cancellation token and awaits the returned `Task` before dequeuing another `ProcessingTask`.

*   **`public Action<Exception>? OnError { get; init; }`**
    Gets an optional failure callback. When `ExecuteAsync` throws, the worker invokes this callback with the exception. An `OperationCanceledException` thrown by the delegate is handled through this same path. The callback is also invoked if `OnComplete` throws, because completion callback invocation is inside the worker's task-execution `try` block.

*   **`public Action? OnComplete { get; init; }`**
    Gets an optional callback invoked after `ExecuteAsync` completes successfully. It is invoked once for that individual `ProcessingTask`; it does not indicate that the worker queue is empty.

*   **`public DateTime CreatedAt { get; init; } = DateTime.UtcNow`**
    Gets the task creation timestamp. By default, it is initialized from `DateTime.UtcNow`, but callers may provide another value during initialization. The worker does not inspect this property or use it for queue ordering.

## Usage

The following example creates a work item and submits it to a worker. The item carries its own operation and callbacks.

```csharp
using NAudioVisualizer.Workers;

using var worker = new AudioProcessingWorker();
var finished = new TaskCompletionSource<bool>();
worker.Start();

var task = new ProcessingTask
{
    Name = "Analyze captured samples",
    ExecuteAsync = async cancellationToken =>
    {
        cancellationToken.ThrowIfCancellationRequested();
        await AnalyzeSamplesAsync(cancellationToken);
    },
    OnComplete = () => finished.TrySetResult(true),
    OnError = exception => finished.TrySetException(exception)
};

worker.EnqueueTask(task);
await finished.Task;
Console.WriteLine("Analysis completed.");
await worker.StopAsync();
```

`StopAsync` cancels the token used by the processing loop. Consequently, the example operation checks the supplied token and passes it to its asynchronous work. Whether an operation stops promptly depends on how its `ExecuteAsync` delegate observes that token.

## How AudioProcessingWorker Consumes ProcessingTask

*   **Queueing**: `EnqueueTask` rejects `null` with `ArgumentNullException`, then appends the task to an internal queue under a lock. Tasks may be enqueued before `Start`; the code does not require the worker to be running when enqueueing.
*   **Ordering**: The worker dequeues tasks in FIFO order and processes one at a time. It awaits the current task's `ExecuteAsync` delegate before retrieving the next queued task.
*   **Idle behavior**: When no task is available, the processing loop waits for 10 milliseconds before checking the queue again.
*   **Cancellation**: The same worker cancellation token is passed to every `ExecuteAsync` invocation. `StopAsync` requests cancellation, and the loop stops without dequeuing further work. A delegate must observe the token if it is to interrupt its own work.
*   **Successful execution**: After the delegate completes, the worker logs elapsed time when a logger is present and invokes `OnComplete` if supplied.
*   **Failed execution**: If the delegate or `OnComplete` throws, the worker logs the failure when a logger is present and invokes `OnError` if supplied. The worker then continues its processing loop unless cancellation ends the loop or a callback itself lets an exception escape.
*   **Clearing pending work**: `ClearQueue` removes queued tasks but does not affect a task that has already been dequeued and is executing. No completion or error callback is invoked for removed tasks.
*   **Metadata**: Only `Name`, `ExecuteAsync`, `OnComplete`, and `OnError` are read during execution. `CreatedAt` remains caller-facing metadata.

## Notes

*   `required` on `Name` and `ExecuteAsync` provides compile-time initialization enforcement; the class does not define additional runtime validation for either value.
*   Property values cannot be reassigned after object initialization because every property uses an `init` accessor.
*   Exceptions thrown by `OnError` are not caught inside the callback invocation. The surrounding processing loop catches them, logs when possible, and continues unless the exception is an `OperationCanceledException`, which ends the loop.
