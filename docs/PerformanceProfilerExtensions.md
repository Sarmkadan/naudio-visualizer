# PerformanceProfilerExtensions

Provides convenience methods for measuring operations with a <see cref="PerformanceProfiler"/>.

## API

### `public static T Measure<T>(this PerformanceProfiler profiler, string operationName, Func<T> action)`
Measures an operation and returns its result.

- **Parameters**
  - `profiler`: The profiler used to measure the operation. Must not be `null`.
  - `operationName`: The name of the operation to measure.
  - `action`: The operation to execute. Must not be `null`.
- **Return value**: The result returned by <paramref name="action"/>.
- **Exceptions**
  - `ArgumentNullException` if <paramref name="profiler"/> or <paramref name="action"/> is <see langword="null"/>.

## Usage

```csharp
using NAudioVisualizer.Utilities; // namespace containing the extensions

// Example: measuring a simple operation
var profiler = new PerformanceProfiler();
int result = profiler.Measure("CalculateSum", () =>
{
    int sum = 0;
    for (int i = 0; i < 1000; i++)
    {
        sum += i;
    }
    return sum;
});

// result contains the sum, and the operation time is recorded in the profiler
```

## Notes

- The method uses a `using` statement with the timer returned by `profiler.StartTimer(operationName)`, ensuring the timer is stopped even if an exception occurs.
- The extension method is thread-safe provided the <see cref="PerformanceProfiler"/> instance is thread-safe; it only reads the supplied arguments and returns a new value.
- The method returns the result of the operation, allowing it to be used in expressions where the result is needed.
- If the operation throws an exception, the timer is still stopped and the exception is propagated to the caller.