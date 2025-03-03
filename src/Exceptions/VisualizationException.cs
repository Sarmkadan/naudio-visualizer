#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;

namespace NAudioVisualizer.Exceptions;

/// <summary>
/// Thrown when an error occurs during visualization generation or rendering.
/// </summary>
public class VisualizationException : Exception
{
    /// <summary>
    /// Type of visualization that failed.
    /// </summary>
    public string? VisualizationType { get; set; }

    public VisualizationException(string message) : base(message) { }

    public VisualizationException(string message, string visualizationType)
        : base(message)
    {
        VisualizationType = visualizationType;
    }

    public VisualizationException(string message, Exception innerException)
        : base(message, innerException) { }

    public VisualizationException(string message, string visualizationType, Exception innerException)
        : base(message, innerException)
    {
        VisualizationType = visualizationType;
    }
}
