using EduPlatform.Application.Features.Lms.DTOs;

namespace EduPlatform.Application.Features.Lms.Interfaces;

public interface ICouponService
{
    Task<CouponDto> CreateCouponAsync(UpsertCouponRequest request, CancellationToken ct = default);
    Task<CouponValidationDto> ValidateCouponAsync(string code, decimal subtotal, CancellationToken ct = default);
}
