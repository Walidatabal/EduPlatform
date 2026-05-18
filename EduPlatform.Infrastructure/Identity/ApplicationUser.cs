using Microsoft.AspNetCore.Identity;

namespace EduPlatform.Infrastructure.Identity;

/// <summary>
/// Application-specific Identity user.
///
/// In addition to ASP.NET Core Identity fields, EduPlatform stores:
/// - FullName for display in MVC/API responses.
/// - IsDeleted for soft-deactivating accounts without deleting audit history.
/// - CreatedAt for audit/reporting.
/// - RefreshToken and RefreshTokenExpiresAt for API token renewal.
///
/// Important:
/// Adding refresh-token properties requires a new EF migration when applying this
/// update to an existing database.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Hashed/opaque refresh token stored server-side for API clients.
    /// Null means the user has no active refresh session.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// UTC expiry date for the refresh token.
    /// </summary>
    public DateTime? RefreshTokenExpiresAt { get; set; }
}
