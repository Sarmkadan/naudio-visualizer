#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.Caching;

/// <summary>
/// Generic cache manager with LRU (Least Recently Used) eviction policy.
/// Provides automatic expiration and memory management for cached items.
/// </summary>
public class CacheManager<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, CacheEntry> _cache;
    private readonly int _maxSize;
    private readonly TimeSpan _defaultExpiration;
    private readonly object _lockObject = new();
    private long _hits;
    private long _misses;
    private long _evictions;
    private long _expirations;

    /// <summary>
    /// Initializes a new instance of the cache manager.
    /// </summary>
    public CacheManager(int maxSize = 1000, TimeSpan? defaultExpiration = null)
    {
        if (maxSize <= 0)
            throw new ArgumentException("Max cache size must be greater than 0.", nameof(maxSize));

        _maxSize = maxSize;
        _defaultExpiration = defaultExpiration ?? TimeSpan.FromHours(1);
        _cache = new Dictionary<TKey, CacheEntry>();
    }

    /// <summary>
    /// Sets a value in the cache.
    /// </summary>
    public void Set(TKey key, TValue value, TimeSpan? expiration = null)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        lock (_lockObject)
        {
            var exp = expiration ?? _defaultExpiration;
            var entry = new CacheEntry
            {
                Value = value,
                ExpiresAt = DateTime.UtcNow.Add(exp),
                CreatedAt = DateTime.UtcNow,
                LastAccessedAt = DateTime.UtcNow
            };

            _cache[key] = entry;

            // Evict old entries if over capacity
            if (_cache.Count > _maxSize)
                EvictOldestEntry();
        }
    }

    /// <summary>
    /// Gets a value from the cache.
    /// Returns false if the key doesn't exist or the entry has expired.
    /// </summary>
    public bool TryGetValue(TKey key, out TValue? value)
    {
        value = default;

        lock (_lockObject)
        {
            if (!_cache.TryGetValue(key, out var entry))
            {
                _misses++;
                return false;
            }

            // Check if expired
            if (DateTime.UtcNow > entry.ExpiresAt)
            {
                _cache.Remove(key);
                _misses++;
                _expirations++;
                return false;
            }

            // Update last accessed time for LRU
            entry.LastAccessedAt = DateTime.UtcNow;
            value = entry.Value;
            _hits++;
            return true;
        }
    }

    /// <summary>
    /// Gets a value with a default fallback.
    /// </summary>
    public TValue? GetOrDefault(TKey key, TValue? defaultValue = default)
    {
        return TryGetValue(key, out var value) ? value : defaultValue;
    }

    /// <summary>
    /// Checks if a key exists in the cache.
    /// </summary>
    public bool Contains(TKey key)
    {
        lock (_lockObject)
        {
            if (!_cache.TryGetValue(key, out var entry))
                return false;

            // Check if expired
            if (DateTime.UtcNow > entry.ExpiresAt)
            {
                if (_cache.Remove(key))
                    _expirations++;

                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Removes a value from the cache.
    /// </summary>
    public bool Remove(TKey key)
    {
        lock (_lockObject)
        {
            return _cache.Remove(key);
        }
    }

    /// <summary>
    /// Gets the current cache size.
    /// </summary>
    public int GetSize()
    {
        lock (_lockObject)
        {
            return _cache.Count;
        }
    }

    /// <summary>
    /// Gets the maximum cache size.
    /// </summary>
    public int GetMaxSize() => _maxSize;

    /// <summary>
    /// Clears all items from the cache.
    /// </summary>
    public void Clear()
    {
        lock (_lockObject)
        {
            _cache.Clear();
        }
    }

    /// <summary>
    /// Removes expired entries from the cache.
    /// </summary>
    public int RemoveExpiredEntries()
    {
        lock (_lockObject)
        {
            var now = DateTime.UtcNow;
            var expiredKeys = new List<TKey>();

            foreach (var kvp in _cache)
            {
                if (now > kvp.Value.ExpiresAt)
                    expiredKeys.Add(kvp.Key);
            }

            foreach (var key in expiredKeys)
            {
                if (_cache.Remove(key))
                    _expirations++;
            }

            return expiredKeys.Count;
        }
    }

    /// <summary>
    /// Evicts the least recently used entry to make room for new items.
    /// </summary>
    private void EvictOldestEntry()
    {
        if (_cache.Count == 0)
            return;

        var oldestKey = default(TKey);
        var oldestTime = DateTime.MaxValue;

        foreach (var kvp in _cache)
        {
            if (kvp.Value.LastAccessedAt < oldestTime)
            {
                oldestKey = kvp.Key;
                oldestTime = kvp.Value.LastAccessedAt;
            }
        }

        if (oldestKey is not null && _cache.Remove(oldestKey))
            _evictions++;
    }

    /// <summary>
    /// Gets cache statistics.
    /// </summary>
    public CacheStatistics GetStatistics()
    {
        lock (_lockObject)
        {
            return new CacheStatistics
            {
                CurrentSize = _cache.Count,
                MaxSize = _maxSize,
                FillPercentage = (double)_cache.Count / _maxSize * 100,
                Hits = _hits,
                Misses = _misses,
                Evictions = _evictions,
                Expirations = _expirations
            };
        }
    }

    /// <summary>
    /// Resets all cache statistics counters.
    /// </summary>
    public void ResetStatistics()
    {
        lock (_lockObject)
        {
            _hits = 0;
            _misses = 0;
            _evictions = 0;
            _expirations = 0;
        }
    }

    /// <summary>
    /// Internal cache entry structure.
    /// </summary>
    private class CacheEntry
    {
        public TValue? Value { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastAccessedAt { get; set; }
    }
}

/// <summary>
/// Cache statistics information.
/// </summary>
public class CacheStatistics
{
    /// <summary>
    /// Gets the current number of cached entries.
    /// </summary>
    public int CurrentSize { get; init; }

    /// <summary>
    /// Gets the maximum number of cached entries.
    /// </summary>
    public int MaxSize { get; init; }

    /// <summary>
    /// Gets the percentage of the cache currently in use.
    /// </summary>
    public double FillPercentage { get; init; }

    /// <summary>
    /// Gets the number of successful cache retrievals.
    /// </summary>
    public long Hits { get; init; }

    /// <summary>
    /// Gets the number of unsuccessful cache retrievals.
    /// </summary>
    public long Misses { get; init; }

    /// <summary>
    /// Gets the number of entries removed by the eviction policy.
    /// </summary>
    public long Evictions { get; init; }

    /// <summary>
    /// Gets the number of expired entries removed from the cache.
    /// </summary>
    public long Expirations { get; init; }

    /// <summary>
    /// Gets the ratio of successful retrievals to total retrieval attempts.
    /// </summary>
    public double HitRate => Hits == 0 && Misses == 0 ? 0 : Hits / ((double)Hits + Misses);
}
