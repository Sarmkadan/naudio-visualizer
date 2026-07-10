# ValidationAndStringUtilityTests

Unit tests for the `ValidationAndStringUtility` class, verifying validation logic and string formatting utilities used in audio processing scenarios. Tests cover sample rate, FFT size, channel count, frequency, amplitude validation, null checks, range validation, string truncation, byte formatting, millisecond formatting, case conversion, and substring counting.

## API

### `ValidateSampleRate_VariousRates_ReturnsExpectedResult`
Validates that the `ValidateSampleRate` method returns the expected boolean result for various sample rates. Tests standard audio sample rates (e.g., 44100, 48000) as well as invalid values.

### `ValidateFftSize_VariousSizes_ReturnsExpectedResult`
Validates that the `ValidateFftSize` method returns the expected boolean result for various FFT sizes. Tests powers of two and invalid sizes.

### `ValidateChannelCount_VariousCounts_ReturnsExpectedResult`
Validates that the `ValidateChannelCount` method returns the expected boolean result for various channel counts. Tests typical mono/stereo configurations and invalid counts.

### `ValidateFrequency_BoundaryValues_ReturnsExpectedResult`
Validates that the `ValidateFrequency` method returns the expected boolean result for boundary frequency values. Tests minimum, maximum, and out-of-range frequencies.

### `ValidateAudioData_NullArray_ReturnsFalse`
Validates that the `ValidateAudioData` method returns `false` when given a null audio data array.

### `ValidateAudioData_NonEmptyArray_ReturnsTrue`
Validates that the `ValidateAudioData` method returns `true` when given a non-empty audio data array.

### `ValidateAmplitude_ValueWithinRange_ReturnsTrue`
Validates that the `ValidateAmplitude` method returns `true` when the amplitude value is within the valid range (e.g., [-1.0, 1.0]).

### `ValidateAmplitude_ValueOutsideRange_ReturnsFalse`
Validates that the `ValidateAmplitude` method returns `false` when the amplitude value is outside the valid range.

### `ThrowIfNull_NullValue_ThrowsArgumentNullException`
Validates that the `ThrowIfNull` method throws an `ArgumentNullException` when given a null value.

### `ThrowIfNull_NonNullValue_DoesNotThrow`
Validates that the `ThrowIfNull` method does not throw when given a non-null value.

### `ThrowIfOutOfRange_ValueBelowMinimum_ThrowsArgumentOutOfRangeException`
Validates that the `ThrowIfOutOfRange` method throws an `ArgumentOutOfRangeException` when the value is below the minimum allowed value.

### `ThrowIfNullOrWhitespace_WhitespaceString_ThrowsArgumentException`
Validates that the `ThrowIfNullOrWhitespace` method throws an `ArgumentException` when given a whitespace string.

### `Truncate_LongerThanMaxLength_TruncatesWithEllipsis`
Validates that the `Truncate` method truncates a string longer than the specified maximum length and appends an ellipsis.

### `Truncate_ShorterThanMaxLength_ReturnsOriginalString`
Validates that the `Truncate` method returns the original string when its length is less than or equal to the specified maximum length.

### `Truncate_WithEllipsisDisabled_TruncatesWithoutEllipsis`
Validates that the `Truncate` method truncates a string without appending an ellipsis when ellipsis is disabled.

### `FormatBytes_VariousSizes_OutputContainsCorrectUnit`
Validates that the `FormatBytes` method returns a string containing the correct unit (e.g., "KB", "MB") for various byte sizes.

### `FormatMilliseconds_NegativeValue_ReturnsInvalidString`
Validates that the `FormatMilliseconds` method returns a string indicating an invalid value when given a negative millisecond value.

### `FormatMilliseconds_LessThanOneSecond_ReturnsMsFormat`
Validates that the `FormatMilliseconds` method returns a string in "ms" format when the value is less than one second.

### `ToSnakeCase_PascalCaseInput_ReturnsSnakeCase`
Validates that the `ToSnakeCase` method converts a PascalCase string to snake_case (e.g., "PascalCase" → "pascal_case").

### `CountOccurrences_MultipleNonOverlappingMatches_ReturnsCorrectCount`
Validates that the `CountOccurrences` method returns the correct count of non-overlapping substring occurrences in a string.

## Usage
