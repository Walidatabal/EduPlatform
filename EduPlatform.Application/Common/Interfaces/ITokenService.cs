namespace EduPlatform.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(string userId, string email, IList<string> roles);
    string GenerateRefreshToken();
}
