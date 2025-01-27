// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.API;

/// <summary>
/// Standard API response wrapper for consistent response formatting.
/// Provides structured responses for success, error, and paginated results.
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the request was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// The response data.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Error message if the request failed.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Error code for programmatic error handling.
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Timestamp of the response.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Request ID for tracing purposes.
    /// </summary>
    public string? RequestId { get; init; }

    /// <summary>
    /// Creates a successful response.
    /// </summary>
    public static ApiResponse<T> CreateSuccess(T data, string? requestId = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            RequestId = requestId
        };
    }

    /// <summary>
    /// Creates an error response.
    /// </summary>
    public static ApiResponse<T> CreateError(string error, string? errorCode = null, string? requestId = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = error,
            ErrorCode = errorCode,
            RequestId = requestId
        };
    }

    /// <summary>
    /// Creates an error response from an exception.
    /// </summary>
    public static ApiResponse<T> CreateErrorFromException(Exception ex, string? requestId = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = ex.Message,
            ErrorCode = ex.GetType().Name,
            RequestId = requestId
        };
    }
}

/// <summary>
/// API response for paginated results.
/// </summary>
public class PaginatedApiResponse<T>
{
    /// <summary>
    /// Indicates if the request was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// The paginated data.
    /// </summary>
    public IEnumerable<T>? Data { get; init; }

    /// <summary>
    /// Pagination information.
    /// </summary>
    public PaginationInfo? Pagination { get; init; }

    /// <summary>
    /// Error message if failed.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Timestamp of response.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Request ID for tracing.
    /// </summary>
    public string? RequestId { get; init; }

    /// <summary>
    /// Creates a successful paginated response.
    /// </summary>
    public static PaginatedApiResponse<T> CreateSuccess(
        IEnumerable<T> data,
        int pageNumber,
        int pageSize,
        int totalCount,
        string? requestId = null)
    {
        return new PaginatedApiResponse<T>
        {
            Success = true,
            Data = data,
            Pagination = new PaginationInfo
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            },
            RequestId = requestId
        };
    }

    /// <summary>
    /// Creates an error response.
    /// </summary>
    public static PaginatedApiResponse<T> CreateError(string error, string? requestId = null)
    {
        return new PaginatedApiResponse<T>
        {
            Success = false,
            Error = error,
            RequestId = requestId
        };
    }
}

/// <summary>
/// Pagination information for paginated responses.
/// </summary>
public class PaginationInfo
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }

    /// <summary>
    /// Checks if there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Checks if there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
}

/// <summary>
/// API request model for creating/updating visualization settings.
/// </summary>
public class UpdateVisualizationRequest
{
    public int? VisualizationType { get; init; }
    public int? SampleRate { get; init; }
    public int? FftSize { get; init; }
    public float? Brightness { get; init; }
    public float? Contrast { get; init; }
    public Dictionary<string, object>? AdditionalSettings { get; init; }
}

/// <summary>
/// API request model for export operations.
/// </summary>
public class ExportRequest
{
    public required string Format { get; init; }
    public string? FilePath { get; init; }
    public bool? Compress { get; init; }
    public bool? IncludeMetadata { get; init; }
}

/// <summary>
/// API request model for audio capture configuration.
/// </summary>
public class AudioCaptureRequest
{
    public int? DeviceId { get; init; }
    public int? SampleRate { get; init; }
    public int? ChannelCount { get; init; }
    public float? Duration { get; init; }
}
