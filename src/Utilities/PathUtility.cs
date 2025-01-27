// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.IO;
using System.Collections.Generic;

namespace NAudioVisualizer.Utilities;

/// <summary>
/// Provides utility methods for path manipulation and validation.
/// Handles cross-platform path operations and normalization.
/// </summary>
public static class PathUtility
{
    /// <summary>
    /// Normalizes a path to use forward slashes regardless of platform.
    /// </summary>
    public static string NormalizePath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        return path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
    }

    /// <summary>
    /// Combines multiple path segments safely.
    /// </summary>
    public static string Combine(params string[] segments)
    {
        if (segments == null || segments.Length == 0)
            return string.Empty;

        string result = segments[0];
        for (int i = 1; i < segments.Length; i++)
        {
            result = Path.Combine(result, segments[i]);
        }

        return result;
    }

    /// <summary>
    /// Gets the absolute path for a relative path.
    /// </summary>
    public static string GetAbsolutePath(string relativePath)
    {
        return Path.GetFullPath(relativePath);
    }

    /// <summary>
    /// Gets the relative path from one directory to another.
    /// </summary>
    public static string GetRelativePath(string fromPath, string toPath)
    {
        if (string.IsNullOrEmpty(fromPath))
            throw new ArgumentException("From path cannot be null or empty.", nameof(fromPath));

        if (string.IsNullOrEmpty(toPath))
            throw new ArgumentException("To path cannot be null or empty.", nameof(toPath));

        var fromUri = new Uri(Path.GetFullPath(fromPath));
        var toUri = new Uri(Path.GetFullPath(toPath));

        return Uri.UnescapeDataString(fromUri.MakeRelativeUri(toUri).ToString());
    }

    /// <summary>
    /// Ensures a path ends with a directory separator.
    /// </summary>
    public static string EnsureTrailingSeparator(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            path += Path.DirectorySeparatorChar;

        return path;
    }

    /// <summary>
    /// Removes the trailing directory separator from a path.
    /// </summary>
    public static string RemoveTrailingSeparator(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        return path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    /// <summary>
    /// Checks if a path is absolute.
    /// </summary>
    public static bool IsAbsolute(string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        return Path.IsPathRooted(path);
    }

    /// <summary>
    /// Checks if a path is relative.
    /// </summary>
    public static bool IsRelative(string path)
    {
        return !IsAbsolute(path);
    }

    /// <summary>
    /// Gets all files in a directory recursively with a specific pattern.
    /// </summary>
    public static IEnumerable<string> GetFilesRecursive(string directoryPath, string pattern = "*.*")
    {
        if (!Directory.Exists(directoryPath))
            yield break;

        var directory = new DirectoryInfo(directoryPath);

        foreach (var file in directory.GetFiles(pattern))
        {
            yield return file.FullName;
        }

        foreach (var subDir in directory.GetDirectories())
        {
            foreach (var file in GetFilesRecursive(subDir.FullName, pattern))
            {
                yield return file;
            }
        }
    }

    /// <summary>
    /// Gets the application base directory.
    /// </summary>
    public static string GetApplicationDirectory()
    {
        return AppDomain.CurrentDomain.BaseDirectory;
    }

    /// <summary>
    /// Gets the directory for storing application data.
    /// </summary>
    public static string GetApplicationDataDirectory()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string appDir = Path.Combine(appData, "NAudioVisualizer");

        if (!Directory.Exists(appDir))
            Directory.CreateDirectory(appDir);

        return appDir;
    }

    /// <summary>
    /// Gets the logs directory.
    /// </summary>
    public static string GetLogsDirectory()
    {
        string logsDir = Path.Combine(GetApplicationDataDirectory(), "logs");

        if (!Directory.Exists(logsDir))
            Directory.CreateDirectory(logsDir);

        return logsDir;
    }

    /// <summary>
    /// Gets the temporary files directory.
    /// </summary>
    public static string GetTempDirectory()
    {
        string tempDir = Path.Combine(GetApplicationDataDirectory(), "temp");

        if (!Directory.Exists(tempDir))
            Directory.CreateDirectory(tempDir);

        return tempDir;
    }

    /// <summary>
    /// Checks if a path is valid for the current operating system.
    /// </summary>
    public static bool IsValidPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        try
        {
            Path.GetFullPath(path);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the size of a directory in bytes.
    /// </summary>
    public static long GetDirectorySize(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            return 0;

        long totalSize = 0;
        var directory = new DirectoryInfo(directoryPath);

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

    /// <summary>
    /// Generates a unique filename by appending a number if the file exists.
    /// </summary>
    public static string GenerateUniqueFileName(string filePath)
    {
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
}
