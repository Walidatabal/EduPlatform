using EduPlatform.Application.Features.Lms.DTOs;

namespace EduPlatform.Application.Features.Lms.Interfaces;

public interface ICartService
{
    Task<CartSummaryDto> GetCartAsync(string userId, CancellationToken ct = default);
    Task AddToCartAsync(string userId, int courseId, CancellationToken ct = default);
    Task RemoveFromCartAsync(string userId, int courseId, CancellationToken ct = default);
    Task ClearCartAsync(string userId, CancellationToken ct = default);
}
