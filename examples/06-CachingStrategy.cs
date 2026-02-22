#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Threading.Tasks;
using NAudioVisualizer.Caching;
using NAudioVisualizer.Configuration;
using NAudioVisualizer.Constants;
using NAudioVisualizer.Services;

namespace NAudioVisualizer.Examples;

/// <summary>
/// Example demonstrating caching strategy for visualization data.
/// Shows how to leverage caching for improved performance with repeated queries.
/// </summary>
public class CachingStrategyExample
{
    public static async Task Main()
    {
        Console.WriteLine("=== NAudio Visualizer - Caching Strategy ===\n");

        var settings = new ApplicationSettings
        {
            DefaultSampleRate = 44100,
            DefaultFftSize = 2048,
            TargetFps = 60,
            MaxFramesPerSession = 5000
        };

        var container = ApplicationConfiguration.ConfigureServices(settings);
        var audioService = container.Resolve<AudioCaptureService>();
        var waveformService = container.Resolve<WaveformService>();
        var spectrumAnalyzer = container.Resolve<SpectrumAnalyzer>();
        var cacheManager = container.Resolve<CacheManager>();

        var frameCount = 0;
        var cacheHits = 0;
        var cacheMisses = 0;

        Console.WriteLine("Initializing cache with 30-second expiration...\n");

        audioService.FrameCaptured += (sender, args) =>
        {
            frameCount++;

            var cacheKey = $"waveform_{args.Frame.Timestamp:O}";

            if (!cacheManager.TryGetValue<dynamic>(cacheKey, out var cachedWaveform))
            {
                var waveform = waveformService.GenerateWaveform(args.Frame);
                cacheManager.Set(cacheKey, waveform, TimeSpan.FromSeconds(30));
                cacheMisses++;
            }
            else
            {
                cacheHits++;
            }

            var spectrumKey = $"spectrum_{args.Frame.Timestamp:O}";
            if (!cacheManager.TryGetValue<dynamic>(spectrumKey, out var cachedSpectrum))
            {
                var spectrum = spectrumAnalyzer.AnalyzeSpectrum(args.Frame, 2048);
                spectrumAnalyzer.ConvertToLogScale(spectrum);
                cacheManager.Set(spectrumKey, spectrum, TimeSpan.FromSeconds(30));
                cacheMisses++;
            }
            else
            {
                cacheHits++;
            }

            if (frameCount % 30 == 0)
            {
                var hitRate = (cacheHits + cacheMisses) > 0 ?
                    (cacheHits * 100.0) / (cacheHits + cacheMisses) : 0;

                Console.WriteLine($"[Frame {frameCount}] Cache Performance");
                Console.WriteLine($"  Hits: {cacheHits}");
                Console.WriteLine($"  Misses: {cacheMisses}");
                Console.WriteLine($"  Hit Rate: {hitRate:F2}%");

                var stats = cacheManager.GetStatistics();
                Console.WriteLine($"  Cache Size: {stats.TotalSizeBytes / 1024 / 1024:F2} MB");
                Console.WriteLine($"  Cached Items: {stats.ItemCount}");
                Console.WriteLine();
            }
        };

        try
        {
            Console.WriteLine("Initializing audio capture...");
            audioService.Initialize(deviceIndex: 0);

            Console.WriteLine("Capturing with caching (15 seconds)...\n");
            await audioService.StartRecordingAsync();
            await Task.Delay(TimeSpan.FromSeconds(15));
            await audioService.StopRecordingAsync();

            Console.WriteLine("\n=== Cache Statistics ===");
            var finalStats = cacheManager.GetStatistics();
            Console.WriteLine($"Total Items Cached: {finalStats.ItemCount}");
            Console.WriteLine($"Total Size: {finalStats.TotalSizeBytes / 1024 / 1024:F2} MB");
            Console.WriteLine($"Total Hits: {cacheHits}");
            Console.WriteLine($"Total Misses: {cacheMisses}");
            Console.WriteLine($"Overall Hit Rate: {(cacheHits * 100.0) / (cacheHits + cacheMisses):F2}%");

            Console.WriteLine("\nClearing cache...");
            cacheManager.Clear();
            Console.WriteLine("Cache cleared successfully.");
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
}
