using EduPlatform.API.Extensions;
using EduPlatform.Application.Common.Interfaces;

using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Student)]
[Produces("application/json")]
public class CartController : ControllerBase
{
    private readonly ILmsPlatformService _service;
    private readonly ICurrentUserService _currentUser;
    public CartController(ILmsPlatformService service, ICurrentUserService currentUser)
    {
        _service = service; _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken ct) =>
        this.ApiOk(await _service.GetCartAsync(_currentUser.UserId!, ct));

    [HttpPost("{courseId:int}")]
    public async Task<IActionResult> Add(int courseId, CancellationToken ct)
    {
        await _service.AddToCartAsync(_currentUser.UserId!, courseId, ct);
        return NoContent();
    }

    [HttpDelete("{courseId:int}")]
    public async Task<IActionResult> Remove(int courseId, CancellationToken ct)
    {
        await _service.RemoveFromCartAsync(_currentUser.UserId!, courseId, ct);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Clear(CancellationToken ct)
    {
        await _service.ClearCartAsync(_currentUser.UserId!, ct);
        return NoContent();
    }
}
