#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NAudioVisualizer.Configuration;
using NAudioVisualizer.Constants;
using NAudioVisualizer.Services;
using NAudioVisualizer.Utilities;

namespace NAudioVisualizer.Examples;

/// <summary>
/// Example demonstrating performance profiling of visualization operations.
/// Measures FFT analysis, waveform generation, and spectrogram building times.
/// </summary>
public class PerformanceProfilingExample
{
    public static async Task Main()
    {
        Console.WriteLine("=== NAudio Visualizer - Performance Profiling ===\n");

        var settings = new ApplicationSettings
        {
            DefaultSampleRate = 48000,
            DefaultFftSize = 2048,
            TargetFps = 60,
            MaxFramesPerSession = 5000
        };

        var container = ApplicationConfiguration.ConfigureServices(settings);
        var audioService = container.Resolve<AudioCaptureService>();
        var waveformService = container.Resolve<WaveformService>();
        var spectrumAnalyzer = container.Resolve<SpectrumAnalyzer>();
        var profiler = new PerformanceProfiler();

        Console.WriteLine("Initializing audio capture...");
        audioService.Initialize(deviceIndex: 0);

        Console.WriteLine("Profiling audio visualization operations...\n");

        var waveformTimes = new System.Collections.Generic.List<double>();
        var spectrumTimes = new System.Collections.Generic.List<double>();
        var totalProcessingTime = Stopwatch.StartNew();

        audioService.FrameCaptured += (sender, args) =>
        {
            profiler.StartMeasure("waveform");
            var waveform = waveformService.GenerateWaveform(args.Frame);
            var waveformTime = profiler.StopMeasure("waveform").TotalMilliseconds;
            waveformTimes.Add(waveformTime);

            profiler.StartMeasure("spectrum");
            var spectrum = spectrumAnalyzer.AnalyzeSpectrum(args.Frame, 2048);
            spectrumAnalyzer.ConvertToLogScale(spectrum);
            var spectrumTime = profiler.StopMeasure("spectrum").TotalMilliseconds;
            spectrumTimes.Add(spectrumTime);

            if (waveformTimes.Count % 30 == 0)
            {
                Console.WriteLine($"[{waveformTimes.Count} frames]");
                Console.WriteLine($"  Waveform Gen: {GetStatsString(waveformTimes)}");
                Console.WriteLine($"  Spectrum Analysis: {GetStatsString(spectrumTimes)}");
            }
        };

        try
        {
            Console.WriteLine("Capturing 10 seconds of audio...\n");
            await audioService.StartRecordingAsync().ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
            await audioService.StopRecordingAsync().ConfigureAwait(false);

            totalProcessingTime.Stop();

            Console.WriteLine("\n=== Performance Summary ===\n");

            PrintStatistics("Waveform Generation", waveformTimes);
            Console.WriteLine();
            PrintStatistics("Spectrum Analysis", spectrumTimes);
            Console.WriteLine();

            Console.WriteLine($"Total Processing Time: {totalProcessingTime.ElapsedMilliseconds} ms");
            Console.WriteLine($"Average FPS: {waveformTimes.Count / (totalProcessingTime.ElapsedMilliseconds / 1000.0):F2}");
            Console.WriteLine($"Combined Processing per Frame: {(waveformTimes.Count > 0 ? (waveformTimes.Count + spectrumTimes.Count) / (totalProcessingTime.ElapsedMilliseconds / 1000.0) / waveformTimes.Count : 0):F2}x");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            container.Dispose();
        }
    }

    private static void PrintStatistics(string operationName, System.Collections.Generic.List<double> times)
    {
        if (times.Count == 0) return;

        Array.Sort(times.ToArray());
        double min = times[0];
        double max = times[^1];
        double avg = 0;
        foreach (var t in times) avg += t;
        avg /= times.Count;

        Console.WriteLine($"{operationName}:");
        Console.WriteLine($"  Count: {times.Count}");
        Console.WriteLine($"  Min: {min:F4} ms");
        Console.WriteLine($"  Max: {max:F4} ms");
        Console.WriteLine($"  Avg: {avg:F4} ms");
        Console.WriteLine($"  P95: {times[(int)(times.Count * 0.95)]:F4} ms");
    }

    private static string GetStatsString(System.Collections.Generic.List<double> times)
    {
        if (times.Count == 0) return "N/A";
        double avg = 0;
        foreach (var t in times) avg += t;
        avg /= times.Count;
        return $"Avg: {avg:F4} ms";
    }
}
