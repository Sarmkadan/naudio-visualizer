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
using Xunit;

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
