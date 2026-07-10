# FileSystemUtility

A utility class providing safe, high-level file system operations such as directory creation, file deletion, size calculation, and path manipulation. Designed to handle common edge cases like invalid paths, concurrent access, and permission issues with consistent behavior.

## API

### `public static bool CreateDirectoryIfNotExists(string path)`

Creates the specified directory if it does not already exist.

- **Parameters**
  - `path`: The absolute or relative path of the directory to create.
- **Return Value**
  - `true` if the directory was created; `false` if it already existed or could not be created.
- **Exceptions**
  - Throws `ArgumentNullException` if `path` is `null`.
  - Throws `ArgumentException` if `path` is empty or contains invalid characters.
  - Throws `UnauthorizedAccessException` if the caller lacks permissions.
  - Throws `PathTooLongException` if the path exceeds system limits.

---

### `public static void EnsureDirectoryExists(string path)`

Ensures that the specified directory exists, creating it if necessary. Unlike `CreateDirectoryIfNotExists`, this method throws on failure rather than returning a boolean.

- **Parameters**
  - `path`: The absolute or relative path of the directory to ensure.
- **Exceptions**
  - Throws `ArgumentNullException` if `path` is `null`.
  - Throws `ArgumentException` if `path` is empty or invalid.
  - Throws `IOException` if the directory cannot be created.
  - Throws `UnauthorizedAccessException` on permission issues.

---

### `public static long GetFileSize(string filePath)`

Returns the size in bytes of the specified file.

- **Parameters**
  - `filePath`: The path to the file.
- **Return Value**
  - The file size in bytes.
- **Exceptions**
  - Throws `ArgumentNullException` if `filePath` is `null`.
  - Throws `FileNotFoundException` if the file does not exist.
  - Throws `UnauthorizedAccessException` if the caller lacks read permissions.
  - Throws `IOException` on general I/O failure.

---

### `public static string FormatFileSize(long bytes)`

Formats a file size in bytes into a human-readable string (e.g., "1.23 MB").

- **Parameters**
  - `bytes`: The size in bytes to format.
- **Return Value**
  - A formatted string with appropriate unit (B, KB, MB, GB, etc.).
- **Exceptions**
  - None.

---

### `public static string GenerateUniqueFileName(string directory, string baseName, string extension)`

Generates a unique filename in the specified directory by appending a numeric suffix if necessary.

- **Parameters**
  - `directory`: The directory where the file should be created.
  - `baseName`: The base name of the file (without extension).
  - `extension`: The file extension (including leading dot, e.g., ".txt").
- **Return Value**
  - A full path to a unique file.
- **Exceptions**
  - Throws `ArgumentNullException` if any parameter is `null`.
  - Throws `ArgumentException` if `extension` is invalid or `directory` does not exist.
  - Throws `UnauthorizedAccessException` on permission issues.

---

### `public static bool SafeDeleteFile(string filePath)`

Safely deletes a file if it exists. Returns whether the file was deleted.

- **Parameters**
  - `filePath`: The path to the file to delete.
- **Return Value**
  - `true` if the file existed and was deleted; `false` if it did not exist.
- **Exceptions**
  - Throws `ArgumentNullException` if `filePath` is `null`.
  - Throws `UnauthorizedAccessException` if the caller lacks delete permissions.
  - Throws `IOException` on general I/O failure.

---

### `public static void SafeDeleteDirectory(string directoryPath)`

Safely deletes a directory and its contents if it exists. Unlike `SafeDeleteFile`, this method throws on failure.

- **Parameters**
  - `directoryPath`: The path to the directory to delete.
- **Exceptions**
  - Throws `ArgumentNullException` if `directoryPath` is `null`.
  - Throws `DirectoryNotFoundException` if the directory does not exist.
  - Throws `UnauthorizedAccessException` on permission issues.
  - Throws `IOException` if the directory cannot be deleted.

---
### `public static bool IsDirectory(string path)`

Determines whether the specified path points to an existing directory.

- **Parameters**
  - `path`: The path to check.
- **Return Value**
  - `true` if the path exists and is a directory; otherwise, `false`.
- **Exceptions**
  - Throws `ArgumentNullException` if `path` is `null`.
  - Throws `ArgumentException` if `path` is invalid.

---
### `public static string GetRelativePath(string relativeTo, string path)`

Computes the relative path from `relativeTo` to `path`.

- **Parameters**
  - `relativeTo`: The base directory used to compute the relative path.
  - `path`: The target path.
- **Return Value**
  - A relative path string.
- **Exceptions**
  - Throws `ArgumentNullException` if either parameter is `null`.
  - Throws `ArgumentException` if paths are invalid or on format failure.

---
### `public static async Task WriteFileAsync(string filePath, byte[] data, bool overwrite = false)`

Asynchronously writes binary data to a file.

- **Parameters**
  - `filePath`: The destination file path.
  - `data`: The byte array to write.
  - `overwrite`: If `true`, overwrites the file if it exists.
- **Exceptions**
  - Throws `ArgumentNullException` if `filePath` or `data` is `null`.
  - Throws `ArgumentException` if `filePath` is invalid.
  - Throws `UnauthorizedAccessException` on permission issues.
  - Throws `IOException` on I/O failure.

---
### `public static async Task<string> ReadFileAsync(string filePath)`

Asynchronously reads the entire contents of a file as a string.

- **Parameters**
  - `filePath`: The path to the file to read.
- **Return Value**
  - A `Task<string>` containing the file contents.
- **Exceptions**
  - Throws `ArgumentNullException` if `filePath` is `null`.
  - Throws `FileNotFoundException` if the file does not exist.
  - Throws `UnauthorizedAccessException` on permission issues.
  - Throws `IOException` on I/O failure.

---
### `public static int CleanupOldFiles(string directory, int maxAgeDays)`

Deletes files in the specified directory older than the given age in days. Returns the number of files deleted.

- **Parameters**
  - `directory`: The directory to clean.
  - `maxAgeDays`: The maximum age in days; files older than this are deleted.
- **Return Value**
  - The number of files deleted.
- **Exceptions**
  - Throws `ArgumentNullException` if `directory` is `null`.
  - Throws `ArgumentException` if `directory` does not exist or `maxAgeDays` is negative.
  - Throws `UnauthorizedAccessException` on permission issues.
  - Throws `IOException` on I/O failure.

---
### `public static long GetDirectorySize(string directoryPath)`

Recursively calculates the total size in bytes of all files under the specified directory.

- **Parameters**
  - `directoryPath`: The root directory to measure.
- **Return Value**
  - The total size in bytes.
- **Exceptions**
  - Throws `ArgumentNullException` if `directoryPath` is `null`.
  - Throws `DirectoryNotFoundException` if the directory does not exist.
  - Throws `UnauthorizedAccessException` on permission issues.
  - Throws `IOException` on I/O failure.

---

## Usage
