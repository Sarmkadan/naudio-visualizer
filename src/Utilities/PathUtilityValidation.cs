#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.IO;
using System.Collections.Generic;

namespace NAudioVisualizer.Utilities;

/// <summary>
/// Provides validation helpers for PathUtility operations.
/// </summary>
public static class PathUtilityValidation
{
    /// <summary>
    /// Validates a path normalization operation.
    /// </summary>
    /// <param name="path">Path to validate</param>
    /// <returns>List of validation problems (empty if valid)</returns>
    public static IReadOnlyList<string> ValidateNormalizePath(string path)
    {
        var problems = new List<string>();

        // Null handling
        if (PathUtility.NormalizePath(null) != null)
            problems.Add("NormalizePath(null) should return null");

        // Empty string handling
        if (PathUtility.NormalizePath(string.Empty) != string.Empty)
            problems.Add("NormalizePath(string.Empty) should return string.Empty");

        // Backslash conversion
        string testPath = "C:\\Users\\Test\\file.txt";
        if (PathUtility.NormalizePath(testPath).Contains('\\'))
            problems.Add("NormalizePath should convert backslashes to platform separators");

        return problems;
    }

    /// <summary>
    /// Validates path combination.
    /// </summary>
    /// <param name="segments">Path segments to validate</param>
    /// <returns>List of validation problems (empty if valid)</returns>
    public static IReadOnlyList<string> ValidateCombine(string[] segments)
    {
        var problems = new List<string>();

        // Null array
        if (PathUtility.Combine(null) != string.Empty)
            problems.Add("Combine(null) should return string.Empty");

        // Empty array
        if (PathUtility.Combine(Array.Empty<string>()) != string.Empty)
            problems.Add("Combine(Array.Empty) should return string.Empty");

        // Valid segments
        string result = PathUtility.Combine("folder", "subfolder", "file.txt");
        if (string.IsNullOrEmpty(result) || !result.Contains("folder"))
            problems.Add("Combine should properly combine segments");

        return problems;
    }

    /// <summary>
    /// Validates absolute path conversion.
    /// </summary>
    /// <param name="relativePath">Relative path to validate</param>
    /// <returns>List of validation problems (empty if valid)</returns>
    public static IReadOnlyList<string> ValidateGetAbsolutePath(string relativePath)
    {
        var problems = new List<string>();

        // Null handling
        try
        {
            PathUtility.GetAbsolutePath(null);
            problems.Add("GetAbsolutePath(null) should throw ArgumentNullException");
        }
        catch (ArgumentNullException) { }
        catch { problems.Add("GetAbsolutePath(null) should throw exception"); }

        // Empty string
        try
        {
            PathUtility.GetAbsolutePath(string.Empty);
            problems.Add("GetAbsolutePath(string.Empty) should throw exception");
        }
        catch { }

        // Valid relative path
        string absolute = PathUtility.GetAbsolutePath("test.txt");
        if (string.IsNullOrEmpty(absolute))
            problems.Add("GetAbsolutePath should return non-empty path");
        if (!Path.IsPathRooted(absolute))
            problems.Add("GetAbsolutePath should return absolute path");

        return problems;
    }

    /// <summary>
    /// Validates relative path calculation.
    /// </summary>
    /// <param name="fromPath">Source path</param>
    /// <param name="toPath">Target path</param>
    /// <returns>List of validation problems (empty if valid)</returns>
    public static IReadOnlyList<string> ValidateGetRelativePath(string fromPath, string toPath)
    {
        var problems = new List<string>();

        // Null fromPath
        try
        {
            PathUtility.GetRelativePath(null, "target");
            problems.Add("GetRelativePath(null, target) should throw ArgumentException");
        }
        catch (ArgumentException) { }
        catch { problems.Add("GetRelativePath(null, target) should throw exception"); }

        // Null toPath
        try
        {
            PathUtility.GetRelativePath("source", null);
            problems.Add("GetRelativePath(source, null) should throw ArgumentException");
        }
        catch (ArgumentException) { }
        catch { problems.Add("GetRelativePath(source, null) should throw exception"); }

        // Empty strings
        try
        {
            PathUtility.GetRelativePath(string.Empty, string.Empty);
            problems.Add("GetRelativePath(string.Empty, string.Empty) should throw exception");
        }
        catch { }

        // Valid paths
        string relative = PathUtility.GetRelativePath("/home/user", "/home/user/docs");
        if (string.IsNullOrEmpty(relative))
            problems.Add("GetRelativePath should return non-empty result");

        return problems;
    }

    /// <summary>
    /// Validates trailing separator addition.
    /// </summary>
    /// <param name="path">Path to validate</param>
    /// <returns>List of validation problems (empty if valid)</returns>
    public static IReadOnlyList<string> ValidateEnsureTrailingSeparator(string path)
    {
        var problems = new List<string>();

        // Null
        if (PathUtility.EnsureTrailingSeparator(null) != null)
            problems.Add("EnsureTrailingSeparator(null) should return null");

        // Empty string
        if (PathUtility.EnsureTrailingSeparator(string.Empty) != string.Empty)
            problems.Add("EnsureTrailingSeparator(string.Empty) should return string.Empty");

        // Add separator
        string path1 = "/home/user";
        if (!PathUtility.EnsureTrailingSeparator(path1).EndsWith(Path.DirectorySeparatorChar.ToString()))
            problems.Add("EnsureTrailingSeparator should add trailing separator");

        // Preserve existing separator
        string path2 = "/home/user/";
        if (!PathUtility.EnsureTrailingSeparator(path2).EndsWith(Path.DirectorySeparatorChar.ToString()))
            problems.Add("EnsureTrailingSeparator should preserve existing separator");

        return problems;
    }

    /// <summary>
    /// Validates trailing separator removal.
    /// </summary>
    /// <param name="path">Path to validate</param>
    /// <returns>List of validation problems (empty if valid)</returns>
    public static IReadOnlyList<string> ValidateRemoveTrailingSeparator(string path)
    {
        var problems = new List<string>();

        // Null
        if (PathUtility.RemoveTrailingSeparator(null) != null)
            problems.Add("RemoveTrailingSeparator(null) should return null");

        // Empty string
        if (PathUtility.RemoveTrailingSeparator(string.Empty) != string.Empty)
            problems.Add("RemoveTrailingSeparator(string.Empty) should return string.Empty");

        // Remove separator
        string path1 = "/home/user/";
        if (PathUtility.RemoveTrailingSeparator(path1).EndsWith(Path.DirectorySeparatorChar.ToString()))
            problems.Add("RemoveTrailingSeparator should remove trailing separator");

        // Preserve non-separator path
        string path2 = "/home/user";
        if (PathUtility.RemoveTrailingSeparator(path2) != path2)
            problems.Add("RemoveTrailingSeparator should not modify paths without trailing separator");

        return problems;
    }

    /// <summary>
    /// Validates absolute path detection.
    /// </summary>
    /// <param name="path">Path to validate</param>
    /// <returns>List of validation problems (empty if valid)</returns>
    public static IReadOnlyList<string> ValidateIsAbsolute(string path)
    {
        var problems = new List<string>();

        // Null
        if (PathUtility.IsAbsolute(null))
            problems.Add("IsAbsolute(null) should return false");

        // Empty string
        if (PathUtility.IsAbsolute(string.Empty))
            problems.Add("IsAbsolute(string.Empty) should return false");

        // Absolute path
        if (!PathUtility.IsAbsolute("/home/user/file.txt"))
            problems.Add("IsAbsolute should return true for absolute paths");

        // Relative path
        if (PathUtility.IsAbsolute("relative/path"))
            problems.Add("IsAbsolute should return false for relative paths");

        return problems;
    }

    /// <summary>
    /// Validates relative path detection.
    /// </summary>
    /// <param name="path">Path to validate</param>
    /// <returns>List of validation problems (empty if valid)</returns>
    public static IReadOnlyList<string> ValidateIsRelative(string path)
    {
        var problems = new List<string>();

        // Null
        if (PathUtility.IsRelative(null))
            problems.Add("IsRelative(null) should return false");

        // Empty string
        if (PathUtility.IsRelative(string.Empty))
            problems.Add("IsRelative(string.Empty) should return false");

        // Relative path
        if (!PathUtility.IsRelative("relative/path"))
            problems.Add("IsRelative should return true for relative paths");

        // Absolute path
        if (PathUtility.IsRelative("/home/user/file.txt"))
            problems.Add("IsRelative should return false for absolute paths");

        return problems;
    }

    /// <summary>
    /// Validates recursive file enumeration.
    /// </summary>
    /// <param name="directoryPath">Directory path to validate</param>
    /// <returns>List of validation problems (empty if valid)</returns>
    public static IReadOnlyList<string> ValidateGetFilesRecursive(string directoryPath)
    {
        var problems = new List<string>();

        // Null
        var files1 = PathUtility.GetFilesRecursive(null);
        if (files1 != null && files1.GetEnumerator().MoveNext())
            problems.Add("GetFilesRecursive(null) should return empty enumerable");

        // Empty string
        var files2 = PathUtility.GetFilesRecursive(string.Empty);
        if (files2 != null && files2.GetEnumerator().MoveNext())
            problems.Add("GetFilesRecursive(string.Empty) should return empty enumerable");

        // Non-existent directory
        var files3 = PathUtility.GetFilesRecursive("/nonexistent/directory/12345");
        if (files3 != null && files3.GetEnumerator().MoveNext())
            problems.Add("GetFilesRecursive(non-existent) should return empty enumerable");

        return problems;
    }

    /// <summary>
    /// Validates application directory retrieval.
    /// </summary>
    /// <returns>List of validation problems (empty if valid)</returns>
    public static IReadOnlyList<string> ValidateGetApplicationDirectory()
    {
        var problems = new List<string>();

        string appDir = PathUtility.GetApplicationDirectory();
        if (string.IsNullOrEmpty(appDir))
            problems.Add("GetApplicationDirectory should return non-empty path");
        if (!Directory.Exists(appDir))
            problems.Add("GetApplicationDirectory should return existing directory");

        return problems;
    }

    /// <summary>
    /// Validates application data directory retrieval.
    /// </summary>
    /// <returns>List of validation problems (empty if valid)</returns>
    public static IReadOnlyList<string> ValidateGetApplicationDataDirectory()
    {
        var problems = new List<string>();

        string appDataDir = PathUtility.GetApplicationDataDirectory();
        if (string.IsNullOrEmpty(appDataDir))
            problems.Add("GetApplicationDataDirectory should return non-empty path");
        if (!Directory.Exists(appDataDir))
            problems.Add("GetApplicationDataDirectory should create and return existing directory");
        if (!appDataDir.Contains("NAudioVisualizer"))
            problems.Add("GetApplicationDataDirectory should contain NAudioVisualizer in path");

        return problems;
    }

    /// <summary>
    /// Validates logs directory retrieval.
    /// </summary>
    /// <returns>List of validation problems (empty if valid)</returns>
    public static IReadOnlyList<string> ValidateGetLogsDirectory()
    {
        var problems = new List<string>();

        string logsDir = PathUtility.GetLogsDirectory();
        if (string.IsNullOrEmpty(logsDir))
            problems.Add("GetLogsDirectory should return non-empty path");
        if (!Directory.Exists(logsDir))
            problems.Add("GetLogsDirectory should create and return existing directory");
        if (!logsDir.Contains("logs"))
            problems.Add("GetLogsDirectory should contain 'logs' in path");

        return problems;
    }

    /// <summary>
    /// Validates temp directory retrieval.
    /// </summary>
    /// <returns>List of validation problems (empty if valid)</returns>
    public static IReadOnlyList<string> ValidateGetTempDirectory()
    {
        var problems = new List<string>();

        string tempDir = PathUtility.GetTempDirectory();
        if (string.IsNullOrEmpty(tempDir))
            problems.Add("GetTempDirectory should return non-empty path");
        if (!Directory.Exists(tempDir))
            problems.Add("GetTempDirectory should create and return existing directory");
        if (!tempDir.Contains("temp"))
            problems.Add("GetTempDirectory should contain 'temp' in path");

        return problems;
    }

    /// <summary>
    /// Validates path validity checking.
    /// </summary>
    /// <param name="path">Path to validate</param>
    /// <returns>List of validation problems (empty if valid)</returns>
    public static IReadOnlyList<string> ValidateIsValidPath(string path)
    {
        var problems = new List<string>();

        // Null
        if (PathUtility.IsValidPath(null))
            problems.Add("IsValidPath(null) should return false");

        // Empty string
        if (PathUtility.IsValidPath(string.Empty))
            problems.Add("IsValidPath(string.Empty) should return false");

        // Valid path
        if (!PathUtility.IsValidPath("test.txt"))
            problems.Add("IsValidPath should return true for valid paths");

        return problems;
    }

    /// <summary>
    /// Validates directory size calculation.
    /// </summary>
    /// <param name="directoryPath">Directory path to validate</param>
    /// <returns>List of validation problems (empty if valid)</returns>
    public static IReadOnlyList<string> ValidateGetDirectorySize(string directoryPath)
    {
        var problems = new List<string>();

        // Null
        if (PathUtility.GetDirectorySize(null) != 0)
            problems.Add("GetDirectorySize(null) should return 0");

        // Empty string
        if (PathUtility.GetDirectorySize(string.Empty) != 0)
            problems.Add("GetDirectorySize(string.Empty) should return 0");

        // Non-existent directory
        if (PathUtility.GetDirectorySize("/nonexistent/directory/12345") != 0)
            problems.Add("GetDirectorySize(non-existent) should return 0");

        // Valid directory
        string tempDir = Path.GetTempPath();
        long size = PathUtility.GetDirectorySize(tempDir);
        if (size < 0)
            problems.Add("GetDirectorySize should return non-negative value");

        return problems;
    }

    /// <summary>
    /// Validates unique filename generation.
    /// </summary>
    /// <param name="filePath">File path to validate</param>
    /// <returns>List of validation problems (empty if valid)</returns>
    public static IReadOnlyList<string> ValidateGenerateUniqueFileName(string filePath)
    {
        var problems = new List<string>();

        // Null
        try
        {
            PathUtility.GenerateUniqueFileName(null);
            problems.Add("GenerateUniqueFileName(null) should throw exception");
        }
        catch { }

        // Empty string
        try
        {
            PathUtility.GenerateUniqueFileName(string.Empty);
            problems.Add("GenerateUniqueFileName(string.Empty) should throw exception");
        }
        catch { }

        // Non-existent file
        string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        string uniqueName = PathUtility.GenerateUniqueFileName(tempFile);
        if (uniqueName != tempFile)
            problems.Add("GenerateUniqueFileName should return same path for non-existent files");

        // Existing file
        string existingFile = Path.GetTempFileName();
        string uniqueName2 = PathUtility.GenerateUniqueFileName(existingFile);
        if (uniqueName2 == existingFile)
            problems.Add("GenerateUniqueFileName should return different path for existing files");
        if (File.Exists(uniqueName2))
            problems.Add("GenerateUniqueFileName should return non-existent path");

        // Cleanup
        try { File.Delete(existingFile); File.Delete(uniqueName2); } catch { }

        return problems;
    }

    /// <summary>
    /// Checks if a PathUtility operation result is valid.
    /// </summary>
    /// <param name="problems">List of validation problems</param>
    /// <returns>True if no problems found</returns>
    public static bool IsValid(this IReadOnlyList<string> problems)
    {
        return problems == null || problems.Count == 0;
    }

    /// <summary>
    /// Ensures a PathUtility operation result is valid, throwing if not.
    /// </summary>
    /// <param name="problems">List of validation problems</param>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    public static void EnsureValid(this IReadOnlyList<string> problems)
    {
        if (problems == null || problems.Count == 0)
            return;

        throw new ArgumentException(
            $"PathUtility validation failed:\n{string.Join("\n", problems)}");
    }
}