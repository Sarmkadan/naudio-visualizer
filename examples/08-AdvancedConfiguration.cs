// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Threading.Tasks;
using NAudioVisualizer.Configuration;
using NAudioVisualizer.Constants;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Services;
using SkiaSharp;

namespace NAudioVisualizer.Examples;

/// <summary>
/// Example demonstrating advanced configuration of visualization parameters.
/// Shows how to customize waveform, spectrum, and spectrogram rendering settings.
/// </summary>
public class AdvancedConfigurationExample
{
    public static async Task Main()
    {
        Console.WriteLine("=== NAudio Visualizer - Advanced Configuration ===\n");

        var settings = new ApplicationSettings
        {
            DefaultSampleRate = 96000,
            DefaultFftSize = 4096,
            TargetFps = 60,
            MaxFramesPerSession = 10000,
            CacheMaxSize = 200 * 1024 * 1024,
            BufferSize = 8192,

            WaveformSettings = new WaveformRenderingSettings
            {
                LineColor = new SKColor(0, 255, 127),
                LineThickness = 2.0f,
                StereoMode = true,
                AmplitudeZoom = 1.5f,
                DownsampleRatio = 4,
                ShowGrid = true,
                GridOpacity = 0.4f
            },

            SpectrumSettings = new SpectrumRenderingSettings
            {
                FrequencyScale = FrequencyScale.Logarithmic,
                MagnitudeScale = MagnitudeScale.Decibel,
                BarWidth = 2.5f,
                SmoothingFactor = 0.90f,
                FrequencyMin = 20,
                FrequencyMax = 20000,
                PeakHold = true
            },

            SpectrogramSettings = new SpectrogramRenderingSettings
            {
                Colormap = ColormapType.Plasma,
                TimeWindowSeconds = 10,
                BrightnessMultiplier = 1.3f,
                ContrastEnhancement = true,
                MagnitudeMin = -100,
                MagnitudeMax = 0
            }
        };

        Console.WriteLine("Configuration Applied:");
        Console.WriteLine($"  Sample Rate: {settings.DefaultSampleRate} Hz");
        Console.WriteLine($"  FFT Size: {settings.DefaultFftSize}");
        Console.WriteLine($"  Target FPS: {settings.TargetFps}");
        Console.WriteLine($"  Buffer Size: {settings.BufferSize}");
        Console.WriteLine($"  Cache Max: {settings.CacheMaxSize / 1024 / 1024} MB\n");

        Console.WriteLine("Waveform Settings:");
        Console.WriteLine($"  Color: RGB({settings.WaveformSettings.LineColor.Red}," +
                         $"{settings.WaveformSettings.LineColor.Green}," +
                         $"{settings.WaveformSettings.LineColor.Blue})");
        Console.WriteLine($"  Line Thickness: {settings.WaveformSettings.LineThickness}");
        Console.WriteLine($"  Stereo Mode: {settings.WaveformSettings.StereoMode}");
        Console.WriteLine($"  Amplitude Zoom: {settings.WaveformSettings.AmplitudeZoom}x");
        Console.WriteLine($"  Downsample Ratio: {settings.WaveformSettings.DownsampleRatio}\n");

        Console.WriteLine("Spectrum Settings:");
        Console.WriteLine($"  Frequency Scale: {settings.SpectrumSettings.FrequencyScale}");
        Console.WriteLine($"  Magnitude Scale: {settings.SpectrumSettings.MagnitudeScale}");
        Console.WriteLine($"  Bar Width: {settings.SpectrumSettings.BarWidth}");
        Console.WriteLine($"  Smoothing Factor: {settings.SpectrumSettings.SmoothingFactor}");
        Console.WriteLine($"  Frequency Range: {settings.SpectrumSettings.FrequencyMin} - " +
                         $"{settings.SpectrumSettings.FrequencyMax} Hz");
        Console.WriteLine($"  Peak Hold: {settings.SpectrumSettings.PeakHold}\n");

        Console.WriteLine("Spectrogram Settings:");
        Console.WriteLine($"  Colormap: {settings.SpectrogramSettings.Colormap}");
        Console.WriteLine($"  Time Window: {settings.SpectrogramSettings.TimeWindowSeconds}s");
        Console.WriteLine($"  Brightness: {settings.SpectrogramSettings.BrightnessMultiplier}x");
        Console.WriteLine($"  Contrast Enhancement: {settings.SpectrogramSettings.ContrastEnhancement}");
        Console.WriteLine($"  Magnitude Range: {settings.SpectrogramSettings.MagnitudeMin} to " +
                         $"{settings.SpectrogramSettings.MagnitudeMax} dB\n");

        var container = ApplicationConfiguration.ConfigureServices(settings);
        var audioService = container.Resolve<AudioCaptureService>();
        var waveformService = container.Resolve<WaveformService>();
        var spectrumAnalyzer = container.Resolve<SpectrumAnalyzer>();

        var frameCount = 0;

        audioService.FrameCaptured += (sender, args) =>
        {
            frameCount++;

            var waveform = waveformService.GenerateWaveform(args.Frame);
            var downsampled = waveformService.DownsampleWaveform(
                waveform,
                targetSamples: 1024
            );

            var spectrum = spectrumAnalyzer.AnalyzeSpectrum(args.Frame, settings.DefaultFftSize);
            spectrumAnalyzer.ConvertToLogScale(spectrum);

            if (frameCount % 30 == 0)
            {
                Console.WriteLine($"[Frame {frameCount}]");
                Console.WriteLine($"  Waveform Peak: {waveform.PeakAmplitude:F4}");
                Console.WriteLine($"  Downsampled Samples: {downsampled.SampleCount}");
                Console.WriteLine($"  Spectrum Bins: {spectrum.FrequencyBins.Length}");
                Console.WriteLine($"  Peak Freq: {spectrum.PeakFrequency:F2} Hz");
                Console.WriteLine();
            }
        };

        try
        {
            Console.WriteLine("Starting audio capture with advanced configuration...\n");
            audioService.Initialize(deviceIndex: 0);
            await audioService.StartRecordingAsync();

            await Task.Delay(TimeSpan.FromSeconds(10));

            await audioService.StopRecordingAsync();
            Console.WriteLine($"\nCapture completed: {frameCount} frames processed");
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

/// <summary>
/// Helper enums for visualization configuration.
/// </summary>
public enum FrequencyScale
{
    Linear,
    Logarithmic
}

public enum MagnitudeScale
{
    Linear,
    Decibel
}

public enum ColormapType
{
    Viridis,
    Plasma,
    Inferno,
    Magma,
    Cividis
}
