using Microsoft.AspNetCore.Identity;

namespace EduPlatform.Infrastructure.Identity;

/// <summary>
/// Application-specific Identity user.
/// Added AvatarUrl for profile picture stored in Azure Blob Storage.
/// Run migration: dotnet ef migrations add AddAvatarUrl
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string    FullName               { get; set; } = string.Empty;
    public bool      IsDeleted              { get; set; }
    public DateTime  CreatedAt              { get; set; } = DateTime.UtcNow;
    public string?   RefreshToken           { get; set; }
    public DateTime? RefreshTokenExpiresAt  { get; set; }

    /// <summary>Azure Blob URL for profile picture. Null = show initials fallback.</summary>
    public string?   AvatarUrl              { get; set; }
}
