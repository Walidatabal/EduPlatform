using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;

namespace EduPlatform.Infrastructure.Services.Lms;

public class CartService : ICartService
{
    private readonly ILmsPlatformService _lms;

    public CartService(ILmsPlatformService lms) => _lms = lms;

    public Task<CartSummaryDto> GetCartAsync(string userId, CancellationToken ct = default) => _lms.GetCartAsync(userId, ct);
    public Task AddToCartAsync(string userId, int courseId, CancellationToken ct = default) => _lms.AddToCartAsync(userId, courseId, ct);
    public Task RemoveFromCartAsync(string userId, int courseId, CancellationToken ct = default) => _lms.RemoveFromCartAsync(userId, courseId, ct);
    public Task ClearCartAsync(string userId, CancellationToken ct = default) => _lms.ClearCartAsync(userId, ct);
}
