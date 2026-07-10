# AudioSessionRepository

Centralizes storage and retrieval of audio session data, including per-frame PCM samples and metadata. Designed for real-time audio visualization pipelines where sessions are recorded, queried, and optionally persisted.

## API

### `AudioSessionData CreateSession(AudioDevice device, int sampleRate, int channelCount)`
Creates and registers a new session with the specified device and format parameters. The session starts immediately and is returned for further use.
**Parameters**
- `device`: Audio output device associated with the session.
- `sampleRate`: Sample rate in Hz of the captured audio.
- `channelCount`: Number of audio channels.
**Returns**
The newly created `AudioSessionData` instance.
**Throws**
`ArgumentNullException` if `device` is null.
`ArgumentOutOfRangeException` if `sampleRate` or `channelCount` is non-positive.

---

### `AudioSessionData? GetSession(Guid id)`
Retrieves the session identified by `id`, or null if no such session exists.
**Parameters**
- `id`: Unique session identifier.
**Returns**
The matching `AudioSessionData` or null.

---

### `IReadOnlyList<AudioSessionData> GetAllSessions()`
Returns a snapshot of all currently tracked sessions.
**Returns**
An immutable list of active sessions, ordered by start time descending.

---

### `void AddFrameToSession(Guid sessionId, AudioFrame frame)`
Appends a new audio frame to the session identified by `sessionId`.
**Parameters**
- `sessionId`: Identifier of the target session.
- `frame`: Audio frame to store.
**Throws**
`KeyNotFoundException` if `sessionId` does not correspond to an active session.
`ArgumentNullException` if `frame` is null.

---
### `IReadOnlyList<AudioFrame> GetSessionFrames(Guid sessionId)`
Returns all frames recorded for the specified session.
**Parameters**
- `sessionId`: Identifier of the session.
**Returns**
An immutable list of frames in chronological order.
**Throws**
`KeyNotFoundException` if `sessionId` is unknown.

---
### `AudioFrame? GetFrame(Guid sessionId, int index)`
Retrieves the frame at the given zero-based index within the specified session.
**Parameters**
- `sessionId`: Identifier of the session.
- `index`: Frame index.
**Returns**
The frame if the index is valid; otherwise null.
**Throws**
`KeyNotFoundException` if `sessionId` is unknown.

---
### `IReadOnlyList<AudioFrame> GetFramesInTimeRange(Guid sessionId, TimeSpan start, TimeSpan duration)`
Returns all frames whose timestamps fall within the half-open interval `[start, start + duration)`.
**Parameters**
- `sessionId`: Identifier of the session.
- `start`: Start of the time range.
- `duration`: Length of the range.
**Returns**
An immutable list of matching frames, ordered by time.
**Throws**
`KeyNotFoundException` if `sessionId` is unknown.
`ArgumentOutOfRangeException` if `duration` is negative.

---
### `IReadOnlyList<AudioFrame> GetRecentFrames(Guid sessionId, int count)`
Returns the most recent `count` frames from the session, or fewer if the session contains less data.
**Parameters**
- `sessionId`: Identifier of the session.
- `count`: Maximum number of frames to return.
**Returns**
An immutable list of the newest frames, ordered newest-first.
**Throws**
`KeyNotFoundException` if `sessionId` is unknown.
`ArgumentOutOfRangeException` if `count` is negative.

---
### `void EndSession(Guid sessionId)`
Marks the session as ended and records the current timestamp as `EndTime`.
**Parameters**
- `sessionId`: Identifier of the session to finalize.
**Throws**
`KeyNotFoundException` if `sessionId` is unknown.
`InvalidOperationException` if the session has already been ended.

---
### `bool DeleteSession(Guid sessionId)`
Removes the session and all its frames from the repository.
**Parameters**
- `sessionId`: Identifier of the session to delete.
**Returns**
`true` if the session existed and was removed; `false` otherwise.
**Throws**
Nothing; all failure modes return `false`.

---
### `int GetFrameCount(Guid sessionId)`
Returns the total number of frames stored for the session.
**Parameters**
- `sessionId`: Identifier of the session.
**Returns**
Frame count, or -1 if the session is unknown.

---
### `void SetMaxFramesPerSession(int value)`
Sets the upper bound on the number of frames retained per session. Older frames are discarded when the limit is exceeded.
**Parameters**
- `value`: New maximum frame count; must be non-negative.
**Throws**
`ArgumentOutOfRangeException` if `value` is negative.

---
### `SessionRepositoryStats GetStats()`
Returns aggregate statistics about the repository and all sessions.
**Returns**
A snapshot of current statistics.

---
### `void Clear()`
Removes all sessions and their frames from the repository.

---
### Properties

#### `Guid SessionId`
Unique identifier for the session. Immutable after creation.

#### `DateTime StartTime`
Timestamp when the session began. Immutable after creation.

#### `DateTime? EndTime`
Timestamp when the session was ended, or null if still active. Immutable after `EndSession` is called.

#### `AudioDevice? Device`
Audio output device associated with the session, or null if unknown.

#### `int SampleRate`
Sample rate in Hz of the captured audio.

#### `int ChannelCount`
Number of audio channels in the session.

## Usage
