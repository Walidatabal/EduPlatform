using EduPlatform.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.API.Extensions;

public static class ControllerResponseExtensions
{
    public static IActionResult ApiOk<T>(this ControllerBase controller, T data, string message = "Success") =>
        controller.Ok(ApiResponse<T>.Ok(data, message, controller.HttpContext.TraceIdentifier));

    public static IActionResult ApiCreated<T>(this ControllerBase controller, string actionName, object routeValues, T data, string message = "Created") =>
        controller.CreatedAtAction(actionName, routeValues, ApiResponse<T>.Ok(data, message, controller.HttpContext.TraceIdentifier));

    public static IActionResult ApiNoContent(this ControllerBase controller) => controller.NoContent();
}
