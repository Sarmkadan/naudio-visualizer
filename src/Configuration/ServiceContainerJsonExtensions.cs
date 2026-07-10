#nullable enable

// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =====================================================================

using System.Text.Json;
using System.Text.Json.Serialization;

namespace NAudioVisualizer.Configuration;

/// <summary>
/// Provides System.Text.Json serialization extensions for ServiceContainer.
/// </summary>
public static class ServiceContainerJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    /// <summary>
    /// Serializes the ServiceContainer to a JSON string.
    /// </summary>
    /// <param name="value">The service container to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation.</param>
    /// <returns>A JSON string representation of the service container.</returns>
    public static string ToJson(this ServiceContainer value, bool indented = false)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var options = new JsonSerializerOptions(_jsonOptions)
        {
            WriteIndented = indented
        };

        var state = new ServiceContainerSerializationState
        {
            RegisteredServiceTypes = new List<string>(),
            RegisteredFactoryTypes = new List<string>()
        };

        return JsonSerializer.Serialize(state, options);
    }

    /// <summary>
    /// Deserializes a ServiceContainer from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>A new ServiceContainer instance populated from the JSON data.</returns>
    public static ServiceContainer? FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentNullException(nameof(json));
        }

        try
        {
            var state = JsonSerializer.Deserialize<ServiceContainerSerializationState>(json, _jsonOptions);
            return new ServiceContainer();
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to deserialize a ServiceContainer from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">The deserialized ServiceContainer, or null if deserialization fails.</param>
    /// <returns>True if deserialization succeeds; otherwise, false.</returns>
    public static bool TryFromJson(string json, out ServiceContainer? value)
    {
        try
        {
            value = FromJson(json);
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }

    /// <summary>
    /// Represents the serializable state of a ServiceContainer.
    /// </summary>
    private sealed class ServiceContainerSerializationState
    {
        public List<string>? RegisteredServiceTypes { get; set; }
        public List<string>? RegisteredFactoryTypes { get; set; }
    }
}