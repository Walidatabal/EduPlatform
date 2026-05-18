namespace EduPlatform.Application.Common.Results;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> FromServiceResult(ServiceResult<T> result)
    {
        return new ApiResponse<T>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data
        };
    }
}