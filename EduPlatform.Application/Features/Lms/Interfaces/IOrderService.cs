using EduPlatform.Application.Features.Lms.DTOs;

namespace EduPlatform.Application.Features.Lms.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<OrderDto>> GetOrdersAsync(string userId, CancellationToken ct = default);
    Task<OrderDto> CheckoutAsync(string userId, CheckoutRequest request, CancellationToken ct = default);
}
