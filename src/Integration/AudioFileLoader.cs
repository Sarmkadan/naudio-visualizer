#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.IO;
using System.Threading.Tasks;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Infrastructure;
using NAudioVisualizer.Utilities;

namespace NAudioVisualizer.Integration;

/// <summary>
/// Loads audio files and converts them to audio frames for visualization.
/// Supports various audio file formats through file extension detection.
/// </summary>
public class AudioFileLoader
{
    private readonly Logger _logger;
    private const int MaxFileSize = 500 * 1024 * 1024; // 500 MB limit

    /// <summary>
    /// Initializes a new instance of the audio file loader.
    /// </summary>
    public AudioFileLoader(Logger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Loads an audio file and returns its metadata.
    /// </summary>
    public async Task<AudioMetadata> LoadAudioFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Audio file not found: {filePath}");

        try
        {
            var fileInfo = new FileInfo(filePath);

            // Validate file size
            if (fileInfo.Length > MaxFileSize)
                throw new InvalidOperationException($"Audio file exceeds maximum size of {FileSystemUtility.FormatFileSize(MaxFileSize)}");

            // Validate file format
            string extension = Path.GetExtension(filePath).ToLower();
            if (!IsSupportedFormat(extension))
                throw new NotSupportedException($"Audio format '{extension}' is not supported.");

            var metadata = new AudioMetadata
            {
                FilePath = Path.GetFullPath(filePath),
                FileName = Path.GetFileName(filePath),
                FileSize = fileInfo.Length,
                CreatedDate = fileInfo.CreationTimeUtc,
                ModifiedDate = fileInfo.LastWriteTimeUtc,
                Format = extension.TrimStart('.'),
                LoadedAt = DateTime.UtcNow
            };

            // Attempt to extract additional metadata if available
            try
            {
                await ExtractAudioPropertiesAsync(metadata);
            }
            catch (Exception ex)
            {
                _logger.Warn($"Could not extract extended audio properties: {ex.Message}");
            }

            _logger.Info($"Loaded audio file: {metadata.FileName} ({FileSystemUtility.FormatFileSize(metadata.FileSize)})");

            return metadata;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to load audio file '{filePath}': {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Checks if the file format is supported.
    /// </summary>
    public bool IsSupportedFormat(string fileExtension)
    {
        if (string.IsNullOrWhiteSpace(fileExtension))
            return false;

        string ext = fileExtension.ToLower().TrimStart('.');
        return ext is "wav" or "mp3" or "flac" or "ogg" or "aiff";
    }

    /// <summary>
    /// Gets all supported audio file extensions.
    /// </summary>
    public static string[] GetSupportedExtensions()
    {
        return new[] { ".wav", ".mp3", ".flac", ".ogg", ".aiff" };
    }

    /// <summary>
    /// Extracts audio properties from the file.
    /// This is a placeholder - actual implementation would parse the audio file header.
    /// </summary>
    private async Task ExtractAudioPropertiesAsync(AudioMetadata metadata)
    {
        // In a real implementation, this would:
        // 1. Read the audio file header
        // 2. Extract sample rate, bit depth, channel count
        // 3. Calculate duration based on file size
        // 4. Extract any ID3 or other tags

        await Task.Delay(10); // Simulate work

        metadata.SampleRate ??= 44100; // Default to CD quality
        metadata.BitDepth ??= 16;
        metadata.ChannelCount ??= 2;
        metadata.Duration ??= TimeSpan.FromSeconds(metadata.FileSize / (metadata.SampleRate * metadata.ChannelCount * (metadata.BitDepth / 8)));
    }

    /// <summary>
    /// Validates an audio file before loading.
    /// </summary>
    public bool ValidateAudioFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                return false;

            var fileInfo = new FileInfo(filePath);

            if (fileInfo.Length == 0)
                return false;

            if (fileInfo.Length > MaxFileSize)
                return false;

            string extension = Path.GetExtension(filePath).ToLower();
            return IsSupportedFormat(extension);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets file information without loading the entire file.
    /// </summary>
    public FileMetadata GetFileInfo(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var fileInfo = new FileInfo(filePath);

        return new FileMetadata
        {
            FilePath = fileInfo.FullName,
            FileName = fileInfo.Name,
            FileSize = fileInfo.Length,
            Extension = fileInfo.Extension,
            CreatedDate = fileInfo.CreationTimeUtc,
            ModifiedDate = fileInfo.LastWriteTimeUtc,
            IsSupported = IsSupportedFormat(fileInfo.Extension)
        };
    }
}

/// <summary>
/// Audio metadata extracted from a file.
/// </summary>
public class FileMetadata
{
    public required string FilePath { get; init; }
    public required string FileName { get; init; }
    public required long FileSize { get; init; }
    public required string Extension { get; init; }
    public required DateTime CreatedDate { get; init; }
    public required DateTime ModifiedDate { get; init; }
    public required bool IsSupported { get; init; }
}
