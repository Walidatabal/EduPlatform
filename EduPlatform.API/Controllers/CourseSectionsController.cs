using EduPlatform.API.Extensions;
using EduPlatform.Application.Common.Interfaces;
using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.API.Controllers;

[ApiController]
[Route("api/courses/{courseId:int}/sections")]
[Authorize(Roles = $"{AppRoles.Teacher},{AppRoles.Admin}")]
[Produces("application/json")]
public class CourseSectionsController : ControllerBase
{
    private readonly ILmsPlatformService _service;
    private readonly ICurrentUserService _currentUser;

    public CourseSectionsController(ILmsPlatformService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> Create(int courseId, [FromBody] UpsertSectionRequest request, CancellationToken ct) =>
        this.ApiOk(await _service.CreateSectionAsync(_currentUser.UserId!, _currentUser.IsInRole(AppRoles.Admin), courseId, request, ct));
}
