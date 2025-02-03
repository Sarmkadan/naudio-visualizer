#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.Middleware;

/// <summary>
/// Encapsulates context information for the current request/operation.
/// Provides scoped storage for request-specific data that needs to be passed through the pipeline.
/// Uses an async-local context to maintain isolation between concurrent operations.
/// </summary>
public class RequestContext : IDisposable
{
    private static readonly System.Threading.AsyncLocal<RequestContext?> _currentContext =
        new();

    private readonly Dictionary<string, object?> _items;
    private readonly Guid _requestId;
    private readonly DateTime _startTime;
    private bool _disposed;

    /// <summary>
    /// Gets the current request context, or null if none is active.
    /// </summary>
    public static RequestContext? Current
    {
        get => _currentContext.Value;
        set => _currentContext.Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the request context.
    /// </summary>
    public RequestContext()
    {
        _requestId = Guid.NewGuid();
        _startTime = DateTime.UtcNow;
        _items = new Dictionary<string, object?>();
    }

    /// <summary>
    /// Gets a unique identifier for this request.
    /// </summary>
    public Guid RequestId => _requestId;

    /// <summary>
    /// Gets the time when this context was created.
    /// </summary>
    public DateTime StartTime => _startTime;

    /// <summary>
    /// Gets the elapsed time since the context was created.
    /// </summary>
    public TimeSpan Elapsed => DateTime.UtcNow - _startTime;

    /// <summary>
    /// Sets a value in the request context.
    /// </summary>
    public void Set(string key, object? value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        lock (_items)
        {
            _items[key] = value;
        }
    }

    /// <summary>
    /// Gets a value from the request context, or null if not found.
    /// </summary>
    public T? Get<T>(string key) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        lock (_items)
        {
            if (_items.TryGetValue(key, out var value))
                return value as T;
            return null;
        }
    }

    /// <summary>
    /// Gets a value with a default if not found.
    /// </summary>
    public T? GetOrDefault<T>(string key, T? defaultValue = default) where T : class
    {
        return Get<T>(key) ?? defaultValue;
    }

    /// <summary>
    /// Checks if a key exists in the context.
    /// </summary>
    public bool Contains(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        lock (_items)
        {
            return _items.ContainsKey(key);
        }
    }

    /// <summary>
    /// Removes a value from the context.
    /// </summary>
    public bool Remove(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        lock (_items)
        {
            return _items.Remove(key);
        }
    }

    /// <summary>
    /// Gets all items in the context.
    /// </summary>
    public IReadOnlyDictionary<string, object?> GetAllItems()
    {
        lock (_items)
        {
            return new Dictionary<string, object?>(_items);
        }
    }

    /// <summary>
    /// Clears all items from the context.
    /// </summary>
    public void Clear()
    {
        lock (_items)
        {
            _items.Clear();
        }
    }

    /// <summary>
    /// Creates a new context and sets it as current.
    /// </summary>
    public static RequestContext Begin()
    {
        var context = new RequestContext();
        Current = context;
        return context;
    }

    /// <summary>
    /// Disposes the current context.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        Clear();
        if (Current == this)
            Current = null;

        _disposed = true;
    }

    /// <summary>
    /// Returns a summary of the request context.
    /// </summary>
    public override string ToString()
    {
        return $"RequestContext {{ Id={_requestId}, Elapsed={Elapsed.TotalMilliseconds:F2}ms, ItemCount={_items.Count} }}";
    }
}
