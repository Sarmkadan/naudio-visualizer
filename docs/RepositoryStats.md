# RepositoryStats

The repository statistics types provide snapshots of the in-memory visualization and audio-session repositories. `VisualizationDataRepository.GetStats()` returns a `RepositoryStats` instance, while `AudioSessionRepository.GetStats()` returns a `SessionRepositoryStats` instance. `AudioSessionData` represents the metadata and current state stored for an individual audio session.

All three types are in the `NAudioVisualizer.Data.Repositories` namespace.

## RepositoryStats

`VisualizationDataRepository.GetStats()` creates a new `RepositoryStats` object while holding the repository lock. The snapshot describes the visualization entries and session index at the time of the call.

### Properties

*   **`public int TotalCount { get; set; }`**
    Gets or sets the number of entries in the visualization repository.

*   **`public int WaveformCount { get; set; }`**
    Gets or sets the number of stored entries whose `VisualizationType` is `VisualizationType.Waveform`.

*   **`public int SpectrumCount { get; set; }`**
    Gets or sets the number of stored entries whose `VisualizationType` is `VisualizationType.Spectrum`.

*   **`public int SpectrogramCount { get; set; }`**
    Gets or sets the number of stored entries whose `VisualizationType` is `VisualizationType.Spectrogram`.

*   **`public int SessionCount { get; set; }`**
    Gets or sets the number of keys in the repository's session index. Only visualization data with a non-null `SourceFrame` is added to this index; the index key used by the repository is `SourceFrame.Id`.

*   **`public DateTime? OldestEntry { get; set; }`**
    Gets or sets the earliest `GeneratedAt` value among stored visualization entries. The value is `null` when the repository contains no entries.

*   **`public DateTime? NewestEntry { get; set; }`**
    Gets or sets the latest `GeneratedAt` value among stored visualization entries. The value is `null` when the repository contains no entries.

### Method

*   **`public RepositoryStats GetStats()`**
    Returns a newly created statistics snapshot. Counts for waveform, spectrum, and spectrogram entries are calculated independently from `TotalCount`; entries of any other visualization type still contribute to `TotalCount` but not to those three type-specific counts.

## SessionRepositoryStats

`AudioSessionRepository.GetStats()` creates a new `SessionRepositoryStats` object while holding the repository lock. Session totals come from the session store, while the frame total is the sum of the counts of all lists in the separate frame store.

### Properties

*   **`public int TotalSessionCount { get; set; }`**
    Gets or sets the number of entries in the session store.

*   **`public int TotalFrameCount { get; set; }`**
    Gets or sets the total number of frames across every list in the frame store. A frame list can exist even when there is no corresponding `AudioSessionData`, because `AddFrameToSession` creates a list for an unknown session ID.

*   **`public int ActiveSessionCount { get; set; }`**
    Gets or sets the number of stored sessions whose `EndTime` is `null`.

*   **`public int CompletedSessionCount { get; set; }`**
    Gets or sets the number of stored sessions whose `EndTime` is not `null`.

*   **`public int MaxFramesPerSession { get; set; }`**
    Gets or sets the repository's current maximum number of retained frames per session. The repository initializes this limit to `5000`. `SetMaxFramesPerSession` changes it and trims older frames from existing lists.

### Method

*   **`public SessionRepositoryStats GetStats()`**
    Returns a newly created statistics snapshot containing the session counts, aggregate retained frame count, and current per-session frame limit.

## AudioSessionData

`AudioSessionData` represents one stored audio recording session. `AudioSessionRepository.CreateSession(AudioMetadata metadata)` initializes it from the metadata's session ID, start time, audio device, sample rate, and channel count. Its `EndTime` remains `null` until `EndSession` is called for the stored session.

### Properties

*   **`public Guid SessionId { get; set; }`**
    Gets or sets the session identifier.

*   **`public DateTime StartTime { get; set; }`**
    Gets or sets the session start time.

*   **`public DateTime? EndTime { get; set; }`**
    Gets or sets the session end time. A `null` value identifies an active session in repository statistics.

*   **`public AudioDevice? Device { get; set; }`**
    Gets or sets the audio device associated with the session.

*   **`public int SampleRate { get; set; }`**
    Gets or sets the session sample rate.

*   **`public int ChannelCount { get; set; }`**
    Gets or sets the session channel count.

*   **`public int FrameCount { get; set; }`**
    Gets or sets the number of frames currently retained for the session. `AddFrameToSession` updates this property when a matching session exists, after enforcing the maximum frame count. Trimming caused by `SetMaxFramesPerSession` does not update this property.

*   **`public DateTime LastFrameTime { get; set; }`**
    Gets or sets the time at which a frame was most recently added to the stored session. It defaults to `DateTime.UtcNow` when the object is created, and `AddFrameToSession` assigns `DateTime.UtcNow` when a matching session exists.

### Methods

*   **`public TimeSpan GetDuration()`**
    Returns `EndTime - StartTime` for a completed session. If `EndTime` is `null`, it returns `DateTime.UtcNow - StartTime`, so the duration of an active session continues to increase.

## Usage

The following example creates a session, adds a frame, and reads snapshots from both repositories.

```csharp
using NAudioVisualizer.Data.Repositories;
using NAudioVisualizer.Domain.Models;

var sessionRepository = new AudioSessionRepository();
AudioSessionData session = sessionRepository.CreateSession(metadata);

sessionRepository.AddFrameToSession(session.SessionId, frame);

SessionRepositoryStats sessionStats = sessionRepository.GetStats();
Console.WriteLine($"Sessions: {sessionStats.TotalSessionCount}");
Console.WriteLine($"Retained frames: {sessionStats.TotalFrameCount}");
Console.WriteLine($"Duration: {session.GetDuration()}");

var visualizationRepository = new VisualizationDataRepository();
visualizationRepository.Store(visualizationData);

RepositoryStats visualizationStats = visualizationRepository.GetStats();
Console.WriteLine($"Visualizations: {visualizationStats.TotalCount}");
Console.WriteLine($"Newest entry: {visualizationStats.NewestEntry}");
```

In this example, `metadata`, `frame`, and `visualizationData` are existing `AudioMetadata`, `AudioFrame`, and `VisualizationData` instances. Each call to `GetStats()` returns a separate mutable snapshot; later repository operations do not update a previously returned statistics object.

## Notes

*   Both repositories calculate their snapshots inside their internal locks.
*   The returned statistics objects and `AudioSessionData` expose public setters and are mutable.
*   `EndSession` sets `EndTime` to `DateTime.UtcNow` only when the session ID exists in the session store.
*   Adding a frame removes only the oldest frame when the new count exceeds the configured maximum. Changing the maximum trims each existing frame list until it satisfies the new limit.
