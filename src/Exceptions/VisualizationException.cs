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

    /// <summary>
    /// Initializes a new instance of the VisualizationException class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public VisualizationException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the VisualizationException class with a specified error message and the visualization type that failed.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="visualizationType">The type of visualization that failed.</param>
    public VisualizationException(string message, string visualizationType)
        : base(message)
    {
        VisualizationType = visualizationType;
    }

    /// <summary>
    /// Initializes a new instance of the VisualizationException class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public VisualizationException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the VisualizationException class with a specified error message, the visualization type that failed, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="visualizationType">The type of visualization that failed.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public VisualizationException(string message, string visualizationType, Exception innerException)
        : base(message, innerException)
    {
        VisualizationType = visualizationType;
    }
}
