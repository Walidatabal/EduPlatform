using EduPlatform.Domain.Common;
using EduPlatform.Domain.Enums;

namespace EduPlatform.Domain.Entities;

public class Coupon : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public CouponDiscountType DiscountType { get; set; } = CouponDiscountType.Percentage;
    public decimal DiscountValue { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public int? MaxUses { get; set; }
    public int UsedCount { get; set; }
}
