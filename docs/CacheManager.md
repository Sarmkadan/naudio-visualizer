# CacheManager

A utility class for managing a size-constrained, time-sensitive in-memory cache of key-value pairs. Items can be automatically expired based on access time or absolute lifetime, and the cache enforces a maximum size by removing least-recently-used entries when capacity is exceeded. The class is designed for scenarios requiring fast lookups and bounded memory usage, such as caching audio metadata or visualization data in real-time applications.

## API

### `CacheManager(int maxSize)`

Constructs a new `CacheManager` instance with the specified maximum number of entries.

- **Parameters**
  - `maxSize`: The maximum number of entries the cache can hold. Must be non-negative.
- **Throws**
  - `ArgumentOutOfRangeException`: Thrown if `maxSize` is negative.

---

### `void Set(TKey key, TValue value, TimeSpan? lifetime = null)`

Inserts or updates a key-value pair in the cache. If the key already exists, its value, expiration time, and access timestamps are updated. If the cache is at maximum capacity, the least-recently-used entry is removed before insertion.

- **Parameters**
  - `key`: The key identifying the entry.
  - `value`: The value to store.
  - `lifetime`: Optional time span after which the entry expires. If `null`, the entry does not expire based on time.
- **Throws**
  - `ArgumentNullException`: Thrown if `key` is `null`.
  - `InvalidOperationException`: Thrown if the cache is corrupted or in an invalid state.

---

### `bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value)`

Retrieves the value associated with the specified key if it exists and has not expired.

- **Parameters**
  - `key`: The key of the entry to retrieve.
  - `value`: When this method returns, contains the associated value if found and valid; otherwise, `null`.
- **Returns**
  - `true` if the key was found and the entry has not expired; otherwise, `false`.
- **Throws**
  - `ArgumentNullException`: Thrown if `key` is `null`.

---

### `TValue? GetOrDefault(TKey key)`

Retrieves the value associated with the specified key if it exists and has not expired; otherwise, returns the default value for `TValue`.

- **Parameters**
  - `key`: The key of the entry to retrieve.
- **Returns**
  - The value if found and valid; otherwise, `default(TValue)`.
- **Throws**
  - `ArgumentNullException`: Thrown if `key` is `null`.

---

### `bool Contains(TKey key)`

Determines whether the cache contains a specific key and the corresponding entry has not expired.

- **Parameters**
  - `key`: The key to locate in the cache.
- **Returns**
  - `true` if the key exists and the entry is valid; otherwise, `false`.
- **Throws**
  - `ArgumentNullException`: Thrown if `key` is `null`.

---

### `bool Remove(TKey key)`

Removes the entry with the specified key from the cache if it exists and has not expired.

- **Parameters**
  - `key`: The key of the entry to remove.
- **Returns**
  - `true` if the entry was found and removed; otherwise, `false`.
- **Throws**
  - `ArgumentNullException`: Thrown if `key` is `null`.

---
### `int GetSize()`

Returns the current number of valid entries in the cache.

- **Returns**
  - The number of entries that are present and have not expired.

---
### `int GetMaxSize()`

Returns the maximum number of entries the cache can hold.

- **Returns**
  - The configured maximum size.

---
### `void Clear()`

Removes all entries from the cache.

---
### `int RemoveExpiredEntries()`

Removes all entries that have expired based on their lifetime and returns the number of entries removed.

- **Returns**
  - The number of entries removed.

---
### `CacheStatistics GetStatistics()`

Returns a snapshot of cache statistics, including size, capacity, and usage metrics.

- **Returns**
  - A `CacheStatistics` object containing current cache metrics.

---
### `TValue? Value` (property)

Gets the value of the cache entry.

- **Value**
  - The stored value, or `null` if the entry is expired or invalid.

---
### `DateTime ExpiresAt` (property)

Gets the absolute expiration time of the cache entry.

- **Value**
  - The expiration timestamp, or `DateTime.MinValue` if the entry does not expire.

---
### `DateTime CreatedAt` (property)

Gets the creation timestamp of the cache entry.

- **Value**
  - The timestamp when the entry was added.

---
### `DateTime LastAccessedAt` (property)

Gets the last access timestamp of the cache entry.

- **Value**
  - The timestamp when the entry was last accessed.

---
### `int CurrentSize` (property)

Gets the current number of entries in the cache, including expired ones.

- **Value**
  - The total number of entries, regardless of expiration.

---
### `int MaxSize` (property)

Gets the maximum number of entries the cache can hold.

- **Value**
  - The configured maximum size.

---
### `double FillPercentage` (property)

Gets the percentage of the cache that is currently filled, including expired entries.

- **Value**
  - A value between 0.0 and 1.0 representing the fill ratio.

## Usage
