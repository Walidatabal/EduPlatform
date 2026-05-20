namespace EduPlatform.Application.Common.Results;

/// <summary>
/// Generic wrapper used between Application/Infrastructure layers.
/// 
/// WHY?
/// Instead of returning raw data directly from services,
/// we return a standardized object that contains:
/// 
/// - Success status
/// - Human readable message
/// - Actual returned data
/// 
/// BENEFITS:
/// - Clean Architecture friendly
/// - Standardized service communication
/// - Easier logging and debugging
/// - Better API consistency
/// - Enterprise-level pattern
/// </summary>
public class ServiceResult<T>
{
    /// <summary>
    /// Indicates whether the operation succeeded or failed.
    /// </summary>
    public bool Success { get; private set; }

    /// <summary>
    /// Human readable message describing the operation result.
    /// Example:
    /// "Category created successfully"
    /// </summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// Actual returned data from the service.
    /// </summary>
    public T? Data { get; private set; }

    /// <summary>
    /// Private constructor to force developers
    /// to use factory methods (Ok / Fail).
    /// </summary>
    private ServiceResult() { }

    /// <summary>
    /// Creates a successful service result.
    /// </summary>
    public static ServiceResult<T> Ok(
        T data,
        string message = "Operation completed successfully")
    {
        return new ServiceResult<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// Creates a failed service result.
    /// </summary>
    public static ServiceResult<T> Fail(string message)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Message = message
        };
    }
}