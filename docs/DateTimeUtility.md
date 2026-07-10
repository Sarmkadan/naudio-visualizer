# DateTimeUtility
The `DateTimeUtility` class provides a set of static methods for working with dates and times in C#. It offers various utility functions for converting between different date and time formats, calculating time intervals, and determining relative dates. This class is designed to simplify common date and time operations, making it easier to work with temporal data in .NET applications.

## API
* `public static long GetCurrentTimestampMs`: Returns the current timestamp in milliseconds since the Unix epoch (January 1, 1970, 00:00:00 UTC). This method does not take any parameters and does not throw any exceptions.
* `public static long ToTimestampMs(DateTime dateTime)`: Converts a `DateTime` object to a timestamp in milliseconds since the Unix epoch. This method takes a `DateTime` object as a parameter and returns the corresponding timestamp. It does not throw any exceptions.
* `public static DateTime FromTimestampMs(long timestampMs)`: Converts a timestamp in milliseconds since the Unix epoch to a `DateTime` object. This method takes a `long` timestamp as a parameter and returns the corresponding `DateTime` object. It does not throw any exceptions.
* `public static string ToIso8601(DateTime dateTime)`: Converts a `DateTime` object to an ISO 8601-formatted string. This method takes a `DateTime` object as a parameter and returns the corresponding ISO 8601 string. It does not throw any exceptions.
* `public static DateTime FromIso8601(string iso8601String)`: Converts an ISO 8601-formatted string to a `DateTime` object. This method takes an ISO 8601 string as a parameter and returns the corresponding `DateTime` object. It throws a `FormatException` if the input string is not a valid ISO 8601 date and time.
* `public static string FormatDuration(TimeSpan timeSpan)`: Formats a `TimeSpan` object as a human-readable duration string. This method takes a `TimeSpan` object as a parameter and returns the formatted duration string. It does not throw any exceptions.
* `public static int DaysBetween(DateTime startDate, DateTime endDate)`: Calculates the number of days between two `DateTime` objects. This method takes two `DateTime` objects as parameters and returns the number of days between them. It does not throw any exceptions.
* `public static bool IsToday(DateTime dateTime)`: Determines whether a `DateTime` object represents the current date. This method takes a `DateTime` object as a parameter and returns `true` if it represents the current date, `false` otherwise. It does not throw any exceptions.
* `public static bool IsInPast(DateTime dateTime)`: Determines whether a `DateTime` object represents a date and time in the past. This method takes a `DateTime` object as a parameter and returns `true` if it represents a date and time in the past, `false` otherwise. It does not throw any exceptions.
* `public static bool IsInFuture(DateTime dateTime)`: Determines whether a `DateTime` object represents a date and time in the future. This method takes a `DateTime` object as a parameter and returns `true` if it represents a date and time in the future, `false` otherwise. It does not throw any exceptions.
* `public static string GetRelativeTime(DateTime dateTime)`: Returns a human-readable string representing the relative time between the given `DateTime` object and the current date and time. This method takes a `DateTime` object as a parameter and returns the relative time string. It does not throw any exceptions.
* `public static DateTime GetStartOfDay(DateTime dateTime)`: Returns the start of the day for a given `DateTime` object. This method takes a `DateTime` object as a parameter and returns the start of the day. It does not throw any exceptions.
* `public static DateTime GetEndOfDay(DateTime dateTime)`: Returns the end of the day for a given `DateTime` object. This method takes a `DateTime` object as a parameter and returns the end of the day. It does not throw any exceptions.
* `public static DateTime GetStartOfWeek(DateTime dateTime)`: Returns the start of the week for a given `DateTime` object. This method takes a `DateTime` object as a parameter and returns the start of the week. It does not throw any exceptions.
* `public static DateTime GetStartOfMonth(DateTime dateTime)`: Returns the start of the month for a given `DateTime` object. This method takes a `DateTime` object as a parameter and returns the start of the month. It does not throw any exceptions.
* `public static DateTime GetEndOfMonth(DateTime dateTime)`: Returns the end of the month for a given `DateTime` object. This method takes a `DateTime` object as a parameter and returns the end of the month. It does not throw any exceptions.
* `public static int CalculateAge(DateTime birthDate)`: Calculates the age in years for a given birth date. This method takes a `DateTime` object as a parameter and returns the age in years. It does not throw any exceptions.
* `public static string GetDayName(DateTime dateTime)`: Returns the day of the week as a string for a given `DateTime` object. This method takes a `DateTime` object as a parameter and returns the day of the week string. It does not throw any exceptions.
* `public static bool IsLeapYear(int year)`: Determines whether a given year is a leap year. This method takes an `int` year as a parameter and returns `true` if it is a leap year, `false` otherwise. It does not throw any exceptions.

## Usage
The following examples demonstrate how to use the `DateTimeUtility` class:
```csharp
// Example 1: Converting between date and time formats
DateTime dateTime = new DateTime(2022, 1, 1, 12, 0, 0);
long timestampMs = DateTimeUtility.ToTimestampMs(dateTime);
string iso8601String = DateTimeUtility.ToIso8601(dateTime);
Console.WriteLine($"Timestamp (ms): {timestampMs}");
Console.WriteLine($"ISO 8601: {iso8601String}");

// Example 2: Calculating time intervals and relative dates
DateTime startDate = new DateTime(2022, 1, 1);
DateTime endDate = new DateTime(2022, 1, 15);
int daysBetween = DateTimeUtility.DaysBetween(startDate, endDate);
string relativeTime = DateTimeUtility.GetRelativeTime(endDate);
Console.WriteLine($"Days between: {daysBetween}");
Console.WriteLine($"Relative time: {relativeTime}");
```

## Notes
The `DateTimeUtility` class is designed to be thread-safe, as all methods are static and do not rely on instance state. However, when working with dates and times, it is essential to consider the implications of daylight saving time (DST) and time zone conversions. The `DateTimeUtility` class does not account for these factors, so you may need to use additional libraries or frameworks to handle these scenarios. Additionally, the `FromIso8601` method may throw a `FormatException` if the input string is not a valid ISO 8601 date and time, so you should handle this exception accordingly in your application.
