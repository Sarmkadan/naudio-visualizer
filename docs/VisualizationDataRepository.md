# VisualizationDataRepository

The `VisualizationDataRepository` class provides an in-memory store for `VisualizationData` objects, typically used to cache or manage audio visualization results such as waveforms, spectra, and spectrograms. It supports insertion, retrieval by identifier, session, or type, deletion, clearing, and statistical queries. The repository also exposes aggregate counts and timestamps for the oldest and newest entries, and includes a pruning operation to remove the oldest records.

## API

### `void Store(VisualizationData data)`

Inserts a new `VisualizationData` object into the repository.  
**Parameters:**  
- `data` – The visualization data to store.  

**Throws:**  
- `ArgumentNullException` if `data` is `null`.  
- `InvalidOperationException` if an entry with the same identifier already exists (if the implementation enforces unique IDs).

### `VisualizationData? GetById(Guid id)`

Retrieves a single visualization data entry by its unique identifier.  
**Parameters:**  
- `id` – The identifier of the entry.  

**Returns:**  
The matching `VisualizationData` object, or `null` if no entry with that identifier exists.

### `IReadOnlyList<VisualizationData> GetBySession(Guid sessionId)`

Returns all visualization data entries that belong to the specified session.  
**Parameters:**  
- `sessionId` – The session identifier to filter by.  

**Returns:**  
A read-only list of matching entries. If none exist, an empty list.

### `IReadOnlyList<VisualizationData> GetByType(VisualizationType type)`

Returns all visualization data entries of the specified type (e.g., waveform, spectrum, spectrogram).  
**Parameters:**  
- `type` – The `VisualizationType` to filter by.  

**Returns:**  
A read-only list of matching entries. If none exist, an empty list.

### `VisualizationData? GetMostRecent()`

Retrieves the most recently stored visualization data entry, based on insertion order or timestamp.  
**Returns:**  
The most recent entry, or `null` if the repository is empty.

### `IReadOnlyList<VisualizationData> GetAll()`

Returns all visualization data entries currently stored in the repository.  
**Returns:**  
A read-only list of all entries. If the repository is empty, an empty list.

### `bool Delete(Guid id)`

Removes the visualization data entry with the specified identifier.  
**Parameters:**  
- `id` – The identifier of the entry to delete.  

**Returns:**  
`true` if the entry was found and removed; `false` if no entry with that identifier existed.

### `int DeleteBySession(Guid sessionId)`

Removes all visualization data entries that belong to the specified session.  
**Parameters:**  
- `sessionId` – The session identifier whose entries should be deleted.  

**Returns:**  
The number of entries removed.

### `void Clear()`

Removes all visualization data entries from the repository. After this call, the repository is empty.

### `int Count`

Gets the total number of visualization data entries currently stored.  
**Value:** An integer representing the count.

### `RepositoryStats GetStats()`

Returns a `RepositoryStats` object containing aggregate statistics about the stored data (e.g., counts per type, oldest/newest timestamps, total entries).  
**Returns:**  
A `RepositoryStats` instance populated with current repository data.

### `int PruneOldest(int count)`

Removes the oldest entries from the repository, up to the specified count.  
**Parameters:**  
- `count` – The maximum number of entries to remove, starting from the oldest.  

**Returns:**  
The actual number of entries removed (may be less than `count` if the repository contains fewer entries).  

**Throws:**  
- `ArgumentOutOfRangeException` if `count` is negative.

### `int TotalCount`

Gets the total number of visualization data entries ever stored (including those that have been deleted, if tracked).  
**Value:** An integer representing the lifetime count.

### `int WaveformCount`

Gets the number of stored entries of type `Waveform`.  
**Value:** An integer.

### `int SpectrumCount`

Gets the number of stored entries of type `Spectrum`.  
**Value:** An integer.

### `int SpectrogramCount`

Gets the number of stored entries of type `Spectrogram`.  
**Value:** An integer.

### `int SessionCount`

Gets the number of distinct sessions represented in the repository.  
**Value:** An integer.

### `DateTime? OldestEntry`

Gets the timestamp of the oldest stored entry, or `null` if the repository is empty.  
**Value:** A `DateTime?` (UTC recommended).

### `DateTime? NewestEntry`

Gets the timestamp of the newest stored entry, or `null` if the repository is empty.  
**Value:** A `DateTime?` (UTC recommended).

## Usage

### Example 1: Storing and retrieving by session

```csharp
var repo = new VisualizationDataRepository();

// Create some visualization data for a session
var sessionId = Guid.NewGuid();
var waveform = new VisualizationData(
    id: Guid.NewGuid(),
    sessionId: sessionId,
    type: VisualizationType.Waveform,
    data: new float[] { 0.1f, 0.5f, -0.3f },
    timestamp: DateTime.UtcNow);

var spectrum = new VisualizationData(
    id: Guid.NewGuid(),
    sessionId: sessionId,
    type: VisualizationType.Spectrum,
    data: new float[] { 0.8f, 0.2f, 0.0f },
    timestamp: DateTime.UtcNow);

repo.Store(waveform);
repo.Store(spectrum);

// Retrieve all entries for the session
var sessionEntries = repo.GetBySession(sessionId);
Console.WriteLine($"Session has {sessionEntries.Count} entries.");

// Retrieve the most recent entry
var latest = repo.GetMostRecent();
if (latest != null)
    Console.WriteLine($"Most recent entry type: {latest.Type}");
```

### Example 2: Statistics and pruning

```csharp
var repo = new VisualizationDataRepository();

// Populate with sample data (omitted for brevity)
// ...

// Get repository statistics
var stats = repo.GetStats();
Console.WriteLine($"Total entries: {repo.TotalCount}");
Console.WriteLine($"Waveforms: {repo.WaveformCount}, Spectra: {repo.SpectrumCount}");
Console.WriteLine($"Oldest entry: {repo.OldestEntry}");
Console.WriteLine($"Newest entry: {repo.NewestEntry}");

// Prune the 10 oldest entries
int removed = repo.PruneOldest(10);
Console.WriteLine($"Pruned {removed} entries.");

// Delete all entries for a specific session
int deleted = repo.DeleteBySession(someSessionId);
Console.WriteLine($"Deleted {deleted} entries from session.");
```

## Notes

- **Thread safety:** This repository is not guaranteed to be thread-safe. Concurrent access from multiple threads must be synchronized externally (e.g., using locks or a concurrent collection wrapper).  
- **Identifier uniqueness:** The `Store` method may throw if an entry with the same `Id` already exists. Ensure that identifiers are unique before insertion.  
- **Empty repository:** All retrieval methods (`GetById`, `GetBySession`, `GetByType`, `GetMostRecent`, `GetAll`) return `null` or an empty list when no matching data exists. Properties `OldestEntry` and `NewestEntry` return `null` when the repository is empty.  
- **Pruning behavior:** `PruneOldest` removes entries based on insertion order or timestamp (oldest first). If the repository contains fewer entries than the requested count, all entries are removed and the actual number removed is returned.  
- **Lifetime tracking:** `TotalCount` may reflect entries that have been deleted, depending on the implementation. It is not necessarily equal to `Count` after deletions.  
- **Session and type counts:** `SessionCount`, `WaveformCount`, `SpectrumCount`, and `SpectrogramCount` are computed from the current state of the repository and do not include deleted entries.
