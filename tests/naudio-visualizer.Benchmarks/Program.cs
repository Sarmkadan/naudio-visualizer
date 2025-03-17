using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Services;

namespace NAudioVisualizer.Benchmarks;

/// <summary>
/// Benchmark class for WaveformService.
/// </summary>
[MemoryDiagnoser]
public class WaveformServiceBenchmarks
{
    private WaveformService _service = null!;
    private AudioFrame _frame = null!;
    private float[] _samples = null!;

    /// <summary>
    /// Initializes the benchmark by creating a WaveformService instance and generating a random audio frame.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        _service = new WaveformService();
        _samples = new float[44100]; // 1 second of audio
        var random = new Random(42);
        for (int i = 0; i < _samples.Length; i++)
        {
            _samples[i] = (float)(random.NextDouble() * 2 - 1);
        }
        _frame = new AudioFrame(_samples, 1, 44100, 0);
    }

    /// <summary>
    /// Generates a waveform for the given audio frame with the specified resolution.
    /// </summary>
    /// <returns>The generated waveform.</returns>
    [Benchmark]
    public WaveformData GenerateWaveform() => _service.GenerateWaveform(_frame, 4);

    /// <summary>
    /// Downsamples the given samples by the specified factor.
    /// </summary>
    /// <returns>The downsampled samples.</returns>
    [Benchmark]
    public float[] DownsampleSamples() => _service.DownsampleSamples(_samples, 4);

    /// <summary>
    /// Calculates the peak values for the given samples with the specified window size.
    /// </summary>
    /// <returns>The peak values.</returns>
    [Benchmark]
    public float[] CalculatePeakValues() => _service.CalculatePeakValues(_samples, 512);

    /// <summary>
    /// Applies a smoothing filter to the given samples with the specified filter size.
    /// </summary>
    /// <returns>The filtered samples.</returns>
    [Benchmark]
    public float[] ApplySmoothingFilter() => _service.ApplySmoothingFilter(_samples, 3);
}

/// <summary>
/// The main program class.
/// </summary>
public class Program
{
    /// <summary>
    /// Runs the benchmark.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<WaveformServiceBenchmarks>();
    }
}
