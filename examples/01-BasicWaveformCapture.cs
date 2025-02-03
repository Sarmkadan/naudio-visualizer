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

namespace NAudioVisualizer.Examples;

/// <summary>
/// Basic example demonstrating real-time waveform capture and analysis.
/// Captures audio from the default device and displays waveform statistics.
/// </summary>
public class BasicWaveformCaptureExample
{
    public static async Task Main()
    {
        Console.WriteLine("=== NAudio Visualizer - Basic Waveform Capture ===\n");

        var settings = new ApplicationSettings
        {
            DefaultSampleRate = AudioConstants.DEFAULT_SAMPLE_RATE,
            DefaultFftSize = AudioConstants.DEFAULT_FFT_SIZE,
            TargetFps = 60,
            MaxFramesPerSession = 5000
        };

        var container = ApplicationConfiguration.ConfigureServices(settings);
        var audioService = container.Resolve<AudioCaptureService>();
        var waveformService = container.Resolve<WaveformService>();

        var logger = container.Resolve<Logger>();
        logger.MinimumLevel = LogLevel.Info;

        var frameCount = 0;
        var startTime = DateTime.UtcNow;

        audioService.FrameCaptured += (sender, args) =>
        {
            frameCount++;

            var waveform = waveformService.GenerateWaveform(args.Frame);

            if (frameCount % 30 == 0)
            {
                var elapsed = DateTime.UtcNow - startTime;
                var fps = frameCount / elapsed.TotalSeconds;

                Console.WriteLine($"[Frame {frameCount}] FPS: {fps:F2}");
                Console.WriteLine($"  Sample Rate: {args.Frame.SampleRate} Hz");
                Console.WriteLine($"  Channels: {args.Frame.ChannelCount}");
                Console.WriteLine($"  Peak Amplitude: {waveform.PeakAmplitude:F4}");
                Console.WriteLine();
            }
        };

        try
        {
            Console.WriteLine("Initializing audio capture...");
            audioService.Initialize(deviceIndex: 0);

            Console.WriteLine("Starting audio capture (30 seconds)...");
            await audioService.StartRecordingAsync().ConfigureAwait(false);

            await Task.Delay(TimeSpan.FromSeconds(30)).ConfigureAwait(false);

            Console.WriteLine("\nStopping audio capture...");
            await audioService.StopRecordingAsync().ConfigureAwait(false);

            var totalTime = DateTime.UtcNow - startTime;
            Console.WriteLine($"\nCapture completed!");
            Console.WriteLine($"Total Frames: {frameCount}");
            Console.WriteLine($"Duration: {totalTime.TotalSeconds:F2}s");
            Console.WriteLine($"Average FPS: {frameCount / totalTime.TotalSeconds:F2}");
        }
        catch (Exception ex)
        {
            logger.Error($"Capture error: {ex.Message}");
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            container.Dispose();
        }
    }
}
