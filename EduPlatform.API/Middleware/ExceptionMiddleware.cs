using System.Text.Json;
using EduPlatform.Application.Common.Exceptions;
using EduPlatform.Application.Common.Models;

namespace EduPlatform.API.Middleware;

/// <summary>
/// Global exception handling middleware for the API pipeline.
///
/// Why a middleware instead of try/catch in every controller?
/// Controllers should not handle exceptions — that is cross-cutting infrastructure
/// concern. ExceptionMiddleware sits at the outermost layer of the pipeline and
/// intercepts any unhandled exception from any controller, service, or repository.
///
/// Exception → HTTP status code mapping:
/// - NotFoundException     → 404 Not Found (expected failure — logged as Information)
/// - ForbiddenException    → 403 Forbidden (expected failure — logged as Warning)
/// - ValidationException   → 422 Unprocessable Entity (business rule violation — logged as Warning)
/// - Any other Exception   → 500 Internal Server Error (unexpected — logged as Error with stack trace)
///
/// Response format:
/// Every error response follows the standard ApiResponse shape:
/// {
///   "success": false,
///   "message": "...",
///   "errors": { ... },  // only for ValidationException
///   "traceId": "00-abc123-..."
/// }
///
/// TraceId:
/// Every response includes context.TraceIdentifier for log correlation.
/// The frontend can display this to users ("please quote TraceId: 00-abc123
/// when contacting support") and support staff can find the exact log entry in Seq.
///
/// Registration in Program.cs:
/// app.UseMiddleware&lt;ExceptionMiddleware&gt;();
/// Must be registered FIRST in the pipeline — before routing, authentication,
/// and authorization — so it catches exceptions from all subsequent middleware.
/// </summary>
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
            // Pass the request down the pipeline to the next middleware/controller.
            await _next(context);
        }
        catch (Exception ex)
        {
            // Any unhandled exception lands here.
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var traceId = context.TraceIdentifier;

        // Map domain exceptions to HTTP status codes and log levels.
        // Pattern-matching switch expression keeps the mapping readable and extensible.
        var (statusCode, message, errors, logLevel) = ex switch
        {
            // Expected: a resource was looked up but does not exist.
            NotFoundException nfe
                => (StatusCodes.Status404NotFound, nfe.Message, null as object, LogLevel.Information),

            // Expected: authenticated user does not have permission.
            ForbiddenException fe
                => (StatusCodes.Status403Forbidden, fe.Message, null as object, LogLevel.Warning),

            // Expected: request failed FluentValidation rules. Errors dict contains field-level messages.
            Application.Common.Exceptions.ValidationException ve
                => (StatusCodes.Status422UnprocessableEntity, "Validation failed.", ve.Errors as object, LogLevel.Warning),

            // Unexpected: database failure, null reference, unhandled edge case, etc.
            _   => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", null as object, LogLevel.Error)
        };

        // Log with structured properties for Seq/Serilog filtering.
        // TraceId allows finding this specific request in the log viewer.
        _logger.Log(
            logLevel,
            ex,
            "Request failed. StatusCode={StatusCode}, TraceId={TraceId}, Path={Path}",
            statusCode, traceId, context.Request.Path);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        // Serialize ApiResponse using camelCase to match JavaScript conventions.
        var response = ApiResponse.Fail(message, errors, traceId);
        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
