// README.md
## PathUtility

`PathUtility` is a utility class for managing file paths and directories. It provides static methods for path normalization, validation, and manipulation, as well as directory operations and file existence checks.

### Usage Example

```csharp
using Utilities;

// Normalize a path
string normalizedPath = PathUtility.NormalizePath("C:/some/path");

// Combine paths
string combinedPath = PathUtility.Combine("C:/base", "subdir", "file.txt");

// Get absolute path
string absolutePath = PathUtility.GetAbsolutePath("relative/path");

// Get relative path
string relativePath = PathUtility.GetRelativePath("C:/base", "C:/base/subdir/file.txt");

// Ensure trailing path separator
string ensuredPath = PathUtility.EnsureTrailingSeparator("C:/some/path");

// Remove trailing path separator
string removedPath = PathUtility.RemoveTrailingSeparator("C:/some/path/");

// Check if path is absolute or relative
bool isAbsolute = PathUtility.IsAbsolute("C:/some/path");
bool isRelative = PathUtility.IsRelative("relative/path");

// Get files recursively
string[] files = PathUtility.GetFilesRecursive("C:/some/directory");

// Check if path exists
bool exists = PathUtility.IsValidPath("C:/some/existing/path");

// Get directory size
long size = PathUtility.GetDirectorySize("C:/some/directory");

// Generate unique file name
string uniqueName = PathUtility.GenerateUniqueFileName("C:/some/existing/file.txt");

// Get application directory
string appDir = PathUtility.GetApplicationDirectory();

// Get application data directory
string appDataDir = PathUtility.GetApplicationDataDirectory();

// Get logs directory
string logsDir = PathUtility.GetLogsDirectory();

// Get temp directory
string tempDir = PathUtility.GetTempDirectory();
```

## PerformanceProfiler

`PerformanceProfiler` is a utility class for tracking and analyzing the execution time of operations. It provides comprehensive performance metrics including total, average, minimum, maximum, and median execution times, along with call counts. The profiler supports both manual timing with `RecordTime` and automatic timing using the disposable `TimingToken` pattern.


### Usage Example

```csharp
using NAudioVisualizer.Utilities;

// Create a profiler instance
var profiler = new PerformanceProfiler("AudioProcessingSession");

// Manual timing approach
profiler.RecordTime("AudioFileLoad", 150);
profiler.RecordTime("AudioFileLoad", 165);
profiler.RecordTime("AudioFileLoad", 142);

// Using the disposable TimingToken (recommended)
using (profiler.StartTimer("AudioProcessing"))
{
    // Simulate audio processing work
    await Task.Delay(200);
}

using (profiler.StartTimer("AudioFileSave"))
{
    // Simulate file saving work
    await Task.Delay(85);
}

// Retrieve performance metrics
Console.WriteLine(profiler.GetReport());

// Get specific metrics
int callCount = profiler.GetCallCount("AudioFileLoad");
double averageTime = profiler.GetAverageTime("AudioFileLoad");
long totalTime = profiler.GetTotalTime("AudioProcessing");
long minTime = profiler.GetMinTime("AudioFileLoad");
long maxTime = profiler.GetMaxTime("AudioFileLoad");
long medianTime = profiler.GetMedianTime("AudioFileLoad");

// Clear all recorded metrics
profiler.Clear();
```

## StringUtility

`StringUtility` is a utility class for string manipulation and formatting. It provides static methods for truncating strings, repeating text, padding strings to specific widths, case conversion, whitespace removal, and formatting numbers for human-readable display.



### Usage Example

```csharp
using NAudioVisualizer.Utilities;

// Truncate a string with ellipsis
string truncated = StringUtility.Truncate("This is a very long string that needs to be shortened", 20);
Console.WriteLine(truncated); // "This is a very lon..."

// Repeat a string multiple times
string repeated = StringUtility.Repeat("naudio-", 3);
Console.WriteLine(repeated); // "naudio-naudio-naudio-"

// Pad a string to center it
string centered = StringUtility.PadCenter("Hello", 11, '-');
Console.WriteLine(centered); // "---Hello----"

// Format bytes to human-readable format
string fileSize = StringUtility.FormatBytes(15728640); // 15MB
Console.WriteLine(fileSize);

// Format milliseconds to time string
string duration = StringUtility.FormatMilliseconds(150000); // "2m 30s"
Console.WriteLine(duration);

// Format large numbers with suffixes
string formattedNumber = StringUtility.FormatLargeNumber(1500000); // "1.5M"
Console.WriteLine(formattedNumber);

// Convert to title case
string titleCase = StringUtility.ToTitleCase("hello world");
Console.WriteLine(titleCase); // "Hello World"

// Convert to snake_case
string snakeCase = StringUtility.ToSnakeCase("HelloWorld");
Console.WriteLine(snakeCase); // "hello_world"

// Convert to camelCase
string camelCase = StringUtility.ToCamelCase("hello_world");
Console.WriteLine(camelCase); // "helloWorld");

// Remove whitespace from a string
string noWhitespace = StringUtility.RemoveWhitespace("Hello  World  Test");
Console.WriteLine(noWhitespace); // "HelloWorldTest");

// Check if string is alphanumeric
bool isAlphanumeric = StringUtility.IsAlphanumeric("Hello123");
Console.WriteLine(isAlphanumeric); // true

// Count occurrences of a substring
int count = StringUtility.CountOccurrences("hello hello world", "hello");
Console.WriteLine(count); // 2
```

## DateTimeUtility

`DateTimeUtility` provides a collection of static helpers for working with dates and times, including timestamp conversions, ISO‑8601 formatting, duration formatting, and common calendar calculations.

### Usage Example

```csharp
using NAudioVisualizer.Utilities;

// Current timestamp in milliseconds since the Unix epoch
long nowMs = DateTimeUtility.GetCurrentTimestampMs();

// Convert a timestamp back to a DateTime
DateTime now = DateTimeUtility.FromTimestampMs(nowMs);

// Format the DateTime as an ISO‑8601 string and parse it back
string iso = DateTimeUtility.ToIso8601(now);
DateTime parsed = DateTimeUtility.FromIso8601(iso);

// Format a TimeSpan as a human‑readable duration
string duration = DateTimeUtility.FormatDuration(TimeSpan.FromMinutes(2.5));

// Calendar calculations
int days = DateTimeUtility.DaysBetween(DateTime.Today, DateTime.Today.AddDays(10));
bool today = DateTimeUtility.IsToday(DateTime.Today);
bool past = DateTimeUtility.IsInPast(DateTime.UtcNow.AddHours(-1));
bool future = DateTimeUtility.IsInFuture(DateTime.UtcNow.AddHours(1));
string relative = DateTimeUtility.GetRelativeTime(DateTime.UtcNow.AddHours(-3));

// Start/end of periods
DateTime startDay = DateTimeUtility.GetStartOfDay(DateTime.Now);
DateTime endDay = DateTimeUtility.GetEndOfDay(DateTime.Now);
DateTime startWeek = DateTimeUtility.GetStartOfWeek(DateTime.Now);
DateTime startMonth = DateTimeUtility.GetStartOfMonth(DateTime.Now);
DateTime endMonth = DateTimeUtility.GetEndOfMonth(DateTime.Now);

// Additional helpers
int age = DateTimeUtility.CalculateAge(new DateTime(1990, 5, 15));
string dayName = DateTimeUtility.GetDayName(DateTime.Now);
bool leapYear = DateTimeUtility.IsLeapYear(2024);
```
