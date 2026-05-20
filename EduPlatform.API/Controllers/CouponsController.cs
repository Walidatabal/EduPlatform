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
[Produces("application/json")]
public class CouponsController : ControllerBase
{
    private readonly ILmsPlatformService _service;
    public CouponsController(ILmsPlatformService service) => _service = service;

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] UpsertCouponRequest request, CancellationToken ct) =>
        this.ApiCreated(nameof(Validate), new { code = request.Code }, await _service.CreateCouponAsync(request, ct));

    [HttpGet("validate/{code}")]
    public async Task<IActionResult> Validate(string code, [FromQuery] decimal subtotal, CancellationToken ct) =>
        this.ApiOk(await _service.ValidateCouponAsync(code, subtotal, ct));
}
