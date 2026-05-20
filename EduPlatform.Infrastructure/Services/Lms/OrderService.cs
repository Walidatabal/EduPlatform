using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;

namespace EduPlatform.Infrastructure.Services.Lms;

public class OrderService : IOrderService
{
    private readonly ILmsPlatformService _lms;

    public OrderService(ILmsPlatformService lms) => _lms = lms;

    public Task<IReadOnlyList<OrderDto>> GetOrdersAsync(string userId, CancellationToken ct = default) => _lms.GetOrdersAsync(userId, ct);
    public Task<OrderDto> CheckoutAsync(string userId, CheckoutRequest request, CancellationToken ct = default) => _lms.CheckoutAsync(userId, request, ct);
}
