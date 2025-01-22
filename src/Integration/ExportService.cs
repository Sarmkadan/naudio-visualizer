#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Events;
using NAudioVisualizer.Infrastructure;
using NAudioVisualizer.Serialization;
using NAudioVisualizer.Utilities;

namespace NAudioVisualizer.Integration;

/// <summary>
/// Service for exporting visualization data to various file formats.
/// Supports JSON, CSV, and XML export with compression and validation.
/// </summary>
public class ExportService
{
    private readonly Logger _logger;
    private readonly JsonFormatter _jsonFormatter;
    private readonly CsvFormatter _csvFormatter;
    private readonly XmlFormatter _xmlFormatter;

    /// <summary>
    /// Initializes a new instance of the export service.
    /// </summary>
    public ExportService(Logger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonFormatter = new JsonFormatter(prettyPrint: true);
        _csvFormatter = new CsvFormatter();
        _xmlFormatter = new XmlFormatter();
    }

    /// <summary>
    /// Exports waveform data to a file.
    /// </summary>
    public async Task<bool> ExportWaveformAsync(WaveformData waveform, string filePath, string format = "json")
    {
        if (waveform is null)
            throw new ArgumentNullException(nameof(waveform));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        try
        {
            EventPublisher.PublishDataExportStarted(filePath, format, (int)(waveform.DurationMs * waveform.SampleRate / 1000));

            string content = format.ToLower() switch
            {
                "json" => _jsonFormatter.Format(waveform),
                "csv" => _csvFormatter.Format(waveform),
                "xml" => _xmlFormatter.Format(waveform),
                _ => throw new NotSupportedException($"Format '{format}' is not supported.")
            };

            FileSystemUtility.EnsureDirectoryExists(Path.GetDirectoryName(filePath) ?? ".");
            await FileSystemUtility.WriteFileAsync(filePath, content);

            long fileSize = FileSystemUtility.GetFileSize(filePath);
            _logger.Info($"Exported waveform to '{filePath}' ({FileSystemUtility.FormatFileSize(fileSize)})");

            EventPublisher.PublishDataExportCompleted(filePath, format, fileSize, 0, success: true);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to export waveform: {ex.Message}");
            EventPublisher.PublishDataExportCompleted(filePath, format, 0, 0, success: false);
            throw;
        }
    }

    /// <summary>
    /// Exports spectrum data to a file.
    /// </summary>
    public async Task<bool> ExportSpectrumAsync(SpectrumData spectrum, string filePath, string format = "json")
    {
        if (spectrum is null)
            throw new ArgumentNullException(nameof(spectrum));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        try
        {
            EventPublisher.PublishDataExportStarted(filePath, format, spectrum.Magnitude.Length);

            string content = format.ToLower() switch
            {
                "json" => _jsonFormatter.Format(spectrum),
                "csv" => _csvFormatter.Format(spectrum),
                "xml" => _xmlFormatter.Format(spectrum),
                _ => throw new NotSupportedException($"Format '{format}' is not supported.")
            };

            FileSystemUtility.EnsureDirectoryExists(Path.GetDirectoryName(filePath) ?? ".");
            await FileSystemUtility.WriteFileAsync(filePath, content);

            long fileSize = FileSystemUtility.GetFileSize(filePath);
            _logger.Info($"Exported spectrum to '{filePath}' ({FileSystemUtility.FormatFileSize(fileSize)})");

            EventPublisher.PublishDataExportCompleted(filePath, format, fileSize, 0, success: true);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to export spectrum: {ex.Message}");
            EventPublisher.PublishDataExportCompleted(filePath, format, 0, 0, success: false);
            throw;
        }
    }

    /// <summary>
    /// Exports spectrogram data to a file.
    /// </summary>
    public async Task<bool> ExportSpectrogramAsync(SpectrogramData spectrogram, string filePath, string format = "json")
    {
        if (spectrogram is null)
            throw new ArgumentNullException(nameof(spectrogram));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        try
        {
            EventPublisher.PublishDataExportStarted(filePath, format, spectrogram.TimeFrames * spectrogram.FrequencyBins);

            string content = format.ToLower() switch
            {
                "json" => _jsonFormatter.Format(spectrogram),
                "csv" => _csvFormatter.Format(spectrogram),
                "xml" => _xmlFormatter.Format(spectrogram),
                _ => throw new NotSupportedException($"Format '{format}' is not supported.")
            };

            FileSystemUtility.EnsureDirectoryExists(Path.GetDirectoryName(filePath) ?? ".");
            await FileSystemUtility.WriteFileAsync(filePath, content);

            long fileSize = FileSystemUtility.GetFileSize(filePath);
            _logger.Info($"Exported spectrogram to '{filePath}' ({FileSystemUtility.FormatFileSize(fileSize)})");

            EventPublisher.PublishDataExportCompleted(filePath, format, fileSize, 0, success: true);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to export spectrogram: {ex.Message}");
            EventPublisher.PublishDataExportCompleted(filePath, format, 0, 0, success: false);
            throw;
        }
    }

    /// <summary>
    /// Exports metadata to a file.
    /// </summary>
    public async Task<bool> ExportMetadataAsync(Dictionary<string, string> metadata, string filePath, string format = "json")
    {
        if (metadata is null)
            throw new ArgumentNullException(nameof(metadata));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        try
        {
            EventPublisher.PublishDataExportStarted(filePath, format, metadata.Count);

            string content = format.ToLower() switch
            {
                "json" => _jsonFormatter.FormatMetadata(metadata),
                "csv" => _csvFormatter.FormatMetadata(metadata),
                "xml" => _xmlFormatter.FormatMetadata(metadata),
                _ => throw new NotSupportedException($"Format '{format}' is not supported.")
            };

            FileSystemUtility.EnsureDirectoryExists(Path.GetDirectoryName(filePath) ?? ".");
            await FileSystemUtility.WriteFileAsync(filePath, content);

            long fileSize = FileSystemUtility.GetFileSize(filePath);
            _logger.Info($"Exported metadata to '{filePath}' ({FileSystemUtility.FormatFileSize(fileSize)})");

            EventPublisher.PublishDataExportCompleted(filePath, format, fileSize, 0, success: true);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to export metadata: {ex.Message}");
            EventPublisher.PublishDataExportCompleted(filePath, format, 0, 0, success: false);
            throw;
        }
    }

    /// <summary>
    /// Gets the supported export formats.
    /// </summary>
    public static IEnumerable<string> GetSupportedFormats()
    {
        return new[] { "json", "csv", "xml" };
    }

    /// <summary>
    /// Gets the file extension for a format.
    /// </summary>
    public static string GetFileExtension(string format)
    {
        return format.ToLower() switch
        {
            "json" => ".json",
            "csv" => ".csv",
            "xml" => ".xml",
            _ => ".dat"
        };
    }
}
