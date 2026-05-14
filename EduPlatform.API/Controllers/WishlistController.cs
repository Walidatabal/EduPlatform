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
public class WishlistController : ControllerBase
{
    private readonly ILmsPlatformService _service;
    private readonly ICurrentUserService _currentUser;
    public WishlistController(ILmsPlatformService service, ICurrentUserService currentUser)
    {
        _service = service; _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken ct) =>
        this.ApiOk(await _service.GetWishlistAsync(_currentUser.UserId!, ct));

    [HttpPost("{courseId:int}")]
    public async Task<IActionResult> Add(int courseId, CancellationToken ct)
    {
        await _service.AddToWishlistAsync(_currentUser.UserId!, courseId, ct);
        return NoContent();
    }

    [HttpDelete("{courseId:int}")]
    public async Task<IActionResult> Remove(int courseId, CancellationToken ct)
    {
        await _service.RemoveFromWishlistAsync(_currentUser.UserId!, courseId, ct);
        return NoContent();
    }
}
