using EduPlatform.API.Extensions;
using EduPlatform.Application.Common.Interfaces;
using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class LiveSessionsController : ControllerBase
{
    private readonly ILmsPlatformService _service;
    private readonly ICurrentUserService _currentUser;

    public LiveSessionsController(ILmsPlatformService service, ICurrentUserService currentUser)
    {
        _service     = service;
        _currentUser = currentUser;
    }

    // ── Session CRUD ───────────────────────────────────────────────────────────

    [HttpGet("course/{courseId:int}")]
    public async Task<IActionResult> GetCourseSessions(int courseId, CancellationToken ct) =>
        this.ApiOk(await _service.GetCourseLiveSessionsAsync(courseId, ct));

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

    // ── Status transitions ────────────────────────────────────────────────────

    [HttpPost("{id:int}/go-live")]
    [Authorize(Roles = $"{AppRoles.Teacher},{AppRoles.Admin}")]
    public async Task<IActionResult> GoLive(int id, CancellationToken ct) =>
        this.ApiOk(await _service.GoLiveAsync(_currentUser.UserId!, _currentUser.IsInRole(AppRoles.Admin), id, ct));

    [HttpPost("{id:int}/complete")]
    [Authorize(Roles = $"{AppRoles.Teacher},{AppRoles.Admin}")]
    public async Task<IActionResult> Complete(int id, CancellationToken ct) =>
        this.ApiOk(await _service.CompleteSessionAsync(_currentUser.UserId!, _currentUser.IsInRole(AppRoles.Admin), id, ct));

    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles = $"{AppRoles.Teacher},{AppRoles.Admin}")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        await _service.CancelSessionAsync(_currentUser.UserId!, _currentUser.IsInRole(AppRoles.Admin), id, ct);
        return NoContent();
    }

    // ── Student join ──────────────────────────────────────────────────────────

    [HttpGet("{id:int}/join")]
    [Authorize(Roles = AppRoles.Student)]
    public async Task<IActionResult> Join(int id, CancellationToken ct) =>
        this.ApiOk(new { meetingUrl = await _service.GetLiveSessionJoinUrlAsync(_currentUser.UserId!, id, ct) });

    // ── Attendance ────────────────────────────────────────────────────────────

    [HttpPost("{id:int}/check-in")]
    [Authorize(Roles = AppRoles.Student)]
    public async Task<IActionResult> CheckIn(int id, CancellationToken ct) =>
        this.ApiOk(await _service.CheckInAsync(_currentUser.UserId!, id, ct));

    [HttpPost("{id:int}/check-out")]
    [Authorize(Roles = AppRoles.Student)]
    public async Task<IActionResult> CheckOut(int id, CancellationToken ct) =>
        this.ApiOk(await _service.CheckOutAsync(_currentUser.UserId!, id, ct));

    [HttpGet("{id:int}/attendance")]
    [Authorize(Roles = $"{AppRoles.Teacher},{AppRoles.Admin}")]
    public async Task<IActionResult> GetAttendance(int id, CancellationToken ct) =>
        this.ApiOk(await _service.GetAttendanceSummaryAsync(_currentUser.UserId!, _currentUser.IsInRole(AppRoles.Admin), id, ct));

    [HttpPatch("attendance/{attendanceId:int}")]
    [Authorize(Roles = $"{AppRoles.Teacher},{AppRoles.Admin}")]
    public async Task<IActionResult> UpdateAttendance(int attendanceId, [FromBody] UpdateAttendanceRequest request, CancellationToken ct) =>
        this.ApiOk(await _service.UpdateAttendanceAsync(_currentUser.UserId!, _currentUser.IsInRole(AppRoles.Admin), attendanceId, request, ct));

    [HttpGet("my-attendance")]
    [Authorize(Roles = AppRoles.Student)]
    public async Task<IActionResult> MyAttendance(CancellationToken ct) =>
        this.ApiOk(await _service.GetMyAttendanceAsync(_currentUser.UserId!, ct));
}
