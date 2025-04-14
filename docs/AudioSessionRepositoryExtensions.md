# AudioSessionRepositoryExtensions

Provides extension methods for retrieving audio session metrics from an `IAudioSessionRepository`, including duration and amplitude statistics.

## API

### `GetSessionDurationSeconds(IAudioSessionRepository session)`

Calculates the total duration of the audio session in seconds.

- **Parameters**
  - `session` – The `IAudioSessionRepository` instance containing the audio session data.
- **Return value**
  - The total duration of the session in seconds, as a `double`.
- **Exceptions**
  - Throws `ArgumentNullException` if `session` is `null`.

---

### `GetSessionDurationMilliseconds(IAudioSessionRepository session)`

Calculates the total duration of the audio session in milliseconds.

- **Parameters**
  - `session` – The `IAudioSessionRepository` instance containing the audio session data.
- **Return value**
  - The total duration of the session in milliseconds, as a `long`.
- **Exceptions**
  - Throws `ArgumentNullException` if `session` is `null`.

---
### `GetAverageRmsEnergy(IAudioSessionRepository session)`

Computes the average RMS energy of the audio session.

- **Parameters**
  - `session` – The `IAudioSessionRepository` instance containing the audio session data.
- **Return value**
  - The average RMS energy as a `double`.
- **Exceptions**
  - Throws `ArgumentNullException` if `session` is `null`.
  - Throws `InvalidOperationException` if the session contains no valid RMS data.

---
### `GetPeakAmplitude(IAudioSessionRepository session)`

Retrieves the peak amplitude observed in the audio session.

- **Parameters**
  - `session` – The `IAudioSessionRepository` instance containing the audio session data.
- **Return value**
  - The peak amplitude as a `double`.
- **Exceptions**
  - Throws `ArgumentNullException` if `session` is `null`.
  - Throws `InvalidOperationException` if no amplitude data is available.

## Usage
