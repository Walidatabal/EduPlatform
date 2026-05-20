using EduPlatform.Application.Features.Lms.DTOs;

namespace EduPlatform.Application.Features.Lms.Interfaces;

public interface IWishlistService
{
    Task<IReadOnlyList<WishlistItemDto>> GetWishlistAsync(string userId, CancellationToken ct = default);
    Task AddToWishlistAsync(string userId, int courseId, CancellationToken ct = default);
    Task RemoveFromWishlistAsync(string userId, int courseId, CancellationToken ct = default);
}
