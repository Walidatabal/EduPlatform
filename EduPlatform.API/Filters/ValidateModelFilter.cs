using EduPlatform.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EduPlatform.API.Filters;

/// <summary>
/// Global MVC action filter that converts ModelState validation errors
/// into the standard ApiResponse format before the controller action runs.
///
/// Why is this needed?
/// ASP.NET Core MVC has its own automatic 400 response for invalid ModelState,
/// but it returns a ProblemDetails object, not the project's ApiResponse shape.
/// Without this filter, validation errors from the API would have a different
/// structure than success responses, making client-side error handling inconsistent.
///
/// How it integrates with FluentValidation:
/// 1. FluentValidation.AspNetCore is registered with AddFluentValidationAutoValidation().
/// 2. When a request arrives, FluentValidation runs the appropriate validator
///    (e.g. CreateCourseCommandValidator) and populates ModelState with failures.
/// 3. This filter checks ModelState BEFORE the controller action runs.
/// 4. If ModelState is invalid, it short-circuits the pipeline and returns
///    400 Bad Request with a structured ApiResponse body including the error dictionary.
/// 5. The controller action never runs — no partial execution.
///
/// Configuration in Program.cs:
/// builder.Services.Configure&lt;ApiBehaviorOptions&gt;(options =>
/// {
///     // Disable the built-in automatic 400 response so this filter handles it instead.
///     options.SuppressModelStateInvalidFilter = true;
/// });
///
/// Error response shape:
/// {
///   "success": false,
///   "message": "Validation failed.",
///   "errors": { "Title": ["Course title is required."], "Price": ["Price cannot be negative."] },
///   "traceId": "00-abc123-..."
/// }
/// </summary>
public class ValidateModelFilter : IActionFilter
{
    /// <summary>
    /// Runs before the controller action.
    /// Short-circuits with 400 if ModelState is invalid.
    /// </summary>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // If ModelState is valid, allow the action to proceed normally.
        if (context.ModelState.IsValid) return;

        // Collect all field-level error messages into a dictionary.
        // Key = property name (matches the request property).
        // Value = array of error messages for that property.
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value!.Errors
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                        ? "Invalid value."  // fallback when message is empty
                        : e.ErrorMessage)
                    .ToArray());

        // Short-circuit: set the Result to prevent the action from running.
        context.Result = new BadRequestObjectResult(
            ApiResponse.Fail(
                "Validation failed.",
                errors,
                context.HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Runs after the controller action. Nothing to do here.
    /// </summary>
    public void OnActionExecuted(ActionExecutedContext context) { }
}
