// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using NAudioVisualizer.Domain.Models;

namespace NAudioVisualizer.Caching;

/// <summary>
/// Specialized cache for visualization data with domain-specific operations.
/// Manages caching of waveform, spectrum, and spectrogram data.
/// </summary>
public class VisualizationCache : IDisposable
{
    private readonly CacheManager<string, WaveformData> _waveformCache;
    private readonly CacheManager<string, SpectrumData> _spectrumCache;
    private readonly CacheManager<string, SpectrogramData> _spectrogramCache;
    private readonly TimeSpan _defaultExpiration;

    /// <summary>
    /// Initializes a new instance of the visualization cache.
    /// </summary>
    public VisualizationCache(TimeSpan? defaultExpiration = null)
    {
        _defaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(30);

        // Initialize separate caches for each data type
        _waveformCache = new CacheManager<string, WaveformData>(500, _defaultExpiration);
        _spectrumCache = new CacheManager<string, SpectrumData>(500, _defaultExpiration);
        _spectrogramCache = new CacheManager<string, SpectrogramData>(100, _defaultExpiration);
    }

    /// <summary>
    /// Caches waveform data with an auto-generated key based on timestamp.
    /// </summary>
    public string CacheWaveform(WaveformData waveform)
    {
        if (waveform == null)
            throw new ArgumentNullException(nameof(waveform));

        string key = GenerateKey("waveform", waveform.Timestamp);
        _waveformCache.Set(key, waveform, _defaultExpiration);

        return key;
    }

    /// <summary>
    /// Retrieves cached waveform data by key.
    /// </summary>
    public WaveformData? GetWaveform(string key)
    {
        return _waveformCache.GetOrDefault(key);
    }

    /// <summary>
    /// Caches spectrum data with an auto-generated key.
    /// </summary>
    public string CacheSpectrum(SpectrumData spectrum)
    {
        if (spectrum == null)
            throw new ArgumentNullException(nameof(spectrum));

        string key = GenerateKey("spectrum", spectrum.Timestamp);
        _spectrumCache.Set(key, spectrum, _defaultExpiration);

        return key;
    }

    /// <summary>
    /// Retrieves cached spectrum data by key.
    /// </summary>
    public SpectrumData? GetSpectrum(string key)
    {
        return _spectrumCache.GetOrDefault(key);
    }

    /// <summary>
    /// Caches spectrogram data with an auto-generated key.
    /// </summary>
    public string CacheSpectrogram(SpectrogramData spectrogram)
    {
        if (spectrogram == null)
            throw new ArgumentNullException(nameof(spectrogram));

        string key = GenerateKey("spectrogram", spectrogram.Timestamp);
        _spectrogramCache.Set(key, spectrogram, _defaultExpiration);

        return key;
    }

    /// <summary>
    /// Retrieves cached spectrogram data by key.
    /// </summary>
    public SpectrogramData? GetSpectrogram(string key)
    {
        return _spectrogramCache.GetOrDefault(key);
    }

    /// <summary>
    /// Gets waveform cache statistics.
    /// </summary>
    public CacheStatistics GetWaveformStats()
    {
        return _waveformCache.GetStatistics();
    }

    /// <summary>
    /// Gets spectrum cache statistics.
    /// </summary>
    public CacheStatistics GetSpectrumStats()
    {
        return _spectrumCache.GetStatistics();
    }

    /// <summary>
    /// Gets spectrogram cache statistics.
    /// </summary>
    public CacheStatistics GetSpectrogramStats()
    {
        return _spectrogramCache.GetStatistics();
    }

    /// <summary>
    /// Clears all cached data.
    /// </summary>
    public void ClearAll()
    {
        _waveformCache.Clear();
        _spectrumCache.Clear();
        _spectrogramCache.Clear();
    }

    /// <summary>
    /// Removes expired entries from all caches.
    /// </summary>
    public int PruneExpired()
    {
        int count = 0;
        count += _waveformCache.RemoveExpiredEntries();
        count += _spectrumCache.RemoveExpiredEntries();
        count += _spectrogramCache.RemoveExpiredEntries();

        return count;
    }

    /// <summary>
    /// Gets the total cache size across all types.
    /// </summary>
    public int GetTotalSize()
    {
        return _waveformCache.GetSize() + _spectrumCache.GetSize() + _spectrogramCache.GetSize();
    }

    /// <summary>
    /// Generates a cache key based on data type and timestamp.
    /// </summary>
    private string GenerateKey(string prefix, double timestamp)
    {
        // Use timestamp with limited precision to group similar frames
        long roundedTimestamp = (long)(timestamp / 100) * 100;
        return $"{prefix}_{roundedTimestamp}";
    }

    /// <summary>
    /// Gets cache health report with statistics.
    /// </summary>
    public string GetHealthReport()
    {
        var waveStats = GetWaveformStats();
        var specStats = GetSpectrumStats();
        var spectroStats = GetSpectrogramStats();

        return $@"
Visualization Cache Health Report
==================================
Waveform:   {waveStats.CurrentSize}/{waveStats.MaxSize} ({waveStats.FillPercentage:F1}%)
Spectrum:   {specStats.CurrentSize}/{specStats.MaxSize} ({specStats.FillPercentage:F1}%)
Spectrogram: {spectroStats.CurrentSize}/{spectroStats.MaxSize} ({spectroStats.FillPercentage:F1}%)
Total Size: {GetTotalSize()} items
";
    }

    public void Dispose()
    {
        ClearAll();
    }
}
