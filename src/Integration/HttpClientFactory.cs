#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Net.Http;
using System.Collections.Generic;

namespace NAudioVisualizer.Integration;

/// <summary>
/// Factory for creating and managing HTTP client instances.
/// Implements singleton pattern to reuse connections and improve performance.
/// </summary>
public static class HttpClientFactory
{
    private static readonly Lazy<HttpClient> _defaultClient = new(() => CreateHttpClient());
    private static readonly Dictionary<string, HttpClient> _namedClients = new();
    private static readonly object _lockObject = new();

    /// <summary>
    /// Gets the default HTTP client instance.
    /// </summary>
    public static HttpClient GetClient()
    {
        return _defaultClient.Value;
    }

    /// <summary>
    /// Gets or creates a named HTTP client instance.
    /// </summary>
    public static HttpClient GetNamedClient(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Client name cannot be null or empty.", nameof(name));

        lock (_lockObject)
        {
            if (!_namedClients.TryGetValue(name, out var client))
            {
                client = CreateHttpClient();
                _namedClients[name] = client;
            }

            return client;
        }
    }

    /// <summary>
    /// Creates a new HTTP client with default settings.
    /// Configured for audio visualization API calls with appropriate timeouts.
    /// </summary>
    private static HttpClient CreateHttpClient()
    {
        var client = new HttpClient();

        // Set reasonable timeouts for API calls
        client.Timeout = TimeSpan.FromSeconds(30);

        // Set default headers
        client.DefaultRequestHeaders.Add("User-Agent", "NAudioVisualizer/1.0");
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        return client;
    }

    /// <summary>
    /// Creates a client configured for webhook delivery.
    /// </summary>
    public static HttpClient CreateWebhookClient()
    {
        var client = CreateHttpClient();
        client.Timeout = TimeSpan.FromSeconds(10); // Shorter timeout for webhooks

        return client;
    }

    /// <summary>
    /// Removes a named client from the cache.
    /// </summary>
    public static bool RemoveNamedClient(string name)
    {
        lock (_lockObject)
        {
            return _namedClients.Remove(name);
        }
    }

    /// <summary>
    /// Clears all named clients.
    /// </summary>
    public static void ClearNamedClients()
    {
        lock (_lockObject)
        {
            foreach (var client in _namedClients.Values)
            {
                client?.Dispose();
            }

            _namedClients.Clear();
        }
    }
}

/// <summary>
/// HTTP client wrapper for making requests with automatic error handling.
/// </summary>
public class HttpRequester
{
    private readonly HttpClient _client;

    /// <summary>
    /// Initializes a new HTTP requester with the default client.
    /// </summary>
    public HttpRequester()
    {
        _client = HttpClientFactory.GetClient();
    }

    /// <summary>
    /// Initializes a new HTTP requester with a named client.
    /// </summary>
    public HttpRequester(string clientName)
    {
        _client = HttpClientFactory.GetNamedClient(clientName);
    }

    /// <summary>
    /// Sends a GET request and returns the response content as string.
    /// </summary>
    public async System.Threading.Tasks.Task<string> GetAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be null or empty.", nameof(url));

        try
        {
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"GET request failed for URL '{url}'.", ex);
        }
    }

    /// <summary>
    /// Sends a POST request with JSON content.
    /// </summary>
    public async System.Threading.Tasks.Task<string> PostJsonAsync(string url, string jsonContent)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be null or empty.", nameof(url));

        try
        {
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"POST request failed for URL '{url}'.", ex);
        }
    }

    /// <summary>
    /// Sends a PUT request with JSON content.
    /// </summary>
    public async System.Threading.Tasks.Task<string> PutJsonAsync(string url, string jsonContent)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be null or empty.", nameof(url));

        try
        {
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(url, content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"PUT request failed for URL '{url}'.", ex);
        }
    }

    /// <summary>
    /// Sends a DELETE request.
    /// </summary>
    public async System.Threading.Tasks.Task DeleteAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be null or empty.", nameof(url));

        try
        {
            var response = await _client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"DELETE request failed for URL '{url}'.", ex);
        }
    }

    /// <summary>
    /// Checks if a URL is accessible.
    /// </summary>
    public async System.Threading.Tasks.Task<bool> IsReachableAsync(string url)
    {
        try
        {
            var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersOnly);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
