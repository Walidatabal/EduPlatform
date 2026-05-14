using EduPlatform.API.Extensions;
using EduPlatform.Application.Common.Interfaces;
using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Student)]
[Produces("application/json")]
public class ProgressController : ControllerBase
{
    private readonly ILmsPlatformService _service;
    private readonly ICurrentUserService _currentUser;
    public ProgressController(ILmsPlatformService service, ICurrentUserService currentUser)
    {
        _service = service; _currentUser = currentUser;
    }

    [HttpPost("lessons/{lessonId:int}/complete")]
    public async Task<IActionResult> CompleteLesson(int lessonId, [FromBody] CompleteLessonRequest request, CancellationToken ct)
    {
        await _service.CompleteLessonAsync(_currentUser.UserId!, lessonId, request, ct);
        return NoContent();
    }

    [HttpGet("courses/{courseId:int}")]
    public async Task<IActionResult> GetCourseProgress(int courseId, CancellationToken ct) =>
        this.ApiOk(await _service.GetCourseProgressAsync(_currentUser.UserId!, courseId, ct));
}
