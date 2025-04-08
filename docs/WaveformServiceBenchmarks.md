# WaveformServiceBenchmarks

The `WaveformServiceBenchmarks` type contains a set of benchmark‑oriented helpers used to measure the performance of waveform generation, down‑sampling, peak detection, and smoothing operations within the **naudio‑visualizer** project. It is intended to be executed as a console application or invoked directly in performance test harnesses.

## API

### Setup
- **Purpose:** Prepares internal state (e.g., allocates buffers, initializes random generators) required by the other benchmark methods. Call this once before invoking any measurement routine.  
- **Parameters:** None.  
- **Return:** `void`.  
- **Throws:**  
  - `InvalidOperationException` – if `Setup` is called after a benchmark has already been run or if required resources cannot be allocated.  
  - `OutOfMemoryException` – if the system lacks sufficient memory to allocate the internal sample buffers.

### GenerateWaveform
- **Purpose:** Produces a `WaveformData` instance containing a synthetic audio signal (e.g., sine wave, noise) that serves as input for downstream benchmark steps.  
- **Parameters:** None.  
- **Return:** `WaveformData` – holds the generated sample array and associated metadata (sample rate, length).  
- **Throws:**  
  - `NotImplementedException` – if the waveform generation logic has not been implemented for the requested type.  
  - `OutOfMemoryException` – if allocating the sample array fails.

### DownsampleSamples
- **Purpose:** Reduces the sample rate of the internally stored waveform by applying an integer‑factor down‑sampling (e.g., taking every *n*‑th sample).  
- **Parameters:** None – operates on the data prepared by `Setup`/`GenerateWaveform`.  
- **Return:** `float[]` – the down‑sampled audio samples.  
- **Throws:**  
  - `InvalidOperationException` – if no source data has been generated or `Setup` has not been called.  
  - `ArgumentException` – if the internal sample buffer is empty or the down‑sample factor is invalid (derived from configuration).

### CalculatePeakValues
- **Purpose:** Computes peak amplitude values over a sliding window across the sample data, useful for measuring envelope detection performance.  
- **Parameters:** None – uses the current sample buffer.  
- **Return:** `float[]` – each element represents the maximum absolute sample value within a corresponding window.  
- **Throws:**  
  - `InvalidOperationException` – if sample data is unavailable (e.g., `Setup` not called).  
  - `DivideByZeroException` – if the configured window size is zero (should never occur with valid configuration).

### ApplySmoothingFilter
- **Purpose:** Applies a smoothing filter (e.g., moving average or low‑pass FIR) to the sample data to evaluate filtering throughput.  
- **Parameters:** None – works on the data set up by prior calls.  
- **Return:** `float[]` – the filtered sample array.  
- **Throws:**  
  - `InvalidOperationException` – if the sample buffer has not been initialized.  
  - `ArgumentException` – if the filter kernel size is less than one or exceeds the buffer length.

### Main
- **Purpose:** Program entry point. Parses optional command‑line arguments (e.g., `--samples 44100 --iterations 100`) to configure the benchmark run, then invokes `Setup` and the measurement methods in sequence, reporting timing results to the console.  
- **Parameters:** `string[] args` – standard command‑line arguments.  
- **Return:** `void`.  
- **Throws:**  
  - Propagates any exceptions thrown by the benchmark methods.  
  - `FormatException` – if an argument cannot be parsed into the expected numeric type.  
  - `FileNotFoundException` – if a required configuration file is specified but missing.

## Usage

### Example 1: Running the benchmark suite from the command line
```csharp
using NaudioVisualizer.Benchmarks;

// Entry point supplied by the project; equivalent to calling WaveformServiceBenchmarks.Main(args)
WaveformServiceBenchmarks.Main(new[] { "--samples", "44100", "--iterations", "5" });
```
This starts the benchmark, configures it to generate 44 100‑sample waveforms and repeat each measurement five times, then prints elapsed times for each step.

### Example 2: Using the benchmark helpers manually in a unit test or custom harness
```csharp
using NaudioVisualizer.Benchmarks;

var bench = new WaveformServiceBenchmarks();

// Prepare the environment
bench.Setup();

// Generate a waveform to work with
WaveformData wf = bench.GenerateWaveform();

// Perform down‑sampling
float[] downsampled = bench.DownsampleSamples();

// Calculate peak values on the down‑sampled data
float[] peaks = bench.CalculatePeakValues();

// Apply a smoothing filter
float[] smoothed = bench.ApplySmoothingFilter();

// At this point `smoothed` contains the final processed samples for further analysis or verification.
```

## Notes
- **State dependency:** All instance methods (`Setup`, `GenerateWaveform`, `DownsampleSamples`, `CalculatePeakValues`, `ApplySmoothingFilter`) rely on internal fields that are initialized by `Setup`. Calling any of them before `Setup` will result in an `InvalidOperationException`.  
- **Thread safety:** The class is **not** thread‑safe. Instance members should be accessed from a single thread only. The `Main` method is safe as the process entry point because it runs on the main thread before any other threads are started.  
- **Edge cases:**  
  - Providing an excessively large sample count may trigger `OutOfMemoryException`.  
  - A down‑sample factor larger than the source length yields an empty result array; callers should verify the length before further processing.  
  - The smoothing filter assumes a symmetric kernel; even‑sized kernels are adjusted internally to the nearest odd size.  
- **Exception handling:** Benchmark methods do not swallow exceptions; they are allowed to propagate so that a harness can detect failures and record them appropriately.  
- **Performance considerations:** The methods allocate new arrays on each call (except `Setup` which may reuse buffers). For tight‑loop benchmarking, consider pooling arrays externally if allocation overhead must be isolated from the algorithm under test.
