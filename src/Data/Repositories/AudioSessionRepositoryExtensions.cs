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
    /// <param name="repository">The audio session repository instance.</param>
    /// <param name="sessionId">The unique identifier of the audio session.</param>
    /// <returns>The duration of the session in seconds, or 0 if the session does not exist.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="repository"/> is <see langword="null"/></exception>
    public static double GetSessionDurationSeconds(this AudioSessionRepository repository, Guid sessionId)
    {
        ArgumentNullException.ThrowIfNull(repository);

        var session = repository.GetSession(sessionId);
        return session?.GetDuration().TotalSeconds ?? 0;
    }

    /// <summary>
    /// Gets the duration of a session in milliseconds.
    /// </summary>
    /// <param name="repository">The audio session repository instance.</param>
    /// <param name="sessionId">The unique identifier of the audio session.</param>
    /// <returns>The duration of the session in milliseconds, or 0 if the session does not exist.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="repository"/> is <see langword="null"/></exception>
    public static long GetSessionDurationMilliseconds(this AudioSessionRepository repository, Guid sessionId)
    {
        ArgumentNullException.ThrowIfNull(repository);

        var session = repository.GetSession(sessionId);
        return session is null ? 0L : (long)session.GetDuration().TotalMilliseconds;
    }

    /// <summary>
    /// Gets the average RMS energy across all frames in a session.
    /// </summary>
    /// <param name="repository">The audio session repository instance.</param>
    /// <param name="sessionId">The unique identifier of the audio session.</param>
    /// <returns>The average RMS energy across all frames, or 0 if no frames exist.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="repository"/> is <see langword="null"/></exception>
    public static double GetAverageRmsEnergy(this AudioSessionRepository repository, Guid sessionId)
    {
        ArgumentNullException.ThrowIfNull(repository);

        var frames = repository.GetSessionFrames(sessionId);
        return frames.Count == 0 ? 0 : frames.Average(f => f.RmsEnergy);
    }

    /// <summary>
    /// Gets the peak amplitude across all frames in a session.
    /// </summary>
    /// <param name="repository">The audio session repository instance.</param>
    /// <param name="sessionId">The unique identifier of the audio session.</param>
    /// <returns>The peak amplitude across all frames, or 0 if no frames exist.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="repository"/> is <see langword="null"/></exception>
    public static double GetPeakAmplitude(this AudioSessionRepository repository, Guid sessionId)
    {
        ArgumentNullException.ThrowIfNull(repository);

        var frames = repository.GetSessionFrames(sessionId);
        return frames.Count == 0 ? 0 : frames.Max(f => f.PeakAmplitude);
    }
}