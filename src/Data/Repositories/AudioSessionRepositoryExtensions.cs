#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using NAudioVisualizer.Domain.Models;

namespace NAudioVisualizer.Data.Repositories;

/// <summary>
/// Extension methods for <see cref="AudioSessionRepository"/> providing additional functionality
/// for working with audio sessions and frames.
/// </summary>
public static class AudioSessionRepositoryExtensions
{
    /// <summary>
    /// Gets the duration of a session in seconds.
    /// </summary>
    public static double GetSessionDurationSeconds(this AudioSessionRepository repository, Guid sessionId)
    {
        if (repository is null)
            throw new ArgumentNullException(nameof(repository));

        var session = repository.GetSession(sessionId);
        if (session is null)
            return 0;

        return session.GetDuration().TotalSeconds;
    }

    /// <summary>
    /// Gets the duration of a session in milliseconds.
    /// </summary>
    public static long GetSessionDurationMilliseconds(this AudioSessionRepository repository, Guid sessionId)
    {
        if (repository is null)
            throw new ArgumentNullException(nameof(repository));

        var session = repository.GetSession(sessionId);
        if (session is null)
            return 0;

        return (long)session.GetDuration().TotalMilliseconds;
    }

    /// <summary>
    /// Gets the average RMS energy across all frames in a session.
    /// </summary>
    public static double GetAverageRmsEnergy(this AudioSessionRepository repository, Guid sessionId)
    {
        if (repository is null)
            throw new ArgumentNullException(nameof(repository));

        var frames = repository.GetSessionFrames(sessionId);
        if (frames.Count == 0)
            return 0;

        return frames.Average(f => f.RmsEnergy);
    }

    /// <summary>
    /// Gets the peak amplitude across all frames in a session.
    /// </summary>
    public static double GetPeakAmplitude(this AudioSessionRepository repository, Guid sessionId)
    {
        if (repository is null)
            throw new ArgumentNullException(nameof(repository));

        var frames = repository.GetSessionFrames(sessionId);
        if (frames.Count == 0)
            return 0;

        return frames.Max(f => f.PeakAmplitude);
    }
}