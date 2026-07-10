# PathUtility

Utility class providing static helpers for common file‑system path operations in the **naudio‑visualizer** project. The members are pure functions that accept string inputs and return new string values or simple results without modifying any global state.

## API

### `public static string NormalizePath(string path)`
- **Purpose**: Returns a canonical form of *path* by removing redundant separators, resolving `.` and `..` segments, and ensuring a consistent separator style.
- **Parameters**:  
  - `path` – The path to normalize. Must not be `null`.
- **Return value**: The normalized path string.
- **Exceptions**:  
  - `ArgumentNullException` if `path` is `null`.  
  - `ArgumentException` if `path` contains invalid characters.

### `public static string Combine(string path1, string path2)`
- **Purpose**: Combines two path fragments into a single path, inserting the appropriate directory separator between them if needed.
- **Parameters**:  
  - `path1` – The first path fragment.  
  - `path2` – The second path fragment.  
  Both must not be `null`.
- **Return value**: The combined path.
- **Exceptions**:  
  - `ArgumentNullException` if either argument is `null`.  
  - `ArgumentException` if the combined result would be invalid.

### `public static string GetAbsolutePath(string relativePath)`
- **Purpose**: Converts *relativePath* to an absolute path based on the current working directory.
- **Parameters**:  
  - `relativePath` – A path that may be relative. Must not be `null`.
- **Return value**: The absolute path.
- **Exceptions**:  
  - `ArgumentNullException` if `relativePath` is `null`.  
  - `ArgumentException` if the resulting absolute path cannot be determined (e.g., contains invalid characters).

### `public static string GetRelativePath(string basePath, string targetPath)`
- **Purpose**: Returns the relative path from *basePath* to *targetPath*.
- **Parameters**:  
  - `basePath` – The base directory. Must be an absolute path and not `null`.  
  - `targetPath` – The target file or directory. Must not be `null`.
- **Return value**: A relative path string; returns an empty string if the paths are identical.
- **Exceptions**:  
  - `ArgumentNullException` if either argument is `null`.  
  - `ArgumentException` if `basePath` is not absolute or if the paths are on different drives (Windows) or different UNC hosts.

### `public static string EnsureTrailingSeparator(string path)`
- **Purpose**: Guarantees that *path* ends with the directory separator character (`Path.DirectorySeparatorChar`). If it already ends with a separator, the string is returned unchanged.
- **Parameters**:  
  - `path` – The path to adjust. Must not be `null`.
- **Return value**: The path with a trailing separator.
- **Exceptions**:  
  - `ArgumentNullException` if `path` is `null`.

### `public static string RemoveTrailingSeparator(string path)`
- **Purpose**: Removes a trailing directory separator from *path* if present; otherwise returns the original string.
- **Parameters**:  
  - `path` – The path to adjust. Must not be `null`.
- **Return value**: The path without a trailing separator.
- **Exceptions**:  
  - `ArgumentNullException` if `path` is `null`.

### `public static bool IsAbsolute(string path)`
- **Purpose**: Determines whether *path* is an absolute path.
- **Parameters**:  
  - `path` – The path to test. May be `null`; returns `false` for `null`.
- **Return value**: `true` if *path* is absolute; otherwise `false`.

### `public static bool IsRelative(string path)`
- **Purpose**: Determines whether *path* is a relative path.
- **Parameters**:  
  - `path` – The path to test. May be `null`; returns `false` for `null`.
- **Return value**: `true` if *path* is relative; otherwise `false`.

### `public static IEnumerable<string> GetFilesRecursive(string directoryPath, string searchPattern = "*")`
- **Purpose**: Enumerates all files matching *searchPattern* in *directoryPath* and its subdirectories.
- **Parameters**:  
  - `directoryPath` – The root directory to search. Must exist and not be `null`.  
  - `searchPattern` – Optional search pattern (default `"*"`) following `Directory.GetFiles` rules.
- **Return value**: An `IEnumerable<string>` yielding full file paths.
- **Exceptions**:  
  - `ArgumentNullException` if `directoryPath` is `null`.  
  - `DirectoryNotFoundException` if the directory does not exist.  
  - `UnauthorizedAccessException` if access to a directory is denied.  
  - `ArgumentException` if *searchPattern* contains invalid characters.

### `public static string GetApplicationDirectory()`
- **Purpose**: Returns the directory containing the currently executing assembly.
- **Parameters**: None.
- **Return value**: The absolute path of the application’s base directory.
- **Exceptions**:  
  - `InvalidOperationException` if the assembly location cannot be determined.

### `public static string GetApplicationDataDirectory()`
- **Purpose**: Returns the standard application‑data folder for the current user (equivalent to `Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)`).
- **Parameters**: None.
- **Return value**: The absolute path to the application data directory.
- **Exceptions**: None (relies on .NET framework calls that do not throw under normal circumstances).

### `public static string GetLogsDirectory()`
- **Purpose**: Returns a sub‑directory under the application data folder intended for log files (e.g., `<AppData>\<Company>\<Product>\Logs`).
- **Parameters**: None.
- **Return value**: The absolute path to the logs directory; the directory is not created automatically.
- **Exceptions**: None.

### `public static string GetTempDirectory()`
- **Purpose**: Returns the system temporary directory (equivalent to `Path.GetTempPath()`).
- **Parameters**: None.
- **Return value**: The absolute path to the temp directory.
- **Exceptions**: None.

### `public static bool IsValidPath(string path)`
- **Purpose**: Checks whether *path* contains only valid characters and does not consist solely of whitespace.
- **Parameters**:  
  - `path` – The path to validate. May be `null`; returns `false` for `null`.
- **Return value**: `true` if the path is syntactically valid; otherwise `false`.
- **Exceptions**: None.

### `public static long GetDirectorySize(string directoryPath)`
- **Purpose**: Calculates the total size, in bytes, of all files contained in *directoryPath* and its subdirectories.
- **Parameters**:  
  - `directoryPath` – The directory to measure. Must exist and not be `null`.
- **Return value**: The total size in bytes.
- **Exceptions**:  
  - `ArgumentNullException` if `directoryPath` is `null`.  
  - `DirectoryNotFoundException` if the directory does not exist.  
  - `UnauthorizedAccessException` if access to any sub‑directory is denied.  
  - `IOException` for other I/O errors (e.g., sharing violations).

### `public static string GenerateUniqueFileName(string directoryPath, string fileName)`
- **Purpose**: Returns a file name that does not already exist in *directoryPath* by appending a numeric suffix (e.g., `file (1).txt`) if necessary.
- **Parameters**:  
  - `directoryPath` – The directory to check for name collisions. Must exist and not be `null`.  
  - `fileName` – The desired file name (including extension). Must not be `null` or empty.
- **Return value**: A unique file name suitable for use with `Path.Combine(directoryPath, result)`.
- **Exceptions**:  
  - `ArgumentNullException` if either argument is `null`.  
  - `ArgumentException` if `fileName` is empty or contains invalid path characters.  
  - `DirectoryNotFoundException` if `directoryPath` does not exist.  
  - `UnauthorizedAccessException` if the directory cannot be accessed.

## Usage

```csharp
using System;
using System.IO;
using NAudio.Visualizer; // namespace containing PathUtility

class Program
{
    static void Main()
    {
        // Example 1: Safely build a log file path and ensure uniqueness
        string logDir = PathUtility.GetLogsDirectory();
        string baseName = "app.log";
        string uniqueLog = PathUtility.GenerateUniqueFileName(logDir, baseName);
        string fullPath = Path.Combine(logDir, uniqueLog); // or PathUtility.Combine(logDir, uniqueLog)
        Console.WriteLine($"Log will be written to: {fullPath}");

        // Example 2: Enumerate all .wav files recursively in a folder
        string musicFolder = @"C:\Users\Me\Music";
        foreach (string wav in PathUtility.GetFilesRecursive(musicFolder, "*.wav"))
        {
            Console.WriteLine(wav);
        }
    }
}
```

```csharp
using System;
using NAudio.Visualizer;

class Demo
{
    static void Main()
    {
        // Example 3: Convert a user‑supplied relative path to absolute and verify it's valid
        string userInput = "..\\config\\settings.xml";
        if (PathUtility.IsRelative(userInput))
        {
            string absolute = PathUtility.GetAbsolutePath(userInput);
            Console.WriteLine($"Absolute path: {absolute}");
            if (PathUtility.IsValidPath(absolute))
            {
                Console.WriteLine("Path is syntactically valid.");
            }
        }
        else
        {
            Console.WriteLine("Input is already absolute.");
        }

        // Example 4: Ensure a directory path ends with a separator before appending a file name
        string baseDir = PathUtility.GetApplicationDirectory();
        string ensured = PathUtility.EnsureTrailingSeparator(baseDir);
        string configPath = PathUtility.Combine(ensured, "config.cfg");
        Console.WriteLine($"Config file path: {configPath}");
    }
}
```

## Notes

- All methods are **static** and operate solely on their inputs; they do not modify any internal state, making them thread‑safe for concurrent calls.
- Methods that depend on `Environment.CurrentDirectory` (`GetAbsolutePath`, `IsAbsolute`, `IsRelative`) are sensitive to changes of the current directory during execution; callers should not rely on the current directory remaining unchanged between calls if the application modifies it elsewhere.
- Path validation (`IsValidPath`) checks only for illegal characters and does **not** verify that the path refers to an existing file or directory.
- `GetFilesRecursive` uses deferred enumeration; enumerating the returned `IEnumerable<string>` may throw `UnauthorizedAccessException` or `DirectoryNotFoundException` if the underlying file system state changes during iteration.
- `GenerateUniqueFileName` does **not** create the file; it merely returns a name that is free at the moment of invocation. In highly concurrent scenarios, another process could create the same name after the check, so callers should be prepared to handle `IOException` when actually creating the file.
- Size calculation (`GetDirectorySize`) may overflow the `long` type for extremely large volumes (> 8 EB); in practice this limit is far beyond typical usage.
- On Unix‑like systems, the directory separator is `/`; the methods abstract away separator differences, but callers should still avoid hard‑coding separators in their own code.
