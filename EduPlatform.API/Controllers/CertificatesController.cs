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
public class CertificatesController : ControllerBase
{
    private readonly ILmsPlatformService _service;
    private readonly ICurrentUserService _currentUser;
    public CertificatesController(ILmsPlatformService service, ICurrentUserService currentUser)
    {
        _service = service; _currentUser = currentUser;
    }

    [HttpGet("my")]
    [Authorize(Roles = AppRoles.Student)]
    public async Task<IActionResult> GetMine(CancellationToken ct) =>
        this.ApiOk(await _service.GetCertificatesAsync(_currentUser.UserId!, ct));

    [HttpPost("courses/{courseId:int}/issue")]
    [Authorize(Roles = AppRoles.Student)]
    public async Task<IActionResult> IssueForCompletedCourse(int courseId, CancellationToken ct) =>
        this.ApiOk(await _service.IssueCertificateAsync(_currentUser.UserId!, courseId, ct));

    [HttpPost("{id:int}/revoke")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Revoke(int id, CancellationToken ct)
    {
        await _service.RevokeCertificateAsync(id, ct);
        return NoContent();
    }
}
