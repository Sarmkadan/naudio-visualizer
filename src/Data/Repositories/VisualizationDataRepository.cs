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
/// Repository for managing visualization data persistence and queries.
/// </summary>
public class VisualizationDataRepository
{
    private readonly Dictionary<Guid, VisualizationData> _store = [];
    private readonly Dictionary<Guid, List<VisualizationData>> _sessionIndex = [];
    private readonly object _lock = new();

    /// <summary>
    /// Stores a visualization data object.
    /// </summary>
    public void Store(VisualizationData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        lock (_lock)
        {
            _store[data.Id] = data;

            // Index by session if source frame exists
            if (data.SourceFrame != null)
            {
                var sessionId = data.SourceFrame.Id;
                if (!_sessionIndex.ContainsKey(sessionId))
                {
                    _sessionIndex[sessionId] = [];
                }
                _sessionIndex[sessionId].Add(data);
            }
        }
    }

    /// <summary>
    /// Retrieves a visualization data by ID.
    /// </summary>
    public VisualizationData? GetById(Guid id)
    {
        lock (_lock)
        {
            return _store.ContainsKey(id) ? _store[id] : null;
        }
    }

    /// <summary>
    /// Gets all visualization data for a specific session.
    /// </summary>
    public IReadOnlyList<VisualizationData> GetBySession(Guid sessionId)
    {
        lock (_lock)
        {
            if (_sessionIndex.ContainsKey(sessionId))
            {
                return _sessionIndex[sessionId].AsReadOnly();
            }
            return [];
        }
    }

    /// <summary>
    /// Gets all visualizations of a specific type.
    /// </summary>
    public IReadOnlyList<VisualizationData> GetByType(VisualizationType type)
    {
        lock (_lock)
        {
            return _store.Values
                .Where(v => v.VisualizationType == type)
                .ToList()
                .AsReadOnly();
        }
    }

    /// <summary>
    /// Gets the most recent visualization of a specific type.
    /// </summary>
    public VisualizationData? GetMostRecent(VisualizationType type)
    {
        lock (_lock)
        {
            return _store.Values
                .Where(v => v.VisualizationType == type)
                .OrderByDescending(v => v.GeneratedAt)
                .FirstOrDefault();
        }
    }

    /// <summary>
    /// Gets all stored visualization data.
    /// </summary>
    public IReadOnlyList<VisualizationData> GetAll()
    {
        lock (_lock)
        {
            return _store.Values.ToList().AsReadOnly();
        }
    }

    /// <summary>
    /// Deletes a visualization by ID.
    /// </summary>
    public bool Delete(Guid id)
    {
        lock (_lock)
        {
            if (_store.ContainsKey(id))
            {
                var data = _store[id];
                _store.Remove(id);

                // Remove from session index
                if (data.SourceFrame != null)
                {
                    var sessionId = data.SourceFrame.Id;
                    if (_sessionIndex.ContainsKey(sessionId))
                    {
                        _sessionIndex[sessionId].Remove(data);
                        if (_sessionIndex[sessionId].Count == 0)
                        {
                            _sessionIndex.Remove(sessionId);
                        }
                    }
                }

                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Deletes all visualizations for a session.
    /// </summary>
    public int DeleteBySession(Guid sessionId)
    {
        lock (_lock)
        {
            if (!_sessionIndex.ContainsKey(sessionId))
                return 0;

            var visualizations = _sessionIndex[sessionId].ToList();
            int count = 0;

            foreach (var vis in visualizations)
            {
                if (_store.Remove(vis.Id))
                    count++;
            }

            _sessionIndex.Remove(sessionId);
            return count;
        }
    }

    /// <summary>
    /// Clears all stored data.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _store.Clear();
            _sessionIndex.Clear();
        }
    }

    /// <summary>
    /// Gets the total count of stored visualizations.
    /// </summary>
    public int Count()
    {
        lock (_lock)
        {
            return _store.Count;
        }
    }

    /// <summary>
    /// Gets statistics about stored visualizations.
    /// </summary>
    public RepositoryStats GetStats()
    {
        lock (_lock)
        {
            var stats = new RepositoryStats
            {
                TotalCount = _store.Count,
                WaveformCount = _store.Values.Count(v => v.VisualizationType == VisualizationType.Waveform),
                SpectrumCount = _store.Values.Count(v => v.VisualizationType == VisualizationType.Spectrum),
                SpectrogramCount = _store.Values.Count(v => v.VisualizationType == VisualizationType.Spectrogram),
                SessionCount = _sessionIndex.Count,
                OldestEntry = _store.Values.OrderBy(v => v.GeneratedAt).FirstOrDefault()?.GeneratedAt,
                NewestEntry = _store.Values.OrderByDescending(v => v.GeneratedAt).FirstOrDefault()?.GeneratedAt
            };

            return stats;
        }
    }

    /// <summary>
    /// Prunes old entries keeping only the most recent count.
    /// </summary>
    public int PruneOldest(int keepCount)
    {
        lock (_lock)
        {
            if (_store.Count <= keepCount)
                return 0;

            int removeCount = _store.Count - keepCount;
            var toRemove = _store.Values
                .OrderBy(v => v.GeneratedAt)
                .Take(removeCount)
                .ToList();

            foreach (var item in toRemove)
            {
                Delete(item.Id);
            }

            return removeCount;
        }
    }
}

/// <summary>
/// Statistics about repository contents.
/// </summary>
public class RepositoryStats
{
    public int TotalCount { get; set; }
    public int WaveformCount { get; set; }
    public int SpectrumCount { get; set; }
    public int SpectrogramCount { get; set; }
    public int SessionCount { get; set; }
    public DateTime? OldestEntry { get; set; }
    public DateTime? NewestEntry { get; set; }
}
