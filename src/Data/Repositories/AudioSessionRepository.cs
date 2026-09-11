#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using NAudioVisualizer.Domain.Models;

namespace NAudioVisualizer.Data.Repositories;

/// <summary>
/// Repository for managing audio session metadata and frame data.
/// </summary>
public class AudioSessionRepository
{
    private readonly Dictionary<Guid, AudioSessionData> _sessions = [];
    private readonly Dictionary<Guid, List<AudioFrame>> _frameStore = [];
    private readonly object _lock = new();
    private int _maxFramesPerSession = 5000;

    /// <summary>
    /// Creates and stores a new audio session.
    /// </summary>
    /// <param name="metadata">The metadata for the new session.</param>
    /// <returns>The created audio session data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when metadata is null.</exception>
    public AudioSessionData CreateSession(AudioMetadata metadata)
    {
        if (metadata is null)
            throw new ArgumentNullException(nameof(metadata));

        lock (_lock)
        {
            var session = new AudioSessionData
            {
                SessionId = metadata.SessionId,
                StartTime = metadata.StartTime,
                Device = metadata.AudioDevice,
                SampleRate = metadata.SampleRate,
                ChannelCount = metadata.ChannelCount
            };

            _sessions[session.SessionId] = session;
            _frameStore[session.SessionId] = [];

            return session;
        }
    }

    /// <summary>
    /// Gets a session by ID.
    /// </summary>
    /// <param name="sessionId">The ID of the session to retrieve.</param>
    /// <returns>The audio session data if found; otherwise, null.</returns>
    public AudioSessionData? GetSession(Guid sessionId)
    {
        lock (_lock)
        {
            return _sessions.ContainsKey(sessionId) ? _sessions[sessionId] : null;
        }
    }

    /// <summary>
    /// Gets all active sessions.
    /// </summary>
    /// <returns>A read-only list of all audio session data.</returns>
    public IReadOnlyList<AudioSessionData> GetAllSessions()
    {
        lock (_lock)
        {
            return _sessions.Values.ToList().AsReadOnly();
        }
    }

    /// <summary>
    /// Adds an audio frame to a session.
    /// </summary>
    /// <param name="sessionId">The ID of the session to add the frame to.</param>
    /// <param name="frame">The audio frame to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when frame is null.</exception>
    public void AddFrameToSession(Guid sessionId, AudioFrame frame)
    {
        if (frame is null)
            throw new ArgumentNullException(nameof(frame));

        lock (_lock)
        {
            if (!_frameStore.ContainsKey(sessionId))
            {
                _frameStore[sessionId] = [];
            }

            _frameStore[sessionId].Add(frame);

            // Enforce max frames per session
            if (_frameStore[sessionId].Count > _maxFramesPerSession)
            {
                _frameStore[sessionId].RemoveAt(0);
            }

            // Update session stats
            if (_sessions.ContainsKey(sessionId))
            {
                var session = _sessions[sessionId];
                session.FrameCount = _frameStore[sessionId].Count;
                session.LastFrameTime = DateTime.UtcNow;
            }
        }
    }

    /// <summary>
    /// Gets all frames for a session.
    /// </summary>
    /// <param name="sessionId">The ID of the session to retrieve frames for.</param>
    /// <returns>A read-only list of audio frames for the specified session.</returns>
    public IReadOnlyList<AudioFrame> GetSessionFrames(Guid sessionId)
    {
        lock (_lock)
        {
            if (_frameStore.ContainsKey(sessionId))
            {
                return _frameStore[sessionId].AsReadOnly();
            }
            return [];
        }
    }

    /// <summary>
    /// Gets a specific frame by session and frame index.
    /// </summary>
    /// <param name="sessionId">The ID of the session containing the frame.</param>
    /// <param name="frameIndex">The zero-based index of the frame to retrieve.</param>
    /// <returns>The audio frame at the specified index if found; otherwise, null.</returns>
    public AudioFrame? GetFrame(Guid sessionId, int frameIndex)
    {
        lock (_lock)
        {
            if (_frameStore.ContainsKey(sessionId) && frameIndex >= 0 && frameIndex < _frameStore[sessionId].Count)
            {
                return _frameStore[sessionId][frameIndex];
            }
            return null;
        }
    }

    /// <summary>
    /// Gets frames within a time range.
    /// </summary>
    /// <param name="sessionId">The ID of the session to retrieve frames from.</param>
    /// <param name="startTime">The inclusive start of the time range.</param>
    /// <param name="endTime">The inclusive end of the time range.</param>
    /// <returns>A read-only list of audio frames whose timestamps fall within the specified range.</returns>
    public IReadOnlyList<AudioFrame> GetFramesInTimeRange(Guid sessionId, DateTime startTime, DateTime endTime)
    {
        lock (_lock)
        {
            if (!_frameStore.ContainsKey(sessionId))
                return [];

            return _frameStore[sessionId]
                .Where(f => f.Timestamp >= startTime && f.Timestamp <= endTime)
                .ToList()
                .AsReadOnly();
        }
    }

    /// <summary>
    /// Gets the most recent frames from a session.
    /// </summary>
    /// <param name="sessionId">The ID of the session to retrieve frames from.</param>
    /// <param name="count">The maximum number of most recent frames to return.</param>
    /// <returns>A read-only list of the most recent audio frames for the specified session.</returns>
    public IReadOnlyList<AudioFrame> GetRecentFrames(Guid sessionId, int count)
    {
        lock (_lock)
        {
            if (!_frameStore.ContainsKey(sessionId))
                return [];

            var frames = _frameStore[sessionId];
            int startIndex = Math.Max(0, frames.Count - count);
            return frames.Skip(startIndex).ToList().AsReadOnly();
        }
    }

    /// <summary>
    /// Ends a session.
    /// </summary>
    /// <param name="sessionId">The ID of the session to end.</param>
    public void EndSession(Guid sessionId)
    {
        lock (_lock)
        {
            if (_sessions.ContainsKey(sessionId))
            {
                var session = _sessions[sessionId];
                session.EndTime = DateTime.UtcNow;
            }
        }
    }

    /// <summary>
    /// Deletes a session and its frames.
    /// </summary>
    /// <param name="sessionId">The ID of the session to delete.</param>
    /// <returns>True if the session or its frames were deleted; otherwise, false.</returns>
    public bool DeleteSession(Guid sessionId)
    {
        lock (_lock)
        {
            bool sessionDeleted = _sessions.Remove(sessionId);
            bool framesDeleted = _frameStore.Remove(sessionId);
            return sessionDeleted || framesDeleted;
        }
    }

    /// <summary>
    /// Gets the frame count for a session.
    /// </summary>
    /// <param name="sessionId">The ID of the session to get the frame count for.</param>
    /// <returns>The number of frames stored for the specified session.</returns>
    public int GetFrameCount(Guid sessionId)
    {
        lock (_lock)
        {
            if (_frameStore.ContainsKey(sessionId))
            {
                return _frameStore[sessionId].Count;
            }
            return 0;
        }
    }

    /// <summary>
    /// Sets the maximum number of frames to keep per session.
    /// </summary>
    /// <param name="maxFrames">The maximum number of frames to store per session. Must be positive.</param>
    /// <exception cref="ArgumentException">Thrown when maxFrames is less than or equal to zero.</exception>
    public void SetMaxFramesPerSession(int maxFrames)
    {
        if (maxFrames <= 0)
            throw new ArgumentException("Max frames must be positive", nameof(maxFrames));

        lock (_lock)
        {
            _maxFramesPerSession = maxFrames;

            // Trim existing sessions
            foreach (var sessionId in _frameStore.Keys.ToList())
            {
                while (_frameStore[sessionId].Count > maxFrames)
                {
                    _frameStore[sessionId].RemoveAt(0);
                }
            }
        }
    }

    /// <summary>
    /// Gets repository statistics.
    /// </summary>
    /// <returns>The current repository statistics.</returns>
    public SessionRepositoryStats GetStats()
    {
        lock (_lock)
        {
            var stats = new SessionRepositoryStats
            {
                TotalSessionCount = _sessions.Count,
                TotalFrameCount = _frameStore.Values.Sum(f => f.Count),
                ActiveSessionCount = _sessions.Values.Count(s => s.EndTime is null),
                CompletedSessionCount = _sessions.Values.Count(s => s.EndTime is not null),
                MaxFramesPerSession = _maxFramesPerSession
            };

            return stats;
        }
    }

    /// <summary>
    /// Clears all data.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _sessions.Clear();
            _frameStore.Clear();
        }
    }
}

/// <summary>
/// Represents an audio recording session.
/// </summary>
public class AudioSessionData
{
    public Guid SessionId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public AudioDevice? Device { get; set; }
    public int SampleRate { get; set; }
    public int ChannelCount { get; set; }
    public int FrameCount { get; set; }
    public DateTime LastFrameTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the duration of the session.
    /// </summary>
    /// <returns>The elapsed time from the session start to its end, or to the current time if the session is still active.</returns>
    public TimeSpan GetDuration()
    {
        var end = EndTime ?? DateTime.UtcNow;
        return end - StartTime;
    }

    /// <summary>
    /// Returns a string representation of the session.
    /// </summary>
    /// <returns>A string describing the session ID, start, end, and frame count.</returns>
    public override string ToString()
    {
        return $"SessionId={SessionId}, Start={StartTime}, End={EndTime ?? DateTime.MinValue}, Frames={FrameCount}";
    }
}

/// <summary>
/// Statistics about session repository.
/// </summary>
public class SessionRepositoryStats
{
    public int TotalSessionCount { get; set; }
    public int TotalFrameCount { get; set; }
    public int ActiveSessionCount { get; set; }
    public int CompletedSessionCount { get; set; }
    public int MaxFramesPerSession { get; set; }

    /// <summary>
    /// Returns a string representation of the repository statistics.
    /// </summary>
    /// <returns>A string describing the session counts, frame count, and max frames per session.</returns>
    public override string ToString()
    {
        return $"TotalSessions={TotalSessionCount}, Active={ActiveSessionCount}, Completed={CompletedSessionCount}, TotalFrames={TotalFrameCount}, MaxFramesPerSession={MaxFramesPerSession}";
    }
}
