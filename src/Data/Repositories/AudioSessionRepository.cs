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

    public TimeSpan GetDuration()
    {
        var end = EndTime ?? DateTime.UtcNow;
        return end - StartTime;
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
}
