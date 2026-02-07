#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.Serialization;

/// <summary>
/// Factory for creating output formatters based on format type.
/// Implements the factory pattern to decouple formatter creation from usage.
/// Supports extensibility for adding new format types.
/// </summary>
public class OutputFormatterFactory
{
    private readonly Dictionary<string, Func<IOutputFormatter>> _formatters;

    /// <summary>
    /// Initializes a new instance of the output formatter factory with default formatters.
    /// </summary>
    public OutputFormatterFactory()
    {
        _formatters = new Dictionary<string, Func<IOutputFormatter>>(StringComparer.OrdinalIgnoreCase)
        {
            ["json"] = () => new JsonFormatter(prettyPrint: true),
            ["csv"] = () => new CsvFormatter(),
            ["xml"] = () => new XmlFormatter()
        };
    }

    /// <summary>
    /// Gets a formatter instance for the specified format.
    /// </summary>
    public IOutputFormatter GetFormatter(string format)
    {
        if (string.IsNullOrWhiteSpace(format))
            throw new ArgumentException("Format cannot be null or empty.", nameof(format));

        if (!_formatters.TryGetValue(format, out var factory))
            throw new NotSupportedException($"Format '{format}' is not supported. Supported formats: {string.Join(", ", _formatters.Keys)}");

        return factory();
    }

    /// <summary>
    /// Registers a custom formatter for a format type.
    /// </summary>
    public void RegisterFormatter(string format, Func<IOutputFormatter> factory)
    {
        if (string.IsNullOrWhiteSpace(format))
            throw new ArgumentException("Format cannot be null or empty.", nameof(format));

        if (factory is null)
            throw new ArgumentNullException(nameof(factory));

        _formatters[format.ToLower()] = factory;
    }

    /// <summary>
    /// Registers a custom formatter instance.
    /// </summary>
    public void RegisterFormatterInstance(string format, IOutputFormatter formatter)
    {
        if (string.IsNullOrWhiteSpace(format))
            throw new ArgumentException("Format cannot be null or empty.", nameof(format));

        if (formatter is null)
            throw new ArgumentNullException(nameof(formatter));

        _formatters[format.ToLower()] = () => formatter;
    }

    /// <summary>
    /// Unregisters a formatter.
    /// </summary>
    public bool UnregisterFormatter(string format)
    {
        if (string.IsNullOrWhiteSpace(format))
            return false;

        return _formatters.Remove(format.ToLower());
    }

    /// <summary>
    /// Checks if a format is supported.
    /// </summary>
    public bool IsFormatSupported(string format)
    {
        return !string.IsNullOrWhiteSpace(format) && _formatters.ContainsKey(format.ToLower());
    }

    /// <summary>
    /// Gets all supported format names.
    /// </summary>
    public IEnumerable<string> GetSupportedFormats()
    {
        return _formatters.Keys;
    }

    /// <summary>
    /// Gets the default formatter (JSON).
    /// </summary>
    public IOutputFormatter GetDefaultFormatter()
    {
        return GetFormatter("json");
    }
}
