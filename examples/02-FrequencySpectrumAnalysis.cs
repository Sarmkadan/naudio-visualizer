#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Threading.Tasks;
using NAudioVisualizer.Configuration;
using NAudioVisualizer.Constants;
using NAudioVisualizer.Services;
using NAudioVisualizer.Data.Repositories;

namespace NAudioVisualizer.Examples;

/// <summary>
/// Example demonstrating real-time frequency spectrum analysis.
/// Captures audio and performs FFT to analyze frequency content.
/// </summary>
public class FrequencySpectrumAnalysisExample
{
    public static async Task Main()
    {
        Console.WriteLine("=== NAudio Visualizer - Frequency Spectrum Analysis ===\n");

        var settings = new ApplicationSettings
        {
            DefaultSampleRate = 48000,
            DefaultFftSize = 2048,
            TargetFps = 60,
            MaxFramesPerSession = 5000
        };

        var container = ApplicationConfiguration.ConfigureServices(settings);
        var audioService = container.Resolve<AudioCaptureService>();
        var spectrumAnalyzer = container.Resolve<SpectrumAnalyzer>();
        var dataRepo = container.Resolve<VisualizationDataRepository>();
        var logger = container.Resolve<Logger>();

        logger.MinimumLevel = LogLevel.Info;

        var frameCount = 0;

        audioService.FrameCaptured += (sender, args) =>
        {
            frameCount++;

            var spectrum = spectrumAnalyzer.AnalyzeSpectrum(
                args.Frame,
                fftSize: 2048
            );

            spectrumAnalyzer.ConvertToLogScale(spectrum);
            dataRepo.Add(spectrum);

            if (frameCount % 30 == 0)
            {
                var bands = spectrumAnalyzer.ExtractFrequencyBands(spectrum, bandCount: 3);

                Console.WriteLine($"\n[Frame {frameCount}] Frequency Analysis");
                Console.WriteLine($"  Peak Frequency: {spectrum.PeakFrequency:F2} Hz");
                Console.WriteLine($"  Peak Magnitude: {spectrum.PeakMagnitude:F2} dB");
                Console.WriteLine($"  Bass (20-250 Hz): {bands[0]:F2} dB");
                Console.WriteLine($"  Midrange (250-2k Hz): {bands[1]:F2} dB");
                Console.WriteLine($"  Treble (2k-20k Hz): {bands[2]:F2} dB");
                Console.WriteLine($"  Bin Count: {spectrum.FrequencyBins.Length}");
            }
        };

        try
        {
            Console.WriteLine("Initializing audio capture with 48kHz sample rate...");
            audioService.Initialize(deviceIndex: 0, sampleRate: 48000);

            Console.WriteLine("Analyzing audio spectrum (20 seconds)...");
            await audioService.StartRecordingAsync();

            await Task.Delay(TimeSpan.FromSeconds(20));

            await audioService.StopRecordingAsync();

            Console.WriteLine("\n\nSpectrum Analysis Completed!");
            Console.WriteLine($"Total Frames Processed: {frameCount}");

            var allSpectrum = dataRepo.Query(TimeSpan.FromSeconds(20));
            Console.WriteLine($"Stored Spectrum Data: {allSpectrum.Count}");
        }
        catch (Exception ex)
        {
            logger.Error($"Analysis error: {ex.Message}");
        }
        finally
        {
            container.Dispose();
        }
    }
}
