# VisualizationSettingsExtensions

Provides extension methods for `VisualizationSettings` that simplify common configuration tasks such as creating default settings, cloning existing settings, and adjusting performance or quality presets. These methods also expose color configuration utilities for rendering visualizations.

## API

### `public static VisualizationSettings CreateDefault()`

Creates a new `VisualizationSettings` instance with default values for all properties. The default configuration balances reasonable performance and visual quality for common use cases.

- **Parameters**: None
- **Return value**: A new `VisualizationSettings` instance with default values.
- **Exceptions**: None

---

### `public static VisualizationSettings Clone(VisualizationSettings settings)`

Creates a deep copy of the provided `VisualizationSettings` instance. Modifications to the returned instance will not affect the original.

- **Parameters**:
  - `settings` – The `VisualizationSettings` instance to clone.
- **Return value**: A new `VisualizationSettings` instance with the same property values as `settings`.
- **Exceptions**:
  - `ArgumentNullException` – Thrown if `settings` is `null`.

---

### `public static VisualizationSettings WithHighPerformance(VisualizationSettings settings)`

Returns a new `VisualizationSettings` instance optimized for high rendering performance. This typically reduces visual fidelity or complexity to minimize CPU/GPU usage.

- **Parameters**:
  - `settings` – The base `VisualizationSettings` to modify.
- **Return value**: A new `VisualizationSettings` instance with performance-focused settings applied.
- **Exceptions**:
  - `ArgumentNullException` – Thrown if `settings` is `null`.

---

### `public static VisualizationSettings WithHighQuality(VisualizationSettings settings)`

Returns a new `VisualizationSettings` instance optimized for high visual quality. This may increase rendering complexity or resource usage.

- **Parameters**:
  - `settings` – The base `VisualizationSettings` to modify.
- **Return value**: A new `VisualizationSettings` instance with quality-focused settings applied.
- **Exceptions**:
  - `ArgumentNullException` – Thrown if `settings` is `null`.

---
### `public static void Validate(VisualizationSettings settings)`

Validates the provided `VisualizationSettings` instance and throws an exception if any property is in an invalid state.

- **Parameters**:
  - `settings` – The `VisualizationSettings` instance to validate.
- **Return value**: None
- **Exceptions**:
  - `ArgumentNullException` – Thrown if `settings` is `null`.
  - `InvalidOperationException` – Thrown if any property value is invalid (e.g., negative values where disallowed).

---
### `public static Color GetBackgroundColor(VisualizationSettings settings)`

Retrieves the effective background color from the `VisualizationSettings` instance, applying any overrides or fallbacks defined in the settings.

- **Parameters**:
  - `settings` – The `VisualizationSettings` instance to query.
- **Return value**: A `Color` representing the background color to use during rendering.
- **Exceptions**:
  - `ArgumentNullException` – Thrown if `settings` is `null`.

---
### `public static Color GetWaveformLineColor(VisualizationSettings settings)`

Retrieves the effective waveform line color from the `VisualizationSettings` instance, applying any overrides or fallbacks defined in the settings.

- **Parameters**:
  - `settings` – The `VisualizationSettings` instance to query.
- **Return value**: A `Color` representing the waveform line color to use during rendering.
- **Exceptions**:
  - `ArgumentNullException` – Thrown if `settings` is `null`.

---
### `public static Color GetSpectrumBarColor(VisualizationSettings settings)`

Retrieves the effective spectrum bar color from the `VisualizationSettings` instance, applying any overrides or fallbacks defined in the settings.

- **Parameters**:
  - `settings` – The `VisualizationSettings` instance to query.
- **Return value**: A `Color` representing the spectrum bar color to use during rendering.
- **Exceptions**:
  - `ArgumentNullException` – Thrown if `settings` is `null`.

## Usage
