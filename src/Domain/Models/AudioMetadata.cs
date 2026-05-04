// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Metadata about the current audio stream being visualized.
/// </summary>
public class AudioMetadata
{
    /// <summary>
    /// Unique identifier for this audio session.
    /// </summary>
    public Guid SessionId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Timestamp when audio capture started.
    /// </summary>
    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp of last audio update.
    /// </summary>
    public DateTime LastUpdateTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Sample rate of the audio stream (Hz).
    /// </summary>
    public int SampleRate { get; set; }

    /// <summary>
    /// Number of audio channels (mono=1, stereo=2).
    /// </summary>
    public int ChannelCount { get; set; }

    /// <summary>
    /// Bit depth (16, 24, 32).
    /// </summary>
    public int BitDepth { get; set; } = 16;

    /// <summary>
    /// Total samples captured so far.
    /// </summary>
    public long TotalSamplesCaptured { get; set; }

    /// <summary>
    /// Total frames processed.
    /// </summary>
    public long TotalFramesProcessed { get; set; }

    /// <summary>
    /// Current audio level (0.0 to 1.0).
    /// </summary>
    public float CurrentLevel { get; set; }

    /// <summary>
    /// Peak audio level recorded during session.
    /// </summary>
    public float PeakLevel { get; set; }

    /// <summary>
    /// Average audio level during session.
    /// </summary>
    public float AverageLevel { get; set; }

    /// <summary>
    /// Current playback duration (seconds).
    /// </summary>
    public double CurrentDurationSeconds { get; private set; }

    /// <summary>
    /// Audio device being used.
    /// </summary>
    public AudioDevice? AudioDevice { get; set; }

    /// <summary>
    /// Whether audio capture is currently active.
    /// </summary>
    public bool IsCapturing { get; set; }

    /// <summary>
    /// CPU usage for audio processing (percentage).
    /// </summary>
    public float CpuUsagePercent { get; set; }

    /// <summary>
    /// Number of buffer underruns (indicates performance issues).
    /// </summary>
    public int BufferUnderruns { get; set; }

    /// <summary>
    /// Frequency with maximum energy in current frame (Hz).
    /// </summary>
    public float DominantFrequency { get; set; }

    /// <summary>
    /// Updates the duration based on samples captured.
    /// </summary>
    public void UpdateDuration()
    {
        CurrentDurationSeconds = (double)TotalSamplesCaptured / SampleRate;
        LastUpdateTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates audio level metrics.
    /// </summary>
    public void UpdateLevelMetrics(float currentLevel)
    {
        CurrentLevel = currentLevel;

        if (currentLevel > PeakLevel)
        {
            PeakLevel = currentLevel;
        }

        // Running average of audio levels
        if (TotalFramesProcessed > 0)
        {
            AverageLevel = (AverageLevel * (TotalFramesProcessed - 1) + currentLevel) / TotalFramesProcessed;
        }
        else
        {
            AverageLevel = currentLevel;
        }
    }

    /// <summary>
    /// Records a buffer underrun event.
    /// </summary>
    public void RecordBufferUnderrun()
    {
        BufferUnderruns++;
    }

    /// <summary>
    /// Resets all metrics to initial state.
    /// </summary>
    public void ResetMetrics()
    {
        StartTime = DateTime.UtcNow;
        LastUpdateTime = DateTime.UtcNow;
        TotalSamplesCaptured = 0;
        TotalFramesProcessed = 0;
        CurrentLevel = 0f;
        PeakLevel = 0f;
        AverageLevel = 0f;
        CurrentDurationSeconds = 0;
        CpuUsagePercent = 0f;
        BufferUnderruns = 0;
        DominantFrequency = 0f;
    }

    /// <summary>
    /// Validates that metadata is consistent.
    /// </summary>
    public bool IsValid()
    {
        return SampleRate > 0 &&
               ChannelCount > 0 &&
               BitDepth > 0 &&
               CurrentLevel >= 0f && CurrentLevel <= 1.0f &&
               PeakLevel >= 0f && PeakLevel <= 1.0f &&
               AverageLevel >= 0f && AverageLevel <= 1.0f;
    }
}
