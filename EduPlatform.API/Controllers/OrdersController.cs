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
public class OrdersController : ControllerBase
{
    private readonly ILmsPlatformService _service;
    private readonly ICurrentUserService _currentUser;
    public OrdersController(ILmsPlatformService service, ICurrentUserService currentUser)
    {
        _service = service; _currentUser = currentUser;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMine(CancellationToken ct) =>
        this.ApiOk(await _service.GetOrdersAsync(_currentUser.UserId!, ct));

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request, CancellationToken ct) =>
        this.ApiOk(await _service.CheckoutAsync(_currentUser.UserId!, request, ct), "Order created as Pending. Payment gateway confirmation is required before enrollment activation.");
}
