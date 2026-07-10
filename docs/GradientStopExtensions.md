# GradientStopExtensions

Extension methods for `GradientStop` that provide color manipulation, positional adjustment, and sequence utilities for gradient definitions.

## API

### `GradientStop WithColor(GradientStop stop, Color color)`
Creates a new `GradientStop` with the same position as `stop` and the specified `color`.

- **Parameters**
  - `stop` – The source gradient stop.
  - `color` – The new color to assign.
- **Returns**
  - A new `GradientStop` instance with the updated color.
- **Throws**
  - `ArgumentNullException` if `stop` is `null`.

---

### `GradientStop WithPosition(GradientStop stop, double position)`
Creates a new `GradientStop` with the same color as `stop` and the specified `position`.

- **Parameters**
  - `stop` – The source gradient stop.
  - `position` – The new position (must be between 0.0 and 1.0).
- **Returns**
  - A new `GradientStop` instance with the updated position.
- **Throws**
  - `ArgumentNullException` if `stop` is `null`.
  - `ArgumentOutOfRangeException` if `position` is outside the valid range.

---

### `void GetArgbComponents(GradientStop stop, out byte a, out byte r, out byte g, out byte b)`
Extracts the ARGB components of the color from the specified `GradientStop`.

- **Parameters**
  - `stop` – The gradient stop whose color components are extracted.
  - `a` – Output parameter for the alpha component.
  - `r` – Output parameter for the red component.
  - `g` – Output parameter for the green component.
  - `b` – Output parameter for the blue component.
- **Throws**
  - `ArgumentNullException` if `stop` is `null`.

---

### `GradientStop WithAlpha(GradientStop stop, byte alpha)`
Creates a new `GradientStop` with the same position and RGB values as `stop`, but with the specified alpha.

- **Parameters**
  - `stop` – The source gradient stop.
  - `alpha` – The new alpha value (0–255).
- **Returns**
  - A new `GradientStop` instance with the updated alpha.
- **Throws**
  - `ArgumentNullException` if `stop` is `null`.

---
### `GradientStop AdjustBrightness(GradientStop stop, double factor)`
Adjusts the brightness of the color in `stop` by multiplying each RGB component by `factor`.

- **Parameters**
  - `stop` – The source gradient stop.
  - `factor` – A multiplier for brightness (e.g., 1.5 brightens, 0.5 darkens).
- **Returns**
  - A new `GradientStop` instance with the adjusted brightness.
- **Throws**
  - `ArgumentNullException` if `stop` is `null`.
  - `ArgumentOutOfRangeException` if `factor` is negative.

---
### `GradientStop AdjustContrast(GradientStop stop, double factor)`
Adjusts the contrast of the color in `stop` by scaling the difference from 128 (mid-gray).

- **Parameters**
  - `stop` – The source gradient stop.
  - `factor` – A contrast multiplier (e.g., 1.2 increases contrast).
- **Returns**
  - A new `GradientStop` instance with the adjusted contrast.
- **Throws**
  - `ArgumentNullException` if `stop` is `null`.
  - `ArgumentOutOfRangeException` if `factor` is negative.

---
### `int IndexIn(IReadOnlyList<GradientStop> stops)`
Returns the zero-based index of `this` gradient stop within the provided list.

- **Parameters**
  - `stops` – The list of gradient stops to search.
- **Returns**
  - The index of the stop, or `-1` if not found.
- **Throws**
  - `ArgumentNullException` if `stops` is `null`.

---
### `bool IsFirst(IReadOnlyList<GradientStop> stops)`
Determines whether `this` gradient stop is the first element in the list.

- **Parameters**
  - `stops` – The list of gradient stops to check.
- **Returns**
  - `true` if the stop is the first element; otherwise, `false`.
- **Throws**
  - `ArgumentNullException` if `stops` is `null` or empty.

---
### `bool IsLast(IReadOnlyList<GradientStop> stops)`
Determines whether `this` gradient stop is the last element in the list.

- **Parameters**
  - `stops` – The list of gradient stops to check.
- **Returns**
  - `true` if the stop is the last element; otherwise, `false`.
- **Throws**
  - `ArgumentNullException` if `stops` is `null` or empty.

---
### `GradientStop? Next(IReadOnlyList<GradientStop> stops)`
Returns the next gradient stop in the list, or `null` if this is the last element.

- **Parameters**
  - `stops` – The list of gradient stops.
- **Returns**
  - The next stop, or `null` if none exists.
- **Throws**
  - `ArgumentNullException` if `stops` is `null`.

---
### `GradientStop? Previous(IReadOnlyList<GradientStop> stops)`
Returns the previous gradient stop in the list, or `null` if this is the first element.

- **Parameters**
  - `stops` – The list of gradient stops.
- **Returns**
  - The previous stop, or `null` if none exists.
- **Throws**
  - `ArgumentNullException` if `stops` is `null`.

---
### `GradientStop Interpolate(GradientStop a, GradientStop b, double t)`
Linearly interpolates between two gradient stops based on parameter `t`.

- **Parameters**
  - `a` – The first gradient stop.
  - `b` – The second gradient stop.
  - `t` – Interpolation factor (0.0 returns `a`, 1.0 returns `b`).
- **Returns**
  - A new `GradientStop` with color and position interpolated between `a` and `b`.
- **Throws**
  - `ArgumentNullException` if `a` or `b` is `null`.
  - `ArgumentOutOfRangeException` if `t` is outside the range [0.0, 1.0].

---
### `bool HasSameColor(GradientStop other)`
Determines whether this gradient stop has the same color as `other`.

- **Parameters**
  - `other` – The gradient stop to compare with.
- **Returns**
  - `true` if the colors are identical; otherwise, `false`.
- **Throws**
  - `ArgumentNullException` if `other` is `null`.

---
### `bool HasSamePosition(GradientStop other)`
Determines whether this gradient stop has the same position as `other`.

- **Parameters**
  - `other` – The gradient stop to compare with.
- **Returns**
  - `true` if the positions are equal; otherwise, `false`.
- **Throws**
  - `ArgumentNullException` if `other` is `null`.

---
### `byte GetBrightness()`
Calculates the perceived brightness of the gradient stop’s color (0–255).

- **Returns**
  - The brightness value, where 0 is black and 255 is white.

---
### `bool IsDark()`
Determines whether the gradient stop’s color is considered dark.

- **Returns**
  - `true` if the brightness is below a threshold (typically 128); otherwise, `false`.

---
### `bool IsLight()`
Determines whether the gradient stop’s color is considered light.

- **Returns**
  - `true` if the brightness is above a threshold (typically 128); otherwise, `false`.

## Usage

### Example 1: Adjusting a Gradient Stop
