using EduPlatform.Application.Common.Interfaces;
using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.API.Controllers;

[ApiController]
[Route("api/sections")]
[Authorize(Roles = $"{AppRoles.Teacher},{AppRoles.Admin}")]
[Produces("application/json")]
public class SectionsController : ControllerBase
{
    private readonly ILmsPlatformService _service;
    private readonly ICurrentUserService _currentUser;

    public SectionsController(ILmsPlatformService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpPut("{sectionId:int}")]
    public async Task<IActionResult> Update(int sectionId, [FromBody] UpsertSectionRequest request, CancellationToken ct)
    {
        await _service.UpdateSectionAsync(_currentUser.UserId!, _currentUser.IsInRole(AppRoles.Admin), sectionId, request, ct);
        return NoContent();
    }

    [HttpDelete("{sectionId:int}")]
    public async Task<IActionResult> Delete(int sectionId, CancellationToken ct)
    {
        await _service.DeleteSectionAsync(_currentUser.UserId!, _currentUser.IsInRole(AppRoles.Admin), sectionId, ct);
        return NoContent();
    }
}
