using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;

namespace EduPlatform.Infrastructure.Services.Lms;

public class CouponService : ICouponService
{
    private readonly ILmsPlatformService _lms;

    public CouponService(ILmsPlatformService lms) => _lms = lms;

    public Task<CouponDto> CreateCouponAsync(UpsertCouponRequest request, CancellationToken ct = default) => _lms.CreateCouponAsync(request, ct);
    public Task<CouponValidationDto> ValidateCouponAsync(string code, decimal subtotal, CancellationToken ct = default) => _lms.ValidateCouponAsync(code, subtotal, ct);
}
