// <copyright file="CacheManagerExtensions.cs" company="NAudioVisualizer">
//     Copyright (c) NAudioVisualizer. All rights reserved.
// </copyright>

namespace NAudioVisualizer.Caching
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods for <see cref="CacheManager{TKey, TValue}"/>.
    /// </summary>
    public static class CacheManagerExtensions
    {
        /// <summary>
        /// Gets the value associated with the specified key from the cache, or adds it using the factory if not present.
        /// </summary>
        /// <param name="cache">The cache manager instance.</param>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="factory">The function used to generate a value for the key.</param>
        /// <param name="expiration">Optional expiration time for the newly added item.</param>
        /// <returns>The value for the key, either retrieved or newly generated.</returns>
        public static TValue GetOrAdd<TKey, TValue>(this CacheManager<TKey, TValue> cache, TKey key, Func<TKey, TValue> factory, TimeSpan? expiration = null)
            where TKey : notnull
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            if (cache.TryGetValue(key, out TValue? value))
            {
                return value!;
            }

            var newValue = factory(key);
            cache.Set(key, newValue, expiration);
            return newValue;
        }

        /// <summary>
        /// Sets multiple key-value pairs in the cache.
        /// </summary>
        /// <param name="cache">The cache manager instance.</param>
        /// <param name="items">The collection of key-value pairs to set.</param>
        public static void SetMany<TKey, TValue>(this CacheManager<TKey, TValue> cache, IEnumerable<KeyValuePair<TKey, TValue>> items)
            where TKey : notnull
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (items == null) throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                cache.Set(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Determines whether the cache is full.
        /// </summary>
        /// <param name="cache">The cache manager instance.</param>
        /// <returns><c>true</c> if the cache size is greater than or equal to the maximum size; otherwise, <c>false</c>.</returns>
        public static bool IsFull<TKey, TValue>(this CacheManager<TKey, TValue> cache)
            where TKey : notnull
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));

            return cache.GetSize() >= cache.GetMaxSize();
        }
    }
}
