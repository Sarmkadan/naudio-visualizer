#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Base class for all visualization data types (waveform, spectrum, spectrogram).
/// </summary>
public abstract class VisualizationData
{
    /// <summary>
    /// Unique identifier for this visualization data.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Type of visualization (Waveform, Spectrum, Spectrogram).
    /// </summary>
    public VisualizationType VisualizationType { get; protected set; }

    /// <summary>
    /// Timestamp when this visualization was generated.
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The audio frame from which this visualization was derived.
    /// </summary>
    public AudioFrame? SourceFrame { get; set; }

    /// <summary>
    /// Number of data points in this visualization.
    /// </summary>
    public int DataPointCount { get; protected set; }

    /// <summary>
    /// Minimum value in the visualization data.
    /// </summary>
    public float MinValue { get; protected set; }

    /// <summary>
    /// Maximum value in the visualization data.
    /// </summary>
    public float MaxValue { get; protected set; }

    /// <summary>
    /// Whether this visualization data has been normalized to 0-1 range.
    /// </summary>
    public bool IsNormalized { get; set; }

    /// <summary>
    /// Gets the visualization data as a float array.
    /// </summary>
    public abstract float[] GetData();

    /// <summary>
    /// Normalizes the visualization data to 0-1 range.
    /// </summary>
    public abstract void Normalize();

    /// <summary>
    /// Validates that the visualization data is consistent.
    /// </summary>
    public abstract bool IsValid();
}

/// <summary>
/// Enumeration of supported visualization types.
/// </summary>
public enum VisualizationType
{
    Waveform = 0,
    Spectrum = 1,
    Spectrogram = 2
}
