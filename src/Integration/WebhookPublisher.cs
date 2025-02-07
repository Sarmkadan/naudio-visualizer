#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NAudioVisualizer.Events;
using NAudioVisualizer.Infrastructure;

namespace NAudioVisualizer.Integration;

/// <summary>
/// Publishes events as webhook calls to registered endpoints.
/// Allows external systems to react to audio visualization events.
/// </summary>
public class WebhookPublisher
{
    private readonly List<WebhookSubscription> _subscriptions;
    private readonly Logger _logger;
    private readonly HttpRequester _httpRequester;
    private readonly object _lockObject = new();

    /// <summary>
    /// Initializes a new instance of the webhook publisher.
    /// </summary>
    public WebhookPublisher(Logger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subscriptions = new List<WebhookSubscription>();
        _httpRequester = new HttpRequester("webhooks");
    }

    /// <summary>
    /// Registers a webhook endpoint for a specific event type.
    /// </summary>
    public void Subscribe<T>(string webhookUrl, string? secret = null) where T : class
    {
        if (string.IsNullOrWhiteSpace(webhookUrl))
            throw new ArgumentException("Webhook URL cannot be null or empty.", nameof(webhookUrl));

        lock (_lockObject)
        {
            var subscription = new WebhookSubscription
            {
                EventType = typeof(T),
                WebhookUrl = webhookUrl,
                Secret = secret,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _subscriptions.Add(subscription);
            _logger.Info($"Registered webhook for {typeof(T).Name}: {webhookUrl}");

            // Subscribe to the event
            EventPublisher.Subscribe<T>(async e => await PublishWebhookAsync(webhookUrl, e, secret)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Publishes an event to a webhook endpoint.
    /// </summary>
    private async Task PublishWebhookAsync<T>(string webhookUrl, T eventData, string? secret) where T : class
    {
        try
        {
            var payload = new
            {
                timestamp = DateTime.UtcNow,
                eventType = typeof(T).Name,
                data = eventData
            };

            string json = JsonSerializer.Serialize(payload);

            // Add optional signature header for webhook verification
            if (!string.IsNullOrEmpty(secret))
            {
                using var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secret));
                string signature = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(json)));
                // Note: In real implementation, add this as X-Webhook-Signature header
            }

            await _httpRequester.PostJsonAsync(webhookUrl, json).ConfigureAwait(false);
            _logger.Debug($"Webhook published successfully to {webhookUrl}");
        }
        catch (HttpRequestException ex)
        {
            _logger.Warn($"Failed to publish webhook to {webhookUrl}: {ex.Message}");
        }
    }

    /// <summary>
    /// Unsubscribes a webhook endpoint.
    /// </summary>
    public bool Unsubscribe(string webhookUrl)
    {
        if (string.IsNullOrWhiteSpace(webhookUrl))
            return false;

        lock (_lockObject)
        {
            var subscription = _subscriptions.Find(s => s.WebhookUrl == webhookUrl);
            if (subscription is not null)
            {
                subscription.IsActive = false;
                _subscriptions.Remove(subscription);
                _logger.Info($"Unregistered webhook: {webhookUrl}");
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Gets all registered webhooks.
    /// </summary>
    public IReadOnlyList<WebhookInfo> GetRegisteredWebhooks()
    {
        lock (_lockObject)
        {
            var webhooks = new List<WebhookInfo>();
            foreach (var sub in _subscriptions)
            {
                webhooks.Add(new WebhookInfo
                {
                    WebhookUrl = sub.WebhookUrl,
                    EventType = sub.EventType.Name,
                    CreatedAt = sub.CreatedAt
                });
            }

            return webhooks.AsReadOnly();
        }
    }

    /// <summary>
    /// Gets the number of registered webhooks.
    /// </summary>
    public int GetSubscriptionCount()
    {
        lock (_lockObject)
        {
            return _subscriptions.Count;
        }
    }

    /// <summary>
    /// Internal class for webhook subscription data.
    /// </summary>
    private class WebhookSubscription
    {
        public required Type EventType { get; init; }
        public required string WebhookUrl { get; init; }
        public string? Secret { get; init; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; init; }
    }
}

/// <summary>
/// Information about a registered webhook.
/// </summary>
public class WebhookInfo
{
    public required string WebhookUrl { get; init; }
    public required string EventType { get; init; }
    public DateTime CreatedAt { get; init; }
}
