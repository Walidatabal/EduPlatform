namespace EduPlatform.Application.Common.Models;

/// <summary>
/// Generic result wrapper for service operations that return data.
///
/// Why use Result&lt;T&gt; instead of throwing exceptions for expected failures?
/// Exceptions should be reserved for truly exceptional, unexpected conditions.
/// When a course is not found or a user has wrong credentials, that is an
/// expected failure — not an exception. Using Result&lt;T&gt; makes the failure
/// path explicit in the method signature and forces callers to handle it.
///
/// Benefits:
/// - Controller code is clear: if (!result.IsSuccess) return NotFound().
/// - No try/catch clutter in controllers for expected failures.
/// - Easier to unit test — check result.IsSuccess instead of catching exceptions.
/// - ExceptionMiddleware still catches any truly unexpected exceptions.
///
/// Factory methods:
/// - Result&lt;T&gt;.Success(data)         → 200 with data
/// - Result&lt;T&gt;.Success(data, 201)     → 201 Created with data
/// - Result&lt;T&gt;.Failure("message")     → 400 Bad Request
/// - Result&lt;T&gt;.NotFound("message")    → 404 Not Found
/// - Result&lt;T&gt;.Forbidden("message")   → 403 Forbidden
///
/// Controller usage example:
/// <code>
/// var result = await courseService.GetAsync(id);
/// if (!result.IsSuccess)
///     return result.StatusCode == 404 ? NotFound() : BadRequest(result.Error);
/// return Ok(result.Data);
/// </code>
/// </summary>
/// <typeparam name="T">The data type returned on success.</typeparam>
public class Result<T>
{
    /// <summary>True if the operation completed successfully. False on any failure.</summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// The returned data on success.
    /// Null on failure — always check <see cref="IsSuccess"/> before accessing Data.
    /// </summary>
    public T? Data { get; private set; }

    /// <summary>
    /// Human-readable error message describing the failure.
    /// Null on success.
    /// Shown to the client in ApiResponse.Message or ValidationSummary.
    /// </summary>
    public string? Error { get; private set; }

    /// <summary>
    /// HTTP status code equivalent of this result.
    /// Used by controllers to return the correct HTTP response.
    /// </summary>
    public int StatusCode { get; private set; }

    // Private constructor — callers use the factory methods below.
    private Result(bool success, T? data, string? error, int statusCode)
    {
        IsSuccess = success;
        Data = data;
        Error = error;
        StatusCode = statusCode;
    }

    /// <summary>Creates a successful result with the given data.</summary>
    /// <param name="data">The data to return. Required.</param>
    /// <param name="statusCode">HTTP status code. Default 200. Use 201 for Created.</param>
    public static Result<T> Success(T data, int statusCode = 200)
        => new(true, data, null, statusCode);

    /// <summary>Creates a failure result with the given error message.</summary>
    /// <param name="error">Description of what went wrong. Shown to the client.</param>
    /// <param name="statusCode">HTTP status code. Default 400 Bad Request.</param>
    public static Result<T> Failure(string error, int statusCode = 400)
        => new(false, default, error, statusCode);

    /// <summary>Creates a 404 Not Found result.</summary>
    /// <param name="error">What was not found. Default: "Resource not found".</param>
    public static Result<T> NotFound(string error = "Resource not found")
        => new(false, default, error, 404);

    /// <summary>Creates a 403 Forbidden result.</summary>
    /// <param name="error">Why access was denied. Default: "Access denied".</param>
    public static Result<T> Forbidden(string error = "Access denied")
        => new(false, default, error, 403);
}

/// <summary>
/// Non-generic result wrapper for operations that perform an action but return no data.
/// Example: DeleteAsync, MarkAsReadAsync, ApproveTeacher.
///
/// Same factory pattern as Result&lt;T&gt; but without a data payload.
/// </summary>
public class Result
{
    /// <summary>True if the operation completed successfully.</summary>
    public bool IsSuccess { get; private set; }

    /// <summary>Error message on failure. Null on success.</summary>
    public string? Error { get; private set; }

    /// <summary>HTTP status code equivalent.</summary>
    public int StatusCode { get; private set; }

    private Result(bool success, string? error, int statusCode)
    {
        IsSuccess = success;
        Error = error;
        StatusCode = statusCode;
    }

    /// <summary>Creates a successful no-data result.</summary>
    public static Result Success(int statusCode = 200) => new(true, null, statusCode);

    /// <summary>Creates a failure result.</summary>
    public static Result Failure(string error, int statusCode = 400) => new(false, error, statusCode);

    /// <summary>Creates a 404 Not Found result.</summary>
    public static Result NotFound(string error = "Resource not found") => new(false, error, 404);
}
