using EduPlatform.Application.Common.Interfaces;
using EduPlatform.Application.Features.Auth.DTOs;
using EduPlatform.Domain.Constants;
using EduPlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace EduPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IEmailService emailService,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _emailService = emailService;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    /// <summary>Register a new user (Student or PendingTeacher).</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var allowedRoles = new[] { AppRoles.Student, AppRoles.PendingTeacher };
        if (!allowedRoles.Contains(req.Role))
            return BadRequest(new { message = "Role must be Student or PendingTeacher." });

        if (!await _roleManager.RoleExistsAsync(req.Role))
            return BadRequest(new { message = $"Role '{req.Role}' does not exist. Please run the role seeder first." });

        if (await _userManager.FindByEmailAsync(req.Email) is not null)
            return BadRequest(new { message = "Email already registered." });

        var user = new ApplicationUser
        {
            UserName = req.Email.Trim(),
            Email = req.Email.Trim(),
            FullName = req.FullName.Trim(),
            EmailConfirmed = true, // Development/demo default. In production, use the email-confirmation workflow.
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        var roleResult = await _userManager.AddToRoleAsync(user, req.Role);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return BadRequest(new { errors = roleResult.Errors.Select(e => e.Description) });
        }

        await _emailService.SendWelcomeAsync(user.Email!, user.FullName);

        var roles = await _userManager.GetRolesAsync(user);
        var response = await BuildAndPersistAuthResponseAsync(user, roles);

        return CreatedAtAction(nameof(Register), response);
    }

    /// <summary>Login and receive JWT access token + persisted refresh token.</summary>
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

        if (await _userManager.IsLockedOutAsync(user))
            return Unauthorized(new { message = "Account is locked. Please try again later or contact an administrator." });

        var roles = await _userManager.GetRolesAsync(user);
        var response = await BuildAndPersistAuthResponseAsync(user, roles);

        return Ok(response);
    }

    /// <summary>Refresh the JWT access token using a valid refresh token.</summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest req)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.RefreshToken == req.RefreshToken);
        if (user is null || user.RefreshTokenExpiresAt is null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
            return Unauthorized(new { message = "Invalid or expired refresh token." });

        if (user.IsDeleted || await _userManager.IsLockedOutAsync(user))
            return Unauthorized(new { message = "Account is not active." });

        var roles = await _userManager.GetRolesAsync(user);
        var response = await BuildAndPersistAuthResponseAsync(user, roles);

        return Ok(response);
    }

    /// <summary>Revoke the current user's refresh token.</summary>
    [HttpPost("revoke")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Revoke()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    /// <summary>Request a password reset token. In development, the token is logged by the EmailService stub.</summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(200)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);

        // Always return OK to avoid revealing whether an email exists.
        if (user is null || user.IsDeleted)
            return Ok(new { message = "If the email exists, a reset link has been sent." });

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var resetUrl = $"/Account/ResetPassword?email={Uri.EscapeDataString(user.Email!)}&token={encodedToken}";

        await _emailService.SendAsync(
            user.Email!,
            "EduPlatform password reset",
            $"<p>Use this password reset token/link:</p><p><code>{encodedToken}</code></p><p>{resetUrl}</p>");

        return Ok(new { message = "If the email exists, a reset link has been sent." });
    }

    /// <summary>Reset password using the token created by forgot-password.</summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null || user.IsDeleted)
            return BadRequest(new { message = "Invalid password reset request." });

        string token;
        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(req.Token));
        }
        catch
        {
            return BadRequest(new { message = "Invalid password reset token." });
        }

        var result = await _userManager.ResetPasswordAsync(user, token, req.NewPassword);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        await _userManager.SetLockoutEndDateAsync(user, null);
        await _userManager.ResetAccessFailedCountAsync(user);

        return Ok(new { message = "Password reset successfully." });
    }

    /// <summary>Get current authenticated user info.</summary>
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

    private async Task<AuthResponse> BuildAndPersistAuthResponseAsync(ApplicationUser user, IList<string> roles)
    {
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshDays = int.TryParse(_configuration["Jwt:RefreshTokenDays"], out var days) ? days : 7;
        var expiresAt = DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:ExpiryHours"] ?? "24"));

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshDays);
        await _userManager.UpdateAsync(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles
        };
    }
}
