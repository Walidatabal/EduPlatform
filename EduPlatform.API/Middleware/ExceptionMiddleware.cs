using System.Text.Json;
using EduPlatform.Application.Common.Exceptions;
using EduPlatform.Application.Common.Models;

namespace EduPlatform.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var traceId = context.TraceIdentifier;

        var (statusCode, message, errors, logLevel) = ex switch
        {
            NotFoundException nfe => (StatusCodes.Status404NotFound, nfe.Message, null as object, LogLevel.Information),
            ForbiddenException fe => (StatusCodes.Status403Forbidden, fe.Message, null as object, LogLevel.Warning),
            Application.Common.Exceptions.ValidationException ve => (StatusCodes.Status422UnprocessableEntity, "Validation failed.", ve.Errors as object, LogLevel.Warning),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", null as object, LogLevel.Error)
        };

        _logger.Log(logLevel, ex, "Request failed. StatusCode={StatusCode}, TraceId={TraceId}, Path={Path}", statusCode, traceId, context.Request.Path);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = ApiResponse.Fail(message, errors, traceId);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
