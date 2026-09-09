#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.IO;
using System.Threading.Tasks;

namespace NAudioVisualizer.Utilities;

/// <summary>
/// Provides utility methods for file system operations.
/// Handles file creation, deletion, directory management with error handling.
/// </summary>
public static class FileSystemUtility
{
    /// <summary>
    /// Creates a directory if it doesn't exist.
    /// Returns true if the directory was created, false if it already existed.
    /// </summary>
    public static bool CreateDirectoryIfNotExists(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));

        try
        {
            var dirInfo = new DirectoryInfo(path);
            if (dirInfo.Exists)
                return false;

            Directory.CreateDirectory(path);
            return true;
        }
        catch (IOException ex)
        {
            throw new IOException($"Failed to create directory '{path}'.", ex);
        }
    }

    /// <summary>
    /// Ensures a directory exists, creating it if necessary.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="path"/> is null, empty, or consists only of white-space characters.</exception>
    public static void EnsureDirectoryExists(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    /// <summary>
    /// Gets the size of a file in bytes.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is null, empty, or consists only of white-space characters.</exception>
    public static long GetFileSize(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var fileInfo = new FileInfo(filePath);
        return fileInfo.Length;
    }

    /// <summary>
    /// Formats a file size in bytes to a human-readable string.
    /// </summary>
    public static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }

    /// <summary>
    /// Generates a unique filename by adding a number suffix if the file exists.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is null, empty, or consists only of white-space characters.</exception>
    public static string GenerateUniqueFileName(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
            return filePath;

        string directory = Path.GetDirectoryName(filePath) ?? ".";
        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
        string extension = Path.GetExtension(filePath);

        int counter = 1;
        string newFileName;

        do
        {
            newFileName = Path.Combine(directory, $"{fileNameWithoutExt}_{counter}{extension}");
            counter++;
        } while (File.Exists(newFileName));

        return newFileName;
    }

    /// <summary>
    /// Safely deletes a file if it exists.
    /// Returns true if the file was deleted, false if it didn't exist.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is null, empty, or consists only of white-space characters.</exception>
    public static bool SafeDeleteFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
            return false;

        try
        {
            File.Delete(filePath);
            return true;
        }
        catch (IOException ex)
        {
            throw new IOException($"Failed to delete file '{filePath}'.", ex);
        }
    }

    /// <summary>
    /// Safely deletes a directory and all its contents.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="dirPath"/> is null, empty, or consists only of white-space characters.</exception>
    public static void SafeDeleteDirectory(string dirPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dirPath);

        if (!Directory.Exists(dirPath))
            return;

        try
        {
            Directory.Delete(dirPath, recursive: true);
        }
        catch (IOException ex)
        {
            throw new IOException($"Failed to delete directory '{dirPath}'.", ex);
        }
    }

    /// <summary>
    /// Checks if a path is a file or directory.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="path"/> is null, empty, or consists only of white-space characters.</exception>
    public static bool IsDirectory(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
    }

    /// <summary>
    /// Gets the relative path from one path to another.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="fromPath"/> or <paramref name="toPath"/> is null, empty, or consists only of white-space characters.</exception>
    public static string GetRelativePath(string fromPath, string toPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fromPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(toPath);

        var fromUri = new Uri(Path.GetFullPath(fromPath));
        var toUri = new Uri(Path.GetFullPath(toPath));

        return Uri.UnescapeDataString(fromUri.MakeRelativeUri(toUri).ToString());
    }

    /// <summary>
    /// Writes text to a file asynchronously.
    /// Creates or overwrites the file.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public static async Task WriteFileAsync(string filePath, string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(content);

        try
        {
            EnsureDirectoryExists(Path.GetDirectoryName(filePath) ?? ".");
            await File.WriteAllTextAsync(filePath, content).ConfigureAwait(false);
        }
        catch (IOException ex)
        {
            throw new IOException($"Failed to write to file '{filePath}'.", ex);
        }
    }

    /// <summary>
    /// Reads text from a file asynchronously.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is null, empty, or consists only of white-space characters.</exception>
    public static async Task<string> ReadFileAsync(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        try
        {
            return await File.ReadAllTextAsync(filePath).ConfigureAwait(false);
        }
        catch (IOException ex)
        {
            throw new IOException($"Failed to read file '{filePath}'.", ex);
        }
    }

    /// <summary>
    /// Cleans up old files in a directory based on retention days.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="directoryPath"/> is null, empty, or consists only of white-space characters.</exception>
    public static int CleanupOldFiles(string directoryPath, int retentionDays)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

        if (!Directory.Exists(directoryPath))
            return 0;

        var directory = new DirectoryInfo(directoryPath);
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
        int deletedCount = 0;

        foreach (var file in directory.GetFiles())
        {
            if (file.LastWriteTimeUtc < cutoffDate)
            {
                try
                {
                    file.Delete();
                    deletedCount++;
                }
                catch
                {
                    // Silently skip files that can't be deleted
                }
            }
        }

        return deletedCount;
    }

    /// <summary>
    /// Gets the total size of a directory and all its contents.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="directoryPath"/> is null, empty, or consists only of white-space characters.</exception>
    public static long GetDirectorySize(string directoryPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

        if (!Directory.Exists(directoryPath))
            return 0;

        var directory = new DirectoryInfo(directoryPath);
        long totalSize = 0;

        foreach (var file in directory.GetFiles())
        {
            totalSize += file.Length;
        }

        foreach (var subDir in directory.GetDirectories())
        {
            totalSize += GetDirectorySize(subDir.FullName);
        }

        return totalSize;
    }
}
