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
using NAudioVisualizer.Data.Repositories;
using NAudioVisualizer.Services;

namespace NAudioVisualizer.Examples;

/// <summary>
/// Example demonstrating session recording and replay functionality.
/// Records audio frames in a session and provides replay capabilities.
/// </summary>
public class SessionRecordingReplayExample
{
    public static async Task Main()
    {
        Console.WriteLine("=== NAudio Visualizer - Session Recording & Replay ===\n");

        var settings = new ApplicationSettings
        {
            DefaultSampleRate = 44100,
            DefaultFftSize = 2048,
            TargetFps = 60,
            MaxFramesPerSession = 10000
        };

        var container = ApplicationConfiguration.ConfigureServices(settings);
        var audioService = container.Resolve<AudioCaptureService>();
        var sessionRepo = container.Resolve<AudioSessionRepository>();
        var logger = container.Resolve<Logger>();

        logger.MinimumLevel = LogLevel.Info;

        var sessionId = Guid.NewGuid().ToString();
        var frameCount = 0;

        Console.WriteLine($"Creating session: {sessionId}\n");

        audioService.FrameCaptured += async (sender, args) =>
        {
            frameCount++;
            await sessionRepo.AddFrameAsync(sessionId, args.Frame);

            if (frameCount % 30 == 0)
            {
                Console.WriteLine($"Recorded {frameCount} frames in session");
            }
        };

        try
        {
            Console.WriteLine("Recording Phase: Capturing 10 seconds of audio...\n");

            audioService.Initialize(deviceIndex: 0);
            await audioService.StartRecordingAsync();
            await Task.Delay(TimeSpan.FromSeconds(10));
            await audioService.StopRecordingAsync();

            Console.WriteLine($"\nRecording completed: {frameCount} frames captured\n");

            Console.WriteLine("Replay Phase: Retrieving and analyzing session data...\n");

            var session = sessionRepo.GetSession(sessionId);
            if (session is null)
            {
                Console.WriteLine("Session not found!");
                return;
            }

            Console.WriteLine($"Session Information:");
            Console.WriteLine($"  ID: {session.SessionId}");
            Console.WriteLine($"  Start Time: {session.StartTime}");
            Console.WriteLine($"  Duration: {session.Duration.TotalSeconds:F2}s");
            Console.WriteLine($"  Frame Count: {session.FrameCount}\n");

            var frames = sessionRepo.GetSessionFrames(sessionId);
            Console.WriteLine($"Retrieved {frames.Count} frames from session\n");

            Console.WriteLine("First 5 frames analysis:");
            for (int i = 0; i < Math.Min(5, frames.Count); i++)
            {
                var frame = frames[i];
                Console.WriteLine($"  Frame {i}:");
                Console.WriteLine($"    Timestamp: {frame.Timestamp}");
                Console.WriteLine($"    Sample Rate: {frame.SampleRate} Hz");
                Console.WriteLine($"    Channels: {frame.ChannelCount}");
                Console.WriteLine($"    Samples: {frame.SampleCount}");
            }

            Console.WriteLine($"\nLast 5 frames analysis:");
            var startIdx = Math.Max(0, frames.Count - 5);
            for (int i = startIdx; i < frames.Count; i++)
            {
                var frame = frames[i];
                Console.WriteLine($"  Frame {i}:");
                Console.WriteLine($"    Timestamp: {frame.Timestamp}");
                Console.WriteLine($"    Duration: {(frames[i].SampleCount / (double)frame.SampleRate) * 1000:F2}ms");
            }

            Console.WriteLine("\nSession recording and replay completed successfully!");
        }
        catch (Exception ex)
        {
            logger.Error($"Session error: {ex.Message}");
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            container.Dispose();
        }
    }
}
