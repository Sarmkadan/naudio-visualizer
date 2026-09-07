# CacheStatistics

The `CacheStatistics` class represents a snapshot of the size and activity counters maintained by a `CacheManager<TKey, TValue>`. The stored properties are init-only, and `HitRate` is calculated from the snapshot's `Hits` and `Misses` values whenever it is read.

## API

### Properties

*   **`public int CurrentSize { get; init; }`**
    Gets the number of entries in the cache when `GetStatistics()` creates the snapshot. This includes entries that have expired but have not yet been detected and removed.

*   **`public int MaxSize { get; init; }`**
    Gets the maximum cache size configured when the `CacheManager<TKey, TValue>` was constructed.

*   **`public double FillPercentage { get; init; }`**
    Gets the percentage of the configured capacity occupied when the snapshot is created. `GetStatistics()` calculates it as `(double)CurrentSize / MaxSize * 100`.

*   **`public long Hits { get; init; }`**
    Gets the number of successful `TryGetValue` retrievals recorded by the cache manager. Successful retrievals performed through `GetOrDefault` also count because that method calls `TryGetValue`.

*   **`public long Misses { get; init; }`**
    Gets the number of unsuccessful `TryGetValue` retrievals. Both absent keys and entries found to be expired increment this counter. Unsuccessful retrievals through `GetOrDefault` also count.

*   **`public long Evictions { get; init; }`**
    Gets the number of entries removed by the least-recently-used eviction logic after a `Set` operation causes the cache to exceed its maximum size.

*   **`public long Expirations { get; init; }`**
    Gets the number of expired entries removed by `TryGetValue`, `Contains`, or `RemoveExpiredEntries`.

*   **`public double HitRate`**
    Gets the ratio of successful retrievals to all recorded retrieval attempts. The formula is:

    ```text
    Hits / (Hits + Misses)
    ```

    The calculation uses floating-point division and returns `0` when both `Hits` and `Misses` are zero. The value is a ratio from `0` to `1`, not a percentage.

## CacheManager Methods

### `GetStatistics`

**`public CacheStatistics GetStatistics()`**

Creates and returns a new `CacheStatistics` snapshot while holding the cache manager's lock. It populates `CurrentSize` from the dictionary count, `MaxSize` from the configured maximum, and `FillPercentage` from those two values. It copies `Hits`, `Misses`, `Evictions`, and `Expirations` from the manager's counters.

Calling `GetStatistics()` does not remove expired entries or change any counters. Because each call returns a new object, an earlier snapshot does not change when the cache or its counters change later.

### `ResetStatistics`

**`public void ResetStatistics()`**

Sets the manager's hit, miss, eviction, and expiration counters to zero while holding the cache manager's lock. It does not clear cached entries and does not change the configured maximum size. Consequently, the next `GetStatistics()` result reports zero for the four counters while still reporting the current cache size and fill percentage. Previously returned `CacheStatistics` snapshots are unaffected.

## Usage

```csharp
using System;
using NAudioVisualizer.Caching;

var cache = new CacheManager<string, string>(maxSize: 2);
cache.Set("track", "demo.wav");

cache.TryGetValue("track", out _);   // Hit
cache.TryGetValue("missing", out _); // Miss

CacheStatistics beforeReset = cache.GetStatistics();
Console.WriteLine($"{beforeReset.Hits} hit, {beforeReset.Misses} miss");
Console.WriteLine($"Hit rate: {beforeReset.HitRate:P0}");
Console.WriteLine($"Cache fill: {beforeReset.FillPercentage:F0}%");

cache.ResetStatistics();
CacheStatistics afterReset = cache.GetStatistics();

Console.WriteLine(afterReset.Hits);       // 0
Console.WriteLine(afterReset.Misses);     // 0
Console.WriteLine(afterReset.CurrentSize); // 1; reset does not clear entries
```

## Notes

*   `CacheStatistics` instances can also be created directly because the class is public and its stored properties use public init accessors.
*   `Contains` does not affect `Hits` or `Misses`, although it can increment `Expirations` when it removes an expired entry.
*   `Remove`, `Clear`, and replacing an existing value with `Set` do not increment the statistics counters.
