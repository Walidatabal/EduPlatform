using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;

namespace EduPlatform.Infrastructure.Services.Lms;

public class WishlistService : IWishlistService
{
    private readonly ILmsPlatformService _lms;

    public WishlistService(ILmsPlatformService lms) => _lms = lms;

    public Task<IReadOnlyList<WishlistItemDto>> GetWishlistAsync(string userId, CancellationToken ct = default) => _lms.GetWishlistAsync(userId, ct);
    public Task AddToWishlistAsync(string userId, int courseId, CancellationToken ct = default) => _lms.AddToWishlistAsync(userId, courseId, ct);
    public Task RemoveFromWishlistAsync(string userId, int courseId, CancellationToken ct = default) => _lms.RemoveFromWishlistAsync(userId, courseId, ct);
}
