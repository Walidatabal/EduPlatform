using System.ComponentModel.DataAnnotations;

namespace EduPlatform.Application.Features.Auth.DTOs;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
