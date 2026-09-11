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

## PathUtilityValidation

`PathUtilityValidation` provides validation helpers for PathUtility operations. Each method validates a specific PathUtility function and returns a list of validation problems (empty if valid).

### Public Methods

- `ValidateNormalizePath(string? path)` - Validates path normalization edge cases (null, empty, backslash conversion)
- `ValidateCombine(IEnumerable<string> segments)` - Validates path combination operations (null, empty, valid segments)
- `ValidateGetAbsolutePath(string relativePath)` - Validates absolute path conversion (null, empty, valid relative path)
- `ValidateGetRelativePath(string fromPath, string toPath)` - Validates relative path calculation (null/empty inputs, valid paths)
- `ValidateEnsureTrailingSeparator(string? path)` - Validates trailing separator addition (null, empty, add/preserve separator)
- `ValidateRemoveTrailingSeparator(string? path)` - Validates trailing separator removal (null, empty, remove/preserve separator)
- `ValidateIsAbsolute(string? path)` - Validates absolute path detection (null, empty, absolute/relative paths)
- `ValidateIsRelative(string? path)` - Validates relative path detection (null, empty, relative/absolute paths)
- `ValidateGetFilesRecursive(string? directoryPath)` - Validates recursive file enumeration (null, empty, non-existent directory)
- `ValidateGetApplicationDirectory()` - Validates application directory retrieval (non-empty, existing directory)
- `ValidateGetApplicationDataDirectory()` - Validates application data directory retrieval (non-empty, existing, contains "NAudioVisualizer")
- `ValidateGetLogsDirectory()` - Validates logs directory retrieval (non-empty, existing, contains "logs")
- `ValidateGetTempDirectory()` - Validates temp directory retrieval (non-empty, existing, contains "temp")
- `ValidateIsValidPath(string? path)` - Validates path validity checking (null, empty, valid path)
- `ValidateGetDirectorySize(string? directoryPath)` - Validates directory size calculation (null, empty, non-existent, valid directory)
- `ValidateGenerateUniqueFileName(string filePath)` - Validates unique filename generation (null, empty, non-existent/existing files)
- `IsValid(this IReadOnlyList<string>? problems)` - Extension method to check if validation passed (no problems)
- `EnsureValid(this IReadOnlyList<string>? problems)` - Extension method to throw ArgumentException if validation failed

### Usage Example

```csharp
using NAudioVisualizer.Utilities;

// Validate path normalization
var normalizeProblems = PathUtilityValidation.ValidateNormalizePath("C:\\Test\\File.txt");
if (normalizeProblems.IsValid())
{
    Console.WriteLine("Path normalization is valid");
}

// Validate path combination
var combineProblems = PathUtilityValidation.ValidateCombine(new[] { "folder", "subfolder", "file.txt" });
if (combineProblems.IsValid())
{
    Console.WriteLine("Path combination is valid");
}

// Validate absolute path conversion
var absoluteProblems = PathUtilityValidation.ValidateGetAbsolutePath("test.txt");
if (absoluteProblems.IsValid())
{
    Console.WriteLine("Absolute path conversion is valid");
}

// Using EnsureValid to throw on validation failure
try
{
    PathUtilityValidation.ValidateNormalizePath(null).EnsureValid();
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Validation failed: {ex.Message}");
}
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

## AudioDeviceExtensions

`AudioDeviceExtensions` provides extension methods for the `AudioDevice` class, offering convenient ways to query device properties and capabilities. These methods simplify common audio device operations such as checking supported sample rates, retrieving device metadata, and determining device availability.

### Usage Example

```csharp
using NAudioVisualizer.Domain.Models;

// Example: Querying audio device properties and capabilities
var audioDevices = AudioDevice.GetAvailableDevices();

foreach (var device in audioDevices)
{
    // Check if device is available
    bool isAvailable = device.IsAvailable();
    Console.WriteLine($"Device '{device.GetName()}' is {(isAvailable ? "available" : "unavailable")}");
    
    // Get basic device information
    Console.WriteLine($"  Manufacturer: {device.GetManufacturer()}");
    Console.WriteLine($"  Channels: {device.GetChannelCount()}");
    Console.WriteLine($"  Bit Depth: {device.GetBitDepth()} bits");
    Console.WriteLine($"  Default Sample Rate: {device.GetDefaultSampleRate()} Hz");
    
    // Check if device is the default system device
    bool isDefault = device.IsDefaultDevice();
    Console.WriteLine($"  Is Default Device: {isDefault}");
    
    // Get all supported sample rates
    var supportedSampleRates = device.GetSupportedSampleRates();
    Console.WriteLine($"  Supported Sample Rates: {string.Join(", ", supportedSampleRates)}");
    
    // Check if a specific sample rate is supported
    bool supports48kHz = device.IsSampleRateSupported(48000);
    Console.WriteLine($"  Supports 48kHz: {supports48kHz}");
    
    bool supports192kHz = device.IsSampleRateSupported(192000);
    Console.WriteLine($"  Supports 192kHz: {supports192kHz}");
    
    // Get device capabilities
    var capabilities = device.GetCapabilities();
    Console.WriteLine($"  Capabilities: {capabilities}");
}

// Example: Filtering devices by capabilities
var inputDevices = audioDevices.Where(d => d.GetCapabilities().HasFlag(DeviceCapabilities.Input));
Console.WriteLine($"Found {inputDevices.Count()} input devices");

// Example: Finding a device with specific requirements
var suitableDevice = audioDevices.FirstOrDefault(d => 
    d.IsAvailable() && 
    d.IsSampleRateSupported(44100) && 
    d.GetChannelCount() >= 2);
```

## ValidationUtility

`ValidationUtility` provides comprehensive validation methods for common audio processing parameters and data validation scenarios. It centralizes validation logic to ensure consistency across the application, with both boolean validation methods and exception-throwing variants for different use cases.

### Usage Example

```csharp
using NAudioVisualizer.Utilities;

// Validate audio processing parameters
bool isValidSampleRate = ValidationUtility.ValidateSampleRate(44100); // true
bool isValidFftSize = ValidationUtility.ValidateFftSize(1024); // true
bool isValidChannelCount = ValidationUtility.ValidateChannelCount(2); // true
bool isValidFps = ValidationUtility.ValidateFps(60); // true
bool isValidFrequency = ValidationUtility.ValidateFrequency(1000f); // true

// Validate audio data
float[] audioData = new float[] { 0.5f, -0.3f, 0.8f };
bool isValidAudioData = ValidationUtility.ValidateAudioData(audioData); // true
bool isValidAmplitude = ValidationUtility.ValidateAmplitude(0.75f); // true

// Validate file paths and durations
bool isValidFilePath = ValidationUtility.ValidateFilePath("audio.wav"); // true
bool isValidDuration = ValidationUtility.ValidateDuration(30.5f); // true
bool isValidDeviceIndex = ValidationUtility.ValidateDeviceIndex(0); // true
bool isValidTimeInMs = ValidationUtility.ValidateTimeInMs(5000); // true
bool isValidNormalization = ValidationUtility.ValidateNormalization(1.0f); // true

// Validate collections and required parameters
bool isValidCollection = ValidationUtility.ValidateCollection(new List<float> { 1.0f, 2.0f }); // true
bool areParametersValid = ValidationUtility.ValidateRequiredParameters("audio.wav", 44100, 2); // true

// Using exception-throwing methods for direct validation in method calls
ValidationUtility.ThrowIfNull(audioData, nameof(audioData));
ValidationUtility.ThrowIfNullOrWhitespace("audio.wav", nameof(filePath));
ValidationUtility.ThrowIfOutOfRange(44100, 8000, 192000, nameof(sampleRate));
ValidationUtility.ThrowIfInvalid(ValidationUtility.ValidateSampleRate(44100), nameof(sampleRate), "must be between 8000 and 192000");
```

## WaveformDataExtensions

`WaveformDataExtensions` provides extension methods for the `WaveformData` class, offering convenient utilities for audio waveform manipulation and analysis. These methods enable common operations such as converting mono to stereo, downsampling, calculating amplitude metrics, splitting stereo channels, and accessing waveform properties without modifying the original waveform data.

### Usage Example

```csharp
using NAudioVisualizer.Domain.Models;

// Create a waveform from audio samples
var samples = new float[] { 0.1f, 0.5f, -0.3f, 0.8f, -0.6f, 0.2f };
var waveform = new WaveformData(samples, channelCount: 1, sampleRate: 44100);

// Calculate waveform properties
Console.WriteLine($"Duration: {waveform.GetDurationSeconds():F3} seconds");
Console.WriteLine($"Total samples: {waveform.GetTotalSampleCount()}");
Console.WriteLine($"Points per channel: {waveform.GetPointsPerChannel()}");
Console.WriteLine($"Peak amplitude: {waveform.GetPeakAmplitude():F3}");
Console.WriteLine($"RMS amplitude: {waveform.GetRmsAmplitude():F3}");
Console.WriteLine($"Average amplitude: {waveform.GetAverageAmplitude():F3}");

// Convert mono to stereo
var stereoWaveform = waveform.ToStereo();
Console.WriteLine($"Stereo channel count: {stereoWaveform.ChannelCount}");

// Downsample waveform (reduce resolution by factor of 2)
var downsampled = waveform.Downsample(2);
Console.WriteLine($"Downsampled sample rate: {downsampled.SampleRate} Hz");

// Normalize waveform to range [0, 1]
var normalized = waveform.NormalizedCopy();
Console.WriteLine($"Normalized peak: {normalized.GetPeakAmplitude():F3}");

// Get sample at specific index
float sampleAtIndex = waveform.GetSample(2);
Console.WriteLine($"Sample at index 2: {sampleAtIndex:F3}");

// Get range of samples
var sampleRange = waveform.GetSampleRange(1, 3);
Console.WriteLine($"Samples from index 1-3: [{string.Join(", ", sampleRange.Select(s => s.ToString("F3")))}]");

// For stereo waveforms: split channels or get channel-specific peaks
var leftSamples = new float[] { 0.1f, 0.2f, 0.3f };
var rightSamples = new float[] { 0.4f, 0.5f, 0.6f };
var stereoFromChannels = leftSamples.ToStereoWaveform(rightSamples, 44100);

var leftPeaks = stereoFromChannels.GetChannelPeaks(0);
var rightPeaks = stereoFromChannels.GetChannelPeaks(1);

if (stereoFromChannels.SplitStereoChannels() is (float[] Left, float[] Right) channels)
{
    Console.WriteLine($"Left channel length: {channels.Left.Length}");
    Console.WriteLine($"Right channel length: {channels.Right.Length}");
}
```

## ValidationAndStringUtilityTests

### Usage Example

```csharp
using FluentAssertions;
using NAudioVisualizer.Utilities;
using Xunit;

// Test ValidationUtility methods
ValidationUtility.ValidateSampleRate(44100).Should().BeTrue(); // Valid sample rate
ValidationUtility.ValidateFftSize(1024).Should().BeTrue(); // Valid FFT size
ValidationUtility.ValidateChannelCount(2).Should().BeTrue(); // Valid channel count
ValidationUtility.ValidateFrequency(1000f).Should().BeTrue(); // Valid frequency
ValidationUtility.ValidateAmplitude(0.75f).Should().BeTrue(); // Valid amplitude

float[] audioData = new float[] { 0.5f, -0.3f, 0.8f };
ValidationUtility.ValidateAudioData(audioData).Should().BeTrue(); // Valid audio data

// Test exception-throwing validation methods
ValidationUtility.ThrowIfNull(audioData, nameof(audioData)); // Does not throw
ValidationUtility.ThrowIfOutOfRange(44100, 8000, 192000, nameof(sampleRate)); // Does not throw
ValidationUtility.ThrowIfNullOrWhitespace("audio.wav", nameof(filePath)); // Does not throw

// Test StringUtility methods
string truncated = StringUtility.Truncate("This is a very long string", 15);
truncated.Should().Be("This is a ve...");

string snakeCase = StringUtility.ToSnakeCase("SampleRate");
snakeCase.Should().Be("sample_rate");

string formattedBytes = StringUtility.FormatBytes(15728640);
formattedBytes.Should().Contain("MB");

int occurrences = StringUtility.CountOccurrences("hello hello world", "hello");
occurrences.Should().Be(2);
```

## AudioBufferAndEventBusTests

### Usage Example

```csharp
using FluentAssertions;
using NAudioVisualizer.Caching;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Events;
using Xunit();

// Test AudioBuffer functionality
var buffer = new AudioBuffer(capacity: 1024, sampleRate: 44100, channelCount: 2);

// Write samples to buffer
buffer.Write(new float[] { 0.1f, 0.2f, 0.3f, 0.4f });
buffer.Count.Should().Be(4);

// Read samples from buffer
var samples = buffer.Read(2, out int actualRead);
samples.Should().HaveCount(2);
actualRead.Should().Be(2);

// Peek at samples without consuming them
buffer.Peek(3);
buffer.Count.Should().Be(4); // Count unchanged

// Test capacity management - oldest samples get overwritten
buffer.Write(new float[1024]); // Fill buffer
buffer.IsFull().Should().BeTrue();

// Test duration calculation
var duration = buffer.GetDurationSeconds();
duration.Should().BeApproximately(0.0232f, precision: 0.0001f); // 1024 samples at 44100 Hz

// Test EventBus functionality
var eventBus = new EventBus();
bool eventReceived = false;

// Subscribe to events
var subscription = eventBus.Subscribe<string>(payload => {
    eventReceived = true;
    payload.Should().Be("test-event");
});

// Publish an event
eventBus.Publish("test-event");
eventReceived.Should().BeTrue();

// Unsubscribe using token
subscription.Dispose();
eventReceived = false;
eventBus.Publish("test-event-after-dispose");
eventReceived.Should().BeFalse();

// Test cache functionality
var cache = new CacheManager<string, int>();
cache.Set("sampleRate", 48000);
cache.TryGetValue("sampleRate", out var retrievedValue).Should().BeTrue();
retrievedValue.Should().Be(48000);
```

## WaveformServiceBenchmarks

`WaveformServiceBenchmarks` is a benchmark class for measuring the performance of various waveform processing operations in the `WaveformService` class. It uses BenchmarkDotNet to provide detailed performance metrics including execution time, memory allocation, and other diagnostic information for optimizing audio waveform generation and processing algorithms.

### Usage Example

```csharp
using NAudioVisualizer.Benchmarks;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Services;

// Create benchmark instance
var benchmarks = new WaveformServiceBenchmarks();

// Initialize the benchmark (required before running any benchmarks)
benchmarks.Setup();

// Generate a waveform from audio samples
WaveformData waveform = benchmarks.GenerateWaveform();

// Downsample audio samples by a factor of 4
float[] downsampled = benchmarks.DownsampleSamples();

// Calculate peak values with a window size of 512
float[] peaks = benchmarks.CalculatePeakValues();

// Apply smoothing filter with a filter size of 3
float[] smoothed = benchmarks.ApplySmoothingFilter();

// Run all benchmarks and display results
WaveformServiceBenchmarks.Program.Main(new string[0]);
```

## MathUtility

`MathUtility` provides mathematical utility functions for audio processing. Includes functions for frequency conversions, dB calculations, window functions, and signal processing.

### Usage Example

```csharp
using NAudioVisualizer.Utilities;

// Convert frequency to MIDI note and back
int midiNote = MathUtility.FrequencyToMidiNote(440f); // Returns 69 (A4)
float frequency = MathUtility.MidiNoteToFrequency(69); // Returns 440f

// Convert amplitude to decibels
float db = MathUtility.AmplitudeToDb(0.5f); // Approximately -6.02f
float amplitude = MathUtility.DbToAmplitude(-6f); // Approximately 0.5f

// Calculate signal metrics
float[] signal = new float[] { 0.2f, -0.5f, 0.8f, -0.3f };
float rms = MathUtility.CalculateRms(signal); // RMS value
float peak = MathUtility.CalculatePeak(signal); // 0.8f

// Apply window functions
float[] windowedSignal = new float[] { 0.2f, -0.5f, 0.8f, -0.3f };
MathUtility.ApplyHannWindow(windowedSignal); // Applies Hann window in-place
MathUtility.ApplyHammingWindow(windowedSignal); // Applies Hamming window in-place

// Power of two operations (useful for FFT)
int nextPowerOfTwo = MathUtility.NextPowerOf2(15); // Returns 16
bool isPowerOfTwo = MathUtility.IsPowerOf2(16); // Returns true

// Interpolation and mapping
float interpolated = MathUtility.Lerp(0f, 10f, 0.5f); // Returns 5f
float mapped = MathUtility.MapRange(0.5f, 0f, 1f, 0f, 100f); // Returns 50f

// Distance calculation
float distance = MathUtility.Distance(0f, 0f, 3f, 4f); // Returns 5f
```

## MathUtilityTests

`MathUtilityTests` is a test class that verifies the behavior of mathematical utility methods provided by the `MathUtility` class. It contains unit tests for frequency-to-MIDI conversion, amplitude-to-decibel conversion, RMS calculation, power-of-two operations, linear interpolation, and range mapping functions.

### Usage Example

```csharp
using FluentAssertions;
using NAudioVisualizer.Utilities;
using Xunit;

// Test frequency to MIDI conversion
MathUtility.FrequencyToMidiNote(440f).Should().Be(69); // A4 note
MathUtility.FrequencyToMidiNote(0f).Should().Be(0); // Non-positive frequency returns 0

// Test amplitude to decibel conversion
MathUtility.AmplitudeToDb(0f).Should().Be(float.NegativeInfinity); // Zero amplitude
MathUtility.AmplitudeToDb(1f).Should().Be(0f); // Unit amplitude = 0 dB

// Test RMS calculation
float[] uniformSignal = new float[] { 0.5f, 0.5f, 0.5f, 0.5f };
MathUtility.CalculateRms(uniformSignal).Should().BeApproximately(0.5f, 0.0001f);
MathUtility.CalculateRms(Array.Empty<float>()).Should().Be(0f); // Empty array

// Test peak calculation
float[] signalWithNegative = new float[] { 0.3f, -0.7f, 0.4f };
MathUtility.CalculatePeak(signalWithNegative).Should().Be(0.7f); // Absolute maximum

// Test power-of-two operations
MathUtility.NextPowerOf2(15).Should().Be(16);
MathUtility.IsPowerOf2(16).Should().BeTrue();
MathUtility.IsPowerOf2(15).Should().BeFalse();

// Test linear interpolation and range mapping
MathUtility.Lerp(0.5f, 0f, 10f).Should().Be(5f); // Midpoint
MathUtility.MapRange(0.5f, 0f, 1f, 10f, 20f).Should().Be(15f); // Midpoint of target range
MathUtility.MapRange(0.5f, 0f, 0.5f, 10f, 20f).Should().Be(10f); // Clamped to target minimum
```

## EventPublisher

`EventPublisher` is a static facade for the EventBus that provides convenient helper methods for publishing events throughout the application. It simplifies event publishing by offering strongly-typed Publish* methods for each event type, along with a generic Subscribe<T> method and a Reset method to clear all subscriptions.

### Usage Example

```csharp
using NAudioVisualizer.Events;
using NAudioVisualizer.Domain.Models;

// Publish an audio capture started event
EventPublisher.PublishAudioCaptureStarted(
    deviceId: 0,
    sampleRate: 44100,
    channelCount: 2);

// Publish a waveform generated event
var waveform = new WaveformData(new float[] { 0.1f, 0.5f, -0.3f }, 1, 44100);
EventPublisher.PublishWaveformGenerated(waveform, generationTimeMs: 10, frameCount: 1024);

// Subscribe to audio capture started events
using var subscription = EventPublisher.Subscribe<AudioCaptureStartedEvent>(e =>
{
    Console.WriteLine($"Capture started: device {e.DeviceId}, {e.SampleRate}Hz, {e.ChannelCount} channels");
});

// Later, reset the event bus (e.g., during application shutdown)
EventPublisher.Reset();
```

## WaveformServiceTests

`WaveformServiceTests` is a comprehensive test class that verifies the behavior of the `WaveformService` class. It contains unit tests for waveform processing operations including downsampling, peak calculation, smoothing filters, frame energy calculation, and zero-crossing detection. The tests use FluentAssertions for readable assertions and Xunit as the testing framework.

### Usage Example

```csharp
using FluentAssertions;
using NAudioVisualizer.Services;
using Xunit;

// Create test instance
var tests = new WaveformServiceTests();

// Test downsampling - reduces sample count by averaging
var samples = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };
var downsampled = tests.DownsampleSamples(samples, 2);
downsampled.Should().HaveCount(2);
downsampled[0].Should().BeApproximately(0.15f, 0.0001f); // Average of first two samples

// Test peak calculation - finds top N peaks in audio data
var peaks = tests.CalculatePeakValues(new float[] { 0.1f, 0.8f, 0.2f, 0.5f }, 2);
peaks.Should().HaveCount(2);
peaks[0].Should().Be(0.8f); // Highest peak
peaks[1].Should().Be(0.5f); // Second highest

// Test smoothing filter - applies moving average to reduce noise
var smoothed = tests.ApplySmoothingFilter(new float[] { 0.0f, 1.0f, 0.0f }, 3);
smoothed.Should().HaveCount(3);
smoothed[1].Should().BeApproximately(0.333f, 0.001f); // Center point after smoothing

// Test frame energy calculation - computes RMS energy per frame
var frameEnergies = tests.CalculateFrameEnergy(new float[] { 0.5f, 0.5f, 0.5f, 0.5f }, 2);
frameEnergies.Should().HaveCount(2);
frameEnergies[0].Should().BeApproximately(0.5f, 0.0001f); // RMS of first frame

// Test zero-crossing detection - counts signal sign changes
int zeroCrossings = tests.CountZeroCrossings(new float[] { -0.1f, 0.1f, -0.1f, 0.1f });
zeroCrossings.Should().Be(3); // Three sign changes in the signal

// Test exception handling - invalid peak count throws
Action invalidPeakCount = () => tests.CalculatePeakValues(new float[] { 0.1f, 0.2f }, 0);
invalidPeakCount.Should().Throw<ArgumentException>();
```

## AudioSessionRepository

`AudioSessionRepository` manages audio session metadata and frame data storage. It provides thread-safe operations for creating sessions, adding audio frames, retrieving frame data with various filtering options, managing session lifecycle, and obtaining repository statistics.

### Usage Example

```csharp
using NAudioVisualizer.Data.Repositories;
using NAudioVisualizer.Domain.Models;

// Create repository instance
var repository = new AudioSessionRepository();

// Create a new audio session
var metadata = new AudioMetadata
{
    SessionId = Guid.NewGuid(),
    StartTime = DateTime.UtcNow,
    AudioDevice = AudioDevice.GetDefaultDevice(),
    SampleRate = 44100,
    ChannelCount = 2
};
var session = repository.CreateSession(metadata);

// Add audio frames to the session
var frame = new AudioFrame
{
    Timestamp = DateTime.UtcNow,
    Samples = new float[] { 0.1f, 0.2f, 0.3f },
    ChannelCount = 2
};
repository.AddFrameToSession(session.SessionId, frame);

// Retrieve session frames
var allFrames = repository.GetSessionFrames(session.SessionId);
var recentFrames = repository.GetRecentFrames(session.SessionId, 100);
var timeRangeFrames = repository.GetFramesInTimeRange(
    session.SessionId,
    DateTime.UtcNow.AddMinutes(-5),
    DateTime.UtcNow
);

// Get specific frame
var firstFrame = repository.GetFrame(session.SessionId, 0);

// Update session limits
repository.SetMaxFramesPerSession(10000);

// End session
repository.EndSession(session.SessionId);

// Get repository statistics
var stats = repository.GetStats();
Console.WriteLine($"Total sessions: {stats.TotalSessionCount}");
Console.WriteLine($"Total frames: {stats.TotalFrameCount}");

// Clean up (optional)
// repository.DeleteSession(session.SessionId);
```

## VstPluginInfo

`VstPluginInfo` and related classes provide a comprehensive model for VST plugin metadata, parameter management, automation lanes, and presets. `VstPluginInfo` captures immutable plugin identity and capabilities. `VstParameter` represents live automatable controls with normalization utilities. `VstParameterAutomationLane` manages timed automation points with support for linear, step, cosine, and cubic spline interpolation. `VstPreset` stores complete parameter snapshots for saving, loading, and categorizing plugin states.

### Usage Example

```csharp
using NAudioVisualizer.Domain.Models;

// Create a plugin info snapshot
var pluginInfo = new VstPluginInfo(
    Id: Guid.NewGuid(),
    Name: "SuperReverb",
    Vendor: "AudioCorp",
    Version: "1.0.0",
    PluginPath: "/plugins/superreverb.dll",
    Category: VstPluginCategory.Reverb,
    ParameterCount: 12,
    IsSynth: false);

// Check validity
bool isValid = pluginInfo.IsValid();

// Work with parameters
var cutoffParam = new VstParameter(
    id: 0,
    name: "Cutoff",
    label: "Hz",
    units: "Hz",
    minValue: 20f,
    maxValue: 20000f,
    defaultValue: 1000f);

// Normalize and denormalize values
float normalized = cutoffParam.NormalizedValue;
float denormalized = cutoffParam.DenormalizeValue(0.5f); // Returns 10010f

// Manage automation lanes
var lane = new VstParameterAutomationLane
{
    PluginId = pluginInfo.Id,
    ParameterId = 0,
    ParameterName = "Cutoff"
};

// Add automation points
lane.AddPoint(0.0, 0.0f, VstAutomationInterpolation.Linear);
lane.AddPoint(2.5, 0.8f, VstAutomationInterpolation.Cosine);
lane.AddPoint(5.0, 0.2f, VstAutomationInterpolation.Step);

// Evaluate at a specific time
float? valueAt3s = lane.Evaluate(3.0);

// Remove a point
lane.RemovePoint(2.5);

// Clear all points
lane.Clear();

// Create and manage presets
var preset = new VstPreset
{
    PluginId = pluginInfo.Id,
    Name: "Warm Pad",
    Description: "Soft, atmospheric reverb setting",
    Category: "Pads",
    Tags = ["warm", "atmospheric", "reverb"],
    ParameterValues = new Dictionary<int, float> { [0] = 0.5f, [1] = 0.3f }
};

bool presetValid = preset.IsValid();
```

## VisualizationDataRepository

`VisualizationDataRepository` provides thread-safe, in-memory storage for visualization data. It supports storing and retrieving visualizations by ID, source-frame session ID, or visualization type; finding the most recent entry; deleting individual entries or complete sessions; clearing the repository; collecting repository statistics; pruning older entries; and exporting a session to an indented JSON file.

### Usage Example

```csharp
using NAudioVisualizer.Data.Repositories;
using NAudioVisualizer.Domain.Models;

var repository = new VisualizationDataRepository();
var sessionId = Guid.NewGuid();
var sourceFrame = new AudioFrame([0.1f, -0.2f, 0.3f], 1, 44100, 0)
{
    Id = sessionId
};

var waveform = new WaveformData([0.1f, -0.2f, 0.3f], 1, 44100)
{
    SourceFrame = sourceFrame
};

// Store visualization data
repository.Store(waveform);

// Retrieve entries
VisualizationData? byId = repository.GetById(waveform.Id);
IReadOnlyList<VisualizationData> bySession = repository.GetBySession(sessionId);
IReadOnlyList<VisualizationData> byType = repository.GetByType(VisualizationType.Waveform);
VisualizationData? mostRecent = repository.GetMostRecent(VisualizationType.Waveform);
IReadOnlyList<VisualizationData> all = repository.GetAll();

// Inspect repository statistics
RepositoryStats stats = repository.GetStats();
Console.WriteLine($"{stats.TotalCount} visualizations across {stats.SessionCount} sessions");

// Keep only the most recently generated entries
int prunedCount = repository.PruneOldest(100);

// Export all entries associated with a source-frame session ID
repository.ExportSessionToJson(sessionId, "visualization-session.json");

// Delete entries
bool deleted = repository.Delete(waveform.Id);
int deletedForSession = repository.DeleteBySession(sessionId);
repository.Clear();
```

## FrequencyBands

`FrequencyBands` represents normalized energy values for the bass, midrange, and treble frequency bands extracted from audio spectrum data. The three bands are:
- Bass: 0–250 Hz
- Midrange: 250–4000 Hz  
- Treble: >4000 Hz

The `BassEnergy`, `MidEnergy`, and `TrebleEnergy` properties are normalized so their sum equals 1.0 when total audio energy is non-zero, making them suitable for visualizing relative energy distribution across the frequency spectrum.

### Usage Example

```csharp
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Services;

// Analyze audio spectrum
var analyzer = new SpectrumAnalyzer();
SpectrumData spectrum = analyzer.AnalyzeSpectrum(audioFrame, fftSize: 2048);

// Extract normalized frequency band energies
FrequencyBands bands = analyzer.ExtractFrequencyBands(spectrum);

// Access individual band values (0.0 to 1.0)
float bass = bands.BassEnergy;
float mid = bands.MidEnergy;
float treble = bands.TrebleEnergy;

// Verify normalization (should be ~1.0 when energy present)
float total = bands.BassEnergy + bands.MidEnergy + bands.TrebleEnergy;

// Convert to array for easier processing
float[] bandArray = bands.ToArray(); // [bass, mid, treble]

// Example usage in visualization
Console.WriteLine($"Bass: {bands.BassEnergy:P0}, Mid: {bands.MidEnergy:P0}, Treble: {bands.TrebleEnergy:P0}");
```

## FileSystemUtility

`FileSystemUtility` provides static helpers for common file system operations, including creating and validating directories, measuring and formatting file sizes, generating unique file names, safely deleting files and directories, asynchronously reading and writing text files, cleaning up files based on retention time, and calculating directory sizes.

### Usage Example

```csharp
using NAudioVisualizer.Utilities;

// Create a directory only when it does not already exist
bool created = FileSystemUtility.CreateDirectoryIfNotExists("output");

// Ensure a directory exists
FileSystemUtility.EnsureDirectoryExists("output/archive");

// Get and format a file's size
long fileSize = FileSystemUtility.GetFileSize("output/report.txt");
string formattedFileSize = FileSystemUtility.FormatFileSize(fileSize);

// Generate an available file name when the requested file already exists
string uniqueFileName = FileSystemUtility.GenerateUniqueFileName("output/report.txt");

// Write and read text asynchronously
await FileSystemUtility.WriteFileAsync(uniqueFileName, "Analysis complete.");
string content = await FileSystemUtility.ReadFileAsync(uniqueFileName);

// Safely delete a file or a directory and all its contents
bool fileDeleted = FileSystemUtility.SafeDeleteFile(uniqueFileName);
FileSystemUtility.SafeDeleteDirectory("output/archive");

// Delete files older than the retention period
int deletedFileCount = FileSystemUtility.CleanupOldFiles("output", 30);

// Calculate the total size of a directory and its contents
long directorySize = FileSystemUtility.GetDirectorySize("output");
```

## EventBus Extensions

`EventBusExtensions` provides extension methods for <see cref="EventBus"/> to simplify common event bus operations.

### Public Methods

- `HasSubscribers<T>(this EventBus bus)` - Determines whether there are any subscribers for the specified event type.
- `PublishIfSubscribed<T>(this EventBus bus, T @event)` - Publishes the event only if there are subscribers for the specified event type.
- `UnsubscribeAllAndPublish<T>(this EventBus bus, T @event)` - Unsubscribes all handlers for the specified event type and then publishes the event.

### Usage Example

```csharp
using NAudioVisualizer.Events;

// Check if there are subscribers before publishing
if (eventBus.HasSubscribers<MyEvent>())
{
    eventBus.Publish(new MyEvent { Data = "Hello" });
}

// Publish only if subscribers exist (more efficient)
eventBus.PublishIfSubscribed(new MyEvent { Data = "Hello" });

// Unsubscribe all handlers and then publish
eventBus.UnsubscribeAllAndPublish(new MyEvent { Data = "Hello" });
```

`EventBusAsyncExtensions` provides asynchronous extension methods for <see cref="EventBus"/>.

### Public Methods

- `PublishAsync<T>(this EventBus bus, T eventData)` - Publishes an event asynchronously.

### Usage Example

```csharp
using NAudioVisualizer.Events;
using System.Threading.Tasks;

// Publish an event asynchronously
await eventBus.PublishAsync(new MyEvent { Data = "Hello" });
```

## CacheStatistics

`CacheStatistics` provides information about the cache usage and performance. It is returned by `CacheManager.GetStatistics()`.

### Properties

- `CurrentSize`: The current number of cached entries.
- `MaxSize`: The maximum number of cached entries.
- `FillPercentage`: The percentage of the cache currently in use.
- `Hits`: The number of successful cache retrievals.
- `Misses`: The number of unsuccessful cache retrievals.
- `Evictions`: The number of entries removed by the eviction policy (LRU).
- `Expirations`: The number of expired entries removed from the cache.
- `HitRate`: The ratio of successful retrievals to total retrieval attempts (calculated as Hits/(Hits+Misses)).

### Usage Example

```csharp
using NAudioVisualizer.Caching;

var cache = new CacheManager<string, int>(maxSize: 100);
cache.Set("key1", 100);
cache.TryGetValue("key1", out var value);

CacheStatistics stats = cache.GetStatistics();
Console.WriteLine($"Cache size: {stats.CurrentSize}/{stats.MaxSize}");
Console.WriteLine($"Fill percentage: {stats.FillPercentage:F1}%");
Console.WriteLine($"Hits: {stats.Hits}, Misses: {stats.Misses}");
Console.WriteLine($"Hit rate: {stats.HitRate:P1}");
Console.WriteLine($"Evictions: {stats.Evictions}, Expirations: {stats.Expirations}");
```

Additionally, the `CacheStatisticsExtensions` class provides a `ToJson` method for serializing the statistics to JSON.

```csharp
string json = stats.ToJson(indented: true);
Console.WriteLine(json);
```

## Logger

`Logger` writes diagnostic messages to a log file and, optionally, the console. Its constructor accepts an optional log file path and console-output flag, while `MinimumLevel` filters messages below the selected `LogLevel`; it provides `Debug`, `Info`, `Warn`, `Error`, and `Critical` methods, supports exception details for error and critical messages, implements the `ILogger` abstraction, and implements `IDisposable` so the underlying log writer can be released with `Dispose`.

### Usage Example

```csharp
using NAudioVisualizer.Infrastructure;

var logger = new Logger(
    logFilePath: "logs/application.log",
    writeToConsole: true)
{
    MinimumLevel = LogLevel.Debug
};

logger.Debug("Starting audio analysis.");
logger.Info("Audio analysis is running.");
logger.Warn("The input signal is close to clipping.");

try
{
    throw new InvalidOperationException("The audio device is unavailable.");
}
catch (Exception exception)
{
    logger.Error("Audio processing failed.", exception);
    logger.Critical("The application cannot continue.", exception);
}

// Logger can be supplied wherever the ILogger abstraction is expected.
ILogger applicationLogger = logger;
applicationLogger.Info("Logged through ILogger.");

// Release the underlying log writer when logging is complete.
logger.Dispose();
```

## ILogger and LoggerExtensions

`ILogger` is a minimal logger abstraction used throughout the application, defining the contract for logging operations. `LoggerExtensions` provides extension methods for the `Logger` class that add convenient formatting and functionality.

### ILogger Interface

The `ILogger` interface defines the core logging contract:

- `LogLevel MinimumLevel { get; set; }` - Gets or sets the minimum log level that will be emitted
- `void Debug(string message)` - Logs a debug message
- `void Info(string message)` - Logs an information message
- `void Warn(string message)` - Logs a warning message
- `void Error(string message, Exception? exception = null)` - Logs an error message, optionally with an exception
- `void Critical(string message, Exception? exception = null)` - Logs a critical message, optionally with an exception

### LogLevel Enum

The `LogLevel` enumeration defines the available log levels:
- `Debug = 0` - Detailed diagnostic information
- `Info = 1` - General informational messages
- `Warn = 2` - Warning messages
- `Error = 3` - Error messages
- `Critical = 4` - Critical messages

### LoggerExtensions

The `LoggerExtensions` class provides extension methods for the `Logger` class:

#### Formatted Logging Methods
- `void Debug(this Logger logger, string message, params object?[]? args)` - Logs a debug message with optional string formatting
- `void Info(this Logger logger, string message, params object?[]? args)` - Logs an information message with optional string formatting
- `void Warn(this Logger logger, string message, params object?[]? args)` - Logs a warning message with optional string formatting
- `void Error(this Logger logger, string message, Exception? exception = null, params object?[]? args)` - Logs an error message with optional string formatting and exception
- `void Critical(this Logger logger, string message, Exception? exception = null, params object?[]? args)` - Logs a critical message with optional string formatting and exception

#### Additional Functionality
- `IDisposable MethodScope(this Logger logger, [CallerMemberName] string methodName = "")` - Logs method entry and exit with timing information for performance monitoring
- `long Time(this Logger logger, string operationName, Action action, params object?[]? args)` - Logs a message with the current execution time and returns the elapsed milliseconds
- `bool If(this Logger logger, bool condition, string message, params object?[]? args)` - Conditionally logs a message based on a condition

### Usage Example

```csharp
using NAudioVisualizer.Infrastructure;

// Create a logger instance
var logger = new Logger(
    logFilePath: "logs/application.log",
    writeToConsole: true)
{
    MinimumLevel = LogLevel.Debug
};

// Basic logging
logger.Debug("Starting audio analysis.");
logger.Info("Audio analysis is running.");
logger.Warn("The input signal is close to clipping.");

try
{
    throw new InvalidOperationException("The audio device is unavailable.");
}
catch (Exception exception)
{
    logger.Error("Audio processing failed.", exception);
    logger.Critical("The application cannot continue.", exception);
}

// Using extension methods with formatting
logger.Info("Processing {Count} files at {Path}", fileCount, directoryPath);

// Method scope timing
using (logger.MethodScope())
{
    // Method execution here
    PerformAudioProcessing();
}

// Timing operations
long elapsedMs = logger.Time("AudioFileProcessing", () => 
{
    ProcessAudioFile("sample.wav");
});

// Conditional logging
logger.If(debugMode, "Debug mode is enabled with level {Level}", logger.MinimumLevel);

// Logger can be supplied wherever the ILogger abstraction is expected.
ILogger applicationLogger = logger;
applicationLogger.Info("Logged through ILogger.");

// Release the underlying log writer when logging is complete.
logger.Dispose();
```

### Implementing a Custom ILogger

To create a custom logger implementation, implement the `ILogger` interface:

```csharp
using NAudioVisualizer.Infrastructure;

public class CustomLogger : ILogger
{
    public LogLevel MinimumLevel { get; set; } = LogLevel.Info;
    
    public void Debug(string message)
    {
        if (MinimumLevel <= LogLevel.Debug)
        {
            // Custom debug logging implementation
            WriteToCustomSink($"[DEBUG] {message}");
        }
    }
    
    public void Info(string message)
    {
        if (MinimumLevel <= LogLevel.Info)
        {
            // Custom info logging implementation
            WriteToCustomSink($"[INFO] {message}");
        }
    }
    
    public void Warn(string message)
    {
        if (MinimumLevel <= LogLevel.Warn)
        {
            // Custom warning logging implementation
            WriteToCustomSink($"[WARN] {message}");
        }
    }
    
    public void Error(string message, Exception? exception = null)
    {
        if (MinimumLevel <= LogLevel.Error)
        {
            // Custom error logging implementation
            var formattedMessage = exception != null 
                ? $"[ERROR] {message}\nException: {exception}"
                : $"[ERROR] {message}";
            WriteToCustomSink(formattedMessage);
        }
    }
    
    public void Critical(string message, Exception? exception = null)
    {
        if (MinimumLevel <= LogLevel.Critical)
        {
            // Custom critical logging implementation
            var formattedMessage = exception != null 
                ? $"[CRITICAL] {message}\nException: {exception}"
                : $"[CRITICAL] {message}";
            WriteToCustomSink(formattedMessage);
        }
    }
    
    private void WriteToCustomSink(string message)
    {
        // Implement your custom logging logic here
        // For example: write to database, send to external service, etc.
        Console.WriteLine(message); // Placeholder
    }
}
```

## AudioProcessingWorker

`AudioProcessingWorker` runs queued `ProcessingTask` instances asynchronously in the background. Use `Start` to begin processing, `EnqueueTask(ProcessingTask)` to add work, `GetQueueDepth` to inspect pending work, `ClearQueue` to remove and count pending tasks, `StopAsync` to stop gracefully, and `Dispose` to release the worker's resources.

### Usage Example

```csharp
using NAudioVisualizer.Workers;

var worker = new AudioProcessingWorker();

worker.Start();

worker.EnqueueTask(new ProcessingTask
{
    Name = "Analyze audio frame",
    ExecuteAsync = async cancellationToken =>
    {
        await ProcessAudioFrameAsync(cancellationToken);
    },
    OnComplete = () => Console.WriteLine("Audio frame processed."),
    OnError = exception => Console.WriteLine(exception.Message)
});

int pendingTasks = worker.GetQueueDepth();
Console.WriteLine($"Pending tasks: {pendingTasks}");

// Remove any work that has not started and get the number of removed tasks.
int clearedTasks = worker.ClearQueue();

await worker.StopAsync();
worker.Dispose();
```

## VisualizerTheme

`VisualizerTheme` defines the background, waveform gradient, and spectrogram palette used by a visualizer. Each `GradientStop(float position, uint color)` pairs a normalized position from `0` through `1` with an ARGB color (`0xAARRGGBB`); `VisualizerTheme(string name, uint backgroundColor, IReadOnlyList<GradientStop> waveformGradient, IReadOnlyList<GradientStop> spectrogramPalette)` requires a non-null, non-whitespace name and at least two stops in both gradient collections. Built-in themes are available through `VisualizerTheme.Presets.Classic`, `Accessible`, and `Monochrome`, while `ColorScheme.Dark`, `Light`, `Neon`, and `Grayscale` provide named wrappers whose `Theme` property exposes a `VisualizerTheme`.

### Usage Example

```csharp
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Themes;

var customTheme = new VisualizerTheme(
    name: "Ocean",
    backgroundColor: 0xFF001122,
    waveformGradient: new[]
    {
        new GradientStop(0f, 0xFF006699),
        new GradientStop(1f, 0xFFCCFFFF)
    },
    spectrogramPalette: new[]
    {
        new GradientStop(0f, 0xFF001122),
        new GradientStop(0.5f, 0xFF0088CC),
        new GradientStop(1f, 0xFFFFFFFF)
    });

VisualizerTheme classic = VisualizerTheme.Presets.Classic;
VisualizerTheme accessible = VisualizerTheme.Presets.Accessible;
VisualizerTheme monochrome = VisualizerTheme.Presets.Monochrome;

ColorScheme darkScheme = ColorScheme.Dark;
ColorScheme lightScheme = ColorScheme.Light;
ColorScheme neonScheme = ColorScheme.Neon;
ColorScheme grayscaleScheme = ColorScheme.Grayscale;

VisualizerTheme darkTheme = darkScheme.Theme;
```

## GradientStop

`GradientStop` defines a single color stop in a gradient, combining a normalized position in the [0, 1] range with an ARGB color value.

### Usage Example

```csharp
using NAudioVisualizer.Domain.Models;

// Create a gradient stop at position 0.0 (start) with green color
var startStop = new GradientStop(0f, 0xFF00FF00);

// Create a gradient stop at position 1.0 (end) with blue color
var endStop = new GradientStop(1f, 0xFFFF0000);

// Use in a VisualizerTheme
var theme = new VisualizerTheme(
    name: "Custom Theme",
    backgroundColor: 0xFF000000,
    waveformGradient: new[] { startStop, endStop },
    spectrogramPalette: new[]
    {
        new GradientStop(0f, 0xFF000000),
        new GradientStop(1f, 0xFFFFFFFF)
    });
```

### GradientStopExtensions

Provides useful extension methods for GradientStop operations:

- `WithColor(uint newColor)` - Creates a new GradientStop with the same position but modified color
- `WithPosition(float newPosition)` - Creates a new GradientStop with the same color but modified position
- `GetArgbComponents(out byte alpha, out byte red, out byte green, out byte blue)` - Gets the ARGB color components
- `WithAlpha(byte alpha)` - Creates a new gradient stop with adjusted alpha/transparency
- `AdjustBrightness(float brightnessFactor)` - Creates a new gradient stop with adjusted brightness (0.0 to 2.0)
- `AdjustContrast(float contrastFactor)` - Creates a new gradient stop with adjusted contrast (0.0 to 2.0)
- `IndexIn(IReadOnlyList<GradientStop> stops)` - Gets the relative position of this gradient stop within a collection
- `IsFirst(IReadOnlyList<GradientStop> stops)` - Determines whether this gradient stop is the first stop in the collection
- `IsLast(IReadOnlyList<GradientStop> stops)` - Determines whether this gradient stop is the last stop in the collection
- `Next(IReadOnlyList<GradientStop> stops)` - Gets the next gradient stop in the collection, or null if this is the last stop
- `Previous(IReadOnlyList<GradientStop> stops)` - Gets the previous gradient stop in the collection, or null if this is the first stop
- `Interpolate(GradientStop other, float t)` - Creates a new gradient stop that is the color-interpolated version between this stop and another
- `HasSameColor(GradientStop other)` - Determines whether two gradient stops have the same color
- `HasSamePosition(GradientStop other)` - Determines whether two gradient stops have the same position
- `GetBrightness()` - Gets the perceived brightness of the gradient stop's color (0-255)
- `IsDark()` - Determines whether the gradient stop's color is considered dark (brightness < 128)
- `IsLight()` - Determines whether the gradient stop's color is considered light (brightness >= 128)

### GradientStopValidation

Provides validation helpers for GradientStop instances:

- `Validate(GradientStop? value)` - Validates a GradientStop instance and returns a list of human-readable problems
- `IsValid(GradientStop? value)` - Determines whether a GradientStop instance is valid
- `EnsureValid(GradientStop? value)` - Ensures that a GradientStop instance is valid, throwing an exception if it is not

## AudioBuffer

`AudioBuffer` is a circular buffer for storing audio data with efficient memory usage. It provides thread-safe operations for writing, reading, and managing audio samples with automatic overwriting of oldest samples when the buffer is full. The buffer tracks sample rate and channel count to enable duration calculations and provides statistics about buffer usage.

### Usage Example

```csharp
using NAudioVisualizer.Domain.Models;

// Create a buffer for stereo audio at 44.1kHz with capacity for 1 second of audio
var buffer = new AudioBuffer(capacity: 44100 * 2, sampleRate: 44100, channelCount: 2);

// Write audio samples to the buffer
float[] samples = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };
buffer.Write(samples);

// Peek at samples without removing them
float[] peeked = buffer.Peek(2); // Returns first 2 samples

// Read and remove samples from the buffer
int actualRead;
float[] readSamples = buffer.Read(2, out actualRead); // Returns 2 samples, actualRead = 2

// Get all available samples
float[] allSamples = buffer.GetAll();

// Check buffer status
bool isFull = buffer.IsFull();
bool isEmpty = buffer.IsEmpty();
int availableSpace = buffer.AvailableSpace();

// Get duration of audio in buffer
double durationSeconds = buffer.GetDurationSeconds();

// Get buffer statistics
AudioBufferStats stats = buffer.GetStats();

// Clear the buffer
buffer.Clear();
```

## AudioDataConverter

## ProcessingTask

`ProcessingTask` represents a unit of work to be executed by the `AudioProcessingWorker`. It encapsulates an asynchronous operation, along with optional completion and error callbacks.

### Public Properties

- `Name` (string): The name of the task.
- `ExecuteAsync` (Func<CancellationToken, Task>): The asynchronous operation to perform.
- `OnError` (Action<Exception>?): Optional callback invoked when the task throws an exception.
- `OnComplete` (Action?): Optional callback invoked when the task completes successfully.
- `CreatedAt` (DateTime): The timestamp when the task was created (set automatically to UTC now).

### ProcessingTaskExtensions

The `ProcessingTaskExtensions` class provides extension methods for `ProcessingTask`.

#### ToJson(this ProcessingTask task)

Serializes the public scalar properties (Name and CreatedAt) of a `ProcessingTask` to a JSON string.

### Usage Example

```csharp
using NAudioVisualizer.Workers;
using NAudioVisualizer.Infrastructure;

// Create a worker (with optional logger)
var worker = new AudioProcessingWorker();

// Start the worker
worker.Start();

// Create a processing task using object initializer
var task = new ProcessingTask
{
    Name = "Analyze audio spectrum",
    ExecuteAsync = async token =>
    {
        // Placeholder for actual spectrum analysis
        await Task.Delay(50, token);
    },
    OnComplete = () => Console.WriteLine("Spectrum analysis complete."),
    OnError = ex => Console.WriteLine($"Analysis failed: {ex.Message}")
};

// Serialize the task for logging (using the extension method)
string taskJson = task.ToJson();
Console.WriteLine($"Serialized task: {taskJson}");

// Enqueue the task
worker.EnqueueTask(task);

// Optionally, check the queue depth
Console.WriteLine($"Tasks in queue: {worker.GetQueueDepth()}");

// Stop the worker when done
await worker.StopAsync();
worker.Dispose();
```

`AudioDataConverter` provides static helpers for converting between decibel and linear amplitude values, formatting frequencies, durations, and audio levels, converting float samples to and from 16-bit PCM, extracting and interleaving audio channels, calculating RMS and peak levels, normalizing samples, and applying gain.

### Usage Example

```csharp
using NAudioVisualizer.Infrastructure;

// Convert between decibels and linear amplitude.
float linear = AudioDataConverter.DbToLinear(-6f);
float referencedLinear = AudioDataConverter.DbToLinear(-6f, 0.5f);
float decibels = AudioDataConverter.LinearToDb(linear);
float referencedDecibels = AudioDataConverter.LinearToDb(referencedLinear, 0.5f);

// Format frequency, duration, and audio levels for display.
string frequency = AudioDataConverter.FormatFrequency(440f);
string duration = AudioDataConverter.FormatDuration(90d);
string level = AudioDataConverter.FormatAudioLevel(0.75f);
string levelDb = AudioDataConverter.FormatAudioLevelDb(0.5f);

float[] samples = [0.25f, -0.5f, 0.75f, -1f];

// Convert float samples to little-endian 16-bit PCM and back.
byte[] pcmBytes = AudioDataConverter.FloatToInt16Pcm(samples);
float[] decodedSamples = AudioDataConverter.Int16PcmToFloat(pcmBytes);

// Extract channels from interleaved audio and combine them again.
float[] interleaved = [0.1f, 0.2f, 0.3f, 0.4f];
float[] leftChannel = AudioDataConverter.ExtractChannel(interleaved, 0, 2);
float[] rightChannel = AudioDataConverter.ExtractChannel(interleaved, 1, 2);
float[] combinedChannels = AudioDataConverter.InterleaveChannels([leftChannel, rightChannel]);

// Measure, normalize, and adjust sample levels.
float rmsLevel = AudioDataConverter.CalculateRmsLevel(samples);
float peakLevel = AudioDataConverter.CalculatePeakLevel(samples);
float[] normalizedSamples = AudioDataConverter.NormalizeSamples(samples);
float[] gainedSamples = AudioDataConverter.ApplyGain(samples, -3f);
```

## ColorUtility

`ColorUtility` provides static helpers for converting between RGB and HSV color spaces, interpolating colors, mapping normalized values to viridis, jet, and grayscale palettes, adjusting brightness and saturation, finding complementary colors, and converting colors to and from six-digit hexadecimal strings.

### Usage Example

```csharp
using System.Drawing;
using NAudioVisualizer.Utilities;

// Convert normalized RGB components to HSV and back to 0-255 RGB components.
ColorUtility.RgbToHsv(0.2f, 0.4f, 0.8f, out float hue, out float saturation, out float value);
ColorUtility.HsvToRgb(hue, saturation, value, out float red, out float green, out float blue);

// Interpolate halfway between two colors.
Color midpoint = ColorUtility.LerpColor(Color.Blue, Color.Red, 0.5f);

// Map normalized values to visualization palettes.
Color viridis = ColorUtility.GetViririsColor(0.75f);
Color jet = ColorUtility.GetJetColor(0.5f);
Color grayscale = ColorUtility.GetGrayscale(0.25f);

// Adjust a color's brightness and saturation.
Color brighter = ColorUtility.AdjustBrightness(midpoint, 1.2f);
Color lessSaturated = ColorUtility.AdjustSaturation(midpoint, 0.5f);

// Find the opposite hue on the color wheel.
Color complementary = ColorUtility.GetComplementaryColor(midpoint);

// Convert between Color and #RRGGBB strings.
string hex = ColorUtility.ColorToHex(complementary);
Color parsed = ColorUtility.HexToColor(hex);
```

## MidiInputService

`MidiInputService` discovers MIDI input devices with `GetAvailableDevicesAsync(CancellationToken)`, opens and closes a selected device with `StartAsync(int, CancellationToken)` and `StopAsync()`, and raises the `NoteReceived` event with a `MidiNoteEventArgs.Note` value for each received note message. Call `Dispose()` when finished to release the active device and other resources; `MidiNoteEvent.GetNoteName(int)` returns a note name with its octave, while `MidiNoteEvent.GetFrequency(int)` calculates its equal-tempered frequency in hertz.

### Usage Example

```csharp
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Services;

var midiInput = new MidiInputService();

IReadOnlyList<MidiDeviceInfo> devices =
    await midiInput.GetAvailableDevicesAsync();

midiInput.NoteReceived += (_, args) =>
{
    MidiNoteEvent note = args.Note;
    Console.WriteLine(
        $"{note.NoteName}: {note.Frequency:F2} Hz, velocity {note.Velocity}");
};

if (devices.Count > 0)
{
    await midiInput.StartAsync(devices[0].Index);

    // Receive note events until MIDI capture is no longer needed.
    await Task.Delay(TimeSpan.FromSeconds(10));

    await midiInput.StopAsync();
}

string middleCName = MidiNoteEvent.GetNoteName(60);
float concertPitchFrequency = MidiNoteEvent.GetFrequency(69);

midiInput.Dispose();
```

## AudioCaptureService

`AudioCaptureService` discovers audio input devices with `GetAvailableDevices()`, configures a selected device with `Initialize(int deviceIndex, int sampleRate, int channelCount)`, and controls capture with `StartRecordingAsync()` and `StopRecordingAsync()`. During capture, `GetCurrentMetadata()` returns the current `AudioMetadata`, `GetBufferedAudio()` returns a copy of the captured samples, `GetAudioBuffer()` provides the current `AudioBuffer`, and `ClearBuffer()` removes buffered samples. Subscribe to `FrameCaptured` to receive each `AudioFrameEventArgs.Frame` and to `DeviceStatusChanged` to receive availability and error details through `AudioDeviceEventArgs`; call `Dispose()` when finished to stop capture and release device and buffer resources.

### Usage Example

```csharp
using NAudioVisualizer.Services;

var audioCapture = new AudioCaptureService();

audioCapture.FrameCaptured += (_, args) =>
{
    if (args.Frame is not null)
    {
        Console.WriteLine("Captured an audio frame.");
    }
};

audioCapture.DeviceStatusChanged += (_, args) =>
{
    Console.WriteLine($"Device available: {args.IsAvailable}");

    if (args.Exception is not null)
    {
        Console.WriteLine(args.Exception.Message);
    }
};

var devices = audioCapture.GetAvailableDevices();

if (devices.Count > 0)
{
    audioCapture.Initialize(deviceIndex: 0, sampleRate: 48000, channelCount: 2);
    await audioCapture.StartRecordingAsync();

    await Task.Delay(TimeSpan.FromSeconds(5));

    var metadata = audioCapture.GetCurrentMetadata();
    float[]? samples = audioCapture.GetBufferedAudio();
    var buffer = audioCapture.GetAudioBuffer();

    Console.WriteLine($"Buffered samples: {samples?.Length ?? 0}");
    audioCapture.ClearBuffer();

    await audioCapture.StopRecordingAsync();
}

audioCapture.Dispose();
```

## ServiceContainer

`ServiceContainer` is a lightweight dependency injection container that registers singleton instances with `Register<T>(T)`, registers lazily created services with `RegisterFactory<T>(Func<ServiceContainer, T>)`, retrieves services with `Resolve<T>()`, checks registrations with `IsRegistered<T>()`, and removes them with `Unregister<T>()`. `ApplicationConfiguration.ConfigureServices()` creates the default application container, while `ApplicationConfiguration.ConfigureServices(ApplicationSettings)` accepts settings such as buffer size, sample rate, FFT size, target frame rate, logging, and session frame limits.

### Usage Example

```csharp
using NAudioVisualizer.Configuration;
using NAudioVisualizer.Data.Repositories;
using NAudioVisualizer.Services;

var container = new ServiceContainer();

// Register an existing singleton instance.
container.Register(new AudioSessionRepository());

// Register a service factory; the first resolved instance is cached.
container.RegisterFactory<AudioCaptureService>(
    serviceContainer => new AudioCaptureService());

bool hasRepository = container.IsRegistered<AudioSessionRepository>();
AudioSessionRepository? repository =
    container.Resolve<AudioSessionRepository>();
AudioCaptureService? captureService =
    container.Resolve<AudioCaptureService>();

bool removed = container.Unregister<AudioCaptureService>();

// Create the default application service container.
ServiceContainer defaultServices =
    ApplicationConfiguration.ConfigureServices();

// Or configure it with application settings.
var settings = new ApplicationSettings
{
    MaxAudioBufferSize = 384000,
    DefaultSampleRate = 48000,
    DefaultFftSize = 4096,
    TargetFps = 60,
    EnableLogging = true,
    MaxFramesPerSession = 10000
};

ServiceContainer configuredServices =
    ApplicationConfiguration.ConfigureServices(settings);
```

## ConfigurationManager

`ConfigurationManager` stores typed configuration values with `GetValue<T>(string, T?)` and `SetValue<T>(string, T)`, loads and persists the configured JSON file with `LoadSettings()` and `SaveSettings()`, restores built-in settings with `ResetToDefaults()`, describes the current values with `GetConfigurationSummary()`, and transfers settings to or from another JSON file with `ExportSettings(string)` and `ImportSettings(string)`.

### Usage Example

```csharp
using NAudioVisualizer.Configuration;
using NAudioVisualizer.Infrastructure;

var logger = new Logger();
var configuration = new ConfigurationManager(logger, "settings.json");

// Read and update typed values.
int sampleRate = configuration.GetValue<int>("audio.sampleRate", 44100);
configuration.SetValue("audio.sampleRate", 48000);

// Reload values from, or save values to, the configured JSON file.
configuration.LoadSettings();
configuration.SaveSettings();

// Restore the built-in defaults and inspect the current configuration.
configuration.ResetToDefaults();
string summary = configuration.GetConfigurationSummary();
Console.WriteLine(summary);

// Export the current values and import values from another JSON file.
configuration.ExportSettings("settings-backup.json");
configuration.ImportSettings("settings-backup.json");
```

## CacheManager

`CacheManager<TKey, TValue>` stores typed values in a size-limited LRU cache. `Set(TKey, TValue, TimeSpan?)` adds or replaces an entry with an optional expiration, `TryGetValue(TKey, out TValue?)` and `GetOrDefault(TKey, TValue?)` retrieve values, `Contains(TKey)` checks for an unexpired entry, `Remove(TKey)` removes an entry, and `RemoveExpiredEntries()` clears expired entries. `GetStatistics()` returns cache usage, hit, miss, eviction, and expiration statistics, while `ResetStatistics()` resets those counters.

### Usage Example

```csharp
using NAudioVisualizer.Caching;

var cache = new CacheManager<string, string>(
    maxSize: 100,
    defaultExpiration: TimeSpan.FromMinutes(30));

// Store an entry with a custom expiration.
cache.Set("current-track", "Example Song", TimeSpan.FromMinutes(5));

if (cache.TryGetValue("current-track", out string? track))
{
    Console.WriteLine(track);
}

string? artist = cache.GetOrDefault("current-artist", "Unknown Artist");
bool hasCurrentTrack = cache.Contains("current-track");
bool removed = cache.Remove("current-track");

int expiredEntryCount = cache.RemoveExpiredEntries();

CacheStatistics statistics = cache.GetStatistics();
Console.WriteLine(
    $"Hits: {statistics.Hits}, misses: {statistics.Misses}, " +
    $"evictions: {statistics.Evictions}, expirations: {statistics.Expirations}");

cache.ResetStatistics();
```

## EventBus

`EventBus` is a central, thread-safe event bus for the application that implements the pub-sub pattern. It decouples event publishers from subscribers and uses weak references internally to automatically clean up dead subscriptions and prevent memory leaks. It provides `Subscribe<T>(Action<T>)` to register handlers (returning an `IDisposable` for easy unsubscription), `Publish<T>(T)` to broadcast events to all active subscribers, `GetSubscriberCount<T>()` to inspect registration counts, `UnsubscribeAll<T>()` to remove handlers for a specific event type, `Clear()` to remove all subscriptions, and `Dispose()` to release resources.

### Usage Example

```csharp
using NAudioVisualizer.Events;

// Create an event bus instance
var eventBus = new EventBus();

// Subscribe to events (returns an IDisposable for cleanup)
var subscription = eventBus.Subscribe<MyEvent>(e =>
{
    Console.WriteLine($"Received event: {e.Data}");
});

// Publish an event to all subscribers
eventBus.Publish(new MyEvent { Data = "Hello" });

// Check subscriber count
int count = eventBus.GetSubscriberCount<MyEvent>();

// Unsubscribe all handlers for a specific event type
eventBus.UnsubscribeAll<MyEvent>();

// Clear all subscriptions
eventBus.Clear();

// Dispose the event bus to release resources and clean up weak references
eventBus.Dispose();
```

## SpectrogramAnalyzer

`SpectrogramAnalyzer` builds time-frequency data from ordered audio frames with `BuildSpectrogram(AudioFrame[], int, int)` and supports streaming analysis through a bounded rolling buffer using `AddSpectrumFrame(SpectrumData)`, `GetCurrentSpectrogram()`, and `SetBufferSize(int)`. It can extract frequency and time slices with `GetFrequencySlice(SpectrogramData, float)` and `GetTimeSlice(SpectrogramData, double)`, calculate frame-to-frame spectral change with `CalculateSpectralFlux(SpectrogramData)`, and locate transient onset frame indices with `DetectTransients(SpectrogramData, float)`.

### Usage Example

```csharp
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Services;

var spectrogramAnalyzer = new SpectrogramAnalyzer();

// Build a spectrogram from an ordered array of captured audio frames.
AudioFrame[] audioFrames = GetAudioFrames();
SpectrogramData spectrogram = spectrogramAnalyzer.BuildSpectrogram(
    audioFrames,
    fftSize: 2048,
    hopSize: 512);

// Extract magnitudes over time at 1 kHz and across frequencies at 0.25 seconds.
float[] frequencySlice = spectrogramAnalyzer.GetFrequencySlice(
    spectrogram,
    frequencyHz: 1000f);
float[] timeSlice = spectrogramAnalyzer.GetTimeSlice(
    spectrogram,
    timeSeconds: 0.25);

// Measure spectral change and detect strong local peaks in that curve.
float[] spectralFlux = spectrogramAnalyzer.CalculateSpectralFlux(spectrogram);
List<int> transientFrames = spectrogramAnalyzer.DetectTransients(
    spectrogram,
    threshold: 0.5f);

// Streaming mode keeps only the newest spectrum frames.
var spectrumAnalyzer = new SpectrumAnalyzer();
spectrogramAnalyzer.SetBufferSize(maxFrames: 200);

SpectrumData spectrumFrame = spectrumAnalyzer.AnalyzeSpectrum(audioFrames[0]);
spectrogramAnalyzer.AddSpectrumFrame(spectrumFrame);

SpectrogramData? currentSpectrogram =
    spectrogramAnalyzer.GetCurrentSpectrogram();
```

## WaveformService

`WaveformService` creates renderable waveform data with `GenerateWaveform(AudioFrame, int)`, reduces sample data with `DownsampleSamples(float[], int)` or min/max buckets with `DownsampleMinMax(float[], int)`, normalizes waveforms with `NormalizeWaveform(WaveformData)`, calculates peaks with `CalculatePeakValues(float[], int)`, smooths samples with `ApplySmoothingFilter(float[], int)`, and counts signal transitions with `CountZeroCrossings(float[])`. It also provides waveform navigation through `ApplyZoomWindow(WaveformData, long, long)`, `ZoomIn(WaveformData, long?)`, `ZoomOut(WaveformData, long?)`, `Pan(WaveformData, long)`, and `GetZoomWindow(WaveformData, out long, out long)`.

### Usage Example

```csharp
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Services;

var waveformService = new WaveformService();
var frame = new AudioFrame(
    samples: new[] { -0.8f, -0.2f, 0.4f, 1.0f, 0.3f, -0.5f, -1.0f, 0.2f },
    channelCount: 1,
    sampleRate: 48000,
    frameIndex: 0);

// Generate and normalize waveform data.
WaveformData waveform = waveformService.GenerateWaveform(
    frame,
    downsamplingFactor: 1);
waveformService.NormalizeWaveform(waveform);

float[] samples = waveform.GetData();

// Prepare reduced and analyzed representations for rendering.
float[] averaged = waveformService.DownsampleSamples(samples, factor: 2);
(float min, float max)[] minMax = waveformService.DownsampleMinMax(
    samples,
    targetBuckets: 4);
float[] peaks = waveformService.CalculatePeakValues(samples, peakCount: 4);
float[] smoothed = waveformService.ApplySmoothingFilter(samples, windowSize: 3);
int zeroCrossings = waveformService.CountZeroCrossings(samples);

// Apply and inspect zoom and pan operations.
waveformService.ApplyZoomWindow(waveform, startSample: 1, lengthSamples: 4);
waveformService.ZoomIn(waveform, zoomCenterSample: 2);
waveformService.ZoomOut(waveform);
waveformService.Pan(waveform, samplesToMove: 1);

bool isZoomed = waveformService.GetZoomWindow(
    waveform,
    out long startSample,
    out long lengthSamples);
```

## SpectrumAnalyzer

`SpectrumAnalyzer` creates FFT magnitude data from an `AudioFrame` with `AnalyzeSpectrum(AudioFrame, int)`, converts and smooths spectrum data in place with `ConvertToLogScale(SpectrumData, float)` and `SmoothSpectrum(SpectrumData, int)`, identifies its strongest frequency with `FindDominantFrequency(SpectrumData)`, and calculates its center of mass with `CalculateSpectralCentroid(SpectrumData)`. It also summarizes the spectrum through logarithmically spaced averages with `CalculateBandEnergies(SpectrumData, int)` and normalized bass, mid, and treble energy with `ExtractFrequencyBands(SpectrumData)`. Per-bin peak holds are controlled by `PeakHoldDecayDbPerSecond`, updated with `UpdatePeakHolds(SpectrumData, double)`, read with `GetPeakHolds()`, and cleared with `ResetPeakHolds()`.

### Usage Example

```csharp
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Services;

var analyzer = new SpectrumAnalyzer
{
    PeakHoldDecayDbPerSecond = 10f
};

AudioFrame audioFrame = GetAudioFrame();
SpectrumData spectrum = analyzer.AnalyzeSpectrum(audioFrame, fftSize: 2048);

// Convert magnitudes to dB and reduce visual noise between adjacent bins.
analyzer.ConvertToLogScale(spectrum, referenceValue: 1f);
analyzer.SmoothSpectrum(spectrum, windowSize: 3);

float dominantFrequency = analyzer.FindDominantFrequency(spectrum);
float spectralCentroid = analyzer.CalculateSpectralCentroid(spectrum);
float[] bandEnergies = analyzer.CalculateBandEnergies(spectrum, bandCount: 8);
FrequencyBands frequencyBands = analyzer.ExtractFrequencyBands(spectrum);

// Update peak holds once per rendered frame, then read or reset them.
analyzer.UpdatePeakHolds(spectrum, elapsedSeconds: 1.0 / 60.0);
float[]? peakHolds = analyzer.GetPeakHolds();
analyzer.ResetPeakHolds();
```

## Validation Helpers

This section provides an overview of all validation classes in the `src/` directory. Each validation class offers methods to validate specific domain objects and return lists of validation problems (empty if valid).

| Validation Class | Methods | Description |
|------------------|---------|-------------|
| `AudioDeviceValidation` | `Validate(AudioDevice)`, `IsValid(AudioDevice)`, `EnsureValid(AudioDevice)` | Validates AudioDevice instances including GUID, name, device index, manufacturer, channel count, sample rates, bit depth, status check time, and capabilities |
| `AudioFrameValidation` | `Validate(AudioFrame)`, `IsValid(AudioFrame)`, `EnsureValid(AudioFrame)` | Validates AudioFrame instances including ID, samples (null, empty, NaN, infinity), channel count, sample rate, timestamp (UTC), frame index, duration, peak amplitude, and RMS energy |
| `GradientStopValidation` | `Validate(GradientStop)`, `IsValid(GradientStop)`, `EnsureValid(GradientStop)` | Validates GradientStop instances including position range [0,1] and color validation (ARGB format) |
| `AudioStreamExceptionValidation` | `Validate(AudioStreamException)`, `IsValid(AudioStreamException)`, `EnsureValid(AudioStreamException)` | Validates AudioStreamException instances including ErrorCode enum validation and message/not-null checks |
| `ConfigurationManagerValidation` | `Validate(ConfigurationManager)`, `IsValid(ConfigurationManager)`, `EnsureValid(ConfigurationManager)` | Validates ConfigurationManager instances including numeric ranges (sample rate, channel count, bit depth, FFT size, FPS, brightness, contrast, display dimensions), boolean settings, and string settings (export format, logging level) |
| `SpectrogramAnalyzerValidation` | `Validate(SpectrogramAnalyzer)`, `IsValid(SpectrogramAnalyzer)`, `EnsureValid(SpectrogramAnalyzer)` | Validates SpectrogramAnalyzer instances by checking internal state via public API (buffer frame count) |
| `PathUtilityValidation` | See PathUtility section above | Validates PathUtility operations including path normalization, combination, absolute/relative conversion, trailing separator handling, file enumeration, directory retrieval, path validation, directory size calculation, and unique filename generation |

### Usage Example

```csharp
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Configuration;

// Validate an audio device
var device = new AudioDevice 
{
    Id = Guid.NewGuid(),
    Name = "Microphone",
    DeviceIndex = 0,
    Manufacturer = "Test Corp",
    ChannelCount = 2,
    SupportedSampleRates = new List<int> { 44100, 48000 },
    DefaultSampleRate = 44100,
    BitDepth = 16,
    LastStatusCheck = DateTime.UtcNow,
    Capabilities = DeviceCapabilities.Input
};

var deviceProblems = AudioDeviceValidation.Validate(device);
if (deviceProblems.Count == 0)
{
    Console.WriteLine("Audio device is valid");
}
else
{
    foreach (var problem in deviceProblems)
    {
        Console.WriteLine($"Validation issue: {problem}");
    }
}

// Using EnsureValid to throw on validation failure
try
{
    AudioDeviceValidation.EnsureValid(device);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Validation failed: {ex.Message}");
}
```

## JSON Serialization Helpers

The following extension classes provide `System.Text.Json` serialization and deserialization capabilities for various domain models, services, and utilities across the application.

| Class | Methods | Description |
|---|---|---|
| `AudioBufferJsonExtensions` | `ToJson`, `FromJson`, `TryFromJson` | Serializes and deserializes `AudioBuffer` instances. |
| `AudioDeviceJsonExtensions` | `ToJson` | Serializes `AudioDevice` essential properties. |
| `AudioFrameJsonExtensions` | `ToJson` | Serializes `AudioFrame` metadata and optionally raw samples. |
| `AudioMetadataJsonExtensions` | `ToJson` | Serializes `AudioMetadata` with camelCase naming. |
| `MidiNoteEventJsonExtensions` | `ToJson` | Serializes `MidiNoteEvent` to JSON. |
| `SpectrogramDataJsonExtensions` | `ToJson` | Serializes `SpectrogramData` metadata and optionally the data matrix. |
| `SpectrumDataJsonExtensions` | `ToJson` | Serializes `SpectrumData` to a JSON string. |
| `VisualizationDataJsonExtensions` | `ToJson`, `FromJson`, `TryFromJson` | Serializes and deserializes abstract `VisualizationData` types. |
| `VisualizationSettingsJsonExtensions` | `ToJson` | Serializes `VisualizationSettings` to JSON. |
| `WaveformDataJsonExtensions` | `ToJson` | Serializes `WaveformData` properties and data points. |
| `ColorSchemeJsonExtensions` | `ToJson` | Serializes `ColorScheme` including theme and gradients. |
| `AudioCaptureStartedEventJsonExtensions` | `ToJson`, `FromJson`, `TryFromJson` | Serializes and deserializes `AudioCaptureStartedEvent`. |
| `EventPublisherJsonExtensions` | `ToJson`, `FromJson`, `TryFromJson` | Serializes and deserializes `EventPublisher` state and subscriber counts. |
| `ServiceContainerJsonExtensions` | `ToJson`, `FromJson`, `TryFromJson` | Serializes and deserializes `ServiceContainer` registered types. |
| `MidiInputServiceJsonExtensions` | `ToJson`, `FromJson`, `TryFromJson` | Serializes and deserializes `MidiInputService` state. |
| `AudioDataConverterJsonExtensions` | `ToJson<T>`, `FromJson<T>`, `TryFromJson<T>` | Generic JSON serialization and deserialization helpers. |
| `MathUtilityJsonExtensions` | `ToJson`, `FromJson`, `TryFromJson` | Serializes and deserializes `MathUtility` type information. |

### Usage Example

```csharp
using NAudioVisualizer.Domain.Models;

// Serialize an AudioBuffer to JSON
var buffer = new AudioBuffer(capacity: 1024, sampleRate: 44100, channelCount: 2);
string json = buffer.ToJson(indented: true);

// Deserialize back to an AudioBuffer
var restoredBuffer = AudioBufferJsonExtensions.FromJson(json);
if (restoredBuffer != null)
{
    Console.WriteLine($"Restored buffer capacity: {restoredBuffer.Capacity}");
}
```

## VST Plugin Model

The VST plugin model provides a comprehensive set of types for managing plugin metadata, parameters, automation, and presets.

### Enums

- `VstPluginState` - Lifecycle states for a VST plugin instance (`Unloaded`, `Loaded`, `Initializing`, `Active`, `Suspended`, `Error`).
- `VstPluginCategory` - Functional categories for classifying plugins (`Undefined`, `Effect`, `Synth`, `Analyzer`, `Spatial`, `Mastering`, `Dynamics`, `EQ`, `Reverb`, `Delay`, `Distortion`, `Modulation`).
- `VstAutomationInterpolation` - Interpolation curves for automation lanes (`Linear`, `CubicSpline`, `Step`, `Cosine`).

### Core Types

- `VstParameter` - Represents a single automatable control. Tracks raw and normalised values (`0.0`–`1.0`), labels, units, and read-only/automated states. Includes `DenormalizeValue` for converting UI values back to the plugin's domain.
- `VstParameterAutomationPoint` - A single timed value on an automation lane, storing `PositionSeconds`, `Value`, and the `Interpolation` shape to apply toward the next point.
- `VstParameterAutomationLane` - A thread-safe, ordered collection of automation points for a specific plugin parameter. Supports adding/removing points, evaluating interpolated values at any timeline position, and clearing the lane.
- `VstPreset` - A complete, named snapshot of a plugin's parameter state. Supports categories, tags, factory flags, author metadata, and stores parameter state either as raw binary chunks or explicit normalised values.

### Usage Example

```csharp
using NAudioVisualizer.Domain.Models;

// Define a parameter
var cutoffParam = new VstParameter(
    id: 0,
    name: "Cutoff",
    label: "Hz",
    units: "Hz",
    minValue: 20f,
    maxValue: 20000f,
    defaultValue: 1000f);

// Create an automation lane for this parameter
var lane = new VstParameterAutomationLane
{
    PluginId = Guid.NewGuid(),
    ParameterId = 0,
    ParameterName = "Cutoff"
};

// Add automation points with different interpolation curves
lane.AddPoint(0.0, 0.0f, VstAutomationInterpolation.Linear);
lane.AddPoint(2.5, 0.8f, VstAutomationInterpolation.Cosine);
lane.AddPoint(5.0, 0.2f, VstAutomationInterpolation.Step);

// Evaluate the lane at a specific time (e.g., 3.0 seconds)
float? valueAt3s = lane.Evaluate(3.0);

// Remove a point and clear the lane if needed
lane.RemovePoint(2.5);
lane.Clear();
```
