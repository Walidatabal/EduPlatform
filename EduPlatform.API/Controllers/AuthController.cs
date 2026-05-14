using EduPlatform.Application.Common.Interfaces;
using EduPlatform.Application.Features.Auth.DTOs;
using EduPlatform.Domain.Constants;
using EduPlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService, IEmailService emailService)
    {
        _userManager = userManager; _tokenService = tokenService; _emailService = emailService;
    }

    /// <summary>Register a new user (Student or PendingTeacher)</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var allowedRoles = new[] { AppRoles.Student, AppRoles.PendingTeacher };
        if (!allowedRoles.Contains(req.Role))
            return BadRequest(new { message = "Role must be Student or PendingTeacher." });

        if (await _userManager.FindByEmailAsync(req.Email) is not null)
            return BadRequest(new { message = "Email already registered." });

        var user = new ApplicationUser { UserName = req.Email, Email = req.Email, FullName = req.FullName };
        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        await _userManager.AddToRoleAsync(user, req.Role);
        await _emailService.SendWelcomeAsync(user.Email!, user.FullName);

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles);

        return CreatedAtAction(nameof(Register), BuildAuthResponse(user, token, roles));
    }

    /// <summary>Login and receive JWT access token</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, req.Password))
            return Unauthorized(new { message = "Invalid credentials." });

        if (user.IsDeleted)
            return Unauthorized(new { message = "Account has been deactivated." });

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles);
        return Ok(BuildAuthResponse(user, token, roles));
    }

    /// <summary>Get current authenticated user info</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Me()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();
        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new { user.Id, user.FullName, user.Email, Roles = roles });
    }

    private static AuthResponse BuildAuthResponse(ApplicationUser user, string token, IList<string> roles) =>
        new()
        {
            AccessToken  = token,
            RefreshToken = Guid.NewGuid().ToString(),
            ExpiresAt    = DateTime.UtcNow.AddHours(24),
            UserId       = user.Id,
            Email        = user.Email!,
            FullName     = user.FullName,
            Roles        = roles
        };
}
