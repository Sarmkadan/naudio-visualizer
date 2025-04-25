#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =====================================================================

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NAudioVisualizer.Events;

/// <summary>
/// Provides System.Text.Json serialization extensions for EventPublisher.
/// </summary>
public static class EventPublisherJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    /// <summary>
    /// Serializes the EventPublisher static class to a JSON string.
    /// </summary>
    /// <param name="indented">Whether to format the JSON with indentation</param>
    /// <returns>A JSON string representation</returns>
    public static string ToJson(bool indented = false)
    {
        var instance = EventPublisher.Instance;

        // Get subscriber counts for all event types
        var subscriberCounts = new Dictionary<string, int>();

        subscriberCounts[nameof(AudioCaptureStartedEvent)] = instance.GetSubscriberCount<AudioCaptureStartedEvent>();
        subscriberCounts[nameof(AudioCaptureStoppedEvent)] = instance.GetSubscriberCount<AudioCaptureStoppedEvent>();
        subscriberCounts[nameof(AudioFrameCapturedEvent)] = instance.GetSubscriberCount<AudioFrameCapturedEvent>();
        subscriberCounts[nameof(WaveformGeneratedEvent)] = instance.GetSubscriberCount<WaveformGeneratedEvent>();
        subscriberCounts[nameof(SpectrumAnalyzedEvent)] = instance.GetSubscriberCount<SpectrumAnalyzedEvent>();
        subscriberCounts[nameof(SpectrogramGeneratedEvent)] = instance.GetSubscriberCount<SpectrogramGeneratedEvent>();
        subscriberCounts[nameof(VisualizationRenderStartedEvent)] = instance.GetSubscriberCount<VisualizationRenderStartedEvent>();
        subscriberCounts[nameof(VisualizationRenderCompletedEvent)] = instance.GetSubscriberCount<VisualizationRenderCompletedEvent>();
        subscriberCounts[nameof(VisualizationErrorEvent)] = instance.GetSubscriberCount<VisualizationErrorEvent>();
        subscriberCounts[nameof(AudioDeviceConnectedEvent)] = instance.GetSubscriberCount<AudioDeviceConnectedEvent>();
        subscriberCounts[nameof(AudioDeviceDisconnectedEvent)] = instance.GetSubscriberCount<AudioDeviceDisconnectedEvent>();
        subscriberCounts[nameof(VisualizationSettingsChangedEvent)] = instance.GetSubscriberCount<VisualizationSettingsChangedEvent>();
        subscriberCounts[nameof(PerformanceMetricsEvent)] = instance.GetSubscriberCount<PerformanceMetricsEvent>();
        subscriberCounts[nameof(DataExportStartedEvent)] = instance.GetSubscriberCount<DataExportStartedEvent>();
        subscriberCounts[nameof(DataExportCompletedEvent)] = instance.GetSubscriberCount<DataExportCompletedEvent>();
        subscriberCounts[nameof(ApplicationShuttingDownEvent)] = instance.GetSubscriberCount<ApplicationShuttingDownEvent>();

        var state = new EventPublisherState
        {
            SubscriberCounts = subscriberCounts,
            TotalSubscribers = subscriberCounts.Count > 0
                ? subscriberCounts.Values.Sum()
                : 0
        };

        var options = indented
            ? new JsonSerializerOptions(_jsonSerializerOptions) { WriteIndented = true }
            : _jsonSerializerOptions;

        return JsonSerializer.Serialize(state, options);
    }

    /// <summary>
    /// Deserializes a JSON string to an EventPublisher representation.
    /// </summary>
    /// <param name="json">The JSON string to deserialize. Must not be null or empty.</param>
    /// <returns>The deserialized EventPublisher representation, or null if parsing fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is null.</exception>
    public static object? FromJson(string json)
    {
        ArgumentNullException.ThrowIfNull(json);

        try
        {
            var state = JsonSerializer.Deserialize<EventPublisherState>(json, _jsonSerializerOptions);
            if (state != null)
            {
                // Rehydrate the event bus state
                EventPublisher.Reset();
                // Note: Subscribers cannot be fully restored as they are delegates
                // The Instance is already initialized by the static constructor
            }
            return EventPublisher.Instance;
        }
        catch (JsonException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to deserialize EventPublisher state: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Attempts to deserialize a JSON string to an EventPublisher representation.
    /// </summary>
    /// <param name="json">The JSON string to deserialize. Must not be null or empty.</param>
    /// <param name="value">Output parameter for the deserialized EventPublisher</param>
    /// <returns>True if deserialization succeeded, false otherwise</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is null.</exception>
    public static bool TryFromJson(string json, out object? value)
    {
        ArgumentNullException.ThrowIfNull(json);

        try
        {
            value = FromJson(json);
            return true;
        }
        catch (JsonException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to deserialize EventPublisher state: {ex.Message}");
            value = null;
            return false;
        }
    }

    /// <summary>
    /// Internal state representation for serialization.
    /// </summary>
    private sealed class EventPublisherState
    {
        public Dictionary<string, int> SubscriberCounts { get; init; } = new();
        public int TotalSubscribers { get; init; }
    }
}