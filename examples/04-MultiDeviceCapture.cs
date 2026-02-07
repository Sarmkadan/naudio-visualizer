#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Linq;
using NAudioVisualizer.Configuration;
using NAudioVisualizer.Constants;
using NAudioVisualizer.Services;

namespace NAudioVisualizer.Examples;

/// <summary>
/// Example demonstrating multi-device audio handling.
/// Enumerates available audio devices and captures from selected device.
/// </summary>
public class MultiDeviceCaptureExample
{
    public static void Main()
    {
        Console.WriteLine("=== NAudio Visualizer - Multi-Device Capture ===\n");

        var settings = new ApplicationSettings
        {
            DefaultSampleRate = AudioConstants.DEFAULT_SAMPLE_RATE,
            DefaultFftSize = AudioConstants.DEFAULT_FFT_SIZE,
            TargetFps = 60
        };

        var container = ApplicationConfiguration.ConfigureServices(settings);
        var audioService = container.Resolve<AudioCaptureService>();
        var logger = container.Resolve<Logger>();

        logger.MinimumLevel = LogLevel.Info;

        try
        {
            Console.WriteLine("Scanning available audio devices...\n");

            var devices = audioService.GetAvailableDevices();

            if (devices.Length == 0)
            {
                Console.WriteLine("No audio devices found!");
                return;
            }

            Console.WriteLine($"Found {devices.Length} audio device(s):\n");

            for (int i = 0; i < devices.Length; i++)
            {
                var device = devices[i];
                Console.WriteLine($"[{i}] {device.Name}");
                Console.WriteLine($"    Device ID: {device.DeviceId}");
                Console.WriteLine($"    Channels: {device.ChannelCount}");
                Console.WriteLine($"    Sample Rates: {string.Join(", ", device.SampleRates.Select(sr => $"{sr}Hz"))}");
                Console.WriteLine($"    Max Input Channels: {device.MaxInputChannels}");
                Console.WriteLine($"    State: {device.State}");
                Console.WriteLine();
            }

            Console.Write("Select device (0-" + (devices.Length - 1) + "): ");
            if (!int.TryParse(Console.ReadLine(), out int selectedIndex) ||
                selectedIndex < 0 || selectedIndex >= devices.Length)
            {
                Console.WriteLine("Invalid selection");
                return;
            }

            var selectedDevice = devices[selectedIndex];
            Console.WriteLine($"\nSelected: {selectedDevice.Name}");

            Console.Write("Select sample rate (default: 44100): ");
            var sampleRateInput = Console.ReadLine();
            int sampleRate = string.IsNullOrWhiteSpace(sampleRateInput) ?
                AudioConstants.DEFAULT_SAMPLE_RATE :
                int.Parse(sampleRateInput);

            Console.WriteLine($"\nInitializing {selectedDevice.Name}...");
            audioService.Initialize(
                deviceIndex: selectedIndex,
                sampleRate: sampleRate,
                channelCount: Math.Min(2, selectedDevice.ChannelCount)
            );

            Console.WriteLine($"Configuration:");
            Console.WriteLine($"  Device: {selectedDevice.Name}");
            Console.WriteLine($"  Sample Rate: {sampleRate} Hz");
            Console.WriteLine($"  Channels: {Math.Min(2, selectedDevice.ChannelCount)}");

            var frameCount = 0;
            audioService.FrameCaptured += (sender, args) =>
            {
                frameCount++;
                if (frameCount % 30 == 0)
                {
                    Console.WriteLine($"  Captured {frameCount} frames from {selectedDevice.Name}");
                }
            };

            Console.WriteLine("\nCapturing audio (10 seconds)...");
            var task = audioService.StartRecordingAsync();
            task.Wait(TimeSpan.FromSeconds(10));
            audioService.StopRecordingAsync().Wait();

            Console.WriteLine($"\nCapture complete!");
            Console.WriteLine($"Total frames: {frameCount}");
        }
        catch (Exception ex)
        {
            logger.Error($"Error: {ex.Message}");
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            container.Dispose();
        }
    }
}
