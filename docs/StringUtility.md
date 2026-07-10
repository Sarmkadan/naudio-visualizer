# StringUtility
The `StringUtility` class provides a set of static methods for performing various string manipulation and formatting operations. These methods can be used to truncate strings, repeat characters, format numbers and bytes, convert case, and more. This utility class is designed to be used throughout an application to simplify string handling and improve code readability.

## API
* `public static string Truncate(string value, int length)`: Truncates a string to the specified length. Returns the truncated string. Throws `ArgumentNullException` if `value` is null.
* `public static string Repeat(string value, int count)`: Repeats a string the specified number of times. Returns the repeated string. Throws `ArgumentNullException` if `value` is null.
* `public static string PadCenter(string value, int length)`: Pads a string with spaces to center it within the specified length. Returns the padded string. Throws `ArgumentNullException` if `value` is null.
* `public static string FormatBytes(long bytes)`: Formats a byte count as a human-readable string (e.g., "1 KB", "2 MB", etc.). Returns the formatted string.
* `public static string FormatMilliseconds(long milliseconds)`: Formats a millisecond count as a human-readable string (e.g., "1 ms", "2 s", etc.). Returns the formatted string.
* `public static string FormatLargeNumber(long number)`: Formats a large number as a human-readable string with commas (e.g., "1,000", "2,000,000", etc.). Returns the formatted string.
* `public static string ToTitleCase(string value)`: Converts a string to title case (first letter of each word capitalized). Returns the converted string. Throws `ArgumentNullException` if `value` is null.
* `public static string ToSnakeCase(string value)`: Converts a string to snake case (all letters in lowercase, words separated by underscores). Returns the converted string. Throws `ArgumentNullException` if `value` is null.
* `public static string ToCamelCase(string value)`: Converts a string to camel case (first letter of each word capitalized, except the first word). Returns the converted string. Throws `ArgumentNullException` if `value` is null.
* `public static string RemoveWhitespace(string value)`: Removes all whitespace characters from a string. Returns the resulting string. Throws `ArgumentNullException` if `value` is null.
* `public static bool IsAlphanumeric(string value)`: Checks if a string contains only alphanumeric characters. Returns `true` if the string is alphanumeric, `false` otherwise. Throws `ArgumentNullException` if `value` is null.
* `public static int CountOccurrences(string value, string search)`: Counts the number of occurrences of a substring within a string. Returns the count. Throws `ArgumentNullException` if `value` or `search` is null.

## Usage
```csharp
// Example 1: Formatting numbers and bytes
long bytes = 1024 * 1024 * 5; // 5 MB
long milliseconds = 1500; // 1.5 seconds
Console.WriteLine(StringUtility.FormatBytes(bytes)); // Output: "5 MB"
Console.WriteLine(StringUtility.FormatMilliseconds(milliseconds)); // Output: "1.5 s"

// Example 2: Converting case and removing whitespace
string original = "  Hello, World!  ";
Console.WriteLine(StringUtility.ToTitleCase(original)); // Output: " Hello, World! "
Console.WriteLine(StringUtility.RemoveWhitespace(original)); // Output: "Hello,World!"
```

## Notes
The `StringUtility` class is designed to be thread-safe, as all methods are static and do not rely on any shared state. However, it is still important to note that some methods may throw exceptions if null or invalid input is provided. Additionally, the `FormatBytes` and `FormatMilliseconds` methods use a fixed set of units (e.g., KB, MB, GB, etc.) and may not be suitable for very large or very small values. The `ToTitleCase`, `ToSnakeCase`, and `ToCamelCase` methods assume that the input string is in a standard English format and may not work correctly for strings in other languages or formats.
