#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NAudioVisualizer.Configuration;
using NAudioVisualizer.Constants;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Services;
using NAudioVisualizer.Integration;

namespace NAudioVisualizer.Examples;

/// <summary>
/// Example demonstrating spectrogram generation from captured audio.
/// Builds time-frequency representation and exports to JSON format.
/// </summary>
public class SpectrogramGenerationExample
{
    public static async Task Main()
    {
        Console.WriteLine("=== NAudio Visualizer - Spectrogram Generation ===\n");

        var settings = new ApplicationSettings
        {
            DefaultSampleRate = 44100,
            DefaultFftSize = 4096,
            TargetFps = 30,
            MaxFramesPerSession = 10000
        };

        var container = ApplicationConfiguration.ConfigureServices(settings);
        var audioService = container.Resolve<AudioCaptureService>();
        var spectrogramAnalyzer = container.Resolve<SpectrogramAnalyzer>();
        var exportService = container.Resolve<ExportService>();
        var logger = container.Resolve<Logger>();

        logger.MinimumLevel = LogLevel.Info;

        var capturedFrames = new List<AudioFrame>();
        var frameCount = 0;

        audioService.FrameCaptured += (sender, args) =>
        {
            frameCount++;
            capturedFrames.Add(args.Frame);

            if (frameCount % 60 == 0)
            {
                Console.WriteLine($"Captured {frameCount} frames ({capturedFrames.Count} stored)");
            }
        };

        try
        {
            Console.WriteLine("Initializing audio capture...");
            audioService.Initialize(deviceIndex: 0);

            Console.WriteLine("Capturing audio for 15 seconds...");
            await audioService.StartRecordingAsync();

            await Task.Delay(TimeSpan.FromSeconds(15));

            await audioService.StopRecordingAsync();

            Console.WriteLine($"\nCaptured {capturedFrames.Count} frames total");
            Console.WriteLine("Building spectrogram...");

            var spectrogram = spectrogramAnalyzer.BuildSpectrogram(
                capturedFrames.ToArray(),
                fftSize: 2048,
                hopSize: 512
            );

            spectrogram.ApplyLogScale();
            spectrogram.Normalize();

            Console.WriteLine($"Spectrogram generated:");
            Console.WriteLine($"  Time bins: {spectrogram.TimeWindows.GetLength(0)}");
            Console.WriteLine($"  Frequency bins: {spectrogram.TimeWindows.GetLength(1)}");
            Console.WriteLine($"  Frequency range: {spectrogram.FrequencyRangeMin:F2} - " +
                            $"{spectrogram.FrequencyRangeMax:F2} Hz");

            Console.WriteLine("\nExporting spectrogram to JSON...");
            var outputPath = "spectrogram_export.json";
            await exportService.ExportToJsonAsync(spectrogram, outputPath);

            Console.WriteLine($"Spectrogram exported to: {outputPath}");
            Console.WriteLine("Done!");
        }
        catch (Exception ex)
        {
            logger.Error($"Error: {ex.Message}");
        }
        finally
        {
            container.Dispose();
        }
    }
}
