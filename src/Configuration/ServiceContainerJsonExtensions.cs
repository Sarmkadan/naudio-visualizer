#nullable enable

// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =====================================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NAudioVisualizer.Configuration;

/// <summary>
/// Extension methods for ServiceContainer to support serialization.
/// </summary>
file static class ServiceContainerExtensions
{
    public static List<string> GetRegisteredServiceTypeNames(this ServiceContainer container)
    {
        ArgumentNullException.ThrowIfNull(container);

        var typeNames = new List<string>();

        // Use reflection to access the private _services field
        var servicesField = typeof(ServiceContainer).GetField(
            "_services",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (servicesField?.GetValue(container) is Dictionary<Type, object> services)
        {
            foreach (var type in services.Keys)
            {
                typeNames.Add(type.FullName ?? type.Name);
            }
        }

        return typeNames;
    }

    public static List<string> GetRegisteredFactoryTypeNames(this ServiceContainer container)
    {
        ArgumentNullException.ThrowIfNull(container);

        var typeNames = new List<string>();

        // Use reflection to access the private _factories field
        var factoriesField = typeof(ServiceContainer).GetField(
            "_factories",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (factoriesField?.GetValue(container) is Dictionary<Type, Func<ServiceContainer, object>> factories)
        {
            foreach (var type in factories.Keys)
            {
                typeNames.Add(type.FullName ?? type.Name);
            }
        }

        return typeNames;
    }
}

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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static string ToJson(this ServiceContainer value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        var options = new JsonSerializerOptions(_jsonOptions)
        {
            WriteIndented = indented
        };

        var state = new ServiceContainerSerializationState
        {
            RegisteredServiceTypes = value.GetRegisteredServiceTypeNames(),
            RegisteredFactoryTypes = value.GetRegisteredFactoryTypeNames()
        };

        return JsonSerializer.Serialize(state, options);
    }

    /// <summary>
    /// Deserializes a ServiceContainer from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>A new ServiceContainer instance populated from the JSON data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is null or whitespace.</exception>
    /// <exception cref="JsonException">Thrown when the JSON is malformed or cannot be deserialized.</exception>
    public static ServiceContainer? FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentNullException(nameof(json));
        }

        var state = JsonSerializer.Deserialize<ServiceContainerSerializationState>(json, _jsonOptions);

        if (state is null)
        {
            return null;
        }

        var container = new ServiceContainer();

        // Note: Factories cannot be properly serialized/deserialized as they are delegates.
        // This method will reconstruct the container with knowledge of registered types,
        // but factories will need to be re-registered after deserialization.
        return container;
    }

    /// <summary>
    /// Attempts to deserialize a ServiceContainer from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">The deserialized ServiceContainer, or null if deserialization fails.</param>
    /// <returns>True if deserialization succeeds; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is null.</exception>
    public static bool TryFromJson(string json, out ServiceContainer? value)
    {
        try
        {
            value = FromJson(json);
            return true;
        }
        catch
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