# MathUtilityTests

The `MathUtilityTests` class is a unit test suite that validates the behavior of the `MathUtility` static helper class in the `naudio-visualizer` project. Each test method exercises a specific mathematical transformation or utility function, covering normal operation, boundary conditions, and edge cases. The tests are designed to be run with a standard .NET test framework (e.g., NUnit or xUnit) and assert expected outcomes using assertions.

## API

The class exposes the following public test methods. Each method takes no parameters and returns `void`. A test passes if all internal assertions succeed; it fails (typically by throwing an assertion exception) if any assertion is violated.

- **`FrequencyToMidiNote_A4Frequency_Returns69`**  
  Verifies that the frequency 440 Hz (A4) is correctly mapped to MIDI note number 69.

- **`FrequencyToMidiNote_NonPositiveFrequency_ReturnsZero`**  
  Ensures that frequencies ≤ 0 produce a MIDI note number of 0 (the lowest valid MIDI note).

- **`AmplitudeToDb_ZeroAmplitude_ReturnsNegativeInfinity`**  
  Confirms that an amplitude of 0 results in `double.NegativeInfinity` (logarithm of zero).

- **`AmplitudeToDb_UnitAmplitude_ReturnsZeroDb`**  
  Checks that an amplitude of 1.0 yields 0 dB.

- **`CalculateRms_UniformSignal_ReturnsUnitValue`**  
  Tests that a constant signal (e.g., all samples equal to 1.0) produces an RMS value of 1.0.

- **`CalculateRms_EmptyArray_ReturnsZero`**  
  Validates that an empty sample array returns an RMS of 0.

- **`CalculatePeak_SignalWithNegativeValues_ReturnsAbsoluteMaximum`**  
  Ensures the peak value is the maximum absolute value in the signal, correctly handling negative samples.

- **`NextPowerOf2_VariousInputs_ReturnsNextPower`**  
  Verifies that for a set of integer inputs, the method returns the smallest power of two that is greater than or equal to the input.

- **`IsPowerOf2_VariousInputs_ReturnsExpectedResult`**  
  Confirms that the method correctly identifies powers of two and non-powers of two for a range of inputs.

- **`Lerp_TBeyondUpperBound_ClampsToEndValue`**  
  Tests that linear interpolation clamps the parameter `t` to the range [0,1], returning the end value when `t` exceeds 1.

- **`MapRange_MidpointValue_ReturnsMidpointOfTargetRange`**  
  Checks that mapping the midpoint of the source range produces the midpoint of the target range.

- **`MapRange_EqualSourceBounds_ReturnsTargetMinimum`**  
  Ensures that when the source range has zero width (min == max), the result is the target range’s minimum value.

## Usage

The following examples demonstrate how to instantiate `MathUtilityTests` and invoke its test methods directly, either in a custom test harness or as part of a debugging session.

**Example 1: Running a frequency-to-MIDI test**

```csharp
var tests = new MathUtilityTests();
try
{
    tests.FrequencyToMidiNote_A4Frequency_Returns69();
    Console.WriteLine("Test passed: A4 frequency maps to MIDI note 69.");
}
catch (AssertionException ex)
{
    Console.WriteLine($"Test failed: {ex.Message}");
}
```

**Example 2: Verifying RMS calculation for an empty array**

```csharp
var tests = new MathUtilityTests();
tests.CalculateRms_EmptyArray_ReturnsZero(); // Throws if assertion fails
Console.WriteLine("Empty array RMS test passed.");
```

In a typical testing framework, these methods are discovered and executed automatically by the test runner. The direct invocation shown above is useful for ad‑hoc verification or when embedding tests in a console application.

## Notes

- **Edge cases** – The test suite explicitly covers boundary conditions: non‑positive frequencies, zero amplitude, empty sample arrays, signals containing only negative values, source ranges with equal bounds, and interpolation parameters outside the [0,1] interval. These tests ensure the underlying `MathUtility` methods handle degenerate inputs gracefully.
- **Thread safety** – `MathUtilityTests` contains no instance fields or shared mutable state. All test methods are stateless and can be executed concurrently without synchronization. The underlying `MathUtility` methods are also stateless and thread‑safe.
- **Test framework independence** – The class does not inherit from any specific test base class or use attributes. It can be used with any assertion library that throws on failure (e.g., NUnit’s `Assert`, xUnit’s `Assert`, or a custom `AssertionException`). The tests are designed to be self‑contained and portable.
