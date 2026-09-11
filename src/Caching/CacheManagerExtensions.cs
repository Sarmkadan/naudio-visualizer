#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.Caching
{
    /// <summary>
    /// Extension methods for <see cref="CacheManager{TKey,TValue}"/>.
    /// </summary>
    public static class CacheManagerExtensions
    {
        /// <summary>
        /// Gets the value associated with the specified key, or adds a new value using the factory function if the key is not present.
        /// </summary>
        /// <param name="cache">The cache manager instance.</param>
        /// <param name="key">The key of the value to get or add.</param>
        /// <param name="factory">A function that creates a new value for the key.</param>
        /// <param name="expiration">Optional expiration time for the cached value. If null, uses the cache's default expiration.</param>
        /// <returns>The value associated with the key.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="cache"/>, <paramref name="key"/>, or <paramref name="factory"/> is null.</exception>
        public static TValue GetOrAdd<TKey, TValue>(this CacheManager<TKey, TValue> cache, TKey key, Func<TKey, TValue> factory, TimeSpan? expiration = null)
            where TKey : notnull
        {
            if (cache is null)
                throw new ArgumentNullException(nameof(cache));
            if (key is null)
                throw new ArgumentNullException(nameof(key));
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));

            if (cache.TryGetValue(key, out var value))
                return value;

            var newValue = factory(key);
            cache.Set(key, newValue, expiration);
            return newValue;
        }

        /// <summary>
        /// Sets multiple key-value pairs in the cache.
        /// </summary>
        /// <param name="cache">The cache manager instance.</param>
        /// <param name="items">The key-value pairs to set in the cache.</param>
        /// <param name="expiration">Optional expiration time for the cached values. If null, uses the cache's default expiration.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="cache"/> or <paramref name="items"/> is null.</exception>
        public static void SetMany<TKey, TValue>(this CacheManager<TKey, TValue> cache, IEnumerable<KeyValuePair<TKey, TValue>> items, TimeSpan? expiration = null)
            where TKey : notnull
        {
            if (cache is null)
                throw new ArgumentNullException(nameof(cache));
            if (items is null)
                throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                cache.Set(item.Key, item.Value, expiration);
            }
        }

        /// <summary>
        /// Determines whether the cache is full (i.e., current size has reached or exceeded maximum size).
        /// </summary>
        /// <param name="cache">The cache manager instance.</param>
        /// <returns>true if the cache is full; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="cache"/> is null.</exception>
        public static bool IsFull<TKey, TValue>(this CacheManager<TKey, TValue> cache)
            where TKey : notnull
        {
            if (cache is null)
                throw new ArgumentNullException(nameof(cache));
            return cache.GetSize() >= cache.GetMaxSize();
        }
    }
}