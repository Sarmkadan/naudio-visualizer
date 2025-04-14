# PathUtilityValidation

`PathUtilityValidation` is a static validation harness for the `PathUtility` class. It exposes a collection of pre‑defined test‑case lists—one per public method of `PathUtility`—and a pair of lifecycle members (`IsValid` and `EnsureValid`) that allow consumers to programmatically verify that the underlying path utilities behave correctly in the current execution environment. The type is intended for diagnostic and self‑test scenarios, not for production path manipulation.

## API

### `public static IReadOnlyList<string> ValidateNormalizePath`
Returns a read‑only list of test‑case descriptions that exercise `PathUtility.NormalizePath`. Each string represents an input/expected‑output pair or a scenario that should be checked. The list is deterministic and immutable.

### `public static IReadOnlyList<string> ValidateCombine`
Returns a read‑only list of test‑case descriptions for `PathUtility.Combine`. Covers edge cases such as null segments, rooted segments, and directory separators.

### `public static IReadOnlyList<string> ValidateGetAbsolutePath`
Returns a read‑only list of test‑case descriptions for `PathUtility.GetAbsolutePath`. Includes relative‑to‑absolute conversion, rooted paths, and paths relative to different base directories.

### `public static IReadOnlyList<string> ValidateGetRelativePath`
Returns a read‑only list of test‑case descriptions for `PathUtility.GetRelativePath`. Covers same‑root, different‑root, and cases where relative path cannot be computed.

### `public static IReadOnlyList<string> ValidateEnsureTrailingSeparator`
Returns a read‑only list of test‑case descriptions for `PathUtility.EnsureTrailingSeparator`. Verifies that a trailing directory separator is appended only when absent.

### `public static IReadOnlyList<string> ValidateRemoveTrailingSeparator`
Returns a read‑only list of test‑case descriptions for `PathUtility.RemoveTrailingSeparator`. Verifies removal of a single trailing separator while preserving root paths.

### `public static IReadOnlyList<string> ValidateIsAbsolute`
Returns a read‑only list of test‑case descriptions for `PathUtility.IsAbsolute`. Includes platform‑specific absolute‑path patterns and relative‑path negatives.

### `public static IReadOnlyList<string> ValidateIsRelative`
Returns a read‑only list of test‑case descriptions for `PathUtility.IsRelative`. Complements `ValidateIsAbsolute` with the inverse expectations.

### `public static IReadOnlyList<string> ValidateGetFilesRecursive`
Returns a read‑only list of test‑case descriptions for `PathUtility.GetFilesRecursive`. Covers existing directories, empty directories, search patterns, and inaccessible paths.

### `public static IReadOnlyList<string> ValidateGetApplicationDirectory`
Returns a read‑only list of test‑case descriptions for `PathUtility.GetApplicationDirectory`. Validates that the returned path points to the executing assembly’s location.

### `public static IReadOnlyList<string> ValidateGetApplicationDataDirectory`
Returns a read‑only list of test‑case descriptions for `PathUtility.GetApplicationDataDirectory`. Checks the platform‑conformant application data folder resolution.

### `public static IReadOnlyList<string> ValidateGetLogsDirectory`
Returns a read‑only list of test‑case descriptions for `PathUtility.GetLogsDirectory`. Ensures the logs directory is derived correctly from the application data path.

### `public static IReadOnlyList<string> ValidateGetTempDirectory`
Returns a read‑only list of test‑case descriptions for `PathUtility.GetTempDirectory`. Verifies that the returned path is a valid, writable temporary directory.

### `public static IReadOnlyList<string> ValidateIsValidPath`
Returns a read‑only list of test‑case descriptions for `PathUtility.IsValidPath`. Covers invalid characters, empty strings, null, and valid paths of various lengths.

### `public static IReadOnlyList<string> ValidateGetDirectorySize`
Returns a read‑only list of test‑case descriptions for `PathUtility.GetDirectorySize`. Includes empty directories, non‑existent paths, and directories with nested files.

### `public static IReadOnlyList<string> ValidateGenerateUniqueFileName`
Returns a read‑only list of test‑case descriptions for `PathUtility.GenerateUniqueFileName`. Verifies uniqueness, format, and collision avoidance.

### `public static bool IsValid`
Indicates whether all validation test cases have passed on the current platform. Returns `true` only if every test list has been executed without failure; otherwise `false`. This property does not run the tests itself—it reflects the outcome of a prior validation run.

### `public static void EnsureValid`
Throws an `InvalidOperationException` if `IsValid` is `false`. Intended as a guard called during application startup or diagnostic routines to halt execution when the path utilities are not functioning as expected.

## Usage

```csharp
// Startup self-check: abort if path utilities are broken in this environment
PathUtilityValidation.EnsureValid();

// Proceed with application logic that relies on PathUtility
string appDir = PathUtility.GetApplicationDirectory();
```

```csharp
// Diagnostic report: iterate all test cases and log failures
var allTests = new Dictionary<string, IReadOnlyList<string>>
{
    ["NormalizePath"] = PathUtilityValidation.ValidateNormalizePath,
    ["Combine"] = PathUtilityValidation.ValidateCombine,
    ["GetAbsolutePath"] = PathUtilityValidation.ValidateGetAbsolutePath,
    ["GetRelativePath"] = PathUtilityValidation.ValidateGetRelativePath,
    ["EnsureTrailingSeparator"] = PathUtilityValidation.ValidateEnsureTrailingSeparator,
    ["RemoveTrailingSeparator"] = PathUtilityValidation.ValidateRemoveTrailingSeparator,
    ["IsAbsolute"] = PathUtilityValidation.ValidateIsAbsolute,
    ["IsRelative"] = PathUtilityValidation.ValidateIsRelative,
    ["GetFilesRecursive"] = PathUtilityValidation.ValidateGetFilesRecursive,
    ["GetApplicationDirectory"] = PathUtilityValidation.ValidateGetApplicationDirectory,
    ["GetApplicationDataDirectory"] = PathUtilityValidation.ValidateGetApplicationDataDirectory,
    ["GetLogsDirectory"] = PathUtilityValidation.ValidateGetLogsDirectory,
    ["GetTempDirectory"] = PathUtilityValidation.ValidateGetTempDirectory,
    ["IsValidPath"] = PathUtilityValidation.ValidateIsValidPath,
    ["GetDirectorySize"] = PathUtilityValidation.ValidateGetDirectorySize,
    ["GenerateUniqueFileName"] = PathUtilityValidation.ValidateGenerateUniqueFileName,
};

foreach (var kvp in allTests)
{
    Console.WriteLine($"--- {kvp.Key} ---");
    foreach (string testCase in kvp.Value)
    {
        Console.WriteLine(testCase);
    }
}

if (!PathUtilityValidation.IsValid)
{
    Console.WriteLine("WARNING: PathUtility validation failed.");
}
```

## Notes

- All `Validate*` members return the same list instance on every access; the collections are statically initialized and never modified.
- `IsValid` and `EnsureValid` are thread‑safe for reads after the validation run has completed. If validation is executed concurrently with calls to these members, the result may be stale or indeterminate. It is recommended to perform validation once during startup and treat the outcome as immutable thereafter.
- The test‑case strings are human‑readable descriptions, not executable code. The actual validation logic resides in the internal test runner, which is not exposed publicly.
- `EnsureValid` throws `InvalidOperationException` unconditionally when `IsValid` is `false`; it does not provide details about which specific utility failed. For granular diagnostics, iterate the individual `Validate*` lists and run the corresponding internal tests.
- The validation results are platform‑sensitive. Path separator differences, case‑sensitivity of the file system, and environment‑specific directories (application data, temp) mean that a passing result on one OS does not guarantee a passing result on another.
