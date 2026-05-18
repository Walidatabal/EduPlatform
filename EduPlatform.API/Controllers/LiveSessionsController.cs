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
[Authorize]
[Produces("application/json")]
public class LiveSessionsController : ControllerBase
{
    private readonly ILmsPlatformService _service;
    private readonly ICurrentUserService _currentUser;
    public LiveSessionsController(ILmsPlatformService service, ICurrentUserService currentUser)
    {
        _service = service; _currentUser = currentUser;
    }

    [HttpGet("course/{courseId:int}")]
    public async Task<IActionResult> GetCourseSessions(int courseId, CancellationToken ct) =>
        this.ApiOk(await _service.GetCourseLiveSessionsAsync(courseId, ct));

    [HttpGet("{id:int}/join")]
    [Authorize(Roles = AppRoles.Student)]
    public async Task<IActionResult> Join(int id, CancellationToken ct) =>
        this.ApiOk(new { meetingUrl = await _service.GetLiveSessionJoinUrlAsync(_currentUser.UserId!, id, ct) });

    [HttpPost]
    [Authorize(Roles = AppRoles.Teacher)]
    public async Task<IActionResult> Create([FromBody] UpsertLiveSessionRequest request, CancellationToken ct) =>
        this.ApiOk(await _service.CreateLiveSessionAsync(_currentUser.UserId!, request, ct));

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{AppRoles.Teacher},{AppRoles.Admin}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpsertLiveSessionRequest request, CancellationToken ct)
    {
        await _service.UpdateLiveSessionAsync(_currentUser.UserId!, _currentUser.IsInRole(AppRoles.Admin), id, request, ct);
        return NoContent();
    }
}
