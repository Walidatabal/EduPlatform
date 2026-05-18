using EduPlatform.API.Extensions;
using EduPlatform.Application.Common.Interfaces;
using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.API.Controllers;

[ApiController]
[Route("api")]
[Authorize(Roles = $"{AppRoles.Teacher},{AppRoles.Admin}")]
[Produces("application/json")]
public class LessonsController : ControllerBase
{
    private readonly ILmsPlatformService _service;
    private readonly ICurrentUserService _currentUser;

    public LessonsController(ILmsPlatformService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpPost("sections/{sectionId:int}/lessons")]
    public async Task<IActionResult> Create(int sectionId, [FromBody] UpsertLessonRequest request, CancellationToken ct) =>
        this.ApiOk(await _service.CreateLessonAsync(_currentUser.UserId!, _currentUser.IsInRole(AppRoles.Admin), sectionId, request, ct));

    [HttpPut("lessons/{lessonId:int}")]
    public async Task<IActionResult> Update(int lessonId, [FromBody] UpsertLessonRequest request, CancellationToken ct)
    {
        await _service.UpdateLessonAsync(_currentUser.UserId!, _currentUser.IsInRole(AppRoles.Admin), lessonId, request, ct);
        return NoContent();
    }

    [HttpDelete("lessons/{lessonId:int}")]
    public async Task<IActionResult> Delete(int lessonId, CancellationToken ct)
    {
        await _service.DeleteLessonAsync(_currentUser.UserId!, _currentUser.IsInRole(AppRoles.Admin), lessonId, ct);
        return NoContent();
    }

    [HttpPost("sections/{sectionId:int}/lessons/reorder")]
    public async Task<IActionResult> Reorder(int sectionId, [FromBody] ReorderLessonsRequest request, CancellationToken ct)
    {
        await _service.ReorderLessonsAsync(_currentUser.UserId!, _currentUser.IsInRole(AppRoles.Admin), sectionId, request, ct);
        return NoContent();
    }
}
