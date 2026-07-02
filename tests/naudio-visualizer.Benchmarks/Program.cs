using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Services;

namespace NAudioVisualizer.Benchmarks;

[MemoryDiagnoser]
public class WaveformServiceBenchmarks
{
    private WaveformService _service = null!;
    private AudioFrame _frame = null!;
    private float[] _samples = null!;

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

    [Benchmark]
    public WaveformData GenerateWaveform() => _service.GenerateWaveform(_frame, 4);

    [Benchmark]
    public float[] DownsampleSamples() => _service.DownsampleSamples(_samples, 4);

    [Benchmark]
    public float[] CalculatePeakValues() => _service.CalculatePeakValues(_samples, 512);

    [Benchmark]
    public float[] ApplySmoothingFilter() => _service.ApplySmoothingFilter(_samples, 3);
}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<WaveformServiceBenchmarks>();
    }
}
