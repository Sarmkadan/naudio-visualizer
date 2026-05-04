// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.Middleware;

/// <summary>
/// Rate limiting middleware using token bucket algorithm.
/// Prevents abuse by limiting operation frequency per client or globally.
/// </summary>
public class RateLimitMiddleware
{
    private readonly Dictionary<string, TokenBucket> _clientBuckets;
    private readonly int _requestsPerSecond;
    private readonly int _burstSize;
    private readonly object _lockObject = new();

    /// <summary>
    /// Initializes a new instance of the rate limit middleware.
    /// </summary>
    public RateLimitMiddleware(int requestsPerSecond = 100, int burstSize = 10)
    {
        if (requestsPerSecond <= 0)
            throw new ArgumentException("Requests per second must be greater than 0.", nameof(requestsPerSecond));

        _requestsPerSecond = requestsPerSecond;
        _burstSize = Math.Max(burstSize, 1);
        _clientBuckets = new Dictionary<string, TokenBucket>();
    }

    /// <summary>
    /// Checks if an operation is allowed based on rate limits.
    /// Returns true if the operation should be allowed, false if rate limit is exceeded.
    /// </summary>
    public bool IsAllowed(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            throw new ArgumentException("Client ID cannot be null or empty.", nameof(clientId));

        lock (_lockObject)
        {
            if (!_clientBuckets.TryGetValue(clientId, out var bucket))
            {
                bucket = new TokenBucket(_requestsPerSecond, _burstSize);
                _clientBuckets[clientId] = bucket;
            }

            return bucket.TryConsumeToken();
        }
    }

    /// <summary>
    /// Gets the remaining tokens for a client.
    /// </summary>
    public int GetRemainingTokens(string clientId)
    {
        lock (_lockObject)
        {
            if (_clientBuckets.TryGetValue(clientId, out var bucket))
                return (int)bucket.GetAvailableTokens();

            return _burstSize;
        }
    }

    /// <summary>
    /// Gets the wait time in milliseconds before the next request is allowed.
    /// </summary>
    public int GetWaitTimeMs(string clientId)
    {
        lock (_lockObject)
        {
            if (_clientBuckets.TryGetValue(clientId, out var bucket))
                return bucket.GetWaitTimeMs();

            return 0;
        }
    }

    /// <summary>
    /// Resets the rate limit for a specific client.
    /// </summary>
    public void ResetClient(string clientId)
    {
        lock (_lockObject)
        {
            _clientBuckets.Remove(clientId);
        }
    }

    /// <summary>
    /// Resets all rate limits.
    /// </summary>
    public void ResetAll()
    {
        lock (_lockObject)
        {
            _clientBuckets.Clear();
        }
    }

    /// <summary>
    /// Gets statistics about rate limit usage.
    /// </summary>
    public RateLimitStats GetStats()
    {
        lock (_lockObject)
        {
            int totalClients = _clientBuckets.Count;
            int throttledClients = 0;

            foreach (var bucket in _clientBuckets.Values)
            {
                if (bucket.GetAvailableTokens() == 0)
                    throttledClients++;
            }

            return new RateLimitStats
            {
                TotalClients = totalClients,
                ThrottledClients = throttledClients,
                RequestsPerSecond = _requestsPerSecond,
                BurstSize = _burstSize
            };
        }
    }

    /// <summary>
    /// Internal token bucket implementation.
    /// </summary>
    private class TokenBucket
    {
        private double _tokens;
        private DateTime _lastRefillTime;
        private readonly double _tokensPerSecond;
        private readonly double _maxTokens;

        public TokenBucket(int tokensPerSecond, int maxTokens)
        {
            _tokensPerSecond = tokensPerSecond;
            _maxTokens = maxTokens;
            _tokens = maxTokens;
            _lastRefillTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Attempts to consume a token. Returns true if successful, false if no tokens available.
        /// </summary>
        public bool TryConsumeToken()
        {
            RefillTokens();

            if (_tokens >= 1.0)
            {
                _tokens -= 1.0;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the number of available tokens.
        /// </summary>
        public double GetAvailableTokens()
        {
            RefillTokens();
            return _tokens;
        }

        /// <summary>
        /// Gets the wait time in milliseconds until the next token is available.
        /// </summary>
        public int GetWaitTimeMs()
        {
            RefillTokens();

            if (_tokens >= 1.0)
                return 0;

            // Calculate how long until we have 1 token
            double tokensNeeded = 1.0 - _tokens;
            double secondsToWait = tokensNeeded / _tokensPerSecond;
            return (int)(secondsToWait * 1000) + 1;
        }

        /// <summary>
        /// Refills tokens based on elapsed time.
        /// </summary>
        private void RefillTokens()
        {
            var now = DateTime.UtcNow;
            var timeSinceLastRefill = now - _lastRefillTime;
            var tokensToAdd = timeSinceLastRefill.TotalSeconds * _tokensPerSecond;

            _tokens = Math.Min(_maxTokens, _tokens + tokensToAdd);
            _lastRefillTime = now;
        }
    }
}

/// <summary>
/// Statistics about rate limiting.
/// </summary>
public class RateLimitStats
{
    public int TotalClients { get; init; }
    public int ThrottledClients { get; init; }
    public int RequestsPerSecond { get; init; }
    public int BurstSize { get; init; }

    /// <summary>
    /// Gets the percentage of clients that are currently throttled.
    /// </summary>
    public double GetThrottlePercentage()
    {
        if (TotalClients == 0)
            return 0;

        return (double)ThrottledClients / TotalClients * 100;
    }
}
