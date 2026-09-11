# FrequencyBandsExtensions

Provides extension methods for <see cref="FrequencyBands"/> instances.

## API

### `public static float[] ToArray(this FrequencyBands bands)`
Returns the frequency band energy values in their declared order.

- **Parameters**
  - `bands`: The frequency band values to convert. Must not be `null`.
- **Return value**: An array containing bass, mid, and treble energy values, in that order.
- **Exceptions**
  - `ArgumentNullException` if <paramref name="bands"/> is `null`.

## Usage

```csharp
using NAudioVisualizer.Services; // namespace containing the extensions

var bands = new FrequencyBands { BassEnergy = 0.5f, MidEnergy = 0.3f, TrebleEnergy = 0.2f };

// Convert to array in the order: bass, mid, treble
float[] array = bands.ToArray(); // [0.5f, 0.3f, 0.2f]
```

## Notes

- The extension method operates on an immutable snapshot; it does not alter the source <see cref="FrequencyBands"/> instance.
- The static extension method itself is thread‑safe provided it does not rely on mutable shared state; it only reads the supplied arguments and returns a new array.
- The method returns the energy values in the order: <see cref="FrequencyBands.BassEnergy"/>, <see cref="FrequencyBands.MidEnergy"/>, <see cref="FrequencyBands.TrebleEnergy"/>.